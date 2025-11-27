$(document).ready(function () {
    var maxLength = 50;
    var StoreID = storeid;
    if (StoreID == 0) {
        $('#popupStoreAlertValue').show();
    }
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
    //  checks whether the cancel icon is already present or not
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

function ChangeStatus(iii) {
    $.ajax({
        url: ROOTURL + 'ExpenseWeeklySetting/ChangeStatus',
        data: { HomeexpenseID: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {
            if (states == "OK") {
                toastr.success('Home Expenses Detail Status Successfully Changed.');
                var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
                grid.refresh();
                /*window.location.href = ROOTURL + 'ExpenseWeeklySetting/HomeExpenseIndexNew';*/
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

$(document).ready(function () {
    $("#search-box").attr('autocomplete', 'off');
    $("#search-box").focus();
    Loader(0);
});

function cleartextsearch() {
    var searchdashbord_val = '';
    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ExpenseCheckIndexGrid',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', Store_val: '', searchdashbord: 'Clear', SearchFlg: ''
        },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            localStorage.removeItem("ECheckFilter_SearchVendor");
            localStorage.removeItem("ECheckFilter_SearchFlg");
            $('#search-box').val("");
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
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }
}

$("#searchicon").change(function () {
    var searchdashbord_val = document.getElementById('search-box').value;
    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ExpenseCheckIndexGrid',
        data: { IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', Store_val: '', searchdashbord: $(this).val(), SearchFlg: SearchFlg },
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
        url: ROOTURL + '/ExpenseAccounts/ExpenseCheckIndexGrid',
        data: { IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 50, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', Store_val: '', searchdashbord: searchdashbord_val, SearchFlg: SearchFlg },
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
                localStorage.removeItem("ECheckFilter_SearchVendor");
                document.getElementById('search-box').value = "";
            }
            else {
                document.getElementById('search-box').value = searchdashbord_val;
                localStorage.setItem("ECheckFilter_SearchVendor", searchdashbord_val);
            }
            localStorage.setItem("ECheckFilter_SearchFlg", SearchFlg);

        },
        error: function () {
            Loader(0);
        }
    });
}

$(function () {
    var $win = $(window);

    $win.scroll(function () {
        if ($win.height() + $win.scrollTop()
            == $(document).height()) {
            myCustomFn();
        }
    });
});

function getDocHeight() {	// $(document).height() value depends on browser
    var D = document;
    return Math.max(
        D.body.scrollHeight, D.documentElement.scrollHeight,
        D.body.offsetHeight, D.documentElement.offsetHeight,
        D.body.clientHeight, D.documentElement.clientHeight
    );
}
function myCustomFn() {
    var page_loadsize = parseInt(PageSize) + parseInt(50);
    document.getElementById('hdnpagevalue').value = page_loadsize;
    $("#hdnpagevalue").val(page_loadsize);
}

$(window).scroll(function () {
    var scroll = $(window).scrollTop();
    if (scroll >= 50) {
        $(".page-header-main").addClass("stikyheader");
    } else {
        $(".page-header-main").removeClass("stikyheader");
    }
});

$(document).ready(function () {
    (function ($) {
        $.fn.fixMe = function () {
            return this.each(function () {
                var $this = $(this),
                    $t_fixed;
                function init() {
                    $this.wrap('<div class="" />');
                    $t_fixed = $this.clone();
                    $t_fixed.find("tbody").remove().end().addClass("fixed").insertBefore($this);
                    resizeFixed();
                }
                function resizeFixed() {
                    $t_fixed.find("th").each(function (index) {
                        $(this).css("width", $this.find("th").eq(index).outerWidth() + "px");
                    });
                }
                function scrollFixed() {
                    var offset = $(this).scrollTop(),
                        tableOffsetTop = $this.offset().top,
                        tableOffsetBottom = tableOffsetTop + $this.height() - $this.find("thead").height();
                    if (offset < tableOffsetTop || offset > tableOffsetBottom)
                        $t_fixed.hide();
                    else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && $t_fixed.is(":hidden"))
                        $t_fixed.show();
                }
                $(window).resize(resizeFixed);
                $(window).scroll(scrollFixed);
                init();
            });
        };
    })(jQuery);

    $(document).ready(function () {
        $("table").fixMe();

    });
});



function ErrorPopup(ID) {

    var DivId = ID;
    $("#" + DivId).show();
}

function closemodal() {
    $(".divIDClass").hide();
}


$(window).on('scroll', function () {
    var scrollTop = $(window).scrollTop();
    if (scrollTop > 50) {
        $('#dvBody').stop().animate({ height: "30px" }, 200);
    }
    else {
        $('#dvBody').stop().animate({ height: "50px" }, 200);
    }
});

function reset() {
    window.location.reload();
}

function viewdata() {

    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/ExpenseWeeklySetting/ViewWithFilter?startdate=" + $("#StartDate").val() + "&enddate=" + $("#EndDate").val(),
        adaptor: new ej.data.UrlAdaptor()
    });
}

function toolbarClick(args) {
    var gridObj = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    if (args.item.id === 'InlineExpenseEditing_pdfexport') {
        gridObj.serverPdfExport("/ExpenseWeeklySetting/PdfExport?startdate=" + $("#StartDate").val() + "&enddate=" + $("#EndDate").val());
    }
    if (args.item.id === 'InlineExpenseEditing_excelexport') {
        gridObj.serverPdfExport("/ExpenseWeeklySetting/ExcelExport?startdate=" + $("#StartDate").val() + "&enddate=" + $("#EndDate").val());
    }
}

function OnChangeStartDate(args) {
    var one = formatDate(new Date(args.value.toDateString()));

    var onClickText = "Homesome Expenses Detail StartDate " + one;

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

    var onClickText = "Homesome Expenses Detail EndDate " + one;

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