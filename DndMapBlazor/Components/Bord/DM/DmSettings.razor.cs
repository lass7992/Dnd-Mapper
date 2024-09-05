using Blazored.LocalStorage;
using DndMapBlazor.Models;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
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

        public string Img { get; set; }

        private bool CamaraModalOpen { get; set; }



        public async Task OpenUpPlayingTab()
        {
            await LocalStorage!.SetItemAsync("Session", sessionGameMetaData!.session!);

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
            sessionGameMetaData!.session!.MesurmentUnit = (100/sessionGameMetaData!.RealWorldScaling);

            await NewCommand(Models.SessionEntites.GameCommunicationCommand.GiveMesurments, sessionGameMetaData!.session.MesurmentUnit.ToString());
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
            sessionGameMetaData!.session!.state = SessionState.Running;
            await SessionUpdatedCallBack.InvokeAsync();
            this.StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await JS!.InvokeVoidAsync("startVideo", "videoFeed");
        }

        private async Task CaptureFrame()
        {
            Img = await JS!.InvokeAsync<String>("getFrame", "videoFeed", "currentFrame");
            StateHasChanged();
        }

        public void OnCloseCamaraModal()
        {
            CamaraModalOpen = false;
        }
    }
}
