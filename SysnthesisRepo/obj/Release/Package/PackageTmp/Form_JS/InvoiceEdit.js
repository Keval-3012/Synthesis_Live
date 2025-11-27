
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
}

function LineItem() {
    $("#InvoiceInformation").removeClass("Tab");
    $("#UserFeedBack").removeClass("Tab");
    $("#LineItems").addClass("Tab");
    $("#1").attr("hidden", true);
    $("#2").removeClass("hidden");
}

function UserFeedBack() {
    $('#userreviewpopupid').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });
}

function UpdateUserReview() {
    var feedback = $('#InvoiceReview').val();

    if (feedback.trim() !== "") {
        $('.msg-review').show();
    } else {
        $('.msg-review').hide();
    }
    toastr.success('User Feedback Update Successfully.');
}

function Getvalues() {
    var aa = localStorage.getItem("FromInvoicePage");
    $("#FromInvoicePage").val(aa);
    if (aa == "") {
        $("#divBack").html("<a href='/Invoices/IndexBeta' class='pull-right btnbackbox' data-toggle='tooltip' data-placement='bottom' title='Back'><i class='fa fa-chevron-left' aria-hidden='true'></i> Back</a>");
        $("#divCancel").html("<a href='/Invoices/IndexBeta' class='buttonbox btncancel' data-toggle='tooltip' data-placement='bottom' title='Cancel'>Cancel</a>");
    }
    else if (aa == "InvoiceReport") {
        $("#divBack").html("<a href='/InvoiceReport/Index' class='pull-right btnbackbox' data-toggle='tooltip' data-placement='bottom' title='Cancel'><i class='fa fa-chevron-left' aria-hidden='true'></i> Back</a>");
        $("#divCancel").html("<a href='/InvoiceReport/Index' class='buttonbox btncancel' data-toggle='tooltip' data-placement='bottom' title='Cancel'>Cancel</a>");
    }

}

function CheckExistence() {
    var vid = $("#VendorId").val();
    var invoiceno = $("#Invoice_Number").val();

    var val = $("#Invoice_Number").val();
    var reg = /^0*/gi;
    if (val.match(reg)) {
        invoiceno = val.replace(reg, '');
    }
    var invoicedate = $("#InvoiceDate").val();
    var type = $("#Type_id").val();
    var storeid = $("#StoreId").val();
    var invoiceid = $("#InvoiceId").val();
    var totalamtvalue = $('#spanamount').html().replace("$", "").replace(",", "").trim();

    $.ajax({
        url: ROOTURL + '/Invoices/CheckExistence',
        beforeSend: function () { Loader(1); },
        data: { vendorid: vid, invoiceno: invoiceno, invoicedate: invoicedate, type: type, storeid: storeid, invoiceid: invoiceid, totalamtvalue: totalamtvalue },
        async: false,
        success: function (data) {
            $("#divdisplayduplicateinvoice").hide();
            var obj = jQuery.parseJSON(data);
            if (obj.result != "") {
                //if (obj.result == "sub") {
                //    event.preventDefault();
                //    $("#divdisplayduplicateinvoice").show();
                //    var output = "<ul style='list-style-type: none;'>";
                //    for (var i in obj.data) {
                //        output += "<li><a class='popupcontent' target='_blank' href=' " + ROOTURL + '/Invoices/Details/' + obj.data[i].InvoiceId + "'>" + obj.data[i].InvoiceNumber + " - " + obj.data[i].InvoiceDate + "</li>";
                //    }
                //    output += "</ul>";
                //    $("#divdisplayduplicateinvoice").html(output)
                //    //showpopupExist();
                //    $('#popupcheckExistsagain').show();
                //    $('#frm1').submit(false);
                //}
                if (obj.result == "sub") {
                    event.preventDefault();
                    $("#divdisplayduplicateinvoice").show();
                    var output = "<ul style='list-style-type: none;'>";
                    //for (var i in obj.data) {
                    //    output += "<li><a  class='popupcontent'  target='_blank' href=' " + ROOTURL + '/Invoices/Details/' + obj.data[i].InvoiceId + "'>" + obj.data[i].InvoiceNumber + " - " + obj.data[i].InvoiceDate + "</li>";
                    //}
                    output += "<li><a  class='popupcontent'  target='_blank' href=' " + ROOTURL + '/Invoices/Details/' + obj.data.idval + "'>" + obj.data.InvoiceNumber + " - " + obj.data.InvDate + "</li>";
                    output += "</ul>";
                    $("#divdisplayduplicateinvoice").html(output);
                    //showpopupExist();
                    $('#popupcheckExistsagain').show();
                    $('#frm1').submit(false);
                }
                else if (obj.result == "main") {
                    event.preventDefault();

                    $("#hrefopeninvoice").attr("href", ROOTURL + '/Invoices/Details/' + obj.data.idval)
                    //showpopupnew();
                    $('#popupExists').show();
                    $('#frm1').submit(false);
                }
                else {
                    document.getElementById('frm1').submit();
                }
            }
            else {
                document.getElementById('frm1').submit();
            }
            Loader(0);

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".loading-containers").attr("style", "display:none");
            Loader(0);
        }
    });
}

