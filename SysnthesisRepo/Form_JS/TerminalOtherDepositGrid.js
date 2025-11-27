

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
        url: '/Terminal/CheckSalesOtherDeposite_UsedAnywhere',
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
function closemodal() {
    $(".divIDClass").hide();
}

function ConfirmDeleteFile(ID) {
    $("#FileDepositId").val(ID);
    $("#PopDeleteOtherDepositFile").show();
}