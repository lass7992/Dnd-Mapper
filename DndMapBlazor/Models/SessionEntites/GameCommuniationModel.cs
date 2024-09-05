namespace DndMapBlazor.Models.SessionEntites
{
    public class GameCommunicationModel
    {
        public GameCommunicationState state { get; set; }
        public GameCommunicationCommand Command { get; set; }
        public string data { get; set; } = "";
    }

    public enum GameCommunicationState 
    {
        Settings,
        Running,
    }

    public enum GameCommunicationCommand
    {
        None,
     
        //Settings
        SetupBord,
        GiveMesurments,

        //running
        SetZone,
        SetField,
        ChangeView,
        UpdateToken,

    }
}
