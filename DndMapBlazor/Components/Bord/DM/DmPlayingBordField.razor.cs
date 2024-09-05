using Blazored.LocalStorage;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using DndMapBlazor.Models.SessionEntites;
using DndMapBlazor.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using DndMapBlazor.Models.SessionEntites.PlayerBord;

namespace DndMapBlazor.Components.Bord.DM
{
    public partial class DmPlayingBordField
    {
        [Parameter]
        public Field? thisField { get; set; }

        [Parameter]
        public SessionGameMetaData? sessionGameMetaData { get; set; }

        [Inject]
        ILocalStorageService? LocalStorage { get; set; }

        //TODO set these by the scaling and width:?
        public double playerBordWidth = 300;
        public double playerBordHeight = 300;

        public double X { get; set; } = -99;
        public double Y { get; set; } = -99;

        public double startX { get; set; } = -99;
        public double startY { get; set; } = -99;

        public double? ClientX;
        public double? ClientY;

        public bool playerTokenHasBeenSet { get; set; } = false;

        public List<PlayerBordToken> tokens { get; set; } = new List<PlayerBordToken>();

        public double GridSizeInPX {get;set;}


        protected override void OnInitialized()
        {
            sessionGameMetaData!.UpdatedDataEvent = new EventCallback(this, SetPlayerBordView);
            base.OnInitialized();

            Random rand = new Random();
            foreach (var player in sessionGameMetaData!.session!.players)
            {
                tokens.Add(new PlayerBordToken()
                {
                    Name = player.Name,
                    img = player.image,
                    X = rand.Next(0, thisField.gridX),
                    Y = rand.Next(0, thisField.gridY),
                });
            }
        }

        private DateTime lastUpdate = DateTime.MinValue;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (DateTime.Now > lastUpdate) {
                double widthPart = (sessionGameMetaData.DmImageWidth / 100);
                GridSizeInPX = (sessionGameMetaData.DmImageWidth - (widthPart * thisField.offsetXStart + widthPart * thisField.offsetXEnd)) / thisField.gridX - 1;
                lastUpdate = DateTime.Now.AddSeconds(0.5);
            }

            return base.OnAfterRenderAsync(firstRender);
        }


        private void SetPlayerBordView()
        {
            double TempScalingX = ((double)sessionGameMetaData!.ClientWindowWidth / ((double)thisField!.gridX * 25.4 + (double)thisField!.offsetXStart + (double)thisField!.offsetXEnd) * (sessionGameMetaData!.RealWorldScaling)) / 100.0;
            double TempScalingY = ((double)sessionGameMetaData!.ClientWindowHeight / ((double)thisField!.gridY * 25.4 + (double)thisField!.offsetYStart + (double)thisField!.offsetYEnd) * (sessionGameMetaData!.RealWorldScaling)) / 100.0;

            playerBordWidth = TempScalingX * (double)sessionGameMetaData.DmImageWidth;
            playerBordHeight = TempScalingY * (double)sessionGameMetaData.DmImageHeight;

            if (X == -99)
            {
                X = ((double)playerBordWidth / (double)sessionGameMetaData.DmImageWidth * 100.0) / 2.0;
                Y = (double)playerBordHeight / (double)sessionGameMetaData.DmImageHeight * 100.0;
                startX = X;
                startY = Y;
            }
        }

        private async Task DragToken(DragEventArgs args, PlayerBordToken token)
        {
            token.X += (int)Math.Round(args.OffsetX / GridSizeInPX);
            token.Y += (int)Math.Round(args.OffsetY / GridSizeInPX);

            var data = JsonSerializer.Serialize(token);

            // Set new command
            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Running,
                Command = GameCommunicationCommand.UpdateToken,
                data = data
            };

            await LocalStorage!.SetItemAsync("command", command);
        }

        private async Task DragView(DragEventArgs args)
        {
            if (!ClientX.HasValue)
            {
                ClientX = args.PageX;
                ClientY = args.PageY;
            }

            X += ((args.OffsetX) / sessionGameMetaData!.DmImageWidth) * 100;

            Y += ((args.OffsetY) / sessionGameMetaData!.DmImageHeight) * 100;

            //        X = ((args.PageX - ClientX!.Value) / SessionGameMetaData!.DmImageWidth) * 100;
            //        Y = ((args.PageY - ClientY!.Value) / SessionGameMetaData!.DmImageHeight) * 100;
            X = Math.Min(X, 100 - startX);
            X = Math.Max(X, startX);
            Y = Math.Min(Y, 100);
            Y = Math.Max(Y, startY);


            var data = JsonSerializer.Serialize(new ChangeView() { xStart = X, yStart = Y });

            // Set new command
            var command = new GameCommunicationModel()
            {
                state = GameCommunicationState.Running,
                Command = GameCommunicationCommand.ChangeView,
                data = data
            };

            await LocalStorage!.SetItemAsync("command", command);
        }
    }
}
