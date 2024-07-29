using DndMapBlazor.Helper;
using DndMapBlazor.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace DndMapBlazor.Components.WorldBuilder;

public partial class FieldEditor : IDisposable
{
    protected override void OnInitialized()
    {
        nfi.NumberDecimalSeparator = ".";
        UpdateGrid();
        UpdateMapTimer = new Timer(async x => await UpdateMapSize(), null, 0, 1000);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }


    [Parameter]
    public Field thisField { get; set; }

    bool loading = false;


    NumberFormatInfo nfi = new NumberFormatInfo();

    double imageWidth = 2;
    double mapWidth = 0;
    double imageHeight = 2;
    double mapHeight = 0;

    public double gridSizeLength = 4;

    public Timer UpdateMapTimer { get; set; }


    int gridX
    {
        get
        {
            return thisField.gridX;
        }
        set
        {
            thisField.gridX = value;
            UpdateGrid();
        }
    }

    int offsetXStart
    {
        get
        {
            return thisField.offsetXStart;
        }
        set
        {
            thisField.offsetXStart = value;
            UpdateGrid();
        }
    }

    int offsetXEnd
    {
        get
        {
            return thisField.offsetXEnd;
        }
        set
        {
            thisField.offsetXEnd = value;
            UpdateGrid();
        }
    }

    int offsetYStart
    {
        get
        {
            return thisField.offsetYStart;
        }
        set
        {
            thisField.offsetYStart = value;
            UpdateGrid();
        }
    }

    int offsetYEnd
    {
        get
        {
            return thisField.offsetYEnd;
        }
        set
        {
            thisField.offsetYEnd = value;
            UpdateGrid();
        }
    }




    private void UpdateGrid()
    {
        mapWidth = imageWidth - thisField.offsetXStart - thisField.offsetXEnd;
        mapHeight = imageHeight - thisField.offsetYStart - thisField.offsetYEnd;

        gridSizeLength = mapWidth / gridX;

        thisField.gridY = (int)(mapHeight / gridSizeLength);
    }

    private async void LoadFiles(InputFileChangeEventArgs e)
    {
        loading = true;
        await using MemoryStream fs = new MemoryStream();
        await e.File.OpenReadStream().CopyToAsync(fs);
        byte[] somBytes = ImageHelper.GetBytes(fs);
        thisField.mapImage = Convert.ToBase64String(somBytes, 0, somBytes.Length);
        loading = false;
        this.StateHasChanged();
    }


    public Wall? currentWall;
    public void AddWall()
    {
        var newWall = new Wall();
        currentWall = newWall;
        thisField.Walls.Add(newWall);
        this.StateHasChanged();
    }

    public void StopWall()
    {
        currentWall = null;
    }



    public void AddWallPointHandler(MouseEventArgs x)
    {
        if (currentWall != null && x.Button == 0)
            currentWall.AddWallpoint((x.OffsetX/imageWidth), (x.OffsetY / imageHeight));
    }

    public async Task UpdateMapSize()
    {
        var newSize = await ImageHelper.GetMapSize(JS, "FieldMap");
        if (newSize.HasValue && (imageWidth != newSize.Value.x || imageHeight != newSize.Value.y))
        {
            imageWidth = newSize.Value.x;
            imageHeight = newSize.Value.y;
            UpdateGrid();
            this.StateHasChanged();
        }
    }

    public void Dispose()
    {
        UpdateMapTimer.Dispose();
    }
}
