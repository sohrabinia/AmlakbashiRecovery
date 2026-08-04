var from_occupied_datesDt = [];
var to_occupied_datesDt = [];
var date_price_dict = {};
var to_date_objectDt, from_date_objectDt;
$(document).ready(function () {
    updateDatePickersDet();
    initializeDatePicker();
});

var datePickerInitialized = false;

function initializeDatePicker() {
    if (datePickerInitialized)
        return;
    datePickerInitialized = true;
    myajax('accomodation/getdatepickerdynamicviewbagold', 'id=' + advertise_id,
        function (ret) {
            fillReserveInfo(advertise_id, ret.from_occupied_dates,
            ret.to_occupied_dates, (typeof rules_string == "undefined" ? null : rules_string));
            date_price_dict = ret.priceList;
            update_datetime_tableDt(to_occupied_datesDt);
        }, false);
}

//document.addEventListener("DOMContentLoaded", function () {    
//});
function updateDatePickersDet(onDone) {
    if ($('#to_date_date_pickerDt').length && $('#from_date_date_pickerDt').length) {
        to_date_objectDt = $("#to_date_date_pickerDt").persiandatepickerDt({
            altField: '#to_date-alt',
            format: 'YYYY/MM/DD',
            minDate: new persianDate() + day_milliseconds,
            altFormat: 'YYYY/MM/DD',
            autoClose: true,
            toolbox: {
                calendarSwitch: { enabled: false },
                todayButton: { enabled: false },
                submitButton: { enabled: true, text: { fa: "بستن", en: close } }
            },
            monthPicker: {
                onSelect: function () { update_datetime_tableDt(to_occupied_datesDt); }
            },
            navigator: {
                onNext: function () { update_datetime_tableDt(to_occupied_datesDt); },
                onPrev: function () { update_datetime_tableDt(to_occupied_datesDt); },
                onSwitch: function () { update_datetime_tableDt(to_occupied_datesDt); },
                scroll: { enabled: false },
                text: { btnNextText: '>', btnPrevText: '<' },
            },
            yearPicker: {
                onSelect: function () { update_datetime_tableDt(to_occupied_datesDt); }
            },
            onShow: function () {
                //if (!datePickerInitialized) {
                //    initializeDatePicker();
                //}
                //else {
                    update_datetime_tableDt(to_occupied_datesDt);
                //};
            },
            initialValue: true,
            onSelect: function (unix) {
                to_date_objectDt.touched = true;
                if (from_date_objectDt && from_date_objectDt.options && from_date_objectDt.options.maxDate != unix) {
                    var cachedValue = from_date_objectDt.getState().selected.unixDate;
                    if (from_date_objectDt.touched) {
                        from_date_objectDt.setDate(cachedValue);
                    }
                }
                if (IsDateRangeIncludeOccupiedDaysDt(from_date_objectDt.getState().selected.unixDate, to_date_objectDt.getState().selected.unixDate)) {
                    alertify.error("محدوده زمانی انتخاب شده شامل روز های رزرو شده میباشد");
                }
                //if(updateReservePrice != undefined && updateReservePrice != null) {
                //    updateReservePrice();
                //}
                //if(updateReserveLabels != undefined && updateReserveLabels != null) {
                //    updateReserveLabels();
                //}
                if (typeof onUpdateDate !== "undefined") {
                    onUpdateDate();
                }
            }
        });
        from_date_objectDt = $("#from_date_date_pickerDt").persiandatepickerDt({
            altField: '#from_date-altDt',
            minDate: new persianDate(),
            format: 'YYYY/MM/DD',
            altFormat: 'YYYY/MM/DD',
            autoClose: true,
            toolbox: {
                calendarSwitch: { enabled: false },
                todayButton: { enabled: false },
                submitButton: { enabled: true, text: { fa: "بستن", en: close } }
            },
            monthPicker: {
                onSelect: function () { update_datetime_tableDt(from_occupied_datesDt); }
            },
            navigator: {
                onNext: function () { update_datetime_tableDt(from_occupied_datesDt); },
                onPrev: function () { update_datetime_tableDt(from_occupied_datesDt); },
                onSwitch: function () { update_datetime_tableDt(from_occupied_datesDt); },
                scroll: { enabled: false },
                text: { btnNextText: '>', btnPrevText: '<' },
            },
            yearPicker: {
                onSelect: function () { update_datetime_tableDt(from_occupied_datesDt); }
            },
            onShow: function () {
                //if (!datePickerInitialized) {
                //    initializeDatePicker();
                //}
                //else {
                    update_datetime_tableDt(from_occupied_datesDt);
                //};
            },
            onSelect: function (unix) {
                var $instantReserveInfo = $('#js-instant-reserve-info');
                if ($instantReserveInfo.length && instantReserveActivated) {
                    var $normalReserveInfo = $('#js-normal-reserve-info');
                    var todayUnix = new Date().getTime();
                    if (unix <= (todayUnix + (day_milliseconds * instantReserveMaxStart))) {
                        instantReserveAvailable = true;
                        $instantReserveInfo.show();
                        $normalReserveInfo.hide();
                        $('#js-reserve-req-title').html('رزرو');
                    }
                    else {
                        instantReserveAvailable = false;
                        $normalReserveInfo.show();
                        $instantReserveInfo.hide();
                        $('#js-reserve-req-title').html('درخواست رزرو');
                    }
                }
                from_date_objectDt.touched = true;
                if (to_date_objectDt && to_date_objectDt.options && to_date_objectDt.options.minDate != unix) {
                    var cachedValue = to_date_objectDt.getState().selected.unixDate;
                    to_date_objectDt.options = { minDate: unix + day_milliseconds };
                    if (to_date_objectDt.touched) {
                        to_date_objectDt.setDate(cachedValue);
                    }
                    if (cachedValue < unix + day_milliseconds) {
                        to_date_objectDt.setDate(unix + day_milliseconds);
                    }
                }
                if (IsDateRangeIncludeOccupiedDaysDt(from_date_objectDt.getState().selected.unixDate, to_date_objectDt.getState().selected.unixDate)) {
                    alertify.error("محدوده زمانی انتخاب شده شامل روز های رزرو شده میباشد");
                }
                //if(updateReservePrice != undefined && updateReservePrice != null) {
                //    updateReservePrice();
                //}
                //if(updateReserveLabels != undefined && updateReserveLabels != null) {
                //    updateReserveLabels();
                //}
                if (typeof onUpdateDate !== "undefined") {
                    onUpdateDate();
                }
            }
        });
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
}
function update_datetime_tableDt(occupiedDays, $element) {
    occupiedDays = JSON.stringify(occupiedDays);
    var $all_labels;
    if ($element != null) {
        $all_labels = $element.children();
    }
    else {
        var $all_labels = $(".Dtdatepicker-container").children();
    }
    $all_labels.each(function () {
        if ($.trim($(this).html())) {
            $(this).css("font", "12px Miransans");
        }
    });
    var $date_time;
    if ($element != null) {
        $date_time = $element.find("[data-unix]");
    }
    else {
        $date_time = $("[data-unix]");
    }
    $date_time.each(function () {
        var persian_date = $(this).attr("data-date");
        $label = $(this).children();
        if (date_price_dict != undefined && date_price_dict[persian_date]) {
            $label.next().html(priceSpecialString(date_price_dict[persian_date]));
        }
        var is_other_month = $label.hasClass("other-month");
        //$label.css("font", "12px Miransans");
        if ($(this).attr("data-unix") < new Date().getTime() - (24 * 60 * 60)) {
        }
        else if (occupiedDays != undefined && (occupiedDays.includes(persian_date + "\\") ||
            occupiedDays.includes(persian_date + "\""))) {
            $(this).addClass("occupied");
            $label.addClass("occupied_day_label");
            $label.css("background-color", is_other_month ? "#f2f2f2" : "lightgray");
            $label.css("color", is_other_month ? "#727272" : "#242424");
        }
        else {
            $(this).removeClass("occupied");
            $label.removeClass("occupied_day_label");
            $label.css("background-color", "white");
            $label.css("color", is_other_month ? "#c1c1c1" : "#242424");
        }
        if (dateIsHoliday(persian_date)) {
            $label.css("color", is_other_month ? "#ffadad" : "red");
        }
        if (is_other_month) {
            //alertify.error($(this).attr("data-date"));
            //$(this).removeAttr("data-date");
            //$(this).removeAttr("data-unix");
            $(this).addClass("disabled");
        }
        var $next_elem = $(this).next();
        if ($next_elem.length > 0 && persian_date == '1398,1,1' && $next_elem.attr('data-date') == '1397,12,2') {
            $(this).attr('data-date', '1397,12,1');
            $(this).attr('data-unix', persianDateToUnix('1397,12,1'));
        }
    });
}
function IsDateRangeIncludeOccupiedDaysDt(unix_from, unix_to) {
    var from_dates = JSON.stringify(from_occupied_datesDt);
    var to_dates = JSON.stringify(to_occupied_datesDt);
    var from_day_array = getDayArrayBetweenUnixes(unix_from, unix_to - day_milliseconds);
    var to_day_array = getDayArrayBetweenUnixes(unix_from + day_milliseconds, unix_to);
    for (var i = 0; i < from_day_array.length; i++) {
        if (from_dates.includes(from_day_array[i] + "\\") ||
            from_dates.includes(from_day_array[i] + "\"")) {
            return true;
        }
    }
    for (var i = 0; i < to_day_array.length; i++) {
        if (to_dates.includes(to_day_array[i] + "\\") ||
            to_dates.includes(to_day_array[i] + "\"")) {
            return true;
        }
    }
    return false;
}

function priceSpecialString(price) {
    var priceStr = price.toString();
    if (priceStr.length > 9) {
        var temp = price / 1000000000;
        return temp.toString() + " میلیارد";
    }
    else if (priceStr.length > 6) {
        var temp = price / 1000000;
        return temp.toString() + " میلیون";
    }
    else if (priceStr.length > 3) {
        var temp = price / 1000;
        return temp.toString() + " هزار";
    }
    else {
        return priceStr;
    }
}