function Edit1(ID) {
    var Divnamelbl = "#" + "1Divnamelbl" + ID;
    var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;

    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    //$(Divnamelbl).hide();
    $(DivQbAccountlbl).hide();
    $(DivQbAccountddl).show();

    $(editid).hide();
    $(update).show();
    $(cancel).show();
}

function Updatedata1(ID) {
    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    var QBval = document.getElementById("1EQBAccount_" + ID).value;
    var Divnamelbl = "#" + "Divnamelbl" + ID;
    if (QBval == "") {
        $("#1EQBAccountReq" + ID).show();
        return false;
    }
    else {
        $("#1EQBAccountReq" + ID).hide();
    }
    $(Divnamelbl).show();
    $.ajax({
        url: ROOTURL + 'GroupAccountMasters/UpdateOtherDeposite_Setting',
        data: { ID: ID, QBAccountid: QBval },
        type: 'POST',
        cache: false,
        dataType: 'json',
        beforeSend: function () { Loader(1); },
        success: function (response) {
            if (response === "Edit") {
                GetData1();
                MyCustomToster(SettingUpdate);
                Loader(0);

                var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
                var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;
                $(DivQbAccountlbl).show();
                $(DivQbAccountddl).hide();
                $(editid).show();
                $(update).hide();
                $(cancel).hide();
            }
        }
    });
}

function Cancel1(ID) {
    var Divnamelbl = "#" + "1Divnamelbl" + ID;
    var DivQbAccountlbl = "#" + "1DivQbAccountlbl" + ID;
    var DivQbAccountddl = "#" + "1DivQbAccountddl" + ID;

    var editid = "#" + "edit1_" + ID;
    var update = "#" + "update1_" + ID;
    var cancel = "#" + "cancel1_" + ID;

    $(cancel).hide();
    $(Divnamelbl).show();
    $(DivQbAccountlbl).show();
    $(DivQbAccountddl).hide();
    $(editid).show();
    $(update).hide();
}

function GetData1() {
    $.ajax({
        type: "GET",
        url: ROOTURL + 'GroupAccountMasters/OtherDepositeAccount',
        success: function (response) {
            $('#grddata1').empty();
            $('#grddata1').append(response);
        }
    });
}