using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.MOV
{
    public class MovieListModel
    {

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<MovieModel> Results { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

    }
}
