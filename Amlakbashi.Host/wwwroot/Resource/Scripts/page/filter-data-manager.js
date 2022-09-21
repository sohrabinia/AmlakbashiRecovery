var filterInputData = {};

initializeFilterData();

function initializeFilterData() {
    filterInputData.page = $('#advertise-filter-form input[name="page"]').attr('value');
    var tempFilter = $('#advertise-filter-form input[name="discount_homes"]').attr('value');
    if (tempFilter == "True") {
        filterInputData.discountHomes = true;
    }
    else {
        filterInputData.discountHomes = false;
    }
    tempFilter = $('#advertise-filter-form input[name="instant_reserve"]').attr('value');
    if (tempFilter == "True") {
        filterInputData.instantReserve = true;
    }
    else {
        filterInputData.instantReserve = false;
    }
    tempFilter = $('#advertise-filter-form input[name="today_empty_homes"]').attr('value');
    if (tempFilter == "True") {
        filterInputData.todayEmptyHomes = true;
    }
    else {
        filterInputData.todayEmptyHomes = false;
    }
    tempFilter = $('#advertise-filter-form input[name="norouz_special"]').attr('value');
    if (tempFilter == 'True') {
        filterInputData.norouzSpecial = true;
    }
    else {
        filterInputData.norouzSpecial = false;
    }
    tempFilter = $('#advertise-filter-form input[name="hygieneProtocol"]').attr('value');
    if (tempFilter == 'True') {
        filterInputData.hygieneProtocol = true;
    }
    else {
        filterInputData.hygieneProtocol = false;
    }
    debugger;
    filterInputData.countryDirection = $('#advertise-filter-form input[name="country_direction"]').attr('value');
    filterInputData.province = $('#advertise-filter-form input[name="Province"]').attr('value');
    filterInputData.city = $('#advertise-filter-form input[name="City"]').attr('value');
    filterInputData.area = $('#advertise-filter-form input[name="Area"]').attr('value');
    filterInputData.capacity = $('#js-capacity-input-filter').attr('value');
    filterInputData.emptyRangeFrom = $('#from_date_date_picker-filter').attr('value');
    filterInputData.emptyRangeTo = $('#to_date_date_picker-filter').attr('value');
    filterInputData.accType = $('#js-acctype-input-filter').attr('value');
    filterInputData.t = $('#js-t-input-filter').attr('value');
    filterInputData.position = $('#js-position-input-filter').attr('value');
    filterInputData.priceMin = $('#js-pricemin-input-filter').attr('value');
    filterInputData.priceMax = $('#js-pricemax-input-filter').attr('value');
    filterInputData.roomList = $('#js-roomlist-input-filter').attr('value');
    filterInputData.parking = $('#js-parking-input-filter').attr('value');
    filterInputData.pool = $('#js-pool-input-filter').attr('value');
    filterInputData.elevator = $('#js-elevator-input-filter').attr('value');
    filterInputData.priceType = $('#js-pricetype-input-filter').attr('value');
    filterInputData.perWC = $('#js-perwc-input-filter').attr('value');
    filterInputData.euWC = $('#js-euwc-input-filter').attr('value');
    filterInputData.wifi = $('#js-wifi-input-filter').attr('value');
    filterInputData.washingMachine = $('#js-washingmachine-input-filter').attr('value');
    filterInputData.jacuzzi = $('#js-jacuzzi-input-filter').attr('value');
    filterInputData.poolTable = $('#js-pooltable-input-filter').attr('value');
    filterInputData.foosball = $('#js-foosball-input-filter').attr('value');
    filterInputData.teaMaker = $('#js-teamaker-input-filter').attr('value');
    filterInputData.filming = $('#js-filming-input-filter').attr('value');
    filterInputData.exclusive = $('#js-exclusive-input-filter').attr('value');
    filterInputData.pets = $('#js-pets-input-filter').attr('value');
    filterInputData.party = $('#js-party-input-filter').attr('value');
    filterInputData.smoking = $('#js-smoking-input-filter').attr('value');
    filterInputData.sort = $('#js-sort-order-input-filter').attr('value');
    filterInputData.ajax = $('#js-ajax-filter').attr('value');
    if (filterInputData.sort != '0') {
        $('div#sort-filter-button').css("background-color", "#fdd835");
    }
    if (window.history.state == null) {
        window.history.replaceState({ filterData: filterInputData }, null, nowUrl);
    }
    if ($('.advertise-list__pin-filter-list-bottom').html() == "") {
        $('.advertise-list__pin-filter-list-bottom').css('display', 'none');
    }
};

