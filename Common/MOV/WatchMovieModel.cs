namespace ZetaDashboard.Common.MOV
{
    public class WatchMovieModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public List<MovieModel> Movies { get; set; } = new List<MovieModel>();

        public WatchMovieModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
