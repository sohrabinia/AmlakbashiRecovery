function copyStringToClipboard(str, name) {
    // Create new element
    var el = document.createElement('textarea');
    // Set value (string to be copied)
    el.value = str;
    // Set non-editable to avoid focus and move outside of view
    el.setAttribute('readonly', '');
    el.style = { position: 'absolute', left: '-9999px' };
    document.body.appendChild(el);
    // Select text inside element
    el.select();
    // Copy text to clipboard
    document.execCommand('copy');
    // Remove temporary element
    document.body.removeChild(el);
    alertify.success(name + " کپی شد");
}

function deleteItem($id, obj) {
    if (confirm("آیا از حذف این آیتم اطمینان دارید ؟")) {
        myajax("Reserve/Delete", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                reserveAdminHubConnection.invoke("deleteReserve", $id);
                alertify.success('رزرو با کد ' + $id + ' حذف شد');
            }
            else {
                showErrorMessage('خطا', ret.val);
            }
        });
    }
}

function callForRequest($id) {
    if (confirm("آیا با میزبان، تماس پاسخ درخواست رزرو گرفته شود؟")) {
        myajax("Reserve/CallForRequest", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                showSuccessMessage("", "تماس با میزبان انجام شد", {
                    onClose: function () {
                        addInfo($id, "توسط سیستم با میزبان تماس گرفته شد", true);
                    }
                }
                );
            }
            else if (ret.stat == 2) {
                showInfoMessage("", "میزبان هم اکنون به درخواست رزرو، پاسخ داده است. نیازی به ارسال تماس نیست.");
            }
            else {
                showErrorMessage("خطا", "متاسفانه عملیات با خطای فنی مواجه شد.");
            }
        });
    }
}

changeSupporterReason = '';

function doSupport($id, elem, force, transfer_reason) {
    force = force == undefined ? false : force;
    if (force) {
        if (transfer_reason == null || transfer_reason == '') {
            showErrorMessage('خطا', 'لطفا دلیل انتقال پشتیبانی را وارد کنید');
            return;
        }
    }
    if (confirm("آیا قصد دارید این رزرو را پشتیبانی کنید؟")) {
        myajax("Reserve/DoSupport", "reserve_id=" + $id + "&force=" + force + '&transfer_reason=' + transfer_reason, function (ret) {
            if (ret.status == 1) {
                $(elem).remove();
                reserveAdminHubConnection.invoke('reserveSupporterAdded', $id, ret.supporterName, ret.supporterPhoto);
            }
            else if (ret.status == 2) {
                showNoYesMessage('',
                    '<div style="color:red;font-size:20px;text-align:center">هشدار: ' + ret.msg + '</div>'
                    + '<div>لطفا دلیل انتقال پشتیبانی را بنویسید: <input type="text" onchange="changeSupporterReason = $(this).val();" /></div>',
                    function () {
                        doSupport($id, elem, true, changeSupporterReason);
                    })
            }
            else {
                showErrorMessage('خطا', ret.msg);
            }
        });
    }
}

function callForPayment($id) {
    if (confirm("آیا با مهمان تماس پرداخت رزرو گرفته شود؟")) {
        myajax("Reserve/CallForPayment", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                showSuccessMessage("", "تماس با مهمان انجام شد", {
                    onClose: function () {
                        addInfo($id, "توسط سیستم با مهمان تماس گرفته شد", true);
                    }
                });
            }
            else if (ret.stat == 2) {
                showInfoMessage("", "مهمان هم اکنون پرداخت کرده است. نیازی به ارسال تماس نیست.");
            }
            else {
                showErrorMessage("خطا", "متاسفانه عملیات با خطای فنی مواجه شد.");
            }
        });
    }
}

function cancelBySystem($id, obj) {
    if (confirm("آیا درخواست رزرو توسط سیستم لغو شود؟")) {
        myajax("Reserve/CancelBySystem", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                reserveAdminHubConnection.invoke('cancelReserve', $id);
                alert("درخواست رزرو لغو شد");
            }
            else if (ret.stat == 2) {
                alert("مهمان/میزبان هم اکنون پاسخ داده است. نیازی به لغو رزرو نیست.");
            }
            else {
                alert("متاسفانه عملیات با خطای فنی مواجه شد.");
            }
        });
    }
}

