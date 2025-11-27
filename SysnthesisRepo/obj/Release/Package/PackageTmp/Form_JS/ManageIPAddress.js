function customFn(args) {

    var ip = /^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$/;
    if (args.value.match(ip)) {
        return true;
    }
    else {
        return false;
    }
}

function actionBegin(args) {
}

function statusDetail(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.Status === "Active") {
        span.className = 'statustxt e-activecolor';
        span.textContent = "Active"
        div.className = 'statustemp e-activecolor'
    }
    if (e.Status === "Inactive") {
        span = document.createElement('span');
        span.className = 'statustxt e-inactivecolor';
        span.textContent = "Inactive"
        div.className = 'statustemp e-inactivecolor'
    }
    div.appendChild(span);
    return div.outerHTML;
}

function queryCellInfo(args) {

    if (args.column.field === 'Status') {
        if (args.cell.textContent === "Active") {
            args.cell.querySelector(".statustxt").classList.add("e-activecolor");
            args.cell.querySelector(".statustemp").classList.add("e-activecolor");
        }
        if (args.cell.textContent === "Inactive") {
            args.cell.querySelector(".statustxt").classList.add("e-inactivecolor");
            args.cell.querySelector(".statustemp").classList.add("e-inactivecolor");
        }
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
    var grid = document.querySelector('#InlineEditing').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/TimeclockIPControls/UrlDatasource",
        insertUrl: "/TimeclockIPControls/Insert",
        updateUrl: "/TimeclockIPControls/Update",
        removeUrl: "/TimeclockIPControls/Remove",

        adaptor: new CustomAdaptor()
    });
};

function actionComplete(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + "/TimeclockIPControls/Editpartial", //render the partial view
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
                url: ROOTURL + "/TimeclockIPControls/Addpartial", //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
    }
    if (args.requestType == 'save') {
    }
}

function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('StaticIp', { minLength: [customFn, '@ViewBag.ValidIPAdd'] });
    form.ej2_instances[0].addRules('StartIP', { minLength: [customFn, '@ViewBag.ValidIPAdd'] });
    form.ej2_instances[0].addRules('EndIP', { minLength: [customFn, '@ViewBag.ValidIPAdd'] });
    form.ej2_instances[0].addRules('MultiUserId', { required: [true, '@ViewBag.LeastOneUser'] });//adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}

function onChange(args) {

    if (args.value == "Static") {
        $("#Main2").attr("hidden", true);
        $("#Main1").removeAttr("hidden");
    }
    else if (args.value == "Range") {
        $("#Main1").attr("hidden", true);
        $("#Main2").removeAttr("hidden");
    }
}

function ValidateRange() {

    var flag = true;
    var startIp = $("#StartIP").val();
    var EndIP = $("#EndIP").val();
    var s = startIp.split('.');
    var st = s[3];
    var e = EndIP.split('.');
    var et = e[3];
    if (s[0] != e[0] && s[1] != e[1] && s[2] != e[2]) {

        alert('@ViewBag.StartEndDiff');
        $("#EndIP").val("");
        flag = false;
    }
    if (st > et) {
        alert('@ViewBag.StartGreatEnd');
        $("#EndIP").val("");
        flag = false;
    }

    return flag;
}