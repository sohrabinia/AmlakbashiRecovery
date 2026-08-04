var toDatePickerSetting;

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

    if ($(e.target).closest('.advertise-list__filter-box-item').length === 0) {
        hideAllFilterItems();
    }
    if ($(e.target).closest('.advertise-list__sort-popup-parent').length === 0) {
        hideSortPopup();
    }
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

function removeDatePickerDates(reloadPage) {
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    onUpdateDate();
    if (reloadPage) {
        $('#advertise-filter-form').submit();
    }
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
                onUpdateDate();
            }
        });
    $('#main-date-picker-to').show();
}

function onUpdateDate() {
    if (firstSelectedDay == undefined && secondSelectedDay == undefined) {
        $("#use-empty-range-checkbox").removeAttr("checked");
        $("#use-empty-range-checkbox").val("false");
    }
    else {
        $("#use-empty-range-checkbox").attr("checked", "checked");
        $("#use-empty-range-checkbox").val("true");
    }
    $('#from_date_date_picker').val(firstSelectedDay == undefined ? '' : englishToPersianNumber(firstSelectedDay.date));
    $('#to_date_date_picker').val(secondSelectedDay == undefined ? '' : englishToPersianNumber(secondSelectedDay.date));
}

//function moreAdvertises(elem)
//{
//    $(elem).css('display', 'none');
//    nextPage();
//}

$(document).ready(function () {
    if (categoryId > 0) {
        myajax("category/addvisited", "catid=" + categoryId, function () { }, false);
    }

    //$.onCreate('div,a', function (elements) {
    //    elements.each(function () {
    //        if ($(this).hasClass("advertise-list-container")) {
    //            findLazyImages();
    //            //$(".ad-load-temp").remove();
    //            //$(this).children("*").each(function () {
    //            //    var new_parent = $(this).parent().parent();
    //            //    $(this).detach();
    //            //    $(this).appendTo(new_parent);
    //            //});
    //            //$(this).remove();
    //        }
    //        else if ($(this).hasClass("home-page__category-item-container-large")) {
    //            findLazyImages();
    //            $(this).addClass("advertise-category__advertise-item-container");
    //            $(this).detach();
    //            $(this).appendTo('.accommodationList');
    //        }
    //        else if ($(this).hasClass('home-page__advertise-item-container')) {
    //            $(this).find('.average-rating').barrating({
    //                theme: 'fontawesome-stars',
    //                readonly: true,
    //                initialRating: null
    //            });
    //            $(this).find(".home-page__favourite-button").click(function (e) {
    //                ToggleFavorite($(this), $(this).attr("advertise_id"));
    //                //e.stopPropagation();
    //                return false;
    //            });
    //        }
    //    });
    //}, true);
    if (categoryCity > 0) {
        $('select[name=city]').val(categoryCity);
    }
    setTimeout(function () {
        if (categoryArea > 0) {
            $('select[name=area]').val(categoryArea);
        }
    }, 0.5);

    $(".share-post__button").on("click", function () {
        $(".share-post__container").slideToggle();
        $('.share-post__container').css('display', 'flex');
    });
    //setTimeout(function () {
    //    var $capacityInput = $('#js-capacity-input');
    //    var $capacityInputParent = $capacityInput.parent();
    //    $capacityInput.insertAfter($capacityInputParent.prev());
    //    $capacityInputParent.remove();
    //}, 500);

    //var priceRangeSlider = $('#js-price-range-slider-root').wRunner({
    //    type: 'range',
    //    rangeValue: {
    //        minValue: 0,
    //        maxValue: 10000000
    //    },
    //    step: 10000,
    //    valueNoteDisplay: false,
    //    onValueUpdate: function (values) {
    //        onChangePriceMin(values.minValue);
    //        onChangePriceMax(values.maxValue);
    //    }
    //});
    //var minValue = parseInt($('#js-pricemin-input-filter').val());
    //var maxValue = parseInt($('#js-pricemax-input-filter').val());
    //var tmp = minValue;
    //minValue = maxValue;
    //maxValue = tmp;
    //if (minValue > 0)
    //    minValue = 10000000 - minValue;
    //else
    //    minValue = 30000;
    //if (maxValue > 0)
    //    maxValue = 10000000 - maxValue;
    //else
    //    maxValue = 10000000;
    //priceRangeSlider.setRangeValue({ minValue: minValue < 0 ? 30000 : minValue, maxValue: maxValue < 0 ? 10000000 : maxValue });

    if ($('.body').width() > 900) {
        $('.advertise-list__pin-filter-right').append($('.advertise-list__pin-filter-list-bottom'));
        $('.advertise-list__pin-filter-list-bottom').css('display', 'flex');
    }
});

