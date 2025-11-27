$(document).ready(function () {
    $('#SelectDeptForDisc').hide();
    $("#Invoice_Number").autocomplete("off");
    $("#InvoiceDate").autocomplete("off");
    $("#Amt_1").autocomplete("off");
    $('#InvoiceDate').datetimepicker({
        format: 'MMM DD,YYYY',
        useCurrent: false,
        maxDate: moment(),
        minDate: new Date('@ViewBag.closingyear', 01 - 1, 01),
    });
    $("#VendorId").select2();
    $("#discountype").select2();
    $("#Disc_Dept_id").select2();
    $("#Disc_Drp_1").select2();
    $("#Drp_1").select2();
    $(".myvalssss").select2();
});

function KeepandSave() {

    if (CheckTotalInvoiceAmount() == false) {
        $(".loading-containers").attr("style", "display:none");
        $(".headermainbox").removeAttr("style", "z-index:0");
        $(".page-sidebar").removeAttr("style", "z-index:0");
        return false;
    }

    if ($("#txtShiftId").val() == "0") {
        MyCustomToster(ShiftAssign);
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
            url: ROOTURL + "/Terminal/CreateCashInvoice",
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
            url: ROOTURL + "/Invoices/getVendorList",
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
            url: ROOTURL + "/Invoices/getDepartmentList",
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
            $("#addresss").empty();
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

    $(".myvalssss").select2('destroy');

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
            url: ROOTURL + "/Invoices/getVendorDepartmentList",
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



function validatedrptext(myVald) {

    var isvalids = true;
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

    $.ajax({
        url: ROOTURL + "/Invoices/CheckExistence",
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
                        output += "<li><a  class='popupcontent'  target='_blank' href=' " + ROOTURL + "/Invoices/Details/" + obj.data[i].InvoiceId + "'>" + obj.data[i].InvoiceNumber + " - " + obj.data[i].InvoiceDate + "</li>";
                    }
                    output += "</ul>";
                    $("#divdisplayduplicateinvoice").html(output);
                    //showpopupExist();
                    $('#popupcheckExistsagain').show();
                    isched = false;
                }
                else if (obj.result == "main") {
                    event.preventDefault();
                    $("#hrefopeninvoice").attr("href", ROOTURL + "/Invoices/Details/" + obj.data)
                    //showpopupnew();
                    $('#popupExists').show();
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
        MyCustomToster(ShiftAssign);
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
            url: ROOTURL + "/Terminal/CreateCashInvoice",
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

function closeRightsmodal() {
    $(".divIDClass1").hide();
}

function closecashmodal(ID) {
    $('#ModelContents').html('');
    $("#divCash").hide();
}

$(function () {
    $('#Startdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#Startdate').on('dp.change', function (e) {
        var id = $(this).val();
        GetData(id);
        $(".other-deposite").hide();
    });
});

$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
});

function GetData(Currentdate) {
    $.ajax({
        url: ROOTURL + "/Terminal/TerminalGrid",
        data: { date: Currentdate },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {

            $('#getdata').empty();
            $('#getdata').append(response);

            var Count = $("#TotalCount").val();
            var DayCount = $("#TotalDayCloseCount").val();

            if (Count == 0) {
                if (DayCount == 0) {
                    document.getElementById("BtnCloseout").classList.add('openotherbox_blue');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_red');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_Green');
                }
                else if (DayCount == 1) {
                    document.getElementById("BtnCloseout").classList.add('openotherbox_Green');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_red');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_blue');
                }
                else if (DayCount == 2) {
                    document.getElementById("BtnCloseout").classList.add('openotherbox_red');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_Green');
                    document.getElementById("BtnCloseout").classList.remove('openotherbox_blue');
                }
                $("#BtnCloseout").show();
            }
            else {
                $("#BtnCloseout").hide();
            }

            Loader(0);
            var tiddata = $("#tid0").val();
            var terval = terminalidval;
            var TerminalId = getParameterByName('TerminalId'); // "lorem"
            if ($("ul#myTab li.liclass.active :input")[0] != undefined) {
                var ii = $("ul#myTab li.liclass.active :input")[0].value;
            }
            if (checkforText(tiddata)) {
                terval = "";
            }
            var TTerminalID = sessionStorage.getItem("StartTerminalId");
            sessionStorage.setItem("StartTerminalId", "");

            if (ii) {
                BindShiftData(ii, "1");
                Bindotherdata(Currentdate, ii);
            }
            else if (TTerminalID != '' && TTerminalID != "" && TTerminalID != null) {
                BindShiftData(TTerminalID, "1");
                Bindotherdata(Currentdate, TTerminalID);
            }
            else if (terval === "") {
                BindShiftData(tiddata, "1");
                Bindotherdata(Currentdate, tiddata);
            }
            else if (TerminalId) {
                BindShiftData(TerminalId, "1");
                Bindotherdata(Currentdate, TerminalId);
            }
            else {
                BindShiftData(terval, "1");
                Bindotherdata(Currentdate, terval);
            }

        }
    });
}

function BindShiftData(id, flag) {
    $(".liclass").removeClass("active");
    $("#li" + id).addClass("active");
    var startdate = $('#Startdate').val();

    $.ajax({
        url: ROOTURL + "/Terminal/ShiftDataGrid",
        data: { date: startdate, terminalid: id },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            $('#shiftdata').empty();
            $('#shiftdata').append(response);
            Loader(0);
            var shiftid = $("#shift0").val();
            var tiddata = $("#ter0").val();
            var tidval = terminalidval;
            var sid_val = Shiftdataid;
            if (sid_val != '' && sid_val != "" && sid_val != null && sid_val != undefined) {
                flag = "2";
            }
            if (flag === "1") {
                BindShifttenderData(id, shiftid);
                Bindotherdata(startdate, id);
                $("#SalesActivityId").val(shiftid);
            }
            else {
                if ($("#lishift_" + sid_val).length) {
                    BindShifttenderData(id, sid_val);
                    Bindotherdata(startdate, id);
                    $("#SalesActivityId").val(sid_val);
                }
                else {
                    BindShifttenderData(id, shiftid);
                    Bindotherdata(startdate, id);
                    $("#SalesActivityId").val(shiftid);
                }
            }
        }
    });
}

