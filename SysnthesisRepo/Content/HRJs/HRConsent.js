
function editClick(consentId) {
    
    var ID = consentId.id;
    window.location.href = '/HRConsentMasters/Edit?ConsentId=' + ID;
    //$.ajax({
    //    url: '/HRConsentMasters/EditConsent',
    //    type: "POST",
    //    data: JSON.stringify({ ConsentId: ID }),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (response) {
            
    //    },
    //    error: function (response) {
    //    }
    //});
}
function deleteClick(consentId) {
    
    var ID = consentId.id;
    var text = "Are you sure you want to delete Consent?";
    if (confirm(text) == true) {
        $.ajax({
            url: '/HRConsentMasters/RemoveConsent',
            type: "POST",
            data: JSON.stringify({ ConsentId: ID }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var toastObj = document.getElementById('toast_type').ej2_instances[0];
                if (response == "Success") {
                    var grid = document.getElementById("HRConsent").ej2_instances[0];
                    grid.refresh();
                    toastObj.content = "Consent Removed Successfully.";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                }
                else {
                    toastObj.content = "Something went wrong.";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
            },
            error: function (response) {
            }
        });
    }
}
function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
}
var AddandEdit = 0;
function actionComplete(args) {
    
    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        if (args.requestType === 'beginEdit') {
            AddandEdit = 1;
            args.dialog.header = 'Edit';
        }
        if (args.requestType === 'add') {
            args.dialog.header = 'Add';
        }
    }
    if (args.requestType == 'save') {
        if (AddandEdit == 1) {
            refreshGrid()
        }
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    }

}
function refreshGrid() {
    var grid = document.getElementById("HRConsent").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}
function toolbarClick(args) {
    if (args.item.id === "Add") {
        window.location.href = '/HRConsentMasters/Create';
    }
}

function created(args) {

    // extending the default UrlAdaptor
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    class CustomAdaptor extends ej.data.UrlAdaptor {
        processResponse(data, ds, query, xhr, request, changes) {
            if (!ej.base.isNullOrUndefined(data.success)) {

                toastObj.content = data.success;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.Error)) {

                toastObj.content = data.Error;
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.data))
                return data.data;
            else
                return data;
        }
    }
    var grid = document.querySelector('#HRConsent').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HRConsentMasters/UrlDatasource",
        insertUrl: "/HRConsentMasters/InsertConsent",
        updateUrl: "/HRConsentMasters/UpdateConsent",
        removeUrl: "/HRConsentMasters/RemoveConsent",
        adaptor: new CustomAdaptor()
    });
};




function AddConsentDetail() {
    
    
    var Consents = {};
    var FormType = $('#FormTypeId').val();
    Consents.Language = $('#LanguageId').val();
    Consents.Type = $('#TypeId').val();
    Consents.Detail = $('#txtDescription').val();
    var Errors = ""; // Main Error Messages Variable

    if (FormType == "0") {
        MyCustomToster("Select Consent Type.");
        //showAlert("Select Language.", "info", 5000);
        return false;
    }

    if (Consents.Language == "0") {
        MyCustomToster("Select Language.");
        //showAlert("Select Language.", "info", 5000);
        return false;
    }

    if (Consents.Type == "0") {
        MyCustomToster("Select Type.");
        //showAlert("Select Type.", "info", 5000);
        return false;
    }

    if (Consents.Detail.length == 0) {
        MyCustomToster("Enter Description.");
        //showAlert("Enter Description.", "info", 5000);
        return false;
    }
    $('#lblNodata').html("");
    
    var Row = $("<tr style='color: #787878!important;background: #FFFFFF;font-size: 15px!important;font-weight:bold;'>");
    $('<td>').html($("#LanguageId option:selected").text()).addClass("TitleCol").appendTo(Row);
    $('<td style="display:none;">').html(Consents.Language).addClass("TitleCol").appendTo(Row);
    $('<td>').html($("#TypeId option:selected").text()).appendTo(Row);
    $('<td style="display:none;">').html(Consents.Type).addClass("TitleCol").appendTo(Row);
    $('<td>').html(Consents.Detail).appendTo(Row);
    $('<td>').html("<div class='text-center'><a class='btn-table' href='#' onclick='Delete($(this))'><img alt='' src='/Content/images/delete.png'/></div>").appendTo(Row);
    //Append row to table's body
    ClearForm();
    $('#table-body').append(Row);
}

function ClearForm() {
    $('#DrpLanguage').val("0");
    $('#DrpType').val("0");
    $('#txtDescription').val("");
}

function Delete(row) { // remove row from table
    
    row.closest('tr').remove();
    var rowCount = $('#grid tbody tr').length;
    if (rowCount == 0) {
        $('#lblNodata').html("No Data!");
    }
}


