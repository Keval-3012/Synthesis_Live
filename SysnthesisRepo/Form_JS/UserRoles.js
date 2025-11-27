$(".btncancel").click(function () {
    $("#UserType").val("");
})
$(function () {
    $('.Itemselect2').select2({
        allowClear: false,
        language: {
            noResults: function () {
                return ' <button type="button" data-toggle="modal" data-target="#IModal" class="btn btn-success" onclick="frmFormLoadComplete();">Add New Role</button>';
            }
        },
        escapeMarkup: function (markup) {
            return markup;
        }
    });
});

function setSelect2() {
    $(".myval").select2({ closeOnSelect: true });
    $('.Itemselect2').select2({
        allowClear: false,
        language: {
            noResults: function () {
                return ' <button type="button" data-toggle="modal" data-target="#IModal" class="btn btn-success" onclick="frmFormLoadComplete();">Add New Role</button>';
            }
        },
        escapeMarkup: function (markup) {
            return markup;
        }
    });
}

function check_uncheck_checkbox(isChecked) {
    if (isChecked) {
        $('input[name="SelectedRoles"]').each(function () {
            this.checked = true;
        });
    } else {
        $('input[name="SelectedRoles"]').each(function () {
            this.checked = false;
        });
    }
}

function PassUserTypeId(utid) {
    $('#UserTypeId').val(utid);
}
function GetStoreWiseRoles(utypeid) {
    var UserTypeId = 0;
    if (utypeid == "") {
        UserTypeId = parseFloat($('#UserTypeLatestId').val());
        $('#UserTypeId').val(UserTypeId)
    }
    else {
        UserTypeId = parseFloat(utypeid);
        $('#UserTypeId').val(UserTypeId)
    }
    var GroupId = parseFloat($('#GroupId').val());;
    $.ajax({
        type: "GET",
        url: '/UserRoles/GetStoreWiseRoles',
        data: { UserTypeId: UserTypeId, GroupId: GroupId },
        async: false,
        success: function (data) {
            $('#dvRoles').html(data);
            $("#" + UserTypeId).addClass("selected");
            setSelect2();
            Checkboxautosyncforexpenses();
            Checkboxautosyncforchecks();
        }
    });
};

function Checkboxautosyncforexpenses() {    
    $("input[type='checkbox']").change(function () {

        let checkboxValue = $(this).val();
        if (checkboxValue.includes("AddExpense")) {
            let correspondingUpdateExpenseValue = checkboxValue.replace("AddExpense", "UpdateExpense");
            let updateExpenseCheckbox = $("input[type='checkbox'][value='" + correspondingUpdateExpenseValue + "']");
            updateExpenseCheckbox.prop("checked", $(this).prop("checked"));
        }
    });

}

function Checkboxautosyncforchecks() {
    $("input[type='checkbox']").change(function () {

        let checkboxValue = $(this).val();
        if (checkboxValue.includes("AddCheck")) {
            let correspondingUpdateCheckValue = checkboxValue.replace("AddCheck", "UpdateCheck");
            let updateChecksCheckbox = $("input[type='checkbox'][value='" + correspondingUpdateCheckValue + "']");
            updateChecksCheckbox.prop("checked", $(this).prop("checked"));
        }
    });

}

function GetRoleList() {
    var GroupId = parseFloat($('#GroupId').val());
    if (GroupId > 0) {
        $.ajax({
            type: "GET",
            url: '/UserRoles/GetRoleList',
            data: { GroupId: GroupId },
            async: false,
            beforeSend: function () {
                Loader(1);
            },
            success: function (data) {
                $("#dvUserRoles").html(data);
                GetStoreWiseRoles("0");
                Loader(0);
            },
            error: function () {
                Loader(0);

            }
        });
    }
};

function GetGroupWiseStore() {
    var UserTypeId = parseFloat($('#UserTypeId').val());
    var GroupId = parseFloat($('#GroupId').val());
    if (UserTypeId > 0 && GroupId > 0) {
        $.ajax({
            type: "GET",
            url: '/UserRoles/GetGroupWiseStore',
            data: { UserTypeId: UserTypeId, GroupId: GroupId },
            async: false,
            success: function (data) {
                $('#dvRoles').html(data);
                setSelect2();
            }
        });
    }
};

function sitmclose() {
    $("#UserTypeId").select2('close');
}

