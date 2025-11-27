
function docReady1(fn) {
    // see if DOM is already available
    if (document.readyState === "complete" || document.readyState === "interactive") {
        // call on next available tick
        setTimeout(fn, 1);
    } else {
        document.addEventListener("DOMContentLoaded", fn);
    }
}
docReady1(function () {
    $('#Validation').hide();
    $('#txtstartdate').attr('autocomplete', 'off');
    $('#txtenddate').attr('autocomplete', 'off');
    $('#AmtMinimum').attr('autocomplete', 'off');
    $('#AmtMaximum').attr('autocomplete', 'off');
    Loader(0);
    var currentDate = new Date();
    $('#txtstartdate').datetimepicker({
        format: 'MM/DD/YYYY',
        useCurrent: false,
        maxDate: currentDate
    });
    $('#txtenddate').datetimepicker({
        format: 'MM/DD/YYYY',
        useCurrent: false,
        maxDate: currentDate
    });

    $('#txtstartdate').on("dp.change", function (e) {
        $('#txtenddate').data("DateTimePicker").minDate(e.date);
    });
    $('#txtenddate').click("dp.change", function (e) {
        $('#txtenddate').data("DateTimePicker").minDate($('#txtstartdate').val());
    });
    document.getElementsByName('radio').value = payment;
    bind();

    var Deptval = localStorage.getItem("InvoiceReport_Dept");
    var Startdate = localStorage.getItem("InvoiceReport_StartDate");
    var Enddate = localStorage.getItem("InvoiceReport_EndDate");
    var Payment = localStorage.getItem("InvoiceReport_Payment");
    var Status = localStorage.getItem("InvoiceReport_Status");
    var Vendor = localStorage.getItem("InvoiceReport_Vendor");
    var AmtMax = localStorage.getItem("InvoiceReport_AmtMax");
    var AmtMin = localStorage.getItem("InvoiceReport_AmtMin");

    localStorage.removeItem("InvoiceReport_Dept");
    localStorage.removeItem("InvoiceReport_StartDate");
    localStorage.removeItem("InvoiceReport_EndDate");
    localStorage.removeItem("InvoiceReport_Payment");
    localStorage.removeItem("InvoiceReport_Status");
    localStorage.removeItem("InvoiceReport_Vendor");
    localStorage.removeItem("InvoiceReport_AmtMax");
    localStorage.removeItem("InvoiceReport_AmtMin");


    if (Deptval != null) {
        $("#DrpLstdept").val(Deptval);
    }
    if (Startdate != null) {
        $("#txtstartdate").val(Startdate);
    }
    if (Enddate != null) {
        $("#txtenddate").val(Enddate);
    }
    if (Status != null) {
        $("#Drpstatus").val(Status);
    }
    if (Vendor != null) {
        $("#VendorName").val(Vendor);
    }
    if (AmtMax != null) {
        $("#AmtMaximum").val(AmtMax);
    }
    if (AmtMin != null) {
        $("#AmtMinimum").val(AmtMin);
    }

    if (Payment == "Cash") {
        $("#radio1").prop('checked', true);
    }
    else if (Payment == "Check/ACH") {
        $("#radio2").prop('checked', true);
    }
    //else if (Payment == null) {
    //    $("#radio1").prop('checked', false);
    //    $("#radio2").prop('checked', false);
    //}

    if (Deptval != null || Startdate != null || Enddate != null || Status != null || Vendor != null || AmtMax != null || AmtMin != null) {
        FunSearchRecord();
    }

});
function closemodal() {
    $(".divIDClass").hide();
}

function Setvalues() {
    localStorage.setItem("FromInvoicePage", "InvoiceReport");

    var chk;
    var chkArray = [];
    $(".chk:checked").each(function () {
        chkArray.push($(this).val());
    });

    if (chkArray.length > 1) {
        chk = '';
    }
    if (chkArray.length == 1) {
        chk = chkArray.toString();
    }
    var element_dept = document.getElementById('DrpLstdept').value;
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    var element_Status = document.getElementById('Drpstatus').value;
    var element_chkpayment = chk;
    var element_VendorName = document.getElementById('VendorName').value;
    var element_AmtMaximum = document.getElementById('AmtMaximum').value;
    var element_AmtMinimum = document.getElementById('AmtMinimum').value;

    localStorage.setItem("InvoiceReport_Dept", element_dept);
    localStorage.setItem("InvoiceReport_StartDate", element_txtstartdate);
    localStorage.setItem("InvoiceReport_EndDate", element_txtenddate);
    localStorage.setItem("InvoiceReport_Payment", element_chkpayment);
    localStorage.setItem("InvoiceReport_Status", element_Status);
    localStorage.setItem("InvoiceReport_Vendor", element_VendorName);
    localStorage.setItem("InvoiceReport_AmtMax", element_AmtMaximum);
    localStorage.setItem("InvoiceReport_AmtMin", element_AmtMinimum);
}

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
        return false;

    return true;
});
$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});  