function Bindotherdata(Currentdate, TerminalId) {
    var iSalesActivityId = $("#SalesActivityId").val();
    var shiftid = $("#shift0").val();
    if (iSalesActivityId == undefined) {
        return false;
    }
    $.ajax({
        url: ROOTURL + "/Terminal/OtherDepositGrid",
        data: { date: Currentdate, TerminalID: TerminalId, SalesActivityId: iSalesActivityId },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            $('#divother').empty();
            $('#divother').append(response);
            $("#iTerminalId").val(TerminalId);
            Loader(0);
            if ($("ul#mySubTab li.lishift.active")[0] != undefined) {
                var activetab = $("ul#mySubTab li.lishift.active")[0].innerText;
                $("#SelectedShift").val(activetab);
            }

        }
    });
}

function BindShifttenderData(id, sid) {
    $(".lishift").removeClass("active");
    $("#lishift_" + sid).addClass("active");
    var startdate = $('#Startdate').val();

    $.ajax({
        url: ROOTURL + "/Terminal/ShiftWisetenderGrid",
        data: { date: startdate, terminalid: id, shiftid: sid },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            $('#shifttenderdata').empty();
            $('#shifttenderdata').append(response);
            $("#iShitfGrid").val(sid);
            Loader(0);
            $('#Cashier_Name').hide();
            if ($("ul#mySubTab li.lishift.active")[0] != undefined) {
                var activetab = $("ul#mySubTab li.lishift.active")[0].innerText;
                $("#SelectedShift").val(activetab);
            }
            var ss = $("#SelectedShift").val();
            setSelectedValue(document.getElementById("SelectShifts_"), String(ss).toLowerCase());
            Bindotherdata(startdate, id);
        }
    });
}

