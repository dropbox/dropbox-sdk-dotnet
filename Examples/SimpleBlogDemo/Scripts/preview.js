$(document).ready(function () {
    $('#preview').prop('disabled', false).click(function (e) {
        var markdown = $('#content').val();
        $.ajax('/Preview', {
            method: 'POST',
            data: {
                __RequestVerificationToken: $('input[name=__RequestVerificationToken]').val(),
                markdown: markdown
            }
        }).done(function (msg) {
            $('#preview-loading').hide();
            $('#preview-content').show().html(msg);
        }).fail(function (jqXHR, textStatus) {
            $('#preview-loading').hide();
            $('#preview-content').show().text("There was an error generating a preview.").addClass('text-danger');
        });
        $('#preview-modal').modal();
        $('#preview-loading').show();
        $('#preview-content').hide().removeClass('text-danger');

        e.preventDefault();
    });
});
