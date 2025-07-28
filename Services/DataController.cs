using MudBlazor;
using System.Text.Json;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;

namespace ZetaDashboard.Services
{
    public class DataController
    {
        private ISnackbar Snackbar { get; set; }
        private BaseService ApiService { get; set; }
        public DataController(ISnackbar snackbar, BaseService apiService) 
        {
            Snackbar = snackbar;
            ApiService = apiService;
        }

        #region Get
        public async Task<T> GetData<T>(ApiResponse<T> response) 
        {
            if (response.Result == ResponseStatus.Ok)
            {
                return response.Data;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                return default;
            }
        }
        public async Task<List<T>> GetData<T>(ApiResponse<List<T>> response)
        {
            if (response.Result == ResponseStatus.Ok)
            {
                return response.Data;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                return default;
            }
        }
        #endregion

        #region Post
        public async Task<T?> LoginAsync<T>(ApiResponse<T> response)
        {
            if (response.Result == ResponseStatus.Ok)
            {
                return response.Data;
            }
            else 
            {
                Snackbar.Add(response.Message, Severity.Error);
                return default;
            }
        }

        public async Task<bool> InsertData<T>(ApiResponse<T> response,UserModel user,string where = "InsertingData",string description = "")
        {
            bool returnn = false;
            var audit = new AuditModel(
                user.Id,
                user.Name, 
                AuditWhat.Post,
                where,
                description,
                response.Result,
                response.Message);
            if (response.Result == ResponseStatus.Ok)
            {
                Snackbar.Add(response.Message, Severity.Success);
                returnn = true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                returnn = false;
            }
            _ = await InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
            return returnn;
        }

        public async Task<bool> InsertAudit<T>(ApiResponse<T> response)
        {
            if (response.Result == ResponseStatus.Ok)
            {
                return true;
            }
            else
            {
                Console.Write(response.Message);
                return false;
            }
        }
        #endregion

        #region Update
        public async Task<bool> UpdateData<T>(ApiResponse<T> response, UserModel user, string where = "UpdateData", string description = "")
        {
            bool returnn = false;
            var audit = new AuditModel(
                user.Id,
                user.Name,
                AuditWhat.Put,
                where,
                description,
                response.Result,
                response.Message);
            if (response.Result == ResponseStatus.Ok)
            {
                Snackbar.Add(response.Message, Severity.Success);
                returnn = true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                returnn = false;
            }
            _ = await InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
            return returnn;
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteData<T>(ApiResponse<T> response, UserModel user, string where = "DeleteData", string description = "")
        {
            bool returnn = false;
            var audit = new AuditModel(
                user.Id,
                user.Name,
                AuditWhat.Delete,
                where,
                description,
                response.Result,
                response.Message);
            if (response.Result == ResponseStatus.Ok)
            {
                Snackbar.Add(response.Message, Severity.Success);
                returnn = true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                returnn = false;
            }
            _ = await InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
            return returnn;
        }
        #endregion
        #region format
        public async Task<T> DeepCoopy<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var json = JsonSerializer.Serialize(entity);
            var copy = JsonSerializer.Deserialize<T>(json);

            if (copy == null)
                throw new InvalidOperationException("Failed to deserialize deep copy.");

            return copy;
        } 
        #endregion
    }
}
