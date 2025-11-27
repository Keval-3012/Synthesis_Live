$(document).ready(function () {

    $("#txtSearchTitle").focus();
    bind();
    $(".add-row").click(function () {
        var table = document.getElementById("data_body");
        var table_len = ($("#data_table tbody tr").length);
        var row = table.insertRow(table_len).outerHTML = "<tr id='row" + table_len + "' class='even set_1_new'><td><input type='text' name='Name' id='Name_" + table_len + "' class='form-control required' style='vertical-align:middle;text-align:left;'/><span class='highlight msg_new'>*</span><span class='highlight' id='NameIsReq_" + table_len + "'>Name Is Required</span></td><td><input type='hidden' id='Id_" + table_len + "' name='Id' value=''><input type='hidden' id='DetailId_" + table_len + "' name='Id' value=''><select class='myval drpcls' id='QBAccount_" + table_len + "' name='QBAccount' onchange = 'javascript:GetId(this.id,this.value);GetCustomer_Vendor(this.id,this.value);GetDetailAccountId(this.id,this.value);'><option value=''>Select QB Account</option></select><span class='highlight msg_new'>*</span><span class='highlight' id='QBAccountReq_" + table_len + "'>QB Account Is Required</span></td><td><input type='hidden' id='Id_" + table_len + "' name='Id' value=''><select class='myval drpcls' id='TypicalBal_" + table_len + "' name='TypicalBal_' onchange = 'javascript:GetIdTypicalbal(this.id,this.value);'><option value=''>Select Typical Balance</option></select><span class='highlight msg_new'>*</span><span class='highlight' id='TypicalBalReq_" + table_len + "'>Typical Balance Is Required</span></td><td><input type='text' name='memo' id='memo_" + table_len + "' class='form-control required' style='vertical-align:middle;text-align:left;'/><span class='highlight msg_new'>*</span><span class='highlight' id='memoReq_" + table_len + "'>Memo Is Required</span></td><td id='divCst_" + table_len + "'><input type='hidden' id='CId_" + table_len + "' name='CId' value=''><select class='myval drpCustcls' id='Customer_" + table_len + "' name='CustomerName' ><option value=''>Select Customer</option></select><span class='highlight msg_new'>*</span><a href='#' id='btnSync_" + table_len + "' onclick='QBSyncVendor_Dept()' style='margin:0px;' class='Chiledred_cor green_cor'>Refresh</a><span class='highlight' id='CustomerReq_" + table_len + "'>Entity Is Required</span></td><td class='text-right'><input type='button' value='Add' id='" + table_len + "' class='Add_btn btn_orange mr5' onclick='javascript:SaveOtherdeposit(this.id)'><input type='button' value='Cancel' id='" + table_len + "' class='Add_btn btn_orange' onclick='javascript:FunCancelAdd(this.id)'></td><td id='testdiv_'" + table_len + ">&nbsp;</td></tr>";

        $("#NameIsReq_" + table_len).hide();
        $("#QBAccountReq_" + table_len).hide();
        $("#TypicalBalReq_" + table_len).hide();
        $("#memoReq_" + table_len).hide();
        $("#CustomerReq_" + table_len).hide();
        $('#Customer_' + table_len).hide();
        $('#btnSync_' + table_len).hide();
        $('#divCst_' + table_len).hide();
        $('#testdiv_' + table_len).show();

        $('#TypicalBal_' + table_len).append(
            "<option value='1'>Credit</option><option value = '2' > Debit</option >"
        )
        $('#QBAccount_' + table_len).Value = "";
        $.getJSON('/GroupAccountMasters/GetQBAccount', function (data) {

            $.each(data, function (i, Desc) {
                $('#QBAccount_' + table_len).append(
                    "<option value='" + Desc.Value + "'>" + Desc.Text + "</option>")
            });

            $(".myval").select2({
                allowclear: true,
            });
        });

        $(".decimalOnly").bind('keypress', function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46)
                return false;

            return true;
        });
        $(".decimalOnly").bind("paste", function (e) {
            e.preventDefault();
        });
    });

    $('#txtSearchTitle').keypress(function (e) {
        if (e.which == 13) {
            $('#btnSearch').click();
        }
    });
});

