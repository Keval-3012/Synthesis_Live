function ShowDetailsInfo(InvoiceID) {
    $.ajax({
        url: '/Invoices/GetInvoiceDetails',
        type: "GET",
        dataType: "JSON",
        data: { InvoiceID: InvoiceID },
        success: function (states) {

            var str = '';
            $.each(states, function (data, value) {
                str += '<li><span>' + value.DepartmentName + '</span><span>$ ' + value.Amount + '</span></li>';
            });
            $("#spnAmount_" + InvoiceID).append('<span class="tooltipTD"><div class="depTooltip">' + str + '</div></span>');
        }
    });
}

function Setvalues() {
    localStorage.setItem("FromInvoicePage", "");
}

function setitems() {
    var Min = document.getElementById('AmtMinimum').value;
    var Max = document.getElementById('AmtMaximum').value;
    var startdate = document.getElementById('txtstartdate').value;
    var enddate = document.getElementById('txtenddate').value;
    var payment = $("#radio1").prop("checked");
    var payment1 = $("#radio2").prop("checked");
    var Dept = document.getElementById('DrpLstdept').value;
    localStorage.setItem("DashboardFilter_AmtMinimum", Min);
    localStorage.setItem("DashboardFilter_AmtMaximum", Max);
    localStorage.setItem("DashboardFilter_StartDate", startdate);
    localStorage.setItem("DashboardFilter_EndDate", enddate);
    if (payment == true) {
        payment = $("#radio1").val();
    }
    else {
        payment = null;
    }
    if (payment1 == true) {
        payment1 = $("#radio2").val();
    }
    else {
        payment1 = null;
    }
    localStorage.setItem("DashboardFilter_Payment1", payment);
    localStorage.setItem("DashboardFilter_Payment2", payment1);
    localStorage.setItem("DashboardFilter_Department", Dept);

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

$(document).ready(function () {
    $('#txtstartdate').attr('autocomplete', 'off');
    $('#txtenddate').attr('autocomplete', 'off');
    $("#search-box").attr('autocomplete', 'off');
    $("#search-box").focus();
    Loader(0);
    $('#txtstartdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: false
    });
    $('#txtenddate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: false
    });

    $('#txtstartdate').on("dp.change", function (e) {
        $('#txtenddate').data("DateTimePicker").minDate(e.date);
    });
    $('#txtenddate').click("dp.change", function (e) {
        $('#txtenddate').data("DateTimePicker").minDate($('#txtstartdate').val());
    });

    document.getElementsByName('radio').value = Payment;
    var searchval = '';
    if (searchval != null) {
        document.getElementById('search-box').value = (searchval).replace(/&amp;/g, '&');
        $("#search-box").val((searchval).replace(/&amp;/g, '&'));
    }

    bind();
});

function Minimum() {
    var Min = document.getElementById('AmtMinimum').value;
    var Max = document.getElementById('AmtMaximum').value;
    if (Min > Max) {
        $('#AmtMaximum').val('');
    }
}

function Maximum() {
    var Min = document.getElementById('AmtMinimum').value;
    var Max = document.getElementById('AmtMaximum').value;
    if (Min > Max) {
        $('#AmtMaximum').val('');
    }
}

function cleartextsearch() {
    var searchdashbord_val = '';
    $.ajax({
        url: '/Invoices/Grid',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: 'Clear', AmtMaximum: '0', AmtMinimum: '0'
},
beforeSend: function () { Loader(1); },
async: false,
    success: function (response) {
        $('#grddata').empty();
        $('#grddata').append(response);
        localStorage.removeItem("DashboardFilter_SearchVendor");
        localStorage.removeItem("Dashboard_SearchFlg");
        $('#search-box').val("");
        Loader(0);
    },
error: function() {
    Loader(0);
}
        });
}


function undofilters() {
    $.ajax({
        url: ROOTURL + '/Invoices/ClearSession',
        data: {},
        beforeSend: function () {
        },
        success: function (response) {
        }
    });

    var searchdashbord_val = '';
    localStorage.removeItem("DashboardFilter_SearchVendor");
    localStorage.removeItem("Dashboard_SearchFlg");
    localStorage.removeItem("DashboardFilter_AmtMinimum");
    localStorage.removeItem("DashboardFilter_AmtMaximum");
    localStorage.removeItem("DashboardFilter_StartDate");
    localStorage.removeItem("DashboardFilter_EndDate");
    localStorage.removeItem("DashboardFilter_Payment1");
    localStorage.removeItem("DashboardFilter_Payment2");
    localStorage.removeItem("DashboardFilter_Department");
    $.ajax({
        url: '/Invoices/Grid',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: 'Clear', AmtMaximum: '0', AmtMinimum: '0'
},
beforeSend: function () { Loader(1); },
async: false,
    success: function (response) {
        $('#grddata').empty();
        $('#grddata').append(response);
        localStorage.removeItem("DashboardFilter_SearchVendor");
        localStorage.removeItem("Dashboard_SearchFlg");
        $('#search-box').val("");
        $('#AmtMinimum').val("");
        $('#AmtMaximum').val("");
        $('#txtstartdate').val("");
        $('#txtenddate').val("");
        $('#radio1').val("");
        $('#radio2').val("");

        Loader(0);
    },
error: function() {
    Loader(0);
}
        });
    }

