// toggle item-fast-edit  
$(".container-item").css("display" , "none");
$(".show-container").css("display", "none");
$(".parent-box").css("display", "none");
var currentAccId;
function getCurrentSetPriceContainer() {
    return $('#set-price-' + currentAccId);
}
function getCurrentOfferContainer() {
    return $('#offer-' + currentAccId);
}
$('.title_setting_list').click(function () {
    var showContainer = $(this).siblings(".show-container");
    //$(".show-container").not(showContainer).hide();
    showContainer.slideToggle("slow");

    var box = $(this).siblings(".parent-box");
    box.slideToggle("slow");

    var containerItem = $(this).siblings(".container-item");
    $(".container-item").not(containerItem).hide();
    containerItem.slideToggle(function () {
        firstSelectedDay = undefined;
        secondSelectedDay = undefined;
        $(".jalali-day").removeClass("jalali-selected-day");
        $('.js-string').html('');
        var elem = $(this);

        var parentTop = elem.parent().parent();
        var clickedElOffset = elem.parent().offset().top + parentTop.scrollTop();
        var containerDivOffset = parentTop.offset().top;
        var calculatedOffset = clickedElOffset - containerDivOffset;
        parentTop.animate(
    {
        scrollTop: calculatedOffset,
    },
    300 //speed
    );
    });
    //$(this).find('.fa').toggleClass('fa-chevron-down fa-chevron-up');

    //When the container-item is opened for the first time
    //read data on server
    var initilized = containerItem.data('initialized');
    currentAccId = containerItem.attr('data-id');
    if (!initilized) {
        initializeContainerItem(containerItem);
        containerItem.data('initialized', true);
    }
});

//initializeContainerItem
//----------------------------------------------
function initializeContainerItem(elem) {
    if (elem.hasClass('js-set-date-parent')) {
        initializecalendarWindow(elem);
    } else if (elem.hasClass('js-set-price-parent')) {
        initializePriceWindow(elem);
    } else if (elem.hasClass('js-set-offer-parent')) {
        initializeOfferWindow(elem);
    }
}