function toggle_filter() {
    $(".dynamic__filter-container").slideToggle();
}

function nextPage() {
    $('[name=page]').val(next_page);
    $('#advertise-filter-form').submit();
}

function previousPage() {
    $('[name=page]').val(previous_page);
    $('#advertise-filter-form').submit();
}

function shareOnWathsapp(text) {
    var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    if (!isMobile) {
        window.open('https://wa.me/?text=' + text, 'whatsappShare', 'width=626,height=436'); return false;
        return false;
    }
}

function onChangeRegion() {
    var sel = document.getElementById('js-acc-type-select');
    if (sel == undefined || sel == null) {
        return;
    }
    var opts = sel.options;
    for (var opt, j = 0; opt = opts[j]; j++) {
        if (opt.value == 81) {
            sel.selectedIndex = j;
            break;
        }
    }
}

$(".public__header-text").click(function () {

    $header = $(this);
    //getting the next element
    $content = $header.next();
    //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
    $content.slideToggle(400);
    $icon = $header.find("i");
    var curr_icon = $icon.attr("class");
    if (curr_icon == "fa fa-chevron-left")
        curr_icon = "fa fa-chevron-down";
    else
        curr_icon = "fa fa-chevron-left";
    $icon.fadeTo(200, 0).promise().done(function () {

        $icon.attr("class", curr_icon).fadeTo(200, 1);
    });
});

function hideAllFilterItems() {
    $('.master__header').show();
    $('.support-chat__button').css('position', '');
    $('.present-prize__button').css('position', '');
    $('.install-app-banner').css('position', '');
    $('.support-chat__button').css('height', '');
    $('.present-prize__button').css('height', '');
    $('.install-app-banner').css('height', '');
    $('.advertise-list__filter-box-item').hide(100);
}

function onShowFilterItem() {
    if (isMobileDevice) {
        $('.master__header').hide();
        $('.support-chat__button').css('position', 'unset');
        $('.present-prize__button').css('position', 'unset');
        $('.install-app-banner').css('position', 'unset');
        $('.support-chat__button').css('height', '0');
        $('.present-prize__button').css('height', '0');
        $('.install-app-banner').css('height', '0');
    }
}

function showDateFilter() {
    onShowFilterItem();
    updateDatePicker([$('#main-date-picker')[0]], jalaliCurrentMonth,
        {
            priceDict: {},
            occupiedList: [],
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: true,
            fromDateLabel: $('#js-from-date-label')[0],
            toDateLabel: $('#js-to-date-label')[0],
            onUpdateDate: function () {
                $('#from_date_date_picker').val(firstSelectedDay == undefined ? '' : englishToPersianNumber(firstSelectedDay.date));
                $('#to_date_date_picker').val(secondSelectedDay == undefined ? '' : englishToPersianNumber(secondSelectedDay.date));
                var fromDateJalaliStr = '-';
                var toDateJalaliStr = '-';

                if (firstSelectedDay != undefined) {
                    var fromDateJalali = gregorianToJalaliDate(new Date(firstSelectedDay.value));
                    fromDateJalaliStr = englishToPersianNumber(fromDateJalali.day) + ' ' + fromDateJalali.monthString;
                }
                if (secondSelectedDay != undefined) {
                    var toDateJalali = gregorianToJalaliDate(new Date(secondSelectedDay.value));
                    toDateJalaliStr = englishToPersianNumber(toDateJalali.day) + ' ' + toDateJalali.monthString;
                }
                $('#js-from-date-label').html(fromDateJalaliStr);
                $('#js-to-date-label').html(toDateJalaliStr);
            }
        });
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    var fDate = $('#from_date_date_picker-filter').val();
    var tDate = $('#to_date_date_picker-filter').val();
    $('#from_date_date_picker').val(fDate);
    $('#to_date_date_picker').val(tDate);
    $('#js-from-date-label').html(fDate == '' ? '-' : fDate)
    $('#js-to-date-label').html(tDate == '' ? '-' : tDate)
    $('#js-date-filter-item').show(100);
}

