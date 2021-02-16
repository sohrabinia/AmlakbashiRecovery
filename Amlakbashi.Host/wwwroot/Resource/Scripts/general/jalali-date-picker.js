var jalaliToday = getJalaliToday();
var jalaliCurrentMonth = jalaliToday;

var currentSelectedDay;
var firstSelectedDay;
var secondSelectedDay;
var firstOccupiedValue;

//elems: date picker elements
//month: initial jalali month
//setting: priceDict (dict), occupiedList (arr),
//monthOffset (int), selectionType (single, multi),
//occupiedSelectEnabled (bool), fromDateLabel (elem)
//toDateLabel (elem), onUpdateDate(function)
//maxSupportedMonth (jalali month)

function updateDatePicker(elems, month, setting) {
    updateDatePickerSeek(elems, month, setting);
    updateMonthYearLabel(elems, month);
    updateDatePickerDays(elems, month);
    updateDatePickerPrices(elems, setting);
    updateDatePickerOccupied(elems, setting);
    if (firstSelectedDay != undefined) {
        updateRangeSelection(elems, setting);
    }
    return setting;
}

function selectDate(elems, dayElem, setting) {
    var $dayElem = $(dayElem);
    if ($dayElem.hasClass('jalali-disabled-day')) {
        return;
    }
    var value = parseInt($dayElem.attr('data-value'));
    price = setting.priceDict[value];
    if (price == undefined) {
        price = {price: 0, off: 0};
    }
    var dayObj = {
        date: $dayElem.attr('data-date'),
        value: value,
        elem: dayElem,
        occupied: setting.occupiedList && setting.occupiedList.includes(value),
        price: price
    }
    if (setting.selectionType == 'multi') {
        selectDateMulti(elems, dayObj, setting);
        return;
    }
    selectDateSignle(dayObj, setting);
}
 
function selectDateSignle(dayObj, setting) {
    if (currentSelectedDay) {
        $(currentSelectedDay.elem).removeClass('jalali-selected-day');
    }
    if (currentSelectedDay != undefined &&
        currentSelectedDay.value == dayObj.value) {
        currentSelectedDay = undefined;
    }
    else {
        currentSelectedDay = dayObj;
        $(dayObj.elem).addClass('jalali-selected-day');
    }
    if (setting.onUpdateDate) {
        setting.onUpdateDate();
    }
}

function selectDateMulti(elems, dayObj, setting) {
    if ((firstSelectedDay == undefined &&
        secondSelectedDay == undefined) ||
        (firstSelectedDay != undefined &&
        secondSelectedDay != undefined))
    {
        secondSelectedDay = undefined;
        firstSelectedDay = dayObj;
        firstOccupiedValue = Math.min.apply(null, setting.occupiedList.filter(function (x) { return x > firstSelectedDay.value }));
        if (setting.fromDateLabel != undefined) {
            $(setting.fromDateLabel).html(dayObj.date.substring(2));
        }
        if (setting.toDateLabel != undefined) {
            $(setting.toDateLabel).html('');
        }
    }
    else {
        if (dayObj.value <= firstSelectedDay.value) {
            firstSelectedDay = undefined;
            if (setting.fromDateLabel != undefined) {
                $(setting.fromDateLabel).html('');
            }
        }
        else {
            secondSelectedDay = dayObj;
            if (setting.toDateLabel != undefined) {
                $(setting.toDateLabel).html(dayObj.date.substring(2));
            }
        }
        firstOccupiedValue = undefined;
    }
    updateRangeSelection(elems, setting);
    if (setting.onUpdateDate) {
        setting.onUpdateDate();
    }
}

