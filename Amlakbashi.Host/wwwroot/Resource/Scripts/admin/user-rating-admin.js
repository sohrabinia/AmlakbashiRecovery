/******rating*******/

function add_comment(user_id) {

    if (user_id > 0) {

        if ($(".js-cm-text").val() == "") {
            alertify.error("لطفا نظر خود را وارد کنید");
            $(".js-cm-text").focus();
            return false;
        }

        $('#addCommentForm').submit();
    }
    else {
        alertify.error("لطفا ابتدا وارد سایت شوید");
    }
}

function onAddComment(data) {
    if (data.status == 1) {
        alertify.success("نظر شما با موفقیت ثبت شد. پس از تایید کارشناس بر روی سایت قرار میگیرد");
    }
    else {
        alertify.error(data.val);
    }
}

$(document).ready(function () {
    bar_rating();
});

function bar_rating() {
    $('.score-rating').barrating({
        theme: 'fontawesome-stars',
        readonly: false,
        deselectable: false,
        onSelect: function (value, text, event) {
            if (typeof (event) !== 'undefined') {
                // rating was selected by a user
                var report_elem = $($(this)[0].$elem);
                if ($('#UserId').val() > 0) {
                    myajax("Advertise/AddScoreAdmin", "advertiseID=" + $("#AdvertiseID").val()
                        + "&ReportID=" + report_elem.attr("report_id")
                        + "&value=" + value
                        + "&user_id=" + $('#UserId').val(), function (ret) {
                            if (ret.status == 1) {
                                //alertify.success("امتیاز شما با موفقیت ثبت شد");
                            }
                            else {
                                alertify.error(ret.val);
                            }
                        }, false);
                }
            } else {
                alertify.error("خطایی رخ داد، لطفا بعدا امتحان کنید.");
            }
        }
    });
}

function ratingDialog(reserve_id, onClose) {
    //var advertise = guestAdvertiseRatingArray[index];
    //settings: color, buttons, onClose, contentUrl, onContentReady, onOpen, disableKeyEvents
    var url = '/reserve/ratingdialog?reserveid=' + reserve_id;
    showMessagePopup('نظرسنجی', '', {
        contentUrl: url,
        onContentReady: bar_rating,
        onClose: onClose,
    });
    //$.confirm({
    //    title: 'نظرسنجی',
    //    content: 'لطفا با امتیاز دهی و نظر درباره اقامتگاه دیگران را راهنمایی کنید.' +
    //        '<div id="js-rating-container"></div>',
    //    type: 'blue',
    //    typeAnimated: true,
    //    columnClass: 'xlarge',
    //    buttons: {
    //        بستن: function () {
    //            if (onClose != undefined && onClose != null) {
    //                onClose();
    //            }
    //        }
    //    },
    //    onContentReady: function () {
    //        $('#js-rating-container').load('/reserve/ratingdialog?reserveid=' +
    //            reserve_id, bar_rating);
    //    }
    //});
}

/******end rating*******/