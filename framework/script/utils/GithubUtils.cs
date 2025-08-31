using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

public class GithubUtils
{
	private const int PAGE_SIZE = 20;
	private static readonly string BASE_URL = "https://api.github.com/graphql";
	private static readonly System.Net.Http.HttpClient httpClient = new();
	private static readonly JsonSerializerOptions options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
	};
	private static readonly string REPOSITORY_ID = "R_kgDOPatLkw";
	private static readonly string GAME_DISCUSSION_CATEGORY_ID = "DIC_kwDOPatLk84CueXj";
	private static readonly string OWNER = "JustDooooIt";
	private static readonly string REPO = "ez-chess";
	private static readonly HashingContext hashingContext = new();
	private static DateTime TimeLine { get; set; } = DateTime.MinValue;

	static GithubUtils()
	{
		options.Converters.Add(new Vector2IConverter());
	}

	private static readonly string CREATE_DISCUSSION_QUERY = $@"
	mutation($repo: ID!, $title: String!, $body: String!, $cat: ID!) {{
	  createDiscussion(input: {{
			repositoryId: $repo
			title: $title,
			body: $body,
			categoryId: $cat
		}}) {{
			discussion {{
				id,
				number,
				title,
				body,
				createdAt,
			}}
		}}
  }}";
	private static readonly string ADD_COMMMENT_QUERY = $@"
  mutation($id: ID!, $body: String!) {{
		addDiscussionComment(input: {{
			discussionId: $id, body: $body
		}}) {{
			comment {{
				id
				body
				createdAt
					author {{
						login
			  }}
		  }}
	  }}
	}}";
	private static readonly string GET_COMMMENTS_QUERY = $@"
  query($number: Int!, $last: Int!, $before: String) {{
		repository(owner: ""{OWNER}"", name: ""{REPO}""){{
			discussion(number: $number) {{
				comments(last: $last, before: $before) {{
					pageInfo {{
						hasPreviousPage,
						hasNextPage,
						startCursor,
						endCursor
					}}
					nodes {{
						id,
						author{{
							login
						}},
					body,
					createdAt
					}}
				}}
			}}
		}}
  }}";
	private static readonly string GET_DISCUSSION_QUERY = $@"
  query($number: Int!) {{
		repository(owner: ""{OWNER}"", name: ""{REPO}""){{
			discussion(number: $number){{
				id,
				number,
				title,
				body,
				createdAt
			}}
		}}
  }}";
	private static readonly string DELETE_COMMENT_QUERY = $@"
  mutation($commentId: ID!){{
	deleteDiscussionComment(input: {{id: $commentId}}){{
	  comment {{
		id
		body
		createdAt
		author {{
		  login
		}}
	  }}
	}}
  }}";
	private static readonly string MARK_DISCUSSION_ROCKET_QUERY = $@"
  mutation AddReadyReaction($subjectId: ID!) {{
	addReaction(input: {{subjectId: $subjectId, content: ROCKET}}) {{
	  reaction {{
		content
	  }}
	  subject {{
		id
	  }}
	  }}
  }}";
	private static readonly string UPDATE_DISCUSSION_QUERY = $@"
	mutation($discussionId: ID!, $body: String){{
		updateDiscussion(input: {{
			discussionId: $discussionId,
			body: $body
		}}){{
			discussion{{
				id,
				number,
				title,
				body,
				createdAt
			}}
		}}
	}}";

	public static async Task<string> Login(string token)
	{
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ez-chess/v1.0.0");
		return await CheckLogin();
	}

	private static async Task<string> CheckLogin()
	{
		var query = new QueryBody() { Query = "query { viewer { id,login } }" };
		var res = await httpClient.PostAsJsonAsync(BASE_URL, query, options);
		if (res.IsSuccessStatusCode)
		{
			var data = await res.Content.ReadFromJsonAsync<JsonObject>(options);
			return data["data"]["viewer"]["login"].GetValue<string>();
		}
		else
		{
			return "";
		}
	}

	public static async Task<RoomMetaData> CreateRoom(string gameName)
	{
		var variables = new
		{
			repo = REPOSITORY_ID,
			title = $"{gameName}-{DateTime.Now}",
			body = JsonSerializer.Serialize(new { gameName, seats = Array.Empty<string>(), observers = Array.Empty<string>(), host = GameState.Instance.Username }),
			cat = GAME_DISCUSSION_CATEGORY_ID
		};
		var request = new { query = CREATE_DISCUSSION_QUERY, variables };
		var json = JsonSerializer.Serialize(request, options);
		var content = new StringContent(json);
		var res = await httpClient.PostAsync(BASE_URL, content);
		var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
		return result["data"]["createDiscussion"]["discussion"].Deserialize<RoomMetaData>(options);
	}

	public static async Task<RoomState> GetRoomState(int discussionNum)
	{
		var variables = new { number = discussionNum };
		var request = new { query = GET_DISCUSSION_QUERY, variables };
		var jsonObject = await DoPost(request);
		return JsonSerializer.Deserialize<RoomState>(jsonObject["data"]["repository"]["discussion"]["body"].GetValue<string>(), options);
	}

	public static async Task<Comments> GetComments(int number, string before = "")
	{
		var variables = new { number, last = PAGE_SIZE, before };
		var request = new { query = GET_COMMMENTS_QUERY, variables };
		var json = JsonSerializer.Serialize(request, options);
		var content = new StringContent(json);
		var res = await httpClient.PostAsync(BASE_URL, content);
		var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
		return result["data"]["repository"]["discussion"]["comments"].Deserialize<Comments>(options);
	}

	public static async Task ProcessComments(int discussionNum, Func<Comment, bool> commentProcessor)
	{
		string before = "";
		do
		{
			var comments = await GetComments(discussionNum, before);
			for (int i = comments.Nodes.Count - 1; i >= 0; i--)
			{
				if (commentProcessor.Invoke(comments.Nodes[i]))
				{
					return;
				}
			}
			if (!comments.PageInfo.HasPreviousPage)
				break;
			before = comments.PageInfo.StartCursor;
		} while (true);
	}

	public static async Task EnterRoom(string discussionId)
	{
		await AddComment(discussionId, "/enter");
	}

	public static async Task ChooseFaction(string discussionId, int faction)
	{
		await AddComment(discussionId, $"/choose/faction/{faction}");
	}

	public static async Task<JsonObject> GetDiscussion(int discussionNum)
	{
		var variables = new { number = discussionNum };
		var request = new { query = GET_DISCUSSION_QUERY, variables };
		return await DoPost(request);
	}

	public static async Task<Dictionary<string, UserData>> WaitForOthers(int discussionNum, int playerCount, string curPlayer)
	{
		int _playerCount = 0;
		HashSet<string> leavedPlayer = [];
		Dictionary<string, UserData> users = [];
		while (_playerCount < playerCount - 1)
		{
			_playerCount = 0;
			leavedPlayer.Clear();
			users.Clear();
			await ProcessComments(discussionNum, (comment) =>
			{
				JsonObject jsonObject;
				try
				{
					jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body, options);
				}
				catch (System.Exception)
				{
					return false;
				}
				if (jsonObject.ContainsKey("commentType") && jsonObject["commentType"].GetValue<int>() == (int)CommentType.PLAYER_STATE &&
						jsonObject.ContainsKey("playerType") && jsonObject["playerType"].GetValue<int>() == (int)UserType.PLAYER)
				{
					if (jsonObject.ContainsKey("state"))
					{
						if (jsonObject["state"].GetValue<int>() == (int)UserState.LEAVED)
						{
							leavedPlayer.Add(comment.Author.Login);
						}
						else if (jsonObject["state"].GetValue<int>() == (int)UserState.ENTERED && !leavedPlayer.Contains(comment.Author.Login) && comment.Author.Login != curPlayer)
						{
							_playerCount++;
							users[comment.Id] = JsonSerializer.Deserialize<UserData>(comment.Body);
						}
					}
				}
				return _playerCount >= playerCount - 1;
			});
			await Task.Delay(2500);
		}
		return users;
	}

	public static async Task<RoomMetaData> GetRoomInfo(int number)
	{
		var variables = new { number };
		var request = new { query = GET_DISCUSSION_QUERY, variables };
		var json = JsonSerializer.Serialize(request, options);
		var content = new StringContent(json);
		var res = await httpClient.PostAsync(BASE_URL, content);
		var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
		return result["data"]["repository"]["discussion"].Deserialize<RoomMetaData>(options);
	}

	public static async Task ExitRoom(string discussionId)
	{
		await AddComment(discussionId, "/exit");
	}

	public static async Task<Comment> AddComment(string id, string body)
	{
		var variables = new { id, body };
		var request = new { query = ADD_COMMMENT_QUERY, variables };
		var result = await DoPost(request);
		return result["data"]["addDiscussionComment"]["comment"].Deserialize<Comment>(options);
	}

	public static async Task<Comment> DeleteComment(string commentId)
	{
		var variables = new { commentId };
		var request = new { query = DELETE_COMMENT_QUERY, variables };
		var result = await DoPost(request);
		return result["data"]["deleteDiscussionComment"]["comment"].Deserialize<Comment>(options);
	}

	public static async Task<Discussion> UpdateDiscussion(string discussionId, string body)
	{
		var variables = new { discussionId, body };
		var request = new { query = UPDATE_DISCUSSION_QUERY, variables };
		var result = await DoPost(request);
		return result["data"]["updateDiscussion"]["discussion"].Deserialize<Discussion>(options);
	}

	public static async Task<RoomState> UpdateRoomState(string discussionId, RoomState roomState)
	{
		var discussion = await UpdateDiscussion(discussionId, JsonSerializer.Serialize(roomState, options));
		return JsonSerializer.Deserialize<RoomState>(discussion.Body, options);
	}

	// public static void Clean(string discussionId, RoomState roomState)
	// {
	// 	roomState.Seats[GameState.Instance.PlayerFaction] = "";
	// 	_ = UpdateRoomState(discussionId, roomState);
	// }

	public static async Task MarkDiscussion(string discussionId)
	{
		var variables = new { subjectId = discussionId };
		var request = new { query = MARK_DISCUSSION_ROCKET_QUERY, variables };
		_ = await DoPost(request);
	}

	private static async Task<JsonObject> DoPost(object request)
	{
		var json = JsonSerializer.Serialize(request, options);
		var content = new StringContent(json);
		var res = await httpClient.PostAsync(BASE_URL, content);
		return await res.Content.ReadFromJsonAsync<JsonObject>();
	}

	public static async Task<Comment> SubmitOperation<T>(string discussionId, PieceAdapter piece, T operation, string preStateHash) where T : Operation
	{
		int pieceType = piece.PieceType;
		string pieceName = piece.Name;
		int faction = piece.Faction;
		var gameData = new GameData<T>()
		{
			CommentType = CommentType.GAME_DATA,
			PieceType = pieceType,
			PieceName = pieceName,
			Faction = faction,
			Operation = operation,
			PreStateHash = preStateHash
		};
		string json = JsonSerializer.Serialize(gameData, options);
		var comment = await AddComment(discussionId, json);
		TimeLine = comment.CreatedAt;
		return comment;
	}

	public static async Task InviteOthers(string discussionId, string player)
	{
		await AddComment(discussionId, $"/invite/@{player}");
	}

	public static async Task ApplyOperation(int discussionNum, int faction, string curStateHash, Action<JsonObject> operationProcessor)
	{
		List<Comment> comments = [];
		await ProcessComments(discussionNum, (comment) =>
		{
			if (comment.CreatedAt <= TimeLine)
				return true;
			var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body);
			if (jsonObject.ContainsKey("commentType") && jsonObject["commentType"].GetValue<int>() == (int)CommentType.GAME_DATA &&
					jsonObject.ContainsKey("faction") && jsonObject["faction"].GetValue<int>() == faction)
			{
				comments.Add(comment);
			}
			return false;
		});
		if (comments.Count > 0)
		{
			TimeLine = comments.Max(e => e.CreatedAt);
			comments.Sort((e1, e2) => e1.CreatedAt.CompareTo(e2.CreatedAt));
		}
		foreach (var comment in comments)
		{
			var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body, options);
			if (jsonObject.ContainsKey("preStateHash") && jsonObject["preStateHash"].GetValue<string>() == curStateHash)
			{
				operationProcessor.Invoke(JsonSerializer.Deserialize<JsonObject>(comment.Body, options));
			}
		}
	}

	public static async Task<Comment> SaveGameData(string discussionId, int faction, List<PieceAdapter> pieces, Func<PieceAdapter, PieceData> pieceProcessor)
	{
		Environment env = new()
		{
			Faction = faction,
			CommentType = CommentType.ENVIRONMENT_DATA
		};
		List<PieceData> pieceDatas = [];
		foreach (var piece in pieces)
		{
			pieceDatas.Add(pieceProcessor.Invoke(piece));
		}
		env.PieceDatas = [.. pieceDatas];
		var json = JsonSerializer.Serialize(env);
		return await AddComment(discussionId, json);
	}

	public static string HashState(string state)
	{
		var err = hashingContext.Start(HashingContext.HashType.Sha256);
		if (err != Error.Ok)
		{
			return "";
		}
		var dataByte = state.ToUtf8Buffer();
		err = hashingContext.Update(dataByte);
		if (err != Error.Ok)
		{
			return "";
		}
		var hashByte = hashingContext.Finish();
		var hexString = hashByte.HexEncode();
		return hexString;
	}

	public static T Deserialize<T>(JsonObject jsonObject)
	{
		return jsonObject.Deserialize<T>(options);
	}

	public static string Serialize<T>(T dict)
	{
		return JsonSerializer.Serialize(dict, options);
	}

}

