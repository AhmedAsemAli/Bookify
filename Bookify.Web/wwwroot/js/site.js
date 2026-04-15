//const { error } = require("jquery");
var updatedRow;
function showSuccessMessage(message = 'Saved successfully!') {
    Swal.fire({
        title: "Success",
        icon: "success",
        text: message,
        draggable: true
    });
}

function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: message
    });
}

function onModalSuccess(item)
{
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow === undefined) {
        $('tbody').append(item);
    } else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined;
    }
    
    KTMenu.init();
    KTMenu.initHandlers();
}


$(document).ready(function () {
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }
    //handel bootstrap modal
    $('body').delegate('.js-render-modal','click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));
        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }
        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal)
            },
            error: function () {
                showErrorMessage();
            }
        });


        modal.modal('show');
    });

});