function frmFormLoadComplete() {

    var text = $('.select2-search__field').val();
    $('#frmUserType').find("#UserType").val(text);
    sitmclose();
    $('#frmUserType').find(".select2").select2({ closeOnSelect: true });
    $("#UserTypeId").select2("close");
}

function AddUserType() {
    
    $("#requ").text("");
    var UserType = $('#UserType').val();
    var GroupId = $('#GroupId').val();
    var LevelsApproverId = $('#LevelsApproverId').val();
    var IsViewInvoiceOnly = false;
    if ($('#IsViewInvoiceOnly').is(':checked')) {
        IsViewInvoiceOnly = true;
    }

    if ($('#UserType').val() == "") {
        $("#requ").text(URoleRequired);
        return false;
    }
    if ($('#LevelsApproverId').val() == "" || $('#LevelsApproverId').val() == "0") {
        $("#ReqLvl").text(LevelRequired);
        return false;
    }
    $.ajax({
        url: '/UserRoles/AddUserTypeData',
        type: "post",
        cache: false,
        data: { UserType: UserType, GroupId: GroupId, LevelsApproverId: LevelsApproverId, IsViewInvoiceOnly: IsViewInvoiceOnly },
        success: function (states) {
            $("#dvUserRoles").html(states);
            $('#UserType').val("");
            $('select#LevelsApproverId').val("");
            $('#IsViewInvoiceOnly').attr('checked', false); // Unchecks it
            $('#IModal').modal('hide');
            GetStoreWiseRoles("");
        }
    });
}

function switchChannel(el) {
    // find all the elements in your channel list and loop over them
    Array.prototype.slice.call(document.querySelectorAll('ul[data-tag="channelList"] li')).forEach(function (element) {
        // remove the selected class
        element.classList.remove('selected');
    });
    // add the selected class to the element that was clicked
    el.classList.add('selected');
}

function ManageCheckboxForViewTimeCard(obj) {

    var ClsName = obj.className;
    ClsName = ClsName.replace('Remember ', '').replace('View', 'Chk');
    const chkobj1 = ClsName.split(" ");
    let chkword1 = chkobj1[0];

    if ($(obj).is(':checked')) {
        var ChkboxList = $("." + chkword1);
        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).attr("disabled", false);
        }
    } else {
        var ChkboxList = $("." + chkword1);
        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).attr("disabled", true);
            $(ChkboxList[i]).prop('checked', false);
        }
    }

    const chkobj = obj.value.split("_");
    let chkword = chkobj[0];
    if (chkword === 'ViewTimeCard') {

        var ChkboxList = $(".View_TimeCard");
        if ($(obj).is(':checked')) {

            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);
            }
        }
    }

    if ($(obj).is(':checked')) {

    } else {

        var ChkboxList = $(".removeval");
        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).prop('checked', false);
        }
    }
}

function ManageCheckboxForViewExpense(obj) {

    var ClsName = obj.className;
    ClsName = ClsName.replace('Remember ', '').replace('View', 'Chk');
    const chkobj1 = ClsName.split(" ");
    let chkword1 = chkobj1[0];

    const chkobj = obj.value.split("_");
    let chkword = chkobj[0];
    if (chkword === 'ViewExpense') {

        var ChkboxList = $(".View_Expense");
        if ($(obj).is(':checked')) {

            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);
            }
        }
    }
}

function ManageCheckbox(obj) {

    var ClsName = obj.className;
    ClsName = ClsName.replace('Remember ', '').replace('View', 'Chk');
    if ($(obj).is(':checked')) {
        var ChkboxList = $("." + ClsName);

        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).attr("disabled", false);
        }
    } else {
        var ChkboxList = $("." + ClsName);
        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).attr("disabled", true);
            $(ChkboxList[i]).prop('checked', false);
        }
    }

    const chkobj = obj.value.split("_");
    let chkword = chkobj[0];
    if (chkword === 'CreateTimeCard') {

        var ChkboxList = $(".Create_TimeCard");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }
    } else if (chkword === 'UpdateTimeCard') {

        var ChkboxList = $(".Update_TimeCard");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }
    }
    else if (chkword === 'DeleteTimeCard') {

        var ChkboxList = $(".Delete_TimeCard");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }
    }

};

