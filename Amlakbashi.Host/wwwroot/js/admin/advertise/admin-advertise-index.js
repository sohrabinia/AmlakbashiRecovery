function showMoreOptionBox(callerButton, advertiseId) {
    loadCollapse(callerButton, $(callerButton).next()[0], '/advertise/GetAdvertiseIndexDetails?advertiseId=' + advertiseId);
}

function quickFilterProcess() {
    $('#more_filter_form [name="Id"]').val($('.box-filter .bar-filter [name="AdvertiseId"').val());
    $('#more_filter_form [name="UserId"]').val($('.box-filter .bar-filter [name="HostUserId"').val());
    $('#more_filter_form [name="Status"]').val($('.box-filter .bar-filter [name="Status"').val());
    $('#more_filter_form').submit();
}

var currentMonth;
var fromDate;
var toDate;
function calculateDateRange() {
    if (!firstSelectedDay) {
        return;
    }
    fromDate = firstSelectedDay.date.replaceAll('/', ',');
    if (secondSelectedDay != undefined) {
        currentMonth = gregorianToJalaliDate(new Date(secondSelectedDay.value));
        var nextDay = new Date(secondSelectedDay.value);
        nextDay.setDate(nextDay.getDate() + 1);
        nextDay = gregorianToJalaliDate(nextDay);
        toDate = nextDay.dateString.replaceAll('/', ',');
    }
    else {
        currentMonth = gregorianToJalaliDate(new Date(firstSelectedDay.value));
        var nextDay = new Date(firstSelectedDay.value);
        nextDay.setDate(nextDay.getDate() + 1);
        nextDay = gregorianToJalaliDate(nextDay);
        toDate = nextDay.dateString.replaceAll('/', ',');
    }
}

function deleteAdvertise($id) {
    showConfirm('آیا از حذف این آگهی مطمئن هستید؟', function () {
        sendGetAjax("/Advertise/Delete", "id=" + $id, function (ret) {
            if (ret.status == 1) {
                $('#js-' + $id).remove();
                successAlert();
            }
            else {
                errorAlert(ret.msg);
            }
        });
    });
}

function confirmHygieneProtocol($id, obj) {
    showConfirm("آیا رعایت پروتکل بهداشتی در این اقامتگاه مورد تایید است؟", function () {
        sendGetAjax("/accomodation/sethygieneprotocoladmin", "id=" + $id + "&value=2", function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert();
            }
        });
    });
}

function rejectHygieneProtocol($id, obj) {
    showConfirm("آیا از عدم تایید پروتکل های بهداشتی این اقامتگاه مطمئن هستید؟", function () {
        sendGetAjax("/accomodation/sethygieneprotocoladmin", "id=" + $id + "&value=3", function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert();
            }
        });
    });
}

function PublishAdvertise($id, obj) {
    showConfirm("آیا از انتشار این آگهی اطمینان دارید؟", function () {
        sendGetAjax("/accomodation/publish", "id=" + $id, function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert();
            }
        });
    });
}

function showOccupiedCalendarPopup(id) {
    loadPopup('/accomodation/getsetoccupiedpopup?id=' + id);
}

function showInstantReservePopup(id) {
    loadPopup('/accomodation/GetInstantReserveDates?residenceId=' + id);
}

function ToggleActivity(residenceId, elem) {
    var active = $(elem).attr('data-value');
    var activityStr = active === "true" ? 'غیرفعال' : 'فعال';
    showConfirm('آیا آگهی ' + activityStr + ' شود؟', function () {
        sendGetAjax("/advertise/UpdateActivity", "residenceId=" + residenceId, function (ret) {
            if (ret.status == 1) {
                if (ret.active === true) {
                    $(elem).attr('data-value', 'true');
                    $(elem).css("color", "limegreen");
                }
                else {
                    $(elem).attr('data-value', 'false');
                    $(elem).css("color", "red");
                }
                successAlert("وضعیت آگهی با موفقیت تغییر یافت");
            }
            else {
                errorAlert("عملیات با خطا مواجه شد");
            }
        })
    });
}

function ToggleAvailable(residenceId, elem) {
    var active = $(elem).attr('data-value');
    active = active === 'true' ? true : false;
    var activityStr = active === true ? 'غیرفعال' : 'فعال';
    active = !active;
    showConfirm('آیا آگهی ' + activityStr + ' شود؟', function () {
        sendPostAjax("/accomodation/available", { id: residenceId, isAvailable: active }, function (ret) {
            if (ret.status == 1) {
                if (active === true) {
                    $(elem).attr('data-value', 'true');
                    $(elem).css("color", "limegreen");
                }
                else {
                    $(elem).attr('data-value', 'false');
                    $(elem).css("color", "red");
                }
                successAlert("وضعیت آگهی با موفقیت تغییر یافت");
            }
            else {
                errorAlert("عملیات با خطا مواجه شد");
            }
        })
    });
}

function NotVerifyAdvertise($id, obj) {
    showConfirm('آیا از عدم تایید این آگهی اطمینان دارید؟', function () {
        sendGetAjax("/Advertise/NotVerify", "id=" + $id, function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert("وضعیت آگهی با موفقیت تغییر یافت");
            }
        });
    });
}

