
/*
  Jalaali years starting the 33-year rule.
*/
var breaks =  [ -61, 9, 38, 199, 426, 686, 756, 818, 1111, 1181, 1210
  , 1635, 2060, 2097, 2192, 2262, 2324, 2394, 2456, 3178
  ]

/*
  Converts a Gregorian date to Jalaali.
*/

function toJalaali(gDate) {
    return toJalaali(gDate.getFullYear(), gDate.getMonth(), gDate.getDate());
}

function toJalaali(gy, gm, gd) {
  if (Object.prototype.toString.call(gy) === '[object Date]') {
    gd = gy.getDate()
    gm = gy.getMonth() + 1
    gy = gy.getFullYear()
  }
  return d2j(g2d(gy, gm, gd))
}

/*
  This function determines if the Jalaali (Persian) year is
  leap (366-day long) or is the common year (365 days), and
  finds the day in March (Gregorian calendar) of the first
  day of the Jalaali year (jy).
  @param jy Jalaali calendar year (-61 to 3177)
  @param withoutLeap when don't need leap (true or false) default is false
  @return
    leap: number of years since the last leap year (0 to 4)
    gy: Gregorian year of the beginning of Jalaali year
    march: the March day of Farvardin the 1st (1st day of jy)
  @see: http://www.astro.uni.torun.pl/~kb/Papers/EMP/PersianC-EMP.htm
  @see: http://www.fourmilab.ch/documents/calendar/
*/
function jalCal(jy, withoutLeap) {  
  var bl = breaks.length
    , gy = jy + 621
    , leapJ = -14
    , jp = breaks[0]
    , jm
    , jump
    , leap
    , leapG
    , march
    , n
    , i

  if (jy < jp || jy >= breaks[bl - 1])
    throw new Error('Invalid Jalaali year ' + jy)

  // Find the limiting years for the Jalaali year jy.
  for (i = 1; i < bl; i += 1) {
    jm = breaks[i]
    jump = jm - jp
    if (jy < jm)
      break
    leapJ = leapJ + div(jump, 33) * 8 + div(mod(jump, 33), 4)
    jp = jm
  }
  n = jy - jp

  // Find the number of leap years from AD 621 to the beginning
  // of the current Jalaali year in the Persian calendar.
  leapJ = leapJ + div(n, 33) * 8 + div(mod(n, 33) + 3, 4)
  if (mod(jump, 33) === 4 && jump - n === 4)
    leapJ += 1

  // And the same in the Gregorian calendar (until the year gy).
  leapG = div(gy, 4) - div((div(gy, 100) + 1) * 3, 4) - 150

  // Determine the Gregorian date of Farvardin the 1st.
  march = 20 + leapJ - leapG

  // return with gy and march when we don't need leap
  if (withoutLeap) return { gy: gy, march: march };


  // Find how many years have passed since the last leap year.
  if (jump - n < 6)
    n = n - jump + div(jump + 4, 33) * 33
  leap = mod(mod(n + 1, 33) - 1, 4)
  if (leap === -1) {
    leap = 4
  }  

  return  { leap: leap
          , gy: gy
          , march: march
          }
}

function j2d(jy, jm, jd) {
  var r = jalCal(jy, true)
  return g2d(r.gy, 3, r.march) + (jm - 1) * 31 - div(jm, 7) * (jm - 7) + jd - 1
}

