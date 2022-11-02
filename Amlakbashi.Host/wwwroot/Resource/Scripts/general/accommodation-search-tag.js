let searchTagInput = $('#search_tag_input');
let searchResultContainer = $('.tag-search-result-container');
let searchResultBox = $('.tag-search-result-box');
let selectedTagsBox = $('.selected-tag-box');
let tagsSelect = $('.selected-tag-container select');

function searchTag() {
    let searchedTag = searchTagInput.val();
    $.get(`/tag/search?title=${searchedTag}`, function (data) {
        searchResultBox.html(data);
    });
    searchResultContainer.show();
}

function selectTag(id, title) {
    selectedTagsBox.append(`<span data-tagid="${id}">${title}<i class="fa fa-times" onclick="deleteTag(${id})"></i></span>`);
    tagsSelect.append(`<option value="${id}" selected></option>`);
    searchResultContainer.hide();
    searchTagInput.val('');
}

function addNewTag() {
    let searchedTag = searchTagInput.val();
    //$.post('/tag/addinresidence', { title: searchedTag }, function (response) {
    //    selectTag(response.id, searchedTag);
    //    alertify.success('تگ مورد نظر با موفقیت افزوده شد');
    //});
    $.post('/tag/addinresidence', { title: searchedTag })
        .done(function (response) {
            selectTag(response.id, searchedTag);
            alertify.success('تگ مورد نظر با موفقیت افزوده شد');
        })
        .fail(function (xhr, status, error) {
            showErrorMessage('خطا', xhr.responseText);
        });
}

function deleteTag(id) {
    selectedTagsBox.find(`span[data-tagid='${id}']`).remove();
    tagsSelect.find(`option[value='${id}']`).remove();
}

$(document).click(function (e) {
    if ($(e.target).closest('.tag-search-result-container').length === 0) {
        searchResultContainer.hide();
    }
});