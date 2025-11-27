function closemodal() {
    $(".divIDClass").hide();
}

function closeRightsmodal() {
    $(".divIDClass1").hide();
}

function closecashmodal(ID) {
    $('#ModelContents').html('');
    $("#divCash").hide();
}




$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
});

function GetData(Currentdate) {
    $.ajax({
        url: ROOTURL + '/Terminal/TerminalGrid',
        data: { date: Currentdate },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {

            $('#getdata').empty();
            $('#getdata').append(response);

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
        url: ROOTURL + '/Terminal/ShiftDataGrid',
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
        url: ROOTURL + '/Terminal/OtherDepositGrid',
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
        url: ROOTURL + '/Terminal/ShiftWisetenderGrid',
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
    if (amountval == "" && Optionidval == "" && Payidval == "" && vendoridval == "") {
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
    if ((vendoridval == '') && ($('#vendorid').is(':enabled'))) {
        $("#MethodIsReq").hide();
        $("#btnotherdepositsubmit").css("cursor", "no-drop");
        document.getElementById("btnotherdepositsubmit").disabled = true;
    }
    else if (($('#Payid').is(':disabled')) && ($('#Other').is(':disabled'))) {
        $("#VendorIsReq").hide();
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
            url: ROOTURL + '/Terminal/Saveotherdepositdata',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false,
            beforeSend: function () { Loader(2); },
            success: function (response) {
                if (response === "sucess") {
                    sessionStorage.setItem("StartTerminalId", Terminalval);
                    GetData(startdate);
                    MyCustomToster('Deposits added Successfully');
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
        url: ROOTURL + '/Terminal/Deleteotherdepositdata',
        data: { Id: Id },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster('Deposits deleted successfully');
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
        url: ROOTURL + '/Terminal/Updateotherdepositdata',
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster('Deposits updated Successfully');
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
        url: ROOTURL + '/Terminal/DeleteotherdepositFile',
        data: { Id: Id },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "Delete") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster('Deposits file deleted successfully');
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
        url: ROOTURL + '/Terminal/SaveSettlementEntry',
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
                MyCustomToster('Settlement entry saved Successfully');
            }
            Loader(0);
        }
    });
}

function delete_SettlementEntry() {
    var Id = document.getElementById("SettlementId").value;
    //alert(Id);
    var startdate = $('#Startdate').val();
    $.ajax({
        url: ROOTURL + '/Terminal/DeleteSettlementEntry',
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
                MyCustomToster('Settlement entry deleted Successfully');
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
    var startdates = Startdate;
    $('#Startdate').val(startdates);
    $.ajax({
        url: '/Terminal/DeleteTenderEntry',
        data: { Id: ID },
        async: false,
        success: function (response) {
            if (response === "Delete") {
                GetData(startdates);
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
        url: ROOTURL + '/Terminal/DeleteCashInvoice',
        data: { Id: ID },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(2); },
        success: function (response) {
            if (response === "sucess") {
                sessionStorage.setItem("StartTerminalId", Terminalval);
                GetData(startdate);
                MyCustomToster('Cash Invoice deleted successfully');
            }
            Loader(0);
        }
    });
}
function ConfirmDayClose() {
    var terminalId = [];
    $('input[id^="tid"]').each(function () {
        terminalId.push($(this).val());
    });
    var terminalIds = terminalId.join(',');
    var date = $('#Startdate').val();
    //for check paid and invoice amount message
    $.ajax({
        url: ROOTURL + '/Terminal/CheckPaidOutAmount?date_val=' + date + '&terminalIds=' + terminalIds,
        type: 'POST',
        cache: false,
        success: function (response) {
            if (response != "") {
                $('#PaidAmountAdjust').show();
                $(".NoshiftFilter").text(response);
                return false;
            }
        },
        error: function () {
            return false;
        }
    });
    //for check ccoffline amount message
    $.ajax({
        url: ROOTURL + '/Terminal/CheckCCOfflineAmount?date_val=' + date + '&terminalIds=' + terminalIds,
        type: 'POST',
        cache: false,
        success: function (response) {
            if (response != "") {
                $('#PaidAmountAdjust').show();
                $(".NoshiftFilter").html(response);
                return false;
            }
        },
        error: function () {
            return false;
        }
    });
    //for shift not selected message
    var shiftName = $('a.tt').first().text();
    var TerminalName = $('.liclass.active a').text();
    var formattedDate = formatDateToMMDDYYYY(date);
    if (shiftName == "Shift#") {
        $('#PaidAmountAdjust').show();
        $(".NoshiftFilter").text("There is no shift assigned for " + TerminalName + " on " + formattedDate + ".");
        return false;
    }
    var paidout = parseFloat($('#PaidOutAmount').text().trim())
    if (isNaN(paidout)) {
        paidout = 0;
    }
    if (paidout != 0) {
        $('#PaidAmountAdjust').show();
        return false;
    }
    else {
        var closeretrn = 0;
        $.ajax({
            url: ROOTURL + '/Terminal/CheckLastThirtydaysClosedOut?date_val=' + date,
            type: 'POST',
            cache: false,

            beforeSend: function () { Loader(1); },
            success: function (response) {
                Loader(0);
                if (response != "") {
                    $('#PreviousDayCheckOutPopup').show();
                    $("#ConfirmDayClose").hide();
                    $("#PreviousDisplayMessage").text("Previous Register day (" + response + ") is still open. Please consider closing it.");
                    closeretrn = 0;
                }
                else {
                    closeretrn = 1;
                }
                if (closeretrn != 0) {
                    $.ajax({
                        url: ROOTURL + '/Terminal/GetStatus',
                        data: { date: date },
                        type: 'POST',
                        cache: false,
                        beforeSend: function () { Loader(1); },
                        success: function (response) {
                            Loader(0);
                            $("#ConfirmDayClose").show();
                            if (response == "") {
                                $("#DisplayMessage").text("Are You Sure You Want To Close Out The Day?");
                            }
                            else if (response == 0) {
                                $("#DisplayMessage").text("Closing Report Is Already Generated, Are You Sure You Want To Update It?");
                            }
                            else if (response == 1) {
                                $("#DisplayMessage").text("Closing Report Is Already Approved, Are You Sure You Want To Update It?");
                            }
                            else if (response == 2) {
                                var cls = $('#BtnCloseout').attr('class');
                                if (cls.toString().includes("_red")) {
                                    $("#DisplayMessage").text("Closing Report Is Already Generated & Updated, Are You Sure You Want To Update It?");
                                }
                                else {
                                    $("#DisplayMessage").text("Closing Report Is Already Generated, Are You Sure You Want To Update It?");
                                }
                            }
                            else if (response == 3) {
                                var cls = $('#BtnCloseout').attr('class');
                                if (cls.toString().includes("_red")) {
                                    $("#DisplayMessage").text("The Register day you selected has already been closed. Would you like to close it again?");
                                }
                                else {
                                    $("#DisplayMessage").text("The Register day you selected has already been closed. Would you like to close it again?");
                                }
                            }
                            //for check previous date checkout
                            //checkPreviousDayClosedOut(date);
                        },
                        error: function () {
                            Loader(0);

                        }
                    });
                }
            },
            error: function () {
                return false;
            }
        });
        
    }
}

function checkPreviousDayClosedOut(date) {
    //$.ajax({
    //    url: ROOTURL + '/Terminal/CheckPreviousDayClosedOut?date_val=' + date,
    //    type: 'POST',
    //    cache: false,

    //    beforeSend: function () { Loader(1); },
    //    success: function (response) {
    //        Loader(0);
    //        var dateMatch = response.match(/\/Date\((-?\d+)\)\//);
    //        var timestamp = parseInt(dateMatch[1]);
    //        var dateObject = new Date(timestamp);
    //        var month = (dateObject.getMonth() + 1).toString().padStart(2, '0');
    //        var day = dateObject.getDate().toString().padStart(2, '0');
    //        var year = dateObject.getFullYear();
    //        var formattedDate = month + '/' + day + '/' + year;

    //        if (formattedDate != "01/01/1") {
    //            $('#PreviousDayCheckOutPopup').show();
    //            $("#ConfirmDayClose").hide();
    //            $("#PreviousDisplayMessage").text("Previous Register day (" + formattedDate + ") is still open. Please consider closing it.");
    //            return false;
    //        }
    //    },
    //    error: function () {
    //        return false;
    //    }
    //});
    $.ajax({
        url: ROOTURL + '/Terminal/CheckLastThirtydaysClosedOut?date_val=' + date,
        type: 'POST',
        cache: false,

        beforeSend: function () { Loader(1); },
        success: function (response) {
            Loader(0);
            if (response != "") {
                $('#PreviousDayCheckOutPopup').show();
                $("#ConfirmDayClose").hide();
                $("#PreviousDisplayMessage").text("Previous Register day (" + response + ") is still open. Please consider closing it.");
                return false;
            }
        },
        error: function () {
            return false;
        }
    });
}

function DayCloseoutPrevious() {
    var date = $('#Startdate').val();
    $.ajax({
        url: ROOTURL + '/Terminal/GetStatus',
        data: { date: date },
        type: 'POST',
        cache: false,
        beforeSend: function () { Loader(1); },
        success: function (response) {
            Loader(0);
            $("#ConfirmDayClose").show();
            $('#PreviousDayCheckOutPopup').hide();
            if (response == "") {
                $("#DisplayMessage").text("Are You Sure You Want To Close Out The Day?");
            }
            else if (response == 0) {
                $("#DisplayMessage").text("Closing Report Is Already Generated, Are You Sure You Want To Update It?");
            }
            else if (response == 1) {
                $("#DisplayMessage").text("Closing Report Is Already Approved, Are You Sure You Want To Update It?");
            }
            else if (response == 2) {
                var cls = $('#BtnCloseout').attr('class');
                if (cls.toString().includes("_red")) {
                    $("#DisplayMessage").text("Closing Report Is Already Generated & Updated, Are You Sure You Want To Update It?");
                }
                else {
                    $("#DisplayMessage").text("Closing Report Is Already Generated, Are You Sure You Want To Update It?");
                }
            }
            else if (response == 3) {
                var cls = $('#BtnCloseout').attr('class');
                if (cls.toString().includes("_red")) {
                    $("#DisplayMessage").text("The Register day you selected has already been closed. Would you like to close it again?");
                }
                else {
                    $("#DisplayMessage").text("The Register day you selected has already been closed. Would you like to close it again?");
                }
            }
        },
        error: function () {
            Loader(0);

        }
    });

}

function DayCloseoutfun() {
    var date = $('#Startdate').val();
    var aa = $("#txtSettlementDone").val();
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
            url: ROOTURL + '/Terminal/CreateCashInvoice',
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

function formatDateToMMDDYYYY(dateString) {
    var dateObj = new Date(dateString);
    var month = ('0' + (dateObj.getMonth() + 1)).slice(-2);
    var day = ('0' + dateObj.getDate()).slice(-2);
    var year = dateObj.getFullYear();
    return month + '/' + day + '/' + year;
}