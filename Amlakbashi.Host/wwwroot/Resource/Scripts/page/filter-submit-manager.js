filterSubject.addListener(SendRequest);

var nowUrl;
var pushStateData;
var isBack;

function SendRequest(newData, isBackRequest) {
    pushStateData = newData;
    isBack = isBackRequest;
    SetAdvertiseFilterForm(newData);
    $('#advertise-filter-form').submit();
}

function SetAdvertiseFilterForm(newData) {
    $('#advertise-filter-form input[name="page"]').attr('value', newData.page);
    $('#advertise-filter-form input[name="discount_homes"]').attr('value', newData.discountHomes);
    $('#advertise-filter-form input[name="instant_reserve"]').attr('value', newData.instantReserve);
    $('#advertise-filter-form input[name="today_empty_homes"]').attr('value', newData.todayEmptyHomes);
    $('#advertise-filter-form input[name="norouz_special"]').attr('value', newData.norouzSpecial);
    $('#advertise-filter-form input[name="hygieneProtocol"]').attr('value', newData.hygieneProtocol);
    $('#advertise-filter-form input[name="country_direction"]').attr('value', newData.countryDirection);
    $('#advertise-filter-form input[name="Province"]').attr('value', newData.province);
    $('#advertise-filter-form input[name="City"]').attr('value', newData.city);
    $('#advertise-filter-form input[name="Area"]').attr('value', newData.area);
    $('#js-capacity-input-filter').attr('value', newData.capacity);
    $('#from_date_date_picker-filter').attr('value', newData.emptyRangeFrom);
    $('#to_date_date_picker-filter').attr('value', newData.emptyRangeTo);
    $('#js-acctype-input-filter').attr('value', newData.accType);
    $('#js-t-input-filter').attr('value', newData.t);
    $('#js-position-input-filter').attr('value', newData.position);
    $('#js-pricemin-input-filter').attr('value', newData.priceMin);
    $('#js-pricemax-input-filter').attr('value', newData.priceMax);
    $('#js-roomlist-input-filter').attr('value', newData.roomList);
    $('#js-parking-input-filter').attr('value', newData.parking);
    $('#js-pool-input-filter').attr('value', newData.pool);
    $('#js-elevator-input-filter').attr('value', newData.elevator);
    $('#js-pricetype-input-filter').attr('value', newData.priceType);
    $('#js-perwc-input-filter').attr('value', newData.perWC);
    $('#js-euwc-input-filter').attr('value', newData.euWC);
    $('#js-wifi-input-filter').attr('value', newData.wifi);
    $('#js-washingmachine-input-filter').attr('value', newData.washingMachine);
    $('#js-jacuzzi-input-filter').attr('value', newData.jacuzzi);
    $('#js-pooltable-input-filter').attr('value', newData.poolTable);
    $('#js-foosball-input-filter').attr('value', newData.foosball);
    $('#js-teamaker-input-filter').attr('value', newData.teaMaker);
    $('#js-filming-input-filter').attr('value', newData.filming);
    $('#js-pets-input-filter').attr('value', newData.pets);
    $('#js-party-input-filter').attr('value', newData.party);
    $('#js-smoking-input-filter').attr('value', newData.smoking);
    $('#js-sort-order-input-filter').attr('value', newData.sort);
    $('#js-ajax-filter').attr('value', newData.ajax);
}

$("#advertise-filter-form").submit(function (e) {
    if (isBack == false) {
        $('html, body').animate({ scrollTop: $('#accListContainer').offset().top - 250 }, 300);
    }
    var form = $(this);
    if ($('#js-ajax-filter').val() == "true") {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: "/category/category",
            data: form.serialize(),
            beforeSend: function () {
                $images = null;
                $('img.home-page__advertise-image').attr('src', '/file/resourceimagegif?file_name=lazy-16_9');
            },
            success: function (response) {
                if (response.status === 0) {
                    window.location.href = response.url;
                    return;
                }
                $("#accListContainer").html(response);
                if (isBack == false) {
                    window.history.pushState({ filterData: pushStateData }, null, nowUrl);
                }
                findLazyImages();
            }
        });
    }
    else {
    }
});