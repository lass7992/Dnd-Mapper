window.GetElementClientSize = (elementId) => {
    var element = document.getElementById(elementId);
    if (element == undefined)
    {
        return { x: 0, y: 0};
    }

    return {
        x: element.width,
        y: element.height
    };
};

window.GetElementSize = (elementId) => {
    var element = document.getElementById(elementId);
    if (element == undefined) {
        return { x: 0, y: 0 };
    }

    return {
        x: element.clientWidth,
        y: element.clientHeight
    };
};


async function downloadFileFromStream(fileName, contentStreamReference)
{
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    triggerFileDownload(fileName, url);
    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;

    if (fileName) {
        anchorElement.download = fileName;
    }

    anchorElement.click();
    anchorElement.remove();
}

async function FullScreen(id)
{
    const screenElement = document.getElementById(id);
    screenElement.requestFullscreen();
}

function GetWindowSize()
{
    return {
        x: window.innerWidth,
        y: window.innerHeight
    };
}

function startVideo(src) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
            let video = document.getElementById(src);
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                video.src = window.URL.createObjectURL(stream);
            }
            video.onloadedmetadata = function (e) {
                video.play();
            };

            //mirror image
            video.style.webkitTransform = "scaleX(1)";
            video.style.transform = "scaleX(1)";

            var camaraVideo = document.getElementById('CamaraVideo');
            camaraVideo.addEventListener('dragover', e => e.preventDefault());

            var dragPoints = document.getElementsByClassName("VideoDragPoint");
            console.log(dragPoints, dragPoints)
            let halfWidth = (dragPoints[0].offsetWidth) / 2

            for (let i = 0; i < dragPoints.length; i++) {
                console.log("etst", halfWidth);
                dragPoints[i].addEventListener('dragstart', e => { console.log('asdf'); e.dataTransfer.setDragImage(dragPoints[i], halfWidth, halfWidth) })
            }
        });
    }
}

function getFrame(src, dest) {
    let video = document.getElementById(src);
    let canvas = document.getElementById(dest);
    canvas.getContext('2d').drawImage(video, 0, 0, 320, 240);

    let dataUrl = canvas.toDataURL("image/jpeg");
    return dataUrl;
}

function getWarpedFrame(src, dest, x1, y1, x2, y2, x3, y3, x4, y4) {
    let gridX = 7;
    let gridY = 8;

    let width = 320
    let height = 240

    let tileSize = 32

    let video = document.getElementById(src);
    width = video.offsetWidth
    height = video.offsetHeight

    let canvas = document.getElementById(dest);
    let ctx = canvas.getContext('2d');

    ctx.drawImage(video, 0, 0, width, height);
    let pixels = ctx.getImageData(0, 0, width, height).data;
    ctx.fillRect(0, 0, width, height)

    let NewWidth = tileSize * gridX;
    let NewHeight = tileSize * gridY;

    ctx.clearRect(0, 0, ctx.width, ctx.height)
    const imgData = ctx.createImageData(NewWidth, NewHeight);

    // Implement thins.
    let XCounter = 0;
    let YCounter = 0;

    let XWrapedWidthTop = x2 - x1; 
    let XWrapedWidthBottom = x3 - x4;
    let XWrapedDiff = XWrapedWidthBottom - XWrapedWidthTop;
    let YWrapedHeightLeft = y4 - y1;
    let YWrapedHeightRight = y3 - y2;
    let YWrapedDiff = YWrapedHeightRight - YWrapedHeightLeft;

    let CurrentWidth = XWrapedWidthTop + XWrapedDiff * ((YCounter) / NewHeight);
    let CurrentPixelWidth = CurrentWidth / NewWidth;




    let YDirLeft = [(x4 - x1), (y4-y1)]
    let YDirRight = [(x3 - x2), (y3-y2)]

    let q = YDirLeft[0] ** 2;
    let q2 = YDirLeft[1] ** 2;
    let q3 = Math.sqrt(q+q2);

    let YHeightLeft = Math.sqrt(YDirLeft[0] ** 2 + YDirLeft[1] ** 2);
    let YHeightRight = Math.sqrt(YDirRight[0] ** 2 + YDirRight[1] ** 2);
    YDirLeft[0] /= NewHeight; 
    YDirLeft[1] /= NewHeight; 
    YDirRight[0] /= NewHeight; 
    YDirRight[1] /= NewHeight; 

    let currentPoints = [[x1,y1],[x2,y2]]

    let XDir = [(currentPoints[1][0] - currentPoints[0][0]), (currentPoints[1][1] - currentPoints[0][1])]
    let XWidth = Math.sqrt(XDir[0] ** 2 + XDir[1] ** 2);
    XDir[0] /= NewWidth; 
    XDir[1] /= NewWidth; 
    XWidth /= NewWidth;



    for (let i = 0; i < imgData.data.length; i += 4) {        

        let red = 0;
        let green = 0;
        let blue = 0;

        //Cal color of pixel
        XStart = currentPoints[0][0] + XCounter * XDir[0];
        XStart = Math.round(XStart)
        YStart = currentPoints[0][1] + XCounter * XDir[1];
        YStart = Math.round(YStart)

        let PixelSize = 0;
        for (let tempX = 0; tempX < Math.max(Math.round(XDir[0]),1); tempX++) {
            for (let tempY = 0; tempY < Math.max(Math.round(XDir[1]),1); tempY++) {

                let PixelIndex = Math.round((XStart + tempX + (YStart+tempY) * width) * 4)

                red += pixels[PixelIndex];
                green += pixels[PixelIndex + 1];
                blue += pixels[PixelIndex + 2];
                PixelSize++;
            }
        }

        if (PixelSize > 0) {
            red /= PixelSize;
            green /= PixelSize;
            blue /= PixelSize;
        } else {
            let h = "asd";
            h = "asd";
        }



        imgData.data[i + 0] = Math.round(red);
        imgData.data[i + 1] = Math.round(green);
        imgData.data[i + 2] = Math.round(blue);
        imgData.data[i + 3] = 255;

        XCounter++;
        if (XCounter >= NewWidth)
        {
            currentPoints[0][0] += YDirLeft[0];
            currentPoints[0][1] += YDirLeft[1];
            currentPoints[1][0] += YDirRight[0];
            currentPoints[1][1] += YDirRight[1];

            XDir = [(currentPoints[1][0] - currentPoints[0][0]), (currentPoints[1][1] - currentPoints[0][1])]
            XWidth = Math.sqrt(XDir[0] ** 2 + XDir[1] ** 2);
            XDir[0] /= NewWidth; 
            XDir[1] /= NewWidth; 
            XWidth /= NewWidth;


            XCounter = 0;
            YCounter++;
        }
    }
    ctx.putImageData(imgData, 0, 0);

    let dataUrl = canvas.toDataURL("image/jpeg");
    return dataUrl;
}


function SetDragImage(dragEvent)
{
    elem = document.getElementById("EmptyDragImage")        
    dragEvent.dataTransfer.setDragImage(elem, 0, 0);
}

function OnDrag(dragEvent) {
    elem = document.getElementById("PlayerViewImage")
    elem.style.left = dragEvent.clientX + 'px'
    elem.style.top = dragEvent.clientY + 'px'
    if (dragEvent.pageX == 0 || dragEvent.pageY == 0)
    {
        elem.style.top = '-1000px'
    }

    dragEvent.preventDefault()
    return;
}