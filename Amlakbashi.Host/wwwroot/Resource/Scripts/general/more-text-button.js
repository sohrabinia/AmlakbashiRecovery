document.addEventListener("DOMContentLoaded", function () {
    $(".show-more-container").each(function () {
        var max_height = $(this).attr("data-max-height");
        if ($(this).height() > max_height) {
            $(this).css("max-height", max_height + "px");
            $(this).css("overflow", "hidden");
            $(this).attr("data-current-state", false);
            $(this).after("<div class='show-more__button' onclick='toggleShowMoreText(this)'>نمایش کل متن <i class='fa fa-angle-down'></i></div>");
        }
    });
});


function toggleShowMoreText(button) {
    var $elem = $(button).prev();
    var currnet_state = $elem.attr("data-current-state");
    if (currnet_state == "true") {
        $(button).html("نمایش کل متن <i class='fa fa-angle-down'></i>");
        $elem.animate({
            "max-height": $elem.attr("data-max-height") + "px",
        }, 1000, function () {
            // Animation complete.
        });
        $elem.attr("data-current-state", false);
    }
    else {
        $(button).html("کوتاه کردن متن <i class='fa fa-angle-up'></i>");
        $elem.animate({
            "max-height": "2000px",
        }, 1000, function () {
            // Animation complete.
        });
        $elem.attr("data-current-state", true);
    }
}