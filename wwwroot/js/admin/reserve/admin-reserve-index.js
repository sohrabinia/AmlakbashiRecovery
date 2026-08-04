function showMoreOptionBox(callerButton, reserveId) {
    loadCollapse(callerButton, $(callerButton).next()[0], '/reserve/GetReserveAdminDetails?reserveId=' + reserveId);
}

function showReserveEditForm(reserveId) {
    loadPopup("/reserve/popupedit?reserveId=" + reserveId);
}

function SubmitReserveEditForm() {
    submitPopup("/reserve/popupedit");
}

function showSupportInfo(reserveId) {
    loadPopup("/reserve/getsupportinfo?reserveId=" + reserveId);
}

function quickFilterProcess() {
    $('#more_filter_form [name="ReserveId"]').val($('.box-filter .bar-filter [name="ReserveId"').val());
    $('#more_filter_form [name="AdvertiseId"]').val($('.box-filter .bar-filter [name="AdvertiseId"').val());
    $('#more_filter_form [name="HostUserId"]').val($('.box-filter .bar-filter [name="HostUserId"').val());
    $('#more_filter_form [name="GuestUserId"]').val($('.box-filter .bar-filter [name="GuestUserId"').val());
    $('#more_filter_form').submit();
}

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

function callForRequest(id) {
    showConfirm('آیا با میزبان، تماس پاسخ درخواست رزرو گرفته شود؟', function () {
        sendGetAjax('/Reserve/CallForRequest', "reserve_id=" + id, function (response) {
            if (response.status == 1) {
                addInfo(id, "توسط سیستم با میزبان تماس گرفته شد", true);
                successAlert("تماس با میزبان انجام شد");
            }
            else if (response.stat == 2) {
                errorAlert("میزبان هم اکنون به درخواست رزرو پاسخ داده است");
            }
            else {
                errorAlert("متاسفانه عملیات با خطای فنی مواجه شد");
            }
        });
    });
}

let changeSupporterReason = '';
let changeSupporterElem = undefined;
function doSupport($id, elem) {
    showConfirm("آیا قصد دارید این رزرو را پشتیبانی کنید؟", function () {
        sendGetAjax("/Reserve/DoSupport", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                $(elem).remove();
                reserveAdminHubConnection.invoke('reserveSupporterAdded', $id, ret.supporterName, ret.supporterPhoto);
                successAlert('عملیات با موفقیت انجام شد');
            }
            else if (ret.status == 2) {
                changeSupporterElem = elem;
                showPopup('<div>هشدار: ' + ret.msg + '</div>'
                    + '<div>لطفا دلیل انتقال پشتیبانی را بنویسید: <input type="text" onchange="changeSupporterReason = $(this).val();" /></div>'
                    + '<p>انتقال پشتیبانی انجام شود؟</p><div style="text-align:center;"><button onclick="moveSupport(' + $id
                    + ')">انتقال پشتیبانی</button>');
            }
            else {
                errorAlert(ret.msg);
            }
        })
    });
}

function moveSupport(id) {
    if (!changeSupporterReason) {
        errorAlert('لطفا دلیل انتقال پشتیبانی را وارد کنید');
        return;
    }
    sendGetAjax("/Reserve/DoSupport", "reserve_id=" + id + "&force=" + true + '&transfer_reason=' + changeSupporterReason, function (ret) {
        hidePopup();
        if (ret.status === 1) {
            $(changeSupporterElem).remove();
            reserveAdminHubConnection.invoke('reserveSupporterAdded', id, ret.supporterName, ret.supporterPhoto);
            successAlert('عملیات با موفقیت انجام شد');
        }
        else {
            errorAlert(ret.msg);
        }
    })
}

function callForPayment($id) {
    showConfirm("آیا با مهمان تماس پرداخت رزرو گرفته شود؟", function () {
        sendGetAjax("/Reserve/CallForPayment", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                addInfo($id, "توسط سیستم با مهمان تماس گرفته شد", true);
                successAlert("تماس با مهمان انجام شد");
            }
            else if (ret.stat == 2) {
                errorAlert('مهمان هم اکنون اقدام به پرداخت کرده است');
            }
            else {
                errorAlert("متاسفانه عملیات با خطای فنی مواجه شد");
            }
        });
    });
}

function cancelBySystem($id, obj) {
    showConfirm("آیا درخواست رزرو توسط سیستم لغو شود؟", function () {
        sendGetAjax("/Reserve/CancelBySystem", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                reserveAdminHubConnection.invoke('cancelReserve', $id);
                successAlert("درخواست رزرو لغو شد");
            }
            else if (ret.stat == 2) {
                errorAlert("این رزرو توسط مهمان/میزبان پاسخ داده شده است و امکان لغو سیستمی آن وجود ندارد");
            }
            else {
                errorAlert("عملیات با خطای فنی مواجه شد");
            }
        });
    });
}

function showSiteClearingHostPopup(id) {
    loadPopup('/Reserve/GetSiteClearingHostInfo?reserveId=' + id);
}

function showSiteRefundGuestPopup(id) {
    loadPopup('/Reserve/GetSiteRefundGuestInfo?reserveId=' + id);
}

