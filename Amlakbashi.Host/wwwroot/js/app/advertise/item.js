var user_is_autenticated = false;
var isNumberForIran = false;
var verifyEmail = false;
var userEmailAddress = "";
if (!(new Date().getHours() > 3)) {
    pastDayOffset = -1;
}
function showReservePopup(advertise_id) {
    firstSelectedDay = undefined;
    secondSelectedDay = undefined;
    $('#reserve_popup').slideUp(500, function () {
        $('#reserve_popup_container').empty();
        $('#reserve_popup_container').load(
            '../../../accomodation/getreservepopup?accomodationid=' + advertise_id,
            function () {
                updateReserveInfo(advertise_id, function () {
                    $('#reserve_popup').slideDown(500);
                    $('.advertise-page__reserve-container').show();
                });
                $(document).on("change", "#guest_count", function () {
                    onChangeGuestCount(this);
                });
            });
    });
}

function clickGuestIncDec(elem) {
    onClickIncDecButton(elem);
    onChangeGuestCount();
}

function onChangeGuestCount(elem) {
    var maxGuestCount = parseInt($(elem).attr('max'));
    if (parseInt($(elem).val()) > maxGuestCount) {
        alertify.error("حداکثر ظرفیت این اقامتگاه " + maxGuestCount + " نفر است");
        $(elem).val($(elem).attr('max'));
    }
    updateReservePrice();
    updateReserveLabels();
}

function moreComments() {
    if (shownCommentsCount < commentsCount) {
        var nextItemsCount = Math.min(commentsCount - shownCommentsCount, 3);
        var $lastShown = $('.js-comment-last-shown').first();
        $lastShown.removeClass('js-comment-last-shown');
        for (var i = 0; i < nextItemsCount; i++) {
            $lastShown = $lastShown.next();
            $lastShown.show();
            if (i == nextItemsCount - 1) {
                $lastShown.addClass('js-comment-last-shown')
            }
        }
        shownCommentsCount += nextItemsCount;
    }
    if (shownCommentsCount >= commentsCount) {
        $('#js-more-comment-btn').hide();
    }
}

$(document).mouseup(function (e) {
    var container = $('#main-date-picker');

    // if the target of the click isn't the container nor a descendant of the container
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        if (firstSelectedDay != undefined &&
            secondSelectedDay == undefined) {
            $(firstSelectedDay.elem.nextSibling).click();
            onUpdateDate();
        }
        container.hide();
    }
});

$(".share-post__button").on("click", function () {
    var popup = document.getElementById("share_popup");
    popup.classList.toggle("show");
});

function hideReservePopup() {
    $('.datepicker-container').remove();
    $('#reserve_popup').slideUp(500);
}
function updateReserveInfo(advertise_id, onDone) {
    jalaliShowLoading();
    myajax('reserve/getreserveinfo', 'accommodation_id=' + advertise_id, function (ret) {
        jalaliHideLoading();
        fillReserveInfo(advertise_id, ret.occupiedList, ret.priceDict,
            rules_string, onDone);
        date_price_dict = ret.priceDict;
    });
}

function hideReserveContainer() {
    $('.advertise-page__reserve-container').slideUp(200);
    hideReservePopup();
}

function showReserveContainer() {
    $('.advertise-page__reserve-container').slideDown(200);
}

function showDatePicker() {
    $('#main-date-picker').show();
}

function fillReserveInfo(id, occupiedList, priceDict, rules_str, onDone) {
    rules_string = rules_str;
    advertise_id = id;
    if (onDone != undefined && onDone != null) {
        onDone();
    }
    updateDatePicker([$('#main-date-picker')[0]], jalaliCurrentMonth,
        {
            priceDict: priceDict,
            occupiedList: occupiedList,
            monthOffset: 0,
            selectionType: 'multi',
            occupiedSelectEnabled: false,
            fromDateLabel: $('#js-from-date-label')[0],
            toDateLabel: $('#js-to-date-label')[0],
            onUpdateDate: function () {
                if (firstSelectedDay != undefined && secondSelectedDay != undefined) {
                    $('#main-date-picker').hide();
                    var guestCount = $("#guest_count").val();
                    if (guestCount < 1) {
                        showGuestCountSelect();
                    }
                }
                onUpdateDate();
            }
        });
}

function showGuestCountSelect() {
    $("#guest_count").focus();
}

