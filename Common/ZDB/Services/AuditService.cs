using MongoDB.Driver;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class AuditService: MongoRepositoryBase<AuditModel>
        {
            private List<string> thispage = new List<string>() { "zdb" };
            public AuditService(MongoContext context)
                : base(context, "audits") { }


            #region Get
            public async Task<ApiResponse<List<AuditModel>>> GetAllAuditsAsync(UserModel loggeduser)
            {
                ApiResponse<List<AuditModel?>> response = new ApiResponse<List<AuditModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
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
                        response.Message = "Error obteniendo adutorias";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un problema al obtener las auditorias";
                    Console.WriteLine(ex.ToString());
                }
                return response;
            }

            public async Task<ApiResponse<List<AuditModel>>> GetAllAuditsByPageAsync(int pageNumber, int pageSize, UserModel loggeduser)
            {
                ApiResponse<List<AuditModel?>> response = new ApiResponse<List<AuditModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    var result = await FindAllByPagedAsync(pageNumber, pageSize);
                    if (result != null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = "Error obteniendo adutorias";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un problema al obtener las auditorias";
                    Console.WriteLine(ex.ToString());
                }
                return response;
            }
            #endregion

            #region Post
            public async Task<ApiResponse<bool>> InsertAuditAsync(AuditModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await InsertAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = "La auditoria se ha insertado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al insertar la auditoria";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            //public async Task<ApiResponse<bool>> UpdateAuditAsync(UserModel model, UserModel loggeduser)
            //{
            //    ApiResponse<bool> response = new ApiResponse<bool>();

            //    try
            //    {
            //        if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
            //        {
            //            response.Result = ResponseStatus.Unauthorized;
            //            response.Message = "No tienes permisos";
            //            return response;
            //        }
            //        await UpdateAsync(model);
            //        response.Result = ResponseStatus.Ok;
            //        response.Message = "El usuario se ha editado correctamente";
            //    }
            //    catch (Exception ex)
            //    {
            //        response.Result = ResponseStatus.InternalError;
            //        response.Message = "Ha ocurrido un error al editar el usuario";
            //        Console.WriteLine($"Error: {ex.Message}");
            //    }
            //    return response;
            //}
            #endregion

            #region Delete
            //public async Task<ApiResponse<bool>> DeleteUserAsync(UserModel model, UserModel loggeduser)
            //{
            //    ApiResponse<bool> response = new ApiResponse<bool>();

            //    try
            //    {
            //        if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Admin, thispage))
            //        {
            //            response.Result = ResponseStatus.Unauthorized;
            //            response.Message = "No tienes permisos";
            //            return response;
            //        }
            //        await DeleteAsync(model);
            //        response.Result = ResponseStatus.Ok;
            //        response.Message = "El usuario se ha borrado correctamente";
            //    }
            //    catch (Exception ex)
            //    {
            //        response.Result = ResponseStatus.InternalError;
            //        response.Message = "Ha ocurrido un error al borrar el usuario";
            //        Console.WriteLine($"Error: {ex.Message}");
            //    }
            //    return response;
            //}
            #endregion
        }

    }
}