function CheckOtherDepositeValue() {    
    var amountval = $('#new_country').val();
    var Optionidval = $('#Optionid').val();
    var Payidval = $('#Payid').val();
    var vendoridval = $('#vendorid').val();
    var departmentidval = $('#DepartmentId').val();
    if (amountval == "" && Optionidval == "" && Payidval == "" && vendoridval == "" && departmentidval == "") {
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else {
        if (Payidval != "") {
            $("#MethodIsReq").hide();
        }
    }
    if ((Payidval == "") && ($('#Payid').is(':enabled'))) {
        $("#VendorIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (($('#vendorid').is(':disabled')) && ($('#Other').is(':disabled'))) {
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;
        $("#MethodIsReq").hide();
    }
    if (amountval == "") {
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else {
        $("#AmountIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;
    }
    if (Optionidval == '') {
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else {
        $("#OptionsIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;
    }
    if ((vendoridval == null || '') && ($('#vendorid').is(':enabled'))) {
        
        $("#MethodIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (($('#Payid').is(':disabled')) && ($('#Other').is(':disabled'))) {
        
        $("#VendorIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;
    }
    if ((departmentidval == null || '') && ($('#DepartmentId').is(':enabled'))) {
        
        $("#DeptIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (($('#Payid').is(':disabled')) && ($('#Other').is(':disabled'))) {
        
        $("#DeptIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;
    }
}

function SaveOtherdeposit() {
    var id = storeid;
    var startdate = $('#Startdate').val();
    var nameval = $('#new_name').val();
    var paymentmode = $('#Payid').val();
    var amountval = $('#new_country').val();
    var Optionsval = $('#Optionid').val();
    var vendorval = $('#vendorid').val();
    var Deptval = $('#DepartmentId').val();
    var Otherval = $('#Other').val();
    var Terminalval = $("#iTerminalId").val();
    var Shiftval = $("#SelectShifts_").val();
    var sShift = $("#SelectShifts_ option:selected").text();
    var iSalesActivityId = $("#SalesActivityId").val();
    if (Optionsval == 'Gift Certificate' || Optionsval == 'House Charge Account' || Optionsval == 'Gratuity' ||
        Optionsval == 'Rebate') {
        if (paymentmode === "") {
            $("#MethodIsReq").show();
        }

        if (paymentmode === null) {
            $("#MethodIsReq").show();
        }
        if (amountval === "") {
            $("#AmountIsReq").show();
        }
    }

    if (Optionsval == 'Rebate' || Optionsval == 'Other' || Optionsval == 'Bottle Deposit') {
        if (vendorval === "") {
            $("#VendorIsReq").show();
        }
        if (vendorval === null) {
            $("#VendorIsReq").show();
        }
        if (amountval === "") {
            $("#AmountIsReq").show();
        }
        if (Deptval === "") {
            $("#DeptIsReq").show();
        }
        if (Deptval === null) {
            $("#DeptIsReq").show();
        }
    }

    if (Optionsval == 'Other') {
        if (paymentmode === "") {
            $("#MethodIsReq").show();
        }

        if (paymentmode === null) {
            $("#MethodIsReq").show();
        }
    }

    if (Optionsval == 'Gift Certificate' || Optionsval == 'House Charge Account' || Optionsval == 'Gratuity'
        || Optionsval == 'Rebate') {
        if (paymentmode === "") {
            $("#MethodIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else if (paymentmode === null) {
            $("#MethodIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else {
            $("#MethodIsReq").hide();
            document.getElementById("btnotherdepositsubmit").disabled = false;
        }
    }
    if (Optionsval == 'Rebate' || Optionsval == 'Bottle Deposit' || Optionsval == 'Other') {
        if (vendorval === "") {
            $("#VendorIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else if (vendorval === null) {
            $("#VendorIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else {
            $("#VendorIsReq").hide();
            document.getElementById("btnotherdepositsubmit").disabled = false;
        }

        if (Deptval === "") {
            $("#DeptIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else if (Deptval === null) {
            $("#DeptIsReq").show();
            return false;
            document.getElementById("btnotherdepositsubmit").disabled = true;
        }
        else {
            $("#DeptIsReq").hide();
            document.getElementById("btnotherdepositsubmit").disabled = false;
        }
    }
    if (Optionsval == 'Other') {

    }
    if (Optionsval === "") {
        $("#OptionsIsReq").show();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (amountval === "") {
        $("#AmountIsReq").show();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (amountval === "") {
        $("#AmountIsReq").show();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else {
        //document.getElementById("Optionid").required = false;
        $("#AmountIsReq").hide();
        $("#MethodIsReq").hide();
        //$("#NameIsReq").hide();
        $("#OptionsIsReq").hide();
        $("#TerminalIsReq").hide();
        $("#ShiftIsReq").hide();
        $("#VendorIsReq").hide();
        $("#DeptIsReq").hide();
        //$("#OtherIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "pointer");
        document.getElementById("btnotherdepositsubmit").disabled = false;

        var formData = new FormData($('form').get(0));
        var uploadField = document.getElementById("UploadFile");
        for (var i = 0; i < uploadField.files.length; i++) {
            formData.append("UploadFile", uploadField.files[i]);
        }
        formData.append("sid", id);
        formData.append("date", startdate);
        formData.append("name", nameval);
        formData.append("payment", paymentmode);
        formData.append("amount", amountval);
        formData.append("options", Optionsval);
        formData.append("vendor", vendorval);
        formData.append("Department", Deptval);
        formData.append("Other", Otherval);
        formData.append("Terminal", Terminalval);
        formData.append("Shift", Shiftval);
        formData.append("ActivitySalesSummuryId", iSalesActivityId);

        $.ajax({
            url: ROOTURL + "/Terminal/Saveotherdepositdata",
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false,
            beforeSend: function () { Loader(2); },
            success: function (response) {
                if (response === "sucess") {
                    sessionStorage.setItem("StartTerminalId", Terminalval);
                    GetData(startdate);
                    MyCustomToster(DepositAdedSucc);
                }
                Loader(0);
            }
        });
    }
}

function delete_Other_Deposite() {
    var Id = document.getElementById("DepositId").value;
    var startdate = $('#Startdate').val();
    var Terminalval = $("#iTerminalId").val();
    $.ajax({
        url: ROOTURL + "/Terminal/Deleteotherdepositdata",
        data: { Id: Id },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster(DepositDeletedSucc);
            }
            Loader(0);
        }
    });
}

function UpdateOtherDeposite(id, paymentmode, amountval, Optionsval, vendorval, Otherval, nameval, Departmentval) {
    var startdate = $('#Startdate').val();
    $("#IspaymentReq").hide();
    $("#IsAmountReq").hide();
    $("#IsvendorReq").hide();
    $("#IspaymentReq").hide();
    $("#IsoptionsReq").show();
    $("#IsAmountReq").show();
    $("#IsDeptReq").hide();
    var Terminalval = $("#iTerminalId").val();
    var formData = new FormData($('form').get(0));
    var uploadField = $("input#UploadFile2_" + id)[0];
    if (uploadField != undefined) {
        for (var i = 0; i < uploadField.files.length; i++) {
            formData.append("UploadFile", uploadField.files[i]);
        }
    }
    var iSalesActivityId = $("#SalesActivityId").val();

    formData.append("Id", id);
    formData.append("name", nameval);
    formData.append("payment", paymentmode);
    formData.append("amount", amountval);
    formData.append("options", Optionsval);
    formData.append("vendor", vendorval);
    formData.append("Department", Departmentval);
    formData.append("Other", Otherval);
    formData.append("Other", Otherval);
    formData.append("ActivitySalesSummuryId", iSalesActivityId);

    $.ajax({
        url: ROOTURL + "/Terminal/Updateotherdepositdata",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster(DepositUpdate);
            }
            Loader(0);
        }
    });
}

function delete_Other_Deposite_File() {
    var Id = document.getElementById("FileDepositId").value;
    var startdate = $('#Startdate').val();
    var Terminalval = $("#iTerminalId").val();
    $.ajax({
        url: ROOTURL + "/Terminal/DeleteotherdepositFile",
        data: { Id: Id },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "Delete") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster(DepFileDel);
            }
            Loader(0);
        }
    });
}

function SaveSettlementEntry() {
    var title = $("#txtSettlement").val();
    if (title == "") {
        $("#SettlementTitleIsReq").show();
        return false;
    }
    var Amount = $("#txtSettlementAmount").val();
    var SettlementID = $("#SalesActivityId").val();
    var startdate = $('#Startdate').val();
    $("#SettlementTitleIsReq").hide();

    $.ajax({
        url: ROOTURL + "/Terminal/SaveSettlementEntry",
        data: { Title: title, SettlementID: SettlementID, Amount: Amount },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                $(".lcs_checkbox_switch").trigger("click");
                $(".lcs_checkbox_switch").prop("disabled", true);
                $("#SettlementRow").hide();

                GetData(startdate);
                MyCustomToster(SettleEntry);
            }
            Loader(0);
        }
    });
}

function delete_SettlementEntry() {
    var Id = document.getElementById("SettlementId").value;
    var startdate = $('#Startdate').val();
    $.ajax({
        url: ROOTURL + "/Terminal/DeleteSettlementEntry",
        data: { Id: Id },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "Delete") {
                $("#PopDeleteSettlementEntry").hide();
                $(".lcs_checkbox_switch").trigger("click");
                $(".lcs_checkbox_switch").prop("disabled", false);
                $("#SettlementRow").show();
                GetData(startdate);
                MyCustomToster(SettleEntryDel);
            }
            Loader(0);
        }
    });
}

function checkforText(requiredText) {
    let found = false;
    requiredText = "li" + requiredText;
    $("ul#myTab li.liclass").each((id, elem) => {
        if (elem.id == requiredText) {
            found = true;
        }
    });
    return found;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function setSelectedValue(selectObj, valueToSet) {
    for (var i = 0; i < selectObj.options.length; i++) {
        if (selectObj.options[i].text.toLowerCase() == valueToSet) {
            selectObj.options[i].selected = true;
            return;
        }
    }
}

function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }
}

function DeleteTenderManual(ID) {
    $('.modal-backdrop').hide();
    var startdate = Startdate;
    $('#Startdate').val(startdate);
    $.ajax({
        url: '/Terminal/DeleteTenderEntry',
        data: { Id: ID },
        async: false,
        success: function (response) {
            if (response === "Delete") {
                GetData(startdate);
                return true;
            }
            Loader(0);
        },
        error: function () {
            return false;
        }
    });
}

function DeleteCashInvoicemodal(ID) {
    var startdate = $('#Startdate').val();
    var Terminalval = $("#iTerminalId").val();
    $.ajax({
        url: ROOTURL + "/Terminal/DeleteCashInvoice",
        data: { Id: ID },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster(DelCashIn);
            }
            Loader(0);
        }
    });
}

function ConfirmDayClose() {

    var date = $('#Startdate').val();
    var paidout = parseFloat($('#PaidOutAmount').text().trim())
    if (isNaN(paidout)) {
        paidout = 0;
    }
    if (paidout != 0) {
        $('#PaidAmountAdjust').show();
        return false;
    }
    else {
        $.ajax({
            url: ROOTURL + "/Terminal/GetStatus",
            data: { date: date },
            type: 'POST',
            cache: false,
            beforeSend: function () { Loader(1); },
            success: function (response) {
                Loader(0);
                $("#ConfirmDayClose").show();
                if (response == "") {
                    $("#DisplayMessage").text(SureCloseout);
                }
                else if (response == 0) {
                    $("#DisplayMessage").text(AlGenerated);
                }
                else if (response == 1) {
                    $("#DisplayMessage").text(AlApproved);
                }
                else if (response == 2) {
                    var cls = $('#BtnCloseout').attr('class');
                    if (cls.toString().includes("_red")) {
                        $("#DisplayMessage").text(AlGenUpdated);
                    }
                    else {
                        $("#DisplayMessage").text(AlGenerated);
                    }
                }
                else if (response == 3) {
                    var cls = $('#BtnCloseout').attr('class');
                    if (cls.toString().includes("_red")) {
                        $("#DisplayMessage").text(AlAppUpdated);
                    }
                    else {
                        $("#DisplayMessage").text(AlApproved);
                    }
                }
            },
            error: function () {
                Loader(0);

            }
        });
    }
}

//function DayCloseoutfun() {
//    var date = $('#Startdate').val();
//    var aa = $("#txtSettlementDone").val();
//    $.ajax({
//        url: ROOTURL + "/Terminal/Daycloseout?date_val=" + date + '&IsSettlementDone=' + aa,
//        type: 'POST',
//        cache: false,

//        beforeSend: function () { Loader(1); },
//        success: function (response) {
//            Loader(0);
//            MyCustomAlert(response, 1);
//            GetData(date);
//            $(".divIDClass").hide();
//        },
//        error: function () {
//            Loader(0);
//            MyCustomAlert("Some Error Occrued", 2);
//            $(".divIDClass").hide();
//        }
//    });

//}

function DayCloseoutfun() {
    var date = $('#Startdate').val();
    var aa = $("#txtSettlementDone").val();
    $.ajax({
        url: ROOTURL + '/Terminal/CheckPreviousDayClosedOut?date_val=' + date,
        type: 'POST',
        cache: false,

        beforeSend: function () { Loader(1); },
        success: function (response) {
            var dateMatch = response.match(/\/Date\((-?\d+)\)\//);
            var timestamp = parseInt(dateMatch[1]);
            var dateObject = new Date(timestamp);
            var month = (dateObject.getMonth() + 1).toString().padStart(2, '0');
            var day = dateObject.getDate().toString().padStart(2, '0');
            var year = dateObject.getFullYear();
            var formattedDate = month + '/' + day + '/' + year;

            if (formattedDate === "01/01/1") {
                $.ajax({
                    url: ROOTURL + '/Terminal/Daycloseout?date_val=' + date + '&IsSettlementDone=' + aa,
                    type: 'POST',
                    cache: false,

                    beforeSend: function () { Loader(1); },
                    success: function (response) {
                        Loader(0);
                        MyCustomAlert(response, 1);
                        GetData(date);
                        $(".divIDClass").hide();
                    },
                    error: function () {
                        Loader(0);
                        MyCustomAlert("Some Error Occrued", 2);
                        $(".divIDClass").hide();
                    }
                });
            }
            else {
                Loader(0);
                MyCustomAlert("Previous Register day (" + formattedDate + ") is still open. Please consider closing it.", 2);
                GetData(date);
                $(".divIDClass").hide();
            }
        },
        error: function () {
        }
    });
}

function CahInvoicePopup(ID, Amount, TerminalId) {
    var DivId = ID;
    var iShift = $("#txtShiftId").val();
    if (Amount == "" || Amount == "0") {
        return false;
    }
    else {
        var Termid = 0;

        var activetab = $("ul#myTab li.active").attr("id");
        var termval = activetab.substr(activetab.indexOf("li") + 2);
        var date = $('#Startdate').val();
        var tiddata = $("#tid0").val();
        var terval = terminalidval;
        var TerminalId = getParameterByName('TerminalId'); // "lorem"

        if (termval) {
            Termid = termval;
        }
        else if (TerminalId) {
            Termid = TerminalId;
        }
        else if (terval === "") {
            Termid = tiddata;
        }
        else {
            Termid = terval;
        }

        var ShiftID = $("#iShitfGrid").val();
        var PaidTotal = $("#txtPaidTotal1").val();
        $.ajax({
            url: ROOTURL + "/Terminal/CreateCashInvoice",
            beforeSend: function () {
                Loader(1);
            },
            success: function (data) {
                Loader(0);
                $('#ModelContents').html('');
                $('#ModelContents').html(data);
                $("#txtTerminalId").val(Termid);
                $("#txtShiftId").val(ShiftID);
                $("#txtPaidOutID").val(ID);
                $("#txtPaidAmt").val(Amount);
                $("#txtPaidTotal").val(PaidTotal);
                $("#txtInvDate").val(date);
                $("#divCash").show();
                sessionStorage.setItem("StartTerminalId", Termid);
            }
        });
    }
}

$("#Optionid").change(function () {
    CheckOtherDepositeValue();
});

$(data_tabletwo).on('change', 'tr td select#edit_ddloptions', function (index, obj) {
    var opt = this.value;
    var PayId = $(this).closest('tr').find('td').eq(5).html();
    getpayment1(opt, $(this), PayId, 1);
})

$(data_tabletwo).on('click', 'tr td a#btnSubOtherEdit', function () {
    var row = $(this).closest('tr');
    var ShiftID = row.find('td').eq(1).html();
    var OptionsID = row.find('td').eq(2).html();
    var VendorId = row.find('td').eq(3).html();
    var DeptId = row.find('td').eq(4).html();
    var desc = row.find('td').eq(5).html();
    var PayId = row.find('td').eq(6).html();
    var Amt = row.find('td').eq(7).html();
    var sShift = row.attr("data-shiftname");
    var soptions = row.attr("data-optionsname");
    var svendor = row.attr("data-Vendorname");
    var sDept = row.attr("data-Deptname");
    var sID = row.attr("data-id");

    $(this).parents("tr").find("td:eq(1)").html('<select id="edit_ddlShift" name="edit_ddlShift" class="form-control" ></select><span class="highlight" id="IsShiftReq">Shift Is Required</span>');
    $(this).parents("tr").find("td:eq(2)").html("<select id='edit_ddloptions' name='edit_ddloptions' class='form-control'></select><span class='highlight' id='IsoptionsReq'>Type Is Required</span>");
    $(this).parents("tr").find("td:eq(3)").html('<div id="vendorNA"><span>N/A</span></div> <div id="vendorNames"><select id="edit_ddlvendor" name="edit_ddlvendor" class="form-control"></select><span class="highlight" id="IsvendorReq">Vendor Is Required</span></div>');
    $(this).parents("tr").find("td:eq(4)").html('<div id="DeptNA"><span>N/A</span></div> <div id="DeptNames"><select id="edit_ddlDept" name="edit_ddlDept" class="form-control"></select><span class="highlight" id="IsDeptReq">Department Is Required</span></div>');
    $(this).parents("tr").find("td:eq(5)").html('<input id="edit_details" name="edit_details"" type="text" value="' + desc + '" style="width:100%"><span class="highlight" id="IsdetailsReq">Detail Is Required</span>');
    $(this).parents("tr").find("td:eq(6)").html("<div id='PaymentNA'><span>N/A</span></div><div id='PaymentNames'><select id='edit_ddlPayment' name='edit_ddlPayment' class='form-control' ></select><span class='highlight' id='IspaymentReq'>Payment Type Is Required</span></div>");
    $(this).parents("tr").find("td:eq(7)").html('<input id="edit_Amount" type="number" name="edit_Amount" type="text" value="' + Amt.trim() + '" style="width:100%"><span class="highlight" id="IsAmountReq">Amount Is Required</span>');
    $(this).closest("tr").find("#UploadFile2_" + sID).attr('disabled', false);

    $(this).parents("tr").find("td:eq(9)").prepend("<a id='btnSubOtherEditUpdate' href='#'><img src='/Content/Admin/images/check.svg' alt='' style='height:15px;width:20px' /></a><a id='btnSubOtherEditCancel' href='#'><img src='/Content/Admin/images/Cancel2.png' alt='' style='height:25px;width:25px' /></a>")
    $(this).hide();
    FillEditShift(sShift, $(this));
    FillEditOptions(soptions, $(this));
    FillEditVendor(svendor, $(this));
    FillEditDepartment(sDept, $(this));
    getpayment1(soptions, $(this), PayId, 0);

    $("#IsShiftReq").hide();
    $("#IsoptionsReq").hide();
    $("#IsvendorReq").hide();
    $("#IsDeptReq").hide();
    $("#IsdetailsReq").hide();
    $("#IspaymentReq").hide();
    $("#IsAmountReq").hide();
});

function FillEditShift(ID, obj) {
    var option = '';
    if (ShiftNameList.length == 0) {
        option += '<option value="0" selected>Shift#</option>';
    }
    if (ShiftNameList.length > 0) {
        if (ShiftNameList[0].Text != "Shift#") {
            option += '<option value="0" selected>Shift#</option>';
        }
    }

    for (var i = 0; i < ShiftNameList.length; i++) {
        option += "<option value='" + ShiftNameList[i].Value + "'" + (ShiftNameList[i].Text == ID ? "selected" : "") + ">" + ShiftNameList[i].Text + "</option>";
    }
    obj.closest('tr').find("#edit_ddlShift").html(option);
}

function FillEditOptions(ID, obj) {
    var option = '<option value=' + 0 + ' Selected>Select Type</option>';
    for (var i = 0; i < iteSelectOptionListm.length; i++) {
        option += "<option value='" + iteSelectOptionListm[i].Value + "'" + (iteSelectOptionListm[i].Text == ID ? "selected" : "") + " >" + iteSelectOptionListm[i].Text + "</option>";
    }
    obj.closest('tr').find("#edit_ddloptions").html(option);
}

function FillEditVendor(ID, obj) {
    
    var option = '<option value=' + 0 + ' Selected>Select Vendor</option>';
    for (var i = 0; i < SelectVendorList.length; i++) {
        if (SelectVendorList[i].Text.trim() == ID.trim()) {
            console.log(1);
        }
        option += "<option value='" + SelectVendorList[i].Value + "'" + (SelectVendorList[i].Text.trim() == ID.trim() ? " selected" : "") + ">" + SelectVendorList[i].Text + "</option>";
    }
    obj.closest('tr').find("#edit_ddlvendor").html(option);
}

function FillEditDepartment(ID, obj) {
    var option = '<option value=' + 0 + ' Selected>Select Department</option>';
    for (var i = 0; i < SelectDepartmentList.length; i++) {
        if (SelectDepartmentList[i].Text.trim() == ID.trim()) {
            console.log(1);
        }
        option += "<option value='" + SelectDepartmentList[i].Value + "'" + (SelectDepartmentList[i].Text.trim() == ID.trim() ? " selected" : "") + ">" + SelectDepartmentList[i].Text + "</option>";
    }
    obj.closest('tr').find("#edit_ddlDept").html(option);
}

$(data_tabletwo).on('click', 'tr td a#btnSubOtherEditUpdate', function () {
    var ShiftName = $(this).parents('tr').find("#edit_ddlShift").find("option:selected").text().trim();
    var Shiftval = $(this).parents('tr').find("#edit_ddlShift").val();
    var OptionsName = $(this).parents('tr').find("#edit_ddloptions").find("option:selected").text().trim();
    var optionsval = $(this).parents('tr').find("#edit_ddloptions").val();
    var Payment = $(this).parents('tr').find("#edit_ddlPayment").find("option:selected").text().trim();
    var paymentval = $(this).parents('tr').find("#edit_ddlPayment").val();
    var amt = $(this).parents('tr').find("#edit_Amount").val();
    var vendorName = $(this).parents('tr').find("#edit_ddlvendor").find("option:selected").text().trim();
    var vendorval = $(this).parents('tr').find("#edit_ddlvendor").val();
    var DeptName = $(this).parents('tr').find("#edit_ddlDept").find("option:selected").text().trim();
    var Deptval = $(this).parents('tr').find("#edit_ddlDept").val();

    if (OptionsName == 'Gift Certificate' || OptionsName == 'House Charge Account' || OptionsName == 'Gratuity' ||
        OptionsName == 'Rebate') {
        if (Payment === "Select Payment Method" || paymentval == "0") {
            $("#IspaymentReq").show();
            return false;
        }
        else {
            $("#IspaymentReq").hide();
        }

        if (amt == "" || amt == "0") {
            $("#IsAmountReq").show();
            return false;
        }
        else {
            $("#IsAmountReq").hide();
        }
    }
    if (OptionsName == 'Rebate' || OptionsName == 'Bottle Deposit' || OptionsName == 'Other') {
        if (vendorval === "0" || vendorName == "Select Vendor") {
            $("#IsvendorReq").show();
            return false;
        }
        else {
            $("#IsvendorReq").hide();
        }

        if (Deptval === "0" || DeptName == "Select Department") {
            $("#IsDeptReq").show();
            return false;
        }
        else {
            $("#IsDeptReq").hide();
        }
        if (amt == "" || amt == "0") {
            $("#IsAmountReq").show();
            return false;
        }
        else {
            $("#IsAmountReq").hide();
        }
    }
    if (OptionsName == 'Other') {
        if (Payment === "Select Payment Method" || paymentval == "0") {
            $("#IspaymentReq").show();
            return false;
        }
        else {
            $("#IspaymentReq").hide();
        }
    }

    else if (amt == "" || amt == "0") {
        $("#IsAmountReq").show();
        return false;
    }
    else if (optionsval == "" || optionsval == "0") {
        $("#IsoptionsReq").show();
    }

    $(this).parents("tr").find("td:eq(1)").text(ShiftName);
    $(this).parents("tr").find("td:eq(2)").text(OptionsName);

    if (vendorName == "Select Vendor" || vendorName == "") {
        $(this).parents("tr").find('#vendorNA').show();
        $(this).parents("tr").find('#vendorNames').hide();
    }
    else {
        $(this).parents("tr").find("td:eq(3)").text(vendorName);
    }

    if (DeptName == "Select Department" || DeptName == "") {
        $(this).parents("tr").find('#DeptNA').show();
        $(this).parents("tr").find('#DeptNames').hide();
    }
    else {
        $(this).parents("tr").find("td:eq(4)").text(DeptName);
    }

    var details = $(this).parents('tr').find("#edit_details").val();
    $(this).parents("tr").find("td:eq(5)").text(details);


    if (Payment == "Select Payment Method" || Payment == "") {
        $(this).parents("tr").find('#PaymentNA').show();
        $(this).parents("tr").find('#PaymentNames').show();
    }
    else {
        $(this).parents("tr").find("td:eq(6)").text(Payment);
    }

    $(this).parents("tr").find("td:eq(7)").text(amt);

    var ID = $(this).parents('tr').attr('data-id');
    if (vendorName == "Select Vendor") {
        vendorName = "";
    }
    if (OptionsName == 'Gift Certificate' || OptionsName == 'House Charge Account' || OptionsName == 'Gratuity' || OptionsName == 'Online Gratuity') {
        vendorName = "";
    }

    if (OptionsName == 'Online Gratuity') {
        Payment = "";
    }

    UpdateOtherDeposite(ID, Payment, amt, OptionsName, vendorName, "", details, DeptName);

    $(this).parents("tr").find("a#btnSubOtherEditCancel").hide();
    $(this).parents("tr").find("a#btnSubOtherEdit").show();
    $(this).parents("tr").find("a#btnSubOtherEditUpdate").hide();
});

function getpayment1(option_val, obj, paylist, isdirect) {

    if (option_val == '') {
        option_val = 0;
    }

    if (option_val == 'Gift Certificate' || option_val == 'House Charge Account' || option_val == 'Gratuity' ||
        option_val == 'Other') {
        document.getElementById("edit_ddlvendor").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlvendor").val(0);
        }
        obj.closest('tr').find("#vendorNames").hide();
        obj.closest('tr').find('#vendorNA').show();
        obj.closest('tr').find('#PaymentNA').hide();
        obj.closest('tr').find('#PaymentNames').show();

        document.getElementById("edit_ddlDept").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlDept").val(0);
        }
        obj.closest('tr').find("#DeptNames").hide();
        obj.closest('tr').find('#DeptNA').show();
    }
    if (option_val == 'Online Gratuity') {
        document.getElementById("edit_ddlvendor").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlvendor").val(0);
        }

        obj.closest('tr').find("#vendorNames").hide();
        obj.closest('tr').find('#vendorNA').show();

        obj.closest('tr').find('#PaymentNA').show();
        obj.closest('tr').find('#PaymentNames').hide();

        document.getElementById("edit_ddlDept").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlDept").val(0);
        }
        obj.closest('tr').find("#DeptNames").hide();
        obj.closest('tr').find('#DeptNA').show();
    }
    if (option_val == 'Bottle Deposit') {
        document.getElementById("edit_ddlvendor").disabled = false;
        obj.closest('tr').find("#vendorNames").show();
        obj.closest('tr').find('#vendorNA').hide();
        obj.closest('tr').find('#PaymentNA').show();
        obj.closest('tr').find('#PaymentNames').hide();

        document.getElementById("edit_ddlDept").disabled = false;
        obj.closest('tr').find("#DeptNames").show();
        obj.closest('tr').find('#DeptNA').hide();
    }
    if (option_val == 'Rebate' || option_val == 'Other') {
        document.getElementById("edit_ddlvendor").disabled = false;
        obj.closest('tr').find("#vendorNames").show();
        obj.closest('tr').find('#vendorNA').hide();
        obj.closest('tr').find('#PaymentNA').hide();
        obj.closest('tr').find('#PaymentNames').show();

        document.getElementById("edit_ddlDept").disabled = false;
        obj.closest('tr').find("#DeptNames").show();
        obj.closest('tr').find('#DeptNA').hide();
    }

    if (option_val == 'Gratuity') {
        document.getElementById("edit_ddlvendor").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlvendor").val(0);
        }

        obj.closest('tr').find("#vendorNames").hide();
        obj.closest('tr').find('#vendorNA').show();

        obj.closest('tr').find('#PaymentNA').hide();
        obj.closest('tr').find('#PaymentNames').show();

        document.getElementById("edit_ddlDept").disabled = true;
        if (isdirect == "1") {
            $("#edit_ddlDept").val(0);
        }

        obj.closest('tr').find("#DeptNames").hide();
        obj.closest('tr').find('#DeptNA').show();
    }

    if (option_val == 'Gift Certificate' || option_val == 'House Charge Account' || option_val == 'Other' || option_val == 'Rebate' || option_val == 'Gratuity') {
        var Id = option_val;
        obj.closest('tr').find("#edit_ddlPayment").empty();
        obj.closest('tr').find("#edit_ddlPayment").append("<option value='0' selected>Select Payment Method</option>");
        $.getJSON('/Terminal/GetPaymethodlist/' + Id, function (data) {
            $.each(data, function (i, model1) {
                obj.closest('tr').find("#edit_ddlPayment").append(
                    "<option value=" + model1.Value + "  " + (model1.Text.trim() == paylist.trim() ? "selected" : "") + ">" + model1.Text + "</option>")
            });
        });
    }
}

