using DndMapBlazor.Helper;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components.Forms;
using DndMapBlazor.Components.Bord.DM;
using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Pages
{
    public partial class DMPage
    {
        private Session session = new Session();

        bool loading = false;
        public EventCallback SessionUpdatedCallBack { get; set; }

        public SessionGameMetaData SessionGameMetaData { get; set; } = new SessionGameMetaData();

        protected override void OnInitialized()
        {
            SessionUpdatedCallBack = new EventCallback(this, SessionUpdatedHandler); 
            base.OnInitialized();
        }


        private void AddPlayer() 
        {
            session.players.Add(new Player());
        }

        private void RemovePlayer(Player pl)
        {
            session.players.Remove(pl);
        }
        private async void AddPlayerImage(InputFileChangeEventArgs e, Player pl)
        {
            loading = true;
            await using MemoryStream fs = new MemoryStream();
            await e.File.OpenReadStream(1000000).CopyToAsync(fs);
            var imageBytes = await ImageHelper.GetBytes(fs);
            pl.image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
            loading = false;
            this.StateHasChanged();
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
                session.World = newZone;
            }

            loading = false;
            session.state = SessionState.SetPlayers;
            this.StateHasChanged();
        }
    }
}