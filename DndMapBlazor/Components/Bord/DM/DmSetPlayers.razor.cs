using DndMapBlazor.Helper;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;

namespace DndMapBlazor.Components.Bord.DM
{
    public partial class DmSetPlayers
    {
        bool loading = false;

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }

        [Parameter]
        public EventCallback<bool> SessionUpdatedCallBack { get; set; }

        private List<int> KickedPlayers { get; set; } = new List<int>();


        private void AddPlayer()
        {
            sessionGameMetaData!.Session!.players.Add(new Models.SessionEntites.Player());
        }

        private void RemovePlayer(Models.SessionEntites.Player pl)
        {
            if (!KickedPlayers.Contains(pl.PlayerId))
            {
                KickedPlayers.Add(pl.PlayerId);
                StateHasChanged();
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    sessionGameMetaData!.Session!.players.Remove(pl);
                    KickedPlayers.Remove(pl.PlayerId);
                    StateHasChanged();
                });
            }
        }

        private async void AddPlayerImage(InputFileChangeEventArgs e, Models.SessionEntites.Player pl)
        {
            loading = true;
            await using MemoryStream fs = new MemoryStream();
            await e.File.OpenReadStream(1000000).CopyToAsync(fs);
            var imageBytes = await ImageHelper.GetBytes(fs);
            pl.image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
            loading = false;    
            StateHasChanged();
        }

        private async void ContinueClicked() 
        {
            sessionGameMetaData!.Session!.state = SessionState.Settings;
            await SessionUpdatedCallBack.InvokeAsync();
        }
    }
}
