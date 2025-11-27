var js = 1;
var top = 0;
var uploadField = document.getElementById("UploadInvoice");

function Setvalues() {
    localStorage.setItem("FromInvoicePage", "");
}

function getVendorList() {
    var StoreId = $('#Storeid').val();
    if (StoreId != "") {
        $.ajax({
            url: '/Invoices/getVendorList',
            type: "GET",
            dataType: "JSON",
            data: { StoreId: StoreId },
            success: function (states) {

                $("#VendorId").html("");
                $('select#VendorId').html('<option value="">--Select--</option>');
                $.each(states, function (data, value) {
                    $("#VendorId").append(
                        $('<option></option>').val(value.VendorId).html(value.VendorName));
                });
            }
        });
    }
    getDepartmentList();
}

function drpStoreselect() {
    getVendorList();
    getDepartmentList();
    PaymentTypeList();
    GetStoreWiseApprovalRights();
    GetStoreWiseCRApprovalRights();
}

function GetStoreWiseApprovalRights() {
    var StoreId = $('#Storeid').val();
    $.ajax({
        type: "POST",
        url: ROOTURL + "/Invoices/GetroleForJSApproval?Role=nnfapprovalInvoice&StoreId=" + StoreId + "&ModuleID=1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if ($('#divNoNeedForApproval').length) {
                if (response == "False") {
                    $("#divNoNeedForApproval").css('display', 'none');
                }
                else {
                    $("#divNoNeedForApproval").css('display', 'block');
                }
                $("#QuickInvoice").prop('checked', false);
            }
        },
        failure: function (response) {
            $("#divNoNeedForApproval").css('display', 'none');
        }
    });
}

