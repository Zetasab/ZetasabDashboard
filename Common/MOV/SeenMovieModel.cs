namespace ZetaDashboard.Common.MOV
{
    public class SeenMovieModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public List<MovieModel> Movies { get; set; }

        public SeenMovieModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
