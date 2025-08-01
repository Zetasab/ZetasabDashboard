using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Common.ZNT.Models;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;

namespace ZetaDashboard.Components.Pages.ZNT.NotesPage
{
    public partial class NoteComponent
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] public CommonServices CService { get; set; } = default!;

        [Parameter] public NoteModel note { get; set; }
        [Parameter] public EventCallback<NoteModel> update { get; set; }
        [Parameter] public BaseService ApiService { get; set; }
        [Parameter] public DataController DController { get; set; }
        [Parameter] public UserModel LoggedUser { get; set; }

        private async Task AddRemoveFavourite()
        {
            if (note.IsFavorite)
            {
                note.IsFavorite = false;
            }
            else
            {
                note.IsFavorite = true;
            }
            await DController.UpdateData(await ApiService.Notes.UpdateNoteAsync(note, LoggedUser),
                LoggedUser,
                $"znt_UpdateNote",
                $"Actualizando la nota {note.Title} favorite:{note.IsFavorite}");
            await update.InvokeAsync(null);
        }
        private async Task AddRemoveDelete()
        {
            if (note.IsDeleted)
            {
                note.IsDeleted = false;
            }
            else
            {
                note.IsPinned = false;
                note.IsFavorite = false;
                note.IsArchivaded = false;
                note.IsDeleted = true;
            }
            await DController.UpdateData(await ApiService.Notes.UpdateNoteAsync(note, LoggedUser),
                LoggedUser,
                $"znt_UpdateNote",
                $"Actualizando la nota {note.Title} borrada: {note.IsDeleted}");
            await update.InvokeAsync(null);
        }
        private async Task AddRemoveArchivated()
        {
            if (note.IsArchivaded)
            {
                note.IsArchivaded = false;
            }
            else
            {
                note.IsPinned = false;
                note.IsDeleted = false;
                note.IsArchivaded = true;
            }
            await DController.UpdateData(await ApiService.Notes.UpdateNoteAsync(note, LoggedUser),
                LoggedUser,
                $"znt_UpdateNote",
                $"Actualizando la nota {note.Title} archivada: {note.IsArchivaded}");
            await update.InvokeAsync(null);
        }
        private async Task AddRemovePinned()
        {
            if (note.IsPinned)
            {
                note.IsPinned = false;
            }
            else
            {
                note.IsDeleted = false;
                note.IsArchivaded = false;
                note.IsPinned = true;
            }
            await DController.UpdateData(await ApiService.Notes.UpdateNoteAsync(note, LoggedUser),
                LoggedUser,
                $"znt_UpdateNote",
                $"Actualizando la nota {note.Title} pinned: {note.IsPinned}");
            await update.InvokeAsync(null);
        }

        private async Task RemoveConfirm()
        {
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar la nota {note.Title}?" }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<ConfirmDeleteDialog>("¡ATENCIÓN!", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await DController.DeleteData(await ApiService.Notes.DeleteNoteAsync(note, LoggedUser),
               LoggedUser,
               $"znt_DeleteNote",
               $"Borrando la nota {note.Title}");
                await update.InvokeAsync(null);
            }
        }

        private async Task OnNoteClick()
        {
            await update.InvokeAsync(note);
        }
    }
}
