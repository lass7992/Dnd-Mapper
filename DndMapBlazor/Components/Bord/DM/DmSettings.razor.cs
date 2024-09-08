using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Drawing;
using System.Drawing.Imaging;

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

        private bool CamaraModalOpen { get; set; }

        double skewX = 0, skewY = 0,scalX = 1,scalY = 1;



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
            Img = await JS!.InvokeAsync<String>("getWarpedFrame", "videoFeed", "currentFrame", 100,100, 320,0 , 320, 240,  0, 240);
            StateHasChanged();
        }

        private async Task WarpImg2()
        {
            string imgBase64 = Img.Substring(Img.IndexOf("base64,")+7);
            Img = "data:image/jpeg;base64," + WarpImgFunc(imgBase64);
            StateHasChanged();
        }

        public string WarpImgFunc(string base64String)
        {
            int GridX = 10;
            int GridY = 7;
            int Width = GridX * 16;
            int Heigh = GridY * 16;

            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Bitmap.FromStream(ms, false);
            Image image = Image.FromStream(ms);

            var newImg = Graphics.FromImage(image);
            newImg.Transform.TransformPoints(new Point[] { new Point(100, 100), new Point(200, 100), new Point(200, 200), new Point(100, 200) });

            ms.Position = 0;
            image.Save(ms, ImageFormat.Jpeg);

            return Convert.ToBase64String(ms.ToArray());
        }

    }
}
