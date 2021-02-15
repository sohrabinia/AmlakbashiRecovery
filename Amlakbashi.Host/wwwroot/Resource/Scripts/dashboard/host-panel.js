// ------------start--show&hide--banner-----------
function scrollTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}
// show-fast-edit 
$('.setting-icon').click(function () {
    showEditPanel($(this), $(this).attr('data-type'));
    scrollTop();
});
//find type banner
function showEditPanel(element, type) {
    const card = element.parent().parent().parent().parent().parent().find('.banner-details');
    var elementToShow;
    switch (type) {
        case 'single':
            elementToShow = card.find('.fast-edit');
            break;
        case 'hotel':
            elementToShow = card.find('.list-room-card');
            break;
        case 'complex':
            elementToShow = card.find('.room-type-list');
            break;
    }
    $('.card-banner').hide();
    elementToShow.show();
    $("#filter-banner").css('display', 'none');
    $("#pagenation-stylee").css('display', 'none');

    $('.exit1').click(function () {
        elementToShow.hide();
        $(".parent-box").css("display", "none");
        $(".container-item").css("display", "none");
        $('.card-banner').show();
        var clickedElmentOffset = $(this).parent().parent().parent().siblings(".card-banner").parent().offset().top;
        $('html, body').animate({
            scrollTop: clickedElmentOffset,
        }, 0);
        $("#filter-banner").css('display', 'flex');
        $("#pagenation-stylee").css('display', 'flex');
    });
}
$('.setting-icon-room').click(function () {
    showEditFastHotel($(this));
});
function showEditFastHotel(element) {
    var hotelDetail = element.closest('.room-card')
    var editPanelHotel = hotelDetail.next('.fast-edit-details')
    hotelDetail.hide(0, function () {
        editPanelHotel.show();
    });
}
$('.setting-icon-complex').click(function () {
    showEditFastComplex($(this));
});
function showEditFastComplex(element) {
    var complexDetail = element.closest('.room-card')
    var editPanelComplex = complexDetail.next('.fast-edit-details')
    $(".container-item").css("display", "none");
    complexDetail.hide(0, function () {
        editPanelComplex.show();

    });
}
$('.exit').click(function () {
    showBannerDtails($(this));
});
function showBannerDtails(element) {
    var editPanel = element.closest('.fast-edit-details')
    var hotel = editPanel.prev('.room-card')
    $(".container-item").css("display", "none");

    editPanel.hide(0, function () {
        hotel.show();
    });
}
//fliter-style
$(".filter-banner__item").click(function () {
    $(this).css({ 'box-shadow': '1px 2px 3px 0 rgba(0,0,0,.1)', 'border': '1px solid #ccc' });
});

//helper-rating
$(".js-helper-rating").click(function () {
    showInfoMessage('', `<div><strong> درحال حاضر شما هیچ امتیازی ندارید.</strong></div>
         نظر مهمانان گذشته شما قطعا در تصمیم گیری مهمان جدید و همچنین افزایش درآمد شما تاثیر گذار است.
         <span style="color:red;">از مهمانان خود بخواهید به شما امتیاز دهند.</span>`)
})
