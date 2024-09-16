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
    canvas.width = 640
    canvas.height = 480
    canvas.getContext('2d').drawImage(video, 0, 0, 640, 480 );

    let dataUrl = canvas.toDataURL("image/jpeg");
    return dataUrl;
}

function getWarpedFrame(src, dest, gridX, gridY, x1, y1, x2, y2, x3, y3, x4, y4, perspecWidth) {
    let tileSize = 32

    let video = document.getElementById(src);
    let width = video.offsetWidth
    let height = video.offsetHeight


    let canvas = document.getElementById(dest);
    let ctx = canvas.getContext('2d');

    canvas.width = width
    canvas.height = height


    ctx.drawImage(video, 0, 0, width, height);
    let pixels = ctx.getImageData(0, 0, width, height).data;
//    ctx.clearRect(0, 0, ctx.width, ctx.height);


    let NewWidth = tileSize * gridX;
    let NewHeight = tileSize * gridY;

    ctx.clearRect(0, 0, ctx.width, ctx.height)
    const imgData = ctx.createImageData(NewWidth, NewHeight);

    let XCounter = 0;
    let YCounter = 0;

    // SetUp YDir
    let YDirLeft = [(x4 - x1), (y4-y1)]
    let YDirRight = [(x3 - x2), (y3 - y2)]
    let YDiffPro = GetDiffPro(YDirLeft, YDirRight);
    YDirLeft[0] /= NewHeight; 
    YDirLeft[1] /= NewHeight; 
    YDirRight[0] /= NewHeight; 
    YDirRight[1] /= NewHeight; 

    //Setup XDir
    let currentPoints = [[x1,y1],[x2,y2]]
    let XDir = [(currentPoints[1][0] - currentPoints[0][0]), (currentPoints[1][1] - currentPoints[0][1])]
    let XDirBot = [(x4 - x3), (y4 - y3)]
    let XDiffPro = GetDiffPro(XDir, XDirBot);
    XDir[0] /= NewWidth;
    XDir[1] /= NewWidth; 


        //Get perspective
    let XPerspective, YDirLeftPerspective, YDirRightPerspective;
    [XDir, XPerspective] = GetPerspective(XDir, YDiffPro, NewWidth);
    [YDirLeft, YDirRight, YDirLeftPerspective, YDirRightPerspective] = GetPerspectives(YDirLeft,YDirRight,XDiffPro, NewHeight);

    for (let i = 0; i < imgData.data.length; i += 4) {        
        XDir[0] = XDir[0] + XPerspective[0];
        XDir[1] = XDir[1] + XPerspective[1];


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
        }

        imgData.data[i + 0] = Math.round(red);
        imgData.data[i + 1] = Math.round(green);
        imgData.data[i + 2] = Math.round(blue);
        imgData.data[i + 3] = 255;

        XCounter++;
        if (XCounter >= NewWidth)
        {
            XCounter = 0;
            YCounter++;

            //Add perspective to ydir;
            YDirLeft[0] = YDirLeft[0] + YDirLeftPerspective[0];
            YDirLeft[1] = YDirLeft[1] + YDirLeftPerspective[1];
            YDirRight[0] = YDirRight[0] + YDirRightPerspective[0];
            YDirRight[1] = YDirRight[1] + YDirRightPerspective[1]; 

            //Add Ydir to currentPoints
            currentPoints[0][0] += YDirLeft[0];
            currentPoints[0][1] += YDirLeft[1];
            currentPoints[1][0] += YDirRight[0];
            currentPoints[1][1] += YDirRight[1];

            //Calculate new YDir
            XDir = [(currentPoints[1][0] - currentPoints[0][0]), (currentPoints[1][1] - currentPoints[0][1])]
            XDir[0] /= NewWidth;
            XDir[1] /= NewWidth; 
            [XDir, XPerspective] = GetPerspective(XDir, YDiffPro, NewWidth);
        }
    }

    ctx.putImageData(imgData, 0, 0);

    //SplitImage into Parts
    let newPixels = ctx.getImageData(0, 0, NewWidth, NewHeight).data;
    let DataArray = []


    canvas.width = tileSize
    canvas.height = tileSize
    
    for (let gx = 0; gx < gridX; gx++)
    {
        let startX = gx * tileSize
        for (let gy = 0; gy < gridY; gy++)
        {
            const imgData = ctx.createImageData(tileSize, tileSize);
            let pixelCount = tileSize * tileSize;


            let startY = gy * tileSize

            let countX = 0;
            let countY = 0;

            for (let i = 0; i < pixelCount; i++)
            {
                let index = ((startX + countX) + (startY + countY) * NewWidth) * 4
                let newIndex = (countX + countY * tileSize) * 4;
                imgData.data[newIndex] = newPixels[index];
                imgData.data[newIndex + 1] = newPixels[index+1];
                imgData.data[newIndex + 2] = newPixels[index+2];
                imgData.data[newIndex + 3] = 255;

                countX += 1;
                if (countX >= tileSize)
                {
                    countX = 0;
                    countY += 1;
                }
            }
            ctx.putImageData(imgData, 0, 0);
            let dataUrl = canvas.toDataURL("image/jpeg");
            DataArray.push(dataUrl);
        }
    }

    return DataArray;
}

function GetDiffPro(Dir1, Dir2)
{
    let Width1 = Math.sqrt(Dir1[0] ** 2 + Dir1[1] ** 2);
    let Width2 = Math.sqrt(Dir2[0] ** 2 + Dir2[1] ** 2);
    let Diff = Width2 - Width1;
    let DiffPro = (Diff / Width2) * 100;
    return DiffPro
}

function GetPerspectives(Dir1, Dir2, DiffPro , endImageWidth)
{
    DiffPro *= 2

    let Perspective1 = [Dir1[0] / 100 * DiffPro, Dir1[1] / 100 * DiffPro]
    let Perspective2 = [Dir2[0] / 100 * DiffPro, Dir2[1] / 100 * DiffPro]

    // scale ydir with perspective    
    Dir1[0] -= Perspective1[0] * 0.5;
    Dir1[1] -= Perspective1[1] * 0.5;
    Dir2[0] -= Perspective2[0] * 0.5;
    Dir2[1] -= Perspective2[1] * 0.5;

    // Scale perspective with steps
    let TotalPerspectiveSteps = 0
    for (let i = 1; i <= endImageWidth; i++) {
        TotalPerspectiveSteps += 1;
    }
    Perspective1[0] /= TotalPerspectiveSteps;
    Perspective1[1] /= TotalPerspectiveSteps;
    Perspective2[0] /= TotalPerspectiveSteps;
    Perspective2[1] /= TotalPerspectiveSteps;

    return [Dir1, Dir2, Perspective1, Perspective2]
}

function GetPerspective(Dir, DiffPro, endImageWidth) {
    DiffPro *= 2

    let Perspective = [Dir[0] / 100 * DiffPro, Dir[1] / 100 * DiffPro]

    // scale ydir with perspective    
    Dir[0] -= Perspective[0] * 0.5;
    Dir[1] -= Perspective[1] * 0.5;

    // Scale perspective with steps
    let TotalPerspectiveSteps = 0
    for (let i = 1; i <= endImageWidth; i++) {
        TotalPerspectiveSteps += 1;
    }
    TotalPerspectiveSteps *= 2; //Apparantly, i dunno why

    Perspective[0] /= TotalPerspectiveSteps;
    Perspective[1] /= TotalPerspectiveSteps;

    return [Dir, Perspective]
}