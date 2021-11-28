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
            errorAlert("عملیات با خطا مواجه شد");
        }
    });
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

// confirm
let confirmContainer = $(".main .confirm-container");
let confirmContent = $(".main .confirm-container .confirm-content");
let confirmMain = $(".main .confirm-container .confirm-content .confirm-main");
let confirmAcceptCallback;
function showConfirm(content, acceptCallback) {
    confirmMain.html(content);
    if (acceptCallback !== undefined) {
        confirmAcceptCallback = acceptCallback;
    }
    confirmContainer.fadeIn(100);
}
function confirmReject() {
    confirmContainer.fadeOut(100, function () {
        confirmMain.empty();
        confirmAcceptCallback = undefined;
    });
}
function confirmAccept() {
    confirmAcceptCallback();
    confirmReject();
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

// submit forms
function submitForm(formId, url, successCallback, beforeCallback, completeCallback) {
    var formData = $('#' + formId).serialize();
    $.ajax({
        url: url,
        type: "post",
        data: formData,
        beforeSend: function () {
            if (beforeCallback != undefined) {
                beforeCallback();
            }
        },
        success: function (response) {
            if (successCallback != undefined) {
                successCallback(response);
            }
            else {
                if (response.status == 1) {
                    alertify.success("عملیات با موفقیت انجام شد");
                }
                else {
                    alertify.error(response.msg);
                }
            }
        },
        error: function (error) {
            alertify.error("عملیات با خطا مواجه شد");
        },
        complete: function (response) {
            if (completeCallback != undefined) {
                completeCallback();
            }
        }
    })
}

function test() {
    successAlert("reza najmi");
}