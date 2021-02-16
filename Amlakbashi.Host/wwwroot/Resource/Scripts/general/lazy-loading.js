//document.addEventListener("DOMContentLoaded", function () {

//});

$(document).ready(function () {
    findLazyImages();
    lazyLoad();
    document.addEventListener("scroll", lazyLoad);
    window.addEventListener("resize", lazyLoad);
    window.addEventListener("orientationchange", lazyLoad);

    document.addEventListener("scroll", asyncPartialLoad);
    window.addEventListener("resize", asyncPartialLoad);
    window.addEventListener("orientationchange", asyncPartialLoad);
    $(".initialPartialContents").each(function (index, item) {
        $(item).removeClass("initialPartialContents");
        $(item).empty();
        var url = $(item).data("url");
        var onLoad = $(item).data("onLoad");
        if (url && url.length > 0) {
            var onLoad = $(item).attr("data-onLoad");
            var onLoadfn;
            if (onLoad != undefined && onLoad != null) {
                onLoadfn = window[onLoad];
            }
            $(item).load(url, onLoadfn);
        }
    });
});

function elementInViewport(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (
      top < (window.pageYOffset + window.innerHeight) &&
      left < (window.pageXOffset + window.innerWidth) &&
      (top + height) > window.pageYOffset &&
      (left + width) > window.pageXOffset
    );
}

function asyncPartialLoad() {
    $(".partialContents").each(function (index, item) {
        if (elementInViewport(item)) {
            $(item).removeClass("partialContents");
            $(item).empty();
            var url = $(item).data("url");
            if (url && url.length > 0) {
                var onLoad = $(item).attr("data-onLoad");
                var onLoadfn;
                if (onLoad != undefined && onLoad != null) {
                    onLoadfn = window[onLoad];
                }
                $(item).load(url, onLoadfn);
            }
        }
    });
}

var $images;

function findLazyImages() {
    $images = $('img.lazy');
}

var lazy_load_active = false;

function lazyLoad() {
    if ($images != null && $images.length > 0) {
        if (!lazy_load_active) {
            lazy_load_active = true;
            $images.each(function () {
                lazyImage = this;
                if ((lazyImage.getBoundingClientRect().top <= window.innerHeight && lazyImage.getBoundingClientRect().bottom >= 0)) {
                    var attr = $(this).attr('data-src');
                    if (attr != undefined && attr != null) {
                        $(this).attr('src', attr);
                    }
                    attr = $(this).attr('data-srcset');
                    if (attr != undefined && attr != null) {
                        $(this).attr('srcset', attr);
                    }
                    $(this).removeClass('lazy');
                }
            });
            lazy_load_active = false;
        }
    }
}