using MudBlazor;
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
    }
}