function showSiteClearingHostPopup($id) {
    showInfoMessage('واریز به حساب کاربر', '',
        { contentUrl: '/Reserve/GetSiteClearingHostInfo?reserveId=' + $id, fullScreen: screen.width < 781 }
    );
}

function showSiteRefundGuestPopup($id) {
    showInfoMessage('واریز به حساب کاربر', '',
        { contentUrl: '/Reserve/GetSiteRefundGuestInfo?reserveId=' + $id, fullScreen: screen.width < 781 }
    );
}

function siteClearingHostWithCredit($id, obj) {
    $.confirm({
        title: '',
        content: 'آیا سهم میزبان از طریق کیف پول تسویه شود؟',
        type: 'blue',
        smoothContent: true,
        rtl: true,
        typeAnimated: true,
        columnClass: 'small',
        animation: 'scale',
        closeIcon: true,
        buttons: {
            بله: function () {
                var type = 1;//ReservePayment.ReservePaymentType.GuestClearing
    myajax("Reserve/SiteClearingWithCredit", "reserve_id=" + $id, function (ret) {
        if (ret.status == 1) {
            $.confirm({
                title: '',
                content: ret.msg + ". آیا پیامک ارسال شود؟",
                type: 'green',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بله: function () {
                        myajax("Reserve/SendSiteClearingWithCreditSms", "reserve_id=" + $id + "&payable_price=" + ret.payable_price + "&transaction_id=" + ret.transaction_id, function () { });
                    },
                    نه: function () {
                        //nothing
                    }
                },

            });
            reserveAdminHubConnection.invoke('reserveCleared', $id);
        }
        else if (ret.status == 0) {
            $.confirm({
                title: 'خطا',
                content: ret.msg,
                type: 'red',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بستن: function () {
                        //nothing
                    }
                }
            });
        }
    });
},
نه: function () {
}
                }
            });
        }

function siteRefundGuestWithCredit($id, obj) {
    $.confirm({
        title: '',
        content: 'آیا مبلغ رزرو از طریق کیف پول به مهمان عودت داده شود؟',
        type: 'blue',
        smoothContent: true,
        rtl: true,
        typeAnimated: true,
        columnClass: 'small',
        animation: 'scale',
        closeIcon: true,
        buttons: {
            بله: function () {
                var type = 1//ReservePayment.ReservePaymentType.GuestClearing;
    myajax("Reserve/SiteRefundGuestWithCredit", "reserve_id=" + $id, function (ret) {
        if (ret.status == 1) {
            $.confirm({
                title: '',
                content: ret.msg,
                type: 'green',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بستن: function () {
                        //nothing
                    }
                }
            });
            reserveAdminHubConnection.invoke('reserveRefunded', $id);
        }
        else if (ret.status == 0) {
            $.confirm({
                title: 'خطا',
                content: ret.msg,
                type: 'red',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بستن: function () {
                        //nothing
                    }
                }
            });
        }
    });
},
نه: function () {
}
                }
            });
        }

function payReserveWithCreditHost($id, obj) {
    $.confirm({
        title: '',
        content: 'آیا درصد املاک باشی از کیف پول میزبان کسر شود؟',
        type: 'blue',
        smoothContent: true,
        rtl: true,
        typeAnimated: true,
        columnClass: 'small',
        animation: 'scale',
        closeIcon: true,
        buttons: {
            بله: function () {
                var type = 1//ReservePayment.ReservePaymentType.GuestClearing;
    myajax("Reserve/PayReserveWithCreditHost", "reserve_id=" + $id + "&pay_reserve_type=" + type, function (ret) {
        if (ret.status == 1) {
            $.confirm({
                title: '',
                content: ret.msg,
                type: 'green',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بستن: function () {
                        //nothing
                    }
                }
            });
            reserveAdminHubConnection.invoke('changeStatus', $id, ret.new_reserve_status);
            reserveAdminHubConnection.invoke('payReserveWithCreditHost', $id);
        }
        else if (ret.status == 0) {
            $.confirm({
                title: 'خطا',
                content: ret.msg,
                type: 'red',
                smoothContent: true,
                typeAnimated: true,
                rtl: true,
                columnClass: 'small',
                animation: 'scale',
                closeIcon: true,
                buttons: {
                    بستن: function () {
                        //nothing
                    }
                }
            });
        }
    });
},
نه: function () {
}
                }
            });
        }

