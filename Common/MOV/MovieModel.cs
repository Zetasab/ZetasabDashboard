using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.MOV
{
    public class MovieModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }


        public string GetPosterUrl(string size = "w500") =>
        string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/{size}{PosterPath}";

        public string GetBackdropUrl(string size = "w780") =>
            string.IsNullOrEmpty(BackdropPath) ? "" : $"https://image.tmdb.org/t/p/{size}{BackdropPath}";

    }
}