function SaveOtherdeposit(Id) {
    var Nameval = document.getElementById("Name_" + Id).value;
    var QBval = document.getElementById("QBAccount_" + Id).value;
    var Typicalbalval = document.getElementById("TypicalBal_" + Id).value;
    var memoval = document.getElementById("memo_" + Id).value;
    var EntrityID = 0;

    if (document.getElementById('Customer_' + Id) != null) {
        EntrityID = document.getElementById("Customer_" + Id).value;
    }
    var DetailID = 0;
    if (document.getElementById('DetailId_' + Id) != null) {
        DetailID = document.getElementById("DetailId_" + Id).value;
    }

    var Flg = "0";

    if (Nameval == "") {
        $("#NameIsReq_" + Id).show();
        Flg = "1";
    } else {
        $("#NameIsReq_" + Id).hide();
    }

    if (QBval == "") {
        $("#QBAccountReq_" + Id).show();
        Flg = "1";
    } else {
        $("#QBAccountReq_" + Id).hide();
    }
    if (Typicalbalval == "") {
        $("#TypicalBalReq_" + Id).show();
        Flg = "1";
    } else {
        $("#TypicalBalReq_" + Id).hide();
    }
    if (memoval == "") {
        $("#memoReq_" + Id).show();
        Flg = "1";
    } else {
        $("#memoReq_" + Id).hide();
    }
    if (DetailID == "49" || DetailID == "1") {
        if (EntrityID == "") {
            $("#CustomerReq_" + Id).show();
            Flg = "1";
        } else {
            $("#CustomerReq_" + Id).hide();
        }
    }

    if (Flg == "1") {
        return false;
    }
    $.ajax({
        url: '/GroupAccountMasters/SaveGroupMaster',
        data: {
            Name: Nameval,
            QBAccountid: QBval,
            Typicalbalid: Typicalbalval,
            memo: memoval,
            EntityVal: EntrityID
        },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            if (response === "sucess") {
                var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
                GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtSearchTitle);
                MyCustomToster('Group Name Created Successfully.');
            } else if (response === "Exists") {
                MyCustomToster(AlreadyExists);
            }
            Loader(0);
        }

    });

}

function GetId(Id, Name) {

    var index = Id.substr(9, 2);
    $.ajax({
        url: '/GroupAccountMasters/GetQBAccountId',
        data: {
            QBId: Name
        },
        success: function (response) {
            $('#Id_' + index).val(response);
        }
    });
}

function GetIdTypicalbal(Id, Name) {

    var index = Id.substr(9, 2);
    $.ajax({
        url: '/GroupAccountMasters/GetTypicalBalanceId',
        data: {
            Typicalbalidval: Name
        },
        success: function (response) {

            $('#Id_' + index).val(response);
        }
    });
}

function GetDetailAccountId(Id, Name) {

    var index = Id.substring(Id.indexOf('_') + 1)
    $('#Customer_' + index).hide();
    $('#btnSync_' + index).hide();
    $('#divCst_' + index).hide();
    $('#testdiv_' + index).show();
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/GetDetailAccountId',
        data: {
            QBId: Name
        },
        success: function (response) {
            $('#DetailId_' + index).val(response);
            if (response == "49" || response == "1") {
                //$('#CustomerReq_' + index).show();
                $('#Customer_' + index).show();
                $('#btnSync_' + index).show();
                $('#divCst_' + index).show();
                $('#testdiv_' + index).hide();
            }
        }
    });
}

function GetDetailAccountId1(Id, Name) {
    var index = Id.substring(Id.indexOf('_') + 1) //Id.charAt(Id.length - 1);
    //$('#CustomerReq_' + index).hide();
    $('#ECustomer_' + index).hide();
    $("#DivCustomernamedrp" + index).hide();
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/GetDetailAccountId',
        data: {
            QBId: Name
        },
        success: function (response) {
            $('#EDetailId_' + index).val(response);
            if (response == "49" || response == "1") {
                //$('#CustomerReq_' + index).show();
                $('#ECustomer_' + index).show();
                $("#DivCustomernamedrp" + index).show();
                $("#DivCustomernamedrp" + index).css("display", "flex");
            }
        }
    });
}

function GetCustomer_Vendor(Id, Name) {
    var table_len = Id.substring(Id.indexOf('_') + 1);
    $('#Customer_' + table_len).html("");
    $('#Customer_' + table_len).html("<option value=''>Select Entity</option>")
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/GetCustomerVendor',
        data: {
            QBId: Name
        },
        success: function (response) {
            $.each(response, function (i, Desc) {
                $('#Customer_' + table_len).append(
                    "<option value='" + Desc.Value + "'>" + Desc.Text + "</option>")
            });
            $(".myval").select2({
                allowclear: true,
            });
        }
    });
}