$(".js-filter-date-picker").persianDatepicker({
    altField: '#to_date-alt',
    format: 'YYYY/MM/DD',
    altFormat: 'YYYY/MM/DD',
    autoClose: true,
    toolbox: {
        calendarSwitch: { enabled: false },
        todayButton: { enabled: true },
        submitButton: { enabled: true, text: { fa: "بستن", en: close } }
    },
    navigator: {
        scroll: { enabled: false },
        text: { btnNextText: '<', btnPrevText: '>' },
    },
    initialValue: false
});

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function showAddInfoDialog(reserve_id) {
    showInfoMessage('افزودن توضیحات به رزرو با کد رزرو ' + reserve_id,
        '<input id="addInfoInput" type="text" />' + '<br />' + '<button onclick="addInfo‌ByElem(' +
        reserve_id + ',this.previousSibling.previousSibling)" style="padding: 5px;margin:5px; width:100px; height: 30px;"><i class="fa fa-plus"></i> افزودن</button>',
        {
            onOpen: function () {
                setTimeout(function () { $('#addInfoInput').focus(); }, 100);
                $('#addInfoInput').keypress(function (e) {
                    if (e.keyCode == 13) {
                        $('#addInfoInput').off('keypress');
                        this.nextSibling.nextSibling.click();
                    }
                });
            },
            autoClose: false
        });
}

function addInfo‌ByElem(reserve_id, input_elem){
    debugger;
    var text = $(input_elem).val();
    if (text == null || text == "") {
        showErrorMessage("خطا", "لطفا متن توضیح را وارد کنید");
        return;
    }
    addInfo(reserve_id, text);
}

function addInfo(reserve_id, text, notShowMessage) {
    if (notShowMessage === true) {
        executeAddInfo(reserve_id, text, true);
    }
    else {
        showNoYesMessage('افزودن توضیحات',
            'متن توضیحات: "' + text + '" اضافه شود؟',
            function () { executeAddInfo(reserve_id, text, false); })
    }
}

var shouldFollowAlert;
var sholdFollowElement;

function toggleShouldFollow(reserve_id, elem) {
    myajax("reserve/getshouldfollowstate", "reserve_id=" + reserve_id, function (ret) {
            if (ret.status == 0) {
                showErrorMessage("خطا", ret.msg);
            }
            else if (ret.status == 1) {
                if (ret.shouldFollow) {
                    showNoYesMessage('حذف در حال پیگیری. ', 'از در حال پیگیری حذف شود؟', function () {
                            myajax("reserve/toggleshouldfollow", "reserve_id=" + reserve_id, function (ret) {
                                    if (ret.status == 0) {
                                        showErrorMessage("خطا", ret.msg);
                                    }
                                    else if (ret.status == 1) {
                                        //reserveAdminHubConnection.invoke('toggleShouldFollow', reserve_id, ret.new_status);
                                        $(elem).css("color", "");
                                    }
                                });
                        })
                }
                else {
                    addToShouldFollow(reserve_id);
                    sholdFollowElement = elem;
                }
            }
        });
}

function addToShouldFollowByElem(reserve_id, input_elem) {
    var text = $(input_elem).val();
    addToShouldFollow(reserve_id, text);
}

