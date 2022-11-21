//Menu
var menu_shown = false;

var messageQuestionList = [];

function searchAdvertiseId() {
    showInfoMessage('جستجو با کد آگهی',
        '<div style="display:flex;flex-flow:row;width:100%;max-width:320px;justify-content:space-around;">' +
        '<input onpaste="onSearchAdvertiseIdChange(event, this)" onkeyup="onSearchAdvertiseIdChange(event, this)" onchange="onSearchAdvertiseIdChange(event, this)" style="width: 65%;max-width: 170px;padding: 5px;" autofocus placeholder="کد آگهی را وارد کنید" />' +
        '<div onclick="doSearchByAdvertiseId(this.previousSibling)" style="background-color:#fdd835;border-radius:5px;padding:5px 10px;cursor:pointer;">جستجو</div>' +
        '</div>'
        );
}

function onSearchAdvertiseIdChange(e, elem) {
    if (e != null && e.keyCode === 13) {
        setTimeout(function () {
            doSearchByAdvertiseId(elem);
        }, 500);
        return;
    }
}

function doSearchByAdvertiseId(inputElem) {
    var id = $(inputElem).val();
    if (typeof id == 'undefined' ||
        id == null ||
        id == '') {
        id = 0;
    }
    myajax('accomodation/getaccurlbyid', 'id=' + id, function (ret) {
        if (ret.status == 0) {
            showErrorMessage('خطا', 'کد آگهی یافت نشد. لطفا کد وارد شده را بررسی کنید.');
            return;
        }
        window.open(ret.url, '_self');
    });
}

function toggleMenu() {
    if (menu_shown) {
        $(".master__header-menu").removeClass("master__header-menu-open");
        $(".master__menu").slideUp();
        menu_shown = false;
    }
    else {
        $(".master__header-menu").addClass("master__header-menu-open");
        $(".master__menu").slideDown(function () {
            if ($(this).is(':visible'))
                $(this).css('display', 'flex');
        });
        menu_shown = true;
    }
}

function sendTokenToServer(token) {
    myajax("user/updateusernotificationtoken", "token=" + token, function (ret) {
    }, false);
}

//End Menu  

function userAskQuestion(id, question) {
    messageQuestionList.push(question);
    if ($('.support-chat__chat-box').is(":visible")) {
        var url = "/supportchat/getchatpopup?id=" + id +
            "&questionlist=" + JSON.stringify(messageQuestionList);
        $(".support-chat__chat-box").load(url, function () {
            var chatPopupChildren = $('.chat-popup__container').children();
            console.log("scrolling into view");
            chatPopupChildren[1].scrollIntoView();
        });
    }
    //var chatPopupChildren = $('.chat-popup__container').children();
    //chatPopupChildren[chatPopupChildren.length - 1].scrollIntoView();
    //myajax('supportchat/useraskquestion',
    //    'id=' + id + '&question=' + question, function (ret) {
    //        if (ret.status == 1) {

    //            if ($('.support-chat__chat-box').is(":visible")) {
    //                var url = "/supportchat/getchatpopup?id=" + id;
    //                $(".support-chat__chat-box").load(url);
    //            }
    //            var chatPopupChildren = $('.chat-popup__container').children();
    //            chatPopupChildren[chatPopupChildren.length - 1].scrollIntoView();
    //        }
    //    });
}

function getReserveToRate() {
    myajax('reserve/getreservetorate', null, function (ret) {
        if (ret.status == 1) {
            ratingDialog(ret.reserveToRate);
        }
    }, false);
}

function sendTokenToServer(token) {
    myajax("user/updateusernotificationtoken", "token=" + token, function (ret) {
    }, false);
}

//$('body').prepend('<a href="#" class="back-to-top"></a>');
//var amountScrolled = 300;

//$(window).scroll(function () {
//    if ($(window).scrollTop() > amountScrolled) {
//        $('a.back-to-top').fadeIn('slow');
//    } else {
//        $('a.back-to-top').fadeOut('slow');
//    }
//});
//$('a.back-to-top').click(function () {
//    $('html, body').animate({
//        scrollTop: 0
//    }, 700);
//    return false;
//});

var current_user_id = 0;
var presentPopupAfterLogin = false;
var isUserloggedIn = false;

function on_login() {
    isUserLoggedIn = true;
    if (typeof on_login_action !== "undefined"){
        on_login_action();
    }
    myajax('user/fetchuserid', '', function(ret) {
        if (ret.status == 1) {
            current_user_id = ret.userId;
        }
    });
    if (presentPopupAfterLogin) {
        showPresentPopup();
    }
}

