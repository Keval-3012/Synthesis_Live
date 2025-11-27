//$('[data-toggle="tooltip"]').tooltip();

var startdate = SearchTitle;
var enddate = SearchTitle1;
$('#txtstartdate').val(enddate);
$('#txtenddate').val(startdate);

$(".fileUploadBtn").click(function () {
    $('.fileUploadBox').addClass('active');
    $('.fileUploadBox').css("display", "block");
    $('.dragdropdownBox').css("display", "block");
    $('.dragdropdownBox').addClass('active');
    $('body').addClass('overflowHidden');
    $
});

$(".fileUploadBox .closebtn").click(function () {
    $('.fileUploadBox').removeClass('active');
    $('.gridtablebox').removeClass('active');
    $('.fileUploadBox').css("display", "none");
    $('.dragdropdownBox').css("display", "none");
    $('body').removeClass('overflowHidden');
});
$(".selectFiles").change(function () {
    $('.greenBtn :input').attr("disabled", true);
    $('.dragdropdownBox').removeClass('active');

    if ($("#storeid").val() == "" || $("#storeid").val() == "0") {
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

        var Sttoreid = 0;
        if ($("#storeid").val() == undefined) {
            Sttoreid = storeid;
        }
        else {
            Sttoreid = $("#storeid").val();
        }
        if (Sttoreid == 0) {
            return false;
        }
        fileData.append('Storeid', Sttoreid);
        $.ajax({
            url: ROOTURL + 'Report/UploadFiles',
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

$(document).ready(function () {
    var currentDate = new Date();
    var formattedDate = moment(currentDate).format('MMM DD, YYYY');
    $('#txtstartdate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false,
        maxDate: currentDate
    });
    $('#txtenddate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false,
        maxDate: currentDate
    });
    $('#txtstartdate').val(formattedDate);
    $('#txtenddate').val(formattedDate);

});