function sendNewFilterData() {
    filterSubject.updateData(filterInputData, false);
}

window.onpopstate = function (event) {
    if (event.state != null) {
        var previousData = event.state.filterData;
        filterSubject.updateData(previousData, true);
        filterInputData = previousData;
    }
};

function PrevNextPage(destPage) {
    filterInputData.ajax = true;
    filterInputData.page = destPage;
    sendNewFilterData();
};

function toggleInstantReserve() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.instantReserve = !filterInputData.instantReserve;
    sendNewFilterData();
}

function toggleTodayEmpty() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.todayEmptyHomes = !filterInputData.todayEmptyHomes;
    sendNewFilterData();
}

function toggleHygieneProtocol() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.hygieneProtocol = !filterInputData.hygieneProtocol;
    sendNewFilterData();
}

function toggleDiscount() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.discountHomes = !filterInputData.discountHomes;
    sendNewFilterData();
}

function toggleNorouz() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.norouzSpecial = !filterInputData.norouzSpecial;
    sendNewFilterData();
}

function doDateFilter() {
    var fDate = $('#from_date_date_picker').val();
    var tDate = $('#to_date_date_picker').val();
    if (fDate == '' && tDate == '') {
        return;
    }
    filterInputData.emptyRangeFrom = fDate;
    filterInputData.emptyRangeTo = tDate;
    filterInputData.page = 1;
    filterInputData.ajax = true;
    sendNewFilterData();
}

function removeDateFilter() {
    filterInputData.emptyRangeFrom = '';
    filterInputData.emptyRangeTo = '';
    filterInputData.page = 1;
    filterInputData.ajax = true;
    sendNewFilterData();
}

function doCapacityFilter() {
    var capacity = $('#js-capacity-input').val();
    if (isNaN(capacity) || capacity == '0') {
        capacity = '-1';
    }
    filterInputData.capacity = capacity > 11 ? 11 : capacity;
    filterInputData.page = 1;
    filterInputData.ajax = true;
    sendNewFilterData();
};

function removeCapacityFilter() {
    filterInputData.capacity = '-1';
    filterInputData.page = 1;
    filterInputData.ajax = true;
    sendNewFilterData();
}

function doRegionFilter() {
    var tempProvince = $('select[name="accItemProvince"]').val();
    var tempCity = $('select[name="accItemCity"]').val();
    if (filterInputData.province != tempProvince || filterInputData.city != tempCity) {
        filterInputData.ajax = false;
    }
    filterInputData.province = tempProvince;
    filterInputData.city = tempCity;
    filterInputData.area = $('select[name="accItemArea"]').val();
    filterInputData.accType = '-1';
    filterInputData.t = '-1';
    filterInputData.page = 1;
    sendNewFilterData();
    filterInputData.ajax = true;
}

function removeRegionFilter() {
    filterInputData.area = '-1';
    filterInputData.city = '-1';
    filterInputData.province = '-1';
    filterInputData.page = 1;
    filterInputData.ajax = false;
    sendNewFilterData();
    filterInputData.ajax = true;
}

function changeSortOrder(sortOrder) {
    filterInputData.sort = sortOrder;
    filterInputData.page = 1;
    filterInputData.ajax = true;
    sendNewFilterData();
}

function doMoreFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    var currentAccType = $('#js-acctype-input').val();
    var prevT = filterInputData.t;
    if (prevT == -1) {
        prevT = 81;
    }
    if (currentAccType != prevT) {
        filterInputData.ajax = false;
    }
    filterInputData.accType = currentAccType;
    filterInputData.t = $('#js-acctype-input').val() == '81' ? '-1' : $('#js-acctype-input').val();
    filterInputData.position = $('#js-position-input').val();

    var priceRangeType = $('#js-pricerange-filter-type-input').val();
    filterInputData.priceType = priceRangeType;
    if (priceRangeType == '3') {
        filterInputData.priceMin = $('#js-pricemin-input').val() == '300000' ? '-1' : $('#js-pricemin-input').val();
        filterInputData.priceMax = $('#js-pricemax-input').val() == '150000000' ? '-1' : $('#js-pricemax-input').val()
    }
    else {
        filterInputData.priceMin = $('#js-pricemin-input').val() == '30000' ? '-1' : $('#js-pricemin-input').val();
        filterInputData.priceMax = $('#js-pricemax-input').val() == '10000000' ? '-1' : $('#js-pricemax-input').val();
    }

    filterInputData.roomList = $('#js-roomlist-input').val();
    filterInputData.parking = $('#js-parking-input').val();
    filterInputData.pets = $('#js-pets-input').val();
    filterInputData.party = $('#js-party-input').val();
    filterInputData.smoking = $('#js-smoking-input').val();
    filterInputData.pool = $('#js-pool-input').val();
    filterInputData.elevator = $('#js-elevator-input').val();
    filterInputData.perWC = $('#js-perwc-input').val();
    filterInputData.euWC = $('#js-euwc-input').val();
    filterInputData.wifi = $('#js-wifi-input').val();
    filterInputData.washingMachine = $('#js-washingmachine-input').val();
    filterInputData.jacuzzi = $('#js-jacuzzi-input').val();
    filterInputData.poolTable = $('#js-pooltable-input').val();
    filterInputData.foosball = $('#js-foosball-input').val();
    filterInputData.teaMaker = $('#js-teamaker-input').val();
    filterInputData.filming = $('#js-filming-input').val();
    filterInputData.exclusive = $('#js-exclusive-input').val();
    sendNewFilterData();
    $('#js-ajax-filter').val(true);
    $('#js-more-filter-item').hide(100);
}

function removeMoreFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    if (filterInputData.accType != '81') {
        filterInputData.ajax = false;
    }
    filterInputData.accType = '81';
    filterInputData.t = '-1';
    filterInputData.position = '-1';
    filterInputData.priceMin = '-1';
    filterInputData.priceMax = '-1';
    changePriceRangeType(0);
    filterInputData.roomList = '';
    filterInputData.parking = '-1';
    filterInputData.pets = '-1';
    filterInputData.party = '-1';
    filterInputData.smoking = '-1';
    filterInputData.pool = '-1';
    filterInputData.elevator = '-1';
    filterInputData.perWC = '-1';
    filterInputData.euWC = '-1';
    filterInputData.wifi = '-1';
    filterInputData.washingMachine = '-1';
    filterInputData.jacuzzi = '-1';
    filterInputData.poolTable = '-1';
    filterInputData.foosball = '-1';
    filterInputData.teaMaker = '-1';
    filterInputData.filming = '-1';
    filterInputData.exclusive = '-1';
    filterInputData.priceType = '0';
    sendNewFilterData();
}

function removeAccTypeFilter() {
    if (filterInputData.accType != '81') {
        filterInputData.accType = '81';
        filterInputData.t = '-1';
        filterInputData.ajax = false;
        filterInputData.page = 1;
    }
    sendNewFilterData();
}

function removePositionFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.position = '-1';
    sendNewFilterData();
}

function removePriceRangeFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.priceType = '0';
    filterInputData.priceMin = '-1';
    filterInputData.priceMax = '-1';
    changePriceRangeType(0);
    sendNewFilterData();
}

function removeRoomListFilter(itemToRemove) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    if (itemToRemove != '-1') {
        var currentVal = filterInputData.roomList;
        currentVal = currentVal.replace(',' + itemToRemove.toString() + ',', ',')
            .replace(',' + itemToRemove.toString(), '')
            .replace(itemToRemove.toString() + ',', '')
            .replace(itemToRemove.toString(), '');
        filterInputData.roomList = currentVal;
    }
    sendNewFilterData();
}

function removeParkingFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.parking = '-1';
    sendNewFilterData();
}

function removePetsFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.pets = '-1';
    sendNewFilterData();
}

function removePartyFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.party = '-1';
    sendNewFilterData();
}

function removeSmokingFilter() {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.smoking = '-1';
    sendNewFilterData();
}

function removePoolFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.pool = '-1';
    sendNewFilterData();
}

function removeElevatorFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.elevator = '-1';
    sendNewFilterData();
}

function removePerWCFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.perWC = '-1';
    sendNewFilterData();
}

function removeEuWCFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.euWC = '-1';
    sendNewFilterData();
}


function removeWifiFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.wifi = '-1';
    sendNewFilterData();
}

function removeWashingMachineFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.washingMachine = '-1';
    sendNewFilterData();
}

function removeJacuzziFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.jacuzzi = '-1';
    sendNewFilterData();
}

function removePoolTableFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.poolTable = '-1';
    sendNewFilterData();
}

function removeFoosballFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.foosball = '-1';
    sendNewFilterData();
}

function removeTeaMakerFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.teaMaker = '-1';
    sendNewFilterData();
}

function removeFilmingFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.filming = '-1';
    sendNewFilterData();
}

function removeExclusiveFilter(dontSubmit) {
    filterInputData.page = 1;
    filterInputData.ajax = true;
    filterInputData.exclusive = '-1';
    sendNewFilterData();
}