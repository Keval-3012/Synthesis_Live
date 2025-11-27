if (isValid == "true") {
    GetVendorList();
}
function KeepandSave() {
    
    //if (validatedrptext(1) == false) {
    //    $(".loading-containers").attr("style", "display:none");
    //    $(".headermainbox").removeAttr("style", "z-index:0");
    //    $(".page-sidebar").removeAttr("style", "z-index:0");
    //    return false;
    //}

    if (CheckTotalInvoiceAmount() == false) {
        $(".loading-containers").attr("style", "display:none");
        $(".headermainbox").removeAttr("style", "z-index:0");
        $(".page-sidebar").removeAttr("style", "z-index:0");
        return false;
    }

    if ($("#txtShiftId").val() == "0") {
        MyCustomToster('After shift assign you can create invoice.');
        return false;
    }
    else {
        var formData = new FormData();
        formData.append('PaymentTypeId', '1');
        formData.append('InvoiceTypeId', '1');
        formData.append('Type', '1');
        var your_form = $("#frm2").serializeArray();
        $.each(your_form, function (key, input) {
            formData.append(input.name, input.value);
        });
        var uploadField = document.getElementById("UploadInvoice");
        for (var i = 0; i < uploadField.files.length; i++) {
            formData.append("UploadInvoice", uploadField.files[i]);
        }
        formData.append("TerminalId", $("#txtTerminalId").val());
        formData.append("ShiftID", $("#txtShiftId").val());
        formData.append("PaidOutID", $("#txtPaidOutID").val());
        formData.append("strInvoiceDate", $("#InvoiceDate").val());
        formData.append("StoreId", $("#storeid3").val());
        formData.append("TotalAmount", $("#totalamounrt").val());
        $.ajax({
            url: ROOTURL + '/Terminal/CreateCashInvoice',
            type: 'POST',
            dataType: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response == "Notsuccess") {
                    return false;
                }
                if (response == "success") {

                    window.location.href = ROOTURL + "/Terminal/Index?StoreId=" + $("#storeid3").val() + "&TerminalId= " + $("#txtTerminalId").val() + "&StartDate=" + $("#txtInvDate").val();
                }
            },
            error: function (response) {
                alert("Whooaaa! Something went wrong..");
            }
        });
    }
}
function GetVendorList() {
    var StoreId = $('#storeid3').val();
    if (StoreId != "") {
        $.ajax({
            url: ROOTURL +  '/Invoices/getVendorList',
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
    GetDepartmentList();
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
function GetDepartmentList() {
    var StoreId = $('#storeid3').val();
    if (StoreId != "") {
        $.ajax({
            url: ROOTURL + '/Invoices/getDepartmentList',
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

            if (Data.SynthesisStatus != null && Data.SynthesisStatus != "" && Data.QBStatus != null && Data.QBStatus != "") {
                if (Data.SynthesisStatus == "Active" && Data.QBStatus == "Active") {
                    $("#spnActive").removeClass("synthesisOnlyActive").addClass("bothActive");
                }
                else {
                    $("#spnActive").removeClass("bothActive").addClass("synthesisOnlyActive");
                }
            }
            else {
                $("#spnActive").removeClass("bothActive").addClass("synthesisOnlyActive");
            }
            //if (Data.SynthesisStatus != null && Data.SynthesisStatus != "") {
            //    $("#VendorSynthesisStatus").append(Data.SynthesisStatus);
            //}
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
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function Binddiscount() {
    var discounttype = $("#discountype").val();
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
    }
}

function Calculatediscount() {
    var percentage = $("#Discount").val();
    var totalamount = $("#totalamounrt").val();
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

function Duplicatepopupclose() {
    $("#popupExists").hide();
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
}

function SubDuplicatepopupclose() {
    $("#popupcheckExistsagain").hide();
    $(".loading-containers").attr("style", "display:none");
    $(".headermainbox").removeAttr("style", "z-index:0");
    $(".page-sidebar").removeAttr("style", "z-index:0");
}

function DisplayDuplicatepopupclose() {
    $("#popupDisplayduplicateinvoice").hide();
}

function PaidAmtHighclose() {
    $("#popupPaidAmtHigh").hide();
}

function closemodal() {
    $(".divIDClass").hide();
}

var uploadField = document.getElementById("UploadInvoice");
uploadField.onchange = function () {
    if (this.files[0].size > 30971520) {
        //alert("File is too big!");
        $("#popupSize").show();
        this.value = "";
    };
};

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
        return false;

    return true;
});
$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

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


$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
        return false;

    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

function ReplaceData(html_val, c_val, r_val) {
    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

function AddField(curIndex) {
    //$(".myval").select2('destroy');
    $(".myvalssss").select2('destroy');

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
    $(".myvalssss").select2({
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
            url: ROOTURL +'/Invoices/getVendorDepartmentList',
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
                $(".myvalssss").select2('destroy');
                if (states.length != 1) {
                    for (var j = 0; j < states.length - 1; j++) {
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
    $(".myvalssss").select2();
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
    var Invoicetype = 1;
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

    $("#totalamounrt").val(n);
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
    $("#totalamounrt").val(totalamount);
    $("#spanamount").html(totalamount);
}

function Calculatediscount() {
    var percentage = $("#Discount").val();
    var totalamount = $("#totalamounrt").val();
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


function validatedrptext(myVald) {
    
    var isvalids = true;
    //$('.drpcls').each(function (index)
    //{
    //    if ($(this).val() === null || $(this).val() === '') {
    //        $(this).addClass('input-validation-error');
    //        event.preventDefault();
    //        isvalids = false;
    //    }
    //    else {
    //         $(this).removeClass('input-validation-error');
    //         $(this).addClass('input-validation-valid');
    //    }
    //});

    let a = 1;
    $(".tbladdoncls .trrowcls").each(function () {
        
        this.id = 'trrow_' + a;
        $(this).find('select').select2()[0].id = 'Drp_' + a;
        a = a + 1;
    });


    var sControl = false;
    var vid = $("#VendorId").val();
    if (vid == null || vid == '') {
        $("#VendorId").addClass('input-validation-error');
        event.preventDefault();
        sControl = true;
    }
    else {
        $("#VendorId").removeClass('input-validation-error');
        $("#VendorId").addClass('input-validation-valid');
    }

    var Drp = $("#Drp_1").val();
    if (Drp == null || Drp == '') {
        $("#Drp_1").addClass('input-validation-error');
        event.preventDefault();
        sControl = true;
    }
    else {
        $("#Drp_1").removeClass('input-validation-error');
        $("#Drp_1").addClass('input-validation-valid');
    }

    //var Amt = $("#Amt_1").val();
    //if (Amt == null || Amt == '') {
    //    $("#Amt_1").addClass('input-validation-error');
    //    event.preventDefault();
    //    sControl = true;
    //}
    //else {
    //    $("#Amt_1").removeClass('input-validation-error');
    //    $("#Amt_1").addClass('input-validation-valid');
    //}

    if (sControl == true) {
        isvalids = false;
        return isvalids;
    }

    if (myVald == 1) {
        var scaninvoice = $("#UploadInvoice").val();

        if (scaninvoice == "") {
            $('#popupid').show();
            isvalids = false;
            return isvalids;
        }
        else {
            $('#popupid').hide();
        }
    }

    $("#hdnreqval").val("1");
    if ($("#frm2").valid()) {
        if (CheckExistence() == false) {
            isvalids = false;
            return isvalids;
        }
        $(".loading-containers").attr("style", "display:block");
        //$(".headermainbox").attr("style", "z-index:0");
        //$(".page-sidebar").attr("style", "z-index:0");
    }
    else {
        event.preventDefault();
        isvalids = false;
        return isvalids;
    }

    if (CheckTotalInvoiceAmount() == "false") {
        isvalids = false;
        return isvalids;
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
        isvalids = false;
        return isvalids;
    }
    else {
        $('#popupDepartment').hide();
    }
    return isvalids;
}

function CheckTotalInvoiceAmount() {
    var PaidAmt = $("#txtPaidAmt").val();
    PaidAmt = parseFloat(PaidAmt).toFixed(2);
    var TotalPaid = $("#txtPaidTotal").val();
    if (TotalPaid == "") {
        TotalPaid = 0;
    }
    PaidAmt = PaidAmt - parseFloat(TotalPaid).toFixed(2);
    PaidAmt = parseFloat(PaidAmt).toFixed(2);
    console.log(PaidAmt);
    var totalamount = parseFloat($("#totalamounrt").val()).toFixed(2);
    if (totalamount == 0) {
        return false;
    }
    PaidAmt = parseFloat(PaidAmt).toFixed(2);
    if (parseFloat(totalamount) > parseFloat(PaidAmt).toFixed(2)) {
        $("#popupPaidAmtHigh").show();
        return false;
    }
    else {
        return true;
    }
}

function CheckExistence() {
    var isched = true;
    var vid = $("#VendorId").val();
    var invoiceno = $("#Invoice_Number").val();

    var val = $("#Invoice_Number").val();
    var reg = /^0*/gi;
    if (val.match(reg)) {
        invoiceno = val.replace(reg, '');
    }
    var invoicedate = $("#InvoiceDate").val();
    var type = 1;
    var storeid = $("#storeid3").val();
    var totalamtvalue = $('#spanamount').html().replace("$", "").replace(",", "").trim();
    //$("#totalamounrt").val();

    $.ajax({
        url: ROOTURL + '/Invoices/CheckExistence',
        beforeSend: function () { Loader(1); },
        data: { vendorid: vid, invoiceno: invoiceno, invoicedate: invoicedate, type: type, storeid: storeid, totalamtvalue: totalamtvalue },
        async: false,
        success: function (data) {
            $("#divdisplayduplicateinvoice").hide();
            var obj = jQuery.parseJSON(data);
            if (obj.result != "") {
                if (obj.result == "sub") {
                    event.preventDefault();
                    $("#divdisplayduplicateinvoice").show();
                    var output = "<ul style='list-style-type: none;'>";
                    for (var i in obj.data) {
                        output += "<li><a  class='popupcontent'  target='_blank' href=' " + ROOTURL + '/Invoices/Details/' + obj.data[i].InvoiceId + "'>" + obj.data[i].InvoiceNumber + " - " + obj.data[i].InvoiceDate + "</li>";
                    }
                    output += "</ul>";
                    $("#divdisplayduplicateinvoice").html(output);
                    showpopupExist();
                    isched = false;
                }
                else if (obj.result == "main") {
                    event.preventDefault();
                    $("#hrefopeninvoice").attr("href", ROOTURL + '/Invoices/Details/' + obj.data)
                    showpopupnew();
                    isched = false;
                }
                else {
                    // $(this.form).submit();
                    isched = true;
                }
            }
            else {
                // $(this.form).submit();
                isched = true;
            }

            Loader(0);
        }
    });
    return isched;
}

$("#btnInvoiceSubmit").click(function () {
    

    if (validatedrptext(1) == false) {
        $(".loading-containers").attr("style", "display:none");
        $(".headermainbox").removeAttr("style", "z-index:0");
        $(".page-sidebar").removeAttr("style", "z-index:0");
        return false;
    }

    if (CheckTotalInvoiceAmount() == false) {
        $(".loading-containers").attr("style", "display:none");
        $(".headermainbox").removeAttr("style", "z-index:0");
        $(".page-sidebar").removeAttr("style", "z-index:0");
        return false;
    }

    if ($("#txtShiftId").val() == "0") {
        MyCustomToster('After shift assign you can create invoice.');
        return false;
    }
    else {
        var formData = new FormData();
        formData.append('PaymentTypeId', '1');
        formData.append('InvoiceTypeId', '1');
        formData.append('Type', '1');
        var your_form = $("#frm2").serializeArray();
        $.each(your_form, function (key, input) {
            formData.append(input.name, input.value);
        });
        var uploadField = document.getElementById("UploadInvoice");
        for (var i = 0; i < uploadField.files.length; i++) {
            formData.append("UploadInvoice", uploadField.files[i]);
        }
        formData.append("TerminalId", $("#txtTerminalId").val());
        formData.append("ShiftID", $("#txtShiftId").val());
        formData.append("PaidOutID", $("#txtPaidOutID").val());
        formData.append("strInvoiceDate", $("#InvoiceDate").val());
        formData.append("StoreId", $("#storeid3").val());
        formData.append("TotalAmount", $("#totalamounrt").val());
        $.ajax({
            url: ROOTURL + '/Terminal/CreateCashInvoice',
            type: 'POST',
            dataType: 'json',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response == "Notsuccess") {
                    return false;
                }
                if (response == "success") {

                    window.location.href = ROOTURL + "/Terminal/Index?StoreId=" + $("#storeid3").val() + "&TerminalId= " + $("#txtTerminalId").val() + "&StartDate=" + $("#txtInvDate").val();
                }
            },
            error: function (response) {
                alert("Whooaaa! Something went wrong..");
            }
        });
    }
});