function createdHRPayRate(args) {
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
    EmployeeChildId = $('#EmployeeChildId').val();
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
    var grid = document.querySelector('#HRPayRate').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployeeProfile/HRPayRateUrlDatasource?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        insertUrl: "/HREmployeeProfile/HRPayRateInsert?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        updateUrl: "/HREmployeeProfile/HRPayRateUpdate",
        removeUrl: "/HREmployeeProfile/HRPayRateRemove",
        adaptor: new CustomAdaptor()
    });
};
function actionBeginHRPayRate(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    if (args.requestType == "save") {
        ej.popups.showSpinner(args.dialog.element);
    }
}
var AddandEdit = 0;

//function actionCompleteHRPayRate(args) {
//    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
//        if (args.requestType === 'beginEdit') {
//            AddandEdit = 1;
//            args.dialog.header = 'Edit';
//        }
//        if (args.requestType === 'add') {
//            args.dialog.header = 'Add';
//        }
//    }
//    if (args.requestType == 'save') {
//        if (AddandEdit == 1) {
//            refreshGrid()
//        }
//        $(window).scrollTop(scrollVal);
//        scrollVal = 0;
//    }

//}

function actionCompleteHRPayRate(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            AddandEdit = 1;
            args.dialog.header = 'Edit Pay Rate';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HREmployeeProfile/Editpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                appendElement(data, args.form); //render the edit form with selected record
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {

            args.dialog.header = 'Add Pay Rate';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HREmployeeProfile/Addpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                //args.form.elements.namedItem('VendorName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
    }

        if (args.requestType == 'save') {
            if (AddandEdit == 1) {
                refreshGrid()
            }
            $(window).scrollTop(scrollVal);
            scrollVal = 0;
        }

    //if (args.requestType == 'save') {
    //    //var grid6 = document.getElementById("InlineEditing").ej2_instances[0];
    //    $(window).scrollTop(scrollVal);
    //    scrollVal = 0;
    //    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    //    grid.refresh();
    //}

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

        //alert("If you want to any change in this vendor then you must have to active first, otherwise the change will not take place in QuickBooks.");
    }

}


function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('PayRateDate', { required: [true, "Date is required"] });
    form.ej2_instances[0].addRules('PayRate', { required: [true, "Pay Rate is required"] });
    ////form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}


function refreshGrid() {
    var grid = document.getElementById("HRPayRate").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}

function GetData(employeePayRateId) {
    openDialogBox(employeePayRateId); 
}

function GetDataForSSN(employeeId) {
    openDialogBoxSSN(employeeId);
}

var dialogObj;
var dialogObjSSN;

function createdSSN() {

    dialogObjSSN = this;
    dialogObjSSN.hide();
}
function created() {

    dialogObj = this;
    dialogObj.hide();
}
function openDialogBox(employeePayRateId) {
    scrollVal = $(window).scrollTop();
    dialogObj.show();
    $(".e-dlg-overlay").remove();
    $("#ValidateRateSSN").html('');
    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/GetValidateRate', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ EmployeePayRateId : employeePayRateId})
    });
    ajax.send().then((data) => {
        $("#ValidateRate").html(data);
    });

    ej.popups.hideSpinner(spinnerTarget);
}

function openDialogBoxSSN(employeeId) {
    scrollVal = $(window).scrollTop();
    dialogObjSSN.show();
    $(".e-dlg-overlay").remove();
    $("#ValidateRate").html('');
    var spinnerTarget = document.getElementById('ajax_dialogSSN')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/GetValidatePasswordForSSN', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ EmployeeId: employeeId })
    });
    ajax.send().then((data) => {
        $("#ValidateRateSSN").html(data);
    });

    ej.popups.hideSpinner(spinnerTarget);
}

function promptBtnClick() {
    dialogObj.hide();
    $(window).scrollTop(scrollVal);
    scrollVal = 0;
}
function SSNpromptBtnClick() {
    dialogObjSSN.hide();
    $(window).scrollTop(scrollVal);
    scrollVal = 0;
}


function GetHourlyRateDetail(args) {
    if (args.action != "enter") {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var EmployeePayRateId = document.getElementById('EmployeePayRateId').value;
        var password = document.getElementById('password').value;
        if (password == '') {
            toastObj.content = "Password is required ";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            return false;
        }

        $.ajax({
            url: ROOTURL + 'HREmployeeProfile/GetHourlyRate',
            type: "POST",
            data: {
                "EmployeePayRateId": EmployeePayRateId, "password": password
            },
            "success": function (data) {
                if (!ej.base.isNullOrUndefined(data.success)) {

                    toastObj.content = data.success;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });

                    $("#dPayRate").val(data.data);
                }
                if (!ej.base.isNullOrUndefined(data.Error)) {

                    toastObj.content = data.Error;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
                
            }

        });
        event.preventDefault();

        console.log(EmployeePayRateId);
        console.log(password);
    }
}

function GetSSNDetail(args) {
    
    if (args.action != "enter") {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var EmployeeId = document.getElementById('EmployeeId').value;
        var password = document.getElementById('password').value;
        if (password == '') {
            toastObj.content = "Password is required ";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            return false;
        }

        $.ajax({
            url: ROOTURL + 'HREmployeeProfile/GetSSN',
            type: "POST",
            data: {
                "EmployeeId": EmployeeId, "Password": password
            },
            "success": function (data) {
                if (!ej.base.isNullOrUndefined(data.success)) {

                    toastObj.content = data.success;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });

                    $("#SSN").val(data.data);
                }
                if (!ej.base.isNullOrUndefined(data.Error)) {

                    toastObj.content = data.Error;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            }

        });
        event.preventDefault();

        console.log(EmployeeId);
        console.log(password);
    }
}

function checknegativevalidationPayrate(args) {
    var numericTextBox = this;

    numericTextBox.element.addEventListener('keypress', function (e) {
        var charCode = (e.which) ? e.which : e.keyCode;
        if (charCode == 45) {
            e.preventDefault();
        }
    });

    numericTextBox.element.addEventListener('blur', function (e) {
        if (numericTextBox.value < 1) {
            numericTextBox.value = 1;
        }
    });
}