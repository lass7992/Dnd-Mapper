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