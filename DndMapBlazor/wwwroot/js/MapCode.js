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