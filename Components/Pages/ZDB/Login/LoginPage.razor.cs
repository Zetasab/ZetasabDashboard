using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.Mongo.Config;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;


namespace ZetaDashboard.Components.Pages.ZDB.Login
{
    public partial class LoginPage
    {
        [Parameter] public string? name { get; set; }
        [Parameter] public string? password { get; set; }
        
        #region Injects
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] AuthenticationStateProvider Auth { get; set; }
        [Inject] NavigationManager Navigator { get; set; }
        [Inject] BaseService ApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] CommonServices CService { get; set; }
        [Inject] IConfiguration Config { get; set; }

        
        #endregion

        #region Vars
        private bool error { get; set; } = false;
        private UserModel UserAccountModel = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                UserAccountModel.Name = name ?? "";
                UserAccountModel.PasswordHash = password ?? "";
                if (name != null && password != null)
                {
                    await OnLogin();
                }
                SetGridSize();
                StartRandomHighlighting();
            }
        }

        private async Task OnLogin()
        {
            try
            {
                var config = Config.GetSection("Mongo").Get<MongoConfig>();

                var client = new MongoClient(config.ConnectionString);
                var database = client.GetDatabase(config.DatabaseName);

                // Comando simple para verificar conexión
                var result = await database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }");

                Console.WriteLine("? Conexión a MongoDB exitosa.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("? Error conectando a MongoDB:");
                Console.WriteLine(ex.Message);
            }

            var adminpassw = UserAccountModel.PasswordHash;
            var usermodel = new UserModel();
            usermodel.Name = UserAccountModel.Name;
            usermodel.PasswordHash = UserAccountModel.PasswordHash;
            var user = await DController.LoginAsync(await ApiService.Users.LoginAsync(usermodel));
            //if (usermodel.Name == "Zetasab" && adminpassw == "Zetasab01!")
            //{
            //    user = new UserModel();
            //    user = UserAccountModel;
            //}
            if (user != null)
            {
                if (CService.CheckLoginPermissions(user)) 
                { 
                    await (Auth as CustomAuthenticationStateProvider).CorrectLogin(user);
                    var audit = new AuditModel(
                        user.Id,
                        user.Name,
                        AuditWhat.Login,
                        nameof(OnLogin),
                        $"El usuario {user.Name} ha logeado correctamente",
                        Common.Mongo.ResponseStatus.Ok);
                    _ = await DController.InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
                    Navigator.NavigateTo("/");
                }
                else
                {
                    Snackbar.Add("No tienes permisos para acceder", Severity.Warning);
                    var audit = new AuditModel(
                        user.Id,
                        user.Name,
                        AuditWhat.Login,
                        nameof(OnLogin),
                        $"El usuario {user.Name} ha logeado pero no tiene permisos",
                        Common.Mongo.ResponseStatus.Unauthorized);
                    _ = await DController.InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
                }
            }
            else
            {
                var audit = new AuditModel(
                        "",
                        UserAccountModel.Name,
                        AuditWhat.Login,
                        nameof(OnLogin),
                        $"El usuario {UserAccountModel.Name} ha logeado pero ha sido fallido",
                        Common.Mongo.ResponseStatus.NotFound);
                _ = await DController.InsertAudit(await ApiService.Audits.InsertAuditAsync(audit));
                UserAccountModel.PasswordHash = "";
                error = true;
                Thread.Sleep(100);
                await InvokeAsync(StateHasChanged);
            }
        }

        #region Password
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private void ButtonTestclick()
        {
            if(isShow)
            {
                    isShow = false;
                    PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                    PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }
        #endregion

        private void OpenUrl(string url)
        {
            JS.InvokeVoidAsync("open", url, "_blank");
        }

        #region Background
        private int Rows = 10;
        private int Cols = 18;
        private HashSet<int> ActiveCells = new();
        private LoginModel loginModel = new();
        private MudBlazor.Breakpoint currentBreakpoint = MudBlazor.Breakpoint.None;

        private Random _rnd = new();

        private void SetGridSize()
        {
            // Responsive: menos cuadraditos en móvil
            if (JS is not null)
            {
                _ = JS.InvokeVoidAsync("eval", @"
                window.setLoginGridSize = () => {
                    if(window.innerWidth < 640){
                        return { rows: 7, cols: 8 };
                    }
                    if(window.innerWidth < 900){
                        return { rows: 8, cols: 12 };
                    }
                    return { rows: 10, cols: 9 };
                }
            ");
            }
        }

        private async void StartRandomHighlighting()
        {
            while (true)
            {
                var idx = _rnd.Next(0, Rows * Cols);
                ActiveCells.Add(idx);
                StateHasChanged();
                await Task.Delay(_rnd.Next(120, 500));
                ActiveCells.Remove(idx);
                StateHasChanged();
            }
        }

        private void ActivateCell(int idx)
        {
            try
            {
                ActiveCells.Add(idx);
                StateHasChanged();
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    ActiveCells.Remove(idx);
                    await InvokeAsync(StateHasChanged);
                });
            }
            catch(Exception ex)
            {

            }
        }

        class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        #endregion
    }
}
