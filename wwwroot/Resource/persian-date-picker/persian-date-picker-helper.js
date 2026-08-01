var from_occupied_dates = [];
var to_occupied_dates = [];
var to_date_object, from_date_object;
$(document).ready(function () {
    updateToFromDatePickers();
});
//document.addEventListener("DOMContentLoaded", function () {
    
//});
function updateToFromDatePickers(onDone) {
    if ($('#to_date_date_picker').length && $('#from_date_date_picker').length) {
        to_date_object = $("#to_date_date_picker").persianDatepicker({
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
                onSelect: function () { update_datetime_table(to_occupied_dates); }
            },
            navigator: {
                onNext: function () { update_datetime_table(to_occupied_dates); },
                onPrev: function () { update_datetime_table(to_occupied_dates); },
                onSwitch: function () { update_datetime_table(to_occupied_dates); },
                scroll: { enabled: false },
                text: { btnNextText: '>', btnPrevText: '<' },
            },
            yearPicker: {
                onSelect: function () { update_datetime_table(to_occupied_dates); }
            },
            onShow: function () { update_datetime_table(to_occupied_dates); },
            initialValue: true,
            onSelect: function (unix) {
                to_date_object.touched = true;
                if (from_date_object && from_date_object.options && from_date_object.options.maxDate != unix) {
                    var cachedValue = from_date_object.getState().selected.unixDate;
                    if (from_date_object.touched) {
                        from_date_object.setDate(cachedValue);
                    }
                }
                if (IsDateRangeIncludeOccupiedDays(from_date_object.getState().selected.unixDate, to_date_object.getState().selected.unixDate)) {
                    if (!allowSelectDisabled) {
                        alertify.error("محدوده زمانی انتخاب شده شامل روز های رزرو شده میباشد");
                    }
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
        from_date_object = $("#from_date_date_picker").persianDatepicker({
            altField: '#from_date-alt',
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
                onSelect: function () { update_datetime_table(from_occupied_dates); }
            },
            navigator: {
                onNext: function () { update_datetime_table(from_occupied_dates); },
                onPrev: function () { update_datetime_table(from_occupied_dates); },
                onSwitch: function () { update_datetime_table(from_occupied_dates); },
                scroll: { enabled: false },
                text: { btnNextText: '>', btnPrevText: '<' },
            },
            yearPicker: {
                onSelect: function () { update_datetime_table(from_occupied_dates); }
            },
            onShow: function () { update_datetime_table(from_occupied_dates); },
            onSelect: function (unix) {
                from_date_object.touched = true;
                if (to_date_object && to_date_object.options && to_date_object.options.minDate != unix) {
                    var cachedValue = to_date_object.getState().selected.unixDate;
                    to_date_object.options = { minDate: unix + day_milliseconds };
                    if (to_date_object.touched) {
                        to_date_object.setDate(cachedValue);
                    }
                    if (cachedValue < unix + day_milliseconds) {
                        to_date_object.setDate(unix + day_milliseconds);
                    }
                }
                if (IsDateRangeIncludeOccupiedDays(from_date_object.getState().selected.unixDate, to_date_object.getState().selected.unixDate)) {
                    if (!allowSelectDisabled) {
                        alertify.error("محدوده زمانی انتخاب شده شامل روز های رزرو شده میباشد");
                    }
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
function update_datetime_table(occupiedDays, $element) {
    occupiedDays = JSON.stringify(occupiedDays);
    var $all_labels;
    if ($element != null) {
        $all_labels = $element.children();
    }
    else {
        var $all_labels = $(".datepicker-container").children();
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
        var is_other_month = $label.hasClass("other-month");
        //$label.css("font", "12px Miransans");
        if ($(this).attr("data-unix") < new Date().getTime() - (24 * 60 * 60)) {
        }
        else if (occupiedDays.includes(persian_date + "\\") ||
            occupiedDays.includes(persian_date + "\"")) {
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
function IsDateRangeIncludeOccupiedDays(unix_from, unix_to) {
    var from_dates = JSON.stringify(from_occupied_dates);
    var to_dates = JSON.stringify(to_occupied_dates);
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