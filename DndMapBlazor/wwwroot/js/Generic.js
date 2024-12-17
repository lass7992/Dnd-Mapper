function ScrollNextCardHolderOnClick()
{
    document.getElementById("cardHolderScroller").scrollBy(400, 0);
}

function ScrollPrevCardHolderOnClick() {
    document.getElementById("cardHolderScroller").scrollBy(-400, 0);
}

function isOverflown(id) {
    element = document.getElementById(id);
    return element.scrollHeight > element.clientHeight || element.scrollWidth > element.clientWidth;
}

function ChangeVisibilityOfCarosel()
{
    let isOverflow = isOverflown('cardHolderScroller');
    CaroselButtonHolder = document.getElementById('CaroselButtonHolder');

    if (isOverflow) {
        CaroselButtonHolder.style.setProperty('visibility', 'visible');
    } else {
        CaroselButtonHolder.style.setProperty('visibility', 'hidden');
    }
}

function Transition() {
    document.body.style.opacity = '0';
    document.body.style.transition = 'none';

    setTimeout(
        function () {
            document.body.style.transition = 'opacity 2s';
            document.body.style.opacity = '1';
        }, 10);
}