$(data_tabletwo).on('click', 'tr td a#btnSubOtherEditCancel', function () {
    var sShift = $(this).parents('tr').attr('data-shiftname');
    var ShiftName = $(this).parents('tr').find("#edit_ddlShift").find("option:selected").text().trim();
    $(this).parents("tr").find("td:eq(1)").text(sShift);

    var soptions = $(this).parents('tr').attr('data-optionsname');
    var OptionsName = $(this).parents('tr').find("#edit_ddloptions").find("option:selected").text().trim();
    $(this).parents("tr").find("td:eq(2)").text(soptions);

    var svendor = $(this).parents('tr').attr('data-Vendorname');
    var vendorName = $(this).parents('tr').find("#edit_ddlvendor").find("option:selected").text().trim();
    if (svendor == "Select Vendor" || svendor == "") {
        $(this).parents("tr").find('#vendorNA').show();
        $(this).parents("tr").find('#vendorNames').hide();
    }
    else if (svendor == "null") {
        $(this).parents("tr").find('#vendorNA').show();
        $(this).parents("tr").find('#vendorNames').hide();
    }
    else {
        $(this).parents("tr").find("td:eq(3)").text(svendor);
    }

    var sDept = $(this).parents('tr').attr('data-Deptname');
    var DeptName = $(this).parents('tr').find("#edit_ddlDept").find("option:selected").text().trim();
    if (sDept == "Select Department" || sDept == "") {
        $(this).parents("tr").find('#DeptNA').show();
        $(this).parents("tr").find('#DeptNames').hide();
    }
    else if (sDept == "null") {
        $(this).parents("tr").find('#DeptNA').show();
        $(this).parents("tr").find('#DeptNames').hide();
    }
    else {
        $(this).parents("tr").find("td:eq(4)").text(svendor);
    }

    var sdetails = $(this).parents('tr').attr('data-details');
    var details = $(this).parents('tr').find("#edit_details").val();
    $(this).parents("tr").find("td:eq(5)").text(sdetails);

    var sPayment = $(this).parents('tr').attr('data-paymentMethod');
    var Payment = $(this).parents('tr').find("#edit_ddlPayment").find("option:selected").text().trim();
    if (sPayment == "Select Payment Method" || sPayment == "") {
        $(this).parents("tr").find('#PaymentNA').show();
        $(this).parents("tr").find('#PaymentNames').hide();
    }
    else {
        $(this).parents("tr").find("td:eq(6)").text(sPayment);
    }

    var samt = $(this).parents('tr').attr('data-amount');
    var amt = $(this).parents('tr').find("#edit_Amount").val();
    $(this).parents("tr").find("td:eq(7)").text(samt);

    $(this).parents("tr").find("a#btnSubOtherEditCancel").hide();
    $(this).parents("tr").find("a#btnSubOtherEdit").show();
    $(this).parents("tr").find("a#btnSubOtherEditUpdate").hide();

});