var js = 1;
function DisableKeepandSave() {
    $("#CheckDup").val(js++);
    if ($("#btnPopupKeepandSave").attr("disabled") == undefined) {
        $("#btnPopupKeepandSave").attr("disabled", true);
    }
    if ($("#btnPopupKeepandSave").attr("disabled") == "disabled") {
        document.getElementById('frm1').submit();
    }
}

function KeepandSave() {
    document.getElementById('frm1').submit();
}

function OpenAnotherInvoicelist() {
    //    showDuplicateInvoicepopup();
    $('#popupDisplayduplicateinvoice').show();
}

function SubDuplicatepopupclose() {
    $("#popupcheckExistsagain").hide();
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
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

function GetStoreWiseCRApprovalRights() {
    var StoreId = $("#StoreId").val();
    $.ajax({
        type: "POST",
        url: ROOTURL + "/Invoices/GetroleForJSApproval?Role=nnfapprovalCrdMemoInvoice&StoreId=" + StoreId + "&ModuleID=1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response == "False") {
                $("#QuickCR").val(0);
                $("#divNoNeedForCRApproval").hide();
            }
            else {
                $("#QuickCR").val(1);
                if ($('#Type_id').val() == "1" && ($("#discountype").val() == "2" || $("#discountype").val() == "3")) {
                    $("#divNoNeedForCRApproval").show();
                }
                else {
                    $("#divNoNeedForCRApproval").hide();
                }
            }
            $("#QuickCRInvoice").prop('checked', false);
        },
        failure: function (response) {
            $("#QuickCR").val(0);
            $("#divNoNeedForCRApproval").hide();
        }
    });
}

$("#UploadInvoice").change(function () {
    if (this.files.length > 0) {
        $("#ifrm").hide()
    }
});

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

//Disable paste
$(".numericOnly").bind("paste", function (e) {
    e.preventDefault();
});

