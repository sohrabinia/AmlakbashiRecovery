setTimeout(function(){ clearSearch(true) }, 500);
function doHomePageSearch() {
    if (currentSelectedRegion != undefined &&
        currentSelectedRegion.href == undefined) {
        currentSelectedRegion = undefined;
    }
    var regionHref = currentSelectedRegion == undefined ? "/app/category/item?regiontype=-2" : currentSelectedRegion.href;
    var fromDate = firstSelectedDay == undefined ? null : firstSelectedDay.date;
    var toDate = secondSelectedDay == undefined ? null : secondSelectedDay.date;
    var guestCount = currentGuestCount > 0 ? currentGuestCount.toString() : null;

    if (guestCount > 10) {
        guestCount = 11;
    }

    var have = false;

    if (fromDate != null) {
        //if (firstQueryAdded) {
        //    regionHref += "&";
        //}
        //else {
        //    regionHref += "?";
        //}
        regionHref += "&";
        regionHref += "empty_range_from=" + fromDate;
        firstQueryAdded = true;
    }
    if (toDate != null) {
        //if (firstQueryAdded) {
        //    regionHref += "?";
        //}
        //else {
        //    regionHref += "&";
        //}
        regionHref += "&";
        regionHref += "empty_range_to=" + toDate;
        firstQueryAdded = true;
    }
    if (guestCount != null) {
        //if (firstQueryAdded) {
        //    regionHref += "?";
        //}
        //else {
        //    regionHref += "&";
        //}
        regionHref += "&";
        regionHref += "capacity=" + guestCount;
        firstQueryAdded = true;
    }
    window.location.href = regionHref;
}


$(".home-page__search-box").click(function () {
    //clearSearch();
    if (isMobileDevice) {
        toggleRegionSearchPopup(true);
        $('.home-page__search-input').focus();
    }
    else {
        toggleSearchListBox(true);
    }
    search_catrgories();
});

function toggleRegionSearchPopup(toggle) {
    if (toggle) {
        $('#js-search-region-popup').show();
    }
    else {
        $('#js-search-region-popup').hide();
    }
}

$(document).click(function (e) {
    if ($(e.target).closest('.home-page__search-box').length === 0) {
        toggleSearchListBox(false);
    }
});

function toggleSearchListBox(toggle) {
    if (toggle) {
        $(".home-page__search-list-box").show();
        $(".home-page__search-box").css("border-bottom-right-radius", "0");
        $(".home-page__search-box").css("border-bottom-left-radius", "0");
    }
    else {
        $(".home-page__search-list-box:not('.js-dont-close')").hide();
        $(".home-page__search-box").css("border-bottom-right-radius", "");
        $(".home-page__search-box").css("border-bottom-left-radius", "");
    }
}

if ($(".home-page__search-input").val() != '') {
    toggleSearchHolder(false);
}

function openFirstSearchResult() {
    var href = $(".home-page__search-list-result-container").find("a:first").attr("href");
    if (href != null)
        window.location.href = href;
}

var currentSelectedRegion = undefined;
var currentGuestCount = 0;

function toggleGuestSelect() {
    if (guestSelectShown) {
        hideGuestSelect();
    }
    else {
        showGuestSelect();
    }
}

var guestSelectShown = false;

function showGuestSelect() {
    console.log('showing guest select');
    guestSelectShown = true;
    $('.home-page__guest-select-box').css('display', 'flex');
}

function hideGuestSelect() {
    console.log('hiding guest select');
    guestSelectShown = false;
    $('.home-page__guest-select-box').css('display', 'none');
}

function selectSearchRegion(elem) {
    if (typeof elem == 'undefined' || elem == null) {
        currentSelectedRegion = undefined;
    }
    else {
        currentSelectedRegion = {
            href: $(elem).attr('data-href'),
            title: $(elem).attr('data-title')
        };
        $(".home-page__search-input").val(currentSelectedRegion.title);
        toggleSearchListBox(false);
    }
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
    //if (firstSelectedDay == undefined) {
    //    showFromDatePicker();
    //}
}

function onChangeGuestCount(elem) {
    currentGuestCount = $(elem).val();
    if (currentGuestCount < 0) {
        currentGuestCount = 0;
    }
    if (currentGuestCount == 0) {
        $('.home-page__guest-input').val('');
        return;
    }
    $('.home-page__guest-input').val(currentGuestCount + ' مهمان');
}

$('.home-page__guest-select-item').children('i').click(function () {
    onChangeGuestCount($(this).parent().find('input')[0]);
});

