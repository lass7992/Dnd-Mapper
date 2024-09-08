﻿window.GetElementClientSize = (elementId) => {
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
            video.style.webkitTransform = "scaleX(-1)";
            video.style.transform = "scaleX(-1)";
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

function getWarpedFrame(src, dest, x1, y1, x2, y2, x3, y3, x4, y4 ) {
    let gridX = 10;
    let gridY = 7;

    let video = document.getElementById(src);
    let canvas = document.getElementById(dest);

    let ctx = canvas.getContext('2d');
    const imgData = ctx.createImageData(16 * gridX, 16 * gridY);

    // Implement thins.
    for (let i = 0; i < imgData.data.length; i += 4) {

        imgData.data[i + 0] = 255;
        imgData.data[i + 1] = 0;
        imgData.data[i + 2] = 0;
        imgData.data[i + 3] = 255;
    }


    ctx.fillRect(0, 0, 320, 240);
    ctx.save();
    ctx.transform(scaleX, skewX, skewY , scaleY, 0, 0);
    ctx.drawImage(video, 0, 0, 320, 240);
    ctx.restore();

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