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
