using DndMapBlazor.Helper;
using DndMapBlazor.Models.WorldBuilderModels;
using DndMapBlazor.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using Microsoft.JSInterop;


namespace DndMapBlazor.Components.WorldBuilder;

public partial class ZoneEditor : IDisposable
{
    [Inject]
    IJSRuntime? JS { get; set; }

    [Parameter]
    public Zone? ThisZone { get; set; }

    [Parameter]
    public EventCallback<WorldMapEntity> ChangeMapEntity { get; set; }

    [Parameter]
    public bool IsTopLevel { get; set; }

    [Parameter]
    public ChangeMap? ChangeMap { get; set; }


    bool loading = false;

    private WorldMapEntity? SelectedMapEntity;
    public Timer? UpdateMapTimer { get; set; }

    double imageWidth = 0;
    double imageHeight = 0;

    NumberFormatInfo nfi = new NumberFormatInfo();

    protected override void OnInitialized()
    {
        nfi.NumberDecimalSeparator = ".";

        if (ThisZone == null)
        {
            ThisZone = new Zone();
        }
        UpdateMapTimer = new Timer(async x => await UpdateMapSize(), null, 0, 100);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    private async void LoadFiles(InputFileChangeEventArgs e)
    {
        loading = true;
        await using MemoryStream fs = new MemoryStream();
        await e.File.OpenReadStream(5120000).CopyToAsync(fs);
        byte[] somBytes = await ImageHelper.GetBytes(fs);
        ThisZone!.mapImage = Convert.ToBase64String(somBytes, 0, somBytes.Length);
        loading = false;
        this.StateHasChanged();
    }

    public void AddZone()
    {
        var newZone = new Zone();
        newZone.ParentZone = ThisZone;
        SelectedMapEntity = newZone;
        ThisZone!.MapEntities.Add(newZone);
        this.StateHasChanged();
    }

    public void AddField()
    {
        var newField = new Field();
        newField.ParentZone = ThisZone;
        SelectedMapEntity = newField;
        ThisZone!.MapEntities.Add(newField);
        this.StateHasChanged();
    }

    public void StopEditing()
    {
        SelectedMapEntity = null;
    }

    public void AddPointHandler(MouseEventArgs x)
    {
        if (SelectedMapEntity != null && x.Button == 0)
        {
            SelectedMapEntity.AddPoint((x.OffsetX/imageWidth), (x.OffsetY/imageHeight));
        }
    }

    public async Task ChangeMapEntityHandler(WorldMapEntity entity)
    {
        await ChangeMapEntity.InvokeAsync(entity);
    }

    public async Task UpdateMapSize()
    {
        var newSize = await ImageHelper.GetMapSize(JS!, "ZoneMap");
        if (newSize.HasValue && (imageWidth != newSize.Value.x || imageHeight != newSize.Value.y))
        {
            imageWidth = newSize.Value.x;
            imageHeight = newSize.Value.y;
            this.StateHasChanged();
        }
    }

    public void Dispose()
    {
        UpdateMapTimer!.Dispose();
    }
}
