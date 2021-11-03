// Declare a proxy to reference the hub.
const reserveAdminHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/reserveadminhub")
    .build();

// Create a function that the hub can call to broadcast messages.
reserveAdminHubConnection.on('addSupporterInfo', (reserve_id, text) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        var $info_count_elem = $reserve_elem.find('.js-info-count');
        var new_count = parseInt($info_count_elem.html().match(/\d+/)[0]) + 1;
        $reserve_elem.find('.js-info-count').html('(' + new_count + ' توضیح)');
        if (text.includes('توسط سیستم با میزبان تماس گرفته شد')) {
            $reserve_elem.find('.js-system-called-host').css('color', '#34A853');
        }
        else if (text.includes('توسط سیستم با مهمان تماس گرفته شد')) {
            $reserve_elem.find('.js-system-called-guest').css('color', '#34A853');
        }
    }
});

reserveAdminHubConnection.on('toggleShouldFollow', (reserve_id, new_status) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        var $_elem = $reserve_elem.find('.should-follow-button');
        if (new_status) {
            $_elem.css('color', '#34A853');
        }
        else {
            $_elem.css('color', '#242424');
        }
    }
});

reserveAdminHubConnection.on('changeStatus', (reserve_id, status_string, status_color) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $status_label = $reserve_elem.find(".reserve-status-label");
        $status_label.html(status_string);
        $status_label.css('color', status_color);
    }
});

reserveAdminHubConnection.on('deleteReserve', (reserve_id) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.remove();
    }
});

reserveAdminHubConnection.on('payReserveWithCreditHost', (reserve_id) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.payReserveWithCreditHost').remove();
    }
});

reserveAdminHubConnection.on('reserveCleared', (reserve_id) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.clearing_button').remove();
    }
});

reserveAdminHubConnection.on('reserveRefunded', (reserve_id) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.refund_button').remove();
    }
});

reserveAdminHubConnection.on('chatRead', (reserve_id, count) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.js-chat-info').css('background-color', '#4485F2');
        $reserve_elem.find('.js-chat-info').html(count);
    }
});

reserveAdminHubConnection.on('changeChatCount', (reserve_id, count, notReadCount) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        var $chat_info = $reserve_elem.find('.js-chat-info');
        if (count == 0) {
            $chat_info.css('display', 'none');
        }
        else {
            $chat_info.css('display', 'unset');
            if (notReadCount > 0) {
                $chat_info.css('background-color', '#EA4335');
                $chat_info.html('!');
            }
            else {
                $chat_info.css('background-color', '#4485F2');
                $chat_info.html(count);
            }
        }
    }
});

reserveAdminHubConnection.on('reserveSupporterAdded', (reserve_id, supporterName, supporterPhoto) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.support_state').css("color", "#34A853");
        $reserve_elem.find('.support_state').html('در حال پشتیبانی');

        $reserve_elem.find('.support_state_td').append('<div style="display:flex;flex-flow:row; justify-content: center; align-items:center;">'
            + (supporterPhoto != '' ?
                ('<img style="border-radius: 90px" width="20" height="20" src="' + supporterPhoto + '" />')
                : '<i class="fa fa-user-circle" style="font-size:20px"></i>')
            + '<div style="margin: 0 5px;color:#34A853;">' + supporterName + '</div>'
            + '</div>');
    }
});

reserveAdminHubConnection.on('changeCallState', (reserve_id, hostOrGuest, new_state, new_state_color) => {
    var $reserve_elem = $('#js-' + reserve_id);
    if ($reserve_elem.length > 0) {
        $reserve_elem.find('.call_state_' + hostOrGuest).css("color", new_state_color);
        $reserve_elem.find('.call_state_' + hostOrGuest).attr('js-call-state', new_state);
    }
});

// Start the connection.
reserveAdminHubConnection.start()
    .then(() => console.log('reserve admin hub connected!'))
    .catch(console.error);