function GetStoreWiseCRApprovalRights() {
    var StoreId = $('#Storeid').val();
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
                if ($('#Type_id').val() == "1") {
                    $("#divNoNeedForCRApproval").show();
                }
                else if ($("#discountype").val() == "2" || $("#discountype").val() == "3") {
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

function PaymentTypeList() {
    var Store_idval = $("#Storeid").val();
    $(".select2-container").remove();
    $('#Payment_type').html(null);
    $("#Payment_type").append("<option selected='selected' value=''>Select Payment Method</option>");

    $.getJSON(ROOTURL + '/Invoices/BindPaymentType/?Store_idval=' + Store_idval, function (data) {
        $.each(data, function (i, LstVendor) {
            $("#Payment_type").append(
                "<option value=" + LstVendor.Value + ">" + LstVendor.Text + "</option>");
        });
        if (Store_idval != 11) {
            $("#Payment_type").val(2).trigger('change');;
        }
        else {
            $("#Payment_type").prop("selectedIndex", 0).val();
        }
    });

    $("select").select2({
        width: "100%",
        formatResult: function (state) {
            if (!state.id) return state.text;
            if ($(state.element).data('active') == "0") {
                return state.text + "<i class='fa fa-dot-circle-o'></i>";
            } else {
                return state.text;
            }
        },
        formatSelection: function (state) {
            if ($(state.element).data('active') == "0") {
                return state.text + "<i class='fa fa-dot-circle-o'></i>";
            } else {
                return state.text;
            }
        }
    });
}

function getDepartmentList() {
    var StoreId = $('#Storeid').val();
    if (StoreId != "") {
        $.ajax({
            url: '/Invoices/getDepartmentList',
            type: "GET",
            dataType: "JSON",
            data: { StoreId: StoreId },
            success: function (states) {
                $("#Disc_Drp_1").html("");
                $('select#Disc_Drp_1').html('<option value="">--Select--</option>');

                $("#Drp_1").html("");
                $('select#Drp_1').html('<option value="">--Select--</option>');
                $.each(states, function (data, value) {
                    $("#Disc_Drp_1").append(
                        $('<option></option>').val(value.DepartmentId).html(value.DepartmentName));

                    $("#Drp_1").append(
                        "<option value=" + value.DepartmentId + ">" + value.DepartmentName + "</option>")
                });
            }
        });
    }
}

function CreateNew() {
    validatedrptextNew(1);
    $('#flagaddnew').val(1);
}

function validatedrptextNew(myVald) {


    if (myVald == 1) {
        var scaninvoice = $("#UploadInvoice").val();
        if (UploadInvoice != '') {
            scaninvoice = document.getElementById("UploadInvoice").defaultvalue;
        }
        if (scaninvoice == "") {
            $('#popupid').show();
            return false;
        }
        else {
            $('#popupid').hide();
        }
        var i = $(".tbladdoncls .trrowcls").length;
        var k = 0;
        var Amount;
        for (var j = 1; j <= i; j++) {
            Amount = $("#Amt_" + j).val();
            if (Amount == "") {
                k++
            }
            //if (Amount == "0") {
            //    k++
            //}
        }
        if (k == i) {
            $('#popupDepartment').show();
            return false;
        }
        else {
            $('#popupDepartment').hide();
        }
    }

}

$(document).ready(function () {



    $(".decimalOnly").bind('keypress', function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
            return false;

        return true;
    });

    $('#Vendor_id').change(function () {
        var vid = document.getElementById("Vendor_id").value;
    });


    $(".decimalOnly").bind("paste", function (e) {
        e.preventDefault();
    });


    var today = document.getElementById("CurrentDate").value;
    

    

    $(".decimalOnly").bind('keypress', function isNumberKey(evt) {

        var charCode = (evt.which) ? evt.which : event.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
            return false;

        return true;
    });
    $(".decimalOnly").bind("paste", function (e) {
        e.preventDefault();
    });

    $(function () {
        $(".setAmount").val("0");
    });

    if (isValid == "true") {
        drpStoreselect();
    }

    uploadField.onchange = function () {
        if (this.files[0].size > 30971520) {
            $("#popupSize").show();
            this.value = "";
        };
    };

});

function Duplicatepopupclose() {
    
    $("#popupExists").hide();
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
}

function SubDuplicatepopupclose() {
    $("#popupcheckExistsagain").hide();
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
}

function DisplayDuplicatepopupclose() {
    $("#popupDisplayduplicateinvoice").hide();
}

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
                    if (lastVal == 1) {
                        $("#Drp_1").val("");
                        $("#Amt_1").val("");

                    }
                    else {
                        $("#trrow_" + lastVal).remove();

                    }
                    var nextVal = parseInt(lastVal) + 1;
                    lastVal = nextVal;
                });
                CheckField();
                AddAmt();
                Calculatediscount();
                lastVal = 1;
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


                        /*elements1[1].selected = true;*/

                        //var elements2 = document.getElementById("Drp_" + (lastVal)).options;
                        //elements2.selected = true;

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
                } else {
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
    //    $(".setAmount").val("0");
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

function AddAmt() {
    var Invoicetype = $('#Type_id').val();
    if (Invoicetype == "2") {
        $('.spanamount').show();
        $('.spanamount').addClass("spanamountBackcolor");
    }
    else {
        $('.spanamount').removeClass("spanamountBackcolor");
        $('.spanamount').show();
    }

    var temp = 0;
    var numItems = $('.addamtcls').each(function () {
        if ($(this).val() != '') {
            temp += parseFloat($(this).val());
        }
    });
    const formatter = new Intl.NumberFormat('en-US', {
        currency: 'USD',
        minimumFractionDigits: 2
    })
    var n = temp.toFixed(2);
    var totalamount = "$ " + formatter.format(n);
    //alert(n);
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

function validatedrptext(myVald, Name) {
    if (Name == "Save") {
        $("#btnName").val(Name);
    }
    if (Name == "Save & New") {
        $("#btnName").val(Name);
    }
    var Flg = 0;
    $('.drpcls').each(function () {
        if ($(this).val() == null || $(this).val() == "") {
            Flg = 1;
            $(this).addClass('input-validation-error');
            event.preventDefault();
        }
        else {

            $(this).removeClass('input-validation-error');
            $(this).addClass('input-validation-valid');
        }
    });
    if ($("#InvoiceDate").val() === null || $("#InvoiceDate").val() === '') {
        $("#InvoiceDate").addClass('input-validation-error');
        event.preventDefault();
    }
    else {

        $("#InvoiceDate").removeClass('input-validation-error');
        $("#InvoiceDate").addClass('input-validation-valid');
    }
    if (myVald == 1) {
        var scaninvoice = $("#UploadInvoice").val();
        if (UploadInvoice != '') {
            scaninvoice = document.getElementById("UploadInvoice").defaultvalue;
        }

        if (scaninvoice == "") {
            $('#popupid').show();
            return false;
        }
        else {
            $('#popupid').hide();
        }

        var i = $(".tbladdoncls .trrowcls").length;
        var k = 0;
        var M = 0;
        var Amount;
        var Department;
        for (var j = 1; j <= i; j++) {
            if (M >= i) {
                break;
            }
            Amount = $("#Amt_" + j).val();
            Department = $("#Drp_" + j).val();
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
            //if (Amount == "") {
            //    k++
            //}
            //if (Amount == "0") {
            //    k++
            //}
            //if (Department == "" ||Department == undefined ||Department == null || Department == 0) { 
            //    $('#popupDepartment').show();
            //    return false;
            //}
            //else {
            //    $('#popupDepartment').hide();
            //}
        }
        
    }
    $("#hdnreqval").val("1");
    if (Flg == 0) {
        if ($("#frmMain").valid()) {

            CheckExistence();
            $("#btnInvoiceSubmit").attr("disabled", true);
            $("#btnNewInvoiceSubmit").attr("disabled", true);
            $(".loading-containers").attr("style", "display:block");
            $(".headermainbox").attr("style", "z-index:0");
            $(".page-sidebar").attr("style", "z-index:0");
        }
        else {
            event.preventDefault();
            $("#btnInvoiceSubmit").attr("disabled", false);
            $("#btnNewInvoiceSubmit").attr("disabled", false);
        }
    }
    else {
        event.preventDefault();
    }


}

function CheckExistence() {
    var vid = $("#VendorId").val();
    var invoiceno = $("#InvoiceNumber").val();
    var val = $("#InvoiceNumber").val();
    var reg = /^0*/gi;
    if (val.match(reg)) {
        invoiceno = val.replace(reg, '');
    }
    var invoicedate = $("#InvoiceDate").val();
    var type = $("#Type_id").val();
    var storeid = $("#Storeid").val();
    var totalamtvalue = $('#spanamount').html().replace("$", "").replace(",", "").trim();
    $.ajax({
        url: ROOTURL + '/Invoices/CheckExistence',
        beforeSend: function () { Loader(1); },
        data: { vendorid: vid, invoiceno: invoiceno, invoicedate: invoicedate, type: type, storeid: storeid, totalamtvalue: totalamtvalue },
        /*async: false,*/
        success: function (data1) {
            $("#divdisplayduplicateinvoice").hide();
            var obj = jQuery.parseJSON(data1);
            if (obj.result != "") {
               
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
                    $('#frmMain').submit(false);
                }
                else if (obj.result == "main") {
                    event.preventDefault();
                    $("#hrefopeninvoice").attr("href", ROOTURL + '/Invoices/Details/' + obj.data.idval)
                    /*showpopupnew();*/
                    $('#popupExists').show();
                    $('#frmMain').submit(false);
                }
                else {
                    document.getElementById('frmMain').submit();
                }
            }
            else {
                document.getElementById('frmMain').submit();
            }

            Loader(0);
        }
    });
}

$(document).ready(function () {
    var today = document.getElementById("CurrentDate").value;
    
    GetStoreWiseCRApprovalRights();
    BinddiscountData();

    $('#frmMain').attr('autocomplete', 'off');
    CheckField();

    $('.groupOfTexbox').keypress(function (event) {
        return isNumber(event, this);
    });
    $('.groupOfTexboxInt').keypress(function (event) {
        return isNumberint(event, this);
    });
    GetStoreWiseApprovalRights();
});

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
        //$('#Type_id').attr("disabled", "true");
    }
    else {
        $('#Type_id').removeAttr("disabled");
        $('#Type_id').val(['']).trigger('change');
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
        $("#divNoNeedForCRApproval").hide();
        $("#SelectDeptForDisc").hide();
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
    var type = $("#Type_id").val();
    $('#Type').val(type);
}

