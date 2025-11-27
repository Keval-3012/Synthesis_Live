$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode !== 46 && charCode !== 45)
        return false;

    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

function ReplaceData(html_val, c_val, r_val) {

    while (html_val.indexOf(c_val) != -1) {
        html_val = html_val.replace((c_val), (r_val));
    }
    return html_val;
}

function Editfunction() {

    var DebitVal = $("#Iddebtot").text();
    var debit = DebitVal.replace("$", "");
    var Creditval = $("#Idcredtot").text();
    var credit = Creditval.replace("$", "");
    var debit_value = $("#hnddebitvalue").val();
    var credit_value = $("#hndcreditvalue").val();
    if (parseFloat(debit_value) == parseFloat(credit_value)) {
        $(".txtedit").show();
        $("#DivApprove").show();
        $("#DivDesableApprove").hide();
        $(".despan").hide();
        $(".crspan").hide();
        $(".clsbtnsubmit").show();
        $(".clsbtnedit").hide();
    }
    else {

        $(".clsgrey2").addClass("bg_greay");
        $(".clsgrey").addClass("bg_greay brd_greay");
        $(".txtedit").show();
        $(".despan").hide();
        $(".crspan").hide();
        $(".clsbtnsubmit").show();
        $(".clsbtnedit").hide();
        $("#DivApprove").hide();
        $("#DivDesableApprove").show();
        $("#lbl").show();
    }
}

$(document).ready(function () {

    var debit_value = $("#hnddebitvalue").val();
    var credit_value = $("#hndcreditvalue").val();
    var DebitVal = $("#Iddebtot").text();
    var debit = DebitVal.replace("$", "");
    var Creditval = $("#Idcredtot").text();
    var credit = Creditval.replace("$", "");
    if (parseFloat(debit_value) == parseFloat(credit_value)) {
        $("#DivApprove").show();
    }
    else {
        $("#DivApprove").hide();
    }
    var hdnstoreid = $("#HndStoreID").val();
    var inputF = $("#HndCloseoutdate").val();
    MarkAsApprove
    //$("#lblclosedate").html(inputF);
    $("#lblclosedate").replaceWith(
        `<a id="lblclosedate" href="/Terminal/Index?StoreId=${hdnstoreid}&TerminalId=&StartDate=${encodeURIComponent(inputF)}&shiftid=" target="_blank" style="color: #385585;">${inputF}</a>`
    );
    $('#txtstartdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: false
    });
});

function closemodal() {
    $(".divIDClass").hide();
}

function Configuration() {
    $("#popupConfiguration").show();
}

function ConfirmApprove(ID, Status) {
    $("#popupApproveAlert").show();
    $("#DisplayIdNumber").val(ID);
    if (Status == "Pending" || Status == "") {
        $("#DisplayStatus").text(SureApprove);
    }
    else {
        $("#DisplayStatus").text(AlreadyUpdated);
    }
}

function MarkAsApprove() {
    var ID = $("#DisplayIdNumber").val();
    var Status = $("#DisplayStatus").text();
    $.ajax({
        url: ROOTURL + '/JournalEntries/MarkAsApprove',
        data: { Id: ID },
        async: false,
        beforeSend: function () { Loader(1); },
        success: function (response) {
            Loader(0);
            if (Status == SureApprove) {
                MyCustomToster(Approve);
            }
            else {
                MyCustomToster(ReApprove);
            }
            window.location.href = ROOTURL + '/JournalEntries/JournalEntriesIndex';
        },
        error: function () {
            Loader(0);
        }
    });
    $(".myval").select2({
        allowclear: true,
    });
}

function setItems() {
    var dateval = $("#txtstartdate").val();
    localStorage.setItem("JournalEntry_Date", dateval);
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, SearchRecords_val, alpha_val, SearchTitle_val) {
    var dateval = $("#txtstartdate").val();

    $.ajax({
        url: ROOTURL + '/JournalEntries/grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: dateval },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            $('#txtstartdate').datetimepicker({
                format: 'MMM-YYYY',
                useCurrent: false,
                daysOfWeekDisabled: [0, 6]

            });
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });

}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + '/JournalEntries/Delete',
        data: { id: ID },
        async: false,
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, SearchRecords, Alpha, '');
            return true;
        },
        error: function () {

            Loader(0);
        }
    });
}


//this all code for syncfusion grid
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
    var grid = document.getElementById("JournalEntriesGrid").ej2_instances[0];
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
    var grid = document.querySelector('#JournalEntriesGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/JournalEntries/UrlDatasource",
        //insertUrl: "/HRStoreManagers/InsertStoreManager",
        //updateUrl: "/HRStoreManagers/UpdateStoreManager",
        //removeUrl: "/HRStoreManagers/RemoveDepartment",
        adaptor: new CustomAdaptor()
    });
};


function ComfirmDeleteej2(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
}

function Deleteej2(ID) {
    var saledate = $("#txtstartdate").val();
    $.ajax({
        url: '/JournalEntries/Delete',
        data: { id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("JournalEntriesGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/JournalEntries/UrlDatasource?saledate=" + encodeURIComponent(saledate),
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

function JournalSearchRecord() {
    var saledate = $("#txtstartdate").val();
    var grid = document.getElementById("JournalEntriesGrid").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/JournalEntries/UrlDatasource?saledate=" + encodeURIComponent(saledate),
        adaptor: new ej.data.UrlAdaptor()
    });
}

function JournalEntryStatusDetails(e) {    
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