function getpayment(option_val, op_id) {
    if (option_val == '') {
        option_val = 0;
    }

    if (option_val == 'Gift Certificate' || option_val == 'House Charge Account') {
        document.getElementById("Payid").disabled = false;
        document.getElementById("vendorid").disabled = true;
        document.getElementById("DepartmentId").disabled = true;
        $("#vendorid").val(0);
        $("#DepartmentId").val(0);
        $("#Payid").val(0);
        $("#PaymentMethod").show();
        $("#PaymentMethodN").show();
        $("#PaymentMethodNA").hide();
        $("#VendorName").hide();
        $("#VendorNameN").hide();
        $("#VendorNameNA").show();
        $("#DeptName").hide();
        $("#DeptNameN").hide();
        $("#DeptNameNA").show();
    }

    if (option_val == 'Bottle Deposit') {
        document.getElementById("vendorid").disabled = false;
        document.getElementById("DepartmentId").disabled = false;
        document.getElementById("Payid").disabled = true;
        $("#vendorid").val(0);
        $("#DepartmentId").val(0);
        $("#Payid").val(0);
        $("#PaymentMethod").hide();
        $("#PaymentMethodN").hide();
        $("#PaymentMethodNA").show();
        $("#VendorName").show();
        $("#VendorNameN").show();
        $("#VendorNameNA").hide();

        $("#DeptName").show();
        $("#DeptNameN").show();
        $("#DeptNameNA").hide();
    }
    if (option_val == 'Rebate' || option_val == 'Other') {
        document.getElementById("vendorid").disabled = false;
        document.getElementById("DepartmentId").disabled = false;
        document.getElementById("Payid").disabled = false;
        $("#vendorid").val(0);
        $("#DepartmentId").val(0);
        $("#Payid").val(0);
        $("#PaymentMethod").show();
        $("#PaymentMethodN").show();
        $("#PaymentMethodNA").hide();
        $("#VendorName").show();
        $("#VendorNameN").show();
        $("#VendorNameNA").hide();
        $("#DeptName").show();
        $("#DeptNameN").show();
        $("#DeptNameNA").hide();
    }


    if (option_val == 'Other') {
        document.getElementById("Payid").disabled = false;
        $("#Payid").val(0);
        $("#PaymentMethod").show();
        $("#PaymentMethodN").show();
        $("#PaymentMethodNA").hide();
    }

    if (option_val == 'Gratuity') {
        document.getElementById("Payid").disabled = false;
        document.getElementById("vendorid").disabled = true;
        document.getElementById("DepartmentId").disabled = true;
        $("#vendorid").val(0);
        $("#DepartmentId").val(0);
        $("#Payid").val(0);
        $("#VendorName").hide();
        $("#VendorNameN").hide();
        $("#VendorNameNA").show();

        $("#DeptName").hide();
        $("#DeptNameN").hide();
        $("#DeptNameNA").show();

        $("#PaymentMethod").show();
        $("#PaymentMethodN").show();
        $("#PaymentMethodNA").hide();
    }

    if (option_val == 'Online Gratuity') {
        document.getElementById("Payid").disabled = true;
        $("#vendorid").val(0);
        $("#DepartmentId").val(0);
        $("#Payid").val(0);
        $("#PaymentMethod").hide();
        $("#PaymentMethodN").hide();
        $("#PaymentMethodNA").show();
    }

    if (option_val == 'Gift Certificate' || option_val == 'House Charge Account' || option_val == 'Rebate' || option_val == 'Other'
        || option_val == 'Gratuity') {
        var Id = option_val;
        $("#Payid").empty();
        $("#Payid").append("<option selected='selected' value=''>Select Payment Method</option>");
        $.getJSON('/Terminal/GetPaymethodlist/' + Id, function (data) {
            $.each(data, function (i, model1) {
                $("#Payid").append(
                    "<option value=" + model1.Value + ">" + model1.Text + "</option>")
            });
        });
    }
}

