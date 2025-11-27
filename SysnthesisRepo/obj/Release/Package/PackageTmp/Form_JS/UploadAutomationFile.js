//function onImageUploadSuccess(args) {
//    //if (args.file.name != null) {
//    //    $("#FileNameInvoice").val(args.file.name);
//    //}
//    var fileNames = $('#FileNameInvoice').val();
//    if (fileNames) {
//        fileNames = fileNames.split(',');
//    } else {
//        fileNames = [];
//    }
//    fileNames.push(args.file.name);
//    $('#FileNameInvoice').val(fileNames.join(','));
//}

let totalFiles = 0;
let uploadedFiles = 0;

document.getElementById("UploadFiles").addEventListener("change", function (event) {
    totalFiles = event.target.files.length;
    document.getElementById("totalFiles").textContent = totalFiles;
});
function onImageUploadSuccess(args) {
    if (args.e.currentTarget.response) {
        var response = JSON.parse(args.e.currentTarget.response);
        var filePath = response.name;

        var existingValue = document.getElementById("FileNameInvoice").value;

        if (existingValue) {
            document.getElementById("FileNameInvoice").value = existingValue + ',' + filePath;
        } else {
            document.getElementById("FileNameInvoice").value = filePath;
        }
        uploadedFiles++;
        document.getElementById("uploadProgress").textContent = uploadedFiles;

        if (uploadedFiles === totalFiles) {
            setTimeout(function () {
                window.location.href = ROOTURL + "Invoices/InvoiceAutomationGrid";
            }, 3000);
        }
    }
    $(".uploadHeaderStatus").show();
}

function SaveUploadedFile() {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    var fileName = $('#FileNameInvoice').val();
    var enabledmode = $('input[name="enabledmode"]:checked').val();

    window.location.href = ROOTURL + "Invoices/InvoiceAutomationGrid";

    //if (!fileName && enabledmode == undefined) {
    //    toastr.error('Please select a file and an option.');
    //    return;
    //} else if (!fileName) {
    //    toastr.error('Please select a file.');
    //    return;
    //} else if (enabledmode == undefined) {
    //    toastr.error('Please select an option.');
    //    return;
    //}

    //$.ajax({
    //    url: ROOTURL + "Invoices/InsertUploadFileAutomation",
    //    type: 'POST',
    //    data: { fileName: fileName, enabledmode: enabledmode },
    //    success: function (response) {
    //        $('#FileNameInvoice').val('');
    //        $('input[name="enabledmode"][value="Automatic"]').prop('checked', true);

    //        // Clear the file uploader component
    //        var uploader = document.getElementById('UploadFiles').ej2_instances[0];
    //        if (uploader) {
    //            uploader.clearAll();
    //        }
    //        if (response.success == "Success") {
    //            filterSelfGrid();
    //            toastObj.content = 'File Uploaded Successfully.';
    //            toastObj.target = document.body;
    //            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
    //        }
    //        else {
    //            filterSelfGrid();
    //            toastObj.content = 'Something went to wrong!';
    //            toastObj.target = document.body;
    //            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.error('Error:', error);
    //    }
    //});
}

function ComfirmDelete(ID) {
    $("#AutomationUserIdD").show();
    $("#UploadPdfAutomationIddlt").val(ID);
}

