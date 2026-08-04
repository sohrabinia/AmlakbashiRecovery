$('.home-page__next-slider').click(function () {
    nextSliderClick($(this));
});

$('.home-page__previous-slider').click(function () {
    previousSliderClick($(this));
});

function nextSliderClick($buttonElem)
{
    var elem = $buttonElem.next().get(0);
    var children = elem.children;
    var current_item_index = 0;
    for (var i = 0; i < children.length; i++) {
        var tableChild = children[i];
        var rect = tableChild.getBoundingClientRect();
        var parent_rect = $(".home-page__category-root").get(0).getBoundingClientRect();
        if (rect.left < parent_rect.left) {
            current_item_index = i;
            break;
        }
    }
    var current_item = $(children[current_item_index]);
    elem = $(elem);
    elem.scrollTo(current_item, 800, { margin: true });
}

function previousSliderClick($buttonElem) {
    var elem = $buttonElem.next().next().get(0);
    var children = elem.children;
    var current_item_index = children.length - 1;
    for (var i = children.length - 1; i >= 0; i--) {
        var tableChild = children[i];
        var rect = tableChild.getBoundingClientRect();
        var parent_rect = $(".home-page__category-root").get(0).getBoundingClientRect();
        if (rect.left + $(tableChild).width() > parent_rect.left + $(".home-page__category-root").width()) {
            current_item_index = i;
            break;
        }
    }
    if (current_item_index > 0 && current_item_index != children.length - 1) {
        var item_count = $(".home-page__category-root").width() / $(children[current_item_index]).width();
        item_count = Number((item_count).toFixed(0)) - 2;
        current_item_index += item_count;
    }
    var current_item = $(children[current_item_index]);
    elem = $(elem);
    elem.scrollTo(current_item, 800, { margin: true });
}

$(".home-page__category-container").scroll(updateLazyLoad);

function updateLazyLoad() {
    lazyLoad();
}

window.onload = function() {
    if (messageShowOnReady !== '')
    {
        showSuccessMessage('', messageShowOnReady);
    }
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
    //        }
    //        else if ($(this).hasClass('home-page__advertise-item-container')){
    //            $(this).find('.average-rating').barrating({
    //                theme: 'fontawesome-stars',
    //                readonly: true,
    //                initialRating: null
    //            });
    //        }
    //        else if ($(this).hasClass('home-page__slider-button')) {
    //            $(this).click(function () {
    //                if ($(this).hasClass('home-page__previous-slider')) {
    //                    previousSliderClick($(this));
    //                }
    //                else {
    //                    nextSliderClick($(this));
    //                }
    //            });
    //        }
    //        //else if ($(this).hasClass('home-page__category-item-container-large')) {
    //        //    var root = $(this).parent().parent();
    //        //    var next = root.next('.home-page__category');
    //        //    if (next != null)
    //        //    {
    //        //        next.css('display', 'inherit');
    //        //        next.addClass('partialContents');
    //        //    }
    //        //}
    //    });
    //}, true);



    //setInterval(function () {
    //    moveRight();
    //}, 3000);


    //var slideCount = $('#slider ul li').length;
    //var slideWidth = $('#slider ul li').width();
    //var slideHeight = $('#slider ul li').height();
    //var sliderUlWidth = slideCount * slideWidth;

    //$('#slider').css({ width: slideWidth, height: slideHeight });

    //$('#slider ul').css({ width: sliderUlWidth, marginLeft: -slideWidth });

    //$('#slider ul li:last-child').prependTo('#slider ul');

    //function moveLeft() {
    //    $('#slider ul').animate({
    //        left: +slideWidth
    //    }, 200, function () {
    //        $('#slider ul li:last-child').prependTo('#slider ul');
    //        $('#slider ul').css('left', '');
    //    });
    //};

    //function moveRight() {
    //    $('#slider ul').animate({
    //        left: -slideWidth
    //    }, 200, function () {
    //        $('#slider ul li:first-child').appendTo('#slider ul');
    //        $('#slider ul').css('left', '');
    //    });
    //};

    //$('a.control_prev').click(function () {
    //    moveLeft();
    //    return false;

    //});

    //$('a.control_next').click(function () {
    //    moveRight();
    //    return false;
    //});

};

//$("#slideshow > div:gt(0)").hide();

//setInterval(function () {
//    $('#slideshow > div:first')
//      .fadeOut(1500)
//      .next()
//      .fadeIn(1500)
//      .end()
//      .appendTo('#slideshow');
//}, 5000);
