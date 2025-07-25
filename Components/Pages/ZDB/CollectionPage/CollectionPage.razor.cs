using Microsoft.AspNetCore.Components;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Pages.ZDB.CollectionPage
{
    public partial class CollectionPage
    {
        #region Injects
        [Inject] MongoInfoService MongoService { get; set; }
        #endregion

        #region Vars
        private List<string> CollectionNames = new();
        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            CollectionNames = await MongoService.GetCollectionNamesAsync();
        }
        #endregion
    }
}
