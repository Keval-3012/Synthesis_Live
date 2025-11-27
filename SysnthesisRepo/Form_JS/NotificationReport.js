function Setvalues() {
    localStorage.setItem("FromInvoicePage", "");
}

$(document).ready(function () {
});

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

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {

    $.ajax({
        url: '@AdminSiteConfiguration.GetURL()/NotificationReport/Grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
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
    document.getElementById('SelectRecords').value = PageSize;
    document.getElementById('txtSearchTitle').value = SearchTitle;
    $(".myval").select2({
        allowclear: true,
    });
}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Delete(ID) {

    $.ajax({
        url: '@AdminSiteConfiguration.GetURL()/UserActivityLog/Delete',
        data: { Id: ID },
        async: false,
        success: function (response) {

            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
        },
        error: function () {
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

