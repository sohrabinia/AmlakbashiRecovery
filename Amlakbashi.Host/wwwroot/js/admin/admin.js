$(".profile-style").click(function () {
    $(".box-setting-profile").slideToggle("fast");
});

$(document).click(function () {
    if ($(window).width() < 960) {
        closeNavigation();
    }
});

$('.js-berger-menu').click(function (event) {
    $('nav.dashboard__navigatoin').toggleClass("js-show-menu");
    $('.js-bg').toggleClass("bg-show-menu");
    if ($('nav.dashboard__navigatoin').hasClass("js-show-menu")) {
        $('nav.dashboard__navigatoin').animate({ scrollTop: 0 });
    }
    event.stopPropagation();
});

$(".main-navigation_list-item").click(function (event) {
    if ($(this).children().hasClass("style-submenu")) {
        event.stopPropagation();
    }
});

if ($(window).width() < 960) {
        $('p.dashboard-left-account').html("خروج");
}

$(".main-navigation_list-item").click(function () {
    $(this).children(".style-submenu").toggleClass("js-submenu");
    $(this).toggleClass("active-item");
    if (!$(this)) {
        $(".main-navigation_list-item").removeClass("active-item");
        $(".main-navigation_list-item").children(".style-submenu").addClass("js-submenu");
    }
});

$(".submenu-child-item").click(function () {
    closeNavigation();
});

function closeNavigation(){
    $('nav.dashboard__navigatoin').addClass("js-show-menu");
    $('.js-bg').removeClass("bg-show-menu");
    $(".main-navigation_list-item").removeClass("active-item");
    $(".main-navigation_list-item").children(".style-submenu").addClass("js-submenu");
};

function showDarkBackground() {
    //$('.js-bg').addClass("bg-show-menu");
    $('.js-loader').show();
    //$('body').css("overflow", "hidden");
}

function hideDarkBackground() {
    //$('.js-bg').removeClass("bg-show-menu");
    $('.js-loader').hide();
    //$('body').css("overflow", "auto");
}

// more filter popup
$(".more-filter").click(function (event) {
    $(".more-filter-container").fadeIn(100);
});

$(".more-filter-container .button-exit").click(function () {
    $(".more-filter-container").fadeOut(100);
});

// collapse detail container
function loadCollapse(callerButton, targetContainer, url) {
    var isOpen = $(targetContainer).children().length > 0;
    if (isOpen == false) {
        $("i", callerButton).toggleClass("fa-plus-square fa-spinner", 5000);
        $(targetContainer).load(url, function () {
            $(targetContainer).toggleClass("js-more-option", 800);
            $("i", callerButton).toggleClass("fa-spinner fa-minus-square", 5000);
        });
    }
    else {
        $(targetContainer).empty();
        $(targetContainer).toggleClass("js-more-option", 800);
        $("i", callerButton).toggleClass("fa-minus-square fa-plus-square", 5000);
    }
}

// popup
let popupContainer = $(".main .popup-container");
let popupContent = $(".main .popup-container .popup-content");
let popupMain = $(".main .popup-container .popup-content .popup-main");
let popupLoader = $(".main .popup-container .popup-loader");
let popupBackBtn = $(".main .popup-container .popup-header .popup-back-btn");
let popupPageStack = [];
function loadPopup(url) {
    if (popupPageStack.indexOf(url) === -1) {
        popupPageStack.push(url);
    }
    if (popupPageStack.length > 1) {
        popupBackBtn.show();
    }
    popupLoader.show();
    popupContainer.fadeIn(100);
    popupMain.load(url, function (responseTxt, statusTxt, xhr) {
        popupLoader.hide();
        if (statusTxt == "success") {
            // do somethings
        }
        if (statusTxt == "error") {
            hidePopup();
            if (xhr.status === 401) {
                errorAlert("شما مجوز دسترسی به این قسمت را ندارید");
            }
            else {
                errorAlert("عملیات با خطا مواجه شد");
            }
        }
    });
}
function showPopup(content) {
    popupMain.html(content);
    popupContainer.fadeIn(100);
}
function hidePopup() {
    popupContainer.fadeOut(100, function () {
        popupMain.empty();
        popupPageStack = [];
        popupBackBtn.hide();
    });
}
function backPopup() {
    popupPageStack.pop();
    let lastIndex = popupPageStack.length - 1;
    if (lastIndex >= 0) {
        loadPopup(popupPageStack[lastIndex]);
    }
    if (lastIndex < 1) {
        popupBackBtn.hide();
    }
}
function submitPopup(url, successCallback) {
    let form = popupMain.find('form');
    if (form) {
        let formData = form.serialize();
        sendPostAjax(url, formData, successCallback, function () { popupLoader.show(); }, hidePopup);
        //sendPostAjax(url, formData, successCallback, null, hidePopup);
    }
}

