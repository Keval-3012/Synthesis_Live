
function setitem() {
    localStorage.setItem("FromInvoicePage", "");
}

function ComfirmDeleteCashInvoice(ID) {
    var Data = searchdashbord;
    $.ajax({
        url: '/Terminal/CheckInvoiceUSedAnywhere',

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

    var DivId = "#divCashInvoice_" + ID;
    $(DivId).show();
}

function closeCashInvoicemodal() {
    $(".divIDClass").hide();
}

function closemodal() {
    $(".divIDClass").hide();
}

function DeleteTenderManualEntry(ID) {
    var DivId = ID;
    $("#" + DivId).show();
}

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    console.log(charCode);
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46 && charCode !== 45)
        return false;
    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

function delete_row(no) {
    document.getElementById("row" + no + "").outerHTML = "";
}

function CreditTotal() {
    var AMEX = 0;
    var Discover = 0;
    var Master = 0;
    var Visa = 0;
    var CCOffline = 0;
    try {
        AMEX = document.getElementById("AMEX").value;
    }
    catch (err) { }
    try {
        Discover = document.getElementById("Discover").value;
    }
    catch (err) { }
    try {
        Master = document.getElementById("Master").value;
    }
    catch (err) { }
    try {
        Visa = document.getElementById("Visa").value;
    }
    catch (err) { }
    try {
        CCOffline = document.getElementById("CCOffline").value;
    }
    catch (err) { }

    if (AMEX == "" || AMEX == '') {
        AMEX = 0;
    }
    if (Discover == "" || Discover == '') {
        Discover = 0;
    }
    if (Master == "" || Master == '') {
        Master = 0;
    }
    if (Visa == "" || Visa == '') {
        Visa = 0;
    }
    if (CCOffline == "" || CCOffline == '') {
        CCOffline = 0;
    }

    var Sum = (parseFloat(AMEX) + parseFloat(Discover) + parseFloat(Master) + parseFloat(Visa) + parseFloat(CCOffline));
    $('#AMEX').parent("td").parent("tr").find(".TotalValue").removeAttr("value");
    $('#AMEX').parent("td").parent("tr").find(".TotalValue").attr("value", Sum.toFixed(2));

    var idse = 0;
    var yt = 0;

    $("#data_table tr").each(function (index, obj) {
        if (index != 0) {
            var rt = idse++;
            var dsr = $(obj).closest('tr').find('.test2').val();
            if (dsr == "") {
                dsr = "0.00";
                $(obj).closest('tr').find('.test2').val("0.00");
            }
            if (dsr != undefined) {
                yt = parseFloat(yt) + parseFloat(dsr)
            }
        }
    })

    $("#divActualTotal").text("");
    $("#divActualTotal").text("$ " + addCommas(yt.toFixed(2)));

    var bcd = $("#txtSalesAmount").val();
    if (bcd == "" || bcd == '') {
        bcd = 0;
    }

    var Difference = yt - bcd;
    Difference = addCommas(Difference.toFixed(2));
    if (Difference == 0) {
        $('.divOverspan').empty();
        $('.divOverspan').append('<span style="font-size: 40px !important;color: #000000;font-weight: 500; text-align: right;">Over/Short <b style="color:grey;font-size: 40px !important;font-weight: 300; text-align: right;">$' + addCommas(Difference) + '</b></span>');
    }
    else if (yt > bcd) {
        $('.divOverspan').empty();
        $('.divOverspan').append('<span style="font-size: 40px !important;color: #000000;font-weight: 500; text-align: right;">Over/Short <b style="color:green;font-size: 40px !important;font-weight: 300; text-align: right;">$' + addCommas(Difference) + '</b></span>');
    }
    else if (yt < bcd) {
        $('.divOverspan').empty();
        $('.divOverspan').append('<span style="font-size: 40px !important;color: #000000;font-weight: 500; text-align: right;">Over/Short <b id="divOverspan2"><span class="colorRed" style="font-size: 40px !important;font-weight: 300; text-align: right;"> $' + addCommas(Difference) + '</span></b></span>');
    }
}

function addCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".");
}

function ChackCashierName() {
    var Titleinps = document.getElementsByName('Title');
    var Amountinps = document.getElementsByName('Amount');
    var Titleresult = "false";
    for (var i = 0; i < Titleinps.length; i++) {
        var inp = Titleinps[i];
        if (inp.value === null || inp.value === "" || inp.value === '') {
            Titleresult = "true";
            break;
        }
    }
    var Amountresult = "false";
    for (var j = 0; j < Amountinps.length; j++) {
        var Amountinp = Amountinps[j];
        if (Amountinp.value === null || Amountinp.value === "" || Amountinp.value === '') {
            Amountresult = "true";
            break;
        }
    }
    if (Titleresult === "true" || Amountresult === "true") {
        event.preventDefault();
        $('#AddMoreName').show();
    }
    else {
        $('#AddMoreName').hide();
    }
}

function ConfirmDelete_Settlement(ID) {
    $("#SettlementId").val(ID);
    $("#PopDeleteSettlementEntry").show();
}
function closeSettlementmodal() {
    $(".divSettlementClass").hide();
}

$("#SubmitShift").click(function (e) {
    sessionStorage.setItem("StartTerminalId", terminal_id);
    var table = document.getElementById("data_table");
    var AMEX = 0;
    var Discover = 0;
    var Master = 0;
    var Visa = 0;
    var CCOffline = 0;
    try {
        AMEX = document.getElementById("AMEX").value;
    }
    catch (err) { }
    try {
        Discover = document.getElementById("Discover").value;
    }
    catch (err) { }
    try {
        Master = document.getElementById("Master").value;
    }
    catch (err) { }
    try {
        Visa = document.getElementById("Visa").value;
    }
    catch (err) { }
    try {
        CCOffline = document.getElementById("CCOffline").value;
    }
    catch (err) { }

    if (AMEX == "" || AMEX == '') {
        AMEX = 0;
    }
    if (Discover == "" || Discover == '') {
        Discover = 0;
    }
    if (Master == "" || Master == '') {
        Master = 0;
    }
    if (Visa == "" || Visa == '') {
        Visa = 0;
    }
    if (CCOffline == "" || CCOffline == '') {
        CCOffline = 0;
    }

    var Sum = (parseFloat(AMEX) + parseFloat(Discover) + parseFloat(Master) + parseFloat(Visa) + parseFloat(CCOffline));
    var aa = $('#AMEX').parent("td").parent("tr").find(".TotalValue").val();

    if (parseFloat(Sum) != 0 && aa != undefined) {
        if (parseFloat(Sum).toFixed(2) != parseFloat(aa).toFixed(2)) {
            MyCustomAlert("Credit cards total values are not matching with the actual credit card values.");
            e.preventDefault();
        }
    }
});