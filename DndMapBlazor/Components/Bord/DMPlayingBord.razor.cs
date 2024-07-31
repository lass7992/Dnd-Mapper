using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using DndMapBlazor.Models.Enums;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models.WorldBuilderModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndMapBlazor.Components.Bord
{
    public partial class DMPlayingBord : IDisposable
    {
        [Inject]
        IJSRuntime JS { get; set; }

        [Parameter]
        public Session? session { get; set; }

        public Timer UpdateMapTimer { get; set; }

        private SessionGameMetaData SessionGameMetaData { get; set; } = new SessionGameMetaData();

        public WorldMapEntity currentZone { get; set; }

        public EventCallback<WorldMapEntity> ChangeMapEntity { get; set; }

        // Change animation
        public double mapChangeAnimationTime { get; set; } = 0.8;
        public ChangeMap changeMap { get; set; } = new ChangeMap();
        public bool RunChangeZone { get; set; } = false;
        public bool RunChangeZoneAnimation { get; set; } = false;


        protected override void OnInitialized()
        {
            currentZone = session.World;
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
        }



        System.Timers.Timer StartAnimationTimer = new System.Timers.Timer();
        System.Timers.Timer EndAnimationTimer = new System.Timers.Timer();

        private void ChangeSelected(WorldMapEntity entity)
        {
            changeMap.FromImage = currentZone.mapImage;
            changeMap.ToImage = entity.mapImage;


            if (entity == session.World || entity == currentZone.ParentZone)
            {
                changeMap.ZoomIn = false;
                changeMap.xPos = ((currentZone.x + (currentZone.width / 2)));
                changeMap.yPos = ((currentZone.y + (currentZone.height / 2)));
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

            currentZone = entity;
        }

        public async Task UpdateMapSize()
        {
            var newSize = await ImageHelper.GetMapSize(JS, "ZoneMap");
            if (newSize.HasValue && (SessionGameMetaData.imageWidth != newSize.Value.x || SessionGameMetaData.imageHeight != newSize.Value.y))
            {
                SessionGameMetaData.imageWidth = newSize.Value.x;
                SessionGameMetaData.imageHeight = newSize.Value.y;
                this.StateHasChanged();
            }
        }

        public void Dispose()
        {
            UpdateMapTimer.Dispose();
        }
    }
}
