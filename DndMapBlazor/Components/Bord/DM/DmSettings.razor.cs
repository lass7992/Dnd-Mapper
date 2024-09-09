using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;

namespace DndMapBlazor.Components.Bord.DM
{
    public partial class DmSettings
    {
        [Inject]
        ILocalStorageService? LocalStorage { get; set; }

        [Inject]
        IJSRuntime? JS { get; set; }

        [Parameter]
        public EventCallback<bool> SessionUpdatedCallBack { get; set; }

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }

        public bool TabOpened { get; set; }

        public string Img { get; set; }

        public string ImgWrapped { get; set; }

        private bool CamaraModalOpen { get; set; }

        double skewX = 0, skewY = 0,scalX = 1,scalY = 1;

        (double x, double y)[] points { get; set; } = new (double x, double y)[] { new(0, 0), new(200, 0), new(200, 200), new(0, 200) };



        public async Task OpenUpPlayingTab()
        {
            await LocalStorage!.SetItemAsync("Session", sessionGameMetaData!.session!);

            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Settings,
                Command = GameCommunicationCommand.SetupBord
            };
            await LocalStorage!.SetItemAsync("command", command);


            await JS!.InvokeVoidAsync("open", "PlayerPage", "_blank");

            TabOpened = true;
        }

        public async Task SaveMesurmentUnit()
        {
            sessionGameMetaData!.session!.MesurmentUnit = (100/sessionGameMetaData!.RealWorldScaling);

            await NewCommand(Models.SessionEntites.GameCommunicationCommand.GiveMesurments, sessionGameMetaData!.session.MesurmentUnit.ToString());
        }

        public async Task NewCommand(GameCommunicationCommand command, string? data = null)
        {
            var newCommand = new GameCommunicationModel()
            {
                state = GameCommunicationState.Settings,
                Command = command,
                data = data ?? ""
            };
            await LocalStorage!.SetItemAsync("command", newCommand);
        }

        public async void StartGame() 
        {
            sessionGameMetaData!.session!.state = SessionState.Running;
            await SessionUpdatedCallBack.InvokeAsync();
            this.StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await JS!.InvokeVoidAsync("startVideo", "videoFeed");
        }

        public void OnCloseCamaraModal()
        {
            CamaraModalOpen = false;
        }

        private async Task CaptureFrame()
        {
            Img = await JS!.InvokeAsync<String>("getFrame", "videoFeed", "currentFrame");
            StateHasChanged();
        }


        private async Task WarpImg()
        {
            ImgWrapped = await JS!.InvokeAsync<String>("getWarpedFrame", "videoFeed", "currentFrame", points[0].x, points[0].y, points[1].x, points[1].y, points[2].x, points[2].y, points[3].x, points[3].y);
            StateHasChanged();
        }

        private async Task DragPoint(DragEventArgs args, int id)
        {
            points[id].x += args.OffsetX;
            points[id].y += args.OffsetY;
        }
    }
}