function updateRangeSelection(elems, setting) {
    var maxValue = Number.MAX_VALUE;
    if (!setting.occupiedSelectEnabled &&
        firstOccupiedValue != undefined) {
        maxValue = firstOccupiedValue;
    }
    elems.forEach(function (elem) {
        $(elem).find('.jalali-real-day').each(function () {
            var value = parseInt($(this).attr('data-value'));
            if (firstSelectedDay == undefined) {
                $(this).removeClass('jalali-selected-day');
                if (($(this).hasClass('jalali-occupied-day'))) {
                    if (!setting.occupiedSelectEnabled &&
                        !($(this).hasClass('jalali-disabled-day'))) {
                        $(this).addClass('jalali-disabled-day');
                    }
                }
                else {
                    if (!($(this).hasClass('jalali-past-day'))) {
                        $(this).removeClass('jalali-disabled-day');
                    }
                }
                if ($(this).hasClass('jalali-occupied-day-temp')) {
                    $(this).addClass('jalali-occupied-day');
                    $(this).addClass('jalali-disabled-day');
                    $(this).removeClass('alali-occupied-day-temp');
                    $(this).find('.jalali-reserved-label').remove();
                    $(this).append('<span class="jalali-reserved-label">رزرو شده</span>');
                    $(this).off('click');
                    $(this).off('hover');
                }
            }
            else {
                if (secondSelectedDay == undefined) {
                    if (value == firstSelectedDay.value) {
                        $(this).addClass('jalali-selected-day');
                    }
                    else {
                        $(this).removeClass('jalali-selected-day');
                    }
                    if (!setting.occupiedSelectEnabled) {
                        if (value == maxValue) {
                            $(this).removeClass('jalali-occupied-day');
                            $(this).removeClass('jalali-disabled-day');
                            $(this).find('.jalali-reserved-label').remove();
                            $(this).find('.jalali-price-label').remove();

                            price = setting.priceDict[$(this).attr('data-value')];
                            if (price) {
                                $(this).append('<span class="jalali-price-label">' + priceToSpecialString(price.price) + '</span>');
                            }
                            $(this).click(function () {
                                selectDate(elems, this, setting);
                            });
                            $(this).hover(function () {
                                handleDayHover(elems, this, setting);
                            });
                            $(this).addClass('jalali-occupied-day-temp');
                        }
                        else if (value > maxValue) {
                            $(this).addClass('jalali-disabled-day');
                        }
                    }
                }
                else {
                    if (value >= firstSelectedDay.value &&
                        value <= secondSelectedDay.value) {
                        $(this).addClass('jalali-selected-day');
                    }
                    else {
                        $(this).removeClass('jalali-selected-day');
                    }
                    if (($(this).hasClass('jalali-occupied-day'))) {
                        if (!setting.occupiedSelectEnabled &&
                            !($(this).hasClass('jalali-disabled-day'))) {
                            $(this).addClass('jalali-disabled-day');
                        }
                    }
                    else {
                        if (!($(this).hasClass('jalali-past-day'))) {
                            $(this).removeClass('jalali-disabled-day');
                        }
                    }
                    if ($(this).hasClass('jalali-occupied-day-temp')) {
                        $(this).addClass('jalali-disabled-day');
                        $(this).off('click');
                        $(this).off('hover');
                    }
                }
            }
        });
    })
}

function handleDayHover(elems, dayElem, setting) {
    if (setting.selectionType != 'multi' ||
        !(firstSelectedDay != undefined &&
          secondSelectedDay == undefined)) {
        elems.forEach(function (elem) {
            $(elem).find('.jalali-real-day').removeClass('jalali-range-candidate-day');
        });
        return;
    }
    var value = parseInt($(dayElem).attr('data-value'));
    if (value <= firstSelectedDay.value) {
        elems.forEach(function (elem) {
            $(elem).find('.jalali-real-day').removeClass('jalali-range-candidate-day');
        });
    }
    elems.forEach(function (elem) {
        $(elem).find('.jalali-real-day').each(function () {
            var dayValue = parseInt($(this).attr('data-value'));
            if (dayValue < value && dayValue > firstSelectedDay.value &&
                !($(this).hasClass('jalali-disabled-day'))) {
                $(this).addClass('jalali-range-candidate-day');
            }
            else {
                $(this).removeClass('jalali-range-candidate-day');
            }
        });
    });
}

function updateMonthYearLabel(elems, month) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        $elem.find('.jalali-year-label').html(month.monthString + ' ' + englishToPersianNumber(month.year));
    });
}

function updateDatePickerSeek(elems, month, setting) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $prevBtn = $elem.find('.js-prev-month-btn');
        if (month.year < jalaliCurrentMonth.year ||
            (month.year == jalaliCurrentMonth.year &&
            month.month <= jalaliCurrentMonth.month)) {
            $prevBtn.css('color', '#f4f4f4');
            $prevBtn.css('borderColor', '#f4f4f4');
            $prevBtn.off('mouseup');
        }
        else {
            $prevBtn.css('color', '#242424');
            $prevBtn.css('borderColor', '#242424');
            $prevBtn.mouseup(function () {
                $(this).off("mouseup");
                $elem.find('.js-next-month-btn').off('mouseup');
                var prevMonth = getPreviousMonthJalali(month);
                setting.monthOffset--;
                updateDatePicker(elems, prevMonth, setting);
            });
        }
        var $nextBtn = $elem.find('.js-next-month-btn');
        if (setting.maxSupportedMonth != undefined &&
            (month.year > setting.maxSupportedMonth.year ||
            (month.year == setting.maxSupportedMonth.year &&
            month.month >= setting.maxSupportedMonth.month))) {
            $nextBtn.css('color', '#f4f4f4');
            $nextBtn.css('borderColor', '#f4f4f4');
            $nextBtn.off('mouseup');
        }
        else {
            $nextBtn.css('color', '#242424');
            $nextBtn.css('borderColor', '#242424');
            $nextBtn.mouseup(function () {
                $(this).off("mouseup");
                $elem.find('.js-prev-month-btn').off('mouseup');
                var nextMonth = getNextMonthJalali(month);
                setting.monthOffset++;
                updateDatePicker(elems, nextMonth, setting);
            });
        }
    });
}