function ManageCheckboxForExpense(obj) {

    const chkobj = obj.value.split("_");
    let chkword = chkobj[0];
    if (chkword === 'AllExpenseCheck') {

        var ChkboxList = $(".All_ExpenseCheck");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }

        if ($(obj).is(':checked')) {

            var ChkboxList = $(".removeexpense");
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);
            }
        }

    }
    else if (chkword === 'IncludeExpenseCheck') {

        var ChkboxList = $(".Include_ExpenseCheck");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }
        if ($(obj).is(':checked')) {

            var ChkboxList = $(".removeexpenseall");
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);
            }
        }
    }
    else if (chkword === 'ExcludeExpenseCheck') {

        var ChkboxList = $(".Exclude_ExpenseCheck");
        if ($(obj).is(':checked')) {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', true);

            }
        }
        else {
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);

            }
        }
        if ($(obj).is(':checked')) {

            var ChkboxList = $(".removeexpenseall");
            for (var i = 0; i < ChkboxList.length; i++) {
                $(ChkboxList[i]).prop('checked', false);
            }
        }
    }

};

function ManageSettingsCheckbox(obj) {

    var ClsName = obj.className;
    ClsName = ClsName.replace('Remember ', '');
    if ($(obj).is(':checked')) {
        var ChkboxList = $("." + ClsName);

        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).prop('checked', true);
        }
    } else {
        var ChkboxList = $("." + ClsName);
        for (var i = 0; i < ChkboxList.length; i++) {
            $(ChkboxList[i]).prop('checked', false);
        }
    }
};

$(document).ready(function () {
    $("#txtSearchTitle").focus();

});

function cleartextsearch() {
    document.getElementById('txtSearchTitle').value = '';
    FunSearchRecord();
}

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
    }
    else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
    //bind();
}
$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
})
function divexpandcollapse(divname, trname) {

    var div = document.getElementById(divname);
    var img = document.getElementById('img' + divname);
    var tr = document.getElementById(trname);

    if (div.style.display == "none") {
        div.style.display = "inline";
        tr.style.display = "";
        img.src = "../Content/Admin/images/btn_rowminus.png";

    } else {
        div.style.display = "none";
        tr.style.display = "none";
        img.src = "../Content/Admin/images/btn_rowplus.png";
    }
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {

    $.ajax({
        url: ROOTURL + "/UserRoles/Grid",
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val },
        beforeSend: function () {
            Loader(1);
            $("#preloader").show();
            $("#status").show();
        },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            $("#preloader").hide();
            $("#status").hide();
            Loader(0);
        },
        error: function () {
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

    $(".myval").select2({
        allowclear: true,
    });
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
    document.getElementById('txtSearchTitle').value = SearchTitle;

    $(".select2-container").remove();

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

function ComfirmDelete(ID) {
    var check = confirm("Are you sure you want to Delete this record?");
    if (check == true) {
        Delete(ID);
        GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
        return true;
    }
    else {
        return false;
    }
}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + "/UserRoles/Delete",
        data: { Id: ID },
        async: false,
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
    return true;
},
error: function() {
    Loader(0);
}
        });
    }

function closemodal() {
    $(".divIDClass").hide();
}

function IsLock(ID) {

    var lock = true;
    $.ajax({
        url: ROOTURL + "/UserMasters/ChangeStatusLock",
        data: { id: ID, active: lock },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
    Loader(0);
},
error: function() {
    Loader(0);
}
        });
    }

function IsUnLock(ID) {
    var lock = false;
    $.ajax({
        url: ROOTURL + "/UserMasters/changestatusunlock",
        data: { id: ID, active: lock },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
    Loader(0);
},
error: function() {
    Loader(0);
}
        });

    }
function ComfirmIsLock(ID) {

    var DivId = "#" + ID + "L";

    $(DivId).show();
    }
function ComfirmIsUnLock(ID) {

    var DivId = "#" + ID + "U";
    $(DivId).show();
    }
function closemodal() {

    $(".divIDClass").hide();

}
function ComfirmReset(ID) {

    $.ajax({
        url: ROOTURL + "/UserMasters/Resetpassword",
        data: { Id: ID },
        async: false,
        success: function (response) {
            $("#displayAddPanelPopup").html(response);
            $("#displayAddPanelPopup").show();
            },
error: function() {
}
        });
    }
function Reset(ID) {

    $.ajax({
        url: ROOTURL + "/UserMasters/Reset",
        data: { Id: ID },
        async: false,
        success: function (response) {

            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
},
error: function() {
}
        });
    }
function closeExcelmodal() {
    $('.modal-backdrop').addClass('hidebox');
    $('.modal-backdrop').removeClass('show');
    $('#mymodal').removeAttr("style");
}