function checkReserve(confirm_required) {
    if (firstSelectedDay == undefined ||
        secondSelectedDay == undefined) {
        showDatePicker();
        return;
    }
    var guestCount = $("#guest_count").val();
    if (guestCount < 1) {
        showGuestCountSelect();
    }
    var from_date = firstSelectedDay.date.replaceAll('/', ',');
    var to_date = secondSelectedDay.date.replaceAll('/', ',');
    myajax("reserve/checkreserve", "advertise_id=" + advertise_id +
        "&from_date=" + from_date + "&to_date=" + to_date +
        "&number_of_guests=" + guestCount, function (ret) {
            if (ret.val == 1) {
                var date_string = 'از ' + firstSelectedDay.date +
                    ' تا ' + secondSelectedDay.date +
                    ' به مدت ' + diffDaysMs(firstSelectedDay.value, secondSelectedDay.value) + ' شب';
                var guest_number_string = 'تعداد نفرات: ' + $("#guest_count").val() + ' نفر';
                price_string = $("#reserve_price_label").html();
                var time_string = 'ساعت ورود: 2 بعداز ظهر - ساعت خروج: 12 ظهر';
                var site_rules_str = '<a target="_blank" href="/rules" style="display:flex;margin-top:5px;color:#242424;">' + 'قوانین سایت' + ' ' + '<span style="padding:2px 15px;margin: 0 5px; cursor:pointer;background-color:#fdd835;color:#242424;border-radius: 5px;max-height: 25px;">' + '?' + '</span>' + '</a>';
                var rules_all_button = '<span id="advertise_rules_link" style="cursor:pointer;color:#242424;background-color:#fdd835;padding:2px 15px;border-radius:5px;margin: 0 10px;">' + '?' + '</span>';
                var content_msg = '' +
                    //date_string + '<br/>' +
                    //guest_number_string + '<br/>' +
                    //price_string + '<br/>' +
                    time_string + '<br/>' +
                    site_rules_str +
                    short_rules_string +
                    rules_all_button +
                    //'<div id="rules_accept_parent">' +
                    //'<input type="checkbox" id="rules_accept_checkbox" class="item__rules_accept_checkbox">' +
                    //'<a class="item_rules_link" href="/rules" target="_blank">قوانین سایت</a> و <span class="fake-link" class="item_rules_link" id="advertise_rules_link">قوانین این اقامتگاه</span> را قبول دارم' +
                    //'</div>' +
                    '<br/>' + '<br/>' +
                    //'<div>قوانین کنسلی: (با توافق با میزبان)</div>' +
                    //'<div>تا ۷۲ ساعت قبل از شروع سفر: 20 درصد از مبلغ به میزبان پرداخت میشود</div>' +
                    //'<div>بعد از 72 ساعت: هزینه یک شب به میزبان پرداخت میشود</div>' +
                    //'<br/>' +
                    '<div style={{fontSize:16}}>آیا قوانین سایت و قوانین اقامتگاه را قبول دارید؟</div>';
                //ret.msg;
                if (!confirm_required) {
                    //if ($('#rules_accept_checkbox').is(':checked')) {
                    if (!user_is_autenticated) {
                        showNoYesMessage('ورود به سایت',
                            'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                            function () {
                                reserve_wait_for_login = true;
                                $('.login__root').appendTo('body');
                                toggle_login();
                            }, undefined, { yesText: 'ورود', noText: 'بستن' });
                        return false;
                    }
                    if (isNumberForIran == false && verifyEmail == false) {
                        showNoYesMessage('ورود به سایت',
                            'برای ثبت درخواست رزرو، ابتدا باید ایمیل خود را ثبت و تایید کنید',
                            function () {
                                showRegisterEmailForm(setEmailToInput);
                                //$("#profileEmail").val(userEmailAddress);
                            }, undefined, { yesText: 'ثبت ایمیل', noText: 'بستن' });
                        return false;
                    }
                    reserve_wait_for_login = false;
                    myajax("reserve/reserverequest", "advertise_id=" + advertise_id +
                        "&from_date=" + from_date + "&to_date=" + to_date +
                        "&number_of_guests=" + $("#guest_count").val() +
                        "&instant_reserve=" + (instantReserveAvailable && instantReserveActivated).toString(), function (ret) {
                            if (ret.val == 1) {
                                //gtag('event', 'book', {
                                //    "items": [
                                //        {
                                //            "id": advertise_id.toString(),
                                //            "name": advertiseTitle,
                                //            "category": provinceName + "/" + cityName,
                                //            "price": last_reserve_price,
                                //            "label": advertiseTitle,
                                //            "title": advertiseTitle,
                                //            "value": last_reserve_price
                                //        }
                                //    ]
                                //});
                                $("#guest_count").val(0);
                                $("#days_label").hide();
                                $("#from_date_label").hide();
                                //$("#to_date_label").hide();
                                $("#reserve_price_label").hide();
                                window.location.href = (instantReserveAvailable && instantReserveActivated) ?
                                    '/app/reserve/list?selecttype=1&reserve_id=' + ret.reserveId + '&initialPayId=' + ret.reserveId
                                    : '/app/reserve/list?selecttype=1&msg=' + 'reserve_request';
                            }
                            else if (ret.val == 0) {
                                showErrorMessage('خطا', ret.msg);
                            }
                            else if (ret.val == 2) {
                                showNoYesMessage('ورود به سایت',
                                    'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                    function () {
                                        reserve_wait_for_login = true;
                                        $('.login__root').appendTo('body');
                                        toggle_login();
                                    }, undefined, { yesText: 'ورود', noText: 'بستن' });
                            }
                            else if (ret.val == 3) {
                                showErrorMessage('مسدود', 'امکان درخواست رزرو برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                                    '<a href="/contact">تماس با پشتیبانی</a>');
                            }
                        }
                    );
                    return;
                }
                showNoYesMessage('اطلاعات رزرو',
                    content_msg,
                    function () {
                        //if ($('#rules_accept_checkbox').is(':checked')) {
                        if (!user_is_autenticated) {
                            showNoYesMessage('ورود به سایت',
                                'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                function () {
                                    reserve_wait_for_login = true;
                                    $('.login__root').appendTo('body');
                                    toggle_login();
                                }, undefined, { yesText: 'ورود', noText: 'بستن' });
                            return false;
                        }
                        if (isNumberForIran == false && verifyEmail == false) {
                            showNoYesMessage('ورود به سایت',
                                'برای ثبت درخواست رزرو، ابتدا باید ایمیل خود را ثبت و تایید کنید',
                                function () {
                                    showRegisterEmailForm(setEmailToInput);
                                }, undefined, { yesText: 'ثبت ایمیل', noText: 'بستن' });
                            return false;
                        }
                        reserve_wait_for_login = false;
                        myajax("reserve/reserverequest", "advertise_id=" + advertise_id +
                            "&from_date=" + from_date + "&to_date=" + to_date +
                            "&number_of_guests=" + $("#guest_count").val() +
                            "&instant_reserve=" + (instantReserveAvailable && instantReserveActivated).toString(), function (ret) {
                                if (ret.val == 1) {
                                    //gtag('event', 'book', {
                                    //    "items": [
                                    //        {
                                    //            "id": advertise_id.toString(),
                                    //            "name": advertiseTitle,
                                    //            "category": provinceName + "/" + cityName,
                                    //            "price": last_reserve_price,
                                    //            "label": advertiseTitle,
                                    //            "title": advertiseTitle,
                                    //            "value": last_reserve_price
                                    //        }
                                    //    ]
                                    //});
                                    $("#guest_count").val(0);
                                    $("#days_label").hide();
                                    $("#from_date_label").hide();
                                    //$("#to_date_label").hide();
                                    $("#reserve_price_label").hide();
                                    window.location.href = (instantReserveAvailable && instantReserveActivated) ?
                                        '/app/reserve/list?selecttype=1&reserve_id=' + ret.reserveId + '&initialPayId=' + ret.reserveId
                                        : '/app/reserve/list?selecttype=1&msg=' + 'reserve_request';
                                }
                                else if (ret.val == 0) {
                                    showErrorMessage('خطا', ret.msg);
                                }
                                else if (ret.val == 2) {
                                    showNoYesMessage('ورود به سایت',
                                        'برای ثبت درخواست رزرو ابتدا باید وارد سایت شوید',
                                        function () {
                                            reserve_wait_for_login = true;
                                            $('.login__root').appendTo('body');
                                            toggle_login();
                                        }, undefined, { yesText: 'ورود', noText: 'بستن' });
                                }
                                else if (ret.val == 3) {
                                    showErrorMessage('مسدود', 'امکان درخواست رزرو برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                                        '<a href="/contact">تماس با پشتیبانی</a>');
                                }
                            }
                        );
                    },
                    function () {
                    },
                    {
                        yesText: (instantReserveAvailable && instantReserveActivated) ? 'تایید و ادامه' : 'تایید و درخواست',
                        noText: (instantReserveAvailable && instantReserveActivated) ? 'لغو رزرو' : 'لغو درخواست',
                        onOpen: function () {
                            $("#advertise_rules_link").click(function () {
                                showInfoMessage('قوانین این اقامتگاه', rules_string);
                            });
                        },
                        autoClose: false
                    }
                );
            }
            else if (ret.val == 0) {
                showErrorMessage('خطا', ret.msg);
            }
        });
}

