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
            private List<string> thispage = new List<string>() { "zdb" };
            public ProyectService(MongoContext context)
                : base(context, "proyects") { }

            #region Get
            public async Task<ApiResponse<List<ProyectModel>>> GetAllProyectsAsync(UserModel loggeduser)
            {
                ApiResponse<List<ProyectModel?>> response = new ApiResponse<List<ProyectModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser,UserModel.EUserPermissionType.Visor,thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var result = await FindAllAsync();
                    if (result != null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = "Error obteniendo proyectos";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Error" + ex.Message;
                }
                return response;
            }

            #endregion


            #region Post
            public async Task<ApiResponse<bool>> InsertProyectAsync(ProyectModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();
                
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await InsertAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = "El proyecto se ha insertado correctamente";
                }
                catch(Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al insertar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            public async Task<ApiResponse<bool>> UpdateProyectAsync(ProyectModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await UpdateAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = "El proyecto se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al editar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeleteProyectAsync(ProyectModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Admin, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await DeleteAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = "El proyecto se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al borrar el proyecto";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }


    }
}
