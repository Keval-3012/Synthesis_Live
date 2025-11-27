function closemodal() {
    $(".divIDClass").hide();
}
function BindGroupData(val_id, itemid) {

    $.getJSON(ROOTURL + 'Configuration/BindGroupData/?groupid=' + val_id, function (data) {

        $("#" + itemid + "typicalbal").html(data.typicalbalance);
        $("#" + itemid + "typicalbal").parent("td").find("#typicalbalid").val(data.typicalBalId);
        $("#" + itemid + "Deptid").html(data.Deptname);
        $("#" + itemid + "Deptid").parent("td").find("#Deptid").val(data.DeptId);
        $("#" + itemid + "memoid").html(data.memo);
        $("#" + itemid + "memoid").parent("td").find("#memoidval").val(data.memo);

        $("#" + itemid + "typicalbal").parent("td").find("#ggID").val(val_id);
    });
}

var resetTenderName = "";

var resetTenderName = "";
var resetStoreId = 0;

function openResetPopup(tenderName, storeId) {
    resetTenderName = tenderName;
    resetStoreId = storeId;
    $("#resetConfirmPopup").show();
}

function closeResetPopup() {
    resetTenderName = "";
    resetStoreId = 0;
    $("#resetConfirmPopup").hide();
}

function confirmReset() {
    $.ajax({
        url: '/Configuration/ResetConfiguration',
        type: 'POST',
        data: {
            tenderName: resetTenderName,
            storeId: resetStoreId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                MyCustomAlert("Configuration Reset Successfully", 1);
                location.reload();
            } else {
                MyCustomAlert("Error while resetting configuration", 0);
            }
        },
        error: function () {
            MyCustomAlert("Something went wrong!", 0);
        }
    });

    closeResetPopup();
}