$(document).on("click", "#btnSave", function (event) {
    
    var FormTypeId = $("#FormTypeId").find("option:selected").val();
    if (FormTypeId == 0) {
        MyCustomToster("Select Form Type.");
        //showAlert("Select Form Type.", "info", 5000);
        return false;
    }

    var tblDetail = $('#grid >tbody >tr').length;
    if (tblDetail == 0) {
        MyCustomToster("Please Enter Consent Detail.");
        //showAlert("Please Enter Consent Detail", "info", 5000);
        return false;
    }
    else {
        var objConsent = new Object();
        var objConsentModelDetails = [];
        var table = $("#grid").children('tbody')[0];
        for (var i = 0; i < table.rows.length; i++) {
            var objDetail = new Object();
            objDetail["ConsentId"] = 0;
            objDetail["Language"] = table.rows[i].cells[1].innerText;
            objDetail["Type"] = table.rows[i].cells[3].innerText;
            objDetail["Detail"] = table.rows[i].cells[4].innerText;
            objConsentModelDetails.push(objDetail);
        }

        objConsent["ConsentModelDetails"] = objConsentModelDetails;
        objConsent["FormTypeId"] = FormTypeId;
        objConsent["ConsentId"] = $("#ConsentId").val();
        var data = JSON.stringify({
            "objConsent": objConsent
        });
        $.ajax({
            url: '/HRConsentMasters/Edit',
            type: "POST",
            data: data,
            dataType: "json",
            contentType: "application/json;",
            success: function (Result) {
                
                //var toastObj = document.getElementById('toast_type2').ej2_instances[0];
                if (Result == 'success') {

                    
                    //toastObj.content = "Consent Updated Successfully.";
                    //toastObj.target = document.body;
                    //toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    //showAlert("Consent detail Updated Successfully", "info", 5000);
                    window.location.reload();
                    window.location.href = "/HRConsentMasters/Index";
                    return false;
                }
                else {
                    alert(Result);
                }

            },
            error: function (Result) {
            }
        });
    }
});

$(document).on("click", "#btnSaveCreate", function (event) {
    
    var FormTypeId = $("#FormTypeId").find("option:selected").val();
    if (FormTypeId == 0) {
        MyCustomToster("Select Form Type.");
        return false;
    }

    var tblDetail = $('#grid >tbody >tr').length;
    if (tblDetail == 0) {
        MyCustomToster("Please Enter Consent Detail.");
        return false;
    }
    else {
        var objConsent = new Object();
        var objConsentModelDetails = [];
        var table = $("#grid").children('tbody')[0];
        for (var i = 0; i < table.rows.length; i++) {
            var objDetail = new Object();
            objDetail["ConsentId"] = 0;
            objDetail["Language"] = table.rows[i].cells[1].innerText;
            objDetail["Type"] = table.rows[i].cells[3].innerText;
            objDetail["Detail"] = table.rows[i].cells[4].innerText;
            objConsentModelDetails.push(objDetail);
        }

        objConsent["ConsentModelDetails"] = objConsentModelDetails;
        objConsent["FormTypeId"] = FormTypeId;
        objConsent["ConsentId"] = $("#ConsentId").val();
        var data = JSON.stringify({
            "objConsent": objConsent
        });
        $.ajax({
            url: '/HRConsentMasters/Create',
            type: "POST",
            data: data,
            dataType: "json",
            contentType: "application/json;",
            success: function (Result) {
                //var toastObj = document.getElementById('toast_type1').ej2_instances[0];
                if (Result == 'success') {
                    
                    //toastObj.content = "Consent Added Successfully.";
                    //toastObj.target = document.body;
                    //toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    window.location.reload();
                    window.location.href = "/HRConsentMasters/Index";
                    return false;
                }
                else {
                    alert(Result);
                }

            },
            error: function (Result) {
            }
        });
    }
});

function GetConsentDetail() {
    var ConsentId = $("#ConsentId").val();
    $.getJSON("/HRConsentMasters/GetConsentDetail", { ConsentId: ConsentId }, function (data) {
        if (data != null && data != '') {
            $('#lblNodata').html("");
            $(grid).find('tbody').html('');
            var str = "";
            $.each(data, function (i, state) {
                str = str + "<tr style='color: #787878!important;background: #FFFFFF;font-size: 15px!important;font-weight:bold;'><td>" + state.LanguageType + "</td><td style='display:none;'>" + state.Language + "</td>" +
                    "<td>" + state.Types + "</td><td style='display:none;'>" + state.Type + "</td><td>" + state.Detail + "</td> " +
                    "<td><div class='text-center'><a class='btn-table' href='#' onclick='Delete($(this))'><img alt='' src='/Content/images/delete.png'/></div></td></tr > ";
            });
            $(grid).find('tbody').append(str);
           
        }
    });
}
function showAlert(message, type, closeDelay) {

    if ($("#alerts-container").length == 0) {
        $("body")
            .append($('<div id="alerts-container" style="z-index: 3000; position: fixed; width: 30%; left: 50%; top: 10%; transform: translate(-50%, -50%);">'));
    }

    type = type || "info";

    var alert = $('<div class="alert alert-' + type + ' fade in">')
        .append(
            $('<button type="button" style="margin-left:15px;" class="close" data-dismiss="alert">')
                .append("&times;")
        )
        .append(message);

    $("#alerts-container").prepend(alert);
    if (closeDelay)
        window.setTimeout(function () { alert.alert("close"); }, closeDelay);
}

function setSelect2() {
    $(".select2").select2({ closeOnSelect: true });

    if ($("#Status").val() == 0) {
        $("#terminationDateContainer").hide();
    }
    else if ($("#Status").val() == 1) {
        $("#terminationDateContainer").hide();
    }
    else {
        $("#terminationDateContainer").show();
    }
}
$(function () {
    setSelect2();
});