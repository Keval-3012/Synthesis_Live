function closemodal() {
    $(".divIDClass").hide();
    window.location.reload();
}

$("#SelChkbox").click(function () {
    if ($(this).prop('checked') == true) {
        $(".trChkbox").prop('checked', true);
    }
    else {
        $(".trChkbox").prop('checked', false);
    }
})

$("#btnForceSync").click(function () {
    var strInvID = "";
    $(".trChkbox").each(function (index, obj) {
        if ($(obj).prop('checked') == true) {
            var JobID = $(obj).parents("tr").find("td")[1].innerHTML;
            console.log(JobID);
            strInvID += JobID + ",";
        }
    });
    if (strInvID != "") {
        var data = JSON.stringify({
            "InvoiceID": strInvID
        });
    }
    $.ajax({
        url: ROOTURL + "/QBConfiguration/UpdateUnsuccessfullDetail",
        type: "POST",
        data: data,
        dataType: "json",
        contentType: "application/json;",
        success: function (Result) {
            if (Result == 'success') {
                $("#progress").css("display", "none");
                $(".modal-title").html("");
                $(".modal-title").append("Invoice Status Update Successfully.");
                $("#popupid").modal("show");
                return false;
            }
            else {
                $("#progress").css("display", "none");
                alert(Result);
            }
        },
        error: function (Result) {
        }
    })
})

$("#btnIgnore").click(function () {
    var strInvID = "";
    $(".trChkbox").each(function (index, obj) {
        if ($(obj).prop('checked') == true) {
            var JobID = $(obj).parents("tr").find("td")[1].innerHTML;
            console.log(JobID);
            strInvID += JobID + ",";
        }
    });
    if (strInvID != "") {
        var data = JSON.stringify({
            "InvoiceID": strInvID
        });
    }
    $.ajax({
        url: ROOTURL + "/QBConfiguration/IgnoreUnsuccessfullDetail",
        type: "POST",
        data: data,
        dataType: "json",
        contentType: "application/json;",
        success: function (Result) {
            if (Result == 'success') {
                $("#progress").css("display", "none");
                $(".modal-title").html("");
                $(".modal-title").append("Invoice Updated Successfully.");
                $("#popupid").modal("show");
                return false;
            }
            else {
                $("#progress").css("display", "none");
                alert(Result);
            }
        },
        error: function (Result) {
        }
    })
})

$(document).ready(function () {
    $(".boxx").each(function (index, obj) {
        if ($(obj).find(".online").text() == "Online Connected") {
            $(obj).find("#box").removeClass("grey_box");
        }
        if ($(obj).find(".desktop").text() == "Offline Connected") {
            $(obj).find("#box").removeClass("grey_box");
        }
    })
})

function check(id, name) {
    window.location.href = ROOTURL + "/QBConfiguration/QBDesktopConfiguration/?StoreID=" + id + '&StoreName=' + name;
}

function InsertQuickbook(StoreID) {
    window.location.href = ROOTURL + '/QBConfiguration/GotoAuthGrant/?StoreID=' + StoreID;
}

function getVendorDepartment(StoreID) {
    $.ajax({
        url: ROOTURL + '/QBConfiguration/getVendorDepartment',
        type: "POST",
        data: { "StoreID": StoreID },
        success: function (data) {
            if (data.Result == 'success') {
            }
            else {

            }
        },
        error: function (Result) {
        }
    });
}

function updateBank() {
    var StoreID = $("#StoreId").val();
    var QbWebId = $("#QBDesktopId").val();
    var BankListID = $("#BankListID").val();
    if (BankListID != "") {

        window.location.href = ROOTURL + "/QBConfiguration/UpdateBank/?StoreID=" + StoreID + '&QbWebId=' + QbWebId + '&ListID=' + BankListID;
    }
    else {
        return false;
    }
}