checkSignalrFetch();

const portalHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/portalhub")
    .build();

function checkSignalrFetch() {
    if (typeof fetch !== "undefined" && typeof AbortController === "undefined") {
        console.warn("Fetch is supported, but not AbortController.  Dropping default fetch so SignalR can override.");
        window.fetch = undefined;
    }
}

var registered_service_worker;

function updateAccDetail() {
    var accIds = "";
    var accId;
    $('.home-page__category-item-container-large').each(function () {
        accId = $(this).attr("id");
        if (accId != undefined && accId.match(/^advertise_\d+/)) {
            accIds += accId.match(/\d+/)[0] + ",";
        }
    });
    if (accIds != "") {
        accIds = accIds.substring(0, accIds.length - 1);
        $.ajax({
            type: "POST",
            url: "/accomodation/getacclistdynamicviewbag",
            data: "{ids:'" + accIds + "'}",
            contentType: "application/json",
            dataType: "json",
            success: function (ret) {
                var dict = ret.price_dict;
                for (var key in dict) {
                    // check if the property/key is defined in the object itself, not in parent
                    if (dict.hasOwnProperty(key)) {
                        $("#advertise_" + key).find('#js-daily-price').html("شروع قیمت: " + dict[key]);
                    }
                }
            }
        });
    }
}

$(document).ready(function () {
    initializePage();
    //if (!lockInPage) {
        //$('#header').load('/post/getheader?ishomepage=' + isHomePage);
        //$('.footer').load('/post/getfooter', function () {
        //});
    //}
});

//function onLoadFooter() {
//    if (current_user_id == 0) {
//        $('.js-present-prize-link').attr('href', '#');
//        $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
//    }
//    //$('#drftsguidrftnbpewmcs').attr('src', 'https://trustseal.enamad.ir/logo.aspx?id=10128&p=lznbgthvlznbqesgukaq');
//    $('#jxlznbqejxlzfukzapfuapfu').attr('src', '/file/resourceimagewebp?file_name=samandehi');
//    $('#js-footer-map-image').attr('src', '/file/resourceimagewebp?file_name=map');

//    $('#js-facebook-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/facebook');
//    $('#js-twitter-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/twitter');
//    $('#js-googleplus-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/googleplus');
//    $('#js-instagram-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/instagram');
//    $('#js-aparat-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/aparat');
//    $('#js-telegram-icon').attr('src', '/file/resourceimagepng?file_name=socialnetwork/telegram');
//}
(function () {
    if (current_user_id == 0)
    {
      $('.js-present-prize-link').attr('href', '#');
      $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
    }
})
//$(window).load(function () {
//    initializePage();
//});
//document.addEventListener("DOMContentLoaded", function () {
//    initializePage();
//});

function initializeSupportChat() {
    $('body').append('<div id="js-temp-holder-master"></div>');
    var $tempHolder = $('#js-temp-holder-master');
    $tempHolder.load('/supportchat/getsupportchatuser', function () {
        $tempHolder.children().each(function () {
            $('body').prepend($(this));
        });
        $tempHolder.empty();
        $(".support-chat__text-input").keydown(function () {
            $(this).css('height', 'auto');
            $(this).css('height', this.scrollHeight);
        });
    });
    $tempHolder.remove();
    if (!checkUserVisited()) {
        setTimeout(showSupportChatInform, 500)
    }
}

function initializePresentPrize() {
    var shown = checkPresentShown();
    if (!shown) {
        $('.present-prize__button').css('display', 'flex');
    }
    else {
        $('.present-prize__button').css('display', 'none');
    }
}

function initializeLoginPopup() {
    //$('body').append('<div id="js-temp-holder-master-2"></div>');
    //var $tempHolder_2 = $('#js-temp-holder-master-2');
    //$tempHolder_2.load('/user/getloginpopup', function () {
    //    $tempHolder_2.children().each(function () {
    //        $('body').prepend($(this));
    //    });
    //    $tempHolder_2.empty();
    //});
    //$tempHolder_2.remove();
}

function initializeMasterHub() {
    portalHubConnection.on('reloadSupportChat', (supportChatId, newCount, userId) => {
        var id = $('#js-support-chat-id').val();
        supportChatId = parseInt(supportChatId);
        id = parseInt(id);
        if (supportChatId == id || (userId > 0 && userId == current_user_id)) {
            $('#js-support-chat-id').val(supportChatId);
            refreshChatBox(supportChatId, newCount);
        }
    });
    portalHubConnection.start()
        .then(() => console.log('portal hub connected!'))
        .catch(console.error);
}