function Delete() {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    var ID = $("#UploadPdfAutomationIddlt").val();
    $.ajax({
        url: ROOTURL + "/Invoices/DeleteInvoiceAutomation",
        type: 'POST',
        data: { Id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            if (response.success == "Delete") {
                filterSelfGrid();
                toastObj.content = 'File Deleted Successfully.';
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            else {
                filterSelfGrid();
                toastObj.content = 'Something went to wrong!';
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
        },
        error: function () {
            Loader(0);
        }
    });
}
function dataBound(e) {

    //var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    //if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
    //    var span = ej.base.createElement('span', {
    //        id: grid.element.id + '_searchcancelbutton',
    //        className: 'e-clear-icon'
    //    });
    //    span.addEventListener('click', (args) => {
    //        document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
    //        grid.search("");
    //    });
    //    grid.element.getElementsByClassName('e-search')[0].appendChild(span);
    //    grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
    //}

}

function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
}

setInterval(refreshGrid, 3000);

function refreshGrid() {
    var grid = document.getElementById("InvoiceBulkAutomation").ej2_instances[0];
    if (grid) {
        grid.refresh();
        filterSelfGrid();
    }
}

$(document).ready(function () {
    var grid = document.getElementById('InvoiceBulkAutomation');
    var count = grid.ej2_instances[0].dataSource.length;
    var gridCountElement = document.getElementById('invoicegridCount');
    if (count === undefined) {
        count = 0;
    }
    gridCountElement.textContent = 'Invoices for Review : ' + count;

/*    $(".upload-section").hide();
    $(".upload-footer").hide();
    $(".uploadHeaderStatus").hide();*/
    $(".upbutton").hide();

    ProcessStatusCheck();
});

function MainDriveopen() {
    $(".uploadContainer").addClass('Uploadactivemain');
    $(".uploadHeaderStatus").hide();
}

function Driveopen() {
    $(".upload-section").show();
    $(".upload-footer").show();
    $(".uploadHeaderStatus").show();
    $(".uploadContainer").removeClass('Uploadactive');
    $(".upbutton").hide();
    $(".downbutton").show();
}

function Driveclose() {
    $(".upload-section").hide();
    $(".upload-footer").hide();
    $(".uploadHeaderStatus").hide();
    $(".uploadContainer").addClass('Uploadactive');
    $(".downbutton").hide();
    $(".upbutton").show();
}

function ProcessStatusCheck() {
    var enabledmode = $('input[name="enabledmode"]:checked').val();
    $.ajax({
        type: 'POST',
        url: '/Invoices/SaveModeToSession',
        data: { mode: enabledmode },
        success: function (response) {
        },
        error: function (xhr, status, error) {
        }
    });
}
function actionComplete(args) {
    if (args.requestType === 'refresh' || args.requestType === 'paging') {
        updateGridCount();
    }
    updateGridCount();
}

function updateGridCount() {
    var grid = document.getElementById('InvoiceBulkAutomation').ej2_instances[0];
    var count = grid.getCurrentViewRecords().length;
    var gridCountElement = document.getElementById('invoicegridCount');
    if (count === undefined) {
        count = 0;
    }

    gridCountElement.textContent = 'Invoices for Review : ' + count;


}
function created(args) {
    // extending the default UrlAdaptor
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    class CustomAdaptor extends ej.data.UrlAdaptor {
        processResponse(data, ds, query, xhr, request, changes) {
            if (!ej.base.isNullOrUndefined(data.success)) {

                toastObj.content = data.success;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.Error)) {

                toastObj.content = data.Error;
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.data))
                return data.data;
            else
                return data;
        }
    }
    var grid = document.querySelector('#InvoiceBulkAutomation').ej2_instances[0];
    var showMyInvoices = document.getElementById('showMyInvoices').checked;
    grid.dataSource = new ej.data.DataManager({
        url: "/Invoices/InvoiceAutomationIndex?ShowMyInvoice=" + showMyInvoices,
        insertUrl: "",
        updateUrl: "",
        adaptor: new CustomAdaptor()
    });
};
function toolbarClick(args) {
}

function ActionEditAddCheck(data) {
    var editHtml = '';
    var deleteHtml = '';

    // Edit button
    //if (data.Is_Processing_Enabled == false) {
    //    var editAnchor = document.createElement('a');
    //    editAnchor.href = '/Invoices/Create?val=' + data.UploadPdfAutomationId;

    //    var editImg = document.createElement('img');
    //    editImg.alt = '';
    //    editImg.src = '/Content/Admin/images/file-plus.svg';

    //    editAnchor.appendChild(editImg);
    //    editHtml = editAnchor.outerHTML;
    //} else {
    //    var emptyAnchor = document.createElement('a');
    //    emptyAnchor.href = '';

    //    var emptyImg = document.createElement('img');
    //    emptyImg.alt = '';
    //    emptyImg.src = '';

    //    emptyAnchor.appendChild(emptyImg);
    //    editHtml = emptyAnchor.outerHTML;
    //}

    if (data.IsProcess == -1 || data.IsProcess == 1 && data.Synthesis_Live_InvID == 0) {
        var editAnchor = document.createElement('a');
        editAnchor.href = '/Invoices/Create?val=' + data.UploadPdfAutomationId;

        var editImg = document.createElement('img');
        editImg.alt = '';
        editImg.src = '/Content/Admin/images/file-plus.svg';

        editAnchor.appendChild(editImg);
        editHtml = editAnchor.outerHTML;
    }

    // Delete button
    var deleteAnchor = document.createElement('a');
    deleteAnchor.href = 'javascript:void(0);';
    deleteAnchor.setAttribute('onclick', 'ComfirmDelete(' + data.UploadPdfAutomationId + ')');

    var deleteImg = document.createElement('img');
    deleteImg.alt = '';
    deleteImg.src = '/Content/Admin/images/trash-2.svg';

    deleteAnchor.appendChild(deleteImg);
    deleteHtml = deleteAnchor.outerHTML;

    return deleteHtml + '&nbsp;&nbsp;' + editHtml;
}