document.getElementById('btnSelectFile').addEventListener('click', function (e) {
    e.preventDefault();
    document.getElementById('UploadFile').click();
});

$("#UploadFile").change(function () {
    if (this.files.length > 0) {
        $("#file-name").text(this.files[0].name);
    }
    else {
        $("#file-name").text("");
    }
});

function openFiledlg(ID) {
    document.getElementById("UploadFile2_" + ID).click();
}

function ChangeFileName(ID) {
    if ($("input#UploadFile2_" + ID)[0].files.length > 0) {
        $("#file-name1_" + ID).text($("input#UploadFile2_" + ID)[0].files[0].name);
    }
    else {
        $("#file-name1_" + ID).text("");
    }
}

$(document).ready(function () {

    $("#PaymentMethod").hide();
    $("#PaymentMethodN").hide();
    $("#VendorName").hide();
    $("#VendorNameN").hide();

    $("#DeptName").hide();
    $("#DeptNameN").hide();
    $("#TerminalIsReq").hide();
    $("#ShiftIsReq").hide();
    document.getElementById("vendorid").disabled = true;
    document.getElementById("DepartmentId").disabled = true;
    document.getElementById("Payid").disabled = true;

    if (DepositeCount > 0) {
        document.getElementById("spanotherdeposit").innerHTML = DepositeCount;
}
        else {
    document.getElementById("spanotherdeposit").innerHTML = '0';
}

$("#AmountIsReq").hide();
$("#MethodIsReq").hide();
$("#OptionsIsReq").hide();
$("#VendorIsReq").hide();
$("#DeptIsReq").hide();
var ss = $("#SelectedShift").val();
setSelectedValue1(document.getElementById("SelectShifts_"), String(ss).toLowerCase());
    });

function setSelectedValue1(selectObj, valueToSet) {

    for (var i = 0; i < selectObj.options.length; i++) {
        if (selectObj.options[i].text.toLowerCase() == "shift#" || selectObj.options[i].text.toLowerCase() == valueToSet) {
        }
        else {
            selectObj.remove(i);
        }
    }

    for (var i = 0; i < selectObj.options.length; i++) {
        if (selectObj.options[i].text.toLowerCase() == valueToSet) {
            selectObj.options[i].selected = true;
        }
    }
}

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46 && charCode !== 45)
        return false;

    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

function ConfirmDelete(ID) {
    $("#DepositId").val(ID);
    $.ajax({
        url: ROOTURL + "/Terminal/CheckSalesOtherDeposite_UsedAnywhere",
        data: { OtherDepositeID: ID },
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
    $("#PopDeleteOtherDeposit").show();
}
function ConfirmDeleteFile(ID) {
    $("#FileDepositId").val(ID);
    $("#PopDeleteOtherDepositFile").show();
}
