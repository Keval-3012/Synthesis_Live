function OnQueryCellInfo(args) {

    /* Add required attributes to render Bootstrap tooltip to cells in queryCellInfo
     * In the below args.data[args.column.field] will retrieve the current cell data*/
    if (args.column.headerText == "Update Date") {
        $(args.cell).attr({
            "data-toggle": "tooltip",
            "data-container": "body",
            "title": args.data["FirstName"]
        });
    }

}
function toolbarClick(args) {
    var ven = $("#MailList").val();
    ven = ven.replace('&', '^');
    var Search = $("#Search").val();
    Search = Search.replace('&', '^');
    var CheckFilter = $("#CheckFilterList").val() || "";
    CheckFilter = CheckFilter.replace('&', '^');
    var gridObj = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    if (args.item.id === 'InlineExpenseEditing_pdfexport') {
        gridObj.serverPdfExport("/CheckListExpense/PdfExport?Mail=" + ven + "&startdate=" + $("#StartDate").val() + "&enddate=" + $("#EndDate").val() + "&Searchtext=" + Search + "&CheckFilter=" + CheckFilter);
    }
    if (args.item.id === 'InlineExpenseEditing_excelexport') {
        gridObj.serverPdfExport("/CheckListExpense/ExcelExport?Mail=" + ven + "&startdate=" + $("#StartDate").val() + "&enddate=" + $("#EndDate").val() + "&Searchtext=" + Search + "&CheckFilter=" + CheckFilter);
    }
}
function SearchCheck(args) {
    viewdata();
}
function updatestatus(obj) {

    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    scrollVal = grid.getContent().querySelector('.e-content').scrollTop;  //getting initial scrollbar value
    $.ajax({
        url: ROOTURL + 'CheckListExpense/UpdateStatus',
        type: "POST",
        data: JSON.stringify({ id: obj.id, value: obj.checked }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response == "success") {

                if (obj.checked == true) {
                    toastr.success('Mail Out Successfully.');
                }
                else {
                    toastr.success('UnMail Out Successfully.');
                }
                var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
                grid.refresh();

            }
            else {
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            grid.getContent().querySelector('.e-content').scrollTop = scrollVal; //setting the scrollbar value back
            scrollVal = 0;
        },
        error: function (response) {
        }
    });

}

// Global variable to cache the last parameters
let lastViewParams = null;

function viewdata() {
    var ven = $("#MailList").val() || "";
    ven = ven.replace('&', '^');

    var Search = encodeURIComponent($("#Search").val() || "");
    //var Search = $("#Search").val() || "";
    //Search = Search.replace('&', '^');

    var startDate = $("#StartDate").val() || "";
    var endDate = $("#EndDate").val() || "";

    var CheckFilter = $("#CheckFilterList").val() || "";
    CheckFilter = CheckFilter.replace('&', '^');

    // Create current parameter object
    var currentParams = {
        ven: ven,
        Search: Search,
        startDate: startDate,
        endDate: endDate,
        CheckFilter: CheckFilter
    };

    // If we already made this exact call before, don't call again
    if (lastViewParams &&
        lastViewParams.ven === currentParams.ven &&
        lastViewParams.Search === currentParams.Search &&
        lastViewParams.startDate === currentParams.startDate &&
        lastViewParams.endDate === currentParams.endDate &&
        lastViewParams.CheckFilter === currentParams.CheckFilter
    ) {
        return; // Skip redundant AJAX call
    }

    // Save the current parameters for future comparison
    lastViewParams = currentParams;

    // Proceed with AJAX call
    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/CheckListExpense/ViewInv?Mail=" + ven +
            "&startdate=" + startDate +
            "&enddate=" + endDate +
            "&Searchtext=" + Search +
            "&CheckFilter=" + CheckFilter,
        adaptor: new ej.data.UrlAdaptor()
    });
}
$(document).ready(function () {
    var maxLength = 50;

    $(".show-read-more").each(function () {
        var myStr = $(this).text();
        if ($.trim(myStr).length > maxLength) {
            var newStr = $.trim(myStr).substring(0, maxLength);
            var removedStr = myStr.substring(maxLength, $.trim(myStr).length);
            $(this).empty().html(newStr);
            $(this).append(' <a href="javascript:void(0);" class="read-more">read more...</a>');
            $(this).append('<span class="more-text">' + removedStr + '</span>');
        }
    });
    $(".read-more").click(function () {
        $(this).siblings(".more-text").contents().unwrap();
        $(this).remove();
    });
});

function ActionBegin() {

}
var flag = true;
function dataBound(e) {

    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
}

function ChangeStatus(iii) {
    $.ajax({
        url: ROOTURL + 'ExpenseWeeklySetting/ChangeStatus',
        data: { HomeexpenseID: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {
            if (states == "OK") {
                toastr.success('Home Expenses Detail Status Successfully Changed.');
                window.location.href = ROOTURL + 'ExpenseWeeklySetting/HomeExpenseIndexNew';
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

$(".modl").click(function () {
    $('#IModal').modal('show');
    $(".file-input-name").text('');
    $(".fileut").val('');
    $("#ExpenseId").val(this.value)
});

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
}

function reset() {
    window.location.reload();
}

function OnChangeStartDate(args) {
    var one = formatDate(new Date(args.value.toDateString()));

    var onClickText = "Uncleared Checks StartDate " + one;

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("Date", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
}

function OnChangeEndDate(args) {
    var one = formatDate(new Date(args.value.toDateString()));

    var onClickText = "Uncleared Checks EndDate " + one;

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("Date", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
}

function OnchangeAll(args) {
    var startdate = $("#StartDate").val();
    var enddate = $("#EndDate").val();
    var value = args.checked;
    if (value == true) {
        $("#bodymsg").html("Are you sure you want to Mail out All Checks?");
    }
    else {
        $("#bodymsg").html("Are you sure you want to UnMail out All Checks?");
    }
    $(".divIDClass1").show();
    
}

function confirmAll() {
    var startdate = $("#StartDate").val();
    var enddate = $("#EndDate").val();
    var Checked = $("#SelectAll").prop("checked");
    var search = $("#Search").val();
    search = search.replace('&', '^');
    var ven = $("#MailList").val();
    ven = ven.replace('&', '^');
    var CheckFilter = $("#CheckFilterList").val() || "";
    CheckFilter = CheckFilter.replace('&', '^');
    
    //if (startdate != null && startdate != "" && enddate != null && enddate != "") {
        $.ajax({
            url: ROOTURL + 'CheckListExpense/UpdateAllStatus',
            type: "POST",
            data: JSON.stringify({ StartDate: startdate, EndDate: enddate, value: Checked, Searchtext: search, Mail: ven, CheckFilter: CheckFilter }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response == "success") {

                    if (Checked == true) {
                        toastr.success('Mail Out All Successfully.');
                    }
                    else {
                        toastr.success('UnMail Out All Successfully.');
                    }
                    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
                    grid.refresh();
                    if (Checked == true) {
                        $("#SelectAll").prop("checked", true);
                    }
                }
                else {
                    toastr.error("Something went wrong!");
                }
                grid.getContent().querySelector('.e-content').scrollTop = scrollVal; //setting the scrollbar value back
                scrollVal = 0;
            },
            error: function (response) {
            }
        });
        $(".divIDClass1").hide();
//    }
}
function ClosemodelNew() {
    var Checked = $("#SelectAll").prop("checked");
    if (Checked == true) {
        $("#SelectAll").prop("checked", false);
    }
    else {
        $("#SelectAll").prop("checked", true);
    }
    $(".divIDClass1").hide();
}