
namespace ZetaDashboard.Common.Services
{
    public partial class HttpService
    {
        public MovieService Movies { get; }

        public HttpService(HttpClient http)
        {
            // All HTTP-based services share the same configured HttpClient
            Movies = new MovieService(http);
        }
    }
}
