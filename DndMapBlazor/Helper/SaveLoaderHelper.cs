using DndMapBlazor.Models;
using System.IO;
using System.Text;
using System.Text.Json;

namespace DndMapBlazor.Helper
{
    public static class SaveLoaderHelper
    {
        public static async Task<Zone> LoadWorld(Stream jsonStream) 
        {
            var newZone = await JsonSerializer.DeserializeAsync<Zone>(jsonStream);

            //Instantiate parentZones
            SetParentsOnZone(newZone!);

            return newZone!;
        }
        private static void SetParentsOnZone(Zone parentZone) 
        {
            foreach (var entity in parentZone.MapEntities)
            {
                entity.ParentZone = parentZone;
                if (entity is Zone) 
                {
                    SetParentsOnZone((Zone)entity);
                }
            }        
        }
    }
}
