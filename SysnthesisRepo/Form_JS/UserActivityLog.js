var top = 0;

$(document).ready(function () {

    if (StatusMessage == "Success") {
        
            MyfunSuccess();
        
    }
    if (StatusMessage == "Delete") {
        
            MyfunSuccess();
        
    }
    if (StatusMessage == "Error") {
        
            MyfunError();
        
    }
    if (StatusMessage == "NoItem") {
        
            MyfunNoItem();
        
    }
    if (StatusMessage == "Exists") {
        
            MyfunExists();
        
    }


    document.getElementById('txtSearchTitle').onkeypress = function (e) {
        if (e.keyCode == 13) {
            document.getElementById('btnSearch').click();
        }
    }
});
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

function FunPageIndex(pageindex)//grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
}

function FunSortData(SortData)//Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, AscVal, PageSize, Alpha, SearchRecords, SearchTitle);
}

function FunPageRecord(PageRecord)//Grid Page per record
{
    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, SearchTitle);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, '');
}
//For Search Button
function FunSearchRecord()//Search
{
    var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtSearchTitle);
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {
    $.ajax({
        url: ROOTURL + '/UserActivityLog/Grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);
        }
    });
}



function bind() {

    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
    document.getElementById('txtSearchTitle').value = "@ViewBag.SearchTitle";
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
        url: ROOTURL + '/UserActivityLog/delete',
        data: { Id: ID },
        async: false,
        success: function (response) {

            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, SearchTitle);
        },
        error: function() {
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

