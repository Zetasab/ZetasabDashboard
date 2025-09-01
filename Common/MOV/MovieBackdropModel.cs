using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.MOV
{
    public class MovieBackdropModel
    {
        [JsonPropertyName("backdrops")]
        public List<Backdrop> Backdrops { get; set; } = new();
    }
    public class Backdrop
    {
        [JsonPropertyName("aspect_ratio")]
        public double AspectRatio { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string? Iso639_1 { get; set; }

        [JsonPropertyName("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        public string GetPosterUrl(string size = "w500") =>
        string.IsNullOrEmpty(FilePath) ? "" : $"https://image.tmdb.org/t/p/{size}{FilePath}";
    }
}
