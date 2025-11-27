function updatestatus(iii, obj) {
    $.ajax({
        url: ROOTURL + '/Invoices/UpdateStatus',
        type: "POST",
        data: JSON.stringify({ id: iii, value: obj.checked }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response == "success") {

                if (obj.checked == true) {
                    toastr.success(StatusApp);
                }
                else {
                    toastr.success(StatusUnApp);
                }
            }
            else {
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
        },
        error: function (response) {
        }
    });
}

function resetStatus(iii) {

    var result = confirm(Reset);
    if (result) {
        $.ajax({
            url: ROOTURL + '/Invoices/ResetStatus',
            type: "POST",
            data: JSON.stringify({ id: iii }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response == "success") {
                    toastr.success(ResetSuccess);
                }
                else {
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
            },
            error: function (response) {
            }
        });
    }
}

function customiseCell(args) {
    if (args.column.field === 'UPCCode') {
        if (args.data['Approved'] == false) {
            if (args.data['UPCCode_Accuracy'] != null) {
                if (args.data['UPCCode_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['UPCCode_Accuracy'] > 85 && args.data['UPCCode_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['UPCCode_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'Description') {
        if (args.data['Approved'] == false) {
            if (args.data['Description_Accuracy'] != null) {
                if (args.data['Description_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Description_Accuracy'] > 85 && args.data['Description_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Description_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'Qty') {
        if (args.data['Approved'] == false) {
            if (args.data['Qty_Accuracy'] != null) {
                if (args.data['Qty_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Qty_Accuracy'] > 85 && args.data['Qty_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Qty_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'Total') {
        if (args.data['Approved'] == false) {
            if (args.data['Total_Accuracy'] != null) {
                if (args.data['Total_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Total_Accuracy'] > 85 && args.data['Total_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Total_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'UnitPrice') {
        if (args.data['Approved'] == false) {
            if (args.data['UnitPrice_Accuracy'] != null) {
                if (args.data['UnitPrice_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['UnitPrice_Accuracy'] > 85 && args.data['UnitPrice_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['UnitPrice_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
}

function InvoiceInformation() {
    $("#LineItems").removeClass("Tab");
    $("#UserFeedBack").removeClass("Tab");
    $("#InvoiceInformation").addClass("Tab");
    $("#2").addClass("hidden");
    $("#1").removeAttr("hidden");

    $("#btnReset").css("display", "none");
}

function LineItem() {
    $("#InvoiceInformation").removeClass("Tab");
    $("#UserFeedBack").removeClass("Tab");
    $("#LineItems").addClass("Tab");
    $("#1").attr("hidden", true);
    $("#2").removeClass("hidden");

    $("#btnReset").css("display", "inline");
}

function UserFeedBack() {
    $('#userreviewpopupid').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });
}

function UserNotes() {
    $('#usernotepopupid').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });
}

function UpdateUserReview() {
    var invoiceid = $("#InvoiceId").val();
    var reviewnote = $("#InvoiceReview").val();
    $.ajax({
        url: ROOTURL + "Invoices/UpdateInvoiceUserReview",
        type: 'POST',
        data: { invoiceid: invoiceid, reviewnote: reviewnote },
        success: function (response) {
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
            };
            if (response.success == "Success") {
                toastr.success('User Feedback Update Successfully.');
                location.reload();
            }
            else {
                toastr.error('Something went to wrong!');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function UpdateUserNote() {
    var invoiceid = $("#InvoiceId").val();
    var invoicetask = $("#InvoiceTasks").val();
    var priorityid = $(".prioritydata").val();
    var assignid = $(".assingtodata").val();
    var duedatetask = $("#dueDateTask").val();
    var invoiceno = $("#InvoiceNumberNT").val();
    if (invoicetask != "" && priorityid != "" && assignid != "" && duedatetask != "") {
        $.ajax({
            url: ROOTURL + "Invoices/UpdateInvoiceUserTasks",
            type: 'POST',
            data: { invoiceid: invoiceid, invoicetask: invoicetask, invoiceno: invoiceno, priorityid: priorityid, assignid: assignid, duedatetask: duedatetask },
            success: function (response) {
                if (response.success == "Success") {
                    toastr.success('User Task Inserted Successfully.');
                    location.reload();
                }
                else {
                    toastr.error('Something went to wrong!');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
    else {
        toastr.error('Please Insert Required Data...');
    }
    
}
function SetStoreidInvoiceGrid() {
    var result = JSON.parse(localStorage.getItem("SelectedPersistData"));


    if (result != null) {
        var storeid = result.storeid;
        $.ajax({
            url: '/Invoices/StoreIdFromSession',
            type: 'POST',
            data: { storeid: storeid },
            success: function (response) {
                window.location.href = "/Invoices/IndexBeta";
            },
            error: function (error) {
                console.error('Error sending storeid to server:', error);
            }
        });
    }
}
function Getvalues() {
    var aa = localStorage.getItem("FromInvoicePage");
    $("#FromInvoicePage").val(aa);
    if (aa == "") {
        localStorage.setItem("FromInvoicePage", '');
        $("#divBack").html("<a onclick='SetStoreidInvoiceGrid()' class='pull-right btnbackbox' data-toggle='tooltip' data-placement='bottom' title='Back'><i class='fa fa-chevron-left' aria-hidden='true'></i> Back</a>");
        $("#divCancel").html("<a onclick='SetStoreidInvoiceGrid()' class='buttonbox btncancel' data-toggle='tooltip' data-placement='bottom' title='Cancel'>Cancel</a>");
    }
    else if (aa == "InvoiceReport") {
        localStorage.setItem("FromInvoicePage", "InvoiceReport");
        $("#divBack").html("<a href='/InvoiceReport/Index' class='pull-right btnbackbox' data-toggle='tooltip' data-placement='bottom' title='Cancel'><i class='fa fa-chevron-left' aria-hidden='true'></i> Back</a>");
        $("#divCancel").html("<a href='/InvoiceReport/Index' class='buttonbox btncancel' data-toggle='tooltip' data-placement='bottom' title='Cancel'>Cancel</a>");
    }
}

function SetItems() {
    var aa = $("#FromInvoicePage").val();
    if (aa == "InvoiceReport") {
        localStorage.setItem("FromInvoicePage", "InvoiceReport");
    }
    else {
        localStorage.setItem("FromInvoicePage", '');
    }
}

var top = 0;
function Loader(val) {
    var doc = document.documentElement;

    if (val == 1) {

        $("#secloader").removeClass('pace-active1');
        $("#secloader").addClass('pace-active');
        $("#dvloader").removeClass('bak1');
        $("#dvloader").addClass('bak');
        $("#globalFooter").addClass('LoaderFooter');
        top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
    }
    else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
}

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
        return false;

    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

$(".numericOnly").bind('keypress', function (e) {
    if (e.keyCode == '9' || e.keyCode == '16') {
        return;
    }
    var code;
    if (e.keyCode) code = e.keyCode;
    else if (e.which) code = e.which;
    if (e.which == 46)
        return false;
    if (code == 8 || code == 46)
        return true;
    if (code < 48 || code > 57)
        return false;
});

$(".numericOnly").bind("paste", function (e) {
    e.preventDefault();
});

$(".numericOnly").bind('mouseenter', function (e) {
    var val = $(this).val();
});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

$('#country_id').change(function () {
    StateBind();

});

function ReplaceData(html_val, c_val, r_val) {
    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

//Type addmore starts
function AddField(val) {
    var val = CheckField();
    var html = $("#trrow_" + val).html();//.replaceWith("_"+val,"_"+(val+1));
    html = ReplaceData(html, "_" + val, "_" + (val + 1));
    $("#trrow_" + val).after("<div id='trrow_" + (val + 1) + "'>" + html + "</div>");
    document.getElementById('aRemove_' + (val + 1)).setAttribute('onclick', 'RemoveField(' + (val + 1) + ')')
    document.getElementById('aAdd_' + (val + 1)).setAttribute('onclick', 'AddField(' + (val + 1) + ')')
    var i_rtn = CheckField();
}

function RemoveField(val) {
    $("#trrow_" + val).remove();
    CheckField();
}

function CheckField() {
    var i = 0;
    $("#tbladdon :input[type='hidden']").each(function () {
        i = i + 1;
    });

    if (i == 1) {
        $(".aremove").hide();
    }
    else {
        $(".aremove").show();
    }
    return i;
}

function AddAmt() {

    var ival = CheckField();
    var temp = 0;
    var i;
    for (i = 0; i < ival; i++) {
        temp += parseFloat(document.getElementById('Amt_' + (i + 1)).value);
    }
    $("#TotalAmtByDept").val(temp);

}

function ddListPaymentTypeChange(paytype) {
    if (paytype == "Cash Payment") {
        $('#Type_id').val([1]).trigger('change');
        $('#Type_id').attr("disabled", "true");
    }
    else {
        $('#Type_id').removeAttr("disabled");
        $('#Type_id').val(['']).trigger('change');
        var typeid = $('#Type_id').val();

        $("#Type").val(typeid);
        if (typeid == "2") {
            $('#spncredit').removeAttr("style");
            $('#spncredit').attr("style", "display:block;")
        }
        else {
            $('#spncredit').removeAttr("style");
            $('#spncredit').attr("style", "display:none;")
        }
    }
}

function ddLTypeChange(paytype) {
    if (paytype == "2") {
        $('#spncredit').attr("style", "display:block;")
    }
    else {
        $('#spncredit').attr("style", "display:none;");
    }
}

function ComfirmOnHold(ID) {
    SetItems();
    var DivId = "#" + ID + "H";
    $(DivId).show();
}

function OnHold(ID) {
    $.ajax({
        url: ROOTURL + '/Invoices/InvoiceOnHold?id=' + ID,
        success: function (response) {
            $("#divstatus").hide();
            location.reload();
        },
        error: function () {
        }
    });
}

function ComfirmApprove(ID) {
    SetItems();
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Approve(ID) {
    var qbtransfer = 0;
    if ($('#QBtransfer').prop("checked") == true) {
        qbtransfer = 1;
    }

    $(".loading-containers").attr("style", "display:block");
    $(".headermainbox").attr("style", "z-index:0");
    $(".page-sidebar").attr("style", "z-index:0");
    $(".divIDClass").attr("style", "display:none");
    $.ajax({
        url: ROOTURL + '/Invoices/InvoiceApprove?id=' + ID + '&QBtransfer=' + qbtransfer,
        success: function (response) {
            $("#divstatus").hide();
            location.reload();
        },
        error: function () {
        }
    });
}

function ComfirmReject(ID) {
    SetItems();
    var DivId = "#" + ID + "R";
    $(DivId).show();
}

function Reject(ID) {
    $.ajax({
        url: ROOTURL + '/Invoices/InvoiceReject?id=' + ID,
        success: function (response) {
            $("#divstatus").hide();
            location.reload();
        },
        error: function () {
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

function ComfirmDelete(ID) {
    SetItems();
    $.ajax({
        url: '/Invoices/CheckInvoiceUSedAnywhere',
        data: { InvoiceID: ID },
        beforeSend: function () { Loader(1); },
        success: function (response) {

            if (response != null) {
                $("p#deleModalbody").html(response);
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
    var DivId = "#" + ID + "L";
    $(DivId).show();
}

function Delete(ID) {
    var aa = "";
    if ($("#deleModalbody").html().toString().includes("CashPaid Out")) {
        aa = "CashPaidOut";
    }
    $.ajax({
        url: ROOTURL + '/Invoices/Delete',
        data: { id: ID, From: aa },
        async: false,
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            if ($("#FromInvoicePage").val() == "InvoiceReport") {
                window.location.href = ROOTURL + '/InvoiceReport/Index';
            }
            else {
                window.location.href = ROOTURL + '/Invoices/IndexBeta';
            }
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}




