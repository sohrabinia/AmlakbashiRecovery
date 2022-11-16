function quickFilterProcess() {
    $('#more_filter_form [name="title"]').val($('.box-filter .bar-filter [name="title"]').val());
    $('#more_filter_form [name="status"]').val($('.box-filter .bar-filter [name="status"').val());
    $('#more_filter_form').submit();
}

function showAddTagPopup() {
    showPopup('<p>لطفا عنوان تگ جدید را وارد کرده و روی دکمه «ثبت» کلیک کنید:</p>' +
        '<div class="add-tag-operation">' +
        '<input type="text" id="new_tag_title" class="text-input" />' +
        '<button onclick="addNewTag()" type="button" class="btn">ثبت</button>' +
        '</div>');
}

function addNewTag() {
    let title = $("#new_tag_title").val();
    if (!title) {
        errorAlert('لطفا عنوان تگ جدید را وارد کنید');
        return;
    }
    sendPostAjax('/tags/add', { title }, function () {
        location.reload();
    }, null, hidePopup);
}

function activeTag(id, elem) {
    showConfirm("آیا از تایید این تگ مطمئنید؟", function () {
        sendPostAjax("/tags/activate", { id }, function () {
            $(elem).css('color', 'limegreen');
            successAlert('تگ مورد نظر با موفقیت تایید شد');
        });
    });
}

function showEditPopup(id, title) {
    showPopup('<p>لطفا عنوان جدید را وارد کرده و روی دکمه «ثبت» کلیک کنید:</p>' +
        '<div class="add-tag-operation">' +
        `<input type="hidden" id="edited_tag_id" value="${id}" /> ` +
        `<input type="text" id="edited_tag_title" class="text-input" value="${title}" /> ` +
        '<button onclick="editTagTitle()" type="button" class="btn">ثبت</button>' +
        '</div>');
}

function editTagTitle() {
    let id = $("#edited_tag_id").val();
    let title = $("#edited_tag_title").val();
    if (!title) {
        errorAlert('لطفا عنوان تگ جدید را وارد کنید');
        return;
    }
    sendPostAjax('/tags/edittitle', { id, title }, function () {
        $(`#js-${id} .column-title`).html(title);
        $(`#js-${id} .column-urltitle`).html(title);
        successAlert('تگ مورد نظر با موفقیت ویرایش شد');
    }, null, hidePopup);
}

function deleteTag(id) {
    showConfirm("آیا از حذف این تگ مطمئنید؟", function () {
        sendPostAjax("/tags/delete", { id }, function () {
            $('#js-' + id).remove();
            successAlert('تگ مورد نظر با موفقیت حذف شد');
        });
    });
}