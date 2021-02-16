$(".js-detail-button").each(function () {
    registerDetailButton(this);
});
function registerDetailButton(btn) {
    $(btn).click(function () {
        var current_state = $(this).attr('data-state') == '1';
        if (!current_state) {
            $('.js-detail-button').each(function () {
                if (!$(this).hasClass('js-stay-expand')) {
                    detailButtonChangeState(this, false);
                }
            });
        }
        detailButtonChangeState(this, !current_state);
    });
}
function detailButtonChangeState(elem, target_state) {
    $(elem).attr('data-state', target_state ? '1' : '0');
    var $target_elem = $('#' + $(elem).attr('data-target-id'));
    var change_text = $(this).attr('data-change-text') == '1';
    if (change_text) {
        $(elem).children('span').html(target_state ? $(this).attr('data-text-collapse') : $(this).attr('data-text-expand'));
    }
    var expand_icon = $(elem).children('i').attr('data-expand-icon');
    var collapse_icon = $(elem).children('i').attr('data-collapse-icon');
    if (expand_icon == undefined || expand_icon == null) {
        expand_icon = 'fa fa-angle-double-down';
    }
    if (collapse_icon == undefined || collapse_icon == null) {
        collapse_icon = 'fa fa-angle-double-up';
    }
    $(elem).children('i').attr('class', target_state ? collapse_icon : expand_icon);
    if (target_state) {
        $target_elem.slideDown();
    }
    else {
        $target_elem.slideUp();
    }
}