function siteClearingHostWithCredit($id, obj) {
    showConfirm('آیا سهم میزبان از طریق کیف پول تسویه شود؟', function () {
        sendGetAjax("/Reserve/SiteClearingWithCredit", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                showConfirm(ret.msg + ". آیا پیامک ارسال شود؟", function () {
                    sendGetAjax("/Reserve/SendSiteClearingWithCreditSms", "reserve_id=" + $id + "&payable_price=" + ret.payable_price + "&transaction_id=" + ret.transaction_id, function () { });
                });
                reserveAdminHubConnection.invoke('reserveCleared', $id);
            }
            else if (ret.status == 0) {
                errorAlert('عملیات با خطای فنی مواجه شد');
            }
        });
    });
}

function siteRefundGuestWithCredit($id, obj) {
    showConfirm('آیا مبلغ رزرو از طریق کیف پول به مهمان عودت داده شود؟', function () {
        sendGetAjax("/Reserve/SiteRefundGuestWithCredit", "reserve_id=" + $id, function (ret) {
            if (ret.status == 1) {
                successAlert(ret.msg);
                reserveAdminHubConnection.invoke('reserveRefunded', $id);
            }
            else if (ret.status == 0) {
                errorAlert(ret.msg);
            }
        });
    });
}

function payReserveWithCreditHost($id, obj) {
    showConfirm('آیا درصد املاک باشی از کیف پول میزبان کسر شود؟', function () {
        sendGetAjax("/Reserve/PayReserveWithCreditHost", "reserve_id=" + $id + "&pay_reserve_type=" + 1, function (ret) {
            if (ret.status == 1) {
                successAlert(ret.msg);
                reserveAdminHubConnection.invoke('changeStatus', $id, ret.new_reserve_status);
                reserveAdminHubConnection.invoke('payReserveWithCreditHost', $id);
            }
            else if (ret.status == 0) {
                errorAlert(ret.msg);
            }
        });
    });
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function addInfo‌ByElem(reserve_id, input_elem){
    var text = $(input_elem).val();
    if (text == null || text == "") {
        errorAlert("لطفا متن توضیح را وارد کنید");
        return;
    }
    addInfo(reserve_id, text);
}

function addInfo(reserve_id, text, notShowMessage) {
    if (notShowMessage === true) {
        executeAddInfo(reserve_id, text, true);
    }
    else {
        showConfirm('متن توضیحات: "' + text + '" اضافه شود؟', function () {
            executeAddInfo(reserve_id, text, false);
            hidePopup();
        });
    }
}

var shouldFollowAlert;
var sholdFollowElement;

function toggleShouldFollow(reserve_id, elem) {
    sendGetAjax("/reserve/getshouldfollowstate", "reserve_id=" + reserve_id, function (ret) {
            if (ret.status == 0) {
                errorAlert(ret.msg);
            }
            else if (ret.status == 1) {
                if (ret.shouldFollow) {
                    showConfirm('از در حال پیگیری حذف شود؟', function () {
                        sendGetAjax("/reserve/toggleshouldfollow", "reserve_id=" + reserve_id, function (ret) {
                            if (ret.status == 0) {
                                errorAlert(ret.msg);
                            }
                            else if (ret.status == 1) {
                                reserveAdminHubConnection.invoke('toggleShouldFollow', reserve_id, ret.new_status);
                            }
                        });
                    });
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
        showConfirm('متن توضیحات: "' + text + '" اضافه شود؟', function () {
            executeAddToShouldFollow(reserve_id, text);
        });
    }
    else {
        showPopup('افزودن به رزرو های در حال پیگیری. کد رزرو: ' + reserve_id +
            '<br/><input id="followReasonInput" type="text" />' + '<br />' + '<button onclick="addToShouldFollowByElem(' +
            reserve_id + ',this.previousSibling.previousSibling)" style="padding: 5px;margin:5px; width:100px; height: 30px;"><i class="fa fa-plus"></i> افزودن</button>');
    }
}

function executeAddToShouldFollow(reserve_id, text) {
    sendGetAjax("/reserve/toggleshouldfollow", "reserve_id=" + reserve_id + "&text=" + text, function (ret) {
            if (ret.status == 0) {
                errorAlert(ret.msg);
            }
            else if (ret.status == 1) {
                hidePopup();
                reserveAdminHubConnection.invoke('toggleShouldFollow', reserve_id, ret.new_status);
                reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
                successAlert('عملیات انجام شد');
            }
        });
}

function executeAddInfo(reserve_id, text, notShowMessage) {
    sendPostAjax("/reserve/addsupporterinfotoreserve", "reserve_id=" + reserve_id + "&text=" + text, function (ret) {
            if (ret.status == 0) {
                errorAlert("عملیات با خطا مواجه شد");
            }
            else if (ret.status == 1) {
                if (notShowMessage === true) {
                    reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
                }
                else {
                    successAlert("اطلاعات مورد نظر با موفقیت ثبت شد");
                    reserveAdminHubConnection.invoke('addSupporterInfoToReserve', reserve_id, text);
                }
            }
        });
}

function showComment(encodedComment) {
    var decodedComment = decodeURI(encodedComment);
    //showInfoMessage('', decodedComment);
    showPopup(decodedComment);
}

function showCancelDiscussion(reserve_id) {
    loadPopup('/canceldiscussion/getreservecanceldiscussion?reserve_id=' + reserve_id);
}

function nextCallState(reserve_id, hostOrGuest, hostOrGuestStr, elem) {
    var currentState = $(elem).attr('js-call-state');
    if (currentState > 1) {
        errorAlert('تماس با ' + hostOrGuestStr + ' قبلا انجام شده');
        return;
    }
    let text = currentState == 0 ? 'آیا تماس با ' + hostOrGuestStr + ' ثبت شود؟' : 'آیا ' + hostOrGuestStr + ' پاسخ داد؟';
    showConfirm(text, function () {
        sendGetAjax("/reserve/nextcallstate", "reserve_id=" + reserve_id + "&hostOrGuest=" + hostOrGuest, function (ret) {
            if (ret.status == 1) {
                if (ret.new_state.toString() == '1') { // called
                    executeAddInfo(reserve_id, "توسط پشتیبان با " + hostOrGuestStr + " تماس گرفته شد", false);
                }
                else if (ret.new_state.toString() == '2') { // answered
                    executeAddInfo(reserve_id, "نتیجه تماس با " + hostOrGuestStr + ": پاسخ داد", false);
                }
                reserveAdminHubConnection.invoke('changeCallState', reserve_id, hostOrGuest, ret.new_state, ret.new_state_color);
                hidePopup();
            }
        });
    });
}

function showCallPopup(user_id, reserve_id, guestOrHostStr) {
    sendGetAjax('/user/getallphonenumbers', 'user_id=' + user_id, function (ret) {
        if (ret.status == 1) {
            var elements_str = ('<div>کد رزرو: ' + reserve_id + '</div>');
            elements_str += ('<div ' + (ret.full_name == 'ثبت نشده' ? 'style="color:red;"' : '') + '>نام ' + guestOrHostStr + ' : ' + ret.full_name + '</div>');
            elements_str += '<div style="display:flex;justify-content:center;">';
            elements_str += '<div style="display:flex;flex-flow:column;align-items:flex-end;font:25px Liransans;">';
            if (ret.main_mobile != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل اصلی: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.main_mobile.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.mobile_2 != null && ret.main_mobile != ret.mobile_1) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل 2: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.mobile_2.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.mobile_3 != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>موبایل 3: </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.mobile_3.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.land_line != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>تلفن ثابت:‌ </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.land_line.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            if (ret.third_person != null) {
                elements_str += ('<div style="display:flex;flex-flow:row;"><div>شخص ثالث:‌ </div><div style="direction:ltr;margin:5px;color:#2200ff;">' + ret.third_person.replace(/(\d{4})(\d{3})(\d{4})/, '$1 $2 $3') + '</div></div>');
            }
            elements_str += '</div></div>';
            showPopup(elements_str);
        }
        else {
            errorAlert(ret.msg);
        }
    });
}

function showInfoPopup(title, msg) {
    showPopup(msg);
}

function addGuestPayment(id) {
    loadPopup('/reserve/addguestpaymentpopup?id=' + id);
}

function editRating(reserve_id) {
    ratingDialog(reserve_id, function () {
    });
}

function toggleDisableAutoCancel(id, elem) {
    var active = $(elem).attr('data-value');
    active = active === 'true';
    active = !active;
    showConfirm('آیا می خواهید ' + (active ? 'لغو خوکار سیستم برای این رزرو غیرفعال شود؟' : 'دوباره لغو خودکار سیستم را برای این رزرو فعال کنید؟'), function () {
        sendGetAjax('/reserve/toggledisableautocancel', 'id=' + id + '&active=' + active, function (ret) {
            if (ret.status == 1) {
                $(elem).attr('data-value', active);
                $(elem).css('color', (active ? 'limegreen' : ''));
                successAlert(active ? 'لغو خودکار سیستم برای این رزرو غیرفعال شد. این رزرو فقط به صورت دستی قابل لغو خواهد بود' : 'لغو خودکار سیستم برای این رزرو دوباره فعال شد');
            }
            else {
                errorAlert(ret.msg);
            }
        });
    });
}

function toggleAccVisited(id, elem) {
    var active = $(elem).attr('data-value');
    active = active === 'true';
    active = !active;
    showConfirm(active ? 'آیا این اقامتگاه توسط مهمان بازدید شده است؟' : 'آیا بازدید اقامتگاه حذف شود؟', function () {
        sendGetAjax('/reserve/toggleaccvisited', 'id=' + id + '&active=' + active, function (ret) {
            if (ret.status == 1) {
                $(elem).attr('data-value', active);
                $(elem).css('color', (active ? 'limegreen' : ''));
                successAlert(active ? 'بازدید اقامتگاه برای این رزرو ثبت شد' : 'بازدید اقامتگاه از این رزرو حذف شد');
            }
            else {
                errorAlert(ret.msg);
            }
        });
    });
}