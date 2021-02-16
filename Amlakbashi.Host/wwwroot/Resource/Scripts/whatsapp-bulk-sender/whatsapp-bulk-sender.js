//var whatsappApiUrl = 'https://eu1.chat-api.com/instance108585/message?token=imx767x89dqxis6t';
//var whatsappApiUrl = 'https://eu15.chat-api.com/instance110643/message?token=q2egv33nykoyz0ax';
var whatsappApiUrl = 'https://eu17.chat-api.com/instance117119/message?token=itoy677w9cpcyt0d';

function sendWhatsappBulkMessage(body, mobiles) {
    for (var i = 0; i < mobiles.length; i++) {
        sendWhatsappMessage(mobiles[i], body);
    }
}

function sendWhatsappAccBrokenBulkMsg(data) {
    for (var i = 0; i < data.length; i++) {
        sendWhatsappAccBrokenMsg(data[i].mobile, data[i].accLink);
    }
}

function sendWhatsappCoronaAdvBulkMsg(data) {
    for (var i = 0; i < data.length; i++) {
        sendWhatsappCoronaAdvMsg(data[i].mobile, data[i].userName);
    }
}

function sendWhatsappCoronaAdvMsg(mobile, userName) {
    var body = userName + '\n\nبرای حفظ سلامتی جسمتان در مقابل  کرونا فعلا سفر غیر ضروری نروید\n\nاما برای حفظ سلامتی روحتان بعد از کرونا حتما سفر بروید\n\nشما می توانید اقامتگاه مورد پسندتان را به "علاقه‌مندی‌" اضافه کنید و بعد از کرونا سفر کنید\n\nاملاک باشی سایت اجاره روزانه خانه ، ویلا و سوئیت\n\n' + 'https://www.amlakbashi.com';
    sendWhatsappMessage(mobile, body);
}

function sendWhatsappAccBrokenMsg(mobile, accLink) {
    var body = "با سلام\nعکس های آگهی شما در سایت املاک باشی دچار مشکل شده و نیاز به ویرایش دارد لطفا لینک ارسالی را باز کرده و ویرایش کنید یا عکس های مربوط به واحدتون رو بفرستید که ما براتون انجام بدیم\n " + accLink + ' \nبا تشکر از همکاری شما';
    sendWhatsappMessage(mobile, body);
}

function sendWhatsappMessage(mobile, body) {
    var data = {
        phone: mobile, // Receivers phone
        body: body, // Message
    };
    // Send a request
    $.ajax(whatsappApiUrl, {
        data: JSON.stringify(data),
        contentType: 'application/json',
        type: 'POST'
    });
}