function initializeServiceWorker() {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.getRegistration("/service_worker.js").then(registration => {
            if (registration == undefined) {
                navigator.serviceWorker.register('/service_worker.js')
                 .then(reg => {
                     console.log('Service worker registered! 😎', reg);
                     messaging.useServiceWorker(reg);
                     registered_service_worker = reg;
                     check_user_login();
                 })
                 .catch(err => {
                     console.log('😥 Service worker registration failed: ', err);
                     check_user_login();
                     sendTokenToServer(null);
                 });
            }
            else {
                messaging.useServiceWorker(registration);
                registered_service_worker = registration;
                check_user_login();
            }
        });
    }
    else {
        check_user_login();
        sendTokenToServer(null);
    }
}

function initializePage() {
    try {
        initializeServiceWorker();
    }
    catch (err) {
    }
    initializeLoginPopup();
    getReserveToRate();
    initializeMasterHub();
    //window.$crisp = []; window.CRISP_WEBSITE_ID = "76fba52f-295b-48d8-a14e-4ace311f993e"; (function () { d = document; s = d.createElement("script"); s.src = "https://client.crisp.chat/l.js"; s.async = 1; d.getElementsByTagName("head")[0].appendChild(s); })();
    initializeSupportChat();
    initializePresentPrize();
    setTimeout(updateAccDetail, 1000);
}

function checkUserVisited() {
    var visited = getCookie("visited") === "yes";
    setCookie("visited", "yes", 365);
    return visited;
}

function checkPresentShown() {
    var shown = getCookie("reserveprizeshown") === "yes";
    return shown;
}

function setPresentShown(temp) {
    if (temp) {
        setCookieForHours("reserveprizeshown", "yes", 3);
    }
    else {
        setCookie("reserveprizeshown", "yes", 365);
    }
}

presentPrizePopup = null;

function showPresentPopup() {
    $('.present-prize__button').hide();
    var setting = {};
    setting.contentUrl = '/post/getpresentandprizepopup';
    var buttons = [{
        title: 'نمایش نده',
        color: 'white',
        bgColor: '#EA4335',
        onclick: function () {
            setPresentShown(false);
        }
    },
    {
        title: 'بعدا',
        color: 'white',
        bgColor: '#34A853',
        onclick: function () {
            setPresentShown(true);
        }
    }];
    setting.buttons = buttons;
    setting.color = '#4485F2';
    showMessagePopup('هدیه سفر', '', setting);
}

function hidePresentPopup() {
    if (presentPrizePopup) {
        presentPrizePopup.close();
    }
}

