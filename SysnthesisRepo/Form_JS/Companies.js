$(document).ready(function () {
    $('#frmuser').attr('autocomplete', 'off');
    $("#Address1").attr("autocomplete", "nope");
    $("#Address2").attr("autocomplete", "nope");
    $("#StoreNo").attr("autocomplete", "nope");
});

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

function cleartextsearch() {
    document.getElementById('txtSearchTitle').value = '';
    FunSearchRecord();
}

function onCompanyGroupFilterChange(selectElement) {
    var selectedGroupId = parseInt(selectElement.value) || 0;
    console.log("Group filter changed to:", selectedGroupId);
    GetDataWithGroup(selectedGroupId);
}

function GetDataWithGroup(groupId) {
    $.ajax({
        url: ROOTURL + "/Companies/grid",
        data: {
            IsBindData: 1,
            currentPageIndex: 1,
            orderby: 'StoreId',
            IsAsc: 0,
            PageSize: 100,
            SearchRecords: 1,
            Alpha: '',
            SearchTitle: '',
            GroupId: groupId
        },
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
    }
    else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
    bind();
}

$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
})

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {
    // Get current group filter
    var groupDropdown = document.getElementById("CompanyGroupFilter");
    var currentGroupId = groupDropdown ? parseInt(groupDropdown.value) || 0 : 0;
    $.ajax({
        url: ROOTURL + "/Companies/grid",
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val, GroupId: currentGroupId },
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

document.getElementById('txtSearchTitle').onkeypress = function (e) {
    if (e.keyCode == 13) {
        document.getElementById('btnSearch').click();
    }
}

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
    var DivId = "#" + ID + "D";
    $(DivId).show();
}


function Delete(ID) {
    $.ajax({
        url: ROOTURL + "/Companies/DeleteConfirmed",
        data: { Id: ID },
        async: false,
        success: function (response) {
            if (response == Deletesucc) {
                MyCustomToster(response);
                GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
                $(".divIDClass").hide();
            }
            else if (response != "") {
                alert(response);
                $(".divIDClass").hide();
            }
        },
        error: function () {
            Loader(0);
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}