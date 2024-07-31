using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Components.Bord
{
    public partial class DmPlayingBordZone 
    {
        [Parameter]
        public Zone thisZone { get; set; }

        [Parameter]
        public SessionGameMetaData SessionGameMetaData { get; set; }

        [Parameter]
        public EventCallback<WorldMapEntity> ChangeMapEntity { get; set; }


        public async Task ChangeMapEntityHandler(WorldMapEntity entity)
        {
            await ChangeMapEntity.InvokeAsync(entity);
        }

    }
}
