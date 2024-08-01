using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndMapBlazor.Pages
{
    public partial class PlayerPage
    {
        [Inject]
        ILocalStorageService LocalStorage { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        public Session? session { get; set; }

        public bool isFullScreen { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var loadedSession = await LocalStorage.GetItemAsync<Session>("Session");
            if (loadedSession != null) 
            {
                session = loadedSession;
            }

            await base.OnInitializedAsync();
        }

        protected void OpenInFullScreen() 
        {
            JS.InvokeVoidAsync("FullScreen", "view");
            isFullScreen = true;
        }
    }
}
