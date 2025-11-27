function onFileRemove(args) {
    args.postRawFile = false;
}

function OpenPopupUploadFileModel() {
    $('#productpopupid').modal({
        backdrop: 'static',
        keyboard: true,
        show: true
    });

}

function reloadpage() {

    if ($('#ShowMyInvoiceDetail').is(":checked")) {
        window.location.href = ROOTURL + "BulkUploadFile/Index?ShowMyInvoice=true";
    } else {
        window.location.href = ROOTURL + "BulkUploadFile/Index?ShowMyInvoice=false";
    }
}
$("#ShowMyInvoiceDetail").change(function () {

    if ($('#ShowMyInvoiceDetail').is(":checked")) {
        window.location.href = ROOTURL + "BulkUploadFile/Index?ShowMyInvoice=true";
    } else {
        window.location.href = ROOTURL + "BulkUploadFile/Index?ShowMyInvoice=false";
    }

});
$(".tab1").show();
$(".tab2").hide();
$(".tab3").hide();

function cleartextsearch() {
    $(".inputvendorname").val("");

}
$(".greenBtn").click(function () {
    tab1s();
    tab3s();
    location.reload();
})
$("#search-box1").change(function () {

    tab1s();
});
$("#search-box2").change(function () {

    tab3s();
});

function tab1s() {

    var inputvendorname = $(".inputvendorname1").val();
    $.ajax({
        url: ROOTURL + 'BulkUploadFile/FileuploadList',
        type: "GET",
        data: { "StoreID": $("#storeids").val(), "Search": inputvendorname },
        success: function (response) {

            var selector = $('#getfileuploaddata').html("");
            $("#PeAnalysis").text(response.length);
            if (response.length > 0) {
                $.each(response, function (j, data) {
                    var htm = "<tr>";
                    htm += "<td>N/R</td>";
                    htm += "<td>N/R</td>";
                    htm += "<td>N/R</td>";
                    htm += "<td>" + data.FileName + "</td>";
                    htm += "<td>" + data.StoreName + "</td>";
                    htm += "<td>" + data.CreatedDates + "</td>";
                    htm += "<td><a href='#'  class='btn btn-warning btn-sm'><i class='glyphicon glyphicon-pause'></i> Processing</a></td>";
                    htm += "</tr>";
                    selector.append(htm);
                })
            }
            else {
                var htm = "<tr><td colspan='3' style='text-align: center;'> Data Not Avalible..! </td></tr>";
                selector.append(htm);
            }
        },
        error: function (Result) {
        }
    });
}

function tab3s() {

}

$(".searchbtn").click(function () {

    var getid = "";
    $(".tbs").each(function (index, obj) {
        if ($(obj)[0].className == "tbs active") {
            getid = $(obj)[0].id;
        }
    });
    if (getid == "tab1") {
        tab1s();
    }
    if (getid == "tab2") {
        searchEvent();
    }
    if (getid == "tab3") {
        tab3s();
    }

})

$(window).load(function () {
    tab1s();
    tab3s();
});
$(document).ready(function () {
    tab1s();
    tab3s();

})
$(".tbs").click(function () {
    $(".tbs").removeClass("active");
    $(this).addClass("active")
    $(".tab1").hide();
    $(".tab2").hide();
    $(".tab3").hide();
    $("." + this.id + "").show();
    $("#stab1").addClass('hidden');
    $("#stab2").addClass('hidden');
    $("#stab3").addClass('hidden');
    $("#s" + this.id).removeClass('hidden');
})
$(".fileUploadBtn").click(function () {
    $('.fileUploadBox').addClass('active');
    $('.dragdropdownBox').addClass('active');
    $('body').addClass('overflowHidden');
});
$(".fileUploadBox .closebtn").click(function () {
    $('.fileUploadBox').removeClass('active');
    $('.gridtablebox').removeClass('active');
    $('body').removeClass('overflowHidden');
});

$(".selectFiles").change(function () {

    $('.greenBtn :input').attr("disabled", true);
    $('.dragdropdownBox').removeClass('active');

    if ($("#storeids").val() == "" || $("#storeids").val() == "0") {
        alert("Select Store");
        $('.dragdropdownBox').addClass('active');
        return false;
    }
    else {

    }


    $('.gridtablebox').addClass('active');
    $("#getexcel").html("");
    for (var i = 0; i < this.files.length; i++) {

        var datae = "<tr>";
        datae += "<td id='Filename'><img src='../../../Content/Admin/images/icons/icon_pdf.png' alt='' />" + this.files[i].name + "</td>";
        datae += "<td id='GetFilename' style='display:none;'>" + this.files[i] + "</td>";
        datae += "<td>";
        datae += "<div class='box'>";
        datae += "<div class='loader-05'></div>";
        datae += "</div>";
        datae += "<a href='#' class='trash'><img src='../../../Content/Admin/images/icons/trash.svg' alt='' class='checkicon' /></a>";
        datae += "</td>";
        datae += "</tr>";
        $("#getexcel").append(datae);
    }

    $("#daafile tr").each(function (index, obj) {

        var filename = $(obj).find("#Filename").text();
        var fileData = new FormData();

        for (var i = 0; i < $(".selectFiles")[1].files.length; i++) {
            if ($(".selectFiles")[1].files[i].name == filename)
                fileData.append("DocFiles", $(".selectFiles")[1].files[i]);
        }

        $.ajax({
            url: ROOTURL + 'BulkUploadFile/UploapFilesProcess',
            type: "POST",
            dataType: 'json',
            processData: false,
            contentType: false,
            data: fileData,
            success: function (response) {

                if (response == "True") {
                    var rely = $(obj).context.innerHTML;
                    $(obj).context.innerHTML = rely.replace('<div class="box"><div class="loader-05"></div></div>', '<img src="../../../Content/Admin/images/icons/check_icon.svg" alt="" class="checkicon" />');
                }
                else {
                    console.log(response);
                }
            },
            error: function (response) {

            }
        });

    })
    $('.greenBtn').attr("disabled", false);
});

