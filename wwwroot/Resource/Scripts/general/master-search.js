setTimeout(function(){ clearSearch(true) }, 500);
function doHomePageSearch() {
    if (currentSelectedRegion != undefined &&
        currentSelectedRegion.href == undefined) {
        currentSelectedRegion = undefined;
    }
    var regionHref = currentSelectedRegion == undefined ? "/ایران" : currentSelectedRegion.href;
    var fromDate = empty_range_from == undefined ? null : empty_range_from;
    var toDate = empty_range_to == undefined ? null : empty_range_to;
    var guestCount = currentGuestCount > 0 ? currentGuestCount.toString() : null;

    if (guestCount > 10) {
        guestCount = 11;
    }

    var firstQueryAdded = false;

    if (fromDate != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "empty_range_from=" + fromDate;
        firstQueryAdded = true;
    }
    if (toDate != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "empty_range_to=" + toDate;
        firstQueryAdded = true;
    }
    if (guestCount != null) {
        if (firstQueryAdded) {
            regionHref += "&";
        }
        else {
            regionHref += "?";
        }
        regionHref += "capacity=" + guestCount;
        firstQueryAdded = true;
    }
    window.open(regionHref, "_self");
}


$(".home-page__search-box").click(function () {
    //clearSearch();
    if (isMobileDevice) {
        toggleRegionSearchPopup(true);
        $('.home-page__search-input').focus();
    }
    else {
        toggleSearchListBox(true);
    }
    //search_catrgories();
});

function toggleRegionSearchPopup(toggle) {
    if (toggle) {
        $('#js-search-region-popup').show();
    }
    else {
        $('#js-search-region-popup').hide();
    }
}

$(document).click(function (e) {
    if ($(e.target).closest('.home-page__search-box').length === 0) {
        toggleSearchListBox(false);
    }
});

function toggleSearchListBox(toggle) {
    if (toggle) {
        $(".home-page__search-list-box").show();
        $(".home-page__search-box").css("border-bottom-right-radius", "0");
        $(".home-page__search-box").css("border-bottom-left-radius", "0");
    }
    else {
        $(".home-page__search-list-box:not('.js-dont-close')").hide();
        $(".home-page__search-box").css("border-bottom-right-radius", "");
        $(".home-page__search-box").css("border-bottom-left-radius", "");
    }
}

if ($(".home-page__search-input").val() != '') {
    toggleSearchHolder(false);
}

function openFirstSearchResult() {
    var href = $(".home-page__search-list-result-container").find("a:first").attr("href");
    if (href != null)
        window.location.href = href;
}

var currentSelectedRegion = undefined;
var currentGuestCount = 0;

function selectSearchRegion(elem) {
    if (typeof elem == 'undefined' || elem == null) {
        currentSelectedRegion = undefined;
    }
    else {
        currentSelectedRegion = {
            href: $(elem).attr('data-href'),
            title: $(elem).attr('data-title')
        };
        $(".home-page__search-input").val(currentSelectedRegion.title);
        toggleSearchListBox(false);
        doHomePageSearch();
    }
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
}

function search_catrgories(e) {
    var $input;
    if (isMobileDevice) {
        $input = $('.home-page__search-box-popup').find('.home-page__search-input');
    }
    else {
        $input = $(".home-page__search-input:visible");
    }
    var search_string = $input.val();

    if ((e != null && e.keyCode === 13)) {
        if (isNaN(search_string) === false) {
            searchByAdvertiseId(search_string);
            return;
        }
        var target = $(".home-page__search-list-result-container").find("div:first");
        if (target.length > 0) {
            selectSearchRegion(target[0]);
        }
        return;
    }

    toggleSearchHolder(false);
    if (typeof search_string == 'undefined' || search_string == null || search_string == '') {
        $(".home-page__search-list-result-container").html('');
        return;
    }

    if (isNaN(search_string) === false) {
        let searchContent = '<div onclick="searchByAdvertiseId(' + search_string +
            ')" style="color:#242424;font:13px Miransans;padding:5px 10px;display: flex;align-items: center;"><i class="fa fa-search"></i><span>کد آگهی '
            + search_string + '</span></div>';
        $(".home-page__search-list-result-container").html(searchContent);
        return;
    }

    if (/^[A-Za-z]*$/.test(search_string)) {
        $(".home-page__search-input").val(search_string.replace(/[A-Za-z]/g, ""));
        alertify.error("لطفا فارسی تایپ کنید");
        return;
    }
    var url = "/Category/SearchCategory?search_string="
        + search_string +
        "&province=" + (typeof initialProvince == 'undefined' ? '-1' : initialProvince) +
        "&city=" + (typeof initialCity == 'undefined' ? '-1' : initialCity) +
        "&area=" + (typeof initialArea == 'undefined' ? '-1' : initialArea);
    $.get(url, function (data) {
        $(".home-page__search-list-result-container").html(data);
    });
    var target = $(".home-page__search-list-result-container").find("div:first");
    if (target.length > 0) {
        currentSelectedRegion = {
            href: $(target).attr('data-href'),
            title: $(target).attr('data-title')
        };
    }
}

function searchByAdvertiseId(id) {
    if (isNaN(id)) {
        return;
    }
    myajax('accomodation/getaccurlbyid', 'id=' + id, function (ret) {
        if (ret.status == 0) {
            showErrorMessage('خطا', 'کد آگهی یافت نشد. لطفا کد وارد شده را بررسی کنید.');
            return;
        }
        window.open(ret.url, '_self');
    });
}

function clearSearch(dontFocus) {
    $(".home-page__search-input").val("");
    if (!dontFocus)
        $(".home-page__search-input").focus();
    toggleSearchHolder(true);
    selectSearchRegion(null);
}

var search_holder_shown = true;

function toggleSearchHolder(toggle) {
    if (toggle) {
        $('#search_holder_root').show();
    }
    else {
        $('#search_holder_root').hide();
    }
    search_holder_shown = toggle;
}

$("span.holder").click(function () {
    $(".home-page__search-input").focus();
});

$(".home-page__search-input").click(search_catrgories);

$('.home-page__search-container').find('input').each(function () {
    if ($(this)[0].parentNode.tagName.toString().toLowerCase() == 'div') {
        $(this)[0].onfocus = function () {
            $(this).parent().addClass('focused-input');
        }
        $(this)[0].onblur = function () {
            $(this).parent().removeClass('focused-input');
        }
    }
});

for (i = 0; i < $('.home-page__search-container').length; i++) {
    // you can omit the 'if' if you want to style the parent node regardless of its
    // element type

}
var isMobileDevice = $('.body').width() < 681;

//if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
//    isMobileDevice = true;
//}

function searchTag(urlTitle) {
    console.log('test');
    window.location.href = `/tag/${urlTitle}`;
}