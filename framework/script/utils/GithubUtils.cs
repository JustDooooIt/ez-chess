using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class GithubUtils
{
  private const int USER_DATA = 0;
  private const int PAGE_SIZE = 10;
  private static readonly string BASE_URL = "https://api.github.com/graphql";
  private static System.Net.Http.HttpClient httpClient = new();
  private static readonly JsonSerializerOptions options = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
  };
  private static readonly string REPOSITORY_ID = "R_kgDOPatLkw";
  private static readonly string GAME_DISCUSSION_CATEGORY_ID = "DIC_kwDOPatLk84CueXj";

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
    repository(owner: ""JustDooooIt"", name: ""ez-chess""){{
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
    repository(owner: ""JustDooooIt"", name: ""ez-chess""){{
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

  public static async Task<string> Login(string token)
  {
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ez-chess/v1.0.0");
    return await CheckLogin();
  }

  private static async Task<string> CheckLogin()
  {
    var query = new QueryBody() { Query = "query { viewer { login }}" };
    var res = await httpClient.PostAsJsonAsync(BASE_URL, query, options);
    if (res.IsSuccessStatusCode)
    {
      var data = await res.Content.ReadFromJsonAsync<JsonObject>();
      return (string)data["data"]["viewer"]["login"];
    }
    else
    {
      return "";
    }
  }

  public static async Task<RoomMetaData> CreateGameRoom(string gameName)
  {
    var variables = new { repo = REPOSITORY_ID, title = $"{gameName}-{DateTime.Now}", body = gameName, cat = GAME_DISCUSSION_CATEGORY_ID };
    var request = new { query = CREATE_DISCUSSION_QUERY, variables };
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
    return result["data"]["createDiscussion"]["discussion"].Deserialize<RoomMetaData>();
  }

  public static async Task<Comments> GetComments(int number, string before = "")
  {
    var variables = new { number, last = PAGE_SIZE, before };
    var request = new { query = GET_COMMMENTS_QUERY, variables };
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
    return result["data"]["repository"]["discussion"]["comments"].Deserialize<Comments>();
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

  public static async Task<CommentNode> GameReady(string discussionId, int discussionNum, string username, int faction)
  {
    bool isExist = false;
    CommentNode handshake = null;
    await ProcessComments(discussionNum, (comment) =>
    {
      var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body.Replace("\n", "\\n").Replace("\r", "\\r"));
      isExist = jsonObject["commentType"].GetValue<int>() == USER_DATA && jsonObject["username"].GetValue<string>() == username;
      if (isExist)
        handshake = comment;
      return isExist;
    });
    if (isExist)
      return handshake;
    var pub = Godot.FileAccess.Open($"user://{username}.pub", Godot.FileAccess.ModeFlags.Read);
    var body = $"{{ \"commentType\": 0, \"username\": \"{username}\", \"faction\": {faction}, \"pub\": \"{pub.GetAsText()}\" }}";
    return await AddComment(discussionId, body);
  }

  public static async Task<CommentNode> SubmitOperation(string discussionId, Operation operation)
  {
    string body = JsonSerializer.Serialize(operation);
    // CryptoUtils.EncryptFor(body, );
    return await AddComment(discussionId, body);
  }

  public static async Task<Dictionary<string, UserData>> WaitOthers(int discussionNumber, int playerCount, CancellationToken cancelToken = default)
  {
    while (true)
    {
      string before = "";
      int _playerCount = 0;
      HashSet<string> names = [];
      Dictionary<string, UserData> users = [];
      cancelToken.ThrowIfCancellationRequested();
      await Task.Delay(1000, CancellationToken.None);
      do
      {
        var comments = await GetComments(discussionNumber, before);
        foreach (var comment in comments.Nodes)
        {
          if (!names.Contains(comment.Author.Login))
          {
            names.Add(comment.Author.Login);
            var jsonObject = JsonSerializer.Deserialize<JsonObject>(comment.Body.Replace("\n", "\\n").Replace("\r", "\\r"));
            if (jsonObject["commentType"].GetValue<int>() == 0)
            {
              if (_playerCount++ < playerCount)
              {
                users[comment.Author.Login] = jsonObject.Deserialize<UserData>();
                if (_playerCount >= playerCount)
                  return users;
              }
            }
          }
        }
        if (!comments.PageInfo.HasPreviousPage)
          break;
        before = comments.PageInfo.StartCursor;
      } while (true);
    }
  }

  public static async Task<RoomMetaData> EnterRoom(int number)
  {
    var variables = new { number };
    var request = new { query = GET_DISCUSSION_QUERY, variables };
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    var result = await res.Content.ReadFromJsonAsync<JsonObject>(options);
    return result["data"]["repository"]["discussion"].Deserialize<RoomMetaData>();
  }

  public static async Task<CommentNode> AddComment(string id, string body)
  {
    var variables = new { id, body };
    var request = new { query = ADD_COMMMENT_QUERY, variables };
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    var result = await res.Content.ReadFromJsonAsync<JsonObject>();
    return result["data"]["addDiscussionComment"]["comment"].Deserialize<CommentNode>();
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
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    return await res.Content.ReadFromJsonAsync<JsonObject>();
  }

  public static async Task<CommentNode> PostOperation<T>(PieceAdapter piece, string discussionId, T operation) where T : Operation
  {
    int pieceType = piece.PieceType;
    string pieceName = piece.Name;
    int faction = piece.Faction;
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    var gameData = new GameData<T>()
    {
      PieceType = pieceType,
      PieceName = pieceName,
      Timestamp = timestamp,
      Faction = faction,
      Operation = operation
    };
    string json = JsonSerializer.Serialize(gameData);
    return await AddComment(discussionId, json);
  }
}


public record RoomMetaData
{
  [JsonPropertyName("id")]
  public string Id { get; set; }
  [JsonPropertyName("number")]
  public int Number { get; set; }
  [JsonPropertyName("title")]
  public string Title { get; set; }
  [JsonPropertyName("createdAt")]
  public DateTime CreatedAt { get; set; }
}

public record Operation
{
  [JsonPropertyName("type")]
  public int Type { get; set; }
}

public record BaseData
{
  [JsonPropertyName("commentType")]
  public CommentType CommentType { get; set; }
}

public record UserData : BaseData
{
  [JsonPropertyName("username")]
  public string Username { get; set; }
  [JsonPropertyName("faction")]
  public int Faction { get; set; }
  [JsonPropertyName("pub")]
  public string Pub { get; set; }
}

public record GameData<T> : BaseData where T : Operation
{
  [JsonPropertyName("pieceName")]
  public string PieceName { get; set; }
  [JsonPropertyName("pieceType")]
  public int PieceType { get; set; }
  [JsonPropertyName("operation")]
  public T Operation { get; set; }
  [JsonPropertyName("faction")]
  public int Faction { get; set; }
  [JsonPropertyName("timestamp")]
  public long Timestamp { get; set; }
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
  [JsonPropertyName("pageInfo")]
  public PageInfo PageInfo { get; set; }
  [JsonPropertyName("nodes")]
  public List<CommentNode> Nodes { get; set; }
}

public record CommentNode
{
  [JsonPropertyName("id")]
  public string Id { get; set; }
  [JsonPropertyName("author")]
  public Author Author { get; set; }
  [JsonPropertyName("body")]
  public string Body { get; set; }
  [JsonPropertyName("createdAt")]
  public DateTime CreatedAt { get; set; }
}

public record Author
{
  [JsonPropertyName("login")]
  public string Login { get; set; }
}

public record PageInfo
{
  [JsonPropertyName("hasPreviousPage")]
  public bool HasPreviousPage { get; set; }
  [JsonPropertyName("hasNextPage")]
  public bool HasNextPage { get; set; }
  [JsonPropertyName("startCursor")]
  public string StartCursor { get; set; }
  [JsonPropertyName("endCursor")]
  public string EndCursor { get; set; }
}

public enum CommentType
{
  READY, GAME
}