function search_catrgories(e) {
    if ((e != null && e.keyCode === 13)) {
        var target = $(".home-page__search-list-result-container").find("div:first");
        if (target.length > 0) {
            selectSearchRegion(target[0]);
        }
        return;
    }
    var $input;
    if (isMobileDevice) {
        $input = $('#js-mobile-region-input');
    }
    else {
        $input = $(".home-page__search-input:visible");
    }
    var search_string = $input.val();

    //if (search_string == '') {
    //    toggleSearchHolder(true);
    //    $(".home-page__search-list-result-container").empty();
    //    selectSearchRegion(null);
    //    return;
    //}
    toggleSearchHolder(false);
    if (typeof search_string == 'undefined' || search_string == null || search_string == '') {
        return;
    }
    if (search_string != '' && /^[A-Za-z]*$/.test(search_string)) {
        //$(".home-page__search-list-result-container").empty();
        $(".home-page__search-input").val(search_string.replace(/[A-Za-z]/g, ""));
        alertify.error("لطفا فارسی تایپ کنید");
        return;
    }
    var url = "/app/Category/search?phrase=" + search_string;
    console.log("searching phrase: " + url);
    $.get(url, function (data) {
        $(".home-page__search-list-result-container").html(data);
    });
    var target = $(".home-page__search-list-result-container").find("div:first");
    if (target.length > 0) {
        currentSelectedRegion = {
            href: $(target).attr('data-href'),
            title: $(target).attr('data-title')
        };
    }
}

function clearSearch(dontFocus) {
    $(".home-page__search-input").val("");
    if (!dontFocus)
        $(".home-page__search-input").focus();
    toggleSearchHolder(true);
    selectSearchRegion(null);
}

var search_holder_shown = true;

function toggleSearchHolder(toggle) {
    if (toggle) {
        $('#search_holder_root').show();
    }
    else {
        $('#search_holder_root').hide();
    }
    search_holder_shown = toggle;
}

$("span.holder").click(function () {
    $(".home-page__search-input").focus();
});

$(".home-page__search-box").click(search_catrgories);

$(document).mouseup(function (e) {
    var container = $('#main-date-picker-from');

    // if the target of the click isn't the container nor a descendant of the container
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.hide();
    }

    container = $('#main-date-picker-to');

    if (!container.is(e.target) && container.has(e.target).length === 0) {
        container.hide();
    }

    //container = $('.home-page__guest-select-box');

    //if (!container.is(e.target) && container.has(e.target).length === 0) {
    //    hideGuestSelect();
    //}
});

function showFromDatePicker() {
    currentSelectedDay = undefined;
    updateDatePicker([$('#main-date-picker-from')[0]], jalaliCurrentMonth,
    {
        priceDict: {},
        occupiedList: [],
        monthOffset: 0,
        selectionType: 'single',
        occupiedSelectEnabled: false,
        onUpdateDate: function () {
            firstSelectedDay = currentSelectedDay;
            $('#main-date-picker-from').hide();
            if (secondSelectedDay != undefined &&
                secondSelectedDay.value <= firstSelectedDay.value) {
                secondSelectedDay = undefined;
            }
            if (secondSelectedDay == undefined) {
                showToDatePicker(true);
            }
            onUpdateDate();
        }
    });
    $('#main-date-picker-from').show();
}

function removeDatePickerDates() {
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    onUpdateDate();
}

function showToDatePicker(dontClear) {
    if (!dontClear) {
        //firstSelectedDay = undefined;
        secondSelectedDay = undefined;
    }
    currentSelectedDay = undefined;
    toDatePickerSetting = updateDatePicker([$('#main-date-picker-to')[0]],
        firstSelectedDay != undefined ? gregorianToJalaliDate(new Date(firstSelectedDay.value)) : jalaliCurrentMonth,
    {
        priceDict: {},
        occupiedList: [],
        selectionType: firstSelectedDay != undefined ? 'multi' : 'single',
        occupiedSelectEnabled: false,
        onUpdateDate: function () {
            if (toDatePickerSetting.selectionType != 'multi') {
                secondSelectedDay = currentSelectedDay;
            }
            $('#main-date-picker-to').hide();
            //if (currentGuestCount == 0) {
            //    showGuestSelect();
            //}
            onUpdateDate();
        }
    });
    $('#main-date-picker-to').show();
}

function onUpdateDate() {
    if (firstSelectedDay == undefined && secondSelectedDay == undefined) {
        //$("#use-empty-range-checkbox").removeAttr("checked");
        //$("#use-empty-range-checkbox").val("false");
    }
    else {
        //$("#use-empty-range-checkbox").attr("checked", "checked");
        //$("#use-empty-range-checkbox").val("true");
    }
    $('#from_date_date_picker').val(firstSelectedDay == undefined ? '' : englishToPersianNumber(firstSelectedDay.date));
    $('#to_date_date_picker').val(secondSelectedDay == undefined ? '' : englishToPersianNumber(secondSelectedDay.date));
}

$('.home-page__search-container').find('input').each(function () {
    if ($(this)[0].parentNode.tagName.toString().toLowerCase() == 'div') {
        $(this)[0].onfocus = function () {
            $(this).parent().addClass('focused-input');
        }
        $(this)[0].onblur = function () {
            $(this).parent().removeClass('focused-input');
        }
    }
});

for (i = 0; i < $('.home-page__search-container').length; i++) {
    // you can omit the 'if' if you want to style the parent node regardless of its
    // element type

}

var isMobileDevice = $('.body').width() < 681;