function Edit(ID) {

    var DivQbAccountlbl = "#" + "DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "DivQbAccountddl" + ID;
    var DivValueInlbl = "#" + "DivValueInlbl" + ID;
    var DivValueInddl = "#" + "DivValueInddl" + ID;
    var DivDescriptionlbl = "#" + "DivDescriptionlbl" + ID;
    var DivDescriptiontxt = "#" + "DivDescriptiontxt" + ID;
    var DivIsActivelbl = "#" + "DivIsActivelbl" + ID;
    var DivIsActivechk = "#" + "DivIsActivechk" + ID;
    var DivSortingNolbl = "#" + "DivSortingNolbl" + ID;
    var DivSortingNotxt = "#" + "DivSortingNotxt" + ID;

    var editid = "#" + "edit_" + ID;
    var update = "#" + "update_" + ID;
    var cancel = "#" + "cancel_" + ID;

    $(DivQbAccountlbl).hide();
    $(DivQbAccountddl).show();
    $(DivValueInlbl).hide();
    $(DivValueInddl).show();
    $(DivDescriptionlbl).hide();
    $(DivDescriptiontxt).show();
    $(DivIsActivelbl).hide();
    $(DivIsActivechk).show();
    $(DivSortingNolbl).hide();
    $(DivSortingNotxt).show();

    $(editid).hide();
    $(update).show();
    $(cancel).show();
}

function Cancel(ID) {
    var DivQbAccountlbl = "#" + "DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "DivQbAccountddl" + ID;
    var DivValueInlbl = "#" + "DivValueInlbl" + ID;
    var DivValueInddl = "#" + "DivValueInddl" + ID;
    var DivDescriptionlbl = "#" + "DivDescriptionlbl" + ID;
    var DivDescriptiontxt = "#" + "DivDescriptiontxt" + ID;
    var DivIsActivelbl = "#" + "DivIsActivelbl" + ID;
    var DivIsActivechk = "#" + "DivIsActivechk" + ID;
    var DivSortingNolbl = "#" + "DivSortingNolbl" + ID;
    var DivSortingNotxt = "#" + "DivSortingNotxt" + ID;

    var editid = "#" + "edit_" + ID;
    var update = "#" + "update_" + ID;
    var cancel = "#" + "cancel_" + ID;

    $(cancel).hide();
    $(DivQbAccountlbl).show();
    $(DivQbAccountddl).hide();
    $(DivValueInlbl).show();
    $(DivValueInddl).hide();
    $(DivDescriptionlbl).show();
    $(DivDescriptiontxt).hide();
    $(DivIsActivelbl).show();
    $(DivIsActivechk).hide();
    $(DivSortingNolbl).show();
    $(DivSortingNotxt).hide();
    $(editid).show();
    $(update).hide();
}

