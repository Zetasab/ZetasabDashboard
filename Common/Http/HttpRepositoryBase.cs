using System.Text;
using System.Text.Json;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Common.Http
{
    public class HttpRepositoryBase
    {
        protected readonly HttpClient _http;
        protected readonly JsonSerializerOptions _json;

        protected HttpRepositoryBase(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        protected async Task<(bool ok, T? data, string? error, string? raw)> TryGetAsync<T>(string url, CancellationToken ct = default)
        {
            using var res = await _http.GetAsync(url, ct);
            var rawContent = await res.Content.ReadAsStringAsync(ct);

            if (res.IsSuccessStatusCode)
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<T>(rawContent, _json);
                    return (true, payload, null, rawContent);
                }
                catch (Exception ex)
                {
                    // Deserialización fallida, devolvemos el raw
                    return (false, default, $"Deserialization error: {ex.Message}", rawContent);
                }
            }

            // No fue éxito → devolvemos raw como error
            return (false, default, $"HTTP {(int)res.StatusCode} {res.ReasonPhrase}", rawContent);
        }

        //protected async Task<(bool ok, T? data, string? error)> TryPostAsync<T>(string url, object body, CancellationToken ct = default)
        //{
        //    var content = new StringContent(JsonSerializer.Serialize(body, _json), Encoding.UTF8, "application/json");
        //    using var res = await _http.PostAsync(url, content, ct);
        //    if (res.IsSuccessStatusCode)
        //    {
        //        var payload = await res.Content.ReadFromJsonAsync<T>(_json, ct);
        //        return (true, payload, null);
        //    }
        //    return (false, default, await res.Content.ReadAsStringAsync(ct));
        //}
        public bool HasPermissions(UserModel user, EUserPermissionType type, List<string> page)
        {
            try
            {
                if (user.UserType == EUserType.SuperAdmin)
                {
                    return true;
                }
                foreach (var perm in user.Permissions)
                {
                    if (page.FirstOrDefault(x => x == perm.Code) != null)
                    {
                        if (perm.UserType >= type)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return false;
        }
    }
}