function showCapacityFilter() {
    onShowFilterItem();
    var val = parseInt($('#js-capacity-input-filter').val());
    $('#js-capacity-input').val((isNaN(val) || val < 1) ? 0 : val);
    $('#js-capacity-filter-item').show(100);
}

//function showAccTypeFilter() {
//    onShowFilterItem();
//    var val = parseInt($('#js-acctype-input-filter').val());
//    var priorVal = parseInt($('#js-t-input-filter').val());
//    $('#js-acctype-input').val(priorVal > 0 ? priorVal : val);
//    $('#js-acctype-filter-item').show(100);
//}

function showRegionFilter() {
    onShowFilterItem();
    $('#js-region-filter-item').show(100);
}

function doSearchTableRegionFilter() {
    var provinve = $('select[name="searchTableProvince"]').val();
    var city = $('select[name="searchTableCity"]').val();
    var area = $('select[name="searchTableArea"]').val();
    $('input[name="Province"]').val(provinve);
    $('input[name="City"]').val(city);
    $('input[name="Area"]').val(area);

    $('#js-acctype-input-filter').val('-1');
    $('#js-t-input-filter').val('-1');

    $('#advertise-filter-form').submit();
}

function toggleSearchTableRegions(thisElem) {
    var $elem = $('#js-search-table-regions');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp(100);
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown(100);
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
        setTimeout(function () {
            $('#js-search-table-regions').show();
        }, 400);
    }
}

//function showPriceRangeFilter() {
//    onShowFilterItem();
//    $('#js-pricerange-filter-item').show(100);
//}

function showMoreFilter() {
    var accTypeVal = parseInt($('#js-acctype-input-filter').val());
    var priorVal = parseInt($('#js-t-input-filter').val());
    $('#js-acctype-input').val(priorVal > 0 ? priorVal : accTypeVal);

    onShowFilterItem();
    $('#js-more-filter-item').show(100);
}

