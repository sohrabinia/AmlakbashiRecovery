var searchByRegionMsg;

function showSearchByRegion() {
    var setting = {};
    setting.contentUrl = '/category/searchbyregionpopup?province='+
            (typeof initialProvince == 'undefined' ? -1 : initialProvince)+
            (typeof initialCity == 'undefined' ? -1 : initialCity) +
            (typeof initialArea == 'undefined' ? -1 : initialArea);
    var buttons = [{
        title: 'بستن',
        color: '#242424',
        bgColor: '#eaeaea',
        onclick: function () {
            searchByRegionMsg.closePopup();
        }
    },
    {
        title: 'انتخاب',
        color: '#242424',
        bgColor: '#fdd835',
        onclick: function () {
            doGeneralSearchRegion();
            searchByRegionMsg.closePopup();
        }
    }];
    setting.autoClose = false;
    setting.buttons = buttons;
    setting.color = '#4485F2';
    searchByRegionMsg = showMessagePopup('لیست شهر ها', '', setting);
}

function selectMostViewRegion(url, title) {
    currentSelectedRegion = {
        href: url,
        title: typeof title == 'undefined' ? '' : title
    };
    if (typeof isPortalHomePage == 'undefined' ||
        !isPortalHomePage) {
        doHomePageSearch();
    }
    $(".home-page__search-input").val(currentSelectedRegion.title);
    if (isMobileDevice) {
        toggleRegionSearchPopup(false);
    }
    else {
        toggleSearchListBox(false);
    }
}

function doGeneralSearchRegion() {
    var $province = $("select[name='generalSearchProvince']");
    var $city = $("select[name='generalSearchCity']");
    var $area = $("select[name='generalSearchArea']");

    myajax("category/regionsearchtourl", "province=" + $province.val() +
        "&city=" + $city.val() + "&area=" + $area.val(), function (ret) {
            if (ret.status == 0) {
                showErrorMessage(ret.msg);
            }
            else {
                currentSelectedRegion = {
                    href: ret.url,
                    title: ret.title
                };
                if (typeof isPortalHomePage == 'undefined' ||
                    !isPortalHomePage) {
                    doHomePageSearch();
                }
                $(".home-page__search-input").val(currentSelectedRegion.title);
                if (isMobileDevice) {
                    toggleRegionSearchPopup(false);
                }
                else {
                    toggleSearchListBox(false);
                }
            }
        }, false);
}