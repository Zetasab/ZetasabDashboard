using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Common.ZNT.Models;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZNT.NotesPage
{
    public partial class NotesPage
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        #endregion

        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }
        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "znt",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "znt",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "znt",
            UserType = EUserPermissionType.Admin
        };
        #endregion
        private List<NoteModel> DataList = new();
        private NoteModel SelectedNote = new();
        //loadings
        private bool datagridLoading = false;
        private bool insertDataLoading = false;
        private bool updateDataLoading = false;
        private bool deleteDataLoading = false;

        private bool pinnedShow = true;
        private bool archivatedShow = false;
        private bool deletedShow = false;
        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en {ApiService.Notes._datos}",
                "Entrando en notas",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            DataList?.Clear();
            DataList = null;
        }
        #endregion


        #region Crud
        private async Task OnInsertData()
        {
            insertDataLoading = true;
            await InvokeAsync(StateHasChanged);

            var note = new NoteModel()
            {
                UserId = LoggedUser.Id,
                Title = "Sin titulo",
                UpdatedAt = DateTime.Now
            };

            var result = await DController.InsertData(
                await ApiService.Notes.InsertNoteAsync(note, LoggedUser),
                LoggedUser,
                $"NotesPage:{nameof(OnInsertData)}",
                $"Insertando {ApiService.Notes._ellaDato} {note.Title}"
                );

            insertDataLoading = false;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                GetList();
            }
        }
        #region Get
        private async Task GetList()
        {
            datagridLoading = true;
            await InvokeAsync(StateHasChanged);

            DataList = await DController.GetData(await ApiService.Notes.GetAllNotesByUserAsync(LoggedUser)) ?? new List<NoteModel>();

            DataList = DataList.OrderByDescending(x => x.UpdatedAt.Value).ToList();

            datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        private async Task ClickNoteOrGet(NoteModel note)
        {
            if (note != null)
            {
                SelectedNote = await DController.DeepCoopy(note);
                IsNoteEditShow = true;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                GetList();
            }
        }
        #endregion

        #region Update
        private async Task SaveNote()
        {
            var resultt = DController.UpdateData(await ApiService.Notes.UpdateNoteAsync(SelectedNote, LoggedUser),
                LoggedUser,
                $"znt_note actualizando nota {SelectedNote.Title}",
                $"Se ha actualizado correctamente la nota");
            if(resultt != null)
            {
                IsNoteEditShow = false;
                GetList();
            }
        }
        #endregion
        #endregion

        #region Events
        private void ChangePinnerShow()
        {
            pinnedShow = !pinnedShow;
            StateHasChanged();
        }

        private void ChangeArchivatedShow()
        {
            archivatedShow = !archivatedShow;
            StateHasChanged();
        }

        private void ChangeDeletedShow()
        {
            deletedShow = !deletedShow;
            StateHasChanged();
        }

        private bool IsNoteEditShow = false;

        private bool isEditing = false;
        private bool isEditing2 = false;

        private void ExitEdit(FocusEventArgs e)
        {
            isEditing = false;
            // aquí podrías guardar si quieres
        }
        private void ExitEdit2(FocusEventArgs e)
        {
            isEditing2 = false;
            // aquí podrías guardar si quieres
        }
        #endregion



    }
}
