//activeOrDeactivebBanner
var idBanner;
function setAccActive(id, active, elem) {
    var btn = $(elem).children().first();
    if (!btn.is(":checked")) {
        btn.prop('checked', true);
        myajax("accomodation/toggleactive", "id=" + id + "&active=" + active, function (ret) {
            $("#banner-status-" + id).find(".js-status-color").css('background-color', ret.statusColor);
            $("#banner-status-" + id).find(".js-status-string").html(ret.statusString);
            if (ret.status == 0) {
                showErrorMessage('خطا', ret.msg);
                if (btn.hasClass("show-banner")) {
                    btn.attr('checked', false);
                    $(elem).next().children().first().prop('checked', true);
                } else if (btn.hasClass("hide-banner")) {
                    btn.attr('checked', false);
                    $(elem).prev().children().first().prop('checked', true);
                }

            }
        });
    }
}
//setTodayEmpty
function setTodayEmpty(id, elem) {
    var inputSetToday = $(elem).children().first();
    if (!inputSetToday.is(":checked")) {
        inputSetToday.prop('checked', true);
        myajax("accomodation/setastodayempty", "id=" + id, function (ret) {
            if (ret.status == 0) {
                inputSetToday.attr('checked', false);
                $(elem).next().children().first().prop('checked', true);
                showErrorMessage('خطا', ret.msg);
            } else {
                updateOccupieddates(id);
            }
        });
    }
}
//setTodayeFull
function setTodayFull(id, elem) {
    var inputSetTodayFull = $(elem).children().first();
    if (!inputSetTodayFull.is(":checked")) {
        inputSetTodayFull.prop('checked', true);
        myajax("accomodation/unsettodayempty", "id=" + id, function (ret) {
            if (ret.status == 0) {
                inputSetTodayFull.prop('checked', false);
                $(elem).prev().children().first().prop('checked', true);
                showErrorMessage('خطا', ret.msg);
            } else {
                updateOccupieddates(id);
            }
        });
    }
}
//fast reserve
function instantReserve(id, userId, banned) {
    var $elem = $('#js-instant-reserve-' + id);
    if (banned) {
        //showInfoMessage('دلیل ممنوعیت', '', { contentUrl: '/accomodation/getInstnatReserveBanReason?id=' + id });
        return;
    } else {
        instantReserveRequest(id, userId, false, $elem);
    }
}
//active fastReserve
function instantReserveRequest(id, userId, ignorMsg, $elem) {
    myajax("accomodation/instantreserverequest", "id=" + id + "&userid=" + userId + '&ignoremsg=' + ignorMsg, function (ret) {
        if (ret.status == 0) {
            showErrorMessage('خطا', ret.msg);
        }
        if (ret.status == 1) {
            if (ret.needMsg) {
                showNoYesMessage('امکان رزرو آنی',
                    '<div>' +
                    '<strong>میزبان گرامی</strong>' +
                    '<br />' +
                    'با فعال سازی امکان رزرو آنی، روز هایی که تقویم اقامتگاه شما در سایت املاک باشی خالی باشند ' +
                    '<strong style="color:#EA4335">بدون نیاز به استعلام از شما</strong>' +
                    ' توسط مهمان در هر ساعت از شبانه روز رزرو خواهد شد. ' +
                    'توجه داشته باشید بعد از فعالسازی این امکان تقویم خود را در سایت املاک باشی به روز نگه دارید که مشمول جریمه نشوید.' +
                    '<br />' +
                    '<strong style="color:#FF7F00">' +
                    'جریمه اولین لغو: 10% مبلغ کل رزرو' +
                    '</strong>' +
                    '<br />' +
                    '<strong style="color:#FF7F00">' +
                    'جریمه دومین لغو: 15% مبلغ کل رزرو' +
                    '</strong>' +
                    '<br />' +
                    '<strong style="color:#FF7F00">' +
                    'جریمه سومین لغو: این امکان برای شما غیر فعال میشود' +
                    '</strong>' +
                    '<br />' +
                    'آیا شرایط را قبول دارید؟' +
                    '</div>'
                    ,
                    function () {
                        instantReserveRequest(id, userId, true);

                    },
                            undefined,
                            {
                                yesText: 'قبول دارم',
                                noText: 'لغو'
                            }
                        );
            }
            else {
                showSuccessMessage('', ret.msg,
                    {
                        onClose: function () {
                            updateInstantReserveData(id, userId, ret.newData);
                        }
                    });
            }
        }
        else {
            showErrorMessage('خطا', ret.msg);
        }
    });
}
//change style fastReserve
//---------------------------------------------
function updateInstantReserveData(id, userId, newData) {
    var $elem = $('#js-instant-reserve-' + id);
    $elem.css('background-color', newData.statusColor);
    var onClickAction = `instantReserveAction(${id} , ${userId}, ${newData.status} , ${newData.banned})`;
    $elem.attr('onclick', onClickAction);
    $elem.html(newData.statusString);
    $elem.find('.js-instant-reserve-button').html(newData.buttonTitle);
}

