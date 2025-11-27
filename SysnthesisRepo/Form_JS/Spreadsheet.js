window.onload = function () {
    $('#HeaderStoreId').prop('disabled', true);
    var spreadsheetObj = ej.base.getComponent(document.getElementById('spreadsheet'), 'spreadsheet');
    fetch('/SpreadSheet/LoadExcel',
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ FileName: fileName, FilePath: AttachLink }),
        })
        .then((response) => response.json())
        .then((data) => {
            console.log(data);
            spreadsheetObj.openFromJson({ file: data });
        })

    setTimeout(function () {
        $("#spreadsheet > .file-input-wrapper").addClass("hidden");
    }, 1000);
}

$(document).ready(function () {
    $("#spreadsheet_sheet_panel").css("height", "750px")
    var message = Message;
    if (message != '') {
        if (message == "0") {
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": false,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "onclick": null,
                "showDuration": "3000",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "10000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
            toastr.success('Successfully Saved Excel File.');
        }
        else {
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": false,
                "progressBar": false,
                "positionClass": "toast-top-right",
                "preventDuplicates": false,
                "onclick": null,
                "showDuration": "3000",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "10000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }

            toastr.error('Excel File Save Failed.');
        }
    }


});

function itemSelect() {

    var spreadsheetObj = ej.base.getComponent(document.getElementById('spreadsheet'), 'spreadsheet');

    spreadsheetObj.save({ url: ROOTURL + "/Spreadsheet/save", fileName: AttachFile, saveType: "Xlsx" });
}

function saveFile() {

    var spreadsheetObj = ej.base.getComponent(document.getElementById('spreadsheet'), 'spreadsheet');
    spreadsheetObj.save({ url: ROOTURL + "/Spreadsheet/SaveFile", fileName: AttachFile, saveType: "Xlsx" });
}

function EditDocument() {

    var DocumentId = $('#DocumentId').val();
    var DocumentCategoryId = $('#DocumentCategoryId').val();
    var KeyWords = $('#KeyWords').val();
    var Notes = $('#Notes').val();
    var chkFav = $('#chkFav').val();
    var Title = $('#Title').val();
    $.ajax({
        url: ROOTURL + "/SpreadSheet/EditDocumentsDetaliValue",
        type: "post",
        cache: false,
        data: { DocumentId: DocumentId, DocumentCategoryId: DocumentCategoryId, KeyWords: KeyWords, Notes: Notes, chkFav: chkFav, Title: Title },
        success: function (states) {

            if (states == "Completed") {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": null,
                    "showDuration": "3000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
                toastr.success('Successfully Edit Document Data.');
                $('#EditModel').modal('hide');
            }
            else {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": null,
                    "showDuration": "3000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }

                toastr.error('Something went to wrong');
            }
        }
    });
}
