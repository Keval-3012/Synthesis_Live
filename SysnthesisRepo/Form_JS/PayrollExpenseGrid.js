

var sDate = localStorage.getItem("PayrollExpense_Date");
localStorage.removeItem("PayrollExpense_Date");
if (sDate != null) {
    $("#txtstartdate").val(sDate);
    FunSearchRecord();
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, SearchRecords_val, alpha_val, SearchTitle_val) {
    var dateval = $("#txtstartdate").val();
    $.ajax({
        url: ROOTURL + 'Report/PayrollExpenseGrid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: dateval },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);
            $("#txtstartdate").val(dateval);
            $('#txtstartdate').datetimepicker({
                format: 'MMM-YYYY',
                useCurrent: false,
                daysOfWeekDisabled: [0, 6]
            });
        },
        error: function () {
            Loader(0);
        }
    });
}

function FunSearchRecord() {
    var element_txtSearchTitle = document.getElementById('txtstartdate').value;
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtSearchTitle);
}

function FunPageIndex(pageindex)//grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val);
}

function FunSortData(SortData)//Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val);
    myCustomFn();
}

function FunPageRecord(PageRecord)//Grid Page per record
{
    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val);
}

function bind() {

    $(".myval").select2({
        allowclear: true,
    });

    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }

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
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
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
    bind();
}

 //syncfuion code start from here
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
    var grid = document.getElementById("PayrollExpenseGrid").ej2_instances[0];
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
    var grid = document.querySelector('#PayrollExpenseGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Report/UrlDatasource",
        adaptor: new CustomAdaptor()
    });
};

function PayExpanseSearchRecord() {
    var paydate = $("#txtstartdate").val();
    var grid = document.getElementById("PayrollExpenseGrid").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Report/UrlDatasource?searchdate=" + encodeURIComponent(paydate),
        adaptor: new ej.data.UrlAdaptor()
    });
}
function PayExpenseStatusDetails(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.IsSync === 1) {
        span.textContent = "Approved"
    }
    else {
        span.textContent = "Pending"
    }
    div.appendChild(span);
    return div.outerHTML;
}

function ApproveDateStatusDetails(data) {
    if (!data.ApproveDate) {
        return "";
    }

    var approveDate = new Date(data.ApproveDate);

    if (isNaN(approveDate.getTime())) {
        return "";
    }

    var year = approveDate.getFullYear();
    var month = ('0' + (approveDate.getMonth() + 1)).slice(-2);
    var day = ('0' + approveDate.getDate()).slice(-2);

    var hours = approveDate.getHours();
    var minutes = ('0' + approveDate.getMinutes()).slice(-2);
    var seconds = ('0' + approveDate.getSeconds()).slice(-2);
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12;
    hours = ('0' + hours).slice(-2);

    var formattedDateTime = `${year}-${month}-${day} ${hours}:${minutes}:${seconds} ${ampm}`;

    return formattedDateTime;
    
}
