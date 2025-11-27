function toggle(button) {

    var List = button.id.split("^");
    switch (button.value) {
        case "on":
            button.value = "off";
            DepartmentId = List[0];
            IsActive = false;
            var text = "Are you sure you want to deactivate Department: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'HRDepartmentMasters/UpdateStatusDepartment',
                    type: "POST",
                    data: JSON.stringify({ DepartmentId: DepartmentId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("HRDepartment").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Department Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("HRDepartment").ej2_instances[0];
                grid.refresh();
            }
            break;
        case "off":
            button.value = "on";
            DepartmentId = List[0];
            IsActive = true;
            var text = "Are you sure you want to activate Department: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'HRDepartmentMasters/UpdateStatusDepartment',
                    type: "POST",
                    data: JSON.stringify({ DepartmentId: DepartmentId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("HRDepartment").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Department Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("HRDepartment").ej2_instances[0];
                grid.refresh();
            }
            break;
    }
}

function dataBound(e) {

    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
        var span = ej.base.createElement('span', {
            id: grid.element.id + '_searchcancelbutton',
            className: 'e-clear-icon'
        });
        span.addEventListener('click', (args) => {
            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
            grid.search("");
        });
        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
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
        //var grid = document.getElementById("HRDepartment").ej2_instances[0];
        //grid.refresh();
        if (AddandEdit == 1) {
            refreshGrid()
        }
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    }
    
}
function refreshGrid() {
    var grid = document.getElementById("HRDepartment").ej2_instances[0];
    if (grid) {
        grid.refresh();
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
    var grid = document.querySelector('#HRDepartment').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HRDepartmentMasters/UrlDatasource",
        insertUrl: "/HRDepartmentMasters/InsertDepartment",
        updateUrl: "/HRDepartmentMasters/UpdateDepartment",
        removeUrl: "/HRDepartmentMasters/RemoveDepartment",
        adaptor: new CustomAdaptor()
    });
};