function check_user_login() {
    let isApp = $('.master_header-account').hasClass('is-app');
    myajax("user/isuserauthenticated", "", function (ret) {
        if (ret.val) {
            isUserLoggedIn = true;
            if (ret.impersonateData.state) {
                $('#js-impersonate-banner').show();
                $('#js-impersonate-fullname').html('ورود به عنوان ' + ret.impersonateData.fullName);
            }
            myajax("user/isuserloginbanned", "", function (ret) {
                $('.master_header-account').css('display', 'flex');
                if (ret.val) {
                    myajax("user/logoutajax", "", function (ret) { });
                    $('.master_header-account').attr('href', '#');
                    $('.master_header-account').attr('onclick', 'toggle_login()');
                    $('.master_header-account').children('p').html('<i class="fa fa-user"></i> ورود');
                    $('.js-present-prize-link').attr('href', '#');
                    $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
                }
                else {
                    if (isApp) {
                        $('.master_header-account').attr('href', "/app/home/dashboard");
                    }
                    else {
                        $('.master_header-account').attr('href', "/dashboard");
                    }
                    $('.master_header-account').attr('onclick', '');
                    $('.master_header-account').children('p').html('<i class="fa fa-user"></i> حساب من ');
                    current_user_id = ret.user_id;
                    if (Notification.permission !== "granted" &&
                        Notification.permission !== "denied") {
                        myajax('user/getpermissionrequestdate', '', function (ret) {
                            var date;
                            if (ret.ticks !== 0) {
                                var now_ticks = ((new Date().getTime() * 10000) + 621355968000000000);
                                var last_ticks = ret.ticks;
                                var diff = now_ticks - last_ticks;
                                var one_day_passed = diff > 864000000000;
                            }
                            if (ret.status === 0 || ret.ticks === 0 || one_day_passed) {
                                showNoYesMessage("امکان اطلاع رسانی رزرو", "کاربر گرامی، لطفا جهت اطلاع رسانی از مراحل رزرو، پیغامی را که بعد از تایید نمایش داده میشود را تایید فرمایید",
                                function () {
                                    messaging.requestPermission().then(function () {
                                        console.log('Notification permission granted.');
                                        // Get Instance ID token. Initially this makes a network call, once retrieved
                                        // subsequent calls to getToken will return from cache.
                                        messaging.getToken().then(function (currentToken) {
                                            if (currentToken) {
                                                sendTokenToServer(currentToken);
                                            } else {
                                                // Show permission request.
                                                console.log('No Instance ID token available. Request permission to generate one.');
                                                // Show permission UI.
                                                sendTokenToServer(null);
                                            }
                                        }).catch(function (err) {
                                            console.log('An error occurred while retrieving token. ', err);
                                            sendTokenToServer(null);
                                        });
                                    }).catch(function (err) {
                                        console.log('Unable to get permission to notify.', err);
                                        sendTokenToServer(null);
                                    });
                                }, function () {
                                    var t = ((new Date().getTime() * 10000) + 621355968000000000);
                                    myajax('user/setpermissionrequestdate', 'ticks=' + t, function (ret) { }, false);
                                }, { yesText: 'باشه', noText: 'بعدا سوال شود' });
                            }
                        }, false);
                    }
                    else if (Notification.permission === "granted") {
                        messaging.getToken().then(function (currentToken) {
                            if (currentToken) {
                                sendTokenToServer(currentToken);
                            } else {
                                // Show permission request.
                                console.log('No Instance ID token available. Request permission to generate one.');
                                // Show permission UI.
                                sendTokenToServer(null);
                            }
                        }).catch(function (err) {
                            console.log('An error occurred while retrieving token. ', err);
                            sendTokenToServer(null);
                        });
                    }
                    else {
                        sendTokenToServer(null);
                    }
                    //messaging.onMessage(function (payload) {
                    //    console.log('Message received. ', payload);
                    //    var actions = [];
                    //    if (payload.data.btn1) {
                    //        actions.push(
                    //            { action: payload.data.btn1, title: payload.data.btn1_title }
                    //        );
                    //    }
                    //    if (payload.data.btn2) {
                    //        actions.push(
                    //            { action: payload.data.btn2, title: payload.data.btn2_title }
                    //        );
                    //    }
                    //    if (payload.data.btn3) {
                    //        actions.push(
                    //            { action: payload.data.btn3, title: payload.data.btn3_title }
                    //        );
                    //    }
                    //    if (payload.data.btn4) {
                    //        actions.push(
                    //            { action: payload.data.btn4, title: payload.data.btn4_title }
                    //        );
                    //    }
                    //    var notificationOptions = {
                    //        body: payload.notification.body,
                    //        icon: '/Resource/img/siteicons/icon-144x144.png',
                    //        badge: '/Resource/img/siteicons/badge.png',
                    //        data: payload.data,
                    //        actions: actions,
                    //        vibrate: [200, 100, 200, 100, 200, 100, 200]
                    //    };
                    //    registered_service_worker.showNotification(payload.notification.title,
                    //        notificationOptions);
                    //});
                }
            }, false);
        }
        else {
            isUserLoggedIn = false;
            $('.master_header-account').css('display', 'flex');
            $('.master_header-account').attr('href', '#');
            $('.master_header-account').attr('onclick', 'toggle_login()');
            $('.master_header-account').children('p').html('<i class="fa fa-user"></i> ورود');
            $('.js-present-prize-link').attr('href', '#');
            $('.js-present-prize-link').attr('onclick', 'showPresentPopup()');
        }
    }, false);
    // Callback fired if Instance ID token is updated.
    messaging.onTokenRefresh(function () {
        messaging.getToken().then(function (refreshedToken) {
            console.log('Token refreshed.');
            // Send Instance ID token to app server.
            sendTokenToServer(refreshedToken);
        }).catch(function (err) {
            console.log('Unable to retrieve refreshed token ', err);
        });
    });
}

function chat_box_inputkeyup() {
    if (event.keyCode === 13) {
        sendSupportChatMessage();
    }
}

function showSupportChat() {
    hideSupportChatInform();
    var id = parseInt($('#js-support-chat-id').val());
    var url = "/supportchat/getchatpopup?id=" + id;
    $('.support-chat__chat-box').load(url, function () {
        if (chatInputIsOpen)
            openChatInput();
    });
    $('.support-chat__button').hide();
    $('.support-chat__container').show(500);
    $(".support-chat__text-input").focus();
    $('.installPopup').slideUp();
}