public record Discussion
{
	public string Id { get; set; }
	public int Number { get; set; }
	public string Body { get; set; }
	public string Title { get; set; }
	public DateTime CreatedAt { get; set; }
}

public class RoomState
{
	public string GameName { get; set; }
	public string[] Seats { get; set; }
	public string[] Placeholder { get; set; }
	public string[] Observers { get; set; }
}

public record RoomMetaData
{
	public string Id { get; set; }
	public int Number { get; set; }
	public string Body { get; set; }
	public string Title { get; set; }
	public DateTime CreatedAt { get; set; }
}

public record Environment : BaseData
{
	public int Faction { get; set; }
	public PieceData[] PieceDatas { get; set; }
}

public record PieceData
{
	public Vector2I Position { get; set; }
	public int Face { get; set; }
	public Dictionary<int, object> Data { get; set; } = [];
}

public record Operation
{
	public int Type { get; set; }
}

public record BaseData
{
	public CommentType CommentType { get; set; }
}

public record UserData : BaseData
{
	public int Faction { get; set; }
	public UserState PlayerState { get; set; }
	public UserType PlayerType { get; set; }
}

public record GameData<T> : BaseData where T : Operation
{
	public string PieceName { get; set; }
	public int PieceType { get; set; }
	public int OperationType { get; set; }
	public T Operation { get; set; }
	public int Faction { get; set; }
	public string PreStateHash { get; set; }
}

public record QueryBody
{
	public string Query { get; set; }
	public Dictionary<string, object> Variables { get; set; }
}

public record Result<T>
{
	public T Data { get; set; }
}

public record Comments
{
	public PageInfo PageInfo { get; set; }
	public List<Comment> Nodes { get; set; }
}

public record Comment
{
	public string Id { get; set; }
	public Author Author { get; set; }
	public string Body { get; set; }
	public DateTime CreatedAt { get; set; }
}

public record Author
{
	public string Login { get; set; }
}

public record PageInfo
{
	public bool HasPreviousPage { get; set; }
	public bool HasNextPage { get; set; }
	public string StartCursor { get; set; }
	public string EndCursor { get; set; }
}

public enum CommentType
{
	PLAYER_STATE, GAMME_READY, GAME_DATA, ENVIRONMENT_DATA
}

public enum UserState
{
	ENTERED, LEAVED
}

public enum UserType
{
	PLAYER, OBSERVER
}
