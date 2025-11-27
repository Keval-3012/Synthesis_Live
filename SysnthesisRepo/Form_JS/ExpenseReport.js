function closemodal() {
    $(".divIDClass").hide();
}

function Setvalues() {
    localStorage.setItem("FromInvoicePage", "ExpenseReport");

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
}

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
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
        //$('#Validation').show();
        // $('#AmtMaximum').val('');
        //$("#btnSearch").attr("style", "display:none;")
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


function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

//For Search Button
function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, dept_val, startdate_val, enddate_val, payment_val, Store_val, Status_val, VendorName_Val, AmtMaximum_val, AmtMinimum_val) {

    $.ajax({
        url: ROOTURL + '/ExpenseReport/grid',
        cache: false,
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, deptname: dept_val, startdate: startdate_val, enddate: enddate_val, payment: payment_val, Store_val: Store_val, Status: Status_val, VendorName: VendorName_Val, AmtMaximum: AmtMaximum_val, AmtMinimum: AmtMinimum_val },
        beforeSend: function () { Loader(1); },
        // async: false,
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);


            if (payment_val == "Expense") {
                $("#radio1").prop('checked', true);
            }
            else if (payment_val == "Check") {
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



function closemodal() {
    $(".divIDClass").hide();
}