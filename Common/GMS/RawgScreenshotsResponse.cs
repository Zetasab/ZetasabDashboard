using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.GMS
{
    public sealed class RawgScreenshotsResponse
    {
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("next")] public string? Next { get; set; }
        [JsonPropertyName("previous")] public string? Previous { get; set; }
        [JsonPropertyName("results")] public List<RawgScreenshot> Results { get; set; } = new();
    }

    public sealed class RawgScreenshot
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("image")] public string? Image { get; set; }
        [JsonPropertyName("width")] public int? Width { get; set; }
        [JsonPropertyName("height")] public int? Height { get; set; }
        [JsonPropertyName("is_deleted")] public bool? IsDeleted { get; set; }

        // Optional helpers
        [JsonIgnore] public double? AspectRatio => (Width > 0 && Height > 0) ? (double)Width! / Height! : null;
        [JsonIgnore] public bool IsLandscape => (Width ?? 0) >= (Height ?? 0);
    }
}
