using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.MOV
{
    public class MovieVideosModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("results")]
        public List<VideoResult> Results { get; set; } = new();
    }
    public class VideoResult
    {
        [JsonPropertyName("iso_639_1")]
        public string? Iso6391 { get; set; }

        [JsonPropertyName("iso_3166_1")]
        public string? Iso31661 { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("site")]
        public string? Site { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("official")]
        public bool Official { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
