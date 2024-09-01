using Blazored.LocalStorage;
using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using DndMapBlazor.Models.WorldBuilderModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using static DndMapBlazor.Helper.ImageHelper;

namespace DndMapBlazor.Pages
{
    public partial class PlayerPage : IDisposable
    {
        [Inject]
        ILocalStorageService? LocalStorage { get; set; }

        [Inject]
        IJSRuntime? JS { get; set; }

        public GameCommunicationModel? command { get; set; }
        public Timer? CommandTimer { get; set; }

        public bool isFullScreen { get; set; } = false;


        public Session? session { get; set; }
        public Dictionary<string, WorldMapEntity> mapDict { get; set; } = new Dictionary<string, WorldMapEntity>();

        PlayerBordEventHolder events { get; set; } = new PlayerBordEventHolder();


        protected override async Task OnInitializedAsync()
        {
            var loadedSession = await LocalStorage!.GetItemAsync<Session>("Session");
            if (loadedSession != null) 
            {
                session = loadedSession;
                session.World!.LoadZone();
            }
            var loadedcommand = await LocalStorage.GetItemAsync<GameCommunicationModel>("command");
            if (loadedcommand != null)
            {
                command = loadedcommand;
                await LocalStorage.RemoveItemAsync("command");
            }

            CommandTimer = new Timer(async x => await UpdateCommand(), null, 0, 300);

            SetUpWorldDict(session!.World!);

            await base.OnInitializedAsync();
        }
        private void SetUpWorldDict(Zone map) 
        {
            mapDict.Add(map.Id.ToString(), map);
            foreach (var subMap in map.MapEntities) 
            {
                if (subMap is Field)
                {
                    mapDict.Add(subMap.Id.ToString(), subMap);
                }
                else if(subMap is Zone) {
                    SetUpWorldDict((Zone)subMap);
                }
            }
        }

        private async Task UpdateCommand() 
        {
            var loadedcommand = await LocalStorage!.GetItemAsync<GameCommunicationModel>("command");
            if (loadedcommand != null)
            {
                command = loadedcommand;
                await LocalStorage.RemoveItemAsync("command");
                handleCommand();
                this.StateHasChanged();
            }
        }


        private async void handleCommand() 
        {
            if (command!.state == GameCommunicationState.Running)
            {
                // Change Map
                if (command.Command is GameCommunicationCommand.SetZone)
                {
                    var newMap = mapDict!.GetValueOrDefault(command.data, null);
                    await events.changeMapEvent.callback.InvokeAsync(newMap);
                }

                // Change Map Not used right now
                if (command.Command is GameCommunicationCommand.SetField)
                {
                    var setField = JsonSerializer.Deserialize<SetField>(command.data);

                    var newMap = mapDict!.GetValueOrDefault(setField!.Id, null);
                    await events.changeMapEvent.callback.InvokeAsync(newMap);
                }

                if (command.Command is GameCommunicationCommand.ChangeView)
                {
                    var newView = JsonSerializer.Deserialize<ChangeView>(command.data);
                    await events.ChangeView.callback.InvokeAsync(newView);
                }

            }
            else if (command.state == GameCommunicationState.Settings) 
            {
                if (command.Command is GameCommunicationCommand.GiveMesurments)
                {
                    if (double.TryParse(command.data, out var newMesurment))
                    {
                        session!.MesurmentUnit = newMesurment;
                    }
                }
            }
        }

        protected async Task OpenInFullScreen() 
        {
            await JS!.InvokeVoidAsync("FullScreen", "view");

            var size = await ImageHelper.GetWindowSize(JS!);
            await LocalStorage!.SetItemAsync("ClientSize", size);

            isFullScreen = true;
        }

        public void Dispose()
        {
            CommandTimer!.Dispose();
        }
    }
}
