using MudBlazor;
using System.Text.Json;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.ZDB.Models;

namespace ZetaDashboard.Services
{
    public class DataController
    {
        private ISnackbar Snackbar { get; set; }
        public DataController(ISnackbar snackbar) 
        {
            Snackbar = snackbar;
        }

        #region Get
        public async Task<T> GetData<T>(ApiResponse<T> response) 
        {
            if (response.Result)
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
            if (response.Result)
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
            if (response.Result)
            {
                return response.Data;
            }
            else 
            {
                Snackbar.Add(response.Message, Severity.Error);
                return default;
            }
        }

        public async Task<bool> InsertData<T>(ApiResponse<T> response)
        {
            if (response.Result)
            {
                Snackbar.Add(response.Message, Severity.Success);
                return true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                return false;
            }
        }
        #endregion

        #region Update
        public async Task<bool> UpdateData<T>(ApiResponse<T> response)
        {
            if (response.Result)
            {
                Snackbar.Add(response.Message, Severity.Success);
                return true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                return false;
            }
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteData<T>(ApiResponse<T> response)
        {
            if (response.Result)
            {
                Snackbar.Add(response.Message, Severity.Success);
                return true;
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
                return false;
            }
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
