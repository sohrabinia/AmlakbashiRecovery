function show_loading_icon() {
    $("#loading-icon").fadeIn();
}

function hide_loading_icon() {
    $("#loading-icon").fadeOut();
}

$(window).focus(function () {
    hide_loading_icon();
});

$(document).ajaxComplete(function () {
    hide_loading_icon();
});