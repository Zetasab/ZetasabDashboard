using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.GMS
{
    public sealed class RawgVideosResponse
    {
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("next")] public string? Next { get; set; }
        [JsonPropertyName("previous")] public string? Previous { get; set; }
        [JsonPropertyName("results")] public List<RawgVideo> Results { get; set; } = new();
    }

    public sealed class RawgVideo
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("preview")] public string? Preview { get; set; }
        [JsonPropertyName("data")] public RawgVideoFiles? Data { get; set; }
    }

    public sealed class RawgVideoFiles
    {
        // "480" is not a valid C# identifier; map it to P480
        [JsonPropertyName("480")] public string? P480 { get; set; }
        [JsonPropertyName("max")] public string? Max { get; set; }

        public string? GetUrl(string quality) => quality switch
        {
            "480" => P480,
            "max" => Max,
            _ => null
        };
    }
}
