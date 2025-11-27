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

function AddDocument() {
    var ExpenseId = $('#ExpenseId').val();
    var files = $("#fileinput").get(0).files;
    var ExpenseId = $("#ExpenseId").val();

    var formData = new FormData();
    formData.append('fileinput', files[0]);
    formData.append('ExpenseId', ExpenseId);
    $.ajax({
        url: '/ExpenseAccounts/SaveExpenseDocument',
        type: "post",
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function () { Loader(1); },
        success: function (states) {
            if (states == "OK") {
                $("#fileinput").text("");
                $('#IModal').modal('hide');
                toastr.success(DocSaved);

                var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
                grid.dataSource = new ej.data.DataManager({
                    url: "/ExpenseAccounts/UrlDatasourceExpense",
                    adaptor: new ej.data.UrlAdaptor()
                });

            }
            Loader(0);
        }
    });
}

function ExcludeExpense(iii) {

    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ExcludeExpense',
        data: { ExpenseId: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {
            if (states == "OK") {
                toastr.success(ExcExpense);
                window.location.href = ROOTURL +'ExpenseAccounts/ExpenseCheckIndexNew';
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

function IncludeExpense(iii) {

    $.ajax({
        url: ROOTURL +'ExpenseAccounts/IncludeExpense',
        data: { selectedval: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {
            if (states == "OK") {
                toastr.success(ExcExpense);
                window.location.href = ROOTURL + 'ExpenseAccounts/ExpenseCheckIndexNew';
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}


function ForceIncludeExpense(iii) {

    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ForceIncludeExpense',
        data: { selectedval: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {

            if (states == "OK") {
                
                toastr.success(ExcExpense);
                window.location.href = ROOTURL + 'ExpenseAccounts/ExpenseCheckIndexNew';
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

function ForceExcludeExpense(iii) {

    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ForceExcludeExpense',
        data: { selectedval: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {

            if (states == "OK") {
                
                toastr.success(ExcExpense);
                window.location.href = ROOTURL + 'ExpenseAccounts/ExpenseCheckIndexNew';
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

function AddDocPopup(iii) {

    //$("#IModal").show();
    $('#IModal').modal('show');
    //$(".fileut").text('');
    $(".file-input-name").text('');
    $(".fileut").val('');
    $("#ExpenseId").val(iii)
};
$(".modl").click(function () {

    //$("#IModal").show();
    $('#IModal').modal('show');
    //$(".fileut").text('');
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

$(".UploadFile").click(function () {
    var id = this.value;
    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ExcludedExpenseListDocument',
        data: { ExpenseCheckID: id },
        async: false,
        success: function (response) {

            if (response.list.length > 0) {
                var htmls = "";
                var htmlModel = "";
                $.each(response.list, function (index, value) {

                    htmls += "<tr>";
                    htmls += "<td> <a href='/UserFiles/ExpenseDocuments/" + value.DocumentName + "' style='padding-left: 14px;' target = '_blank' >" + value.DocumentName + "</a></td>";
                    if (response.type == 1 || response.id == value.CreatedBy) {
                        htmls += "<td> <a href='#' onclick='return ErrorPopup(" + value.ExpenseCheckDocumentId + ");' data-toggle='tooltip' data-placement='top' data-original-title='Delete'><img src='../Content/Admin/images/trash-2.svg' alt=''/> </a></td>";
                    }
                    else {
                        htmls += "<td></td>"
                    }
                    htmls += "</tr>";
                    if (response.type == 1 || response.id == value.CreatedBy) {
                        htmlModel += "<div id='" + value.ExpenseCheckDocumentId + "' class='divIDClass modal-popup modal-danger modal-message' style='position: fixed;  left:45%; width:300px; top: 10px; display:none'>";
                        htmlModel += "<div class='modal-content '>";
                        htmlModel += "<div class='modal-header text-center'>";
                        htmlModel += "<i class='glyphicon glyphicon-trash'></i>";
                        htmlModel += "</div>";
                        htmlModel += "<div class='modal-title'>Message</div>";
                        htmlModel += "<div class='modal-body '>Are you sure you want to remove this Document?</div>";
                        htmlModel += "<div class='modal-footer' style='text-align:center'>";
                        htmlModel += "<a class='btn btn-danger' onclick='Delete(" + value.ExpenseCheckDocumentId + ")'>Yes </a>";
                        htmlModel += "<a class='btn' data-dismiss='modal' onclick='closemodal()'>No</a>";
                        htmlModel += "</div>";
                        htmlModel += "</div>";
                        htmlModel += "</div>";
                        $("#ModelPopup").html("");
                        $("#ModelPopup").append(htmlModel);
                    }
                });
                $("#UploadDocumentFile").html("");
                $("#UploadDocumentFile").append(htmls);
            }
            else {
                $("#UploadDocumentFile").html("");
            }
            $('#ViewFile').modal('show');
        },
        error: function () {
            Loader(0);
        }
    });

});
function UploadFile(iii) {
    var id = iii;
    $.ajax({
        url: ROOTURL + 'ExpenseAccounts/ExcludedExpenseListDocument',
        data: { ExpenseCheckID: id },
        async: false,
        success: function (response) {

            if (response.list.length > 0) {
                var htmls = "";
                var htmlModel = "";
                $.each(response.list, function (index, value) {

                    htmls += "<tr>";
                    htmls += "<td> <a href='/UserFiles/ExpenseDocuments/" + value.DocumentName + "' style='padding-left: 14px;' target = '_blank' >" + value.DocumentName + "</a></td>";
                    if (response.type == 1 || response.id == value.CreatedBy) {
                        htmls += "<td> <a href='#' onclick='return ErrorPopup(" + value.ExpenseCheckDocumentId + ");' data-toggle='tooltip' data-placement='top' data-original-title='Delete'><img src='../Content/Admin/images/trash-2.svg' alt=''/> </a></td>";
                    }
                    else {
                        htmls += "<td></td>"
                    }
                    htmls += "</tr>";
                    if (response.type == 1 || response.id == value.CreatedBy) {
                        htmlModel += "<div id='" + value.ExpenseCheckDocumentId + "' class='divIDClass modal-popup modal-danger modal-message' style='position: fixed;  left:45%; width:300px; top: 10px; display:none'>";
                        htmlModel += "<div class='modal-content '>";
                        htmlModel += "<div class='modal-header text-center'>";
                        htmlModel += "<i class='glyphicon glyphicon-trash'></i>";
                        htmlModel += "</div>";
                        htmlModel += "<div class='modal-title'>Message</div>";
                        htmlModel += "<div class='modal-body '>Are you sure you want to remove this Document?</div>";
                        htmlModel += "<div class='modal-footer' style='text-align:center'>";
                        htmlModel += "<a class='btn btn-danger' onclick='Delete(" + value.ExpenseCheckDocumentId + ")'>Yes </a>";
                        htmlModel += "<a class='btn' data-dismiss='modal' onclick='closemodal()'>No</a>";
                        htmlModel += "</div>";
                        htmlModel += "</div>";
                        htmlModel += "</div>";
                        $("#ModelPopup").html("");
                        $("#ModelPopup").append(htmlModel);
                    }
                });
                $("#UploadDocumentFile").html("");
                $("#UploadDocumentFile").append(htmls);
            }
            else {
                $("#UploadDocumentFile").html("");
            }
            $('#ViewFile').modal('show');
        },
        error: function () {
            Loader(0);
        }
    });
}

function ErrorPopup(ID) {

    var DivId = ID;
    $("#" + DivId).show();
}
function Delete(ID) {

    $('.modal-backdrop').hide();
    $.ajax({
        url: '/ExpenseAccounts/delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            window.location.reload();
            return true;
        },
        error: function () {
            return false;
        }
    });
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

$('#rbExpense').click(function () {
    $('#rbCheck').prop('checked', false);
    $('#rbExpense').prop('checked', true);
});

$('#rbCheck').click(function () {
    $('#rbCheck').prop('checked', true);
    $('#rbExpense').prop('checked', false);
});

$('#ddlStatus').on('change', function () {


    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/ExpenseAccounts/UrlDatasourceExpense?Status=" + $("#ddlStatus").val(),
        adaptor: new ej.data.UrlAdaptor()
    });

});