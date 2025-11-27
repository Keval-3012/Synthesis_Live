function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1, 2}[\/-]\d{1, 2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}

//function getVendorList() {
//    var StoreId = $('#Storeid').val();
//    if (StoreId != "") {
//        $.ajax({
//            url: '/Invoices/getVendorList',
//            type: "GET",
//            dataType: "JSON",
//            data: { StoreId: StoreId },
//            success: function (states) {
//                $("#VendorId").html("");
//                $('select#VendorId').html('<option value="">--Select--</option>');
//                $.each(states, function (data, value) {
//                    $("#VendorId").append(
//                        $('<option></option>').val(value.VendorId).html(value.VendorName));
//                });
//            }
//        });
//    }
//}

function CreateNew() {
    validatedrptextNew(1);
    $('#flagaddnew').val(1);
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

function decimalOnly(evt) {
    
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
        return false;
    return true;
}
function preventPaste(e) {
    e.preventDefault();
}
function ReplaceData(html_val, c_val, r_val) {
    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

///* END EXTERNAL SOURCE */
function KeepandSave() {
    document.getElementById('frmMain').submit();
}

function AddAmt(iii) {    
    var temp = 0;
    var numItems = $('.addamtcls-' + iii).each(function () {
        if ($(this).val() != '') {
            temp += parseFloat($(this).val());
        }
    });
    const formatter = new Intl.NumberFormat('en-US', {
        currency: 'USD',
        minimumFractionDigits: 2
    })
    var n = temp.toFixed(2);
    var totalamount = formatter.format(n);

    $("#TotalAmount").val(n);

    ////add val in Invoice Amount textbox by himanshu 30-01-2025
    //$("#InvoiceAmount").val(n);
    ////end

    $("#spanamount").html(totalamount);
    $("#lblTotalAmount-" + iii + "").html(totalamount);
    GetAllStoreTotal(iii);
}

function RemoveAmt(val, iii) {
    var finaltotal = 0;
    var Totalamt = $('#spanamount').html();

    var temp = document.getElementById('Amt_' + (val) + '-' + iii).value;

    var total = Totalamt.split("$").pop();

    finaltotal = total - temp;
    var totalamount = "$" + (finaltotal);

    $("#TotalAmount").val(totalamount);
    $("#spanamount").html(totalamount);
}

$(function () {
    $(".setAmount").val("0");
});

function Setvalues() {
    localStorage.setItem("FromInvoicePage", "");
}

$("select#Type_id")[0].selectedIndex = 1;
if ($("#SplitEqually")[0].checked == true) {
    document.getElementById("dvStorePercentage").classList.remove("percentage");
}

var _binddata1 = "";
$('.chkstoreonclick').change(function () {

    if ($(this).is(':checked') == true) {
        var sname = $(this)[0].labels[0].innerText;
        var sid = $(this).next()[0].nextSibling.nextSibling.defaultValue;

        var _binddata = '';// document.getElementById("dvbindstore").innerHTML;

        _binddata += "<div id=" + sid + " class='storeWiseDepartments'>";
        _binddata += "<span class='storeWiseDepartmentsTitle'>" + sname + "</span>";
        _binddata += "<select class='myval1_" + sid + " drpcls' id='VendorId_" + sid + "'></select>";

        _binddata += "<div id='tbladdon_" + sid + "' class='tbladdoncls_" + sid + " checkvalidation'><input type='hidden' id='hdninvoicecount_" + sid + "' value='1' /><input type='hidden' id='hdnreqval_" + sid + "' value='0' />";
        _binddata += "<div id='trrow_1_" + sid + "' class='commonadd trrowcls_" + sid + "'>";

        _binddata += "<div class='storeWiseDepartmentsContainer bg_red'><select class='drpcls myval-" + sid + " required' id='Drp_1-" + sid + "'></select>";

        _binddata += "<input type='text' id='Amt_1-" + sid + "' onkeyup='AddAmt(" + sid + ")' maxlength='18' class='form-control groupOfTexbox decimalOnly addamtcls-" + sid + "  required setAmount' value='0.00' onkeypress='return decimalOnly(event)' onpaste='return preventPaste(event)'/>";
        _binddata += "<a id='aAdd_1-" + sid + "' onclick='AddField(1," + sid + ")'><i class='fa fa-plus'></i></a><a id='aRemove_1-" + sid + "' class='aremove1-" + sid + "' onclick='RemoveField(1," + sid + ")' style='display:none;'><i class='fa fa-minus'></i></a>";
        _binddata += "</div></div></div>";

        _binddata += "<div class='splitWiseTotal'><span class='totalAmount'>Total Amount</span>";
        _binddata += "<span class='totalAmount'>$</span><span class='totalAmount stotal' id='lblTotalAmount-" + sid + "'>0.00</span></div>";
        _binddata += "</div>";

        $('#dvbindstore').append(_binddata);
        drpStoreselect(sid);

        $("#SplitEqually").val();
        if ($("#SplitEqually")[0].checked == true) {
            SplitAmount();
            document.getElementById("dvStorePercentage").classList.remove("percentage");
        }
        else if ($("#Percentage")[0].checked == true) {
            document.getElementById("dvStorePercentage").classList.add("percentage");
        }
        else {
            document.getElementById("dvStorePercentage").classList.remove("percentage");
        }
    }
    else {
        //remove selected store..
        var sname = $(this)[0].labels[0].innerHTML;
        var sid = $(this).next()[0].nextSibling.nextSibling.defaultValue;

        document.getElementById(sid).remove();
        drpStoreselect(sid);

        if ($("#SplitEqually")[0].checked == true) {
            SplitAmount();

        }
        GetAllStoreTotal();
    }
});
$('#SplitEqually').click(function () {
    document.getElementById("dvStorePercentage").classList.remove("percentage");
});
$('#Percentage').click(function () {
    document.getElementById("dvStorePercentage").classList.add("percentage");
});
$('#Custom').click(function () {
    document.getElementById("dvStorePercentage").classList.remove("percentage");
});

$(document).on('input', '#txtPercentage', function () {

    var per = $(this).val();
    var sid = $(this).prev().val();
    const formatter = new Intl.NumberFormat('en-US', {
        currency: 'USD',
        minimumFractionDigits: 2
    })

    const a = parseFloat($("#InvoiceAmount").val());
    if (a != '' && a != undefined) {
        var chk = document.getElementById(sid);
        if (chk != null) {
            var data = document.getElementById(sid).innerHTML;
            if (data != null && data != '' && data != undefined) {

                const amt = (parseFloat($("#InvoiceAmount").val()) * parseFloat(per)) / 100;
                var dptamt = document.getElementsByClassName('addamtcls-' + sid);
                if (dptamt != null) {

                    var n = amt.toFixed(2);
                    var totalamount = formatter.format(n);
                    if (totalamount != 'NaN') {
                        dptamt[0].value = totalamount.toString().replace(',', '');
                    }
                    else {
                        dptamt[0].value = "0.00";
                    }

                    var storeamt = document.getElementById('lblTotalAmount-' + sid);
                    if (storeamt != null && storeamt != undefined) {
                        if (totalamount != 'NaN') {
                            storeamt.innerHTML = totalamount.toString().replace(',', '');
                        }
                        else {
                            storeamt.innerHTML = "0.00";
                        }
                    }
                    GetAllStoreTotal();
                }
            }
        }
    }
});

$(document).on('input', '#InvoiceAmount', function () {

    //For SplitAmount
    if ($("#SplitEqually")[0].checked == true) {
        const formatter = new Intl.NumberFormat('en-US', {
            currency: 'USD',
            minimumFractionDigits: 2
        })

        const a = parseFloat($("#InvoiceAmount").val());
        if (a != '' && a != undefined) {
            var data = document.getElementById('dvbindstore');
            if (data != null && data != '' && data != undefined) {

                const amt = parseFloat($("#InvoiceAmount").val()) / parseFloat(data.children.length);
                for (var i = 0; i < data.children.length; i++) {

                    var dptamt = document.getElementsByClassName('addamtcls-' + data.children[i].id);
                    if (dptamt != null) {

                        var n = amt.toFixed(2);
                        var totalamount = formatter.format(n);
                        if (totalamount != 'NaN') {
                            dptamt[0].value = totalamount.toString().replace(',', '');
                        }
                        else {
                            dptamt[0].value = "0.00";
                        }

                        var storeamt = document.getElementById('lblTotalAmount-' + data.children[i].id);
                        if (storeamt != null && storeamt != undefined) {
                            if (totalamount != 'NaN') {
                                storeamt.innerHTML = totalamount.toString().replace(',', '');
                            }
                            else {
                                storeamt.innerHTML = "0.00";
                            }
                        }
                        GetAllStoreTotal();
                    }
                }
            }
        }
    }
});

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

const getCircularReplacer = () => {
    const seen = new WeakSet();
    return (key, value) => {
        if (typeof value === 'object' && value !== null) {
            if (seen.has(value)) {
                return;
            }
            seen.add(value);
        }
        return value;
    };
};

function SplitAmount() {
    const a = parseFloat($("#InvoiceAmount").val());
    if (a != '' && a != undefined) {
        let x = 0;
        var numItems = $('.stotal').each(function () {
            x = x + 1;
        });
        const formatter = new Intl.NumberFormat('en-US', {
            currency: 'USD',
            minimumFractionDigits: 2
        })

        if (x > 0) {
            const amt = parseFloat($("#InvoiceAmount").val()) / parseFloat(x);
            for (var i = 0; i < $('.setAmount').length; i++) {

                var n = amt.toFixed(2);
                var totalamount = formatter.format(n);

                $('.setAmount')[i].value = totalamount.toString().replace(',', '');
            }

            var numItems = $('.stotal').each(function () {

                var n = amt.toFixed(2);
                var totalamount = formatter.format(n);
                $(this)[0].innerHTML = totalamount.toString().replace(',', '');
            });
        }
        GetAllStoreTotal();
    }
}

function GetAllStoreTotal(iii) {

    var temp = 0;

    var numItems = $('.stotal').each(function () {

        if ($(this)[0].innerHTML != '') {

            temp += parseFloat($(this)[0].innerHTML.replace(/\,/g, ''));
        }
    });
    const formatter = new Intl.NumberFormat('en-US', {
        currency: 'USD',
        minimumFractionDigits: 2
    })

    var n = temp.toFixed(2);

    var totalamount = "$ " + formatter.format(n);
    if (n != "NaN") {
        $("#TotalAmount").val(n);

        ////add val in Invoice Amount textbox by himanshu 30-01-2025
        //$("#InvoiceAmount").val(n);
        ////end
    }
    if (n != "NaN") {
        $("#spanamount").html(totalamount);
    }
}

//Type addmore starts
function AddField(curIndex, iii) {

    $(".myval-" + iii).select2('destroy');
    var lastVal = $("#hdninvoicecount_" + iii + "").val();
    var nextVal = parseInt(lastVal) + 1;

    var html = $("#trrow_" + lastVal + "_" + iii).html();

    if (html == undefined) {

        const myArray = ($(".tbladdoncls_" + iii + "").find(".trrowcls_" + iii).last().attr("id")).split("_");
        lastVal = myArray[1];
        html = $("#trrow_" + lastVal + "_" + iii).html();
    }

    if ($("#hdnreqval_" + iii + "").val() == 1) {
        html = ReplaceData(html, "drpcls valid", "drpcls input-validation-error");
    }

    html = ReplaceData(html, "_" + lastVal, "_" + nextVal);

    $("#trrow_" + lastVal + "_" + iii).after("<div id='trrow_" + nextVal + "_" + iii + "' class='trrowcls_" + iii + "'>" + html + "</div>");
    console.log(html);

    document.getElementById('aRemove_' + nextVal + "-" + iii).setAttribute('onclick', 'RemoveField(' + nextVal + ',' + iii + ')')
    var yourUl = document.getElementById('aRemove_' + nextVal + "-" + iii);
    yourUl.style.display = yourUl.style.display === 'none' ? '' : 'none';

    document.getElementById('aAdd_' + (nextVal) + "-" + iii).setAttribute('onclick', 'AddField(' + (nextVal) + ',' + iii + ')')
    $("#hdninvoicecount_" + iii + "").val(nextVal);

    $("#trrow_" + (nextVal) + "_" + iii + " :input[type='text']").each(function () {
        $(this).val("");
    });
    var elements1 = document.getElementById("Drp_" + (nextVal) + "-" + iii).options;
    for (var i = 0; i < elements1.length; i++) {
        elements1[i].selected = false;
    }

    var i_rtn = CheckField(iii);

    $('.groupOfTexbox').keypress(function (event) {
        return isNumber(event, this)
    });
    $('.groupOfTexboxInt').keypress(function (event) {
        return isNumberint(event, this)
    });
    $(".myval-" + iii).select2({
        width: "50%",
    });
    $("#Amt_" + nextVal + "-" + iii).val("");
    $(".trrowcls_" + iii + "").last().find("select").focus();

}

function Select2Load(iii) {
    $(".myval-" + iii).select2();
}

function RemoveField(val, iii) {
    $("#trrow_" + val + "_" + iii).remove();
    CheckField(iii);
    AddAmt(iii);
}

function CheckField(iii) {

    var i = 0;
    i = $(".tbladdoncls_" + iii + " .trrowcls_" + iii + "").length;
    if (i == 1) {
        $(".aremove1-" + iii + "").hide();
    }
    else {
        $(".aremove1-" + iii + "").show();
    }
    return i;
}

function RemoveAmt(val, iii) {
    var finaltotal = 0;
    var Totalamt = $('#spanamount').html();
    var temp = document.getElementById('Amt_' + (val) + '-' + iii).value;

    var total = Totalamt.split("$").pop();

    finaltotal = total - temp;
    var totalamount = "$" + (finaltotal);

    $("#TotalAmtByDept").val(totalamount);
    $("#spanamount").html(totalamount);
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
                    $('#frmMain').submit(false);
                }
                else if (obj.result == "main") {
                    event.preventDefault();
                    $("#hrefopeninvoice").attr("href", ROOTURL + '/Invoices/Details/' + obj.data)
                    showpopupnew();
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
        $('#Type_id').val(['']).trigger('change');
        var typeid = $('#Type_id').val();
    }
}

function ddLTypeChange(paytype) {
    $("#QuickCRInvoice").prop('checked', false);
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
    var type = $("#Type_id").val();
    $('#Type').val(type);
}

$('#Vendor_id').change(function () {
    var vid = document.getElementById("Vendor_id").value;
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
                $("#ToolTipInstuctions").append("<label>Vendor Name</label><span class='highlight' style='margin-right: auto;'>*</span><span class='InvoiceTool'>" + Data.Instruction + "</span>");
            }
            else {
                $("#ToolTipInstuctions").append("<label>Vendor Name</label><span class='highlight' style='margin-right: auto;'>*</span><span class='InvoiceTool'>Instructions</span>");
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

function BinddiscountData() {
    var today = document.getElementById("CurrentDate").value;
    $('#Invoice_Date').datetimepicker({
        format: 'MMM DD,YYYY',
        useCurrent: false,
        maxDate: today
    });
    var discounttype = $("#discountype").val();
    var totalamtvalue = $("#totalamounrt").val();
    $("#QuickCRInvoice").prop('checked', false);

    if (discounttype === "2") {
        $("#divpercentage").show();
        $("#divdiscountamt").show();

        $("#divper").show();
        $("#divdolar").show();
        $('#Discountamount').attr('readonly', true);
        $('#SelectDeptForDisc').show();
        $('#spanamount').show();
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

        $("#divdiscountamt").show();

        $('#SelectDeptForDisc').show();
        $('#spanamount').show();
        if ($("#QuickCR").val() == "1") {
            $("#divNoNeedForCRApproval").show();
        }
        else {
            $("#divNoNeedForCRApproval").hide();
        }

    } else {
        $("#divpercentage").hide();
        $("#divdiscountamt").hide();

        $("#divper").hide();
        $("#divdolar").hide();
        $('#SelectDeptForDisc').hide();
        $('#spanamount').show();
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
        var Amount;
        for (var j = 1; j <= i; j++) {
            Amount = $("#Amt_" + j).val();
            if (Amount == "") {
                $("#Amt_" + j).val(0);
                k++
            }
            //if (Amount == "0") {
            //    k++
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