function addToShouldFollow(reserve_id, text) {
    if (text != undefined) {
        showNoYesMessage('افزودن به در حال پیگیری', 'متن توضیحات: "' + text + '" اضافه شود؟',
            function () { executeAddToShouldFollow(reserve_id, text); })
    }
    else {
        shouldFollowAlert = showInfoMessage('افزودن به رزرو های در حال پیگیری. کد رزرو: ' + reserve_id,
            '<input id="followReasonInput" type="text" />' + '<br />' + '<button onclick="addToShouldFollowByElem(' +
            reserve_id + ',this.previousSibling.previousSibling)" style="padding: 5px;margin:5px; width:100px; height: 30px;"><i class="fa fa-plus"></i> افزودن</button>',
            {
                onOpen: function () {
                    setTimeout(function () { $('#followReasonInput').focus(); }, 100);
                    $('#followReasonInput').keypress(function (e) {
                        if (e.keyCode == 13) {
                            $('#followReasonInput').off('keypress');
                            this.nextSibling.nextSibling.click();
                        }
                    });
                },
                autoClose: false
            });
    }
}

function executeAddToShouldFollow(reserve_id, text) {
    myajax("reserve/toggleshouldfollow",
        "reserve_id=" + reserve_id + "&text=" + text, function (ret) {
            if (ret.status == 0) {
                showErrorMessage("خطا", ret.msg);
            }
            else if (ret.status == 1) {
                shouldFollowAlert.btnClick(0);
                //reserveAdminHubConnection.invoke('toggleShouldFollow', reserve_id, ret.new_status);
                $(sholdFollowElement).css("color", "limegreen");
                reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
            }
        });
}

function executeAddInfo(reserve_id, text, notShowMessage) {
    myajax("reserve/addsupporterinfotoreserve",
        "reserve_id=" + reserve_id + "&text=" + text, function (ret) {
            if (ret.status == 0) {
                showErrorMessage("خطا", "متاسفانه عملیات با خطا مواجه شد");
            }
            else if (ret.status == 1) {
                if (notShowMessage === true) {
                    reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
                    //window.location.href = window.location.href;
                }
                else {
                    showSuccessMessage("", "اطلاعات مورد نظر با موفقیت ثبت شد", {
                        onClose: function () {
                            reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
                            //window.location.href = window.location.href;
                        }
                    });
                }
            }
        });
}

function showComment(encodedComment) {
    debugger;
    var decodedComment = decodeURI(encodedComment);
    showInfoMessage('', decodedComment);
}

function showInfo(reserve_id) {
    $.confirm({
        title: false,
        content: 'url:/reserve/getreservesupporterinfo?reserve_id=' + reserve_id,
        buttons: {
            "بستن": function () {
            }
        }
    });
}
function showCancelDiscussion(reserve_id) {
    $.confirm({
        title: false,
        content: 'url:/canceldiscussion/getreservecanceldiscussion?reserve_id=' + reserve_id,
        buttons: {
            "بستن": function () {
            }
        }
    });
}

function nextCallState(reserve_id, hostOrGuest, hostOrGuestStr, elem) {
    var currentState = $(elem).attr('js-call-state');
    if (currentState > 1) {
        showInfoMessage('', 'تماس با ' + hostOrGuestStr + ' قبلا انجام شده');
        return;
    }
    showNoYesMessage('ثبت تماس با ' + hostOrGuestStr,
        currentState == 0 ? 'آیا تماس با ' + hostOrGuestStr + ' ثبت شود؟' :
            'آیا ' + hostOrGuestStr + ' پاسخ داد؟',
        function () {
            myajax("reserve/nextcallstate", "reserve_id=" + reserve_id + "&hostOrGuest=" + hostOrGuest, function (ret) {
                if (ret.status == 1) {
                    if (ret.new_state.toString() == '@((int)Reserve.CallState.Called)') {
                        executeAddInfo(reserve_id, "توسط پشتیبان با " + hostOrGuestStr + " تماس گرفته شد", false);
                    }
                    else if (ret.new_state.toString() == '@((int)Reserve.CallState.Answered)') {
                        executeAddInfo(reserve_id, "نتیجه تماس با " + hostOrGuestStr + ": پاسخ داد", false);
                    }
                    reserveAdminHubConnection.invoke('changeCallState',
                        reserve_id, hostOrGuest, ret.new_state, ret.new_state_color);
                }
            });
        })
}

