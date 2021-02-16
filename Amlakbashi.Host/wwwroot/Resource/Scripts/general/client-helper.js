function clientIsInIran(callback) {
    $.ajax('https://www.extreme-ip-lookup.com/json/')
    .then(
        function success(response) {
            callback(response.countryCode == "IR");
        }
    );
}
