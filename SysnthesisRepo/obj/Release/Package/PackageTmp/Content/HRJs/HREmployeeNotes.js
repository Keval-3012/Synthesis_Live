function createdHRNotes(args) {
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
    var grid = document.querySelector('#HRNotes').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployeeProfile/HRNotesUrlDatasource?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        insertUrl: "/HREmployeeProfile/HRNotesInsert?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId + "&EmployeeChildId=" + EmployeeChildId,
        adaptor: new CustomAdaptor()
    });
};

function actionBeginHRNotes(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    if (args.requestType == "save") {
        ej.popups.showSpinner(args.dialog.element);
    }
}

function actionCompleteHRNotes(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'add') {
            args.dialog.header = 'Add';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HREmployeeProfile/HRNotesAddPartial', //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                HRNotesappendElement(data, args.form); //Render the edit form with selected record
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
            HRNotesrefreshGrid()
        }
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

    }
}
function HRNotesrefreshGrid() {
    var grid = document.getElementById("HRNotes").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}
function HRNotesappendElement(elementString, form) {

    form.querySelector("#NotesdialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('Notes', { required: [true, "Note is required"] });
    //form.ej2_instances[0].addRules('Time', { required: [true, "Time is required"] });
    ////form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#NotesdialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}