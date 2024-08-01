using Blazored.LocalStorage;
using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace DndMapBlazor.Pages
{
    public partial class DMPage
    {
        private Session session = new Session();

        bool loading = false;



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



        private async Task LoadWorld(InputFileChangeEventArgs e)
        {
            loading = true;
//            await using MemoryStream fs = new MemoryStream();
//            await e.File.OpenReadStream(5120000).CopyToAsync(fs);

            var newZone = await SaveLoaderHelper.LoadWorld(e.File.OpenReadStream(5120000));


            if (newZone != null) { 
                session.World = newZone;
            }

            loading = false;
            session.state = SessionState.SetPlayers;
            this.StateHasChanged();
        }
    }
}