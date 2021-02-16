/******date and time*******/

var day_milliseconds = 86400000;

function datediff(date_from, date_to) {
    var from = new persianDate([date_from.year, date_from.month, date_from.date])
    var to = new persianDate([date_to.year, date_to.month, date_to.date])
    return Math.round((to - from) / day_milliseconds);
}

function persianDateIsFriday(persian_date) {
    try {
        var date = persian_date.split(",");
        var d = $.calendars.newDate(date[0], date[1], date[2], 'persian', 'fa');
        var js_date = d._calendar.toJSDate(parseInt(date[0]), parseInt(date[1]), parseInt(date[2]));
        return js_date.getDay() == 5;
    }
    catch (err) {
        return false;
    }
}

var persian_holidays = ["1397,6,8", "1397,6,28", "1397,6,29",
                       "1397,8,8", "1397,8,16", "1397,8,17",
                       "1397,9,4", "1397,11,20", "1397,11,22",
                       "1397,12,29", "1398,1,1", "1398,1,3",
                       "1398,1,4", "1398,1,12", "1398,1,13",
                       "1398,1,14", "1398,1,30", "1398,2,1",
                       "1398,3,3", "1398,3,6",
                       "1398,3,14", "1398,3,15", "1398,3,16",
                       "1398,4,8", "1398,5,21",
                       "1398,5,29", "1398,6,18", "1398,6,19",
                       "1398,7,27", "1398,8,3",
                       "1398,8,5", "1398,8,7",
                       "1398,8,15", "1398,11,9",
                       "1398,11,22", "1398,12,16", "1398,12,18",
                       "1398,12,29",
                       "1399,1,2", "1399,1,3", "1399,1,4",
                       "1399,1,12", "1399,1,13", "1399,1,20",
                       "1399,2,25", "1399,3,4", "1399,3,5",
                       "1399,3,14", "1399,3,15", "1399,3,28",
                       "1399,5,18", "1399,6,8", "1399,7,16",
                       "1399,7,24", "1399,7,26", "1399,8,4",
                       "1399,8,13", "1399,10,27", "1399,11,22",
                       "1399,12,7", "1399,12,21", "1399,12,30",
                       "1400,1,1", "1400,1,2", "1400,1,3",
                       "1400,1,4", "1400,1,8", "1400,1,12",
                       "1400,2,13", "1400,2,23", "1400,3,15",
                       "1400,3,16", "1400,4,29", "1400,5,6",
                       "1400,5,26", "1400,5,27", "1400,7,5",
                       "1400,7,13", "1400,7,14", "1400,7,22",
                       "1400,8,1", "1400,10,16", "1400,11,25",
                       "1400,12,9", "1400,12,29"];

function dateIsHoliday(persian_date) {
    return persianDateIsFriday(persian_date) || persian_holidays.includes(persian_date);
}

function unixToPersianDate(unix) {
    var date = new persianDate(unix);
    date.toLocale('en').toCalendar('persian');
    return date.format("YYYY,M,D");
    //return date.year() + "," + date.month() + "," + date.days();
}
function persianDateToUnix(persian_date) {
    var split_str = persian_date.split(',');
    return date = new persianDate([parseInt(split_str[0]), parseInt(split_str[1]), parseInt(split_str[2])]);
    return date.unix();
}
function getDayArrayBetweenUnixes(unix_from, unix_to) {
    var current_persian_date = new persianDate(unix_from);
    var last_persian_date = new persianDate(unix_to);
    var day_array = [];
    while (current_persian_date.unix() < last_persian_date.unix()) {
        day_array.push(unixToPersianDate(current_persian_date));
        current_persian_date = current_persian_date.add("days", 1);
    }
    return day_array;
}

/******end date and time*******/