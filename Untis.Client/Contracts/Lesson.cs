using System.Text.Json.Serialization;

namespace Untis.Client.Contracts;

public class Lesson
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("date")]
    public int Date { get; set; }
    [JsonPropertyName("startTime")]
    public int StartTime { get; set; }
    [JsonPropertyName("endTime")]
    public int EndTime { get; set; }
    [JsonPropertyName("kl")]
    public required ShortData[] Kl { get; set; }
    [JsonPropertyName("te")]
    public required ShortData[] Te { get; set; }
    [JsonPropertyName("su")]
    public required ShortData[] Su { get; set; }
    [JsonPropertyName("ro")]
    public required ShortData[] Ro { get; set; }
    [JsonPropertyName("lstext")]
    public string? LsText { get; set; }
    [JsonPropertyName("lsnumber")]
    public int LsNumber { get; set; }
    [JsonPropertyName("activityType")]
    public string? ActivityType { get; set; } = "Unterricht";
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("info")]
    public string? Info { get; set; }
    [JsonPropertyName("substText")]
    public string? SubstText { get; set; }
    [JsonPropertyName("statflags")]
    public string? StatFlags { get; set; }
    [JsonPropertyName("sg")]
    public string? Sg { get; set; }
    [JsonPropertyName("bkRemark")]
    public string? BkRemark { get; set; }
    [JsonPropertyName("brText")]
    public string? BrText { get; set; }
}