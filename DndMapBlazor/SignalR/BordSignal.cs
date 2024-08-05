using Microsoft.AspNetCore.SignalR;

namespace DndMapBlazor.SignalR
{
    public class BordSignal : Hub
    {
        public async Task SendMessage(string sessionID, string message)
        {
            await Clients.All.SendAsync("BordMessage", sessionID, message);
        }
    }
}