function onUpdateDate() {
    if (firstSelectedDay) {
        instantReserveActivated = firstSelectedDay.value <= maxInstantReserveStartUnix;
        updateReserveRequestButton();
    }
    updateReservePrice();
    updateReserveLabels();
}

var last_reserve_price;
function updateReservePrice() {
    $("#reserve_price_label").html("در حال محاسبه");
    if (!firstSelectedDay || !secondSelectedDay || $("#guest_count").val() == 0) {
        $("#reserve_price_label").hide();
        return;
    }
    var from_date = firstSelectedDay.date.replaceAll('/', ',');
    var to_date = secondSelectedDay.date.replaceAll('/', ',');
    if (from_date == to_date) {
        $("#reserve_price_label").hide();
        return;
    }
    myajax("reserve/getreserveprice", "advertise_id=" + advertise_id +
        "&from_date=" + from_date + "&to_date=" + to_date +
        "&number_of_guests=" + $("#guest_count").val(), function (ret) {
            if (ret.val == 1) {
                last_reserve_price = parseInt(ret.price);
                var price_string;
                if (ret.price == ret.without_discount_price) {
                    price_string = "مبلغ " + getPriceString(ret.price);
                }
                else {
                    price_string = "مبلغ <strike>" + getPriceString(ret.without_discount_price) + "</strike> " + "<br />" + getPriceString(ret.price);
                }
                $("#reserve_price_label").html(price_string);
                $("#reserve_price_label").show();
            }
        }, false);
}
$(document).on("click", ".occupied_day_label", function () {
    alertify.error("این روز قبلا رزرو شده است");
});
$(document).on("change", "#guest_count", function () {
    onChangeGuestCount(this);
});
function setEmailToInput() {
    $("#profileEmail").val(userEmailAddress);
}
function updateReserveLabels() {
    var data_is_incorrect = false;
    if ($("#guest_count").val() == 0 || $("#guest_count").val() == '') {
        data_is_incorrect = true;
    }
    if (!firstSelectedDay || !secondSelectedDay) {
        data_is_incorrect = true;
    }
    if (data_is_incorrect) {
        $("#days_label").hide();
        $("#from_date_label").hide();
        //$("#to_date_label").hide();
        return;
    }
    $("#days_label").html(diffDaysMs(firstSelectedDay.value, secondSelectedDay.value) + " شب " + "&nbsp&nbsp" + $("#guest_count").val() + " نفر");
    var from_date = firstSelectedDay.date.slice(2);
    var to_date = secondSelectedDay.date.slice(2);
    var from_hour = '2 بعد از ظهر';
    var to_hour = '12 ظهر';
    $("#from_date_label").html("از " + from_date + " تا " + to_date);
    //$("#to_date_label").html("تا " + to_date + " " + to_hour);
    $("#days_label").show();
    $("#from_date_label").show();
    //$("#to_date_label").show();
}
function on_login_action() {
    //$("#write_comment_root").css("display", "unset");
    //$(".comment__login").css("display", "none");
    user_is_autenticated = true;
    $("#write_comment_root").show();
    $(".comment__login").hide();
    if (reserve_wait_for_login) {
        checkReserve(false);
    }
    if (show_contact_wait_for_login) {
        show_contact(show_contact_element, show_contact_id);
    }
}