function GetCustomer_Vendor1(Id, Name) {
    var table_len = Id.substring(Id.indexOf('_') + 1);
    var aa = $("#SelectedEntity_" + table_len).val();

    $('#ECustomer_' + table_len).hide();
    $('#ECustomer_' + table_len).html("");
    $('#ECustomer_' + table_len).html("<option value=''>Select Entity</option>")
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/GetCustomerVendor',
        data: {
            QBId: Name
        },
        success: function (response) {
            $.each(response, function (i, Desc) {
                $('#ECustomer_' + table_len).append(
                    "<option value='" + Desc.Value + "' " + (Desc.Value == aa ? "selected" : "") + ">" + Desc.Text + "</option>")
            });
            $('#ECustomer_' + table_len).show();
            $(".myval").select2({
                allowclear: true,
            });
        }
    });
}

function QBSyncVendor_Dept() {
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/QBSyncVendor_Dept',
        type: "POST",
        data: {},
        success: function (data) {
            if (data == 'Success') {
                toastr.success(EntUpdate);
            }
        },
        error: function (result) { }
    });
}

function cleartextsearch() {
    document.getElementById('txtSearchTitle').value = '';
    FunSearchRecord();
}
var top = 0;

function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $("#secloader").removeClass('pace-active1');
        $("#secloader").addClass('pace-active');
        $("#dvloader").removeClass('bak1');
        $("#dvloader").addClass('bak');
        $("#globalFooter").addClass('LoaderFooter');
        top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
    } else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
    bind();
}

function FunPageIndex(pageindex) //grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
}

function FunSortData(SortData) //Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, AscVal, PageSize, Alpha, SearchRecords, SearchTitle);
}

function FunPageRecord(PageRecord) //Grid Page per record
{

    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, SearchTitle);
}

function FunAlphaSearchRecord(alpha) //Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, '');
}
//For Search Button
function FunSearchRecord() //Search
{

    var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;

    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtSearchTitle);

}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {

    $.ajax({
        url: '/GroupAccountMasters/grid',
        data: {
            IsBindData: IsBindData_val,
            currentPageIndex: PageIndex,
            orderby: orderby_val.trim(),
            IsAsc: isAsc_val,
            PageSize: PageSize_val,
            SearchRecords: SearchRecords_val,
            Alpha: alpha_val,
            SearchTitle: SearchTitle_val
        },
        beforeSend: function () {
            Loader(1);
        },
        // async: false,
        success: function (response) {

            //    $("body").html(response);
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);
        }
    });
}


//document.getElementById('txtSearchTitle').onkeypress = function (e) {
//    if (e.keyCode == 13) {
//        document.getElementById('btnSearch').click();
//    }
//}

function bind() {
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
    $(".myval").select2({
        //placeholder: "select",
        allowclear: true,
        //minimumResultsForSearch: -1
    });
    document.getElementById('txtSearchTitle').value = SearchTitle;

}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();

}

function Edit(ID) {

    var Divnamelbl = "#" + "Divnamelbl" + ID;
    var Divnametxt = "#" + "Divnametxt" + ID;
    var DivQbAccountlbl = "#" + "DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "DivQbAccountddl" + ID;
    var Divtypicalnamelbl = "#" + "Divtypicalnamelbl" + ID;
    var Divtypicalnameddl = "#" + "Divtypicalnameddl" + ID;
    var Divmemolbl = "#" + "Divmemolbl" + ID;
    var Divmemotxt = "#" + "Divmemotxt" + ID;
    var DivCustomernamedrp = "#" + "DivCustomernamedrp" + ID;
    var DivCustomernamelbl = "#" + "DivCustomernamelbl" + ID;

    var editid = "#" + "edit_" + ID;
    var update = "#" + "update_" + ID;
    var DivId = "#" + "Delete_" + ID;
    var cancel = "#" + "cancel_" + ID;
    //$('select[id^="EQBAccount_' + ID + '"] option:contains(' + $(DivQbAccountlbl).text() + ')').attr("selected", "selected");
    //$(".myval").select2({
    //    allowclear: true,
    //});
    //$('select[id^="Etypicalname_' + ID + '"] option:contains(' + $(Divtypicalnamelbl).text() + ')').attr("selected", "selected");
    //$(".myval").select2({
    //    allowclear: true,
    //});

    $(Divnamelbl).hide();
    $(DivQbAccountlbl).hide();
    $(Divtypicalnamelbl).hide();
    $(Divmemolbl).hide();
    $(DivCustomernamelbl).hide();

    $(Divnametxt).show();
    $(DivQbAccountddl).show();
    $(Divtypicalnameddl).show();
    $(Divmemotxt).show();
    $(DivCustomernamedrp).hide();

    GetDetailAccountId1("EQBAccount_" + ID, document.getElementById("EQBAccount_" + ID).value);
    GetCustomer_Vendor1("EQBAccount_" + ID, document.getElementById("EQBAccount_" + ID).value);

    $(editid).hide();
    $(DivId).hide();
    $(update).show();
    $(cancel).show();

}

