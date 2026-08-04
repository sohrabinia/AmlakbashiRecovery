$(document).ready(function () {
    var positionElems = $('#js-positions-container');
    positionElems.each(function () {
        var parent = $(this);
        $(this).children().each(function () {
            $(this).click(function () {
                onClickCustomRadioItem($(this), parent.attr('multi'));
                addValueToCustomRadioButton(0);
            });
        });
    });

    var roomElems = $('#js-room-container');
    roomElems.each(function () {
        var parent = $(this);
        $(this).children().each(function () {
            $(this).click(function () {
                onClickCustomRadioItem($(this), parent.attr('multi'));
                addValueToCustomRadioButton(1);
            });
        });
    });
});

function onClickCustomRadioItem(elem, isMultiMode) {
    isMultiMode = typeof isMultiMode !== typeof undefined && isMultiMode !== false;
    if (isMultiMode) {
        if (elem.hasClass("filter-selected-item")) {
            elem.removeClass('filter-selected-item');
        }
        else {
            elem.addClass('filter-selected-item');
        }
    }
    else {
        if (elem.hasClass("filter-selected-item")) {
            elem.removeClass('filter-selected-item');
        }
        else {
            elem.parent().children().removeClass('filter-selected-item');
            elem.addClass('filter-selected-item');
        }
    }
}

function addValueToCustomRadioButton(id) {
    if (id == '0') {
        var parentElem = $('#js-positions-container');
    }
    else {
        var parentElem = $('#js-room-container');
    }

    var isMultiMode = typeof parentElem.attr('multi') !== typeof undefined;
    var value = '';
    parentElem.children('.filter-selected-item').each(function () {
        selectedValue = $(this).attr('data-value');
        if (isMultiMode) {
            if (typeof value == 'undefined' || value == null || value == '') {
                value = selectedValue;
            }
            else {
                value = value + ',' + selectedValue;
            }
        }
        else {
            value = selectedValue;
        }
    });

    if (id == 0) {
        if (value == '' || value == null || typeof value == 'undefined') {
            $('#js-position-input').attr('value', '-1');
        }
        else {
            $('#js-position-input').attr('value', value);
        }
    }
    else {
        $('#js-roomlist-input').attr('value', value);
    }
}

function changeCheckLists(elem) {
    elem = $(elem);
    var value = elem.prev().val();
    if (value == '1') {
        elem.prev().val('-1');
        elem.find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }
    else {
        elem.prev().val('1');
        elem.find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
}