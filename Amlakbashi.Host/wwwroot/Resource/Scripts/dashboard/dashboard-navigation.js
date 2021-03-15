
$(document).click(function () {
    $('nav.dashboard__navigatoin').addClass("js-show-menu");
    $('.js-bg').removeClass("bg-show-menu");
    $('body').css("overflow", "auto");
})
$('.js-berger-menu').on('click', function (event) {
    $('nav.dashboard__navigatoin').toggleClass("js-show-menu");
    $('.js-bg').toggleClass("bg-show-menu");
    if ($('nav.dashboard__navigatoin').hasClass("js-show-menu")) {
        $('nav.dashboard__navigatoin').animate({ scrollTop: 0 });
    }
    if (!$('nav.dashboard__navigatoin').hasClass("js-show-menu")) {
        $('body').css("overflow", "hidden");
    } 
    event.stopPropagation();
})