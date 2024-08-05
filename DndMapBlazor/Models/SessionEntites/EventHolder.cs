using DndMapBlazor.Models.SessionEntites.PlayerBord;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Models.SessionEntites
{
    public class EventHolder<T>
    {
        public EventCallback<T> callback;
    }
}