function Updatedata(ID) {
    debugger;
    var ValueInVal = document.getElementById("EValueIn_" + ID).value;
    var QBval = document.getElementById("EQBAccount_" + ID).value;
    var DescVal = document.getElementById("EDescription_" + ID).value;
    var IsActiveVal = $("#EIsActive_" + ID).is(':checked');
    var NewSortingNo = document.getElementById("ESortingNo_" + ID).value;
    var cancel = "#" + "cancel_" + ID;
    var update = "#" + "update_" + ID;
    var Temp = $("#TempID" + ID).val();

    //if (QBval == "") {
    //    $("#EQBAccountReq" + ID).show();
    //    return false;
    //}
    //else { $("#EQBAccountReq" + ID).hide(); }

    //if (ValueInVal == "") {
    //    $("#EValueInReq" + ID).show();
    //    return false;
    //}
    //else { $("#EValueInReq" + ID).hide(); }

    // REMOVED: QBAccount validation - not required anymore
    $("#EQBAccountReq" + ID).hide();

    // REMOVED: ValueIn validation - not required anymore
    $("#EValueInReq" + ID).hide();

    // UPDATED: Use null for empty sorting number (removal)
    if (NewSortingNo == "" || NewSortingNo == null || NewSortingNo == undefined) {
        NewSortingNo = null; // Send null to remove sorting
    }

    // No validation needed - 0 is allowed for removal
    $("#ESortingNoReq" + ID).hide();

    $.ajax({
        url: ROOTURL + '/PayrollAccount/UpdatePayrollAccount',
        data: {
            ID: ID,
            QBAccountid: QBval,
            Description: DescVal,
            ValueIn: ValueInVal,
            IsActive: IsActiveVal,
            NewSortingNo: NewSortingNo
        },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(1); },
        success: function (response) {
            if (response === "Edit") {
                GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords);
                MyCustomToster('Payroll Account Updated Successfully.');
            }
            else if (response === "Exists") {
                MyCustomToster('Payroll Account Name Is Already Exists.');
            }
            Loader(0);
        }
    });
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {
    $.ajax({
        url: ROOTURL + '/PayrollAccount/grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

//function CheckDuplicate(Temp, SortingNo) {
//    var retval = false;
//    var str = "";
//    $('#data_body tr').each(function (index1) {
//        var row = $(this)
//        var row_val1 = row.find("td:nth-child(6) input").val();
//        if (index1 != Temp) {
//            if (row_val1 == SortingNo) {
//                str = row_val1;
//                retval = true;
//            }
//        }
//    })
//    if (retval == true) {
//        MyCustomToster('Sorting No "' + str + '" Duplicate');
//    }
//    return retval;
//}

function Edit1(ID) {
    
    var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;
    var DivVendorlbl = "#" + "1DivVendorlbl" + ID;
    var DivVendorddl = "#" + "1DivVendorddl" + ID;

    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    $(DivQbAccountlbl).hide();
    $(DivQbAccountddl).show();
    $(DivVendorlbl).hide();
    $(DivVendorddl).show();

    $(editid).hide();
    $(update).show();
    $(cancel).show();
}

function Cancel1(ID) {
    var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;
    var DivVendorlbl = "#" + "1DivVendorlbl" + ID;
    var DivVendorddl = "#" + "1DivVendorddl" + ID;

    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    $(cancel).hide();
    $(DivQbAccountlbl).show();
    $(DivQbAccountddl).hide();
    $(DivVendorlbl).show();
    $(DivVendorddl).hide();
    $(editid).show();
    $(update).hide();
}

function GetData1() {
    $.ajax({
        type: "GET",
        url: ROOTURL + '/PayrollAccount/PayrollBankAccount',
        success: function (response) {
            $('#grddata1').empty();
            $('#grddata1').append(response);
        }
    });
}

function Updatedata1(ID) {
    
    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    var QBval = document.getElementById("1EQBAccount_" + ID).value;
    if (QBval == "") {
        $("#1EQBAccountReq" + ID).show();
        return false;
    }
    else { $("#1EQBAccountReq" + ID).hide(); }

    var Vendorval = document.getElementById("1EVendor_" + ID).value;
    if (Vendorval == "") {
        $("#1EVendorReq" + ID).show();
        return false;
    }
    else { $("#1EVendorReq" + ID).hide(); }
    var updateBankAccount_Setting = {
        ID: ID,
        QBAccountid: QBval,
        VendorId: Vendorval,
        storeid:0
    }
    $.ajax({
        url: ROOTURL + '/PayrollAccount/UpdateBankAccount_Setting',
        //data: { ID: ID, QBAccountid: QBval, VendorId: Vendorval },
        data: updateBankAccount_Setting,
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(1); },
        success: function (response) {
            if (response === "Edit") {
                GetData1();
                MyCustomToster('Setting Updated Successfully.');
                Loader(0);

                var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
                var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;
                var DivVendorlbl = "#" + "1DivVendorlbl" + ID;
                var DivVendorddl = "#" + "1DivVendorddl" + ID;
                $(DivQbAccountlbl).show();
                $(DivQbAccountddl).hide();
                $(DivVendorlbl).show();
                $(DivVendorddl).hide();
                $(editid).show();
                $(update).hide();
                $(cancel).hide();
            }
            else if (response === "Save") {
                GetData1();
                MyCustomToster('Setting Save Successfully.');
                Loader(0);

                var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
                var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;
                var DivVendorlbl = "#" + "1DivVendorlbl" + ID;
                var DivVendorddl = "#" + "1DivVendorddl" + ID;
                $(DivQbAccountlbl).show();
                $(DivQbAccountddl).hide();
                $(DivVendorlbl).show();
                $(DivVendorddl).hide();
                $(editid).show();
                $(update).hide();
                $(cancel).hide();
            }
        }

    });
}