function d2j(jdn) {
  var gy = d2g(jdn).gy // Calculate Gregorian year (gy).
    , jy = gy - 621
    , r = jalCal(jy, false)
    , jdn1f = g2d(gy, 3, r.march)
    , jd
    , jm
    , k

  // Find number of days that passed since 1 Farvardin.
  k = jdn - jdn1f
  if (k >= 0) {
    if (k <= 185) {
      // The first 6 months.
      jm = 1 + div(k, 31)
      jd = mod(k, 31) + 1
      return  { jy: jy
              , jm: jm
              , jd: jd
              }
    } else {
      // The remaining months.
      k -= 186
    }
  } else {
    // Previous Jalaali year.
    jy -= 1
    k += 179
    if (r.leap === 1)
      k += 1
  }
  jm = 7 + div(k, 30)
  jd = mod(k, 30) + 1
  return  { jy: jy
          , jm: jm
          , jd: jd
          }
}
function g2d(gy, gm, gd) {
  var d = div((gy + div(gm - 8, 6) + 100100) * 1461, 4)
      + div(153 * mod(gm + 9, 12) + 2, 5)
      + gd - 34840408
  d = d - div(div(gy + 100100 + div(gm - 8, 6), 100) * 3, 4) + 752
  return d
}
function d2g(jdn) {
  var j
    , i
    , gd
    , gm
    , gy
  j = 4 * jdn + 139361631
  j = j + div(div(4 * jdn + 183187720, 146097) * 3, 4) * 4 - 3908
  i = div(mod(j, 1461), 4) * 5 + 308
  gd = div(mod(i, 153), 5) + 1
  gm = mod(div(i, 153), 12) + 1
  gy = div(j, 1461) - 100100 + div(8 - gm, 6)
  return  { gy: gy
          , gm: gm
          , gd: gd
          }
}
function div(a, b) {
  return ~~(a / b)
}

function mod(a, b) {
  return a - ~~(a / b) * b
}
var
persianNumbers = [/۰/g, /۱/g, /۲/g, /۳/g, /۴/g, /۵/g, /۶/g, /۷/g, /۸/g, /۹/g],
arabicNumbers = [/٠/g, /١/g, /٢/g, /٣/g, /٤/g, /٥/g, /٦/g, /٧/g, /٨/g, /٩/g];
persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'],

jalaliWeekDays = ['شنبه', 'یکشنبه', 'دوشنبه', 'سه شنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه'];
jalaliMonthNames = ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
jalaliWeekDaysShort = ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'];

var jalaliHolidays = ["1400/1/1", "1400/1/2", "1400/1/3",
                       "1400/1/4", "1400/1/9", "1400/1/12",
                       "1400/2/14", "1400/2/23", "1400/3/15",
                       "1400/3/16", "1400/4/30", "1400/5/7",
                       "1400/5/27", "1400/5/28", "1400/7/5",
                       "1400/7/13", "1400/7/15",
                       "1400/8/2", "1400/10/16", "1400/11/26",
                        "1400/12/10", "1400/12/29",
                        "1401/1/1", "1401/1/2", "1401/1/3",
                        "1401/1/4", "1401/1/13", "1401/2/12",
                        "1401/2/13", "1401/3/5", "1401/3/14",
                        "1401/3/15", "1401/4/18", "1401/4/26",
                        "1401/5/16", "1401/5/17", "1401/7/2",
                        "1401/7/4", "1401/7/12", "1401/7/21",
                        "1401/10/6", "1401/11/15", "1401/11/22",
                        "1401/11/29", "1401/12/16", "1401/12/29"];


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function persianToEnglishNumber (str) {
    if (typeof str === 'string') {
        for (var i = 0; i < 10; i++) {
            str = str.replace(persianNumbers[i], i).replace(arabicNumbers[i], i);
        }
    }
    return str;
};

function englishToPersianNumber(str) {
    str = str.toString();
    for (var i = 0; i < 10; i++) {
        str = str.replaceAll(i.toString(), persianDigits[i]);
    }
    return str;
};

var pastDayOffset;

