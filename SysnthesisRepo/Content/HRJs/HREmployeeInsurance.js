function createdHRInsurance(args) {
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
    var grid = document.querySelector('#HRInsurance').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployeeProfile/HRInsuranceUrlDatasource?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        insertUrl: "/HREmployeeProfile/HRInsuranceInsert?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        updateUrl: "/HREmployeeProfile/HRInsuranceUpdate",
        removeUrl: "/HREmployeeProfile/HRInsuranceRemove",
        adaptor: new CustomAdaptor()
    });
};

function actionBeginHRInsurance(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    if (args.requestType == "save") {
        ej.popups.showSpinner(args.dialog.element);
    }
}

function actionCompleteHRInsurance(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            AddandEdit = 1;
            args.dialog.header = 'Edit Insurance';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HREmployeeProfile/HRInsuranceEditPartial', //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                HRInsuranceappendElement(data, args.form); //render the edit form with selected record
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {

            args.dialog.header = 'Add Insurance';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HREmployeeProfile/HRInsuranceAddPartial', //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                HRInsuranceappendElement(data, args.form); //Render the edit form with selected record
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
            HRInsurancerefreshGrid()
        }
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

    }

}
function HRInsurancerefreshGrid() {
    var grid = document.getElementById("HRInsurance").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}
function HRInsuranceappendElement(elementString, form) {

    form.querySelector("#InsuranceDialogTempData").innerHTML = elementString;
    form.ej2_instances[0].addRules('sEffectiveDate', { required: [true, "Effective Date is required"] });
    //form.ej2_instances[0].addRules('Time', { required: [true, "Time is required"] });
    ////form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#InsuranceDialogTempData").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}