function GetVendorData(obj) {

    var setvendorname = obj.innerHTML; //$(this).text();
    $(".inputvendorname").val(setvendorname);
    var SearchFlg = "V";
    $.ajax({
        url: ROOTURL + '/Invoices/Grid',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: setvendorname, AmtMaximum: '0', AmtMinimum: '0', SearchFlg: SearchFlg
},
beforeSend: function () { Loader(1); },
async: false,
    success: function (response) {
        $('#grddata').empty();
        $('#grddata').append(response);
        document.getElementById('search-box').value = setvendorname;
        localStorage.setItem("DashboardFilter_SearchVendor", setvendorname);
        localStorage.setItem("Dashboard_SearchFlg", SearchFlg);
        $('#search-box').val(setvendorname);
        Loader(0);
    },
error: function() {
    Loader(0);
}
        });
    }
//vendor link close
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

$("#searchicon").change(function () {

    var searchdashbord_val = document.getElementById('search-box').value;
    $.ajax({
        url: 'Invoices/Grid',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: $(this).val(), AmtMaximum: '0', AmtMinimum: '0', SearchFlg: 'S'
},
    beforeSend: function () { Loader(1); },
    async: false,
    success: function (response) {
        $('#grddata').empty();
        $('#grddata').append(response);
        Loader(0);
    },
    error: function () {
    Loader(0);
}
        });
    });

$("#search-box").change(function () {
    searchEvent();
});

function searchEvent() {
    var SearchFlg = 'S';

    var searchdashbord_val = document.getElementById('search-box').value;
    if (searchdashbord_val == "" || searchdashbord_val == undefined) {
        searchdashbord_val = "Clear";
        SearchFlg = '';
    }

    $.ajax({
        url: '/Invoices/Grid',
        data: { IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: searchdashbord_val, AmtMaximum: '0', AmtMinimum: '0', SearchFlg: SearchFlg },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (response) {

            $('#grddata').empty();
            $('#grddata').append(response);
            if (searchdashbord_val == "Clear") {
                searchdashbord_val = "";
            }
            Loader(0);
            $('#search-box').val(searchdashbord_val);
            if (searchdashbord_val == null || searchdashbord_val == '' || searchdashbord_val == "" || searchdashbord_val == undefined) {
                localStorage.removeItem("DashboardFilter_SearchVendor");
                document.getElementById('search-box').value = "";
            }
            else {
                document.getElementById('search-box').value = searchdashbord_val;
                localStorage.setItem("DashboardFilter_SearchVendor", searchdashbord_val);
            }
            localStorage.setItem("Dashboard_SearchFlg", SearchFlg);

        },
        error: function () {
            Loader(0);
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

function bind() {

    $(".myval").select2({
        allowclear: true,
    });
    var pay1 = IsFilter;
    var AmtMinimum = localStorage.getItem("DashboardFilter_AmtMinimum");
    var AmtMaximum = localStorage.getItem("DashboardFilter_AmtMaximum");
    var StartDate = localStorage.getItem("DashboardFilter_StartDate");
    var EndDate = localStorage.getItem("DashboardFilter_EndDate");
    var Payment = localStorage.getItem("DashboardFilter_Payment1");
    var Payment1 = localStorage.getItem("DashboardFilter_Payment2");
    var Department = localStorage.getItem("DashboardFilter_Department");
    var Vendor = localStorage.getItem("DashboardFilter_SearchVendor");
    var SrchTypeFlg = localStorage.getItem("Dashboard_SearchFlg");

    if (Payment == "Cash") {
        $("#radio1").prop('checked', true);
    }
    else if (Payment1 == "Check/ACH") {
        $("#radio2").prop('checked', true);
    }
    else if (IsFilter == "1") {

        $("#radio1").prop('checked', true);
        $("#radio2").prop('checked', true);
    }

    if (Payment == "Cash" && Payment1 == "Check/ACH") {
        $("#radio1").prop('checked', true);
        $("#radio2").prop('checked', true);
    }
    else if (Payment == null && Payment1 == null) {
        $("#radio1").prop('checked', false);
        $("#radio2").prop('checked', false);
    }
    $(".select2-container").remove();

    var dept = Department;
    if (dept != '0' && dept != null && dept != '') {
        document.getElementById('DrpLstdept').value = Department;
    }
    if (StartDate != null && StartDate != '') {
        if (document.getElementById("txtstartdate")) {
            document.getElementById('txtstartdate').value = StartDate;
        }
    }
    if (EndDate != null && EndDate != '') {
        if (document.getElementById('txtenddate')) {
            document.getElementById('txtenddate').value = EndDate;
        }
    }
    if (Payment != null && Payment != '') {
        document.getElementsByName('radio').value = Payment
    }

    if (AmtMinimum != null) {
        if (document.getElementById('AmtMinimum')) {
            document.getElementById('AmtMinimum').value = AmtMinimum;
        }
    }
    if (AmtMaximum != null) {
        if (document.getElementById('AmtMaximum')) {
            document.getElementById('AmtMaximum').value = AmtMaximum;
        }
    }

    if (Vendor != null) {
        $(".inputvendorname").val(Vendor);
    }
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

