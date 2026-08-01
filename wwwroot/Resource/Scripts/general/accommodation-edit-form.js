//function advertiseSelectOwner($this) {
//    if ($($this).val() == "2") {
//        $(".js-owner").removeClass("hide-aditem");
//    }
//    else {
//        $(".js-owner").addClass("hide-aditem");
//    }
//}

function onAdvertisedigitschange($this) {

    if ($($this).hasClass("toman")) {

        var str_toman = "";
        var price_val = parseInt($($this).val())
        if (price_val >= 1000000000) {
            str_toman += Math.floor(price_val / 1000000000) + " میلیارد";
            price_val = price_val % 1000000000;
        }
        if (price_val >= 1000000) {
            if (str_toman.length > 3) {
                str_toman += " و " + Math.floor(price_val / 1000000) + " میلیون";
            }
            else {
                str_toman += Math.floor(price_val / 1000000) + " میلیون";
            }
            price_val = price_val % 1000000;
        }
        if (price_val >= 1000) {
            if (str_toman.length > 3) {
                str_toman += " و " + Math.floor(price_val / 1000) + " هزار";
            }
            else {
                str_toman += Math.floor(price_val / 1000) + " هزار";
            }
            price_val = price_val % 1000;

        }
        if (price_val > 0) {

            if (str_toman.length > 3) {
                str_toman += " و " + price_val;
            }
            else {
                str_toman += price_val;
            }
        }

        $($this).parent().next().text(str_toman + " تومان");

        //$($this).val($($this).val().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    }
}

function send_accomodation_form(admin, status_content) {
    if (admin) {
        $("#status-content").val(status_content);
        $("#not-verify").val($("#not-verify-dp").val());
    }
    if (validateAccomodationForm(admin, document, !admin)) {
        document.getElementById("ManageForm").submit();
    }
}
var prev_daily_price;
var prev_holiday_price;
var prev_holiday_pike_price;
var prev_rent_price;
var prev_more_than_capacity_price;
var prev_norouz_price;

function validateFormPrices(admin, rootElement) {
    var $daily_price_elem = $(rootElement).find("input[name='DailyPrice']");
    var $holiday_price_elem = $(rootElement).find("input[name='HolidayPrice']");
    var $holiday_pike_price_elem = $(rootElement).find("input[name='HolidayPikePrice']");
    var $rent_price_elem = $(rootElement).find("input[name='RentPrice']");
    var $more_than_capacity_price_elem = $(rootElement).find("input[name='MoreThanCapacityPrice']");
    var $norouz_price_elem = $(rootElement).find("input[name='NorouzPrice']");
    if (!validateFormPrice($daily_price_elem, prev_daily_price)) {
        return "قیمت پایه شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_daily_price + " قیمت جدید: " + $daily_price_elem.val();
    }
    if (!validateFormPrice($holiday_price_elem, prev_holiday_price)) {
        return "قیمت روز های تعطیل شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_holiday_price + " قیمت جدید: " + $holiday_price_elem.val();
    }
    if (!validateFormPrice($holiday_pike_price_elem, prev_holiday_pike_price)) {
        return "قیمت پیک تعطیلات شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_holiday_pike_price + " قیمت جدید: " + $holiday_pike_price_elem.val();
    }
    if (!validateFormPrice($rent_price_elem, prev_rent_price)) {
        return "قیمت اجاره ماهیانه شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_rent_price + " قیمت جدید: " + $rent_price_elem.val();
    }
    if (!validateFormPrice($more_than_capacity_price_elem, prev_more_than_capacity_price)) {
        return "قیمت بیشتر از ظرفیت شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_more_than_capacity_price + " قیمت جدید: " + $more_than_capacity_price_elem.val();
    }
    if (!validateFormPrice($norouz_price_elem, prev_norouz_price)) {
        return "قیمت نوروز شما اختلاف فاحشی دارد. آیا از تغییر قیمت اطمینان دارید؟ قیمت قدیم: " + prev_norouz_price + " قیمت جدید: " + $norouz_price_elem.val();
    }
    return "";
}

function validateFormPrice($elem, prev_price) {
    if ($elem.length == 0 || prev_price < 1) {
        return true;
    }
    var new_price = parseInt($elem.val());
    if (Math.abs(new_price - prev_price) >= prev_price ||
        Math.abs(new_price - prev_price) >= new_price) {
        return false;
    }
    return true;
}

function validateAccomodationForm(admin, rootElement, validatePrices) {

    admin = typeof admin !== 'undefined' ? admin : false;
    status_content = typeof status_content !== 'undefined' ? status_content : '';
    if (validatePrices === true) {
        var validate_prices_result = validateFormPrices(admin, rootElement);
        if (validate_prices_result != "") {
            showNoYesMessage("هشدار", validate_prices_result, function () {
                if (executeValidateForm(false, document)) {
                    document.getElementById("ManageForm").submit();
                }
            }, function () {
                return false;
            }, {
                onClose: function () {
                    return false;
                },
                noText: 'اصلاح قیمت'
            });
            return false;
        }
        else {
            return executeValidateForm(admin, rootElement);
        }
    }
    else {
        return executeValidateForm(admin, rootElement);
    }
}

function executeValidateForm(admin, rootElement) {
    var flag = true;

    $(rootElement).find('.unit_count_input').removeClass("mandatory");
    if ($(rootElement).find('.unit_count_input').length > 0) {
        if ($(rootElement).find('.unit_count_input').filter(function () {
            return $(this).val() > 0;
        }).length == 0) {
            flag = false;
            var error_string = $(rootElement).find('.unit_count_input[data-type-id]').length > 0 ?
                'لطفا تعداد واحد های مجتمع را مشخص کنید' :
                'لطفا تعداد اتاق ها را مشخص کنید';
            alertify.error(error_string, 11000);
            $(rootElement).find('.unit_count_input').addClass("mandatory");
        }
    }

    $(rootElement).find("select.js-mandatory").removeClass("mandatory");
    $(rootElement).find("select.js-mandatory").each(function () {
        if ($(this).find('option').length > 1) {
            var min_value_ = 1;
            var min_value_attr = $(this).attr("data-min-value");
            if (min_value_attr != null && min_value_attr != undefined) {
                min_value_ = parseInt(min_value_attr);
            }
            if ($(this).val() < min_value_) {
                flag = false;
                alertify.error($(this).parent().prev().text(), 11000);
                $(this).addClass("mandatory");
            }
        }
    });

    $(rootElement).find("input.js-mandatory").removeClass("mandatory");
    $(rootElement).find("input.js-mandatory").each(function () {
        if ($(this).hasClass("digit")) {
            var min_value = 1;
            if ($(this).attr("allowzero") === '1')
                min_value = 0;
            if ($(this).val() == "" || $(this).val() < min_value) {
                flag = false;
                alertify.error($(this).parent().prev().text(), 11000);
                $(this).addClass("mandatory");
            }
            var min_value_attr = $(this).attr('data-min_value');
            if (typeof min_value_attr !== undefined && min_value_attr !== false) {
                if ($(this).val() == "" || $(this).val() < parseInt(min_value_attr)) {
                    flag = false;
                    alertify.error($(this).parent().prev().text() + ". حداقل مبلغ " + min_value_attr + " تومان", 11000);
                    $(this).addClass("mandatory");
                }
            }
        }
        else {
            if ($(this).attr("type") == "number") {
                var allow_zero = $(this).attr('data-allow-zero');
                if (allow_zero !== 'True' && ($(this).val() == '' || parseInt($(this).val()) < 1)) {
                    flag = false;
                    var error_msg_;
                    if ($(this).hasClass("Room_input")) {
                        error_msg_ = "تعداد اتاق خواب را وارد کنید. در صورتی که واحد شما اتاق خواب ندارد آگهی را در دسته اتاق و سوئیت ثبت کنید";
                    }
                    else {
                        error_msg_ = $(this).parent().parent().prev().text();
                    }
                    alertify.error(error_msg_, 11000);
                    $(this).addClass("mandatory");
                }
            }
            else {
                if ($(this).val() == "") {
                    flag = false;
                    alertify.error($(this).parent().prev().text(), 11000);
                    $(this).addClass("mandatory");
                }
            }
        }
    });

    $(rootElement).find("input.js-mandatory-depend").removeClass("mandatory");
    $(rootElement).find("input.js-mandatory-depend").each(function () {
        if ($(this).hasClass("digit") &&
            $(this).attr('mandatory-depend') == $("#TypeID").val()) {
            if ($(this).val() == "" || $(this).val() < 1) {
                flag = false;
                alertify.error($(this).parent().prev().text(), 11000);
                $(this).addClass("mandatory");
            }
        }
    });

    //if ($("#tmp-title").val().length < 5) {
    //    flag = false;
    //    alertify.error("لطفا عنوانی مناسب برای اقامتگاه انتخاب کنید", 11000);
    //    $("input[name='Title']").addClass("mandatory");
    //}

    $(rootElement).find("textarea.js-mandatory").removeClass("mandatory");
    $(rootElement).find("textarea.js-mandatory").each(function () {
        if ($(this).val() == "") {
            flag = false;
            alertify.error($(this).parent().prev().text(), 11000);
            $(this).addClass("mandatory");
        }
    });
    if (admin) {
        if ($("#mandatory-check").val() != "check") {
            flag = true;
        }
    }
    return flag;
}

function custom_show_alert(msg) {
    alertify.error(msg, 11000);
}
//var $removingAlbumPhoto;
//var $removingPhotoID;
//var currentDropZoneIndex = 0;

//function setAsRemovingPhoto(elem) {
//    $removingAlbumPhoto = $(elem).parent().parent().parent().parent().find('.AlbumPhoto');
//    $removingPhotoID = $(elem).parent().parent().parent().parent().find('.PhotoID');
//}
//function RemoveOldPhoto(obj, $id) {
//    if (confirm("آیا عکس مورد نظر پاک شود؟")) {
//        setAsRemovingPhoto(obj);
//        $(obj).parent().remove();
//        var myString = $removingAlbumPhoto.val();
//        var subString = "," + $id;
//        if (myString.indexOf(subString) >= 0) {
//            var myString = myString.replace(subString, '');
//            $removingAlbumPhoto.val(myString);
//            if (myString == ',') {
//                $removingPhotoID.val(0);
//            }
//            else if ($removingPhotoID.val() == $id) {
//                $removingAlbumPhoto.parent().find('.dz-preview:first').click();
//            }
//        }
//    }
//    event.stopPropagation();
//}
//function initializeNewDropZone(elem) {
//    $(elem).attr("id", "dropzone_" + currentDropZoneIndex);
//    var myDropzone = new Dropzone("#dropzone_" + currentDropZoneIndex, {
//        url: "/Post/SaveUploadedFile",
//        previewTemplate: $("#tpl").html(),
//        addRemoveLinks: false,
//        paramName: "file", // The name that will be used to transfer the file
//        maxFilesize: 10000,  // MB
//        timeout: 380000, // mili second
//        parallelUploads: 1,
//        ignoreHiddenFiles: false,
//        dictRemoveFileConfirmation: "آیا عکس مورد نظر پاک شود؟"
//    });

//    myDropzone.on("uploadprogress", function (file, progress, bytesSent) {
//        var $progress = file.previewElement.querySelector(".progress-text");
//        $progress.style.width = Math.floor(progress) + "%";
//        $progress.querySelector(".dz-percent").textContent = Math.floor(progress) + "%";
//        // Display the progress
//    });

//    myDropzone.on("addedfile", function (file) {
//        if (this.files.length > 50) {
//            alertify.error("حداکثر 50 عکس مجاز می باشد .");
//            myDropzone.removeFile(file);
//        }
//    });

//    myDropzone.on("complete", function (data) {
//        if (data.xhr) {
//            var ret = jQuery.parseJSON(data.xhr.responseText);
//            if (ret.Status == 1) {

//                var $album_photo = $(data.previewElement).parent().parent().parent().find('.AlbumPhoto');
//                $album_photo.val($album_photo.val() + ret.id + ",");
//                var $progress = data.previewElement.querySelector(".progress-text");
//                $($progress.querySelector(".dz-percent")).hide();
//                $($progress.querySelector(".dz-finish")).show();

//                var $js_id = data.previewElement.querySelector(".js-id");
//                $($js_id).val(ret.id);
//                if ($(data.previewElement).parent().children(".dz-preview-active").length == 0) {
//                    $(data.previewElement).parent().find('.dz-preview:first').click();;
//                }
//            }
//            else {
//                alertify.error(ret.Message);
//                myDropzone.removeFile(data);
//            }
//        }
//        else {
//            myDropzone.removeFile(data);
//        }
//    });

//    myDropzone.on("removedfile", function (data) {
//        if (data.xhr) {
//            var ret = jQuery.parseJSON(data.xhr.responseText);
//            if (ret.Status == 1) {
//                var myString = $removingAlbumPhoto.val();
//                var subString = "," + ret.id;
//                if (myString.indexOf(subString) >= 0) {
//                    var myString = myString.replace(subString, '');
//                    $removingAlbumPhoto.val(myString);
//                    if (myString == ',') {
//                        $removingPhotoID.val(0);
//                    }
//                    else if ($removingPhotoID.val() == ret.id) {
//                        $removingAlbumPhoto.parent().find('.dz-preview:first').click();
//                    }
//                }
//            }
//        }
//    });

//    myDropzone.on("error", function (file, message) {
//        if (message.indexOf("Max filesize") >= 0)
//            custom_show_alert("حداکثر سایز فایل 10 مگابایت میتواند باشد .");
//        else {
//            custom_show_alert("فایل انتخابی مورد قبول نیست .");
//        }
//        myDropzone.removeFile(file);
//    });
//    currentDropZoneIndex++;
//}
//function setupDropZone() {
//    // script for imgaes upload dropzone
//    Dropzone.autoDiscover = false;
//    $(document).off("click", ".dz-preview").on("click", ".dz-preview", function () {
//        var $photo_id = $(this).parent().parent().parent().children('.PhotoID');
//        $(this).parent().children(".dz-preview").removeClass("dz-preview-active");
//        $(this).parent().children(".dz-preview").find(".dz-main-photo").hide();
//        $photo_id.val($(this).find(".js-id").val());
//        $(this).addClass("dz-preview-active");
//        $(this).find(".dz-main-photo").show();
//    });
//}

var refreshing_form_elements = false;

function refreshAdvertiseFormElements() {
    refreshing_form_elements = true;
    var typeID = parseInt($('#TypeID').val());
    resetChildrenCount();
    var myData = $('#ManageForm').serialize();
    refresh_form_elements('_AccFormBasicElements', myData, function () {
        refresh_form_elements('_AccFormGeneralElements', myData, function () {
            refresh_form_elements('_AccFormElements', myData, function () {
                $('#form-container').children('.form-step').each(function () {
                    var elem_id = $(this).attr('id');
                    if (elem_id != '_AccFormBasicElements' &&
                        elem_id != '_AccFormGeneralElements' &&
                        elem_id != '_AccFormElements' &&
                        elem_id != 'confirmation-form-step') {
                        $(this).remove();
                    }
                });
                if (typeof onRefreshFormElementDone != 'undefined') {
                    onRefreshFormElementDone();
                }
                refreshing_form_elements = false;
                if (typeID == 87 || typeID == 3 || typeID == 4 || typeID == 6 || typeID == 7) {
                    increaseHotelUnitCount(typeID, 1);
                }
            });
        });
    });
}

function refresh_form_elements(partial_string, newData, onDone) {
    newData += '&partial_view=' + partial_string;
    $.ajax({
        type: "POST",
        url: "/accomodation/getaccformpartialview",
        data: newData,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (data) {
            $('#' + partial_string).html(data);
            onRefreshFormElements(function () {
                if (onDone != undefined && onDone != null) {
                    onDone();
                }
            });
        }
    });
}

var currentChildUnitIndex = -1;
var currentApartmentIndex = -1;
var currentSuitIndex = -1;
var currentHouseIndex = -1;
var currentVillaIndex = -1;
var currentHutIndex = -1;

function resetChildrenCount() {
    currentChildUnitIndex = -1;
    currentApartmentIndex = -1;
    currentSuitIndex = -1;
    currentHouseIndex = -1;
    currentVillaIndex = -1;
    currentHutIndex = -1;
}

function getComplexChildrenCount(advertise_type) {
    switch (advertise_type) {
        case 82:
            return currentApartmentIndex + 1;
        case 1:
            return currentSuitIndex + 1;
        case 2:
            return currentHouseIndex + 1;
        case 83:
            return currentVillaIndex + 1;
        case 9:
            return currentHutIndex + 1;
        default:
            return 0;
    }
}

function newHotelUnit(parent_type) {
    currentChildUnitIndex++;
    var number = currentChildUnitIndex + 1;
    var unit_title;
    var parent_title;
    switch (parent_type) {
        case 87:
            unit_title = "اتاق نوع";
            parent_title = "اتاق های هتل";
            break;
        case 3:
            unit_title = "چادر نوع";
            parent_title = "چادر های کمپ";
            break;
        case 4:
            unit_title = "اتاق نوع";
            parent_title = "اتاق های اقامتگاه";
            break;
        case 6:
            unit_title = "اتاق نوع";
            parent_title = "اتاق های مسافرخانه";
            break;
        case 7:
            unit_title = "اتاق نوع";
            parent_title = "اتاق های پانسیون";
            break;
    }
    var parent_id = parent_type + "_units";
    //if ($('#' + parent_id).length == 0) {
    //    $('#form-container').append('<div class="form-step" id="' + parent_type + '_units" data-title="' + parent_title + '"><div class="form-step-button-container"></div></div>');
    //}
    //$('#' + parent_id).append('<div class="advertise-form__hotel-unit-container form-step" id="unit_' + currentChildUnitIndex + '" data-title="' + unit_title + ' ' + number + '"></div>');
    if ($('#' + parent_id).length == 0) {
        $('#hotel-add-remove-container').before('<div id="' + parent_type + '_units"></div>');
    }
    $('#' + parent_id).append('<div class="advertise-form__hotel-unit-container advetise-form__part" id="unit_' + currentChildUnitIndex + '"></div>');
    $("#unit_" + currentChildUnitIndex).load('/accomodation/gethotelunititem?index=' + currentChildUnitIndex + '&type_id=' + parent_type + '&unit_title=' + unit_title.replace(' نوع', ''), null, function () {
        if (onHotelUnitCountUpdated != null) {
            onHotelUnitCountUpdated();
        }
    });
}

function removeLastHotelUnit(parent_type) {
    var parent_id = parent_type + "_units";
    if ($('#' + parent_id).length == 0) {
        return;
    }
    $('#' + parent_id).children('.advertise-form__hotel-unit-container').last().remove();
    currentChildUnitIndex--;
    if ($('#' + parent_id).children('.advertise-form__hotel-unit-container').length == 0) {
        $('#' + parent_id).remove();
    }
    if (onHotelUnitCountUpdated != null) {
        onHotelUnitCountUpdated();
    }
}

function newComplexUnit(advertise_type, parent_type, advertise_id) {
    advertise_id == undefined || advertise_id == null ? 0 : advertise_id;
    currentChildUnitIndex++;
    var number;
    var unit_title;
    var parent_title;
    switch (advertise_type) {
        case 82:
            currentApartmentIndex++;
            number = currentApartmentIndex + 1;
            unit_title = "واحد";
            parent_title = "واحد های آپارتمان";
            break;
        case 1:
            currentSuitIndex++;
            number = currentSuitIndex + 1;
            unit_title = "اتاق سوئیت";
            parent_title = "واحد های اتاق و سوئیت";
            break;
        case 2:
            currentHouseIndex++;
            number = currentHouseIndex + 1;
            unit_title = "خانه";
            parent_title = "خانه های ویلایی";
            break;
        case 83:
            currentVillaIndex++;
            number = currentVillaIndex + 1;
            unit_title = "ویلا";
            parent_title = "ویلا ها";
            break;
        case 9:
            currentHutIndex++;
            number = currentHutIndex + 1;
            unit_title = "کلبه";
            parent_title = "کلبه ها";
            break;
    }
    var parent_id = advertise_type + "_units";
    if ($('#' + parent_id).length == 0) {
        var $confirm_step = $('#confirmation-form-step');
        if ($confirm_step.length > 0) {
            $confirm_step.before('<div class="form-step" id="' + advertise_type + '_units" data-title="' + parent_title + '"><div class="form-step-button-container"></div></div>');
        }
        else {
            $('#form-container').append('<div class="form-step" id="' + advertise_type + '_units" data-title="' + parent_title + '"><div class="form-step-button-container"></div></div>');
        }
    }
    $('#' + parent_id).append('<div class="advertise-form__hotel-unit-container form-step" id="unit_' + currentChildUnitIndex + '" data-title="' + unit_title + ' ' + number + '"></div>');
    $("#unit_" + currentChildUnitIndex).load('/accomodation/getcomplexunititem?advertise_type=' + advertise_type + '&index=' + currentChildUnitIndex + '&number=' + number + '&parent_type=' + parent_type + '&advertise_id=' + advertise_id, null, function () {
        if ($('#' + parent_id).children('.form-step').length > 1) {
            if (parseInt($("#unit_" + currentChildUnitIndex).find('.js-advertise-id').first().val()) < 1) {
                copyFormElements($('#' + parent_id).children('.form-step').first()[0],
                    $("#unit_" + currentChildUnitIndex)[0]);
            }
        }
        if (onComplexUnitCountUpdated != null) {
            onComplexUnitCountUpdated();
        }
    });
}

function removeLastComplexUnit(advertise_type) {
    var parent_id = advertise_type + "_units";
    if ($('#' + parent_id).length == 0) {
        return;
    }
    $('#' + parent_id).children('.form-step').last().remove();
    currentChildUnitIndex--;
    switch (advertise_type) {
        case 82:
            currentApartmentIndex--;
            break;
        case 1:
            currentSuitIndex--;
            break;
        case 2:
            currentHouseIndex--;
            break;
        case 83:
            currentVillaIndex--;
            break;
        case 9:
            currentHutIndex--;
            break;
    }
    if ($('#' + parent_id).children('.form-step').length == 0) {
        $('#' + parent_id).remove();
    }
    if (onComplexUnitCountUpdated != null) {
        onComplexUnitCountUpdated();
    }
}

function validateComplexUnitRange(elem, current_count, new_count) {
    var min = parseInt($(elem).attr('data-min'));
    var max = parseInt($(elem).attr('data-max'));
    if (new_count < min) {
        $(elem).val(current_count);
        if (min > 0) {
            alertify.error('شما نمیتوانید آگهی ثبت شده را حذف کنید. دکمه غیرفعال سازی روی آگهی ها موجود میباشد');
        }
        return false;
    }
    else if (new_count > max) {
        alertify.error('شما میتوانید حداکثر ' + max + ' واحد ثبت کنید');
        $(elem).val(current_count);
        return false;
    }
    return true;
}

function onChangeComplexUnitCount(elem, parent_type) {
    if (refreshing_form_elements)
        return;
    var advertise_type = parseInt($(elem).attr('data-type-id'));
    var current_count = parseInt(getComplexChildrenCount(advertise_type));
    var new_count = parseInt($(elem).val());
    if (!validateComplexUnitRange(elem, current_count, new_count)) {
        return;
    }
    if (new_count > current_count) {
        var count = new_count - current_count;
        for (var i = 0; i < count; i++) {
            newComplexUnit(advertise_type, parent_type);
        }
    }
    else if (new_count < current_count) {
        var count = current_count - new_count;
        for (var i = 0; i < count; i++) {
            removeLastComplexUnit(advertise_type);
        }
    }
}
function increaseHotelUnitCount(advertise_type, min, max) {
    onChangeHotelUnitCount(currentChildUnitIndex + 2, advertise_type, min, max);
}
function decreaseHotelUnitCount(advertise_type, min, max) {
    onChangeHotelUnitCount(currentChildUnitIndex, advertise_type, min, max);
}
function onChangeHotelUnitCount(new_count, advertise_type, min, max) {
    if (refreshing_form_elements)
        return;
    var current_count = currentChildUnitIndex + 1;
    if (new_count < 1) {
        alertify.error('باید حداقل یک نوع موجود باشد');
        return;
    }
    if (new_count > 30) {
        alertify.error('شما میتوانید حداکثر ' + 30 + 'نوع اتاق داشته باشید');
        return;
    }
    if (new_count < min) {
        alertify.error('شما نمیتوانید آگهی ثبت شده را حذف کنید. دکمه غیرفعال سازی روی آگهی ها موجود میباشد');
        return;
    }
    if (new_count > current_count) {
        var count = new_count - current_count;
        for (var i = 0; i < count; i++) {
            newHotelUnit(advertise_type);
        }
    }
    else if (new_count < current_count) {
        var count = current_count - new_count;
        for (var i = 0; i < count; i++) {
            removeLastHotelUnit(advertise_type);
        }
    }
}

function addOldAdvertise(btnElem, parent_type) {
    var $selectElement = $('#add-old-advertise-select');
    if ($selectElement.val() == -1) {
        alertify.error('ابتدا باید از لیست آگهی های ثبت شده یک مورد را انتخاب کنید');
        $selectElement.focus();
        return;
    }
    var $selectedOptionElement = $selectElement.children('option[value=' + $selectElement.val() + ']');
    var advertise_type = $selectedOptionElement.attr('data-advertise-type');
    newComplexUnit(parseInt(advertise_type), parseInt(parent_type), $selectElement.val());
    $('.unit_count_input[data-type-id=' + advertise_type + ']').val(
        parseInt($('.unit_count_input[data-type-id=' + advertise_type + ']').val()) + 1);
    $selectedOptionElement.remove();
    if ($selectElement.children('option').length == 1) {
        $selectElement.parent().parent().remove();
    }
}

function copyFormElements(from, to) {
    var fromIndex = parseInt($(from).attr('id').replace('unit_', ''));
    if ($(from).find('.uploader-container').length > 0) {
        var $from_image_container = $(from).find('.uploader-container').first();
        var $to_image_container = $(to).find('.uploader-container').first();
        $from_image_container.children('.dz-preview').each(function () {
            $to_image_container.append($(this).clone());
            $to_image_container.children('.dz-preview').last().find('.dz-remove').attr('onclick', 'RemoveOldPhoto(this, ' + $to_image_container.children('.dz-preview').last().find('.js-id').first().val() + ');');
        });
        setupDropZone();
    }
    var child_from_string = 'childs[' + fromIndex + '].';
    var forbidden_properties = ['Floor', 'AdvertiseID', 'Available'];
    $(from).find('*[name]').each(function () {
        var from_element = this;
        var name_to_search = $(this).attr('name').replace(child_from_string, '');
        if (name_to_search == "Elevator") {

        }
        if (!forbidden_properties.includes(name_to_search)) {
            $(to).find('*[name]').each(function () {
                if ($(this).attr('name').split('.')[1] == name_to_search) {
                    if (name_to_search == "Elevator") {

                    }
                    $(this).val($(from_element).val());
                    if ($(this).attr('type') == 'checkbox') {

                        if ($(from_element).is(':checked')) {
                            $(this)[0].setAttribute("checked", "checked");
                        }
                        else {
                            $(this)[0].setAttribute("checked", ""); // For IE
                            $(this)[0].removeAttribute("checked"); // For other browsers
                        }
                    }
                }
            });
        }
    });
}

$('.js-form-boolean').change(function () {
    onChangeFormBoolean(this);
});
function onChangeFormBoolean(elem) {
    $(elem).val($(elem).is(':checked') ? 'true' : 'false');
}

/******confirm helper******/
function confirmCheckbox(elem) {
    var $input = $(elem);
    $input.prop('checked', !$input.prop('checked'));
    if (!$input.prop('checked')) {
        showNoYesMessage('تایید', $input.attr('data-check-confirm-msg'), function () {
            $input.prop("checked", true);
            $input.val('true');
        }, function () {
            $input.prop("checked", false);
            $input.val('false');
        });
    }
    else {
        showNoYesMessage('تایید', $input.attr('data-uncheck-confirm-msg'), function () {
            $input.prop("checked", false);
            $input.val('false');
        }, function () {
            $input.prop("checked", true);
            $input.val('true');
        });
    }
}
/******end confirm helper******/

/*****form step*****/
var form_step_elements = [];
var form_child_step_dict = {};
var form_selected_index = 0;
var form_child_selected_index = 0;
var disableFormButtons;

function unlockFormButton(elem) {
    $(elem).removeClass('disabled-button');
}

function initializeFormSteps(disableButtons) {
    //disableFormButtons = false;
    disableFormButtons = disableButtons == undefined || disableButtons == null ?
    false : disableButtons;
    updateStepsFormElements();
    $('#prev_step_button').addClass('hidden-button');
    $('#form-submit-button').addClass('hidden-button');
}
function updateStepsFormElements(onDone) {
    form_step_elements = [];
    form_child_step_dict = {};
    form_child_selected_index = 0;
    var index = 0;
    $('#form-container').children('.form-step').each(function () {
        if (index != form_selected_index) {
            $(this).hide();
        }
        form_step_elements.push(this);
        var child_index = 0;
        $(this).children('.form-step').each(function () {
            if (child_index != 0) {
                $(this).hide();
            }
            else {
                form_child_step_dict[index] = [];
            }
            form_child_step_dict[index].push(this);
            child_index++;
        });
        index++;
    });
    $('.form-step-button-container').empty();
    for (var i = 0; i < index; i++) {
        var select_text = i == form_selected_index ? ' form-step-button-selected' : '';
        var disabled_text = disableFormButtons && i != form_selected_index ? ' disabled-button' : '';
        $('#form-step-button-container').append(
            '<div class="form-step-button' + select_text + disabled_text + '" data-index="' + i + '">' +
            $(form_step_elements[i]).attr('data-title') + '</div>');
        if ((i in form_child_step_dict)) {
            for (var j = 0; j < form_child_step_dict[i].length; j++) {
                select_text = j == 0 ? ' form-step-button-selected' : '';
                disabled_text = disableFormButtons ? ' disabled-button' : '';
                $(form_step_elements[i]).find('.form-step-button-container').append(
                    '<div class="form-step-button' + select_text + disabled_text + '" data-index="' + j +
                    '" data-parent-index="' + i + '">' +
                    $(form_child_step_dict[i][j]).attr('data-title') + '</div>');
            }
        }
    }
    $('.form-step-button').click(function () {
        if ($(this).hasClass('disabled-button')) {
            return;
        }
        var index = parseInt($(this).attr('data-index'));
        var $form_container = $(this).attr('data-parent-index') == undefined ?
            $('#form-container') :
            $(this).parent().parent();
        $form_container.children('.form-step').hide();
        if ($form_container.hasClass('form-step')) {
            var parent_index = parseInt($(this).attr('data-parent-index'));
            $(form_child_step_dict[parent_index][index]).fadeIn(1000);
            form_child_selected_index = index;
        }
        else {
            $(form_step_elements[index]).fadeIn(1000);
            form_selected_index = index;
            if ($(form_step_elements[index]).children('.form-step').length == 0) {
                form_child_selected_index = 0;
            }
        }
        $(this).parent().children('.form-step-button').removeClass('form-step-button-selected');
        $(this).addClass('form-step-button-selected');
        if (form_selected_index <= 0 && form_child_selected_index <= 0) {
            $('#next_step_button').removeClass('hidden-button');
            $('#prev_step_button').addClass('hidden-button');
            $('#form-submit-button').addClass('hidden-button');
        }
        else if (form_selected_index >= form_step_elements.length - 1 &&
            (form_child_step_dict[form_selected_index] == undefined ||
            form_child_step_dict[form_selected_index] == null ||
            form_child_selected_index >= form_child_step_dict[form_selected_index].length - 1)) {
            $('#prev_step_button').removeClass('hidden-button');
            $('#next_step_button').addClass('hidden-button');
            $('#form-submit-button').removeClass('hidden-button');
        }
        else {
            $('#next_step_button').removeClass('hidden-button');
            $('#prev_step_button').removeClass('hidden-button');
            $('#form-submit-button').addClass('hidden-button');
        }
        if (!tempDisableAnimation) {
            $([document.documentElement, document.body]).animate({
                scrollTop: $(this).offset().top
            }, 200);
        }
    });
    for (var i = 0; i < index; i++) {
        if ((i in form_child_step_dict)) {
            $(form_step_elements[i]).find('.form-step-button-container').children().last().click();
        }
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
    if (typeof OnUpdateStepFormElements != 'undefined') {
        OnUpdateStepFormElements();
    }
}

function nextFormSubStep(btn, is_admin) {
    if ($(btn).hasClass('hidden-button')) {
        return;
    }
    seekFormSubStep(true, is_admin);
}

function prevFormSubStep(btn, is_admin) {
    if ($(btn).hasClass('hidden-button')) {
        return;
    }
    seekFormSubStep(false, is_admin);
}

var coppied_steps = [];

function seekFormSubStep(forward, is_admin) {
    var step_element = form_step_elements[form_selected_index];
    //validate current form
    var validate_element = step_element;
    if ($(step_element).children('.form-step').length > 0) {
        validate_element = form_child_step_dict[form_selected_index][form_child_selected_index];
    }
    if (forward && !validateAccomodationForm(false, validate_element)) {
        if (!is_admin) {
            return;
        }
    }
    if (form_child_selected_index == 0 &&
            disableFormButtons &&
            !coppied_steps.includes(form_selected_index)) {
        coppied_steps.push(form_selected_index);
        if ($(step_element).children('.form-step').length > 1) {
            $(step_element).children('.form-step:not(:first)').each(function () {
                if (parseInt($(this).find('.js-advertise-id').first().val()) < 1) {
                    copyFormElements(form_child_step_dict[form_selected_index][0], $(this));
                }
            });
        }
    }
    //change form
    if ($(step_element).children('.form-step').length == 0) {
        SeekFormStep(forward, is_admin);
    }
    else {
        if (forward && form_child_selected_index >=
            form_child_step_dict[form_selected_index].length - 1) {
            SeekFormStep(forward, is_admin);
        }
        else if (!forward && form_child_selected_index <= 0) {
            SeekFormStep(forward, is_admin);
        }
        else {
            var target_child_index = forward ? form_child_selected_index + 1 :
                form_child_selected_index - 1;
            var $buttons_root = $(step_element).children('.form-step-button-container').first();
            if (forward && is_admin && target_child_index > 0) {
                var $current_button = $buttons_root.children('.form-step-button:eq(' + (target_child_index - 1).toString() + ')');
                $current_button.css('background-color', '#73C362');
            }
            var $target_button = $buttons_root.children('.form-step-button:eq(' + target_child_index + ')');
            unlockFormButton($target_button[0]);
            $target_button.click();
        }
    }
}

var tempDisableAnimation = false;

function SeekFormStep(forward, is_admin) {
    is_admin = is_admin == undefined || is_admin == null ? false : is_admin;
    var target_index = forward ? form_selected_index + 1 : form_selected_index - 1;
    var $buttons_root = $('#form-container').parent().parent().children('.form-step-button-container').first();
    if (forward && is_admin && target_index > 0) {
        var $current_button = $buttons_root.children('.form-step-button:eq(' + (target_index - 1).toString() + ')');
        $current_button.css('background-color', '#73C362');
    }
    var $target_button = $buttons_root.children('.form-step-button:eq(' + target_index + ')');
    var target_element = form_step_elements[target_index];
    unlockFormButton($target_button[0]);
    $target_button.click();
    form_selected_index = target_index;
    if ($(target_element).children('.form-step-button-container').length > 0) {
        var $step_buttons = $(target_element).children('.form-step-button-container')
            .first().children('.form-step-button');
        if (forward) {
            unlockFormButton($step_buttons.first()[0]);
            $step_buttons.first().click();
        }
        else {
            unlockFormButton($step_buttons.last()[0]);
            $step_buttons.last().click();
        }
        if ($step_buttons.length == 1) {
        }
    }
}

function onRefreshFormElements(onDone) {
    $('.advertise-form__hotel-unit-container').remove();
    var myData = $('#ManageForm').serialize();
    myData += '&partial_view=_AccMultiform';
    $.ajax({
        type: "POST",
        url: "/accomodation/getaccformpartialview",
        data: myData,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (data) {
            $('#form-container').append(data);
            updateStepsFormElements(onDone);
        }
    });
}

function onComplexUnitCountUpdated() {
    updateStepsFormElements();
}

function onHotelUnitCountUpdated() {
    updateStepsFormElements();
}

/*****end form step*****/