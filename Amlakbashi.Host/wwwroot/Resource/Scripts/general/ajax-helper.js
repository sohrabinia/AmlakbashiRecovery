function myajax(url, data, State, show_loading) {
    show_loading = typeof show_loading !== 'undefined' ? show_loading : true;
    if (show_loading)
        show_loading_icon();
    $.ajax({
        type: "GET",
        url: "/" + url,
        contentType: "application/json; charset=utf-8",
        data: data,
        cache: false,
        success: function (result) {
            hide_loading_icon();
            if (typeof State == 'function') {
                State(result);
            }
        },
        error: function (result) { hide_loading_icon(); alert("خطایی رخ داده است."); },
        complete: function (result) { hide_loading_icon(); }
    });
}