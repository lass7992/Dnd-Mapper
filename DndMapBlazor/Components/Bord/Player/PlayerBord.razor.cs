using DndMapBlazor.Models;
using DndMapBlazor.Models.Enums;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models.SessionEntites.PlayerBord;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using DndMapBlazor.Models.WorldBuilderModels;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Tracing;

namespace DndMapBlazor.Components.Bord.Player
{
    public partial class PlayerBord
    {
        [Parameter]
        public Session? session { get; set; }

        [Parameter]
        public PlayerBordEventHolder events { get; set; }

        public WorldMapEntity? CurrentMap { get; set; }

        public ChangeMap changeMap { get; set; } = new ChangeMap();
        public bool RunChangeAnimation { get; set; } = false;
        public bool RunChangeAnimationStart { get; set; } = false;
        System.Timers.Timer StartAnimationTimer = new System.Timers.Timer();
        System.Timers.Timer EndAnimationTimer = new System.Timers.Timer();
        public double mapChangeAnimationTime { get; set; } = 0.8;

        //FieldEvents
        public ChangeView StartsView { get; set; }


        protected override void OnInitialized()
        {
            StartAnimationTimer = new System.Timers.Timer(10);
            StartAnimationTimer.Elapsed += (x, y) =>
            {
                RunChangeAnimationStart = true;
                this.StateHasChanged();
            };
            StartAnimationTimer.AutoReset = false;
            EndAnimationTimer = new System.Timers.Timer(mapChangeAnimationTime * 1000);
            EndAnimationTimer.AutoReset = false;
            EndAnimationTimer.Elapsed += (x, y) =>
            {
                RunChangeAnimationStart = false;
                RunChangeAnimation = false;
                this.StateHasChanged();
            };

            CurrentMap = session!.World;
            events.changeMapEvent.callback = new EventCallback<WorldMapEntity>(this, ChangeMapHandler);

            base.OnInitialized();
        }

        private void ChangeMapHandler(WorldMapEntity entity) 
        {
            changeMap.FromImage = CurrentMap.mapImage;
            changeMap.ToImage = entity.mapImage;


            if (entity == session.World || entity == CurrentMap.ParentZone)
            {
                changeMap.ZoomIn = false;
                changeMap.xPos = ((CurrentMap.x + (CurrentMap.width / 2)));
                changeMap.yPos = ((CurrentMap.y + (CurrentMap.height / 2)));
            }
            else
            {
                changeMap.ZoomIn = true;
                changeMap.xPos = (entity.x + (entity.width / 2));
                changeMap.yPos = (entity.y + (entity.height / 2));
            }

            changeMap.height = 800;

            RunChangeAnimation = true;
            RunChangeAnimationStart = false;
            StartAnimationTimer.Start();
            EndAnimationTimer.Start();

            CurrentMap = entity;
        }
    }
}
