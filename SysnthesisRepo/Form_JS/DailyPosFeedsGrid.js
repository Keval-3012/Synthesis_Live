function BindTerminal() {
    $('#DrpLstTerminal').html(null)
    $("#DrpLstTerminal").append("<option selected='selected' value=''>All Terminal</option>");
    $('#DrpLstTerminal').Value = "";
    var param = {
        StoreId: storeid
    };
    $.getJSON('/Report/BindTerminalByStoreId/', param, function (data) {
        $.each(data, function (i, Terminal) {
            $("#DrpLstTerminal").append(
                "<option value=" + Terminal.Value + ">" + Terminal.Text + "</option>")
        });
        if (Terminalid == "0") {
            $('#DrpLstTerminal').val('');
        }
        else {
            $('#DrpLstTerminal').val(Terminalid);
        }
    });
}

if (Terminalid == "0") {
    $('#DrpLstTerminal').val('0');
}
else {
    $('#DrpLstTerminal').val(Terminalid);
}
if (ShiftName == "0") {
    $('#DrpLstShift').val('0');
}
else {
    $('#DrpLstShift').val(ShiftName);
}
bind();

function cleartextsearch() {
    FunSearchRecord();
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

function FunPageIndex(pageindex)//grid pagination
{

    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, startdate, enddate, Storeid, Terminalid, ShiftName);
}

function FunSortData(SortData)//Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, AscVal, PageSize, Alpha, SearchRecords, startdate, enddate, Storeid, Terminalid, ShiftName);
}

function FunPageRecord(PageRecord)//Grid Page per record
{

    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, startdate, enddate, Storeid, Terminalid, ShiftName);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, startdate, enddate, Storeid, Terminalid, ShiftName);
}
//For Search Button
//function FunSearchRecord()//Search
//{
//    //var element_Store = document.getElementById('DrpLstStore').value;
//    var element_Store = 0;
//    var element_txtstartdate = document.getElementById('txtstartdate').value;
//    var element_txtenddate = document.getElementById('txtenddate').value;
//    var element_Terminal = document.getElementById('DrpLstTerminal').value;
//    var element_Shift = document.getElementById('DrpLstShift').value;
//    console.log(element_Shift);
//    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtstartdate, element_txtenddate, element_Store, element_Terminal, element_Shift);
//}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, startdate_val, enddate_val, Storeid_val, Tereminalid_val, Shift_Val) {
    console.log(Shift_Val);
    var IsFalse = true;
    $.ajax({
        url: ROOTURL + 'Report/DailyPosFeedsGrid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, startdate: startdate_val, enddate: enddate_val, Storeid: Storeid_val, Terminalid: Tereminalid_val, IsFalse: IsFalse, ShiftName: Shift_Val },
        async: false,
        beforeSend: function () { Loader(1); },
        success: function (response) {
            Loader(0);
            $('#grddata').empty();
            $('#grddata').append(response);
        }
    });
}
function bind() {
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
    if (Storeid != 0) {
        document.getElementById('DrpLstStore').value = Storeid;
    }

    var Terminalid1 = Terminalid;
    if (Terminalid1 != 0) {
        BindTerminal();
        document.getElementById('DrpLstTerminal').value = Terminalid;
    }


}


function closemodal() {

    $(".overlaytransparent").hide();
    $(".divIDClass").hide();
}

function ErrorPopup(ID) {

    var DivId = ID;
    $("#" + DivId).show();
    $(".overlaytransparent").show();
}

function Delete(ID) {

    $('.modal-backdrop').hide();
    $.ajax({
        url: '/Report/DeleteDailyPOS',
        data: { Id: ID },
        async: false,
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, startdate, enddate, Storeid, Terminalid);
            return true;
        },
        error: function () {
            return false;
        }
    });
}

