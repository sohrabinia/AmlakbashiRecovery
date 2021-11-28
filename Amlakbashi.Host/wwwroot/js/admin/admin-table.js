$(".more-filter").click(function (event) {
    $(".more-filter-container").css("display", "flex");
    showDarkBackground();
    event.stopPropagation();
});

function showAdminDescriptionWindow(elem) {
    $(elem).parent().siblings(".box-description-suport").css("display", "flex");
    showDarkBackground();
    event.stopPropagation();
}

function hideAdminDescriptionWindow(elem) {
    $(".box-description-suport").css("display", "none");
    hideDarkBackground();
}

$(".view-description-support").click(function (event) {
    showAdminDescriptionWindow(this);
});

$(".exit-description-support").click(function () {
    hideAdminDescriptionWindow(this);
});

$(".exit-box-edit").click(function () {
    hideAdminEditWindow(this);
});

$(".button-exit").click(function () {
    $(".more-filter-container").css("display", "none");
    hideDarkBackground();
});

function showDarkBackground() {
    $('.js-bg').addClass("bg-show-menu");
    $('body').css("overflow", "hidden");
}

function hideDarkBackground() {
    $('.js-bg').removeClass("bg-show-menu");
    $('body').css("overflow", "auto");
}