function BindAddress(vid) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: '/Invoices/BindVendorAddress',
        data: { vid: vid },
        success: function (Data) {
            $("#divvendoraddress").attr("style", "display:block;");
            //$("#divvendorQBStatus").attr("style", "display:block;");
            $("#addresss").empty();
            //$("#VendorQBStatus").empty();
            //$("#VendorSynthesisStatus").empty();
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
            //else {
            //    $("#VendorSynthesisStatus").append("");
            //}
            //if (Data.QBStatus != null && Data.QBStatus != "") {
            //    $("#VendorQBStatus").append(Data.QBStatus);
            //}
            //else {
            //    $("#VendorQBStatus").append("");
            //}
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
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}




// THE SCRIPT THAT CHECKS IF THE KEY PRESSED IS A NUMERIC OR DECIMAL VALUE.
function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (
        (charCode != 45 || $(element).val().indexOf('-') != -2) &&      // “-” CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -1)    // “.” CHECK DOT, AND ONLY ONE.
        && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function isNumberint(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (
        (charCode != 45 || $(element).val().indexOf('-') != -2) &&      // “-” CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -2) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function closemodal() {
    $(".divIDClass").hide();
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
}

function Binddiscount() {
    var discounttype = $("#discountype").val();
    $("#QuickCRInvoice").prop('checked', false);
    if (discounttype === "2") {
        $("#divpercentage").show();
        $("#divdiscountamt").show();
        $("#Discount").val("");
        $("#Discountamount").val("");
        $("#divper").show();
        $("#divdolar").show();
        $('#Discountamount').attr('readonly', true);
        $('#SelectDeptForDisc').show();
        $("#Disc_Drp_1").val("");
        if ($("#QuickCR").val() == "1") {
            $("#divNoNeedForCRApproval").show();
        }
        else {
            $("#divNoNeedForCRApproval").hide();
        }
    }
    else if (discounttype === "3") {
        $("#divpercentage").hide();
        $('#Discountamount').attr('readonly', false);
        $("#divper").hide();
        $("#divdolar").show();
        $("#Discount").val("");
        $("#divdiscountamt").show();
        $("#Discountamount").val("");
        $('#SelectDeptForDisc').show();
        $("#Disc_Drp_1").val("");
        if ($("#QuickCR").val() == "1") {
            $("#divNoNeedForCRApproval").show();
        }
        else {
            $("#divNoNeedForCRApproval").hide();
        }
    } else {
        $("#divpercentage").hide();
        $("#divdiscountamt").hide();
        $("#Discountamount").val("");
        $("#divper").hide();
        $("#divdolar").hide();
        $("#Discount").val("");
        $("#Discountamount").val("");
        $('#Discountamount').attr('readonly', false);
        $('#SelectDeptForDisc').hide();
        $("#Disc_Drp_1").val("");
        $("#divNoNeedForCRApproval").hide();
    }
}


function Calculatediscount() {
    var percentage = $("#Discount").val();
    var totalamount = $("#TotalAmount").val();
    var discountamount = $("#Discountamount").val();

    if (parseFloat(discountamount) > parseFloat(totalamount)) {
        $("#Discountamount").val("");
    }
    if (parseFloat(percentage) > 0) {
        if (parseFloat(percentage) > 100) {
            $("#Discount").val("");
            $("#Discountamount").val("");
        } else {
            discountamount = parseFloat(percentage) * parseFloat(totalamount) / 100;
            $("#Discountamount").val(parseFloat(discountamount).toFixed(2));
        }
    }
}

function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}

function getVendorList() {
    var StoreId = $('#Storeid').val();
    if (StoreId != "") {
        $.ajax({
            url: '/Invoices/getVendorList',
            type: "GET",
            dataType: "JSON",
            data: { StoreId: StoreId },
            success: function (states) {
                $("#VendorId").html("");
                $('select#VendorId').html('<option value="">--Select--</option>');
                $.each(states, function (data, value) {
                    $("#VendorId").append(
                        $('<option></option>').val(value.VendorId).html(value.VendorName));
                });
            }
        });
    }
}

function CreateNew() {
    validatedrptextNew(1);
    $('#flagaddnew').val(1);
}
function validatedrptextNew(myVald) {
    if (myVald == 1) {
        var scaninvoice = $("#UploadInvoice").val();
        if (UploadInvoice != '') {
            scaninvoice = document.getElementById("UploadInvoice").defaultvalue;
        }
        if (scaninvoice == "") {
            $('#popupid').show();
            return false;
        }
        else {
            $('#popupid').hide();
        }
        var i = $(".tbladdoncls .trrowcls").length;
        var k = 0;
        var Amount;
        for (var j = 1; j <= i; j++) {
            Amount = $("#Amt_" + j).val();
            if (Amount == "") {
                k++
            }
        }
        if (k == i) {
            $('#popupDepartment').show();
            return false;
        }
        else {
            $('#popupDepartment').hide();
        }
    }
}

function Duplicatepopupclose() {
    $("#popupExists").hide();
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
    $("#btnInvoiceSubmit").attr("disabled", false);
}
function SubDuplicatepopupclose() {
    $("#popupcheckExistsagain").hide();
    $("#btnInvoiceSubmit").attr("disabled", false);
    $("#btnNewInvoiceSubmit").attr("disabled", false);
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
}
function DisplayDuplicatepopupclose() {

    $("#popupDisplayduplicateinvoice").hide();
}
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




function ReplaceData(html_val, c_val, r_val) {

    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

///* END EXTERNAL SOURCE */
function DisableKeepandSave() {
    $("#CheckDup").val(js++);
    if ($("#btnPopupKeepandSave").attr("disabled") == undefined) {
        $("#btnPopupKeepandSave").attr("disabled", true);
    }
    if ($("#btnPopupKeepandSave").attr("disabled") == "disabled") {
        document.getElementById('frmMain').submit();
    }

}
function KeepandSave() {
    document.getElementById('frmMain').submit();
}
//function OpenAnotherInvoicelist() {
//    showDuplicateInvoicepopup();
//}


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
    //$("#tdaction_" + val).hide();

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

    $(".trrowcls").last().find("select").focus();
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
        $('.spanamount').show();
        $('.spanamount').addClass("spanamountBackcolor");
    }
    else {
        $('.spanamount').removeClass("spanamountBackcolor");
        $('.spanamount').show();
    }

    var temp = 0;
    var numItems = $('.addamtcls').each(function () {
        if ($(this).val() != '') {
            temp += parseFloat($(this).val());
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

    $("#TotalAmount").val(totalamount);
    $("#spanamount").html(totalamount);
}
function UserFeedBack() {
    $('#userreviewpopupid').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });
}

function UpdateUserReview() {
    toastr.success('User Feedback Update Successfully.');
}
//function InvoiceInformation() {
//    $("#UserFeedBack").removeClass("Tab");
//    $("#InvoiceInformation").addClass("Tab");
//    $("#2").addClass("hidden");
//    $("#1").removeAttr("hidden");
//}