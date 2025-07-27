using MongoDB.Driver;
using SharpCompress.Common;
using System.Diagnostics.Metrics;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class ProyectService : MongoRepositoryBase<ProyectModel>
        {
            public ProyectService(MongoContext context)
                : base(context, "proyects") { }

            #region Get
            public async Task<ApiResponse<List<ProyectModel>>> GetAllProyectsAsync()
            {
                ApiResponse<List<ProyectModel?>> response = new ApiResponse<List<ProyectModel?>>();
                try
                {
                    var result = await FindAllAsync();
                    if (result != null)
                    {
                        response.Result = true;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "Error obteniendo proyectos";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Error" + ex.Message;
                }
                return response;
            }

            #endregion


            #region Post
            public async Task<ApiResponse<bool>> InsertProyectAsync(ProyectModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();
                
                try
                {
                    await InsertAsync(model);
                    response.Result = true;
                    response.Message = "El proyecto se ha insertado correctamente";
                }
                catch(Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al insertar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            public async Task<ApiResponse<bool>> UpdateProyectAsync(ProyectModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await UpdateAsync(model);
                    response.Result = true;
                    response.Message = "El proyecto se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al editar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeleteProyectAsync(ProyectModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await DeleteAsync(model);
                    response.Result = true;
                    response.Message = "El proyecto se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al borrar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }


    }
}