function showSupportChatInform() {
    $('.support-chat__user-inform').show(500);
}

function hideSupportChatInform() {
    $('.support-chat__user-inform').hide(500);
}

function hideSupportChat() {
    $('.support-chat__container').hide(500);
    $('.support-chat__button').show();
}

var chatInputIsOpen = false;

function openChatInput() {
    $('.support-chat__input-container').show();
    $(".support-chat__text-input").focus();
    chatInputIsOpen = true;
}

var is_sending_chat = false;

function sendSupportChatMessage(id, text, questionNumber) {
    if (id == undefined) {
        id = parseInt($('#js-support-chat-id').val());
    }
    if (is_sending_chat) {
        return;
    }
    if (typeof text === 'undefined' || text == null) {
        text = $(".support-chat__text-input").val();
    }
    if (text == '' || text == null || typeof text === 'undefined')
        return;
    if (!isUserLoggedIn) {
        showNoYesMessage('ورود به سایت',
            'برای چت با پشتیبانی ابتدا باید وارد سایت شوید', toggle_login, undefined,
            { yesText: 'ورود', noText: 'بستن' });
        return;
    }
    is_sending_chat = true;
    $(".support-chat__text-input").val("");
    $.ajax({
        type: "POST",
        url: "/supportchat/sendtextuser",
        data: {
            user_id: current_user_id,
            id: id,
            text: text,
        },
        success: function (ret) {
            is_sending_chat = false;
            if (ret.status == 1) {
                $('#js-support-chat-id').val(ret.id);
                portalHubConnection.invoke('reloadSupportChat', ret.id, 0, current_user_id);
                $(".support-chat__text-input").focus();
                $(".support-chat__chat-box").stop().animate({ scrollTop: $(".support-chat__chat-box")[0].scrollHeight }, 1000);
            }
            else {
                showErrorMessage('خطا',
                    'متاسفانه ارسال پیام با خطا مواجه شد');
            }
        }
    });
}

//function refreshSupporterName(id){
//    if (id == undefined) {
//        id = parseInt($('#js-support-chat-id').val());
//    }
//    if (id > 0) {
//        myajax('supportchat/getsupportername','id=' + id,function(ret){
//            $('#js-supporter-label').html('پشتیبان: ' + ret.name);
//        },false);
//    }
//}

function refreshChatBox(id, newCount) {
    if ($('.support-chat__chat-box').is(":visible")) {
        var url = "/supportchat/getchatpopup?id=" + id +
            "&questionlist=" + JSON.stringify(messageQuestionList);
        $(".support-chat__chat-box").load(url);
        $(".support-chat__chat-box").stop().animate({ scrollTop: $(".support-chat__chat-box")[0].scrollHeight }, 1000);
    }
    newCount = parseInt(newCount);
    if (newCount > 0) {
        $('.support-chat__new-count').html(newCount);
        $('.support-chat__new-count').show();
    }
    else {
        $('.support-chat__new-count').hide();
    }
    var chatPopupChildren = $('.chat-popup__container').children();
    chatPopupChildren[chatPopupChildren.length-1].scrollIntoView();
}

var iOS = false,
p = navigator.platform;
if (p === 'iPad' || p === 'iPhone' || p === 'iPod') {
    iOS = true;
}
//if (iOS === false) {
    //let deferredPrompt;
    //window.addEventListener('beforeinstallprompt', event => {

    //    // Prevent Chrome 67 and earlier from automatically showing the prompt
    //    event.preventDefault();

    //    // Stash the event so it can be triggered later.
    //    deferredPrompt = event;

    //    // Attach the install prompt to a user gesture
    //    $('.installBtn').click(function () {
    //        // Show the prompt
    //        deferredPrompt.prompt();

    //        // Wait for the user to respond to the prompt
    //        deferredPrompt.userChoice
    //          .then((choiceResult) => {
    //              if (choiceResult.outcome === 'accepted') {
    //                  $('.installBanner').css('display', 'none');
    //                  $('.installPopup').slideUp();
    //                  console.log('User accepted the A2HS prompt');
    //              } else {
    //                  console.log('User dismissed the A2HS prompt');
    //              }
    //              deferredPrompt = null;
    //          });
    //    });
        // Update UI notify the user they can add to home screen
        //$('.installBanner').css('display', 'flex');
        //$('.installPopup').slideDown();
    //});
    //document.addEventListener("scroll", function () {
    //    if (window.pageYOffset > 1000) {
    //        $('.installPopup').slideUp();
    //    }
    //});
//}