function SynData() {

    var id = storeid;
    var startdate = $('#Startdate').val();
    $.ajax({
        url: ROOTURL + 'DataSync/Index',
        data: { date: startdate },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            if (response === "sucess") { }
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, startdate, enddate, Storeid, Terminalid);
            Loader(0);
        }
    });
}

function ExportExcelData() {
    var StoreId = document.getElementById('DrpLstStore').value;
    var StartDate = document.getElementById('txtstartdate').value;
    var EndDate = document.getElementById('txtenddate').value;
    var TerminalId = document.getElementById('DrpLstTerminal').value;
    alert(StoreId); alert(StartDate); alert(EndDate); alert(TerminalId);

    $.ajax({
        url: ROOTURL + 'DailyPosFeeds/ExportExcelData',
        data: { startdate: StartDate, enddate: EndDate, Storeid: StoreId, Terminalid: TerminalId },
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, StartDate, EndDate, StoreId, TerminalId);
            Loader(0);
        }
    });
}


//syncfusion code start from here
function dataBound(e) {

    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
        var span = ej.base.createElement('span', {
            id: grid.element.id + '_searchcancelbutton',
            className: 'e-clear-icon'
        });
        span.addEventListener('click', (args) => {
            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
            grid.search("");
        });
        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
    }

}


function refreshGrid() {
    var grid = document.getElementById("DailyPosFeedsGrid").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}

function created(args) {
    // extending the default UrlAdaptor
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    class CustomAdaptor extends ej.data.UrlAdaptor {
        processResponse(data, ds, query, xhr, request, changes) {
            if (!ej.base.isNullOrUndefined(data.success)) {

                toastObj.content = data.success;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.Error)) {

                toastObj.content = data.Error;
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.data))
                return data.data;
            else
                return data;
        }
    }
    var grid = document.querySelector('#DailyPosFeedsGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Report/UrlDatasourceDailyPos",
        adaptor: new CustomAdaptor()
    });
};


function ComfirmDeleteej2(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
}

function Deleteej2(ID) {
    var startdate = document.getElementById('txtstartdate').value;
    var enddate = document.getElementById('txtenddate').value;
    var terminalid = "0";
    var shiftid = "0";
    //var terminalid = document.getElementById('DrpLstTerminal').value;
    //var shiftid = document.getElementById('DrpLstShift').value;
    $.ajax({
        url: '/Report/DeleteDailyPOS',
        data: { id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("DailyPosFeedsGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/Report/UrlDatasourceDailyPos?startdate=" + encodeURIComponent(startdate) + "&enddate=" + encodeURIComponent(enddate) + "&terminalid=" + encodeURIComponent(terminalid) + "&shiftName=" + encodeURIComponent(shiftid),
                adaptor: new ej.data.UrlAdaptor()
            });
            //return true;
            toastr.success('Entry deleted successfully.');
        },
        error: function () {
            Loader(0);
        }
    });
}

function FunSearchRecord() {
    //var element_Store = document.getElementById('DrpLstStore').value;
    var startdate = document.getElementById('txtstartdate').value;
    var enddate = document.getElementById('txtenddate').value;
    var terminalid = "0";
    var shiftid = "0";
    //terminalid = document.getElementById('DrpLstTerminal').value;
    //shiftid = document.getElementById('DrpLstShift').value;
    //if (storeid === "0") {
    //    terminalid = "0";
    //    shiftid = "0";
    //}
    //else {
    //    terminalid = document.getElementById('DrpLstTerminal').value;
    //    shiftid = document.getElementById('DrpLstShift').value;
    //}
    var grid = document.getElementById("DailyPosFeedsGrid").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Report/UrlDatasourceDailyPos?startdate=" + encodeURIComponent(startdate) + "&enddate=" + encodeURIComponent(enddate) + "&terminalid=" + encodeURIComponent(terminalid) +"&shiftName=" + encodeURIComponent(shiftid),
        adaptor: new ej.data.UrlAdaptor()
    });
}