function toggleAccTypeContainer(thisElem) {
    var $elem = $('#js-acctype-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

function changeAccTypeValue(elem) {
    var selectedIsSelf = $(elem).hasClass('filter-selected-item');
    $('#js-acctype-input').val(selectedIsSelf ? 81 : $(elem).attr('data-value'));
    var $parent = $(elem).parent();
    $parent.children('.advertise-list__filter-select-item').each(function () {
        $(this).removeClass('filter-selected-item');
    });
    if (!selectedIsSelf)
        $(elem).addClass('filter-selected-item');
    $('#js-ajax-filter').val(false);
}

function togglePositionsContainer(thisElem) {
    var $elem = $('#js-positions-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

function togglePriceRangeContainer(thisElem) {
    var $elem = $('#js-pricerange-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

//function onUpdatePriceSlide() {
//    $('#price-min-label').html(
//        getPriceThousandSeperatorStr($('#js-pricemin-input').val()) + " تومان");
//    $('#price-max-label').html(
//        getPriceThousandSeperatorStr($('#js-pricemax-input').val()) + " تومان");
//}

//function onChangePriceMin(val) {
//    val = 10000000 - val;
//    $('#js-pricemax-input').val(val);
//    var priceString = getPriceThousandSeperatorStr(val);
//    priceString += " تومان";
//    $('#price-max-label').html(priceString);
//}

//function onChangePriceMax(val) {
//    val = 10000000 - val;
//    if (val < 30000)
//        val = 30000;
//    $('#js-pricemin-input').val(val);
//    var priceString = getPriceThousandSeperatorStr(val);
//    priceString += " تومان";
//    $('#price-min-label').html(priceString);
//}

function toggleRoomContainer(thisElem) {
    var $elem = $('#js-room-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

function toggleFacilitiesContainer(thisElem) {
    var $elem = $('#js-facilities-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

$('.advertise-list__more-filter-expand-btn').click(function () {
    var scrollTop = $('.advertise-list__more-filter-expand-btn').scrollTop();
    var thisTop = $(this).offset().top;
    var parentTop = $('.advertise-list__more-filter-expand-btn').offset().top;

    var targetTop = scrollTop + thisTop - parentTop;

    $('.advertise-list__more-filter-parent').animate({
        scrollTop: targetTop
    }, 500);
});

function toggleRulesContainer(thisElem) {
    var $elem = $('#js-rules-container');
    var currentState = false;
    if ($elem.css('display') != 'none') {
        currentState = true;
    }
    if (currentState) {
        $elem.slideUp();
        $(thisElem).find('i').removeClass('fa-chevron-up');
        $(thisElem).find('i').addClass('fa-chevron-down');
    }
    else {
        $elem.slideDown();
        $(thisElem).find('i').removeClass('fa-chevron-down');
        $(thisElem).find('i').addClass('fa-chevron-up');
    }
}

var priceOptions = [30000, 50000, 100000, 150000, 200000,
    250000, 300000, 350000, 400000, 500000,
    600000, 700000, 800000, 900000, 1000000,
    1200000, 1400000, 1600000, 1800000,
    2000000, 2500000, 3500000, 4000000,
    5000000, 10000000];

var monthlyPriceOptions = [300000, 1000000, 2000000, 3000000,
    4000000, 5000000, 6000000, 7000000,
    8000000, 9000000, 10000000, 11000000,
    12000000, 14000000, 16000000, 18000000,
    20000000, 25000000, 30000000, 40000000,
    45000000, 50000000, 150000000];

function changePriceRangeType(newValue) {
    var currentValue = parseInt($('#js-pricerange-filter-type-input').val());
    if (currentValue == newValue)
        return;

    if (currentValue == 3 || newValue == 3) {
        var $priceMinSelect = $('#js-pricemin-input');
        var $priceMaxSelect = $('#js-pricemax-input');
        $priceMinSelect.empty();
        $priceMaxSelect.empty();
        if (newValue == 3) {
            for (var i = 0; i < monthlyPriceOptions.length; i++) {
                var priceOption = monthlyPriceOptions[i];
                $priceMinSelect.append(
                    '<option '
                    + (i == 0 ? 'selected="selected"' : '')
                    + ' value="'
                    + priceOption
                    + '">'
                    + (priceOption == 150000000 ?
                        "بیشتر از پنجاه میلیون" :
                        (getPriceThousandSeperatorStr(priceOption)
                            + " تومان"))
                    + '</option>'
                )
                $priceMaxSelect.append('<option ' + (i == monthlyPriceOptions.length - 1 ? 'selected="selected"' : '') + ' value="'
                    + priceOption
                    + '">'
                    + (priceOption == 150000000 ?
                        "بیشتر از پنجاه میلیون" :
                        (getPriceThousandSeperatorStr(priceOption))
                        + " تومان")
                    + '</option>'
                )
            }
        }
        else {
            for (var i = 0; i < priceOptions.length; i++) {
                var priceOption = priceOptions[i];
                $priceMinSelect.append('<option '
                    + (i == 0 ? 'selected="selected"' : '')
                    + ' value="'
                    + priceOption
                    + '">'
                    + getPriceThousandSeperatorStr(priceOption)
                    + " تومان"
                    + '</option>'
                )
                $priceMaxSelect.append('<option '
                    + (i == monthlyPriceOptions.length - 1 ? 'selected="selected"' : '')
                    + ' value="'
                    + priceOption
                    + '">'
                    + getPriceThousandSeperatorStr(priceOption)
                    + " تومان"
                    + '</option>'
                )
            }
        }
    }
    $('#js-pricerange-filter-type-input').val(newValue);
}

function toggleSortPopup() {
    var $elem = $('.advertise-list__sort-popup-parent');
    $elem.slideToggle(200);
}

function hideSortPopup() {
    $('.advertise-list__sort-popup-parent').slideUp(200);
}