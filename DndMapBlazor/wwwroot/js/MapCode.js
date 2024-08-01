window.GetElementSize = (elementId) => {
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

function FullScreen(id)
{
    const screenElement = document.getElementById(id);
    screenElement.requestFullscreen();
}