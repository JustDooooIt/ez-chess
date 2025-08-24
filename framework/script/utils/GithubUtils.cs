using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public class GithubUtils
{
  private const int USER_DATA = 0;
  private const int PAGE_SIZE = 10;
  private static readonly string BASE_URL = "https://api.github.com/graphql";
  private static System.Net.Http.HttpClient httpClient = new();
  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
  };
  private static readonly string REPOSITORY_ID = "R_kgDOPatLkw";
  private static readonly string GAME_DISCUSSION_CATEGORY_ID = "DIC_kwDOPatLk84CueXj";
  private static readonly string OWNER = "JustDooooIt";
  private static readonly string REPO = "ez-chess";

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

  public static async Task<(string, string)> Login(string token)
  {
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ez-chess/v1.0.0");
    return await CheckLogin();
  }

  private static async Task<(string, string)> CheckLogin()
  {
    var query = new QueryBody() { Query = "query { viewer { id,login } }" };
    var res = await httpClient.PostAsJsonAsync(BASE_URL, query, options);
    if (res.IsSuccessStatusCode)
    {
      var data = await res.Content.ReadFromJsonAsync<JsonObject>(options);
      return (data["data"]["viewer"]["id"].GetValue<string>(), data["data"]["viewer"]["login"].GetValue<string>());
    }
    else
    {
      return ("", "");
    }
  }

  public static async Task<RoomMetaData> CreateGameRoom(string gameName)
  {
    var variables = new { repo = REPOSITORY_ID, title = $"{gameName}-{DateTime.Now}", body = gameName, cat = GAME_DISCUSSION_CATEGORY_ID };
    var request = new { query = CREATE_DISCUSSION_QUERY, variables };
    var json = JsonSerializer.Serialize(request, options);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
    return result["data"]["createDiscussion"]["discussion"].Deserialize<RoomMetaData>(options);
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

  public static async Task ProcessComments(int discussionNumber, Func<CommentNode, bool> commentProcessor)
  {
    string before = "";
    do
    {
      var comments = await GetComments(discussionNumber, before);
      foreach (var comment in comments.Nodes)
      {
        if (commentProcessor.Invoke(comment))
        {
          return;
        }
      }
      if (!comments.PageInfo.HasPreviousPage)
        break;
      before = comments.PageInfo.StartCursor;
    } while (true);
  }

  public static async Task<UserData> EnterRoom(string discussionId, int faction, PlayerType playerType)
  {
    var body = new { commentType = 0, faction, state = (int)PlayerState.ENTERED, playerType };
    var comment = await AddComment(discussionId, JsonSerializer.Serialize(body));
    return JsonSerializer.Deserialize<UserData>(comment.Body);
  }

  public static async Task<Dictionary<string, UserData>> WaitForOthers(int discussionNum, int playerCount, string curPlayer)
  {
    int _playerCount = 0;
    HashSet<string> leavedPlayer = [];
    Dictionary<string, UserData> users = [];
    while (_playerCount < playerCount - 1)
    {
      _playerCount = 0;
      users.Clear();
      await ProcessComments(discussionNum, (comment) =>
      {
        var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body, options);
        if (jsonObject.ContainsKey("commentType") && jsonObject["commentType"].GetValue<int>() == (int)CommentType.PLAYER_STATE &&
      jsonObject.ContainsKey("playerType") && jsonObject["playerType"].GetValue<int>() == (int)PlayerType.PLAYER)
        {
          if (jsonObject.ContainsKey("state"))
          {
            if (jsonObject["state"].GetValue<int>() == (int)PlayerState.LEAVED)
            {
              leavedPlayer.Add(comment.Id);
            }
            else if (jsonObject["state"].GetValue<int>() == (int)PlayerState.ENTERED && !leavedPlayer.Contains(comment.Id) && comment.Author.Login != curPlayer)
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

  // public static async Task<Dictionary<string, UserData>> WaitOthers(int discussionNumber, int playerCount, CancellationToken cancelToken = default)
  // {
  //   while (true)
  //   {
  //     string before = "";
  //     int _playerCount = 0;
  //     HashSet<string> names = [];
  //     Dictionary<string, UserData> users = [];
  //     cancelToken.ThrowIfCancellationRequested();
  //     do
  //     {
  //       var comments = await GetComments(discussionNumber, before);
  //       foreach (var comment in comments.Nodes)
  //       {
  //         if (!names.Contains(comment.Author.Login))
  //         {
  //           names.Add(comment.Author.Login);
  //           var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body.Replace("\n", "\\n").Replace("\r", "\\r"), options);
  //           if (jsonObject["commentType"].GetValue<int>() == (int)CommentType.GAMME_READY)
  //           {
  //             if (_playerCount++ < playerCount)
  //             {
  //               users[comment.Author.Login] = jsonObject.Deserialize<UserData>(options);
  //               if (_playerCount >= playerCount)
  //                 return users;
  //             }
  //           }
  //         }
  //       }
  //       if (!comments.PageInfo.HasPreviousPage)
  //         break;
  //       before = comments.PageInfo.StartCursor;
  //     } while (true);
  //     await Task.Delay(1000, CancellationToken.None);
  //   }
  // }

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

  public static async Task<UserData> LeaveRoom(string discussionId, int faction, PlayerType playerType)
  {
    var body = new { commentType = 0, faction, state = (int)PlayerState.LEAVED, playerType };
    var comment = await AddComment(discussionId, JsonSerializer.Serialize(body));
    return JsonSerializer.Deserialize<UserData>(comment.Body);
  }

  public static async Task<CommentNode> AddComment(string id, string body)
  {
    var variables = new { id, body };
    var request = new { query = ADD_COMMMENT_QUERY, variables };
    var result = await DoPost(request);
    return result["data"]["addDiscussionComment"]["comment"].Deserialize<CommentNode>(options);
  }

  public static async Task<CommentNode> DeleteComment(string commentId)
  {
    var variables = new { commentId };
    var request = new { query = DELETE_COMMENT_QUERY, variables };
    var result = await DoPost(request);
    return result["data"]["deleteDiscussionComment"]["comment"].Deserialize<CommentNode>();
  }

  private static async Task<JsonObject> DoPost(object request)
  {
    var json = JsonSerializer.Serialize(request, options);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    return await res.Content.ReadFromJsonAsync<JsonObject>();
  }

  public static async Task<CommentNode> SubmitOperation<T>(PieceAdapter piece, string discussionId, T operation) where T : Operation
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
      Operation = operation
    };
    string json = JsonSerializer.Serialize(gameData, options);
    return await AddComment(discussionId, json);
  }

  public static async Task<CommentNode> SaveGameData(string discussionId, int faction, List<PieceAdapter> pieces, Func<PieceAdapter, PieceData> pieceProcessor)
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
}


public record RoomMetaData
{
  public string Id { get; set; }
  public int Number { get; set; }
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
  public string Pub { get; set; }
}

public record GameData<T> : BaseData where T : Operation
{
  public string PieceName { get; set; }
  public int PieceType { get; set; }
  public int OperationType { get; set; }
  public T Operation { get; set; }
  public int Faction { get; set; }
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
  public List<CommentNode> Nodes { get; set; }
}

public record CommentNode
{
  public string Id { get; set; }
  public Author Author { get; set; }
  public string Body { get; set; }
  public DateTime CreatedAt { get; set; }
}

public record Author
{
  // public string Id { get; set; }
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

public enum PlayerState
{
  ENTERED, LEAVED
}

public enum PlayerType
{
  PLAYER, OBSERVER
}
