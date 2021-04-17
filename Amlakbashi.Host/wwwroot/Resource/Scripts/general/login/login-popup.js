//---------intltelInput------------
var input = document.querySelector("#mobile")
var intl = window.intlTelInput(input, {
    preferredCountries: ["ir"],
    utilsScript: "/file/getjs?src=/resource/scripts/general/login/utils.js"
});

//------------------------
//$(".edit-number-login").click(function () {
//    restart_login();
//});
function scrollTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}
$(".exit__icon").click(function () {
    $(".login__bg").fadeOut(500);
    $(".login__container").css({ 'transition': '0.8s', 'display': 'none' });
    scrollTop();
    onLoginFinish();
});
//------------------------------------
var can_send_login_message = true;
$(".login__bg").click(function () {
    $('#login-box-details').toggleClass('animate');
});
function toggle_login() {
    if ($(".login__container").css("display") == "none") {
        if (!checked_mobile_current)
            restart_login();
        $(".login__bg").show();
        toggle_login_container(true, function () {
            $(".login__container").css("display", "flex");
            $("input#mobile").focus();
        });
        onLoginStart();
    }
    //$(".login__bg").fadeOut(500);
    //toggle_login_container(false);

}
var pressKey = $(document).keypress(function (event) {

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if ($(".login__container").length == 0 ||
        $(".login__container").css("display") == "none") {
        return;
    }
    if (keycode == '13') {
        if ($("#login_form").css("display") != "none" && $(".input-code").css("display") == "none") {
            login();
        }
        else if ($("#login_form_email").css("display") != "none") {
            login("email");
        }
        else if ($("#verification_form").css("display") != "none") {
            login_verification();
        }
    }

});
function toggle_login_container(direction, onDone) {
    if (direction) {
        $(".login__container").css("opacity", 0);
        $(".login__container").css("display", "flex");
        $(".login__container").animate({
            opacity: 1
        }, 200, onDone);
    }
    else {
        $(".login__container").animate({
            opacity: 0
        }, 200, function () {
            $(".login__container").css("display", "none");
            if (onDone != undefined && onDone != null) {
                onDone();
            }
        });
    }
}
$("#login_form").find("input").keyup(function () {
    if ($(this).val() > 0) {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
});
$("#email").keyup(function () {
    if ($(this).val() != null && $(this).val() != "") {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
});

function SignUp_button() {
    if ($("#lname").val() != null && $("#fname").val() != null && $("#lname").val() != "" && $("#fname").val() != "") {
        $(".login__box-button").css('background', '#fdd835');
    } else {
        $(".login__box-button").css('background', '#e2e2e2');
    }
}
var mobileCurrent
var login_in_progress = false;
var checked_mobile_current = false;

function login(step) {
    if (login_in_progress) {
        return;
    }
    if (!check_login_mobile())
        return;
    step = step !== "email" ? "mobile" : "email";
    toggle_login_container(false);
    var email = $('#email').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    login_in_progress = true;
    myajax("user/popuplogin", "mobile=" + mobile + "&email=" + email +
        "&step=" + step + "&send_verification=" + can_send_login_message,
        function (ret) {
            login_in_progress = false;
            if (ret.isNumberForIran == true) {
                isNumberForIran = ret.isNumberForIran;
            }
            if (ret.status == 0) {
                //$("#login-error-container").html(ret.msg);
                alertify.error(ret.msg);
            }
            else if (ret.status == 1) {
                checked_mobile_current = true;
                mobileCurrent = function () {
                    $(".input-code").show();
                    $("#resend_form").show();
                    $("#check-number").show();
                    $(".button-get-number").css('display', 'none');
                    $(".iti.iti--allow-dropdown").css('display', 'none');
                    $(".login__box-button").css('background', '#e2e2e2');
                    $(".icon-back").css('display', 'block');
                    $("#login_form").find('.login__please-enter-mobile').html("");
                    var mobileNumber = mobile.replace("00989", "09");
                    $("#login_form").find('.login__please-enter-mobile').append(`<div class="login__please-enter-mobile">کد تایید به شماره موبایل ${mobileNumber} ارسال شد. </div><div>برای ورود کد تایید را وارد نمایید.</div>`);
                    //$(".iti").css('margin', '15px auto 0 auto')
                    //$("#mobile").prop("disabled", true).css({ 'cursor': 'no-drop', 'opacity': '0.3' });
                    //$(".iti__flag-container").prop("disabled", true).css({ 'cursor': 'no-drop', 'opacity': '0.3' });
                    $("div#selectRoot").css('margin-top', '-23px');
                    $("#login_form .login__please-enter-mobile").css('margin', '35px auto 8px auto');
                    //$("#mobile_label").html(mobileNumber);
                    if (can_send_login_message) {
                        can_send_login_message = false;
                        startCountDown($("#count_down_timer")[0], function () {
                            can_send_login_message = true;
                            $("#count_down_timer").html("");
                            $("#resend_button").attr("onclick", "resend_login_sms()");
                            $("#resend_button").html("درخواست ارسال مجدد").css({ 'cursor': 'pointer', 'color': '#242424', 'font': '13px Miransans' });
                            $("#resend_button").click(function () {
                                $("#resend_button").css({ 'cursor': 'auto', 'color': '#ccc' });
                            })
                        });
                    }
                    $("#code").on('keypress', function (e) {
                        if (e.which == 13) {
                            login_success(ret.mobile, ret.isNew);
                        }
                    });
                    $("#success__code").click(function () {
                        login_success(ret.mobile, ret.isNew);
                    });
                    setTimeout(function () { $("#code").focus() }, 1000);
                }
                mobileCurrent();
                console.log(mobileCurrent);
            }
            else if (ret.status == 2) {
                toggle_login_container(false);
                showErrorMessage('مسدود',
                    'امکان ورود به سایت برای شما مسدود شده است, جهت فعالسازی با پشتیبانی تماس بگیرید: ' +
                    '<a href="/contact">تماس با پشتیبانی</a>');
            }
            else if (ret.status == 3) {
                //login_success_email(ret.email);
                //$(".login_form").hide();
                //$("#code").val(ret.code);
                //show_verification_form();
                $(".login_form").hide();
                $('#loginPasswordForm').show();
            }
            //else if (ret.status == 4) {
            //    $(".login_form").hide();
            //    //$("#login_form_email").show();
            //    //$(".login__box-button").css('background', '#e2e2e2');
            //    //setTimeout(function () { $("#email").focus() }, 1300);
            //    $('#loginPasswordForm').show();
            //}
            toggle_login_container(true, function () {
                $(".login__container").css("display", "flex");
                if (ret.status == 0) {
                    $('#login-error-container').show();
                }
            });
        });
}

function showCodeForm() {
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    myajax("user/popupsendsmsagain", "mobile=" + mobile,
        function (ret) {
            login_in_progress = false;
            if (ret.status == 1) {
                $("#login_form").show();
                $('#loginPasswordForm').hide();
                $(".input-code").show();
                $("#resend_form").show();
                $("#check-number").show();
                $(".button-get-number").css('display', 'none');
                $(".iti.iti--allow-dropdown").css('display', 'none');
                $(".login__box-button").css('background', '#e2e2e2');
                $(".icon-back").css('display', 'block');
                $("#login_form").find('.login__please-enter-mobile').html("");
                var mobileNumber = mobile.replace("00989", "09");
                $("#login_form").find('.login__please-enter-mobile').append(`<div class="login__please-enter-mobile">کد تایید به شماره موبایل ${mobileNumber} ارسال شد. </div><div>برای ورود کد تایید را وارد نمایید.</div>`);
                $("div#selectRoot").css('margin-top', '-23px');
                $("#login_form .login__please-enter-mobile").css('margin', '35px auto 8px auto');
                if (can_send_login_message) {
                    can_send_login_message = false;
                    startCountDown($("#count_down_timer")[0], function () {
                        can_send_login_message = true;
                        $("#count_down_timer").html("");
                        $("#resend_button").attr("onclick", "resend_login_sms()");
                        $("#resend_button").html("درخواست ارسال مجدد").css({ 'cursor': 'pointer', 'color': '#242424', 'font': '13px Miransans' });
                        $("#resend_button").click(function () {
                            $("#resend_button").css({ 'cursor': 'auto', 'color': '#ccc' });
                        })
                    });
                }
                $("#success__code").click(function () {
                    loginForgotSuccess();
                });
                setTimeout(function () { $("#code").focus() }, 1000);
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function saveNewPass() {
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    var pass = $("#forgotPass").val();
    var confPass = $("#forgotConfirmPass").val();
    myajax("user/savenewpass", "mobile=" + mobile + "&code=" + code + "&password=" + pass + "&confirmPassword=" + confPass,
        function (ret) {
            login_in_progress = false;
            if (ret.status == 1) {
                toggle_login();
                verification_success();
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function loginForgotSuccess() {
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    myajax("user/popupverifycode", "mobile=" + mobile + "&code=" + code, function (ret) {
        if (ret.correct) {
            $('.login_form').hide();
            $("#loginForgotPasswordForm").show();
        } else {
            alertify.error('کد وارد شده اشتباه است');
        }
    });
}

function loginPassword() {
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    var pass = $("#passLogin").val();
    myajax("user/popuploginpass", "mobile=" + mobile + "&password=" + pass,
        function (ret) {
            login_in_progress = false;
            if (ret.status == 1) {
                toggle_login();
                verification_success();
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function showForgotPasswordForm() {
    $('#loginPasswordForm').hide();
    $(".login_form").show();
}

function login_verification() {
    if (login_in_progress) {
        return;
    }
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    var fname = $("#fname").val();
    var lname = $("#lname").val();
    var pass = $("#pass").val();
    var passConfirm = $("#confirmPass").val();
    var presentorCode = $('#presentorcode').val();
    login_in_progress = true;
    myajax("user/popuploginverification", "mobile=" + mobile + "&code=" +
        code + "&fname=" + fname + "&lname=" + lname + "&password=" + pass +
        "&confirmPassword=" + passConfirm + "&presentorcode=" + presentorCode,
        function (ret) {
            login_in_progress = false;
            if (ret.status == 1) {
                toggle_login();
                verification_success();
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function registerEmail() {
    var email = $("#email").val();
    myajax("user/PopupRegisterEmail", "email=" + email,
        function (ret) {
            if (ret.status == 1) {
                $('.login_form').hide();
                $("#confirmEmailForm").show();
            }
            else {
                alertify.error(ret.msg);
            }
        });
}

function confirmEmail() {
    var emailCode = $("#emailCode").val();
    myajax("user/PopupConfirmEmail", "emailcode=" + emailCode,
        function (ret) {
            if (ret.status == 1) {
                //toggle_login();
                $('.login__container').hide();
                $('.login__bg').hide();
                onLoginFinish();
                alertify.success("ایمیل شما با موفقیت ثبت شد");
                verifyEmail = true;
                //if (typeof on_login !== "undefined") {
                //    on_login();
                //}
            }
            else {
                alertify.error("کد وارد شده اشتباه است");
            }
        });
}

function resend_login_sms() {
    if (login_in_progress) {
        return;
    }
    login_in_progress = true;
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    myajax("user/popupsendsmsagain", "mobile=" + mobile, function (ret) {
        login_in_progress = false;
        if (ret.status == 1) {
            $("#resend_button").removeAttr("onclick");
            $("#resend_button").append('<span id="count_down_timer"></span>');
            if (can_send_login_message) {
                can_send_login_message = false;
                startCountDown($("#count_down_timer")[0], function () {
                    can_send_login_message = true;
                    $("#count_down_timer").html("");
                    $("#resend_button").attr("onclick", "resend_login_sms()");
                    $("#resend_button").html("درخواست ارسال مجدد ").css({ 'cursor': 'pointer', 'color': '#242424' });
                });
            }
        }
    });
}
$(".icon-back").click(function () {
    can_send_login_message = true;
    $(".input-code").hide();
    $("#check-number").hide();
    $(".icon-back").css('display', 'none');
    $(".button-get-number").css('display', 'block');
    $(".button-get-number p").css('background', '#fdd835');
    $(".iti.iti--allow-dropdown").css('display', 'block');
    $("div#selectRoot").css('margin-top', '0');
    $("#login_form").find('.login__please-enter-mobile').html("");
    $("#login_form").find('.login__please-enter-mobile').append('<div class="login__please-enter-mobile">برای ورود یا ثبت نام، شماره موبایل خود را وارد کنید</div>');
    //$("#mobile").prop("disabled", false).css({ 'cursor': 'pointer', 'opacity': '1' });
    //$(".iti__flag-container").prop("disabled", false).css({ 'cursor': 'pointer', 'opacity': '1' });
});
function resend_login_email(email) {
    if (login_in_progress) {
        return;
    }
    login_in_progress = true;
    myajax("user/popupresendemail", "email=" + email, function (ret) {
        login_in_progress = false;
        showSuccessMessage('ایمیل ارسال شد', 'ایمیل تایید دوباره ارسال شد. لطفا مجددا ایمیل خود را بررسی کنید',
            {
                onClose: function () {
                    $("#email_resend_button").removeAttr("onclick");
                    $("#email_resend_button").append('<span id="email_count_down_timer"></span>');
                    if (can_send_login_message) {
                        can_send_login_message = false;
                        startCountDown($("#email_count_down_timer")[0], function () {
                            can_send_login_message = true;
                            $("#email_count_down_timer").html("");
                            $("#email_resend_button").attr("onclick", "resend_login_email('" + email + "')");
                            $("#email_resend_button").html("دریافت مجدد ایمیل فعال سازی");
                            $("div#resend_form_email").css('background', '#FDD835');
                            $("div#resend_form_email .login__resend-button").css('color', '#242424');
                            $("#resend_form_email").click(function () {
                                $("div#resend_form_email").css('background', '#f3f3f3');
                                $("div#resend_form_email .login__resend-button").css('color', '#ccc');
                            });
                        });
                    }
                }
            });
    });
}
function restart_login() {
    $(".login_form").hide();
    $("#login_form").show();

}
function login_success(mobile, isNew) {
    //$("#fname").val(fname == null ? "" : fname);
    //$("#lname").val(lname == null ? "" : lname);
    var code = $('#code').val();
    var number = intl.getNumber();
    var mobile = number.replace("+", "00");
    $.ajax({
        url: "/user/popupverifycode",
        type: "post",
        data: {
            mobile: mobile,
            code: code
        },
        success: function (ret) {
            if (ret.correct) {
                $(".login_form").hide();
                show_verification_form(ret.fname, ret.lname, isNew);
            } else {
                alertify.error('کد وارد شده اشتباه است');
            };
        },
        error: function (jqXhr, textStatus, errorMessage) {
        }
    });
}

function login_success_email(email, onDone) {
    show_email_verification_form(email, onDone);
}

function show_verification_form(fname, lname, isNew) {
    if (isNew == undefined) {
        isNew = false;
    }
    if (fname != undefined) {
        $("#fname").val(fname);
    }
    if (lname != undefined) {
        $("#lname").val(lname);
    }
    $(".login__box-button").css('background', '#e2e2e2');
    //checked_mobile_current = true;
    //$("#login_form").css("display", "none");
    //$("#verification_form").css("display", "unset");
    $(".icon-back").css('display', 'none');
    $(".input-code").hide();
    $("#verification_form").show();
    $("#resend_form").hide();
    $("#check-number").hide()
    if (isNew) {
        $('#js-presentor-code-container').show();
        $('#presentorCodeContainer').show();
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
}

function show_email_verification_form(email, onDone) {
    checked_mobile_current = true;
    //$("#login_form").css("display", "none");
    //$("#verification_form").css("display", "unset");
    $("#login_form_email").hide();
    $("#email_verification_form").show();
    $("#resend_form_email").show();
    $("#email_label").html(email).css("color", "#f00");
    if (can_send_login_message) {
        can_send_login_message = false;
        startCountDown($("#email_count_down_timer")[0], function () {
            can_send_login_message = true;
            $("#email_count_down_timer").html("");
            $("#email_resend_button").attr("onclick", "resend_login_email('" + email + "')");
            $("#email_resend_button").html("دریافت مجدد ایمیل فعال سازی");
            $("div#resend_form_email").css('background', '#FDD835');
            $("div#resend_form_email .login__resend-button").css('color', '#242424');
            $("#resend_form_email").click(function () {
                $("div#resend_form_email").css('background', '#f3f3f3');
                $("div#resend_form_email .login__resend-button").css('color', '#ccc');
            });
        });
    }
    if (onDone != undefined && onDone != null) {
        onDone();
    }
}
$("#edit_mail_button").click(function () {
    $("#email_verification_form").hide();
    $("#resend_form_email").hide();
    $(".login_form").hide();
    $("#login_form_email").show();
    $("#email").focus;
})
function verification_success() {
    alertify.success("با موفقیت وارد حساب خود شدید");
    $(".master_header-account").attr("href", "/dashboard");
    $(".master_header-account").removeAttr("onclick");
    $(".master_header-account").find("span").html("حساب من");
    $('.login__container').hide();
    $('.login__bg').hide();
    onLoginFinish();
    if (typeof on_login !== "undefined") {
        on_login();
    }
}

if ($("#MobileLoginVerification").length) {
    alertify.success("کد فعال سازی 4 رقمی به تلفن همراه شما پیامک شد");
}

function onMobileKeyPress(e) {
    if (e.which == 32)
        return false;
}
function check_login_mobile() {
    var number = intl.getNumber();
    console.log('number coming from intl: ' + number);
    var mobile = number.replace("+", "00");
    if (mobile == "") {
        alertify.error("لطفا تلفن همراه خود را وارد کنید");
        $("#mobile").focus();
        return false;
    }
    if (!validateMobile(mobile.toString())) {

        alertify.error("لطفا تلفن همراه خود را درست وارد نمایید");
        $("#mobile").focus();
        return false;
    }
    return true;
}

setTimeout(function () {
    $("#resendCode").css("color", "black");
    $("#resendCode").css("cursor", "pointer");
}, 1 * 5 * 1000);

function startCountDown(elem, onDone) {
    var minutes = 2;
    var seconds = 0;
    var x = setInterval(function () {
        if (minutes < 0) {
            clearInterval(x);
            if (onDone != undefined) {
                onDone();
            }
        }
        var formattedMinutes = ("0" + minutes).slice(-2);
        var formattedSeconds = ("0" + seconds).slice(-2);
        $(elem).html(formattedMinutes + ":" + formattedSeconds);
        seconds -= 1;
        if (seconds < 0) {
            seconds = 59;
            minutes -= 1;
        }
    }, 1000);
}
$(function () {
    $('#mobile').keyup(function (e) {
        var ctrlKey = 67, vKey = 86;
        if (e.keyCode != ctrlKey && e.keyCode != vKey) {
            $('#mobile').val(persianToEnglish($(this).val()));
        }
    });
});
function persianToEnglish(input) {
    var inputstring = input;
    var persian = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]
    var english = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
    for (var i = 0; i < 10; i++) {
        inputstring = inputstring.toString().replace(persian[i], english[i]);
    }

    return inputstring;
}

function onLoginStart() {
    msgPopupKeyPressEnabled = false;
}

function onLoginFinish() {
    msgPopupKeyPressEnabled = true;
}
