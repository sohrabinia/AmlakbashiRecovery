var emailEditMessage = undefined;
var emailSent = false;

function showRegisterEmailForm(callback) {
    emailSent = false;
    emailEditMessage = showInfoMessage(
        'ثبت ایمیل', '', {
        contentUrl: '/user/popupregisteremail', buttons: [{
            title: 'انصراف',
            color: 'white',
            bgColor: '#EA4335'
        },
        {
            title: 'تایید',
            color: 'white',
            bgColor: '#34A853',
            onclick: doRegisterEmailAction
        }], autoClose: false, onContentReady: callback
    });
}

function doRegisterEmailAction() {
    if (emailSent) {
        profileConfirmEmail();
    }
    else {
        profileRegisterEmail();
    }
}

function profileRegisterEmail() {
    var email = $("#profileEmail").val();
    $.ajax({
        url: "/user/PopupRegisterEmail",
        type: "post",
        data: { email: email },
        success: function (ret) {
            if (ret.status == 1) {
                emailSent = true;
                $("#profileRegisterEmailForm").hide();
                $("#profileConfirmEmailForm").show();
            }
            else {
                alertify.error(ret.msg);
            }
        },
        error: function (ret) {
            alertify.error(ret.msg);
        }
    });
}

function profileConfirmEmail() {
    var emailCode = $("#profileEmailCode").val();
    if (emailCode == '') {
        alertify.error("لطفا کد تایید ارسال شده به ایمیل خود را وارد کنید");
        return;
    }
    $.ajax({
        url: "/user/PopupConfirmEmail",
        type: "post",
        data: { emailCode: emailCode },
        success: function (ret) {
            if (ret.status == 1) {
                alertify.success("ایمیل شما با موفقیت ثبت شد");
                emailSent = false;
                emailEditMessage.close();
                verifyEmail = true;
                $("#emailSpan").html($("#profileEmail").val());
            }
            else {
                alertify.error("کد وارد شده اشتباه است");
            }
        },
        error: function (ret) {
            alertify.error(ret.msg);
        }
    });
}