//function StatusCheck(data) {
//    return data.IsProcess === 1 ? "Approved" : "Pending";
//}
function StatusCheck(data) {

    var SpanTag = document.createElement('span');
    if (data.IsProcess === 1 && data.Is_Processing_Enabled == true && data.Synthesis_Live_InvID != 0) {
        if (data.UploadedStatus == "Processed") {
            SpanTag.className = "approved";
            SpanTag.textContent = data.UploadedStatus;
        }
        else {
            SpanTag.className = "autoprocess";
            SpanTag.textContent = data.UploadedStatus;
        }
        
    } else if (data.IsProcess === 0 && data.Is_Processing_Enabled == true) {
        SpanTag.className = "autoprocess";
        SpanTag.textContent = "Automatic Processing";
    } else if (data.Is_Processing_Enabled == false) {
        SpanTag.textContent = "Manual Processing";
    } else if (data.IsProcess === -1) {
        SpanTag.className = "readingerror";
        SpanTag.textContent = "Analysis Failed";
    }
    else if (data.IsProcess == 1 && data.Synthesis_Live_InvID == 0) {
        SpanTag.className = "readingerror";
        SpanTag.textContent = "Analysis Failed";
    }
    return SpanTag.outerHTML;
}

function Filenamelinkcheck(data) {
    if (data.IsProcess == 1 && data.Is_Processing_Enabled == true && data.Synthesis_Live_InvID != 0) {
        var grid = document.querySelector(".e-grid").ej2_instances[0]
        var a = document.createElement('a');
        a.href = '/Invoices/InvoiceAutomationEdit?Id=' + data.Synthesis_Live_InvID;
        a.textContent = data.FileName;
        a.style.color = '#3C7BE5';

        return a.outerHTML;
    }
    else if (data.Is_Processing_Enabled == false) {
        var grid = document.querySelector(".e-grid").ej2_instances[0]
        var a = document.createElement('a');
        a.href = '/Invoices/Create?val=' + data.UploadPdfAutomationId;
        a.textContent = data.FileName;
        a.style.color = '#3C7BE5';

        return a.outerHTML;
    }
    else if (data.IsProcess == -1) {
        var grid = document.querySelector(".e-grid").ej2_instances[0]
        var a = document.createElement('a');
        a.href = '/Invoices/Create?val=' + data.UploadPdfAutomationId;
        a.textContent = data.FileName;
        a.style.color = '#3C7BE5';

        return a.outerHTML;
    }
    else if (data.IsProcess == 1 && data.Synthesis_Live_InvID == 0) {
        var grid = document.querySelector(".e-grid").ej2_instances[0]
        var a = document.createElement('a');
        a.href = '/Invoices/Create?val=' + data.UploadPdfAutomationId;
        a.textContent = data.FileName;
        a.style.color = '#3C7BE5';

        return a.outerHTML;
    }
    else {
        var grid = document.querySelector(".e-grid").ej2_instances[0]
        var a = document.createElement('a');
        a.href = 'javascript:void(0);';
        a.textContent = data.FileName;
        a.style.color = '#3C7BE5';

        return a.outerHTML;
    }

}

function filterSelfGrid() {
    var grid = document.querySelector('#InvoiceBulkAutomation').ej2_instances[0];
    var showMyInvoices = document.getElementById('showMyInvoices').checked;
    $.ajax({
        url: ROOTURL + "Invoices/InvoiceAutomationIndex",
        type: 'POST',
        data: { ShowMyInvoice: showMyInvoices },
        success: function (response) {
            response.forEach(function (item) {
                if (item.CreatedDate) {
                    var timestampMatch = item.CreatedDate.match(/\/Date\(([-+]?\d+)\)\//);
                    if (timestampMatch) {
                        var timestamp = parseInt(timestampMatch[1]);
                        var date = new Date(timestamp);

                        // Format the date to MM/dd/yyyy
                        var formattedDate = ("0" + (date.getMonth() + 1)).slice(-2) + "/" +
                            ("0" + date.getDate()).slice(-2) + "/" +
                            date.getFullYear() + " " +
                            ("0" + date.getHours()).slice(-2) + ":" +
                            ("0" + date.getMinutes()).slice(-2) + ":" +
                            ("0" + date.getSeconds()).slice(-2);

                        item.CreatedDate = formattedDate;
                    }
                }
            });
            var grid = document.querySelector("#InvoiceBulkAutomation").ej2_instances[0];
            grid.dataSource = response;
            var count = response.length;
            document.querySelector("#tab1 span").textContent = count;
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}