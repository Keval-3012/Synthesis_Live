function onImageUploadSuccess(args) {
    //if (args.file.name != null) {
    //    $("#FileNameInvoice").val(args.file.name);
    //}
    if (args.e.currentTarget.response) {
        var response = JSON.parse(args.e.currentTarget.response);
        var filePath = response.name;

        var existingValue = document.getElementById("FileNameInvoice").value;

        if (existingValue) {
            document.getElementById("FileNameInvoice").value = existingValue + ',' + filePath;
        } else {
            document.getElementById("FileNameInvoice").value = filePath;
        }
    }
}


function SaveUploadedReciptFile() {
    var fileName = $('#FileNameInvoice').val();

    //var companyName = document.getElementById('CustomerReciptID').ej2_instances[0].value;

    //if (!fileName && companyName == null) {
    //    toastr.error('Please select a CompanyName and File.');
    //    return;
    //} else if (!fileName) {
    //    toastr.error('Please select a file.');
    //    return;
    //} else if (companyName == null) {
    //    toastr.error('Please select CompanyName.');
    //    return;
    //}
    var companyName = "0";
    window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
    //$.ajax({
    //    url: ROOTURL + "CustomerInformation/SaveFile1",
    //    type: 'POST',
    //    data: { FileName: fileName, CompanyNameId: companyName },
    //    success: function (response) {
    //        toastr.options = {
    //            "closeButton": true,
    //            "debug": false,
    //            "newestOnTop": false,
    //            "progressBar": false,
    //            "positionClass": "toast-top-right",
    //            "preventDuplicates": false,
    //            "onclick": null,
    //            "showDuration": "3000",
    //            "hideDuration": "1000",
    //            "timeOut": "5000",
    //            "extendedTimeOut": "10000",
    //            "showEasing": "swing",
    //            "hideEasing": "linear",
    //            "showMethod": "fadeIn",
    //            "hideMethod": "fadeOut"
    //        };
    //        if (response.success == "Success") {
    //            toastr.success('File Uploaded Successfully.');
    //            window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
    //        }
    //        else {
    //            toastr.error('Something went to wrong!');
    //            window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.error('Error:', error);
    //    }
    //});
}

function OpenPopupSendEmailModel(id) {
    $('#emailmodalpopup').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });

    $.ajax({
        url: ROOTURL + "CustomerInformation/GetCustomerReceivedReceiptData",
        type: 'POST',
        data: { CustomersReceiveablesReceiptsId: id },
        success: function (response) {
            if (response.CustomersReceiveablesReceiptsId != "") {
                $("#CustomersReceiveablesReceiptsId").val(response.CustomersReceiveablesReceiptsId);
                $("#EmailFileName").text(response.FileName);
                if (response.IsEmailTriggered == true) {
                    $("#sendEmailButton").text("Re-Send Email");
                } else {
                    $("#sendEmailButton").text("Send Email");
                }
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function SendEmailToCustomer() {
    $("#sendEmailButton").prop("disabled", true).addClass('button-green-email');
    var customerreceiptid = $("#CustomersReceiveablesReceiptsId").val();
    $.ajax({
        url: ROOTURL + "CustomerInformation/SendReceiptEmailToCustomer",
        type: 'POST',
        data: { customerreceiptid: customerreceiptid },
        success: function (response) {
            if (response.success == "Success") {
                toastr.success('Email Sent Successfully.');
                window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
            }
            else {
                toastr.error('Something went to wrong!');
                window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Delete(ID) {
    
    $.ajax({
        url: ROOTURL + "/CustomerInformation/DeleteCustomerReceipt",
        type: 'POST',
        data: { Id: ID },
        success: function (response) {
            if (response.success == "Delete") {
                $(".divIDClass").hide();
                toastr.success('Data Deleted Successfully.');
                setTimeout(function () {
                    window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
                }, 700);
            }
        },
        error: function () {
            Loader(0);
        }
    });
}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});


function SelectCompanyNamePopup(id) {
    $.ajax({
        url: ROOTURL + "CustomerInformation/GetCompanyName",
        type: 'POST',
        data: { CustomersReceiveablesReceiptsId: id },
        success: function (response) {
            
            if (response.CompanyNameId != "") {
                var data = document.getElementById("CustomerReciptID").ej2_instances[0];
                data.value = response.CompanyNameId
                
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });

    $('#companynamemodalpopup').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });
    $('#CustomersReceiptsId').val(id);

    
}

function SaveCompanyName() {
    
    var CustomersReceiptsId = $('#CustomersReceiptsId').val();
    var CompanyNameId = document.getElementById('CustomerReciptID').ej2_instances[0].value;

    $.ajax({
        url: ROOTURL + "CustomerInformation/UpdateCompanyName",
        type: 'POST',
        data: { CustomersReceiveablesReceiptsId: CustomersReceiptsId, CompanyNameId: CompanyNameId },
        success: function (response) {
            
            if (response.success == "Success") {
                toastr.success('Company selected Successfully.');
                window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
            }
            else {
                toastr.error('Something went to wrong!');
                window.location.href = ROOTURL + "CustomerInformation/CustomerRecipt";
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function DownloadCustomerReceipt() {
    var CustomersReceiveablesReceiptsId = $("#CustomersReceiveablesReceiptsId").val();
    window.location.href = ROOTURL + "/CustomerInformation/CustomerReceiptDownload?id=" + CustomersReceiveablesReceiptsId;
}