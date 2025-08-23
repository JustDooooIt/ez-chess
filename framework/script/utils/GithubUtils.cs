using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class GithubUtils
{
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

  public static async Task ProcessComments(int discussionNumber, Action<CommentNode> commentProcessor)
  {
    string before = "";
    do
    {
      var comments = await GetComments(discussionNumber, before);
      foreach (var comment in comments.Nodes)
      {
        commentProcessor.Invoke(comment);
      }
      if (!comments.PageInfo.HasPreviousPage)
        break;
      before = comments.PageInfo.StartCursor;
    } while (true);
  }

  public static async Task<bool> GameReady(string discussionId, string username, int faction)
  {
    var pub = Godot.FileAccess.Open($"user://{username}.pub", Godot.FileAccess.ModeFlags.Read);
    var body = $"{{ \"commentType\": 0, \"username\": \"{username}\", \"faction\": {faction}, \"pub\": \"{pub.GetAsText()}\" }}";
    return await AddComment(discussionId, body);
  }

  public static async Task<bool> SubmitOperation(string discussionId, Operation operation)
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

  public static async Task<bool> AddComment(string id, string body)
  {
    var variables = new { id, body };
    var request = new { query = ADD_COMMMENT_QUERY, variables };
    var json = JsonSerializer.Serialize(request);
    var content = new StringContent(json);
    var res = await httpClient.PostAsync(BASE_URL, content);
    return res.IsSuccessStatusCode;
  }

  public class RoomMetaData
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

  public class Operation
  {
    [JsonPropertyName("commentType")]
    public CommentType CommentType { get; set; }
    [JsonPropertyName("operationType")]
    public int OperationType { get; set; }
  }

  public class UserData
  {
    [JsonPropertyName("commentType")]
    public CommentType CommentType { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("faction")]
    public int Faction { get; set; }
    [JsonPropertyName("pub")]
    public string Pub { get; set; }
  }

  public class QueryBody
  {
    public string Query { get; set; }
    public Dictionary<string, object> Variables { get; set; }
  }

  public class Result<T>
  {
    public T Data { get; set; }
  }

  public class Comments
  {
    [JsonPropertyName("pageInfo")]
    public PageInfo PageInfo { get; set; }
    [JsonPropertyName("nodes")]
    public List<CommentNode> Nodes { get; set; }
  }

  public class CommentNode
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

  public class Author
  {
    [JsonPropertyName("login")]
    public string Login { get; set; }
  }

  public class PageInfo
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
    READY
  }
}
