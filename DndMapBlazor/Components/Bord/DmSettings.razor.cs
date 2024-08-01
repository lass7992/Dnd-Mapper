using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndMapBlazor.Components.Bord
{
    public partial class DmSettings
    {
        [Inject]
        ILocalStorageService LocalStorage { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        [Parameter]
        public Session session { get; set; }

        private async Task OpenUpPlayingTab()
        {
            await LocalStorage.SetItemAsync<Session>("Session", session);
            await JS.InvokeVoidAsync("open", "PlayerPage", "_blank");
        }
    }
}