function instantReserveButton(id, userId, elem) {
    if ($(elem).is(":checked")) {
        instantReserveActive(id, userId, elem);
    } else {
        instantReserveDeactive(id, userId, elem);
    }
}
//Active fastReserve
function instantReserveActive(id, userId, elem) {
    myajax("accomodation/instantreserverequest", "id=" + id + "&userid=" + userId + '&ignoremsg=' + true, function (ret) {
        if (ret.status == 0) {
            $(elem).prop("checked", false)
            showErrorMessage('خطا', ret.msg);
        }
        else if (ret.status == 1) {
            $('#maxInstantReserve-' + id).css('display', 'flex');
        }
    });
}
//Deactive fastReserve
function instantReserveDeactive(id, userId, elem) {
    myajax('accomodation/instantreservecancel', 'id=' + id + "&userid=" + userId, function (ret) {
        if (ret.status == 0) {
            $(elem).prop("checked", true)
            showErrorMessage('خطا', ret.msg);
        }
        else if (ret.status == 1) {
            $('#maxInstantReserve-' + id).css('display', 'none');
        }
    });
}

//jalaliDate on set calender
//---------------------------------------------
function setFullRooms(id, elem) {
    if ($(elem).hasClass('deactive-set-button')) {
        return;
    }
    calculateRange();
    jalaliShowLoading();
    myajax("accomodation/checksetasoccupiedfordaterange", "advertise_id=" + id + "&from_date=" + from_date + "&to_date=" + to_date + "&forremove" + false, function (ret) {
        jalaliHideLoading();
        if (ret.status == 0) {
            showErrorMessage('خطا', ret.msg);
            return;
        }
        jalaliShowLoading();
        myajax("accomodation/setasoccupiedfordaterange", "advertise_id=" + id + "&from_date=" + from_date + "&to_date=" + to_date, function (ret) {
            jalaliHideLoading();
            if (ret.status == 0) {
                showErrorMessage('خطا', ret.msg);
                return;
            }
            alertify.success(' محدوده انتخاب شده به روز های پر اضافه شد ');
            firstSelectedDay = undefined;
            secondSelectedDay = undefined;
            updateButton();
            updateDatePicker([$('#calendars-edit-fast-' + id)[0]], jalaliCurrentMonth,
            {
                priceDict: {},
                occupiedList: ret.occupiedList,
                extrinsicList: ret.extrinsicList,
                monthOffset: 0,
                selectionType: 'multi',
                occupiedSelectEnabled: true,
                onUpdateDate: onUpdateDateCalendar
            });
        }, false);
    }, false);
}
function setEmptyRooms(id, elem) {
    if ($(elem).hasClass('deactive-set-button')) {
        return;
    }
    calculateRange();
    jalaliShowLoading();
    myajax("accomodation/checksetasoccupiedfordaterange", "advertise_id=" + id + "&from_date=" + from_date + "&to_date=" + to_date + "&forremove" + true, function (ret) {
        jalaliHideLoading();
        if (ret.status == 0) {
            showErrorMessage('خطا', ret.msg);
            return;
        }
                jalaliShowLoading();
                myajax("accomodation/removefromoccupiedfordaterange", "advertise_id=" + id + "&from_date=" + from_date + "&to_date=" + to_date, function (ret) {
                    jalaliHideLoading();;
                    if (ret.status == 0) {
                        showErrorMessage('خطا', ret.msg);
                        return;
                    }
                    alertify.success('محدوده انتخاب شده از روز های پر حذف شد');
                    firstSelectedDay = undefined;
                    secondSelectedDay = undefined;
                    updateButton();
                    updateDatePicker([$('#calendars-edit-fast-' + id)[0]], jalaliCurrentMonth,
                    {
                        priceDict: {},
                        occupiedList: ret.occupiedList,
                        extrinsicList: ret.extrinsicList,
                        monthOffset: 0,
                        selectionType: 'multi',
                        occupiedSelectEnabled: true,
                        onUpdateDate: onUpdateDateCalendar
                    });
                }, false);
    }, false);
};
//set-date-picker-price
//---------------------------------------------
var inputPrice = $(".inputPrice");
function setPrice(id, elem) {
    if ($(elem).hasClass('deactive-set-button')) {
        return;
    }
    var price = $(elem).parent().siblings(".input-group-price").find(".js-price-input").val();
    price = persianToEnglishNumber(price);
    if (isNaN(price)) {
        price = 0;
    }
    if (price < 30000) {
        showErrorMessage('خطا', 'حداقل قیمت: 30000 تومان');
        return;
    }
    calculateRange();
        jalaliShowLoading();
        myajax("accomodation/setpricefordaterange", "advertise_id=" + id + "&from_date=" + from_date + "&to_date=" + to_date + "&price=" + price, function (ret) {
            jalaliHideLoading();
            if (ret.status == 0) {
                showErrorMessage('خطا', ret.msg);
                return;
            }
            alertify.success('  قیمت برای محدوده انتخاب شده ثبت شد ');
            $(".js-price-input").val('');
            onChangeInputPrice();
            firstSelectedDay = undefined;
            secondSelectedDay = undefined;
            onInputPrice(getCurrentSetPriceContainer().find(".js-price-input"));
            updateNewPrice(id, ret.priceDict);
        }, false);
}
//set-date-picker-offer
//---------------------------------------------
function setOffer(id, elem) {
    if ($(elem).hasClass('deactive-set-button')) {
        return;
    }
    var discount_percent = $('#js-percent-input-' + id).val();
    if (isNaN(discount_percent)) {
        discount_percent = 0;
    }
    if (discount_percent < 4) {
        showErrorMessage('خطا', 'مقدار تخفیف باید بیشتر از 3% باشد');
        return;
    }
    calculateRange();
    myajax('accomodation/adddiscount', 'id=' + id + '&from=' + from_date + '&to=' + to_date + '&discount=' + discount_percent,
        function (ret) {
            if (ret.status == 0) {
                showErrorMessage('خطا', ret.msg);
            }
            else {
                alertify.success('  تخفیف با موفقیت ثبت شد  ');
                showDiscounts(id, elem);
                $('#js-percent-input-' + id).val(0);
                firstSelectedDay = undefined;
                secondSelectedDay = undefined;
                updateNewPrice(id, ret.priceDict);
            }
        });
}
function showDiscounts(id) {
    myajax("accomodation/getdiscounts", "id=" + id, function (ret) {
        $("#list-offer-" + id).html("");
        var items_string = "";
        ret.discounts.forEach(function (item) {
            items_string += ("<li data-discount-id='" + item.id + "' "
                + " class='item-list-offer'"
                + ">"
                + "<span>" + item.percent + " درصد"
                + "از" + item.dateString
                + " </span>"
                + `<span onclick='deletePerecent(${item.id} , this)'  class='trash wrapper-left-banner div-unset'> `
                + "<i class='fa fa-trash'></i>"
                + "</span>"
                + "</li>");
        });
        $("#list-offer-" + id).append('<li class="title-list-offer">لیست تخفیف ها</li>');
        $("#list-offer-" + id).append(items_string);
    });
};
//Delete-date-picker-offer
//---------------------------------------------
function deletePerecent(id, elem) {
   var parentID =$(elem).parent().parent().parent().attr('data-id');
    showNoYesMessage('  حذف تخفیف', 'آیا از حذف این تخفیف مطمئن هستید؟', function () {
        myajax("accomodation/removediscount", "discount_id=" + id, function (ret) {
            if (ret.status == 0) {
                showErrorMessage('خطا', ret.msg);
            } else{
                alertify.success('  تخفیف با موفقیت حذف شد  ');
                $(elem).parent().remove();
                updateNewPrice(parentID, ret.priceDict);
            }
        });
    });
}
function updateNewPrice(id, priceDict) {
    updateDatePicker([$('#acc-set-offer-' + id)[0]], jalaliCurrentMonth,
        {
            priceDict: priceDict,
            occupiedList: [],
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: false,
            onUpdateDate: onUpdateDateOffer
        });
    updateDatePicker([$('#acc-set-price-' + id)[0]], jalaliCurrentMonth,
        {
            priceDict: priceDict,
            occupiedList: [],
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: false,
            onUpdateDate: onUpdateDatePrice
        });
}
//min&max day on reserve
//---------------------------------------------
var maxStr;
var minStr;
function setStayDuration(id) {
    minStr = persianToEnglishNumber($('#js-min-input-' + id).val());
    maxStr = persianToEnglishNumber($('#js-max-input-' + id).val());
    myajax('accomodation/setstayduration', 'id=' + id + '&minstr=' + minStr + '&maxstr=' + maxStr, function (ret) {
        if (ret.status == 1) {
            var minDay = ret.data.min == 0 ? $('#js-min-input-' + id).val('') : ret.data.min;
            var maxDay = ret.data.max == 0 ? $('#js-max-input-' + id).val('') : ret.data.max;
            $('#js-min-input-' + id).html(minDay);
            $('#js-max-input-' + id).html(maxDay);
            showSuccessMessage('', ret.msg);
        }
        else {
            showErrorMessage('خطا', ret.msg);
        }
    });
}
$(".js-input")
  .keyup(function () {
      newval = $(this).val().replace(/[^0-9.]/g, "");
      $(this).val(newval);
      if ($(this).hasClass('set_min-max-day')) {
          if ($(this).val() == 0) {
              $(this).val('');
              //$(".placeholderDays").text("تعداد روز مد نظر را وارد کنید");
          }
      }
  })
  .keyup();