function FunCancelAdd(ID) {
    // $("#row"+ID).hide();
    document.location.reload();
}

function Cancel(ID) {
    var Divnamelbl = "#" + "Divnamelbl" + ID;
    var Divnametxt = "#" + "Divnametxt" + ID;
    var DivQbAccountlbl = "#" + "DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "DivQbAccountddl" + ID;
    var Divtypicalnamelbl = "#" + "Divtypicalnamelbl" + ID;
    var Divtypicalnameddl = "#" + "Divtypicalnameddl" + ID;
    var Divmemolbl = "#" + "Divmemolbl" + ID;
    var Divmemotxt = "#" + "Divmemotxt" + ID;
    var DivCustomernamedrp = "#" + "DivCustomernamedrp" + ID;
    var DivCustomernamelbl = "#" + "DivCustomernamelbl" + ID;

    var editid = "#" + "edit_" + ID;
    var update = "#" + "update_" + ID;
    var DivId = "#" + "Delete_" + ID;
    var cancel = "#" + "cancel_" + ID;

    $(cancel).hide();
    $(Divnamelbl).show();
    $(DivQbAccountlbl).show();
    $(Divtypicalnamelbl).show();
    $(Divmemolbl).show();
    $(DivCustomernamelbl).show();

    $(Divnametxt).hide();
    $(DivQbAccountddl).hide();
    $(Divtypicalnameddl).hide();
    $(Divmemotxt).hide();
    $(DivCustomernamedrp).hide();

    $(editid).show();
    $(DivId).show();
    $(update).hide();
}

function Updatedata(ID) {
    var Nameval = document.getElementById("EName_" + ID).value;
    var QBval = document.getElementById("EQBAccount_" + ID).value;
    var Typicalbalval = document.getElementById("Etypicalname_" + ID).value;
    var memoval = document.getElementById("Ememo_" + ID).value;
    var cancel = "#" + "cancel_" + ID;
    var update = "#" + "update_" + ID;
    var EntrityID = null;
    if ($("#DivCustomernamedrp" + ID).is(':visible')) {
        EntrityID = document.getElementById("ECustomer_" + ID).value;
    }
    var DetailID = document.getElementById("EDetailId_" + ID).value;

    if (Nameval == "") {
        $("#ENameIsReq" + ID).show();
        $("#EQBAccountReq" + ID).hide();
        $("#ETypicalBalReq" + ID).hide();
        $("#EmemoReq" + ID).hide();
        return false;
    } else {
        $("#NameIsReq_" + ID).hide();
    }

    if (QBval == "") {
        $("#ENameIsReq" + ID).hide();
        $("#EQBAccountReq" + ID).show();
        $("#ETypicalBalReq" + ID).hide();
        $("#EmemoReq" + ID).hide();
        return false;
    } else {
        $("#EQBAccountReq" + ID).hide();
    }
    if (Typicalbalval == "") {
        $("#ENameIsReq" + ID).hide();
        $("#EQBAccountReq" + ID).hide();
        $("#ETypicalBalReq" + ID).show();
        $("#EmemoReq" + ID).hide();
        return false;
    } else {
        $("#ETypicalBalReq" + ID).hide();
    }
    if (memoval == "") {
        $("#ENameIsReq" + ID).hide();
        $("#EQBAccountReq" + ID).hide();
        $("#ETypicalBalReq" + ID).hide();
        $("#EmemoReq" + ID).show();
        return false;
    } else {
        $("#EmemoReq" + ID).hide();
        $(cancel).hide();
        $(update).hide();
    }
    if (DetailID == "49" || DetailID == "1") {
        if (EntrityID == "") {
            $("#ECustomerReq" + ID).show();
            return false;
        } else {
            $("#ECustomerReq" + ID).hide();
        }
    }


    $.ajax({
        url: '/GroupAccountMasters/UpdateGroupMaster',
        data: {
            ID: ID,
            Name: Nameval,
            QBAccountid: QBval,
            Typicalbalid: Typicalbalval,
            memo: memoval,
            EntityVal: EntrityID
        },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            if (response === "Edit") {
                // $(".other-deposite").slideToggle("slow");
                GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
                MyCustomToster('Group Name Updated Successfully.');
            } else if (response === "Exists") {
                MyCustomToster(AlreadyExists);
            }
            Loader(0);
        }

    });
}

function Delete(ID) {

    $.ajax({
        url: '/GroupAccountMasters/Delete',
        data: {
            Id: ID
        },
        async: false,
        success: function (response) {

            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
            //return true;
        },
        error: function () {
            //return false;
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}