function showAddInfoDialog(advertise_id) {
    showPopup('افزودن توضیحات به آگهی ' + advertise_id
        + '<br/><input id="addInfoInput" type="text" style="width: 300px;" />' + '<br />' + '<button onclick="addInfo(' + advertise_id
        + ')" style="padding: 5px;margin:5px; width:100px; height: 30px;"><i class="fa fa-plus"></i> افزودن</button>');
}

function addInfo(advertise_id) {
    var text = $('#addInfoInput').val();
    if (!text) {
        errorAlert('لطفا توضیح مورد نظر را وارد کنید');
        return;
    }
    sendGetAjax("/advertise/addsupporterinfotoadvertise",
        "advertise_id=" + advertise_id + "&text=" + text, function (ret) {
            hidePopup();
            if (ret.status == 0) {
                errorAlert("عملیات با خطا مواجه شد");
            }
            else if (ret.status == 1) {
                successAlert('اطلاعات مورد نظر با موفقیت ثبت شد');
            }
        });
}

function showInfo(advertise_id) {
    loadPopup('/advertise/getadvertisesupporterinfo?advertise_id=' + advertise_id);
}

$('#js-status-filter-select').change(function () {
    if ($(this).val() == "0") {
        $('#js-sort-filter-select').val('modify');
    }
});

// ********* Price Popup ***********

function showPricePopup(id) {
    loadPopup('/accomodation/GetPricesInfo?residenceId=' + id);
}

function updateCalendarPrices(pricesList) {
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    updateDatePicker([$('.price-date-picker')[0]], jalaliCurrentMonth,
        {
            priceDict: pricesList,
            occupiedList: [],
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: false
        });
}

function onChangeInputPrice(inputElem) {
    var price = $(inputElem).val();
    if (isNaN(price)) {
        price = 0;
    }
    var persianStr = price < 1 ? '' : getPriceString(price);
    $(inputElem).next().html(persianStr);
}

function updateMainPrices() {
    submitPopup('/accomodation/updatemainprices');
}

function updateManualPrices(residenceId) {
    calculateDateRange();
    if (!fromDate || !toDate) {
        errorAlert('لطفا بازه زمانی مورد نظر را انتخاب کنید');
        return;
    }
    var priceInput = $('.calendar-prices-container input');
    var price = priceInput.val();
    if (price < 30000) {
        errorAlert('قیمت وارد شده اشتباه است');
        return;
    }
    sendPostAjax("/accomodation/updatemanualprices", { residenceId, fromDate, toDate, price }, function (ret) {
        if (ret.status == 1) {
            priceInput.val('');
            updateCalendarPrices(ret.priceDict);
        }
        else {
            errorAlert(ret.msg);
        }
    });
}

// *********** Discount Popup **************

function showDiscountPopup(id) {
    loadPopup('/accomodation/GetDiscountInfo?residenceId=' + id);
}

function addDiscount(residenceId) {
    calculateDateRange();
    if (!fromDate || !toDate) {
        errorAlert('لطفا بازه زمانی مورد نظر را انتخاب کنید');
        return;
    }
    var discountInput = $('.calendar-prices-container input');
    var discount = discountInput.val();
    if (discount < 1 || discount > 100) {
        errorAlert('مقدار تخفیف وارد شده اشتباه است');
        return;
    }
    sendPostAjax("/accomodation/adddiscount", { residenceId, fromDate, toDate, discount }, null, null, hidePopup);
}

function deleteDiscount(discountId) {
    sendPostAjax("/accomodation/deletediscount", { discountId }, null, null, hidePopup);
}

// *********** Video Popup **************

function showVideoPopup(id) {
    loadPopup('/accomodation/GetAdminVideoInfo?residenceId=' + id);
}

function confirmVideo(residenceId) {
    stopVideoConnection();
    showConfirm("آیا از تایید این ویدیو مطمئنید؟", function () {
        showDarkBackground();
        setTimeout(function () {
            sendPostAjax("/accomodation/SetVideoStatus", { residenceId, status: 2 }, null, null, hidePopup);
        }, 1000);
    });
}

function notConfirmVideo(residenceId) {
    stopVideoConnection();
    showConfirm("دلیل عدم تایید ویدیو: <br/><textarea id='not_confirm_reason' style='width: 95%;'></textarea><br/><br/>" + 
        "آیا از عدم تایید این ویدیو مطمئنید؟", function () {
        let notConfirmReason = $('#not_confirm_reason').val();
        if (!notConfirmReason) {
            errorAlert("لطفا دلیل عدم تایید را وارد کنید");
            return;
        }
        showDarkBackground();
        setTimeout(function () {
            sendPostAjax("/accomodation/SetVideoStatus", { residenceId, status: 3, notConfirmReason }, null, null, hidePopup);
        }, 1000);
    });
}

function stopVideoConnection() {
    var video = document.getElementById('video_play_container');
    if (video) {
        video.srcObject = null;
    }
}