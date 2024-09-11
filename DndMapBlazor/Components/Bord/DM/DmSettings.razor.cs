using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DndMapBlazor.Components.Bord.DM
{
    public partial class DmSettings
    {
        [Inject]
        ILocalStorageService? LocalStorage { get; set; }

        [Inject]
        IJSRuntime? JS { get; set; }

        [Parameter]
        public EventCallback<bool> SessionUpdatedCallBack { get; set; }

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }

        public bool TabOpened { get; set; }

        private bool CamaraModalOpen { get; set; }

        public async Task OpenUpPlayingTab()
        {
            await LocalStorage!.SetItemAsync("Session", sessionGameMetaData!.Session!);

            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Settings,
                Command = GameCommunicationCommand.SetupBord
            };
            await LocalStorage!.SetItemAsync("command", command);


            await JS!.InvokeVoidAsync("open", "PlayerPage", "_blank");

            TabOpened = true;
        }

        public async Task SaveMesurmentUnit()
        {
            sessionGameMetaData!.Session!.MesurmentUnit = (100/sessionGameMetaData!.RealWorldScaling);

            await NewCommand(Models.SessionEntites.GameCommunicationCommand.GiveMesurments, sessionGameMetaData!.Session.MesurmentUnit.ToString());
        }

        public async Task NewCommand(GameCommunicationCommand command, string? data = null)
        {
            var newCommand = new GameCommunicationModel()
            {
                state = GameCommunicationState.Settings,
                Command = command,
                data = data ?? ""
            };
            await LocalStorage!.SetItemAsync("command", newCommand);
        }

        public async void StartGame() 
        {
            sessionGameMetaData!.Session!.state = SessionState.Running;
            await SessionUpdatedCallBack.InvokeAsync();
            this.StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await JS!.InvokeVoidAsync("startVideo", "videoFeed");
        }

        public void OnCloseCamaraModal()
        {
            CamaraModalOpen = false;
        }
        private async Task DragPoint(DragEventArgs args, int id)
        {
            sessionGameMetaData.CamaraPoints[id].x += args.OffsetX;
            sessionGameMetaData.CamaraPoints[id].y += args.OffsetY;
        }
    }
}