function gregorianToJalaliDate(gDate) {
    var gDateClone = new Date(gDate.getTime());
    pastDayOffset = pastDayOffset == undefined ? 0 : pastDayOffset;
    var gToday = new Date();
    var pastDayBoundary = new Date();
    if (pastDayOffset != 0) {
        pastDayBoundary.setDate(pastDayBoundary.getDate() + pastDayOffset);
    }
    gToday.setHours(0, 0, 0, 0);

    pastDayBoundary.setHours(0, 0, 0, 0);
    //var localDateString = gDate.toLocaleDateString('fa-IR');
    //var dateStringEnglishDigit = persianToEnglishNumber(localDateString);
    //dateStringEnglishDigit = dateStringEnglishDigit.replace(/[^\/\d]/g, '');
    //if (parseInt(dateStringEnglishDigit.split('/')[0]) > 1900) {
        var convertedDate = toJalaali(gDate);
        var dateStringEnglishDigit = convertedDate.jy + '/' + convertedDate.jm + '/' + convertedDate.jd;
    //}
    var dateStringPersianDigit = englishToPersianNumber(dateStringEnglishDigit);
    let gregorianDayOfWeek = gDate.getDay();
    let jalaliDayOfWeek = gregorianDayOfWeek == 6 ? 0 : (gregorianDayOfWeek + 1);
    var jalaliDateSplit = dateStringEnglishDigit.split('/');
    var jYear = parseInt(jalaliDateSplit[0]);
    var jMonth = parseInt(jalaliDateSplit[1]);
    var jDay = parseInt(jalaliDateSplit[2]);
    gDateClone.setHours(0, 0, 0, 0);
    return {
        year: jYear,
        month: jMonth,
        day: jDay,
        monthString: jalaliMonthNames[jMonth - 1],
        dayOfWeek: jalaliDayOfWeek,
        dayOfWeekString: jalaliWeekDays[jalaliDayOfWeek],
        dayOfWeekStringShort: jalaliWeekDaysShort[jalaliDayOfWeek],
        dateStringPersian: dateStringPersianDigit,
        dateString: dateStringEnglishDigit,
        gregorianDate: new Date(gDate.getTime()),
        isToday: gDate.toDateString() === gToday.toDateString(),
        isPastDay: gDate < pastDayBoundary,
        isHoliday: jalaliDayOfWeek == 6 || jalaliHolidays.includes(dateStringEnglishDigit),
        value: jYear == 1400 && jMonth == 1 && jDay == 2 ? 1616358600000 : gDateClone.valueOf()
    };
}

function getJalaliToday() {
    return gregorianToJalaliDate(new Date());
}

function getJalaliTomorrow() {
    var date = new Date();
    date.setDate(date.getDate() + 1);
    return gregorianToJalaliDate(date);
}

function getJalaliYesterday() {
    var date = new Date();
    date.setDate(date.getDate() - 1);
    return gregorianToJalaliDate(date);
}

function getJalaliMonthDays(jDate) {
    var gDate = jDate.gregorianDate;
    var dayList = [];
    var gPrevDay = new Date(gDate.getTime());
    var gNextDay = new Date(gDate.getTime());
    if (getJalaliToday().month > 6) {
        gNextDay.addHours(1);
    }
    if (jDate.day > 1) {
        while (true) {
            gPrevDay.setDate(gPrevDay.getDate() - 1);
            var jPrevDay = gregorianToJalaliDate(gPrevDay);
            dayList.unshift(jPrevDay);
            if (jPrevDay.day <= 1) {
                break;
            }
        }
    }
    dayList.push(gregorianToJalaliDate(gDate));
    while (true) {
        gNextDay.setDate(gNextDay.getDate() + 1);
        var jNextDay = gregorianToJalaliDate(gNextDay);
        var jDay = jNextDay.day;
        if (jDay > 1) {
            dayList.push(jNextDay);
        }
        else {
            break;
        }
    }
    return dayList;
}

function getJalaliMonthWeeksCount(dayList) {
    var count = 0;
    for (var i = 0; i < dayList.length; i++) {
        if (dayList[i].dayOfWeek == 6) {
            count++;
        }
        else if (i == dayList.length - 1) {
            count++;
        }
    }
    return count;
}

function getNextMonthJalali(jDate) {
    var gNextDay = new Date(jDate.gregorianDate.getTime());
    while (true) {
        gNextDay.setDate(gNextDay.getDate() + 1);
        var jNextDay = gregorianToJalaliDate(gNextDay);
        var jDay = jNextDay.day;
        if (jDay < 2) {
            var result = gregorianToJalaliDate(gNextDay);
            return result;
        }
    }
}

