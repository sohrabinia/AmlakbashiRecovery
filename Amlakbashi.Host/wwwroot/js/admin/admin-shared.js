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

function loadHtmlContent(callerButton, targetContainer, url) {
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

function loadPopup(url) {
    showDarkBackground();
    var popupWindow = $(".admin-table .admin-table-row .edit-box");
    popupWindow.css("display", "flex");
    popupWindow.load(url);
}

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