function showMoreOptionBox(callerButton, advertiseId) {
    loadCollapse(callerButton, $(callerButton).next()[0], '/advertise/GetAdvertiseIndexDetails?advertiseId=' + advertiseId);
}

$(".box-filter .submit-btn").click(function (event) {
    $('#more_filter_form [name="Id"]').val($('.box-filter .bar-filter [name="AdvertiseId"').val());
    $('#more_filter_form [name="UserId"]').val($('.box-filter .bar-filter [name="HostUserId"').val());
    $('#more_filter_form [name="Status"]').val($('.box-filter .bar-filter [name="Status"').val());
    $('#more_filter_form').submit();
});

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

//function SortDown($id, obj) {
//    if (confirm("آیا از پرت کردن این آگهی اطمینان دارید؟")) {
//        myajax("Advertise/SortDown", "id=" + $id, function (ret) {
//            if (ret.status == 1) {
//                $(obj).parent().parent().remove();
//                alert("آگهی مورد نظر پرت شد .");
//            }
//        });
//    }
//}

function SuspenAdvertise($id, obj) {
    showConfirm('آیا از تعلیق این آگهی اطمینان دارید؟', function () {
        sendGetAjax("/accomodation/suspend", "id=" + $id, function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert("آگهی مورد نظر تعلیق شد");
            }
        })
    });
}

function NotVerifyAdvertise($id, obj) {
    showConfirm('آیا از عدم تایید این آگهی اطمینان دارید؟', function () {
        sendGetAjax("/Advertise/NotVerify", "id=" + $id, function (ret) {
            if (ret.status == 1) {
                $(obj).remove();
                successAlert("آگهی مورد نظر به تایید نشده ها پیوست");
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