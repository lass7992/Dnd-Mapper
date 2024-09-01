using Blazored.LocalStorage;
using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using DndMapBlazor.Models.Enums;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using DndMapBlazor.Models.WorldBuilderModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using static DndMapBlazor.Helper.ImageHelper;

namespace DndMapBlazor.Components.Bord.DM
{
    public partial class DMPlayingBord : IDisposable
    {
        [Inject]
        IJSRuntime? JS { get; set; }

        [Inject]
        ILocalStorageService? LocalStorage { get; set; }

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }

        public Timer? UpdateMapTimer { get; set; }

        public WorldMapEntity? currentMap { get; set; }

        public EventCallback<WorldMapEntity> ChangeMapEvent { get; set; }

        // Change animation
        public double mapChangeAnimationTime { get; set; } = 0.8;
        public ChangeMap changeMap { get; set; } = new ChangeMap();
        public bool RunChangeZone { get; set; } = false;
        public bool RunChangeZoneAnimation { get; set; } = false;


        protected override void OnInitialized()
        {
            currentMap = sessionGameMetaData!.session!.World;

            UpdateMapTimer = new Timer(async x => await UpdateMapSize(), null, 0, 100);

            StartAnimationTimer = new System.Timers.Timer(10);
            StartAnimationTimer.Elapsed += (x, y) =>
            {
                RunChangeZoneAnimation = true;
                this.StateHasChanged();
            };
            StartAnimationTimer.AutoReset = false;
            EndAnimationTimer = new System.Timers.Timer(mapChangeAnimationTime * 1000);
            EndAnimationTimer.AutoReset = false;
            EndAnimationTimer.Elapsed += (x, y) =>
            {
                RunChangeZoneAnimation = false;
                RunChangeZone = false;
                this.StateHasChanged();
            };

            ChangeMapEvent = new EventCallback<WorldMapEntity>(this, ChangeMap);
        }

        protected override async Task OnInitializedAsync()
        {
            // Set new command
            await CommandNewZone();

            var windowSize = await LocalStorage!.GetItemAsync<WindowsSize>("ClientSize");
            sessionGameMetaData!.ClientWindowHeight = windowSize!.height;
            sessionGameMetaData!.ClientWindowWidth = windowSize!.width;

            await base.OnInitializedAsync();
        }


        System.Timers.Timer StartAnimationTimer = new System.Timers.Timer();
        System.Timers.Timer EndAnimationTimer = new System.Timers.Timer();

        private void ChangeSelected(WorldMapEntity entity)
        {
            changeMap.FromImage = currentMap!.mapImage;
            changeMap.ToImage = entity.mapImage;


            if (entity == sessionGameMetaData!.session!.World || entity == currentMap.ParentZone)
            {
                changeMap.ZoomIn = false;
                changeMap.xPos = ((currentMap.x + (currentMap.width / 2)));
                changeMap.yPos = ((currentMap.y + (currentMap.height / 2)));
            }
            else
            {
                changeMap.ZoomIn = true;
                changeMap.xPos = (entity.x + (entity.width / 2));
                changeMap.yPos = (entity.y + (entity.height / 2));
            }

            changeMap.height = 800;

            RunChangeZone = true;
            RunChangeZoneAnimation = false;
            StartAnimationTimer.Start();
            EndAnimationTimer.Start();

            currentMap = entity;
        }

        public async Task UpdateMapSize()
        {
            var newSize = await ImageHelper.GetMapSize(JS!, "ZoneMap");
            if (newSize.HasValue && (sessionGameMetaData!.DmImageWidth != newSize.Value.x || sessionGameMetaData.DmImageHeight != newSize.Value.y))
            {
                sessionGameMetaData.DmImageWidth = newSize.Value.x;
                sessionGameMetaData.DmImageHeight = newSize.Value.y;
                this.StateHasChanged();
                await sessionGameMetaData.UpdatedDataEvent.InvokeAsync();
            }
        }

        public async void ChangeMap(WorldMapEntity map) 
        {
            ChangeSelected(map);
            if(map is Zone) 
            {
                await CommandNewZone();

            }else if (map is Field) 
            {
                await CommandNewField((Field)map);
            }
        }

        private async Task CommandNewZone() 
        {
            // Set new command
            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Running,
                Command = GameCommunicationCommand.SetZone,
                data = currentMap!.Id.ToString()
            };
            await LocalStorage!.SetItemAsync("command", command);
        }

        private async Task CommandNewField(Field field)
        {
            var data = JsonSerializer.Serialize(new SetField() { Id = field.Id.ToString(), xStart = 0, yStart = 0, xEnd = sessionGameMetaData!.ClientWindowWidth, yEnd = sessionGameMetaData.ClientWindowHeight });

            // Set new command
            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Running,
                Command = GameCommunicationCommand.SetField,
                data = data
            };

            await LocalStorage!.SetItemAsync("command", command);
        }

        public void Dispose()
        {
            UpdateMapTimer!.Dispose();
        }
    }
}
