using MongoDB.Driver;
using SharpCompress.Common;
using System.Diagnostics.Metrics;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.PLN.Models;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZNT.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class PlanListService : MongoRepositoryBase<PlanListModel>
        {
            private List<string> thispage = new List<string>() { "pln" };
            public PlanListService(MongoContext context)
                : base(context, "pln_planlists") { }

            public string _dato = "lista de planes";
            public string _datos = "listas de planes";
            public string _ellaDato = "la lista de planes";
            public string _loslasDatos = "las listas de planes";

            #region Get
            public async Task<ApiResponse<List<PlanListModel>>> GetAllPlanListsAsync(UserModel loggeduser)
            {
                ApiResponse<List<PlanListModel?>> response = new ApiResponse<List<PlanListModel?>>();
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
                        response.Message = $"No hay {_loslasDatos}";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al obtener los proyectos";
                    Console.WriteLine(ex.ToString());
                }
                return response;
            }
            public async Task<ApiResponse<List<PlanListModel>>> GetAllPlanListsByUserAsync(UserModel loggeduser)
            {
                ApiResponse<List<PlanListModel>> response = new();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var filter = Builders<PlanListModel>.Filter.Or(
                         Builders<PlanListModel>.Filter.Eq(x => x.OwnerId, loggeduser.Id),
                         Builders<PlanListModel>.Filter.AnyEq(x => x.UsersIds, loggeduser.Id)
                     // alternativa: Builders<PlanListModel>.Filter.ElemMatch(x => x.UsersIds, u => u == loggeduser.Id)
                     );
                    var result = await FindAllAsync(filter);

                    if (result is { Count: > 0 })
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"No se encontraron {_loslasDatos} para el usuario.";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al obtener los proyectos";
                    Console.WriteLine(ex.ToString());
                }

                return response;
            }
            #endregion


            #region Post
            public async Task<ApiResponse<bool>> InsertPlanListAsync(PlanListModel model, UserModel loggeduser)
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
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha insertado correctamente";
                }
                catch(Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al insertar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            public async Task<ApiResponse<bool>> UpdatePlanListAsync(PlanListModel model, UserModel loggeduser)
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
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al editar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeletePlanListAsync(PlanListModel model, UserModel loggeduser)
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
                    await DeleteAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al borrar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }


    }
}