//set-dat-picker-calendar
//----------------------------------------------
function initializecalendarWindow(elem) {
    updateDatePicker([$('#calendars-edit-fast-' + currentAccId)[0]], jalaliCurrentMonth,
    {
        priceDict: {},
        occupiedList: occupiedDictionary[currentAccId],
        selectionType: 'multi',
        occupiedSelectEnabled: true,
        monthOffset: 0,
        onUpdateDate: onUpdateDateCalendar
    });
    updateOccupieddates(currentAccId);
}
function onUpdateDateCalendar() {
    onUpdateDate();
    updateButton();
}
//set-date-picker-price
//----------------------------------------------
function initializePriceWindow(elem) {
    $(".jalali-loading").show();
    myajax("accomodation/getpricedict", "id=" + currentAccId, function (ret) {
        $(".jalali-loading").hide();
        if (ret.status == 0) {
            showErrorMessage('خطا', ret.msg);
        }
        else if (ret.status == 1) {
            updateDatePicker([$('#acc-set-price-' + currentAccId)[0]], jalaliCurrentMonth,
                {
                    priceDict: ret.priceDict,
                    occupiedList: [],
                    monthOffset: 0,
                    selectionType: 'multi',
                    occupiedSelectEnabled: false,
                    onUpdateDate: onUpdateDatePrice
                });
        }
    }, false);
    onUpdateDatePrice();
}
function onUpdateDatePrice() {
    if (firstSelectedDay != undefined ||
        secondSelectedDay !== undefined) {
        const inputElem = getCurrentSetPriceContainer().find(".js-price-input");
        inputElem.val('');
        onChangeInputPrice();
    }
    onUpdateDate();
    onInputPrice();
}
function onInputPrice() {
    if (firstSelectedDay == undefined) {
        getCurrentSetPriceContainer().find(".js-price-input").attr('disabled', 'true');
        getCurrentSetPriceContainer().find(".js-price-input").attr('placeholder', '');
    }
    else {

        getCurrentSetPriceContainer().find(".js-price-input").removeAttr('disabled');
        getCurrentSetPriceContainer().find(".js-price-input").attr('placeholder', 'قیمت را وارد کنید');
    }
    updateButton();
}
function onChangeInputPrice() {
    var val = getCurrentSetPriceContainer().find(".js-price-input").val();
    if (isNaN(val)) {
        val = 0;
    }
    var str = val < 1 ? '' : getPriceString(val);
    var priceLabel = getCurrentSetPriceContainer().find('.js-input-price-string');
    priceLabel.html(str);
}
$(".buttonClick").click(function () {
    if (firstSelectedDay == undefined) {
        alertify.error('ابتدا از روی جدول تاریخ را انتخاب کنید')
    }
})
//set-date-picker-offer
//----------------------------------------------
function initializeOfferWindow(elem) {
    $(".jalali-loading").show();
    myajax("accomodation/getpricedict", "id=" + currentAccId, function (ret) {
        $(".jalali-loading").hide();
        if (ret.status == 0) {
            showErrorMessage('خطا', ret.msg);
        }
        else if (ret.status == 1) {
            updateDatePicker([$('#acc-set-offer-' + currentAccId)[0]], jalaliCurrentMonth,
                {
                    priceDict: ret.priceDict,
                    occupiedList: [],
                    monthOffset: 0,
                    selectionType: 'multi',
                    occupiedSelectEnabled: false,
                    onUpdateDate: onUpdateDatePrice
                });
        }
    }, false);
    onUpdateDateOffer();
}
function onUpdateDateOffer() {
    onUpdateDate();
    updateButton();
}
//--------setting-datePicker------
//----------------------------------------------
function onUpdateDate() {
    if (firstSelectedDay != undefined &&
    secondSelectedDay != undefined) {
        var startDay = gregorianToJalaliDate(new Date(firstSelectedDay.value)).dayOfWeekString;
        var endDay = gregorianToJalaliDate(new Date(secondSelectedDay.value)).dayOfWeekString;
        var DayString = gregorianToJalaliDate(new Date(firstSelectedDay.value)).day;
        var toDayString = gregorianToJalaliDate(new Date(secondSelectedDay.value)).day;
        var month = gregorianToJalaliDate(new Date(firstSelectedDay.value)).month;
        var tomonth = gregorianToJalaliDate(new Date(secondSelectedDay.value)).month;

        $('.js-string').html(`از  ${startDay} ${month}/${DayString}  تا   ${endDay} ${tomonth}/${toDayString}`);
    }
    else {
        $('.js-string').html('');
    }
}
function updateButton() {
    if (firstSelectedDay == undefined) {
        if (!($('.set-button_on-calendar').hasClass('deactive-set-button'))) {
            $('.set-button_on-calendar').addClass('deactive-set-button');
        }
    }
    else {
        $('.set-button_on-calendar').removeClass('deactive-set-button');
    }
}

var currentMonth;
var from_date;
var to_date;
function calculateRange() {
    from_date = firstSelectedDay.date.replaceAll('/', ',');
    if (secondSelectedDay != undefined) {
        currentMonth = gregorianToJalaliDate(new Date(secondSelectedDay.value));
        var nextDay = new Date(secondSelectedDay.value);
        nextDay.setDate(nextDay.getDate() + 1);
        nextDay = gregorianToJalaliDate(nextDay);
        to_date = nextDay.dateString.replaceAll('/', ',');
    }
    else {
        currentMonth = gregorianToJalaliDate(new Date(firstSelectedDay.value));
        var nextDay = new Date(firstSelectedDay.value);
        nextDay.setDate(nextDay.getDate() + 1);
        nextDay = gregorianToJalaliDate(nextDay);
        to_date = nextDay.dateString.replaceAll('/', ',');
    }
}

function updateOccupieddates(id) {
    myajax("accomodation/getoccupieddates", "id=" + id, function (ret) {
        if (ret.status == 1) {
            updateDatePicker([$('#calendars-edit-fast-' + id)[0]], jalaliCurrentMonth,
            {
                priceDict: {},
                occupiedList: ret.occupiedList,
                monthOffset: 0,
                selectionType: 'multi',
                occupiedSelectEnabled: true,
                onUpdateDate: onUpdateDateCalendar
            });
        }
    }, false);
}