$(document).on('click', '.trash', function () {

    $(this).closest("tr").remove();
})

$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
})

$('.stop-propagation').on('click', function (e) {
    e.stopPropagation();
});

$(".decimalOnly").bind('keypress', function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
        return false;

    return true;
});

$(".decimalOnly").bind("paste", function (e) {
    e.preventDefault();
});

$(document).ready(function () {

    $('#txtenddate').attr('autocomplete', 'off');
    $("#search-box").attr('autocomplete', 'off');

    $("#search-box").focus();
    Loader(0);
    document.getElementsByName('radio').value = payment;
    var searchval = searchdashbord;
    if (searchval != null) {
        document.getElementById('search-box').value = (searchval).replace(/&amp;/g, '&');
        $("#search-box").val((searchval).replace(/&amp;/g, '&'));
    }

    bind()


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
        url: ROOTURL + 'BulkUploadFile/BulkInvoiceList',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 20, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: '', AmtMaximum: '0', AmtMinimum: '0'
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
}

// vendor link functionality
$('.clsvendorname').on("click", function () {

    var setvendorname = $(this).text();
    $(".inputvendorname").val(setvendorname);
    $.ajax({
        url: ROOTURL + 'BulkUploadFile/BulkInvoiceList',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 20, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: setvendorname, AmtMaximum: '0', AmtMinimum: '0'
        },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            document.getElementById('search-box').value = setvendorname;
            $('#search-box').val(setvendorname);
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
});
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
        url: ROOTURL + 'BulkUploadFile/BulkInvoiceList',
        data: {
            IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 20, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: $(this).val(), AmtMaximum: '0', AmtMinimum: '0'
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

    var searchdashbord_val = document.getElementById('search-box').value;

    $.ajax({
        url: ROOTURL + 'BulkUploadFile/BulkInvoiceList',
        data: { IsBindData: 1, currentPageIndex: 1, orderby: OrderByVal, IsAsc: IsAscVal, PageSize: 20, SearchRecords: SearchRecords, Alpha: Alpha, deptname: '', startdate: '', enddate: '', payment: '', Store_val: '', searchdashbord: searchdashbord_val, AmtMaximum: '0', AmtMinimum: '0' },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (response) {

            $("#InvoicesRevi").text(response.length);
            $('#grddata').empty();
            $('#grddata').append(response);
            $('#search-box').val(searchdashbord_val)

            Loader(0);
            $('#search-box').val(searchdashbord_val)


        },
        error: function () {
            Loader(0);
        }
    });
}

function FunSearchRecord()//Search
{

    window.FunSearchRecord = FunSearchRecord

    var element_chkpayment = chk;
    var storeid = "0";
    if (Session["StoreId"] != null) {
        storeid = Session["StoreId"].ToString();
    }
}

function FunPageIndex(pageindex)//grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum);
}

function FunSortData(SortData)//Grid header sorting
{
    GetData(0, CurrentPageIndex, SortData, AscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum);
    myCustomFn();
}

function FunPageRecord(PageRecord)//Grid Page per record
{
    GetData(0, 1, OrderByVal, IsAscVal, PageRecord, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum);
}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();

}

function Delete(ID) {
    $.ajax({
        url: '/BulkUploadFile/Delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            if($('#ShowMyInvoiceDetail').is(":checked")) {
                GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum, true);
            }
            else
            {
                GetData(1, CurrentPageIndex, OrderByVal, IsAscVal, PageSize, Alpha, SearchRecords, deptname, startdate, enddate, payment, Store_val, AmtMaximum, AmtMinimum, false);
            }

            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, dept_val, startdate_val, enddate_val, payment_val, Store_val, Maximum_val, Minimum_val, showMyInvoice) {
    var Data = searchdashbord;
    $.ajax({
        url: ROOTURL + 'BulkUploadFile/BulkInvoiceList',

        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: 20, SearchRecords: SearchRecords_val, Alpha: alpha_val, deptname: dept_val, startdate: startdate_val, enddate: enddate_val, payment: payment_val, Store_val: Store_val, searchdashbord: Data, AmtMaximum: Maximum_val, AmtMinimum: Minimum_val, ShowMyInvoice: showMyInvoice },
        beforeSend: function () { Loader(1); },
        success: function (response) {

            if (response != null) {
                $('#grddata').empty();
                $('#grddata').append(response);

            }
            Loader(0);
            if (payment_val == "Cash") {
                $("#radio1").prop('checked', true);
            }
            else if (payment_val == "Check/ACH") {
                $("#radio2").prop('checked', true);
            }
        },
        error: function () {
            Loader(0);
        }
    });
}

function bind() {

    $(".myval").select2({
        allowclear: true,
    });

    document.getElementsByName('radio').value = payment;

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