$(".numericOnly").bind('mouseenter', function (e) {
    var val = $(this).val();
    if (val != '0') {
        val = val.replace(/[^0-9]+/g, "")
        $(this).val(val);
    }
});

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function ReplaceData(html_val, c_val, r_val) {

    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

//Type addmore starts
function AddField(curIndex) {
    $(".myval").select2('destroy');
    var lastVal = $("#hdninvoicecount").val();
    var nextVal = parseInt(lastVal) + 1;

    var html = $("#trrow_" + lastVal).html();//.replaceWith("_"+val,"_"+(val+1));

    if (html == undefined) {
        lastVal = ($(".tbladdoncls").find(".trrowcls").last().attr("id")).replace("trrow_", "");
        html = $("#trrow_" + lastVal).html();
    }

    if ($("#hdnreqval").val() == 1) {

        html = ReplaceData(html, "drpcls valid", "drpcls input-validation-error");

    }
    html = ReplaceData(html, "_" + lastVal, "_" + nextVal);

    $("#trrow_" + lastVal).after("<div id='trrow_" + nextVal + "'class=trrowcls>" + html + "</div>");

    document.getElementById('aRemove_' + nextVal).setAttribute('onclick', 'RemoveField(' + nextVal + ')')
    document.getElementById('aAdd_' + (nextVal)).setAttribute('onclick', 'AddField(' + (nextVal) + ')')
    $("#hdninvoicecount").val(nextVal);

    $("#trrow_" + (nextVal) + " :input[type='text']").each(function () {
        $(this).val("");
    });
    var elements1 = document.getElementById("Drp_" + (nextVal)).options;
    for (var i = 0; i < elements1.length; i++) {
        elements1[i].selected = false;
    }


    var i_rtn = CheckField();


    $('.groupOfTexbox').keypress(function (event) {
        return isNumber(event, this)
    });
    $('.groupOfTexboxInt').keypress(function (event) {
        return isNumberint(event, this)
    });
    $(".myval").select2({
        width: "50%",
    });

    $("#Amt_" + nextVal).val("");
    $(".trrowcls").last().find("select").focus();
}

function getVendorDepartmentList() {
    var VendorId = $('#VendorId').val();
    if (VendorId != "") {
        $.ajax({
            method: "GET",
            url: ROOTURL + '/Invoices/getVendorDepartmentList',
            data: { VendorId: VendorId },
            success: function (states) {

                var lastVal = 1;
                $(".tbladdoncls .trrowcls").each(function () {

                    var nextVal = parseInt(lastVal) + 1;
                    lastVal = nextVal;
                });
                CheckField();
                AddAmt();
                Calculatediscount();

                $(".myval").select2('destroy');
                if (states.length != 1) {

                    for (var j = 0; j < states.length - 1; j++) {

                        var nextVal = parseInt(lastVal) + 1;

                        var html = $("#trrow_" + lastVal).html();

                        if (html == undefined) {
                            lastVal = ($(".tbladdoncls").find(".trrowcls").last().attr("id")).replace("trrow_", "");
                            html = $("#trrow_" + lastVal).html();
                        }

                        if ($("#hdnreqval").val() == 1) {
                            html = ReplaceData(html, "drpcls valid", "drpcls input-validation-error");
                        }
                        html = ReplaceData(html, "_" + lastVal, "_" + nextVal);

                        $("#trrow_" + lastVal).after("<div id='trrow_" + nextVal + "'class=trrowcls>" + html + "</div>");

                        document.getElementById('aRemove_' + nextVal).setAttribute('onclick', 'RemoveField(' + nextVal + ')')
                        document.getElementById('aAdd_' + (nextVal)).setAttribute('onclick', 'AddField(' + (nextVal) + ')')
                        $("#hdninvoicecount").val(nextVal);

                        $("#trrow_" + (nextVal) + " :input[type='text']").each(function () {
                            $(this).val("");
                        });

                        var elements1 = document.getElementById("Drp_" + (nextVal)).options;
                        for (var i = 0; i < elements1.length; i++) {

                            elements1[i].selected = false;
                        }

                        var i_rtn = CheckField();

                        $('.groupOfTexbox').keypress(function (event) {
                            return isNumber(event, this)
                        });
                        $('.groupOfTexboxInt').keypress(function (event) {
                            return isNumberint(event, this)
                        });

                        $("#Drp_" + lastVal).val(states[j]);
                        lastVal = nextVal;

                        $("#Drp_" + nextVal).val(states[j + 1]);

                    }
                }
                else {

                    for (var j = 0; j < states.length; j++) {
                        $("#Drp_" + 1).val(states[j]);

                    }
                }
                $(".trrowcls").last().find("select").focus();
                Select2Load();

            }
        });
    }
}

function Select2Load() {
    $(".myval").select2();
    $(".setAmount").val("");
}

//for invoiceautomationedit
function getVendorDepartmentListAutomation() {
    var VendorId = $('#VendorId').val();
    if (VendorId != "") {
        $.ajax({
            method: "GET",
            url: ROOTURL + '/Invoices/getVendorDepartmentList',
            data: { VendorId: VendorId },
            success: function (states) {

                var lastVal = 1;
                $(".tbladdoncls .trrowcls").each(function () {

                    var nextVal = parseInt(lastVal) + 1;
                    lastVal = nextVal;
                });
                CheckField();
                AddAmt();
                Calculatediscount();

                $(".myval").select2('destroy');
                if (states.length != 1) {

                    for (var j = 0; j < states.length - 1; j++) {

                        var nextVal = parseInt(lastVal) + 1;

                        var html = $("#trrow_" + lastVal).html();

                        if (html == undefined) {
                            lastVal = ($(".tbladdoncls").find(".trrowcls").last().attr("id")).replace("trrow_", "");
                            html = $("#trrow_" + lastVal).html();
                        }

                        if ($("#hdnreqval").val() == 1) {
                            html = ReplaceData(html, "drpcls valid", "drpcls input-validation-error");
                        }
                        html = ReplaceData(html, "_" + lastVal, "_" + nextVal);

                        $("#trrow_" + lastVal).after("<div id='trrow_" + nextVal + "'class=trrowcls>" + html + "</div>");

                        document.getElementById('aRemove_' + nextVal).setAttribute('onclick', 'RemoveField(' + nextVal + ')')
                        document.getElementById('aAdd_' + (nextVal)).setAttribute('onclick', 'AddField(' + (nextVal) + ')')
                        $("#hdninvoicecount").val(nextVal);

                        $("#trrow_" + (nextVal) + " :input[type='text']").each(function () {
                            $(this).val("");
                        });

                        var elements1 = document.getElementById("Drp_" + (nextVal)).options;
                        for (var i = 0; i < elements1.length; i++) {

                            elements1[i].selected = false;
                        }

                        var i_rtn = CheckField();

                        $('.groupOfTexbox').keypress(function (event) {
                            return isNumber(event, this)
                        });
                        $('.groupOfTexboxInt').keypress(function (event) {
                            return isNumberint(event, this)
                        });

                        $("#Drp_" + lastVal).val(states[j]);
                        lastVal = nextVal;

                        $("#Drp_" + nextVal).val(states[j + 1]);

                    }
                }
                else {

                    for (var j = 0; j < states.length; j++) {
                        $("#Drp_" + 1).val(states[j]);

                    }
                }
                $(".trrowcls").last().find("select").focus();
                $(".myval").select2();

            }
        });
    }
}

function RemoveField(val) {

    $("#trrow_" + val).remove();

    CheckField();
    AddAmt();
    Calculatediscount();
}

function CheckField() {

    var i = 0;
    i = $(".tbladdoncls .trrowcls").length;
    if (i == 1) {
        $(".aremove").hide();
    }
    else {
        $(".aremove").show();
    }
    return i;
}

//AddAmt();
function AddAmt() {
    var Invoicetype = $('#Type_id').val();

    if (Invoicetype == "2") {
        $('.spanamount').show()
        $('.spanamount').addClass("spanamountBackcolor");
    }
    else {
        $('.spanamount').show()
    }

    var temp = 0;
    var numItems = $('.addamtcls').each(function () {
        if ($(this).val() != '') {
            temp += parseFloat($(this).val());
        }
        else {

        }
    });

    const formatter = new Intl.NumberFormat('en-US', {
        currency: 'USD',
        minimumFractionDigits: 2
    })
    var n = temp.toFixed(2);
    var totalamount = "$ " + formatter.format(n);
    $("#TotalAmount").val(n);
    $("#spanamount").html(totalamount);
    Calculatediscount();
}

function RemoveAmt(val) {

    var finaltotal = 0;
    var Totalamt = $('#spanamount').html();

    var temp = document.getElementById('Amt_' + (val)).value;

    var total = Totalamt.split("$").pop();

    finaltotal = total - temp;
    var totalamount = "$" + (finaltotal);

    $("#TotalAmtByDept").val(totalamount);
    $("#spanamount").html(totalamount);
}

function Duplicatepopupclose() {
    $("#popupExists").hide();
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
}

function ddListPaymentTypeChange(paytype) {
    if (paytype != "") {
        $.ajax({
            url: '/Invoices/getInvoiceType',
            type: "GET",
            dataType: "JSON",
            data: { paytype: paytype },
            success: function (states) {

                $("#Type_id").html("");
                if (paytype != "1") {
                    $('select#Type_id').html('<option value="">--Select--</option>');
                }
                $.each(states, function (data, value) {
                    $("#Type_id").append(
                        $('<option></option>').val(value.InvoiceTypeId).html(value.InvoiceType));
                });
            }
        });
    }
    if (paytype == 1) {
        $('#Type_id').val([1]).trigger('change');
    }
    else {
        $('#Type_id').removeAttr("disabled");
        $('#Type_id').val([""]).trigger("change");
        var typeid = $('#Type_id').val();
    }
}

function ddLTypeChange(paytype) {
    $("#QuickCRInvoice").prop('checked', false);
    if (paytype == "2") {
        $("#divdiscountlabel").hide();
        $("#divdiscount").hide();
        $('.spncredit').show()
        $('.spanamount').show();
        if ($('#spanamount').text().trim() != "") {

            $('.spanamount').addClass("spanamountBackcolor");
        }
        else {
            $('.spanamount').removeClass("spanamountBackcolor");
        }
        $("#SelectDeptForDisc").hide();
        $("#divNoNeedForCRApproval").hide();
    }
    else {
        $("#divdiscountlabel").show();
        $("#divdiscount").show();
        $('.spncredit').hide();
        $('.spanamount').show();
        $('.spanamount').removeClass("spanamountBackcolor");
        if ($("#discountype").val() != 2 && $("#discountype").val() != 3) {
            $("#SelectDeptForDisc").hide();
            $("#divNoNeedForCRApproval").hide();
        }
        else {
            $("#SelectDeptForDisc").show();
            if ($("#QuickCR").val() == "1") {
                $("#divNoNeedForCRApproval").show();
            }
            else {
                $("#divNoNeedForCRApproval").hide();
            }
        }
    }

    $("#discountype").val("1").change();
    $("#Disc_Drp_1").val(0);
    var type = $("#Type_id").val();
    $('#Type').val(type);
}

$('#Vendorname').change(function () {
    var vid = document.getElementById("Vendorname").value;
    BindAddress(vid);
});

function BindAddress(vid) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: '/Invoices/BindVendorAddress',
        data: { vid: vid },
        success: function (Data) {
            $("#divvendoraddress").attr("style", "display:block;");
            $("#addresss").empty();
            if (Data.Address != null && Data.Address != "") {
                $("#addresss").append(Data.Address);
            }
            else {
                $("#addresss").append("Not Available");
            }
            var strspn = '';
            if (Data.SynthesisStatus != null && Data.SynthesisStatus != "" && Data.QBStatus != null && Data.QBStatus != "") {
                if (Data.SynthesisStatus == "Active" && Data.QBStatus == "Active") {
                    $("#spnActive").removeClass("synthesisOnlyActive").addClass("bothActive");
                    strspn = "<span class='bothActive' id='spnActive'></span>";
                }
                else {
                    $("#spnActive").removeClass("bothActive").addClass("synthesisOnlyActive");
                    strspn = "<span class='synthesisOnlyActive' id='spnActive'></span>";
                }
            }
            else {
                $("#spnActive").removeClass("bothActive").addClass("synthesisOnlyActive");
                strspn = "<span class='synthesisOnlyActive' id='spnActive'></span>";
            }
            $("#divvendorphone").attr("style", "display:block;");
            $("#PhoneNumber").empty();
            if (Data.PhoneNumber != null && Data.PhoneNumber != "") {
                $("#PhoneNumber").append(Data.PhoneNumber);
            }
            else {
                $("#PhoneNumber").append("Not Available");
            }
            $("#ToolTipInstuctions").empty();
            if (Data.Instruction != null && Data.Instruction != "") {
                $("#ToolTipInstuctions").append("<label>Vendor Name</label><span class='highlight' style='margin-right: auto;'>*</span>" + strspn + "<span class='InvoiceTool'>" + Data.Instruction + "</span>");
            }
            else {
                $("#ToolTipInstuctions").append("<label>Vendor Name</label><span class='highlight' style='margin-right: auto;'>*</span>" + strspn + "<span class='InvoiceTool'>Instructions</span>");
            }
            $(".suggetion").hide();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
    $(".loading-containers").attr("style", "display:none");
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
}

$('#UploadInvoice').change(function () {
    var i = $(this).prev('label').clone();
    var file = $('#UploadInvoice')[0].files[0].name;
    $(this).prev('label').text(file);
});

var uploadField = document.getElementById("UploadInvoice");

uploadField.onchange = function () {

    if (this.files[0].size > 30971520) {
        $("#popupSize").show();
        this.value = "";
    };
};

function Binddiscount() {
    var discounttype = $("#discountype").val();
    $("#QuickCRInvoice").prop('checked', false);
    if (discounttype === "2") {
        $("#divpercentage").show();
        $("#divdiscountamt").show();
        $("#Discount").val("");
        $("#DiscountAmount").val("");
        $("#divper").show();
        $("#divdolar").show();
        $('#DiscountAmount').attr('readonly', true);
        $('#SelectDeptForDisc').show();
        if ($("#QuickCR").val() == "1") {
            $("#divNoNeedForCRApproval").show();
        }
        else {
            $("#divNoNeedForCRApproval").hide();
        }
    }
    else if (discounttype === "3") {
        $("#divpercentage").hide();
        $('#DiscountAmount').attr('readonly', false);
        $("#divper").hide();
        $("#divdolar").show();
        $("#Discount").val("");
        $("#divdiscountamt").show();
        $("#DiscountAmount").val("");
        $('#SelectDeptForDisc').show();
        if ($("#QuickCR").val() == "1") {
            $("#divNoNeedForCRApproval").show();
        }
        else {
            $("#divNoNeedForCRApproval").hide();
        }
    } else {
        $("#divpercentage").hide();
        $("#divdiscountamt").hide();
        $("#DiscountAmount").val("");
        $("#divper").hide();
        $("#divdolar").hide();
        $("#Discount").val("");
        $("#DiscountAmount").val("");
        $('#DiscountAmount').attr('readonly', false);
        $('#SelectDeptForDisc').hide();
        $("#divNoNeedForCRApproval").hide();
    }

}

function Calculatediscount() {
    var percentage = $("#Discount").val();
    var totalamount = $("#TotalAmount").val();
    var DiscountAmount = $("#DiscountAmount").val();
    if (parseFloat(DiscountAmount) > parseFloat(totalamount)) {
        $("#DiscountAmount").val("");
    }
    if (parseFloat(percentage) > 0) {
        if (parseFloat(percentage) > 100) {
            $("#Discount").val("");
            $("#DiscountAmount").val("");
        }
        else {
            DiscountAmount = parseFloat(percentage) * parseFloat(totalamount) / 100;
            $("#DiscountAmount").val(parseFloat(DiscountAmount).toFixed(2));
        }

    }
}

function validatedrptext() {
    $(".loading-containers").attr("style", "display:block");
    $(".headermainbox").attr("style", "z-index:0");
    $(".page-sidebar").attr("style", "z-index:0");
    $('.drpcls').each(function () {
        if ($(this).val() === null || $(this).val() === '') {
            $(this).addClass('input-validation-error');
            event.preventDefault();
            $(".loading-containers").attr("style", "display:none");
            $(".headermainbox").attr("style", "z-index:0");
            $(".page-sidebar").attr("style", "z-index:0");
            return false;
        }
        else {
            $(this).removeClass('input-validation-error');
            $(this).addClass('input-validation-valid');
        }
    });

    if ($("#SelectDeptForDisc").is(':visible')) {
        if ($("#Disc_Drp_1").val() === null || $("#Disc_Drp_1").val() === '') {
            $("#Disc_Drp_1").addClass('input-validation-error');
            event.preventDefault();
            $(".loading-containers").attr("style", "display:none");
            $(".headermainbox").attr("style", "z-index:0");
            $(".page-sidebar").attr("style", "z-index:0");
            return false;
        }
        else {

            $("#Disc_Drp_1").removeClass('input-validation-error');
            $("#Disc_Drp_1").addClass('input-validation-valid');
        }
    }
    var i = $(".tbladdoncls .trrowcls").length;
    var M = 0;
    var k = 0;
    var Amount;
    var Department;
    for (var j = 1; j <= i+10; j++) {
        if (M >= i) {
            break;
        }
        Amount = $("#Amt_" + j).val();
        Department = $("#Drp_" + j).val();

        if (Amount == "") {
            k++
        }
        if (!(Amount == undefined && Department == undefined)) {
            M++;
        }
        if (Amount != "" && Amount != undefined && Amount != 0 && Amount != null) {
            if (Department == "" || Department == undefined || Department == null || Department == 0) {
                $('#popupDepartment').show();
                return false;
            }
            else {
                $('#popupDepartment').hide();
            }
        }
        else {
            $('#popupDepartment').hide();
        }
        //if (Amount == "" || Department == "" || Amount == undefined || Department == undefined || Amount == null || Department == null || Amount == 0 || Department == 0) {
        //    $('#popupDepartment').show();
        //    return false;
        //}
        //else {
        //    $('#popupDepartment').hide();
        //}
    }


    if ($("#frm1").valid()) {
        CheckExistence();
        $(".loading-containers").attr("style", "display:block");
        $(".headermainbox").attr("style", "z-index:0");
        $(".page-sidebar").attr("style", "z-index:0");
    }
    else {
        event.preventDefault();
        $(".loading-containers").attr("style", "display:none");
    }
}

function changeHeight() {
    $('#right_panel').toggleClass('active');
    if (!$("#right_panel").hasClass('active')) {
        $("#right_panel").addClass("button_bottom")
        $(".page-sidebar.sidebar-fixed").attr("style", "z-index:99999 !important");
    } else {
        $("#right_panel").removeClass("button_bottom")
        $(".page-sidebar.sidebar-fixed").attr("style", "z-index:0 !important");
    }
    $('#pdfSection').toggleClass('active');
}

function getDepartmentList() {
    var storeid = $("#StoreId").val();
    if (storeid != "") {
        $.ajax({
            url: '/Invoices/getDepartmentList',
            type: "GET",
            dataType: "JSON",
            data: { StoreId: storeid },
            success: function (states) {
                $("#Disc_Drp_1").html("");
                $('select#Disc_Drp_1').html('<option value="">--Select--</option>');

                $.each(states, function (data, value) {
                    $("#Disc_Drp_1").append(
                        $('<option></option>').val(value.DepartmentId).html(value.DepartmentName));

                });
            }
        });
    }
}