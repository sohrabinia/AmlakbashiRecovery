function showSuccessAlert(title, message, onClose) {
    $.confirm({
        title: title,
        content: message,
        type: 'green',
        escapeKey: "بستن",
        typeAnimated: true,
        rtl: true,
        buttons: {
            بستن: {
                keys: ['enter']
            }
        },
        onClose: function () {
            if (typeof onClose !== 'undefined' && onClose != null) {
                onClose();
            }
        }
    });
}
function showNormalAlert(title, message, onClose, onOpen, onContentReady) {
    return $.confirm({
        title: title,
        content: message,
        escapeKey: "بستن",
        type: 'blue',
        typeAnimated: true,
        rtl: true,
        buttons: {
            بستن: {
            }
        },
        onClose: function () {
            if (typeof onClose !== 'undefined' && onClose != null) {
                onClose();
            }
        },
        onOpen: function () {
            if (typeof onOpen !== 'undefined' && onOpen != null) {
                onOpen();
            }
        },
        onContentReady: function () {
            if (typeof onContentReady !== 'undefined' && onContentReady != null) {
                onContentReady();
            }
        }
    });
}
function showWarningAlert(title, message, onClose) {
    $.confirm({
        title: title,
        content: message,
        type: 'yellow',
        escapeKey: "بستن",
        typeAnimated: true,
        rtl: true,
        buttons: {
            بستن: function () {
            }
        },
        onClose: function () {
            if (typeof onClose !== 'undefined' && onClose != null) {
                onClose();
            }
        }
    });
}
function showErrorAlert(title, message, onClose) {
    $.confirm({
        title: title,
        content: message,
        type: 'red',
        escapeKey: "بستن",
        typeAnimated: true,
        rtl: true,
        buttons: {
            بستن: function () {
            }
        },
        onClose: function () {
            if (typeof onClose !== 'undefined' && onClose != null) {
                onClose();
            }
        }
    });
}
function showConfirmAlert(title, message, onYes, onNo, onClose, yesText, noText) {
    if (yesText === 'undefined' || yesText == null) {
        yesText = "بله";
    }
    if (noText === 'undefined' || noText == null) {
        noText = "خیر";
    }
    $.confirm({
        title: title,
        escapeKey: 'no',
        content: message,
        type: 'blue',
        typeAnimated: true,
        rtl: true,
        buttons: {
            yes: {
                text: yesText,
                btnClass: 'btn-green',
                keys: ['enter'],
                action: function () {
                    if (typeof onYes !== 'undefined' && onYes != null) {
                        onYes();
                    }
                }
            },
            no: {
                text: noText,
                btnClass: 'btn-red',
                action: function () {
                    if (typeof onNo !== 'undefined' && onNo != null) {
                        onNo();
                    }
                }
            }
        },
        onClose: function () {
            if (typeof onClose !== 'undefined' && onClose != null) {
                onClose();
            }
        }
    });
}