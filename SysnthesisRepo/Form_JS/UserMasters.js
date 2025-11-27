$(document).ready(function () {
    $('#frmuser').attr('autocomplete', 'off');
    $("#ConfirmPassword").attr("autocomplete", "nope");
});
$("body").on('click', '.toggle-password', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") === "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$('#Password, #ConfirmPassword').on('keyup', function () {
    if ($('#Password').val() != "" && $('#ConfirmPassword').val() != "") {
        if ($('#Password').val() == $('#ConfirmPassword').val()) {
            $('#message').html('Password Matched').css('color', 'green');
            $('#btnSave').removeAttr("disabled");
        } else {
            $('#message').html('Password Not Matched').css('color', 'red');
            $('#btnSave').attr("disabled", true);
        }
    }
});
var top = 0;
function Loader(val) {
    var doc = document.documentElement;

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
}

function getUserTypeData() {
    var GroupId = $('#GroupId').val();
    if (GroupId == "") {
        GroupId = "0";
    }
    $.ajax({
        url: '/UserMasters/getUserTypeData',
        type: "GET",
        dataType: "JSON",
        data: { GroupId: GroupId },
        success: function (states) {
            $("#UserTypeId").html("");
            $('select#UserTypeId').html('<option value="">--Select--</option>');
            $.each(states, function (data, value) {
                $("#UserTypeId").append(
                    $('<option></option>').val(value.UserTypeId).html(value.UserType));
            });
        }
    });
}


$(document).ready(function () {
    $('#frmuser').attr('autocomplete', 'off');
});

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
}

function cleartextsearch() {
    document.getElementById('txtSearchTitle').value = '';
    FunSearchRecord();
}

var top = 0;
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
$('.burgermenu').click(function () {
    $('#sidebar').toggleClass('showmenu');
    $('.page-content').toggleClass('marleft110');
})
function divexpandcollapse(divname, trname) {

    var div = document.getElementById(divname);
    var img = document.getElementById('img' + divname);
    var tr = document.getElementById(trname);

    if (div.style.display == "none") {
        div.style.display = "inline";
        tr.style.display = "";
        img.src = "../Content/Admin/images/btn_rowminus.png";

    } else {
        div.style.display = "none";
        tr.style.display = "none";
        img.src = "../Content/Admin/images/btn_rowplus.png";
    }
}