//setMaxInstantReserveStart
//--------------------------------------

function setMaxInstantReserve(id) {
    var temp_max_irstart = $("#setMaxInstantReserve-" + id).val();
    myajax('accomodation/setinstantreservestart', 'id=' + id + '&maxStart=' + temp_max_irstart, function (ret) {
        if (ret.status == 1) {
            var strMaxReserveDay = ret.data.max == 0 ? $('#setMaxInstantReserve-' + id).val('') : ret.data.max;
            $('#setMaxInstantReserve-' + id).html(strMaxReserveDay);
            alertify.success(' باموفقیت ثبت  شد');
        }
        else {
            showErrorMessage('خطا', ret.msg);
        }
    });
}

//delet-banner
function deleteButton(id, elem) {
    showNoYesMessage(' حذف آگهی', 'آیا از حذف این آگهی مطمئن هستید؟', function () {
        myajax("accomodation/delete", "id=" + id, function (ret) {
            if (ret.status == 0) {
                alertify.error(ret.msg);
            }
            if (ret.status == 1) {
                alertify.success('آگهی با موفقیت حذف شد');
                $(elem).parent().parent().parent().parent().parent().remove();
            }
        });
    });
}

function showOccupiedPopup(id, title) {
    showInfoMessage('  کد آگهی: ' + id, '', { contentUrl: '/accomodation/getsetoccupiedpopup?id=' + id, fullScreen: screen.width < 781 });
}

function getInstantReserveDates(id) {
    showInfoMessage('رزرو آنی آگهی ' + id, '', { contentUrl: '/accomodation/GetInstantReserveDates?residenceId=' + id });
}

function showNorouzPopup(id) {
    showInfoMessage('  کد آگهی: ' + id, '', { contentUrl: '/accomodation/getsetminnorouzreservepopup?id=' + id, fullScreen: screen.width < 781 });
}