
$(function () {
    refreshNumberInputs();
});

function refreshNumberInputs() {
    // add and sub number 
    $('.sub').click(function () {
        if ($(this).siblings('.js-input').val() > 0)
            $(this).siblings('.js-input').val(+$(this).siblings('.js-input').val() - 1).trigger("input");
    });
    $('.add').click(function () {
        $(this).next().val(+$(this).next().val() + 1).trigger("input");
    });
}
