//settings: color, buttons, onClose, contentUrl, onContentReady, onOpen, disableKeyEvents, fullScreen, yesText, noText, autoClose

var isLoadingContent = false;
var msgPopupKeyPressEnabled = true;

function showInfoMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#4485F2';
    return showMessagePopup(title, content, setting);
}

function showErrorMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#EA4335';
    return showMessagePopup(title, content, setting);
}

function showSuccessMessage(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    setting.color = '#34A853';
    return showMessagePopup(title, content, setting);
}

function showNoYesMessage(title, content, onYes, onNo, setting) {
    if (setting == undefined) {
        setting = {};
    }
    var buttons = [{
        title: setting.noText != undefined ? setting.noText : 'خیر',
        color: 'white',
        bgColor: '#EA4335',
        onclick: onNo
    },
    {
        title: setting.yesText != undefined ? setting.yesText : 'بله',
        color: 'white',
        bgColor: '#34A853',
        onclick: onYes
    }];
    setting.buttons = buttons;
    setting.color = '#4485F2';
    return showMessagePopup(title, content, setting);
}

function showMessagePopup(title, content, setting) {
    if (setting == undefined) {
        setting = {};
    }
    if (setting.autoClose == undefined) {
        setting.autoClose = true;
    }
    if (setting.color == undefined) {
        setting.color = '#fdd835';
    }
    if (setting.buttons == undefined) {
        setting.buttons = [{
            title: 'بستن',
            color: '#242424',
            bgColor: '#ededed',
        }];
    }
    if (popupMsgList == undefined) {
        popupMsgList = [];
    }
    var msg = {
        buttons: setting.buttons,
        autoClose: setting.autoClose,
        onClose: setting.onClose,
        enterKeyDisabled: setting.enterKeyDisabled,
        elementId: 'js-msg-popup-' + (popupMsgList.length + 1),
        closePopup: function () {
            if (popupMsgList.length > 1) {
                $('#' + popupMsgList[popupMsgList.length - 2].elementId).css(
                    'z-index', 10000000000 + popupMsgList.length - 1);
            }
            $('#' + this.elementId).hide(100,
                function () {
                    $(this).remove();
                });
            if (this.onClose != undefined) {
                setTimeout(this.onClose, 200);
            }
            for (var i = 0; i < popupMsgList.length; i++) {
                if (popupMsgList[i] === this) {
                    popupMsgList.splice(i, 1);
                    i--;
                }
            }
            updateBlurBg();
        },
        btnClick: function (index, forceClose) {
            if ((forceClose || this.autoClose) && !isLoadingContent) {
                this.closePopup();
            }
            if (this.buttons[index].onclick != undefined) {
                setTimeout(this.buttons[index].onclick, 200);
            }
        },
        close: function () {
            this.btnClick(0, true);
        },
        disableKeyEvents: setting.disableKeyEvents
    };
    if (popupMsgList.length > 0) {
        $('#' + popupMsgList[popupMsgList.length - 1].elementId).css(
            'z-index', 2147483640);
    }
    popupMsgList.push(msg);
    var $blurBg = getBlurBg();
    var $parent = getMessageParent();
    $parent.append('<div style="margin-top:' + (30 * (popupMsgList.length - 1)) + 'px;" id="' + msg.elementId + '" class="message-popup__message-container"></div>')
    var $elem = $('#' + msg.elementId);
    $elem.css('display', 'none');
    $elem.css('z-index', 10000000000 + popupMsgList.length);
    var fullScreen = setting.fullScreen = undefined ? false : setting.fullScreen;
    if (fullScreen) {
        $elem.css('width', '100%');
        $elem.css('top', 0);
        $elem.css('bottom', 0);
        $elem.css('display', 'flex');
        $elem.css('flex-flow', 'column');
        $elem.css('justify-content', 'space-between');
        $elem.css('background-color', 'white');
    }
    $elem.load('/post/getmessagepopup?fullScreen=' + fullScreen, function () {
        var $titleElem = $elem.find('.message-popup__message-title');
        var $contentElem = $elem.find('.message-popup__message-content');
        var $buttonContainer = $elem.find('.message-popup__message-button-container');
        $titleElem.css('background-color', setting.color);
        $titleElem.css('color', 'white');
        setting.buttons.forEach(function (btn, index) {
            $buttonContainer.append('<div ' +
                'style="color:' + btn.color + ';background-color:' + btn.bgColor + '" ' +
                'onclick="popupMsgList[' + (popupMsgList.length - 1) + '].btnClick(' + index + ', ' + (index == 0 ? "true" : "false") + ')"' +
                'class="message-popup__message-button">' +
                btn.title + '</div>');
        });
        $titleElem.html(title);
        if (setting.contentUrl != undefined) {
            msgPopupShowLoading();
            isLoadingContent = true;
            $contentElem.load(setting.contentUrl, function () {
                $blurBg.css('z-index', 10000000000 + popupMsgList.length);
                $blurBg.show(100, function () {
                    isLoadingContent = false;
                });
                msgPopupHideLoading();
                if (setting.onContentReady != undefined) {
                    setting.onContentReady();
                }
                if (setting.onOpen != undefined) {
                    setting.onOpen();
                }
            })
        }
        else {
            $contentElem.html(content);
            isLoadingContent = true;
            $blurBg.show(100, function () {
                isLoadingContent = false;
            });
            if (setting.onOpen != undefined) {
                setting.onOpen();
            }
        }
        $elem.show(100);
    });
    return msg;
}

function updateBlurBg() {
    if (popupMsgList.length < 1) {
        $('.message-popup__blur-bg').hide(100);
    }
}

function getBlurBg() {
    var $elem = $('.message-popup__blur-bg');
    if ($elem.length == 0) {
        $('body').prepend('<div class="message-popup__blur-bg" style="z-index:9999999999"></div>');
        $elem = $('.message-popup__blur-bg');
    }
    return $elem;
}
function getMessageParent() {
    var $elem = $('.message-popup__parent');
    if ($elem.length == 0) {
        $('body').append('<div class="message-popup__parent"></div>');
        $elem = $('.message-popup__parent');
    }
    return $elem;
}

function ButtonClickLastMessage(btnIndex, forceClose) {
    var msg = popupMsgList[popupMsgList.length - 1];
    if (msg.buttons != undefined && msg.buttons.length > btnIndex) {
        msg.btnClick(btnIndex, forceClose);
    }
}
document.addEventListener("keydown", msgPopupKeyDown, false);

function msgPopupKeyDown(e) {
    if (msgPopupKeyPressEnabled == false)
        return;
    if (popupMsgList == undefined || popupMsgList.length == 0) {
        return;
    }
    var msg = popupMsgList[popupMsgList.length - 1];
    if (msg.disableKeyEvents) {
        return;
    }
    var key = e.which;
    if (key == 13) {
        if (!msg.enterKeyDisabled) {
            if (msg.buttons.length == 1) {
                ButtonClickLastMessage(0);
            }
            else {
                ButtonClickLastMessage(1);
            }
        }
    }
    else if (key == 27) {
        ButtonClickLastMessage(0, true);
    }
}

function msgPopupShowLoading() {
    $('.message-popup__loading').show();
}

function msgPopupHideLoading() {
    $('.message-popup__loading').hide();
}

var popupMsgList;