$(document).on("click", ".thumbs img", function (val) {
    var info = $(this).attr("info");
    $(".largeimg .largeitem").hide();
    $(".largeimg .largeitem[info='" + info + "']").show();
})

//$.fn.digits = function () {
//    return this.each(function () {
//        $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
//    })
//}
$('.advertise-page__slider-image').show();
$('.advertise-image-library').each(function () {
    $(this).slick({
        infinite: true,
        lazyLoad: 'ondemand',
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: false,
        autoplaySpeed: 8000,
        responsive: [
            {
                breakpoint: 1201,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 1101,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 901,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 781,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 551,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 250,
                settings: {
                    slidesToShow: 1,
                    arrows: false
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });
});

function ToggleFavorite($this, $id) {
    myajax("Advertise/TggleFavorite", "id=" + $id + "&flag=" + $($this).hasClass("active"), function (ret) {
        //'status == 2' means login required
        if (ret.status == 2) {
            window.location.href = "../user/publiclogin?returnUrl=" + window.location.href;
        }
        else if (ret.status == 1) {
            if ($($this).hasClass("active")) {
                $($this).removeClass("active");
                //$this.children('span').html('افزودن به علاقه مندی ها');
                //alertify.error('از علاقه مندی ها حذف شد.');
            }
            else {
                $($this).addClass("active");
                //$this.children('span').html('حذف از علاقه مندی ها');
                //alertify.success('به علاقه مندی ها  اضافه شد .');
            }
        }
    }, false);
}
function onClickFavouriteBtn(elem) {
    ToggleFavorite($(elem), $(elem).attr("advertise_id"));
}

$(document).ready(function () {
    lightbox.option({
        'alwaysShowNavOnTouchDevices': true,
        'positionFromTop': 10
    });
    $('.item-content__profile-rate').barrating({
        theme: 'fontawesome-stars',
        readonly: true,
        initialRating: hostUserRating
    });
    $("#phonnum").hide();
    $(".thumbs img").first().click();

    //$.onCreate('div,a', function (elements) {
    //    elements.each(function () {
    //        if ($(this).hasClass("advertise-list-container")) {
    //            findLazyImages();
    //            $(this).children("*").each(function () {
    //                var new_parent = $(this).parent().parent();
    //                $(this).detach();
    //                $(this).appendTo(new_parent);
    //            });
    //            $(this).remove();
    //            if ($(".advertise-page__contact-box").css("position") == "fixed") {
    //                $(".advertise-page__contact-box").stick_in_parent();
    //            }
    //        }
    //        else if ($(this).hasClass('home-page__advertise-item-container')) {
    //            $(this).find('.average-rating').barrating({
    //                theme: 'fontawesome-stars',
    //                readonly: true,
    //                initialRating: null
    //            });
    //            //$(this).find(".home-page__favourite-button").click(function (e) {
    //            //    ToggleFavorite($(this), $(this).attr("advertise_id"));
    //            //    //e.stopPropagation();
    //            //    return false;
    //            //});
    //            if ($(".advertise-page__contact-box").css("position") == "fixed") {
    //                $(".advertise-page__contact-box").stick_in_parent();
    //            }
    //        }
    //    });
    //}, true);

    //$(".digit").digits();
    $('.average-rating').barrating({
        theme: 'fontawesome-stars',
        readonly: true,
        initialRating: null
    });
    initializeGuestAndDate();

    $('#guest_count').focus(function () {
        $(this).val('');
    });
});

function initializeGuestAndDate() {
    if (initialGuestCount > 0) {
        $('#guest_count').val(initialGuestCount);
    }
    if (typeof fromDateValue != 'undefined' ||
        typeof toDateValue != 'undefined') {
        if (typeof fromDateValue != 'undefined') {
            var fromDateG = new Date(fromDateValue);
            var fromDateJ = gregorianToJalaliDate(fromDateG);
            firstSelectedDay = {
                date: fromDateJ.dateString,
                value: fromDateValue
            }
            $('#js-from-date-label').html(fromDateJ.dateString.substring(2));
        }
        if (typeof toDateValue != 'undefined') {
            var toDateG = new Date(toDateValue);
            var toDateJ = gregorianToJalaliDate(toDateG);
            secondSelectedDay = {
                date: toDateJ.dateString,
                value: toDateValue
            }
            $('#js-to-date-label').html(toDateJ.dateString.substring(2));
        }
    }

    onUpdateDate();
}

function shareOnWathsapp(text) {
    var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    if (!isMobile) {
        window.open('https://wa.me/?text=' + text, 'whatsappShare', 'width=626,height=436'); return false;
        return false;
    }
}