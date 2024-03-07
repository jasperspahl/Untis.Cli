using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Untis.Client.Contracts;
using Untis.Client.Contracts.Enum;
using Untis.Client.JsonRpc;
using InvalidOperationException = System.InvalidOperationException;
using NotImplementedException = System.NotImplementedException;

namespace Untis.Client;

public abstract class UntisClientBase: IUntisClient
{
    protected readonly HttpClient Client;
    protected string School;
    protected readonly string Id;
    protected SessionInformation? SessionInformation;
    
    protected UntisClientBase(string school, string baseurl, string identity)
    {
        School = school;
        Id = identity;
        Client = new HttpClient
        {
            BaseAddress = new Uri(baseurl)
        };
        Client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        Client.DefaultRequestHeaders.Add("Pragma", "no-cache");
        Client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
    }
    
    private JsonContent CreateRpcContent(string method, object parameters)
    {
        return new JsonRpcRequest<object>(method, parameters, Id).ToJsonContent();
    }

    protected async Task<T?> PostAsync<T>(string method, object parameters, bool validateSession = true,
        string url = "/WebUntis/jsonrpc.do")
    {
        if (validateSession && !await IsLoggedInAsync())
        {
            throw new InvalidOperationException("Not logged in");
        }

        var uri =
            $"{url}?school={School}";
        var response = await Client.PostAsync(
            uri,
            CreateRpcContent(method, parameters)
        );
        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var result = JsonSerializer.Deserialize<JsonRpcResult<T>>(content);
            if (result != null) return result.Result;
            throw new InvalidOperationException("Invalid response");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            var error = JsonSerializer.Deserialize<JsonRpcError>(content);
            if (error != null) throw new UntisException(error);
            throw;
        }
    }

    public abstract Task<bool> LoginAsync();

    public async Task<bool> LogoutAsync()
    {
        await Client.PostAsync(
            $"/WebUntis/jsonrpc.do?school={School}",
            CreateRpcContent("logout", new { })
        );
        SessionInformation = null;
        return true;
    }

    public async Task<bool> IsLoggedInAsync()
    {
        if (SessionInformation == null) return false;
        var response = await PostAsync<object>("getLatestImportTime", new { }, false);
        return response != null;
    }

    public Task<IEnumerable<Lesson>> GetOwnTimetableForTodayAsync()
    {
        return GetOwnTimetableAsync(DateTime.Today, DateTime.Today);
    }

    public Task<IEnumerable<Lesson>> GetOwnTimetableForThisWeekAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Lesson>> GetOwnTimetableFor(DateTime date)
    {
        return GetOwnTimetableAsync(date, date);
    }

    public Task<IEnumerable<Lesson>> GetOwnTimetableAsync(DateTime? start, DateTime? end)
    {
        if (SessionInformation == null) throw new InvalidOperationException("Not logged in");
        if (SessionInformation.PersonId == null) throw new InvalidOperationException("No person id");
        return GetTimetableAsync((int)SessionInformation.PersonId, (UntisElementType)SessionInformation!.PersonType!, start, end);
    }

    private class TimetableRequestOptionsElement
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
    }

    private class TimetableRequestOptions
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("element")]
        public required TimetableRequestOptionsElement Element { get; set; }
        [JsonPropertyName("showLsText")]
        public bool ShowLsText { get; set; }
        [JsonPropertyName("showStudentgroup")]
        public bool ShowStudentgroup { get; set; }
        [JsonPropertyName("showLsNumber")]
        public bool ShowLsNumber { get; set; }
        [JsonPropertyName("showSubstText")]
        public bool ShowSubstText { get; set; }
        [JsonPropertyName("showInfo")]
        public bool ShowInfo { get; set; }
        [JsonPropertyName("showBooking")]
        public bool ShowBooking { get; set; }
        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }
        
        [JsonPropertyName("klasseFields")]
        public required List<string> KlasseFields { get; set; }
        [JsonPropertyName("roomFields")]
        public required List<string> RoomFields { get; set; }
        [JsonPropertyName("subjectFields")]
        public required List<string> SubjectFields { get; set;}
        [JsonPropertyName("teacherFields")]
        public required List<string> TeacherFields { get; set; }
    }

    public async Task<IEnumerable<Lesson>> GetTimetableAsync(int id, UntisElementType type, DateTime? start, DateTime? end)
    {
        var options = new TimetableRequestOptions()
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Element = new TimetableRequestOptionsElement()
            {
                Id = id,
                Type = (int) type
            },
            ShowLsText = true,
            ShowStudentgroup = true,
            ShowLsNumber = true,
            ShowSubstText = true,
            ShowInfo = true,
            ShowBooking = true,
            KlasseFields = new List<string> {"id", "name", "longname", "externalkey"},
            RoomFields = new List<string> {"id", "name", "longname", "externalkey"},
            SubjectFields = new List<string> {"id", "name", "longname", "externalkey"},
            TeacherFields = new List<string> {"id", "name", "longname", "externalkey"}
        };
        if (start is not null) options.StartDate = start.Value.ToString("yyyyMMdd");
        if (end is not null) options.EndDate = end.Value.ToString("yyyyMMdd");
        return await PostAsync<List<Lesson>>("getTimetable", new
        {
            options
        }, false) ?? new List<Lesson>();
    }

    public void Dispose()
    {
        LogoutAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        Client.Dispose();
    }
}