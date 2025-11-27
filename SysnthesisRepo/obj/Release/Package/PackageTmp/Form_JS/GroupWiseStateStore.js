function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }

    var grid = document.getElementById("GroupWiseStateGrid").ej2_instances[0];
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

function refreshGrid() {
    var grid = document.getElementById("GroupWiseStateGrid").ej2_instances[0];
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
    var grid = document.querySelector('#GroupWiseStateGrid').ej2_instances[0];

    grid.dataSource = new ej.data.DataManager({

        url: "/GroupWiseStateStore/UrlDatasource",
        insertUrl: "/GroupWiseStateStore/InsertGroupWiseStateStore",
        updateUrl: "/GroupWiseStateStore/UpdateGroupWiseStateStore",
        removeUrl: "/GroupWiseStateStore/DeleteGroupWiseStateStore",

        adaptor: new CustomAdaptor()
    });
}
var AddandEdit = 0;
function actionComplete(args) {
    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';

            var ajax = new ej.base.Ajax({
                url: ROOTURL + "GroupWiseStateStore/EditPartial",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                appendCustomerElement(data, args.form); //render the edit form with selected record
                args.form.elements.namedItem('GroupName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {
            args.dialog.header = 'Add';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + "GroupWiseStateStore/AddPartial",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {
                console.log("Data =>", data);
                appendCustomerElement(data, args.form); //render the edit form with selected record
                args.form.elements.namedItem('GroupName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
    }
    if (args.requestType == 'save') {        
        refreshGrid()
        scrollVal = 0;
        $(window).scrollTop(scrollVal);

    }
}

function toolbarClick(args) {
    var gridObj = document.getElementById("GroupWiseStateGrid").ej2_instances[0];
}

function appendCustomerElement(elementString, form) {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('GroupName', { required: [true, "Group Name is required"] });
    //form.ej2_instances[0].addRules('MuiltiStoreAccess', { required: [true, "Store selection is required"] });
    //form.ej2_instances[0].addRules('UserId', { required: [true, "User Name is required"] });
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}
