using System.Globalization;
using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.GMS
{
    public sealed class RawgAchievementsResponse
    {
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("next")] public string? Next { get; set; }
        [JsonPropertyName("previous")] public string? Previous { get; set; }
        [JsonPropertyName("results")] public List<RawgAchievement> Results { get; set; } = new();
    }

    public sealed class RawgAchievement
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("image")] public string? Image { get; set; }

        // RAWG returns percent as string (e.g., "0.60")
        [JsonPropertyName("percent")] public string? Percent { get; set; }

        // Convenience helpers (not serialized)
        [JsonIgnore]
        public double? PercentValue =>
            double.TryParse(Percent, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;

        [JsonIgnore]
        public string PercentDisplay =>
            PercentValue is double p ? $"{p:0.##}%" : "-";
    }
}
