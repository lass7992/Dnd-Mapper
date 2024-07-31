namespace DndMapBlazor.Models.SessionEntites
{
    public class Session
    {
        public Zone? World { get; set; }
        public List<Player> players { get; set; } = new List<Player>();

        public SessionState state { get; set; } = SessionState.setWorld;
    }


    public enum SessionState
    {
        loadSesion,
        setWorld,
        SetPlayers,
        Started
    }

    
}
