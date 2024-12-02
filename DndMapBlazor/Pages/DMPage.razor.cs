using DndMapBlazor.Helper;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components.Forms;
using DndMapBlazor.Components.Bord.DM;
using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Pages
{
    public partial class DMPage
    {
        bool loading = false;
        public EventCallback SessionUpdatedCallBack { get; set; }

        public SessionGameMetaData SessionGameMetaData { get; set; } = new SessionGameMetaData();

        protected override void OnInitialized()
        {
            SessionUpdatedCallBack = new EventCallback(this, SessionUpdatedHandler); 
            base.OnInitialized();
        }

        private void SessionUpdatedHandler(object sender) 
        {
            this.StateHasChanged();
        }

        private async Task LoadWorld(InputFileChangeEventArgs e)
        {
            loading = true;
//            await using MemoryStream fs = new MemoryStream();
//            await e.File.OpenReadStream(5120000).CopyToAsync(fs);

            var newZone = await SaveLoaderHelper.LoadWorld(e.File.OpenReadStream(5120000));


            if (newZone != null) {
                newZone.LoadZone();
                SessionGameMetaData!.Session!.World = newZone;
            }

            loading = false;
            SessionGameMetaData!.Session!.state = SessionState.SetPlayers;
            this.StateHasChanged();
        }
    }
}