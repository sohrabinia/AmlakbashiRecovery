function validateEmail(elementValue) {
    var emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    return emailPattern.test(elementValue);
}

function validateMobile(mobile) {
    return (mobile.match(/[0][9][0-9]{9}/) && mobile.length == 11) ||
        (mobile.substring(0, 2) == "00" && mobile.length > 11);
}

function persianNumberToEnglish(persian_number) {
    var persian = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    var arabic = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    persian_number = persian_number.trim();
    for (var i = 0; i < persian.length; i++) {
        persian_number = persian_number.replaceAll(persian[i], i.toString());
        persian_number = persian_number.replaceAll(arabic[i], i.toString());
    }
    return persian_number;
}

function validateTell(tell) {
    if (!tell.match(/0+[1-8]+\d{9}/)) {
        return false
    }
    else {
        if (tell.length != 11)
            return false;
        return true;

    }
}

function validateNumber(element) {
    var value = element.value;
    value = persianNumberToEnglish(value);
    if (!value.match(/^\d+$/)) {
        value = value.replace(/\D/g, '');
    }
    element.value = value;
}