function updateDatePickerDays(elems, month) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        $monthElem.empty();
        var days = getJalaliMonthDays(month);
        var currWeek = 0;

        if (days[0].dayOfWeek > 0) {
            currWeek++;
            $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            for (var i = 0; i < days[0].dayOfWeek; i++) {
                $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
            }
        }
        days.forEach(function (item, index) {
            if (item.dayOfWeek == 0) {
                currWeek++;
                $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            }
            $monthElem.find('.js-week-' + currWeek).append('<div data-value="' + item.value + '" data-date="' + item.dateString + '" class="jalali-day jalali-real-day' +
                (item.isPastDay ? ' jalali-past-day' : '') +
                (item.isToday ? ' jalali-today' : '') +
                (item.isPastDay ? ' jalali-disabled-day' : '') +
                (item.isHoliday && !item.isPastDay ? ' jalali-holiday' : '') +
                '">' +
                '<span>' + englishToPersianNumber(item.day) + '</span>' +
                '</div>');
        });
        for (var i = days[days.length - 1].dayOfWeek + 1; i < 7; i++) {
            $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
        }
        while (currWeek < 6) {
            currWeek++;
            $monthElem.append('<div class="jalali-week js-week-' + currWeek + '"></div>');
            for (var i = 0; i < 7; i++) {
                $monthElem.find('.js-week-' + currWeek).append('<div class="jalali-day jalali-empty-day"></div>');
            }
        }
    });
}

function updateDatePickerPrices(elems, setting) {
    if (setting.priceDict == undefined) {
        return;
    }
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        $monthElem.find('.jalali-price-label').remove();
        var price;
        $monthElem.find('.jalali-real-day').each(function () {
            price = setting.priceDict[$(this).attr('data-value')];
            if (price) {
                $(this).append('<span class="jalali-price-label">' + priceToSpecialString(price.price) + '</span>');
            }
        });
    });
}

function updateDatePickerOccupied(elems, setting) {
    elems.forEach(function (elem) {
        var $elem = $(elem);
        var $monthElem = $elem.find('.jalali-month');
        var $occDays = $monthElem.find('.jalali-occupied-day')
        $occDays.removeClass('jalali-occupied-day');
        $occDays.removeClass('jalali-disabled-day');
        var occupied;
        $monthElem.find('.jalali-real-day').each(function () {
            var value = parseInt($(this).attr('data-value'));
            occupied = setting.occupiedList && setting.occupiedList.includes(value);
            if (occupied) {
                $(this).find('.jalali-price-label').hide();
                $(this).append('<span class="jalali-reserved-label">رزرو شده</span>');
                $(this).addClass('jalali-occupied-day');
                if (!setting.occupiedSelectEnabled) {
                    $(this).addClass('jalali-disabled-day');
                }
                $(this).off('click');
                $(this).off('hover');
            }
            else {
                $(this).find('.jalali-price-label').show();
            }
            if (!occupied || setting.occupiedSelectEnabled) {
                $(this).click(function () {
                    selectDate(elems, this, setting);
                });
                $(this).hover(function () {
                    handleDayHover(elems, this, setting);
                });
                if (setting.selectionType === 'multi') {
                    if (firstSelectedDay && firstSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        firstSelectedDay.elem = this;
                    }
                    if (secondSelectedDay && secondSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        secondSelectedDay.elem = this;
                    }
                    updateRangeSelection(elems, setting);
                }
                else {
                    if (currentSelectedDay && currentSelectedDay.value === value) {
                        $(this).addClass('jalali-selected-day');
                        currentSelectedDay.elem = this;
                    }
                }
            }
        });
    });
}

function jalaliShowLoading() {
    $('.jalali-loading').css('display', 'flex');
}

function jalaliHideLoading() {
    $('.jalali-loading').css('display', 'none');
}

function priceToSpecialString(price) {
    var priceStr = price.toString();
    priceStr = priceStr.slice(0, -3);
    return priceStr;
}