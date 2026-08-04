function onClickIncDecButton(elem) {
    var $button = $(elem);
    var $inputElement;
    if ($button.attr("data-action") === "plus") {
        $inputElement = $button.next();
    }
    else {
        $inputElement = $button.prev();
    }
    var min = $inputElement.attr("min");
    var max = $inputElement.attr("max");
    var oldValue = $inputElement.val();
    if (oldValue == '')
        oldValue = 0;

    if ($button.attr("data-action") === "plus") {
        if (max == null || parseInt(max) >= parseInt(oldValue) + 1) {
            $inputElement.val(parseInt(oldValue) + 1);
        }
    } else {
        if (min == null || parseInt(min) <= parseInt(oldValue) - 1) {
            $inputElement.val(parseInt(oldValue) - 1);
        }
    }
}

function clampNumberInput(elem) {
    if ($(elem).attr('min') != null) {
        if ($(elem).val() < parseInt($(elem).attr('min'))) {
            $(elem).val($(elem).attr('min'));
        }
        if ($(elem).val() > parseInt($(elem).attr('max'))) {
            $(elem).val($(elem).attr('max'));
        }
    }
}