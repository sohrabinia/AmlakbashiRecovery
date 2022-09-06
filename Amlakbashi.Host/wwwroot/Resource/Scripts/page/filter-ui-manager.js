filterSubject.addListener(updateInstantReserveUI);
filterSubject.addListener(updateTodayEmptyUI);
filterSubject.addListener(updateDiscountUI);
filterSubject.addListener(updateNorouzUI);
filterSubject.addListener(updateHygieneProtocolUI);
filterSubject.addListener(updateDateUI);
filterSubject.addListener(updateCapacityUI);
filterSubject.addListener(updateSortUI);
filterSubject.addListener(updateRemoveMoreFilterUI);
filterSubject.addListener(updateMoreFilterBarUI);
filterSubject.addListener(updateMoreFilterContainerUI);

function updateInstantReserveUI(newData) {
    if (newData.instantReserve == true) {
        $('#instant-reserve-filter-button').css("background-color", "#fdd835");
    }
    else {
        $('#instant-reserve-filter-button').css("background-color", "white");
    }
}

function updateTodayEmptyUI(newData) {
    if (newData.todayEmptyHomes == true) {
        $('#today-empty-filter-button').css("background-color", "#fdd835");
    }
    else {
        $('#today-empty-filter-button').css("background-color", "white");
    }
}

function updateHygieneProtocolUI(newData) {
    if (newData.hygieneProtocol == true) {
        $('#corona-special-filter-button').css("background-color", "#fdd835");
    }
    else {
        $('#corona-special-filter-button').css("background-color", "white");
    }
}

function updateDiscountUI(newData) {
    if (newData.discountHomes == true) {
        $('#discount-homes-filter-button').css("background-color", "#fdd835");
    }
    else {
        $('#discount-homes-filter-button').css("background-color", "white");
    }
}

function updateNorouzUI(newData) {
    if (newData.norouzSpecial == true) {
        $('#norouz-special-filter-button').css("background-color", "#fdd835");
    }
    else {
        $('#norouz-special-filter-button').css("background-color", "white");
    }
}

function updateDateUI(newData) {
    hideAllFilterItems();
    if (newData.emptyRangeFrom != "" && newData.emptyRangeTo != "") {
        $('#divDateFilter').css("background-color", "#fdd835");
        $('#divDateFilter span').text(newData.emptyRangeFrom.substring(5, newData.emptyRangeFrom.length) + " تا " +
            newData.emptyRangeTo.substring(5, newData.emptyRangeTo.length));
        $('#from_date_date_picker').val(newData.emptyRangeFrom);
        $('#to_date_date_picker').val(newData.emptyRangeTo);
    }
    else if (newData.emptyRangeFrom != "") {
        $('#divDateFilter').css("background-color", "#fdd835");
        $('#divDateFilter span').text(newData.emptyRangeFrom.substring(5, newData.emptyRangeFrom.length));
        $('#from_date_date_picker').val(newData.emptyRangeFrom);
        $('#to_date_date_picker').val('');
    }
    else {
        $('#divDateFilter').css("background-color", "white");
        $('#divDateFilter span').text("تاریخ سفر");
        $('#from_date_date_picker').val('');
        $('#to_date_date_picker').val('');
    }
    $('#js-date-filter-item').hide(100);
}

function updateCapacityUI(newData) {
    hideAllFilterItems();
    var val = parseInt(newData.capacity);
    if (val < 1) {
        $('#divCapacityFilter').css("background-color", "white");
        $('#divCapacityFilter span').text("نفرات");
    }
    else {
        $('#divCapacityFilter').css("background-color", "#fdd835");
        $('#divCapacityFilter span').text(val + " نفر");
    }
    $('#js-capacity-input').val(newData.capacity);
    $('#js-capacity-filter-item').hide(100);
}

function updateSortUI(newData) {
    $('div.advertise-list__sort-popup-item').removeClass("sort-item-selected");
    $('div#sortItem' + newData.sort).addClass("sort-item-selected");
    if (newData.sort == '0') {
        $('div#sort-filter-button').css("background-color", "white");
        $('div#sort-filter-button span').text("مرتب سازی");
    }
    else {
        $('div#sort-filter-button').css("background-color", "#fdd835");
        $('div#sort-filter-button span').text($('#sortItem' + newData.sort + ' span').text());
    }
    $('div.advertise-list__sort-popup-parent').slideUp(200);
}

