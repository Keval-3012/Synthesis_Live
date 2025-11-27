function toggle(button) {

    var List = button.id.split("^");
    switch (button.value) {
        case "on":
            button.value = "off";
            StoreId = List[0];
            IsActive = false;
            var text = "Are you sure you want to deactivate Store Manager: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'HRStoreManagers/UpdateStatusStoreManager',
                    type: "POST",
                    data: JSON.stringify({ StoreId: StoreId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("HRStoreManager").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Store Manager Status Successfully Updated.";
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
                var grid = document.getElementById("HRStoreManager").ej2_instances[0];
                grid.refresh();
            }
            break;
        case "off":
            button.value = "on";
            StoreId = List[0];
            IsActive = true;
            var text = "Are you sure you want to activate Store Manager: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'HRStoreManagers/UpdateStatusStoreManager',
                    type: "POST",
                    data: JSON.stringify({ StoreId: StoreId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("HRStoreManager").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Store Manager Status Successfully Updated.";
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
                var grid = document.getElementById("HRStoreManager").ej2_instances[0];
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

var designationData = []; // Variable to store the designation data

// Fetch designation data using AJAX
function fetchDesignationData() {    
    var designationDataManager = new ej.data.DataManager({
        url: '/HRStoreManagers/GetFirstName',
        adaptor: new ej.data.UrlAdaptor(),
    });
    designationDataManager.executeQuery(new ej.data.Query()).then(function (data) {
            designationData = data.result.map((item) => ({
                FirstName: item.FirstName,
                UserName: item.UserName,
                StoreName: item.StoreName,
            }));
            // Store the fetched data
        });

}

// Call the function to fetch designation data
fetchDesignationData();

function actionBegin(args) {    
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    if (args.requestType === 'filterbeforeopen') {
        args.filterModel.options.dataSource = designationData;
        args.filterModel.options.filteredColumns = args.filterModel.options.filteredColumns.filter(function (col) {
            if (col.field == 'FirstName') {
                return true;
            }
            if (col.field == 'UserName') {
                return true;
            }
            if (col.field == 'StoreName') {
                return true;
            }
            return false;
        });
    }
}

//function actionBegin(args) {
//    if (args.requestType == "beginEdit") {
//        scrollVal = $(window).scrollTop();
//    }
//    if (args.requestType === "filterbeforeopen" && args.columnName === "FirstName") {
//        debugger;
//        var ajax = new ej.base.Ajax({
//            url: ROOTURL + 'HRStoreManagers/GetFirstName', //render the partial view
//            type: "POST",
//            contentType: "application/json",
//            data: JSON.stringify({ columnName : args.columnName })
//        });
//        ajax.send().then(function (data) {
//            debugger;
//            args.filterModel.options.dataSource = [
//                { FirstName: "Test1" },
//                { FirstName: "Test2" },
//                { FirstName: "Test3" },
//                { FirstName: "Test4" }
//            ]
//            /*appendElement(data, args.form); //render the edit form with selected record*/
//            /*ej.popups.hideSpinner(args.dialog.element);*/
//        }).catch(function (xhr) {
//            console.log(xhr);
//            /*ej.popups.hideSpinner(args.dialog.element);*/
//        });
//        //args.filterModel.options.dataSource = [
//        //    { FirstName: "Test1" },
//        //    { FirstName: "Test2" },
//        //    { FirstName: "Test3" },
//        //    { FirstName: "Test4" }
//        //]
//    }
//}
var AddandEdit = 0;
function actionComplete(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HRStoreManagers/Editpartial', //render the partial view
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

            args.dialog.header = 'Add';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'HRStoreManagers/Addpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                //args.form.elements.namedItem('FirstName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
    }
    if (args.requestType == 'save') {
        //var grid6 = document.getElementById("InlineEditing").ej2_instances[0];
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
        var grid = document.getElementById("HRStoreManager").ej2_instances[0];
        grid.refresh();
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

        //alert("If you want to any change in this vendor then you must have to active first, otherwise the change will not take place in QuickBooks.");
    }

}

function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    //form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}
function refreshGrid() {
    var grid = document.getElementById("HRStoreManager").ej2_instances[0];
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
                fetchDesignationData();
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
    var grid = document.querySelector('#HRStoreManager').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HRStoreManagers/UrlDatasource",
        insertUrl: "/HRStoreManagers/InsertStoreManager",
        updateUrl: "/HRStoreManagers/UpdateStoreManager",
        //removeUrl: "/HRStoreManagers/RemoveDepartment",
        adaptor: new CustomAdaptor()
    });
};

function PasswordHideShow(a) {
    let input = $($(a).attr('toggle'));
    if (input.attr('type') === 'password') {
        input.attr('type', 'text');
        $(a).removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        input.attr('type', 'password');
        $(a).removeClass('fa-eye-slash').addClass('fa-eye');
    }
};

function PasswordMatchCheck() {
    let password = $('#Password').val();
    let confirmPassword = $('#ConfirmPassword').val();
    if (password !== confirmPassword && confirmPassword != "") {
        $('#password-error').show();
        $('#password-error').html('Passwords do not match.').css('color', 'red');
    } else if (password == confirmPassword && password != "" && confirmPassword != "") {
        $('#password-error').show();
        $('#password-error').html('Matching').css('color', 'green');
    }
    else {
        $('#password-error').hide();
    }
}
