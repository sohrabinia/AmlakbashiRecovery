Dropzone.autoDiscover = false;
var haveChangedInput = false;

removeDefultInput();

$(".static-advertise__btn-item.static-advertise__btn-next").click(function () {
    if ($("#TypeID option:selected").val() != 0) {
        showNoYesMessage('تکمیل فرآیند',
            "آیا از تکمیل آگهی مطمئن هستید؟",
            function () {
                $("input#tabInput").val(-1);
                $('#AccForm').submit();
            })
    }
})

$(".changed-input-target").on("change", function () {
    haveChangedInput = true;
});

$("button.add, button.sub").on("click", function () {
    haveChangedInput = true;
});

function checkHaveChange(tab, id, level, type) {
    debugger;
    if (level < tab) {
        return;
    }
    if (haveChangedInput) {
        showNoYesMessage("", "آیا تغییرات انجام شده ذخیره شود؟", function () { nextFormSubStep(tab) },
            function () { chooseStep(tab, id, level, type) }, { noText: 'خیر', yesText: 'بله' });
    }
    else {
        chooseStep(tab, id, level, type);
    }
}

function chooseStep(tab, id, level, type) {
    if (tab == 1) {
        goBasicStep(id);
    }
    else if (tab == 2) {
        goGeneralStep(id, level);
    }
    else if (tab == 3) {
        goExtraStep(id, level);
    }
    else {
        goChildStep(id, level, type, tab);
    }
}

function goBasicStep(id) {
    window.location.href = "/accomodation/accbasicform?id=" + id;
}

function goGeneralStep(id, level) {
    if (level > 1 || level == undefined) {
        window.location.href = "/accomodation/accgeneralform?id=" + id;
    }
}

function goExtraStep(id, level) {
    if (level > 2 || level == undefined) {
        window.location.href = "/accomodation/accextraform?id=" + id;
    }
}

function goChildStep(parentId, level, type, tab) {
    if (level < 4) {
        return;
    }
    var id;
    if (tab > 4) {
        id = tab;
    }
    else if(tab == 0) {
        id = 0;
    }
    else {
        id = -1;
    }
    if (type == 0) {
        window.location.href = "/accomodation/acccomplexform?parentid=" + parentId + "&id=" + id;
    }
    else if (type == 1) {
        window.location.href = "/accomodation/acchotelform?parentid=" + parentId + "&id=" + id;
    }
}

// Admin
function checkHaveChangeAdmin(tab, id, type) {
    debugger;
    if (haveChangedInput) {
        showNoYesMessage("", "آیا تغییرات انجام شده ذخیره شود؟", function () { nextFormSubStep(tab) },
            function () { chooseAdminStep(tab, id, type) }, { noText: 'خیر', yesText: 'بله' });
    }
    else {
        chooseAdminStep(tab, id, type);
    }
}

function chooseAdminStep(tab, id, type) {
    if (tab == 1) {
        goAdminBasicStep(id);
    }
    else if (tab == 2) {
        goAdminGeneralStep(id);
    }
    else if (tab == 3) {
        goAdminExtraStep(id);
    }
    else if (tab == 5) {
        goAdminStatusStep(id);
    }
    else {
        goAdminChildStep(id, type, tab);
    }
}

function goAdminBasicStep(id) {
    window.location.href = "/accomodation/adminbasicform?id=" + id;
}

function goAdminGeneralStep(id) {
    window.location.href = "/accomodation/admingeneralform?id=" + id;
}

function goAdminExtraStep(id) {
    window.location.href = "/accomodation/adminextraform?id=" + id;
}

function goAdminChildStep(parentId, type, tab) {
    var id;
    if (tab > 5) {
        id = tab;
    }
    else {
        id = 0;
    }
    if (type == 0) {
        window.location.href = "/accomodation/admincomplexform?parentid=" + parentId + "&id=" + id;
    }
    else if (type == 1) {
        window.location.href = "/accomodation/adminhotelform?parentid=" + parentId + "&id=" + id;
    }
}

function goAdminStatusStep(id) {
    window.location.href = "/accomodation/adminstatusform?id=" + id;
}

function removeDefultInput() {
    $(".js-input").click(function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });
}

//jQuery(document).click(function () {
//    jQuery('.box-view-rooms').removeClass("acitve-box-rooms");
//});
//$(".js-btn-view-rooms").click(function (event) {
//    $(".box-view-rooms").toggleClass("acitve-box-rooms", 1000, "easeOutSine");
//    event.stopPropagation();
//});

$(".form-submit_button").click(function () {
    if (typeof uploadingFiles !== 'undefined' && uploadingFiles) {
        showErrorMessage("", "لطفا تا اتمام فرآیند آپلود عکس ها منتظر بمانید.");
        return;
    }
    $('#AccForm').submit();
})

function nextFormSubStep(tab) {
    if (typeof uploadingFiles !== 'undefined' && uploadingFiles) {
        showErrorMessage("", "لطفا تا اتمام فرآیند آپلود عکس ها منتظر بمانید.");
        return;
    }
    $("input#tabInput").val(tab);
    $('#AccForm').submit();
}

function forceSaveFormSubmit() {
    $('#forceSaveInput').val('true');
    $('#AccForm').submit();
}

$(".js-disable-error").click(function () {
    $(this).removeClass("input-validation-error");
})