function updateMoreFilterContainerUI(newData) {
    hideAllFilterItems();
    $('#js-positions-container div').removeClass("filter-selected-item");
    if (newData.position != '-1') {
        $('#js-positions-container div[data-value="' + newData.position + '"]').addClass("filter-selected-item");
    }
    $('#js-position-input').val(newData.position);
    $('[name="js-price-range-type"]').prop("checked", false);
    $('#js-price-range-type-' + newData.priceType).prop("checked", true);
    changePriceRangeType(newData.priceType);
    if (newData.priceType == '3') {
        if (newData.priceMin == '-1') {
            $('#js-pricemin-input').val('300000');
        }
        else {
            $('#js-pricemin-input').val(newData.priceMin);
        }

        if (newData.priceMax == '-1') {
            $('#js-pricemax-input').val('150000000');
        }
        else {
            $('#js-pricemax-input').val(newData.priceMax);
        }
    }
    else {
        if (newData.priceMin == '-1') {
            $('#js-pricemin-input').val('30000');
        }
        else {
            $('#js-pricemin-input').val(newData.priceMin);
        }

        if (newData.priceMax == '-1') {
            $('#js-pricemax-input').val('10000000');
        }
        else {
            $('#js-pricemax-input').val(newData.priceMax);
        }
    }

    $('#js-room-container div').removeClass("filter-selected-item");
    $('#js-roomlist-input').val(newData.roomList);
    var roomIds = newData.roomList.split(',');
    for (var i = 0; i < roomIds.length; i++) {
        $('#js-room-container div[data-value="' + roomIds[i] + '"]').addClass("filter-selected-item");
    }

    if (newData.pool == '1') {
        $('#js-pool-input').val('1');
        $('#js-pool-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-pool-input').val('-1');
        $('#js-pool-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.elevator == '1') {
        $('#js-elevator-input').val('1');
        $('#js-elevator-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-elevator-input').val('-1');
        $('#js-elevator-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.parking == '1') {
        $('#js-parking-input').val('1');
        $('#js-parking-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-parking-input').val('-1');
        $('#js-parking-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.perWC == '1') {
        $('#js-perwc-input').val('1');
        $('#js-perwc-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-perwc-input').val('-1');
        $('#js-perwc-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.euWC == '1') {
        $('#js-euwc-input').val('1');
        $('#js-euwc-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-euwc-input').val('-1');
        $('#js-euwc-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.wifi == '1') {
        $('#js-wifi-input').val('1');
        $('#js-wifi-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-wifi-input').val('-1');
        $('#js-wifi-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.washingMachine == '1') {
        $('#js-washingmachine-input').val('1');
        $('#js-washingmachine-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-washingmachine-input').val('-1');
        $('#js-washingmachine-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.jacuzzi == '1') {
        $('#js-jacuzzi-input').val('1');
        $('#js-jacuzzi-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-jacuzzi-input').val('-1');
        $('#js-jacuzzi-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.poolTable == '1') {
        $('#js-pooltable-input').val('1');
        $('#js-pooltable-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-pooltable-input').val('-1');
        $('#js-pooltable-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.foosball == '1') {
        $('#js-foosball-input').val('1');
        $('#js-foosball-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-foosball-input').val('-1');
        $('#js-foosball-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.teaMaker == '1') {
        $('#js-teamaker-input').val('1');
        $('#js-teamaker-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-teamaker-input').val('-1');
        $('#js-teamaker-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.filming == '1') {
        $('#js-filming-input').val('1');
        $('#js-filming-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-filming-input').val('-1');
        $('#js-filming-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.pets == '1') {
        $('#js-pets-input').val('1');
        $('#js-pets-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-pets-input').val('-1');
        $('#js-pets-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.party == '1') {
        $('#js-party-input').val('1');
        $('#js-party-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-party-input').val('-1');
        $('#js-party-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }

    if (newData.smoking == '1') {
        $('#js-smoking-input').val('1');
        $('#js-smoking-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=checked-3');
    }
    else {
        $('#js-smoking-input').val('-1');
        $('#js-smoking-input').next().find('img').attr('src', '/file/resourceimagepng?file_name=unchecked-3');
    }
}

function updateMoreFilterBarUI(newData) {
    $('.advertise-list__pin-filter-list-bottom').css('display', 'flex');
    $('.body').attr("data-more-filter", "true");

    if (newData.t != '-1') {
        accTypeName = $('#js-acctype-container').children("div[data-value='" + newData.t + "']").children('span').text();
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeAccTypeFilter()" class="advertise-list__pin-filter-list-item"><span>' + accTypeName + '</span><i class="fa fa-times"></i></div>');
    }

    if (newData.position != '-1') {
        regionName = $('#js-positions-container').children("div[data-value='" + newData.position + "']").children('span').text();
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePositionFilter()" class="advertise-list__pin-filter-list-item"><span>' + regionName + '</span><i class="fa fa-times"></i></div>');
    }

    if (newData.priceMin != '-1' || newData.priceMax != '-1') {
        var payString = "";
        if (newData.priceType != '0') {
            if (newData.priceType == '1') {
                payString = "قیمت تعطیلات ";
            }
            else if (newData.priceType == '2') {
                payString = "قیمت پیک تعطیلات ";
            }
            else if (newData.priceType == '4') {
                payString = "قیمت نوروز ";
            }
            else {
                payString = "قیمت ماهانه ";
            }
        }

        if (newData.priceMin != '-1') {
            if (newData.priceType == 3) {
                payString = payString + "از " + newData.priceMin.slice(0, newData.priceMin.length - 6) + " میلیون ";
            }
            else {
                if (newData.priceMin.length < 7) {
                    payString = payString + "از " + newData.priceMin.slice(0, newData.priceMin.length - 3) + " هزار ";
                }
                else {
                    payString = payString + "از " + newData.priceMin.charAt(0) + " میلیون ";
                    if (newData.priceMin.charAt(1) != '0') {
                        payString = payString + "و " + newData.priceMin.slice(1, newData.priceMin.length - 3) + " هزار ";
                    }
                }
            }
        }

        if (newData.priceMax != '-1') {
            if (newData.priceType == 3) {
                payString = payString + "تا " + newData.priceMax.slice(0, newData.priceMax.length - 6) + " میلیون ";
            }
            else {
                if (newData.priceMax.length < 7) {
                    payString = payString + "تا " + newData.priceMax.slice(0, newData.priceMax.length - 3) + " هزار";
                }
                else {
                    payString = payString + "تا " + newData.priceMax.charAt(0) + " میلیون ";
                    if (newData.priceMax.charAt(1) != '0') {
                        payString = payString + "و " + newData.priceMax.slice(1, newData.priceMax.length - 3) + " هزار ";
                    }
                }
            }
        }
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePriceRangeFilter()" class="advertise-list__pin-filter-list-item"><span>' + payString + '</span><i class="fa fa-times"></i></div>');
    }

    if (newData.roomList != '') {
        var roomIds = newData.roomList.split(',');
        for (var i = 0; i < roomIds.length; i++) {
            roomName = $('#js-room-container').children("div[data-value='" + roomIds[i] + "']").children('span').text();
            $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeRoomListFilter(' + roomIds[i] + ')" class="advertise-list__pin-filter-list-item"><span>' + roomName + '</span><i class="fa fa-times"></i></div>');
        }
    }

    if (newData.pool == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePoolFilter()" class="advertise-list__pin-filter-list-item"><span>استخر </span><i class="fa fa-times"></i></div>');
    }

    if (newData.elevator == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeElevatorFilter()" class="advertise-list__pin-filter-list-item"><span>آسانسور </span><i class="fa fa-times"></i></div>');
    }

    if (newData.parking == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeParkingFilter()" class="advertise-list__pin-filter-list-item"><span>پارکینگ </span><i class="fa fa-times"></i></div>');
    }

    if (newData.perWC == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePerWCFilter()" class="advertise-list__pin-filter-list-item"><span>سرویس ایرانی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.euWC == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeEuWCFilter()" class="advertise-list__pin-filter-list-item"><span>سرویس فرنگی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.wifi == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeWifiFilter()" class="advertise-list__pin-filter-list-item"><span>وای فای </span><i class="fa fa-times"></i></div>');
    }

    if (newData.washingMachine == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeWashingMachineFilter()" class="advertise-list__pin-filter-list-item"><span>ماشین لباسشویی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.jacuzzi == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeJacuzziFilter()" class="advertise-list__pin-filter-list-item"><span>جکوزی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.poolTable == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePoolTableFilter()" class="advertise-list__pin-filter-list-item"><span>بیلیارد </span><i class="fa fa-times"></i></div>');
    }

    if (newData.foosball == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeFoosballFilter()" class="advertise-list__pin-filter-list-item"><span>فوتبال دستی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.teaMaker == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeTeaMakerFilter()" class="advertise-list__pin-filter-list-item"><span>چای ساز </span><i class="fa fa-times"></i></div>');
    }

    if (newData.filming == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeFilmingFilter()" class="advertise-list__pin-filter-list-item"><span>فیلم برداری </span><i class="fa fa-times"></i></div>');
    }

    if (newData.pets == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePetsFilter()" class="advertise-list__pin-filter-list-item"><span>حیوان خانگی </span><i class="fa fa-times"></i></div>');
    }

    if (newData.party == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removePartyFilter()" class="advertise-list__pin-filter-list-item"><span>مهمانی گرفتن </span><i class="fa fa-times"></i></div>');
    }

    if (newData.smoking == '1') {
        $('.advertise-list__pin-filter-list-bottom').append('<div onclick="removeSmokingFilter()" class="advertise-list__pin-filter-list-item"><span>استعمال دخانیات </span><i class="fa fa-times"></i></div>');
    }

    if ($('.advertise-list__pin-filter-list-bottom').html() == "") {
        $('.advertise-list__pin-filter-list-bottom').css('display', 'none');
        $('.body').attr("data-more-filter", "false");
    }
}

function updateRemoveMoreFilterUI(newData) {
    $('#js-more-filter-item').hide(100);
    $('.advertise-list__pin-filter-list-bottom').html("");
}