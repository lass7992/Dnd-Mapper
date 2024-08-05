using DndMapBlazor.Models.SessionEntites.PlayerBord;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Models.SessionEntites
{
    public class PlayerBordEventHolder
    {
        public EventHolder<PlayerMapEntity> addEntity { get; set; } = new EventHolder<PlayerMapEntity>();
        public EventHolder<MoveEntity> MoveEntity { get; set; } = new EventHolder<MoveEntity>();
        public EventHolder<ChangeView> ChangeView { get; set; } = new EventHolder<ChangeView>();
        public EventHolder<WorldMapEntity> changeMapEvent { get; set; } = new EventHolder<WorldMapEntity>();
        
    }
}
