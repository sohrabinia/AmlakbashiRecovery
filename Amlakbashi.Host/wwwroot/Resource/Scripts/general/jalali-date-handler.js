var
persianNumbers = [/۰/g, /۱/g, /۲/g, /۳/g, /۴/g, /۵/g, /۶/g, /۷/g, /۸/g, /۹/g],
arabicNumbers = [/٠/g, /١/g, /٢/g, /٣/g, /٤/g, /٥/g, /٦/g, /٧/g, /٨/g, /٩/g];
persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'],

jalaliWeekDays = ['شنبه', 'یکشنبه', 'دوشنبه', 'سه شنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه'];
jalaliMonthNames = ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
jalaliWeekDaysShort = ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج'];

var jalaliHolidays = [  "1400/12/10", "1400/12/29",
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
    //if (pastDayOffset != 0) {
    //    pastDayBoundary.setDate(pastDayBoundary.getDate() + pastDayOffset);
    //}
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
        value: jYear == 1401 && jMonth == 1 && jDay == 2 ? 1647894600000 : gDateClone.valueOf()
        //value: gDateClone.valueOf()
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
    //if (getJalaliToday().month > 6) {
    //    gNextDay.addHours(1);
    //}
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