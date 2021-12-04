const reserveAdminHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/reserveadminhub")
    .build();

reserveAdminHubConnection.on('addSupporterInfo', (reserve_id, text) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        var infoCountElem = reserveElem.find('.support-desc-btn small');
        var newCount = parseInt(infoCountElem.html()) + 1;
        infoCountElem.html(newCount);
        if (text.includes('توسط سیستم با میزبان تماس گرفته شد')) {
            reserveElem.find('.js-system-called-host').css('color', 'limegreen');
        }
        else if (text.includes('توسط سیستم با مهمان تماس گرفته شد')) {
            reserveElem.find('.js-system-called-guest').css('color', 'limegreen');
        }
    }
});

reserveAdminHubConnection.on('toggleShouldFollow', (reserve_id, new_status) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        var shouldFollowBtn = reserveElem.find('.should-follow-btn');
        if (new_status) {
            shouldFollowBtn.css('color', 'limegreen');
        }
        else {
            shouldFollowBtn.css('color', '');
        }
    }
});

reserveAdminHubConnection.on('changeStatus', (reserve_id, status_string, status_color) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        statusLabel = reserveElem.find(".status");
        statusLabel.html(status_string);
        statusLabel.css('color', status_color);
    }
});

reserveAdminHubConnection.on('payReserveWithCreditHost', (reserve_id) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        reserveElem.find('.pay-reserve-with-credit-host-btn').remove();
    }
});

reserveAdminHubConnection.on('reserveCleared', (reserve_id) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        reserveElem.find('.clearing-btn').remove();
    }
});

reserveAdminHubConnection.on('reserveRefunded', (reserve_id) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        reserveElem.find('.refund-btn').remove();
    }
});

reserveAdminHubConnection.on('chatRead', (reserve_id, count) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        let chatBtn = reserveElem.find('.chat-info-btn small');
        chatBtn.css('color', '');
        chatBtn.html(count);
    }
});

reserveAdminHubConnection.on('changeChatCount', (reserve_id, count, notReadCount) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        var chatBtn = reserveElem.find('.chat-info-btn small');
        if (count == 0) {
            chatBtn.css('display', 'none');
        }
        else {
            chatBtn.css('display', 'unset');
            if (notReadCount > 0) {
                chatBtn.css('color', 'red');
                chatBtn.html('!');
            }
            else {
                chatBtn.css('color', '');
                chatBtn.html(count);
            }
        }
    }
});

reserveAdminHubConnection.on('reserveSupporterAdded', (reserve_id, supporterName, supporterPhoto) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        let supportStateElem = reserveElem.find('.support-state');
        supportStateElem.html('در حال پشتیبانی');
        reserveElem.find('.status-spport').append('<div style="display:flex; flex-flow:row; justify-content: center; align-items:center;">'
            + (supporterPhoto != '' ?
                ('<img src="' + supporterPhoto + '" />')
                : '<i class="fa fa-user-circle"></i>')
            + '<div>' + supporterName + '</div>'
            + '</div>');
    }
});

reserveAdminHubConnection.on('changeCallState', (reserve_id, hostOrGuest, new_state, new_state_color) => {
    var reserveElem = $('#js-' + reserve_id);
    if (reserveElem.length > 0) {
        reserveElem.find('.call-state-' + hostOrGuest).css("color", new_state_color);
        reserveElem.find('.call-state-' + hostOrGuest).attr('js-call-state', new_state);
    }
});

reserveAdminHubConnection.start()
    .then(() => console.log('reserve admin hub connected!'))
    .catch(console.error);