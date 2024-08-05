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
        public Session? session { get; set; }

        [Parameter]
        public EventCallback<bool> SessionUpdatedCallBack { get; set; }

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }


        public bool TabOpened { get; set; }

        public async Task OpenUpPlayingTab()
        {
            await LocalStorage!.SetItemAsync("Session", session!);

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
            session.MesurmentUnit = (100/sessionGameMetaData.RealWorldScaling);

            NewCommand(Models.SessionEntites.GameCommunicationCommand.GiveMesurments, session.MesurmentUnit.ToString());
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
            session!.state = SessionState.Running;
            await SessionUpdatedCallBack.InvokeAsync();
            this.StateHasChanged();
        }
    }
}
