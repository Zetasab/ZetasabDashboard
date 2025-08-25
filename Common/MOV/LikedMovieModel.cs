namespace ZetaDashboard.Common.MOV
{
    public class LikedMovieModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public List<MovieModel> Movies { get; set; } = new List<MovieModel>();

        public LikedMovieModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
