function showRatingDetail(id, userid) {
    var url = "/accomodation/userratingdetailpopup?id=" + id + "&userid=" + userid;
    showInfoMessage('', '', { contentUrl: url });
}