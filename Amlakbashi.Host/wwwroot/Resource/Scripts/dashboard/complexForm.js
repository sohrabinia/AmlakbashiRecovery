$(window).on("load", function () {
    if ($("#TypeID option:selected").val() == 0) {
        $(".addAccommodation_button-creat-new-room i.fa.fa-plus").css({ 'background': '#a9a9a9', 'cursor': 'not-allowed' , 'border' : 'none'});
        $(".static-advertise__btn-add p").css('cursor', 'not-allowed');
        $(".static-advertise__btn-item.static-advertise__btn-next").css({ 'background': '#a9a9a9', 'cursor': 'not-allowed' , 'border': 'none' });
    }
});
//$(".static-advertise__btn-add p").click(function () {
//    checkSubmitButton();
//})
$(".addAccommodation_button-creat-new-room").click(function () {
    checkSubmitButton();
})
function checkSubmitButton() {
    if ($("#TypeID option:selected").val() != 0) {
        submitButton();
    }
}
function submitButton(event) {
    $('#saveFormInput').val('true');
    $('#AccForm').submit();
    $(".static-advertise__btn-add p").off('click');
    $(".addAccommodation_button-creat-new-room").off('click'); 
    $(this).off(event);
};

//SCROLL CENTER 
$(function () {
    var parentActive = $("#form-step-button-container");
    var active = parentActive.find(".form-step-button-selected");
    scrollCenter(parentActive, active);
})
function scrollCenter(parentActive, active) {
    var activeWidth = active.width() / 2;

    var pos = active.position().left + activeWidth;
    var elpos = parentActive.scrollLeft();
    var elW = parentActive.width();
    pos = pos + elpos - elW / 2;
    parentActive.animate({
        scrollLeft: pos
    }, 300);
}