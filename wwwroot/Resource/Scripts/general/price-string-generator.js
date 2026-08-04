function getPriceString(price_val) {
    str_toman = "";
    if (price_val >= 1000000000) {
        str_toman += Math.floor(price_val / 1000000000) + " میلیارد";
        price_val = price_val % 1000000000;
    }
    if (price_val >= 1000000) {
        if (str_toman.length > 3) {
            str_toman += " و " + Math.floor(price_val / 1000000) + " میلیون";
        }
        else {
            str_toman += Math.floor(price_val / 1000000) + " میلیون";
        }
        price_val = price_val % 1000000;
    }
    if (price_val >= 1000) {
        if (str_toman.length > 3) {
            str_toman += " و " + Math.floor(price_val / 1000) + " هزار";
        }
        else {
            str_toman += Math.floor(price_val / 1000) + " هزار";
        }
        price_val = price_val % 1000;

    }
    if (price_val > 0) {

        if (str_toman.length > 3) {
            str_toman += " و " + price_val;
        }
        else {
            str_toman += price_val;
        }
    }

    return str_toman + " تومان";
}

function getPriceThousandSeperatorStr(price) {
    return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}