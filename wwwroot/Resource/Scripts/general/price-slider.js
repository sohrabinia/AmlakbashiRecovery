sliders = [document.getElementById('js-pricemin-input'),
    document.getElementById('js-pricemax-input')];
function updatePriceLabels() {
    //avoids slider overlap var sliders
    document.querySelectorAll(".price-slider input");
    var val1 = parseInt(sliders[0].value);
    var val2 = parseInt(sliders[1].value);
    if (val1 >= val2) { sliders[0].value = val2 - 3; return; }
    if (val2 <= val1) { sliders[1].value = val1 + 3; return; }
    //adds color when a range is selected
    if (val1 > 0 || val2 < 99) {
        sliders[0].style.background =
            sliders[1].style.background =
            "-webkit-gradient(linear, 0 0,100% 0, color-stop(0, white), color-stop(" +
            (val1 / 100) + ", white),color-stop(" + (val1 / 100) +
            ", #f0f0f0), color-stop(" + (val2 / 100) + ", #f0f0f0), color-stop(" +
            (val2 / 100) + ", white))";
    }
    else {
        sliders[0].style.background = sliders[1].style.background = '';
    }
    if (typeof onUpdatePriceSlide != 'undefined')
        onUpdatePriceSlide();
}

//function updatePriceLabels() {
//    console.log("price slider min: " + $('#js-price-min').val() + " max: " + $('#js-price-max').val());
//}