function Minimum() {
    var Min = document.getElementById('AmtMinimum').value;
    var Max = document.getElementById('AmtMaximum').value;
    if (Min > Max) {

    }
}

function Maximum() {
    var Min = document.getElementById('AmtMinimum').value;
    var Max = document.getElementById('AmtMaximum').value;
    if (Min > Max) {
    }
}
var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }
    bind();
}

function FunSearchRecord()//Search
{
    
    var chk;
    var chkArray = [];
    $(".chk:checked").each(function () {
        chkArray.push($(this).val());
    });

    if (chkArray.length > 1) {
        chk = '';
    }
    if (chkArray.length == 1) {
        chk = chkArray.toString();
    }

    window.FunSearchRecord = FunSearchRecord
    //  var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
    var element_dept = document.getElementById('DrpLstdept').value;
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    var element_Status = document.getElementById('Drpstatus').value;
    var element_chkpayment = chk;
    var element_VendorName = document.getElementById('VendorName').value;
    var element_AmtMaximum = document.getElementById('AmtMaximum').value;
    var element_AmtMinimum = document.getElementById('AmtMinimum').value;
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_dept, element_txtstartdate, element_txtenddate, element_chkpayment, '', element_Status, element_VendorName, element_AmtMaximum, element_AmtMinimum);
}

function FunPageIndex(pageindex)//grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, Status, VendorName, AmtMaximum, AmtMinimum);
}

function FunSortData(SortData)//Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, AscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, Status, VendorName, AmtMaximum, AmtMinimum);
}

function FunPageRecord(PageRecord)//Grid Page per record
{
    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, Status, VendorName, AmtMaximum, AmtMinimum);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, Status, VendorName, AmtMaximum, AmtMinimum);
}
function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

//For Search Button
function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, dept_val, startdate_val, enddate_val, payment_val, Store_val, Status_val, VendorName_Val, AmtMaximum_val, AmtMinimum_val) {
    
    $.ajax({
        url: ROOTURL + 'InvoiceReport/grid',
        cache: false,
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, deptname: dept_val, startdate: startdate_val, enddate: enddate_val, payment: payment_val, Store_val: Store_val, Status: Status_val, VendorName: VendorName_Val, AmtMaximum: AmtMaximum_val, AmtMinimum: AmtMinimum_val },
        beforeSend: function () { Loader(1); },
        // async: false,
        success: function (response) {
            
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);

            if (payment_val == "Cash") {
                $("#radio1").prop('checked', true);
            }
            else if (payment_val == "Check/ACH") {
                $("#radio2").prop('checked', true);
            }
            else if (payment_val == "") {
                $("#radio1").prop('checked', false);
                $("#radio2").prop('checked', false);
            }
        },
        error: function () {
            Loader(0);
        }
    });
}

function bind() {
    $('#txtstartdate').attr('autocomplete', 'off');
    $('#txtenddate').attr('autocomplete', 'off');
    $('#AmtMinimum').attr('autocomplete', 'off');
    $('#AmtMaximum').attr('autocomplete', 'off');
    $(".myval").select2({
        allowclear: true,
    });
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }

    $(".select2-container").remove();
    var pay = payment;

    if (pay == "Cash") {
        $("#radio1").prop('checked', true);
    }
    else if (pay == "Check/ACH") {
        $("#radio2").prop('checked', true);
    }

    var dept = deptname;
    if (dept != '0') {
        document.getElementById('DrpLstdept').value = deptname;
    }
    document.getElementById('txtstartdate').value = startdate;
    document.getElementById('txtenddate').value = enddate;
    var Vender = VendorName;
    if (Vender != '0') {
        document.getElementById('VendorName').value = VendorName;
    }
    if (AmtMinimum != '0') {
        document.getElementById('AmtMinimum').value = AmtMinimum;
    }
    if (AmtMaximum != '0') {
        document.getElementById('AmtMaximum').value = AmtMaximum;
    }

    var statusid = Status;
    if (statusid != '0') {
        document.getElementById('Drpstatus').value = Status;
    }
    //$('#txtstartdate').datetimepicker({
    //    format: 'MM/DD/YYYY',
    //    useCurrent: false
    //});
    //$('#txtenddate').datetimepicker({
    //    format: 'MM/DD/YYYY',
    //    useCurrent: false
    //});
    // $("#radio").val(payment');
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



function Delete(ID) {
    $.ajax({
        url: ROOTURL + 'Invoices/delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, Status, VendorName, AmtMaximum, AmtMinimum);
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}
function closemodal() {
    $(".divIDClass").hide();
    //$(".overlaybox").hide();
}