$(document).on("click", "#btnSave", function (event) {
    var iStoreId = document.getElementById("StoreId").value;
    if (iStoreId == "") {
        alert("Select Store");
        return false;
    }
    if ($("#QBDesktopId").val() == "") {
        document.getElementById("QBDesktopId").value = 1;
    }

    var str = document.getElementById("QBCompanyPath").value;
    var patt = /.qbw/g;
    var result = patt.test(str);
    if (result != true) {
        alert("Please enter valid Quickbook file path.")
        return false;
    }

    var model = new FormData();
    model.append("QBCompanyPath", document.getElementById("QBCompanyPath").value);
    model.append("UserName", document.getElementById("UserName").value);
    model.append("Password", document.getElementById("Password").value);
    model.append("AppName", document.getElementById("AppName").value);
    model.append("QBDesktopId", document.getElementById("QBDesktopId").value);
    model.append("Description", document.getElementById("Description").value);
    model.append("IsActive", true);
    model.append("StoreId", iStoreId);
    $("#progress").css("display", "block");
    $.ajax({
        url: ROOTUROL + "/QBConfiguration/QBDesktopConfiguration",
        type: "POST",
        data: model,
        contentType: false,
        processData: false,
        success: function (Result) {
            if (Result == 'success') {
                $("#progress").css("display", "none");
                alert("Record Save successfully");
                window.location.reload();
                return false;
            }
            else {
                $("#progress").css("display", "none");
                if (Result == "User Expired") {
                    window.location.href = ROOTUROL + "/Login/Index";
                }
                else {
                    alert(Result);
                }
            }
        },
        error: function (Result) {
        }
    });
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

$("#StoreId").change(function () {
    var StoreId = $('#StoreId').val();

    $.ajax({
        url: ROOTURL + "/QBConfiguration/getSyncInvoiceData",
        type: "post",
        cache: false,
        data: { StoreId: StoreId },
        beforeSend: function () {
            Loader(1);
            $("#preloader").show();
            $("#status").show();
        },
        success: function (states) {
            $("#adta").html(states);
            $("#preloader").hide();
            $("#status").hide();
            Loader(0);

        },
        error: function () {
            Loader(0);
        }
    });
});

function getDateFromAspNetFormat(date) {
    const re = /-?\d+/;
    const m = re.exec(date);
    return parseInt(m[0], 10);
}

$(document).on("click", "#main", function () {
    $("#viewerro").html("");
    var ControllerName = $(this).closest("tr").find(".ControllerName").text();
    var FunctionName = $(this).closest("tr").find(".FunctionName").text();
    var ErrorMesg = $(this).closest("tr").find(".ErrorMesg").text();
    var erro = "<li>ControllerName : " + ControllerName + "</li>";
    erro += "<li>FunctionName : " + FunctionName + "</li>";
    erro += "<li>ErrorMesg : " + ErrorMesg + "</li>";
    $("#viewerro").append(erro);
    $('#myModal').modal('show');
});

function QBUnsuccessfull(id) {
    window.location.href = ROOTURL + "/QBConfiguration/GetUnsuccessfullInvoice/?StoreID=" + id;
}

function QBSca(id, flag) {
    window.location.href = ROOTURL + "/QBConfiguration/SyncData/?StoreID=" + id + '&Flag=' + flag;
}

function QBSynca(id, flag, Type) {
    $("#div_" + id).show();
    $("#flg_" + id).val(flag);
    $("#Type_" + id).val(Type);
}

$(document).ready(function () {
    $(".boxx").each(function (index, obj) {
        if ($(obj).find(".online").text() == "Online QB") {
            $(obj).find("#box").addClass("grey_box");
        }
        if ($(obj).find(".desktop").text() == "Desktop") {
            $(obj).find("#box").addClass("grey_box");
        }
    })
})

$(document).ready(function () {
    $.ajax({
        url: ROOTURL + "/QBConfiguration/CheckIsSync",
        type: "POST",
        success: function (data) {
            if (data == 'success') {
                $("#rd").show();
                $("#gn").hide();
            }
            else if (data == 'NOTsuccess') {
                $("#rd").hide();
                $("#gn").show();
            }
        },
        error: function (Result) {
        }
    });

    $(".red_cor").click(function () {
        var classNa = this.className;
        $.ajax({
            url: ROOTURL + "/QBConfiguration/QBSyncUpdateData",
            type: "POST",
            data: { "Class": classNa },
            success: function (data) {
                if (data == 'success') {
                    $("#rd").show();
                    $("#gn").hide();
                }
                else if (data == 'Notsuccess') {
                    $("#rd").hide();
                    $("#gn").show();
                }
                window.location.href = ROOTURL + "/QBConfiguration/QBSyncOnlineData";
            },
            error: function (result) {
            }
        });
    });
});