function showCallPopup(user_id, reserve_id, guestOrHostStr) {
    myajax('user/getallphonenumbers', 'user_id=' + user_id, function (ret) {
        if (ret.status == 1) {
            var elements_str = ('<div>کد رزرو: ' + reserve_id + '</div>');
            elements_str += ('<div ' + (ret.full_name == 'ثبت نشده' ? 'style="color:red;"' : '') + '>نام ' + guestOrHostStr + ' : ' + ret.full_name + '</div>');
            elements_str += '<div style="display:flex;justify-content:center;">';
            elements_str += '<div style="display:flex;flex-flow:column;align-items:flex-end;font:25px Liransans;">';
            if (ret.main_mobile != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل اصلی: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.main_mobile.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.mobile_1 != null && ret.main_mobile != ret.mobile_1) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل ۱: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.mobile_1.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.mobile_2 != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل ۲: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.mobile_2.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.land_line != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>تلفن ثابت:‌ </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.land_line.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.third_person != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>شخص ثالث:‌ </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.third_person.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            elements_str += '</div></div>';
            showInfoMessage('', elements_str);
        }
        else {
            showErrorMessage('خطا', ret.msg);
        }
    });
}

function showInfoPopup(title, msg) {
    showInfoMessage(title, msg);
}

function addGuestPayment(id) {
    addGuestPaymentMsg = showInfoMessage(
        'افزودن پرداخت مهمان', '', { contentUrl: '/reserve/addguestpaymentpopup?id=' + id });
}

function onAddGuestPaymentDone() {
    alertify.success('پرداخت با موفقیت اضافه شد');
    addGuestPaymentMsg.close();
}

function editRating(reserve_id) {
    ratingDialog(reserve_id, function () {
        //window.location.href = window.location.href;
    });
}

function toggleDisableAutoCancel(id, elem) {
    var active = $(elem).attr('data-value');
    active = active === 'true';
    active = !active;
    showNoYesMessage(active ? 'غیرفعال سازی لغو خودکار سیستم' : 'فعال سازی مجدد لغو سیستم',
        'آیا می خواهید ' + (active ? 'لغو خوکار سیستم برای این رزرو غیرفعال شود؟' : 'دوباره لغو خودکار سیستم را برای این رزرو فعال کنید؟'),
        function () {
            myajax('reserve/toggledisableautocancel', 'id=' + id + '&active=' + active, function (ret) {
                    if (ret.status == 1) {
                        $(elem).attr('data-value', active);
                        $(elem).css('color', (active ? 'limegreen' : ''));
                        alertify.success(active ? 'لغو خودکار سیستم برای این رزرو غیرفعال شد. این رزرو فقط به صورت دستی قابل لغو خواهد بود' : 'لغو خودکار سیستم برای این رزرو دوباره فعال شد');
                    }
                    else {
                        alertify.error(ret.msg);
                    }
                });
        });
}

function toggleAccVisited(id, elem) {
    var active = $(elem).attr('data-value');
    active = active === 'true';
    active = !active;
    showNoYesMessage(active ? 'ثبت بازدید اقامتگاه' : 'حذف بازدید اقامتگاه',
        active ? 'آیا این اقامتگاه توسط مهمان بازدید شده است؟' : 'آیا بازدید اقامتگاه حذف شود؟',
        function () {
            myajax('reserve/toggleaccvisited',
                'id=' + id + '&active=' + active, function (ret) {
                    if (ret.status == 1) {
                        $(elem).attr('data-value', active);
                        $(elem).css('color', (active ? 'limegreen' : ''));
                        alertify.success(active ? 'بازدید اقامتگاه برای این رزرو ثبت شد' : 'بازدید اقامتگاه از این رزرو حذف شد');
                    }
                    else {
                        alertify.error(ret.msg);
                    }
                });
        });
}

function showMoreOptionBox(callerButton, reserveId) {
    loadHtmlContent(callerButton, $(callerButton).next()[0], '/reserve/GetReserveAdminDetails?reserveId=' + reserveId);
}

function showReserveEditForm(reserveId) {
    loadPopup("/reserve/popupedit?reserveId=" + reserveId);
}

function SubmitReserveEditForm() {
    submitForm('reserve_edit_form', "/reserve/popupedit", null, function () {
        $(".box-button-container i").css("display", "inline-block");
    }, function () {
        $(".box-button-container i").hide();
    });
}