// confirm
let confirmContainer = $(".main .confirm-container");
let confirmContent = $(".main .confirm-container .confirm-content");
let confirmMain = $(".main .confirm-container .confirm-content .confirm-main");
let confirmAcceptCallback;
function showConfirm(content, acceptCallback) {
    confirmMain.html(content);
    if (typeof acceptCallback === 'function') {
        confirmAcceptCallback = acceptCallback;
    }
    confirmContainer.fadeIn(100);
}
function confirmReject(acceptCallback) {
    confirmContainer.fadeOut(100, function () {
        confirmMain.empty();
        confirmAcceptCallback = undefined;
        if (typeof acceptCallback === 'function') {
            acceptCallback();
        }
    });
}
function confirmAccept() {
    if (typeof confirmAcceptCallback === 'function') {
        confirmReject(confirmAcceptCallback);
    }
    else {
        confirmReject();
        errorAlert('اشکال در فراخوانی تابع');
    }
}

// alert
let alertContainer = $(".alert-container");
let alertContent = $(".alert-container .alert-content")
let alertMain = $(".alert-container .alert-content .alert-main");
let alertSign = $(".alert-container .alert-content .alert-sign");
function showAlert(content, color) {
    alertMain.html(content);
    if (color) {
        alertContent.css('border-color', color);
        alertSign.css('color', color);
    }
    alertContainer.fadeIn(100);
}
function alertClose() {
    alertContainer.fadeOut(100, function () {
        alertMain.empty();
    });
}
function errorAlert(content) {
    showAlert(content, '#FF3C3C');
}
function successAlert(content) {
    showAlert(content, '#50C878');
}

// ajax
function sendPostAjax(url, data, successCallback, beforeSendCallback, completeCallback) {
    sendAjaxRequest(url, data, 'post', successCallback, beforeSendCallback, completeCallback);
}

function sendGetAjax(url, data, successCallback, beforeSendCallback, completeCallback) {
    sendAjaxRequest(url, data, 'get', successCallback, beforeSendCallback, completeCallback);
}

function sendAjaxRequest(url, data, type, successCallback, beforeSendCallback, completeCallback) {
    $.ajax({
        url: url,
        type: type,
        data: data,
        cache: false,
        beforeSend: function () {
            if (typeof beforeSendCallback === 'function') {
                beforeSendCallback();
            }
            else {
                showDarkBackground();
            }
        },
        success: function (response) {
            if (typeof successCallback === 'function') {
                successCallback(response);
            }
            else {
                if (response.status === 1) {
                    successAlert('عملیات با موفقیت انجام شد');
                }
                else {
                    errorAlert(response.msg);
                }
            }
        },
        error: function (error) {
            if (error.status === 401) {
                errorAlert('شما مجوز دسترسی به این قسمت را ندارید');
            }
            else {
                errorAlert('عملیات با خطا مواجه شد<br/>' + error.status + ' - ' + error.statusText + '<br/>' + error.responseText);
            }
        },
        complete: function (response) {
            hideDarkBackground();
            if (typeof completeCallback === 'function') {
                completeCallback();
            }
        }
    });
}