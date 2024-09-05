using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models;
using Microsoft.AspNetCore.Components;
using DndMapBlazor.Models.SessionEntites.PlayerBord;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using DndMapBlazor.Models.WorldBuilderModels;
using System.Diagnostics.Tracing;
using Microsoft.JSInterop;

namespace DndMapBlazor.Components.Bord.Player
{
    public partial class PlayerBordField
    {
        [Inject]
        IJSRuntime? JS { get; set; }

        [Parameter]
        public Session? session { get; set; }

        [Parameter]
        public Field? CurrentField { get; set; }


        [Parameter]
        public PlayerBordEventHolder? events { get; set; }


        [Parameter]
        public ChangeView? StartsView { get; set; }


        public double xStart { get; set; }
        public double yStart { get; set; }
        public double xEnd { get; set; }
        public double yEnd { get; set; }


        public Dictionary<Guid, PlayerMapEntity> mapEntities { get; set; } = new Dictionary<Guid, PlayerMapEntity>();


        public double GridSizeInPX { get; set; }

        public List<PlayerBordToken> tokens { get; set; } = new List<PlayerBordToken>();


        protected override void OnInitialized()
        {
            events!.addEntity.callback = new EventCallback<PlayerMapEntity>(this, AddEntityHandler);
            events!.MoveEntity.callback = new EventCallback<MoveEntity>(this, MoveEntityHandler);
            events!.ChangeView.callback = new EventCallback<ChangeView>(this, ChangeViewHandler);
            events!.UpdateToken.callback = new EventCallback<PlayerBordToken>(this, UpdateToken);
            base.OnInitialized();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var size = await Helper.ImageHelper.GetElementSize(JS!, "Grid");
                if (size.HasValue)
                {
                    GridSizeInPX = size.Value.x / CurrentField!.gridX;
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }


        private void UpdateToken(PlayerBordToken token) 
        {
            var foundToken = tokens.Find(x => x.id == token.id);
            if (foundToken != null) 
            {
                foundToken.X = token.X;
                foundToken.Y = token.Y;
            }
            else { 
                tokens.Add(token);
            }

            StateHasChanged();
        }

        private void AddEntityHandler(PlayerMapEntity entity) 
        {
            mapEntities.Add(entity.Id, entity);
        }

        private void MoveEntityHandler(MoveEntity move)
        {
            if (mapEntities.TryGetValue(move.Id, out var entity)) 
            {
                entity.y = move.y;
                entity.x = move.x;
            }
        }

        private void ChangeViewHandler(ChangeView view)
        {
            bool changed = false;
            if (view.xStart != xStart) 
            {
                xStart = view.xStart;
                changed = true;
            }
            if (view.yStart != yStart)
            {
                yStart = view.yStart;
                changed = true;
            }
            if (view.xEnd != xEnd)
            {
                xEnd = view.xEnd;
                changed = true;
            }
            if (view.yEnd != yEnd)
            {
                yEnd = view.yEnd;
                changed = true;
            }
            if (changed)
            {
                this.StateHasChanged();
            }
        }
    }
}
