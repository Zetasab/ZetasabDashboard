using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using ZetaDashboard.Common.Http.Config;

namespace ZetaDashboard.Handlers
{
    public sealed class RawgApiKeyHandler : DelegatingHandler
    {
        private readonly string _config;

        public RawgApiKeyHandler(string config)
        {
            _config = config;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri is not null && !string.IsNullOrWhiteSpace(_config))
            {
                var uriBuilder = new UriBuilder(request.RequestUri);

                // Parsear la query existente
                var parsed = QueryHelpers.ParseQuery(uriBuilder.Query);

                // Convertir a Dictionary<string,string>
                var dict = parsed.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToString(), // 👈 aquí pasamos de StringValues a string
                    StringComparer.OrdinalIgnoreCase
                );

                // Añadir la API key si no está
                if (!dict.ContainsKey("key"))
                    dict["key"] = _config;

                // Reconstruir la querystring
                uriBuilder.Query = string.Join("&", dict.Select(kvp =>
                    $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                request.RequestUri = uriBuilder.Uri;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
