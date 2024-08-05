namespace DndMapBlazor.Models.SessionEntites
{
    public class Session
    {
        public Guid id { get; set; } = Guid.NewGuid();

        public Zone? World { get; set; }
        public List<Player> players { get; set; } = new List<Player>();
        public SessionState state { get; set; } = SessionState.setWorld;

        public double MesurmentUnit { get; set; }
    }


    public enum SessionState
    {
        loadSesion,
        setWorld,
        SetPlayers,
        Settings,
        Running
    }

    
}
