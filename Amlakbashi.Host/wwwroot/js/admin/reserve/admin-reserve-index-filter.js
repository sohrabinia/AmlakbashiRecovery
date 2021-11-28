function toggleMyReserves(target) {
    var $input_elem = $('#show_my_reserves_input');
    $input_elem.val(target);
    $('#more_filter_form').submit();
}

function toggleShouldFollowReserves(shouldFollow) {
    var $input_elem = $('#should_follow_input');
    $input_elem.val(shouldFollow);
    $('#more_filter_form').submit();
}

function toggleDisableAutoCancelFilter(disableAutoCancel) {
    var $input_elem = $('#disableAutoCancelFilter');
    $input_elem.val(disableAutoCancel);
    $('#more_filter_form').submit();
}

function toggleAccVisitFilter(accVisited) {
    var $input_elem = $('#accVisitedFilter');
    $input_elem.val(accVisited);
    $('#more_filter_form').submit();
}

function toggleStatus(target) {
    if ($('select#reserve-status-input').val() == target.toString()) {
        target = -1;
    }
    var $option = $('select#reserve-status-input option[value="' + target + '"]');
    $option.attr('selected', 'selected');
    $('#more_filter_form').submit();
}

function toggleMainFilter(target) {
    var $input_elem = $('#main_filter_input');
    $input_elem.val(target);
    $('#filter_form').submit();
}

function toggleInstantResFilter(target) {
    var $input_elem = $('#instantReserveFilter');
    $input_elem.val(target);
    $('#filter_form').submit();
}

function toggleTodayFinishStay(today) {
    var $input = $('#reserve_end_date_picker');
    if (today == $input.val() && $('select#general_status_input').val() == "0") {
        $input.val('');
        var $option = $('select#general_status_input option[value="' + "-1" + '"]');
        $option.attr('selected', 'selected');
    }
    else {
        $input.val(today);
        var $option = $('select#general_status_input option[value="' + "0" + '"]');
        $option.attr('selected', 'selected');
    }
    $('#more_filter_form').submit();
}

function toggleSiteTodayClearing(today) {
    var $date_input = $('#clearing_date_picker');
    new_status = 1;
    new_date = today;
    if ($('select#cleared_status_input').val() == "1" &&
        $date_input.val() == today) {
        new_status = -1;
        new_date = ''
    }
    var $option = $('select#cleared_status_input option[value="' + new_status + '"]');
    $option.attr('selected', 'selected');
    $date_input.val(new_date);
    $('#more_filter_form').submit();
}