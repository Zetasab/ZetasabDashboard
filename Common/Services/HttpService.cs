
namespace ZetaDashboard.Common.Services
{
    public partial class HttpService
    {
        public MovieService Movies { get; }
        public GameService Games { get; }

        public HttpService(IHttpClientFactory factory)
        {
            var tmdbClient = factory.CreateClient("tmdb");
            var rawgClient = factory.CreateClient("rawg");

            Movies = new MovieService(tmdbClient);
            Games = new GameService(rawgClient);
        }
    }
}
