function createdHRTraining(args) {
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
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
    var grid = document.querySelector('#HRTraining').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployeeProfile/HRTrainingHistoryUrlDatasource?EmployeeId=" + EmployeeId + "&StoreId=" + StoreId,
        adaptor: new CustomAdaptor()
    });
};

function actionBeginHRTraining(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
}

function actionCompleteHRTraining(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0]
}

function HRTrainingappendElement(elementString, form) {

    form.querySelector("#TrainingdialogTemp").innerHTML = elementString;
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#TrainingdialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}