function getPreviousMonthJalali(jDate) {
    var gPrevDay = new Date(jDate.gregorianDate.getTime());
    while (true) {
        gPrevDay.setDate(gPrevDay.getDate() - 1);
        var jPrevDay = gregorianToJalaliDate(gPrevDay);
        if (jDate.month != jPrevDay.month &&
            jPrevDay.day < 2) {
            var result = gregorianToJalaliDate(gPrevDay);
            return result;
        }
    }
}


function getDiffDays(date1, date2) {
    const diffTime = Math.abs(date2 - date1);
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}

function diffDaysMs(date1_ms, date2_ms) {
    var one_day = 1000 * 60 * 60 * 24;
    var difference_ms = date2_ms - date1_ms;
    var days = Math.ceil(difference_ms / one_day);
    return days;
}
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
            var isHostPanel = false;
            var isExtrinsic = false;
            if (setting.extrinsicList != undefined) {
                isHostPanel = true;
                isExtrinsic = occupied && setting.extrinsicList.includes(value);
            }
            if (occupied) {
                $(this).find('.jalali-price-label').hide();
                if (isExtrinsic) {
                    $(this).append('<span class="jalali-filled-label">پر شده</span>');
                }
                else {
                    if (isHostPanel) {
                        $(this).append('<span class="jalali-reserved-label-host">رزرو شده</span>');
                    }
                    else {
                        $(this).append('<span class="jalali-reserved-label">رزرو شده</span>');
                    }
                }
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
$('.home-page__next-slider').click(function () {
    nextSliderClick($(this));
});

$('.home-page__previous-slider').click(function () {
    previousSliderClick($(this));
});

function nextSliderClick($buttonElem)
{
    var elem = $buttonElem.next().get(0);
    var children = elem.children;
    var current_item_index = 0;
    for (var i = 0; i < children.length; i++) {
        var tableChild = children[i];
        var rect = tableChild.getBoundingClientRect();
        var parent_rect = $(".home-page__category-root").get(0).getBoundingClientRect();
        if (rect.left < parent_rect.left) {
            current_item_index = i;
            break;
        }
    }
    var current_item = $(children[current_item_index]);
    elem = $(elem);
    elem.scrollTo(current_item, 800, { margin: true });
}

function previousSliderClick($buttonElem) {
    var elem = $buttonElem.next().next().get(0);
    var children = elem.children;
    var current_item_index = children.length - 1;
    for (var i = children.length - 1; i >= 0; i--) {
        var tableChild = children[i];
        var rect = tableChild.getBoundingClientRect();
        var parent_rect = $(".home-page__category-root").get(0).getBoundingClientRect();
        if (rect.left + $(tableChild).width() > parent_rect.left + $(".home-page__category-root").width()) {
            current_item_index = i;
            break;
        }
    }
    if (current_item_index > 0 && current_item_index != children.length - 1) {
        var item_count = $(".home-page__category-root").width() / $(children[current_item_index]).width();
        item_count = Number((item_count).toFixed(0)) - 2;
        current_item_index += item_count;
    }
    var current_item = $(children[current_item_index]);
    elem = $(elem);
    elem.scrollTo(current_item, 800, { margin: true });
}

$(".home-page__category-container").scroll(updateLazyLoad);

function updateLazyLoad() {
    lazyLoad();
}

window.onload = function() {
    if (messageShowOnReady !== '')
    {
        showSuccessMessage('', messageShowOnReady);
    }
    //$.onCreate('div,a', function (elements) {
    //    elements.each(function () {
    //        if ($(this).hasClass("advertise-list-container")) {
    //            findLazyImages();
    //            $(this).children("*").each(function () {
    //                var new_parent = $(this).parent().parent();
    //                $(this).detach();
    //                $(this).appendTo(new_parent);
    //            });
    //            $(this).remove();
    //        }
    //        else if ($(this).hasClass('home-page__advertise-item-container')){
    //            $(this).find('.average-rating').barrating({
    //                theme: 'fontawesome-stars',
    //                readonly: true,
    //                initialRating: null
    //            });
    //        }
    //        else if ($(this).hasClass('home-page__slider-button')) {
    //            $(this).click(function () {
    //                if ($(this).hasClass('home-page__previous-slider')) {
    //                    previousSliderClick($(this));
    //                }
    //                else {
    //                    nextSliderClick($(this));
    //                }
    //            });
    //        }
    //        //else if ($(this).hasClass('home-page__category-item-container-large')) {
    //        //    var root = $(this).parent().parent();
    //        //    var next = root.next('.home-page__category');
    //        //    if (next != null)
    //        //    {
    //        //        next.css('display', 'inherit');
    //        //        next.addClass('partialContents');
    //        //    }
    //        //}
    //    });
    //}, true);



    //setInterval(function () {
    //    moveRight();
    //}, 3000);


    //var slideCount = $('#slider ul li').length;
    //var slideWidth = $('#slider ul li').width();
    //var slideHeight = $('#slider ul li').height();
    //var sliderUlWidth = slideCount * slideWidth;

    //$('#slider').css({ width: slideWidth, height: slideHeight });

    //$('#slider ul').css({ width: sliderUlWidth, marginLeft: -slideWidth });

    //$('#slider ul li:last-child').prependTo('#slider ul');

    //function moveLeft() {
    //    $('#slider ul').animate({
    //        left: +slideWidth
    //    }, 200, function () {
    //        $('#slider ul li:last-child').prependTo('#slider ul');
    //        $('#slider ul').css('left', '');
    //    });
    //};

    //function moveRight() {
    //    $('#slider ul').animate({
    //        left: -slideWidth
    //    }, 200, function () {
    //        $('#slider ul li:first-child').appendTo('#slider ul');
    //        $('#slider ul').css('left', '');
    //    });
    //};

    //$('a.control_prev').click(function () {
    //    moveLeft();
    //    return false;

    //});

    //$('a.control_next').click(function () {
    //    moveRight();
    //    return false;
    //});

};

//$("#slideshow > div:gt(0)").hide();

//setInterval(function () {
//    $('#slideshow > div:first')
//      .fadeOut(1500)
//      .next()
//      .fadeIn(1500)
//      .end()
//      .appendTo('#slideshow');
//}, 5000);

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
var searchByRegionMsg;

function showSearchByRegion() {
    var setting = {};
    setting.contentUrl = '/category/searchbyregionpopup?province='+
            (typeof initialProvince == 'undefined' ? -1 : initialProvince)+
            (typeof initialCity == 'undefined' ? -1 : initialCity) +
            (typeof initialArea == 'undefined' ? -1 : initialArea);
    var buttons = [{
        title: 'بستن',
        color: '#242424',
        bgColor: '#eaeaea',
        onclick: function () {
            searchByRegionMsg.closePopup();
        }
    },
    {
        title: 'انتخاب',
        color: '#242424',
        bgColor: '#fdd835',
        onclick: function () {
            doGeneralSearchRegion();
            searchByRegionMsg.closePopup();
        }
    }];
    setting.autoClose = false;
    setting.buttons = buttons;
    setting.color = '#4485F2';
    searchByRegionMsg = showMessagePopup('لیست شهر ها', '', setting);
}

function selectMostViewRegion(url, title) {
    currentSelectedRegion = {
        href: url,
        title: typeof title == 'undefined' ? '' : title
    };
    if (typeof isPortalHomePage == 'undefined' ||
        !isPortalHomePage) {
        doHomePageSearch();
    }
    $(".home-page__search-input").val(currentSelectedRegion.title);
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
    else {
        toggleSearchListBox(false);
    }
}

function doGeneralSearchRegion() {
    var $province = $("select[name='generalSearchProvince']");
    var $city = $("select[name='generalSearchCity']");
    var $area = $("select[name='generalSearchArea']");

    myajax("app/category/regionsearchtourl", "province=" + $province.val() +
        "&city=" + $city.val() + "&area=" + $area.val(), function (ret) {
            if (ret.status == 0) {
                showErrorMessage(ret.msg);
            }
            else {
                currentSelectedRegion = {
                    href: ret.url,
                    title: ret.title
                };
                if (typeof isPortalHomePage == 'undefined' ||
                    !isPortalHomePage) {
                    doHomePageSearch();
                }
                $(".home-page__search-input").val(currentSelectedRegion.title);
                if (isMobileDevice) {
                    toggleRegionSearchPopup(false);
                }
                else {
                    toggleSearchListBox(false);
                }
            }
        }, false);
}
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
//const slider = document.querySelector('.js-slider-scroll');
//let isDown = false;
//let startX;
//let scrollLeft;

//slider.addEventListener('mousedown', (e) => {
//    isDown = true;
//    slider.classList.add('active');
//    startX = e.pageX - slider.offsetLeft;
//    scrollLeft = slider.scrollLeft;
//});
//slider.addEventListener('mouseleave', () => {
//    isDown = false;
//    slider.classList.remove('active');
//});
//slider.addEventListener('mouseup', () => {
//    isDown = false;
//    slider.classList.remove('active');
//});
//slider.addEventListener('mousemove', (e) => {
//    if (!isDown) return;
//    e.preventDefault();
//    const x = e.pageX - slider.offsetLeft;
//    const walk = (x - startX) * 2; //scroll-fast
//    slider.scrollLeft = scrollLeft - walk;
//    console.log(walk);
//});
class Slider {
    constructor(selector, config = {}) {
        this.slider = $(selector);
        this.slides = this.slider.children();
        this.activeSlide = this.slides.first();
        this.activeClass = config.activeClass ? config.activeClass : 'slide-item-active';
        this.nextElement = config.nextElement ? $(config.nextElement) : null;
        this.prevElement = config.prevElement ? $(config.prevElement) : null;
        this.timer = config.timer ? config.timer : null;
        this.timerInterval = null;
        this.initSlider();
    }

    initSlider() {
        this.slides.addClass('slide-item');
        this.activeSlide.addClass(this.activeClass);
        this.setupEventListeners();
        if (this.timer) {
            this.attachTimer()
        }
    }

    attachTimer() {
        this.timerInterval = setInterval(() => {
            this.goNextSlide()
        }, this.timer);
    }

    reAttachTimer() {
        if (this.timerInterval && this.timer) {
            clearInterval(this.timerInterval);
            this.attachTimer();
        }
    }

    setEventListenerIfExist(element, eventName, callback) {
        if (element && element.length > 0) {
            element.on(eventName, callback);
        }
    }

    setupEventListeners() {
        this.setEventListenerIfExist(this.nextElement, 'click', () => {
            this.goNextSlide();
            this.reAttachTimer();
        });
        this.setEventListenerIfExist(this.prevElement, 'click', () => {
            this.goPreviousSlide();
            this.reAttachTimer();
        });
    }

    goNextSlide() {
        var nextSlide = this.activeSlide.next();
        this.activeSlide.removeClass(this.activeClass);
        if (nextSlide.length) {
            nextSlide.addClass(this.activeClass);
            this.activeSlide = nextSlide;
            return this.activeSlide;
        }
        this.activeSlide = this.slides.first();
        this.activeSlide.addClass(this.activeClass);
        return this.activeSlide;
    }

    goPreviousSlide() {
        var previousSlide = this.activeSlide.prev()
        this.activeSlide.removeClass(this.activeClass);
        if (previousSlide.length) {
            previousSlide.addClass(this.activeClass);
            this.activeSlide = previousSlide;
            return this.activeSlide;
        }
        this.activeSlide = this.slides.last();
        this.activeSlide.addClass(this.activeClass);
        return this.activeSlide;
    }
}

var slider = new Slider('.home-page__banner-discount', {
    nextElement: '.next',
    prevElement: '.prev',
    timer: 5000
})
var norozSlider = new Slider('.home-page__banner-norouz', {
    nextElement: '.next',
    prevElement: '.prev',
    timer: 5000
})

//carousel 

class Carousel {
    constructor(selector, config = {}) {
        this.parentSlider = document.querySelector(selector);
        this.slider = config.slider ? $(config.slider) : null;
        this.slides = this.slider.children();
        this.firstChild = this.slides.first();
        this.startOffSetLeft = $(this.firstChild).offset().left;
        this.activeSlide = this.firstChild;
        this.activeClass = config.activeClass ? config.activeClass : 'slide-item-active';
        this.nextElement = config.nextElement ? $(config.nextElement) : null;
        this.prevElement = config.prevElement ? $(config.prevElement) : null;
        this.slidesPerViewXl = config.slidesPerViewXl ? config.slidesPerViewXl : this.slidesPerView;
        this.slidesPerViewLg = config.slidesPerViewLg ? config.slidesPerViewLg : this.slidesPerView;
        this.slidesPerViewMd = config.slidesPerViewMd ? config.slidesPerViewMd : this.slidesPerView;
        this.slidesPerViewM = config.slidesPerViewM ? config.slidesPerViewM : this.slidesPerView;
        this.slidesPerViewSm = config.slidesPerViewSm ? config.slidesPerViewSm : this.slidesPerView;
        this.slidesPerViewXSm = config.slidesPerViewXSm ? config.slidesPerViewXSm : this.slidesPerView;
        this.slidesPerViewXs = config.slidesPerViewXs ? config.slidesPerViewXs : this.slidesPerView;
        this.slidesPerViewXXs = config.slidesPerViewXXs ? config.slidesPerViewXXs : this.slidesPerView;
        this.slidesPerView = null;
        this.widthSlides = null;
        this.isDown = false;
        this.startX;
        this.elmScrollLeft;
        this.initSlider();
    }
    initSlider() {
        this.slides.addClass('slide-item');
        this.activeSlide.addClass(this.activeClass);
        this.calculetslidesPerView();
        this.setupEventListeners();
    }
    calculetslidesPerView() {
        var widthShowItems = $('body').width();
        if (widthShowItems >= 1200) {
            this.slidesPerView = this.slidesPerViewXl;
        } else if (widthShowItems >= 970) {
            this.slidesPerView = this.slidesPerViewLg;
        } else if (widthShowItems >= 768) {
            this.slidesPerView = this.slidesPerViewMd;
        } else if (widthShowItems >= 680) {
            this.slidesPerView = this.slidesPerViewM;   
        } else if (widthShowItems >= 560) {
            this.slidesPerView = this.slidesPerViewSm;
        }else if (widthShowItems >= 450) {
            this.slidesPerView = this.slidesPerViewXSm;     
        } else if (widthShowItems >= 380 ) {
            this.slidesPerView = this.slidesPerViewXs;     
        } else if (widthShowItems >= 320 ) {
            this.slidesPerView = this.slidesPerViewXXs;
        }
        this.widthSlides = widthShowItems / this.slidesPerView;
        this.slides.css('min-width', this.widthSlides);
        return this.slidesPerView;
    }

    setEventListenerIfExist(element, eventName, callback) {
        if (element && element.length > 0) {
            element.on(eventName, callback);
        }
    }

    setupEventListeners() {
        this.setEventListenerIfExist(this.nextElement, 'click', () => {
            this.goNextSlide();
        });
        this.setEventListenerIfExist(this.prevElement, 'click', () => {
            this.goPreviousSlide();
        });
        this.parentSlider.addEventListener('mousedown', (e) => {
            this.onDragStart(e);
        });
        this.parentSlider.addEventListener('mouseleave', (e) => {
            this.isDown = false;
        });
        this.parentSlider.addEventListener('mouseup', (e) => {
            this.isDown = false;
            this.onDragEnd(e);
        });
        this.parentSlider.addEventListener('mousemove', (e) => {
            if (!this.isDown) return;
            e.preventDefault();
            this.onDragMoving(e);
        });
    }

    onDragStart(e) {
        this.isDown = true;
        this.startX = e.pageX - this.parentSlider.offsetLeft;
        this.elmScrollLeft = this.parentSlider.scrollLeft;
    }

    onDragEnd(e) {
        var mouseupSlid = $(e.target).closest('.carousel-item');
        if (!mouseupSlid.hasClass('slide-item-active')) {
            this.slides.removeClass('slide-item-active');
            mouseupSlid.addClass('slide-item-active');
            this.activeSlide = mouseupSlid;
            this.calculetCurrentItem();
            return this.activeSlide;
        }
        this.calculetCurrentItem();
    }

    onDragMoving(e) {
        const x = e.pageX - this.parentSlider.offsetLeft;
        const walk = (x - this.startX) * 3; //scroll-fast
        this.parentSlider.scrollLeft = this.elmScrollLeft - walk;
        this.calculetCurrentItem();
    }

    goNextSlide() {
        this.prevElement.css('display', 'flex');
        var nextSlide = this.activeSlide.next();
        this.activeSlide.removeClass(this.activeClass);
        $(this.parentSlider).animate({
            scrollLeft: '-=' + this.widthSlides,
            transition: 'all 2s cubic-bezier(0.0, 0.0, 0.58, 1.0);'
        }, 150);
        nextSlide.addClass(this.activeClass);
        this.activeSlide = nextSlide;
        this.calculetCurrentItem();
        return this.activeSlide;
    }

    goPreviousSlide() {
        this.nextElement.css('display', 'flex');
        var previousSlide = this.activeSlide.prev()
        this.activeSlide.removeClass(this.activeClass);
        $(this.parentSlider).animate({
            scrollLeft: '+=' + this.widthSlides,
            transition: 'all 2s cubic-bezier(0.0, 0.0, 0.58, 1.0);'
        }, 150);
        previousSlide.addClass(this.activeClass);
        this.activeSlide = previousSlide;
        this.calculetCurrentItem();
        return this.activeSlide;
    }

    calculetCurrentItem() {
        debugger;
        var lastOffsetLeft = this.slides.last().offset().left;
        var firstOfffsetLeft = this.slides.first().offset().left;
        if (lastOffsetLeft >= 0 - this.widthSlides / 2) {
            this.nextElement.css("color", "#d9d9d9");
            this.prevElement.css("color", "#242424");
        } else if (firstOfffsetLeft <= this.startOffSetLeft) {
            this.nextElement.css("color", "#242424");
            this.prevElement.css("color", "#d9d9d9");
        } else {
            this.prevElement.css("color", "#242424");
            this.nextElement.css("color", "#242424");
        }
    }
}

var carouselMedium = new Carousel('.home-page_amlakbashi-medium', {
    slider: '.home-page_amlakbashi-medium .carousel',
    nextElement: '.home-page-medium-box .btnPrevious',
    prevElement: '.home-page-medium-box .btnNext',
    slidesPerViewXl: '6',
    slidesPerViewLg: '5',
    slidesPerViewMd: '4.5',
    slidesPerViewM: '3.8',
    slidesPerViewSm: '3.5',
    slidesPerViewXSm: '3',
    slidesPerViewXs: '3',
    slidesPerViewXXs: '2.5',
})

var carouselVisited = new Carousel('.home-page_box-visited', {
    slider: '.home-page_box-visited .carousel',
    nextElement: '.home-page-visited-box .btnPrevious',
    prevElement: '.home-page-visited-box .btnNext',
    slidesPerViewXl: '5',
    slidesPerViewLg: '4.3',
    slidesPerViewMd: '3.3',
    slidesPerViewM: '3.5',
    slidesPerViewSm: '3.2',
    slidesPerViewXSm: '2.5',
    slidesPerViewXs: '2.2',
    slidesPerViewXXs: '1.8',
})
