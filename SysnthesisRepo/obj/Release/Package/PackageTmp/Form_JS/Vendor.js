
function toggle(button) {

    var List = button.id.split("^");
    switch (button.value) {
        case "on":
            button.value = "off";
            VendorID = List[0];
            IsActive = false;
            var text = "Are you sure you want to deactivate vendor: " + List[1] + " from store " + List[2] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'VendorMasters/UpdateVendorStatus',
                    type: "POST",
                    data: JSON.stringify({ VendorId: VendorID, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("InlineEditing").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = VendorStatus == undefined ? '' : VendorStatus;
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
                var grid = document.getElementById("InlineEditing").ej2_instances[0];
                grid.refresh();
            }
            break;
        case "off":
            button.value = "on";
            VendorID = List[0];
            IsActive = true;
            var text = "Are you sure you want to activate vendor: " + List[1] + " from store " + List[2] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'VendorMasters/UpdateVendorStatus',
                    type: "POST",
                    data: JSON.stringify({ VendorId: VendorID, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("InlineEditing").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = VendorStatus == undefined ? '' : VendorStatus;
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
                var grid = document.getElementById("InlineEditing").ej2_instances[0];
                grid.refresh();
            }
            break;
    }
}

function rowDataBound(args) {
    //check the condition

    if (args.data.Status == "Active") {
        args.row.querySelector('[title="Copy Vendor"]').classList.add('e-primary')

    }
    else {

        args.row.querySelector('[title="Copy Vendor"]').classList.add('e-hide')

    }
}
function dataBound(e) {

    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    //  checks whether the cancel icon is already present or not
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

function loadgrid() {
    var row = new ej.base.closest(event.target, '.e-row'); // get row element
    var index = row.getAttribute('aria-rowindex')
    var grid = document.getElementById('InlineEditingCopy').ej2_instances[0];

    var rowData = grid.currentViewData[index];
    var vendorId = $("#VendorCopyID").val();

    dialogObj.show()
    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'VendorMasters/PasteVendordatastoreWise',//render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ StoreId: rowData.StoreId, vendorId: vendorId })
    });
    ajax.send().then((data) => {
        $("#dialogTempData").html(data);

    });
    ej.popups.hideSpinner(spinnerTarget);

}
var scrollVal = 0;
//$(window).scroll(function (event) {
//    scrollVal = $(window).scrollTop();
//    // Do something
//});
function onfiltering(e) {

    var CBObj = document.getElementById("MultiDepartmentId").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('DepartmentName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}

var dialogObj;


function createds() {

    dialogObj = this;
    dialogObj.hide();
}
function toolbarClick(args) {
    var gridObj = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.item.id === 'InlineEditing_pdfexport') {
        gridObj.serverPdfExport(ROOTURL + 'VendorMasters/PdfExport');

    }
    if (args.item.id === 'InlineEditing_excelexport') {
        /*gridObj.showSpinner();*/
        //Loader(1);
        gridObj.serverExcelExport(ROOTURL + 'VendorMasters/ExcelExport');
    }
    if (args.item.id === 'InlineEditing_print') {
        this.columns[0].visible = false;
    }
}

function printComplete(args) {
    this.columns[0].visible = true;
}


function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    var storeId = storeid;
    if (storeId == '') {
        storeId = '0';
    }
    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    var column = grid.getColumnByField('StoreName');
    if (storeId != '0') {
        if (column.visible == true) {
            column.visible = false;

            grid.refresh();
        }
    }
}
function statusDetails(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.Statuss === "Active") {
        span.className = 'statustxts e-activecolor';
        span.textContent = "Active"
        div.className = 'statustemp e-activecolor'
    }
    if (e.Statuss === "Inactive") {
        span = document.createElement('span');
        span.className = 'statustxts e-inactivecolor';
        span.textContent = "Inactive"
        div.className = 'statustemp e-inactivecolor'
    }
    div.appendChild(span);
    return div.outerHTML;
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
function queryCellInfos(args) {

    if (args.column.field === 'Statuss') {
        if (args.cell.textContent === "Active") {
            args.cell.querySelector(".statustxts").classList.add("e-activecolor");
            args.cell.querySelector(".statustemps").classList.add("e-activecolor");
        }
        if (args.cell.textContent === "Inactive") {
            args.cell.querySelector(".statustxts").classList.add("e-inactivecolor");
            args.cell.querySelector(".statustemps").classList.add("e-inactivecolor");
        }
    }
}
$(document).ready(function () {
    $(".loading-containers").attr("style", "display:block");
    sessionStorage.setItem("VendorIsAsc", "1");
});
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
    //.DataSource(dataManager => {dataManager.Url("/VendorMasters/UrlDatasource").Adaptor("UrlAdaptor").InsertUrl("/VendorMasters/Insert").UpdateUrl("/VendorMasters/Update").RemoveUrl("/VendorMasters/Remove"); })
    var grid = document.querySelector('#InlineEditing').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/VendorMasters/UrlDatasource",
        insertUrl: "/VendorMasters/Insert",
        updateUrl: "/VendorMasters/Update",
        //removeUrl: "/VendorMasters/Remove",

        adaptor: new CustomAdaptor()
    });
};
function load() {

    this.columns[12].commands[0].buttonOption.click = function (args) {


        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var grid = document.getElementById('InlineEditing').ej2_instances[0];
        var rowData = grid.currentViewData[index];
        $("#VendorCopyID").val(rowData.VendorId);
        $("#VendorName").val(rowData.VendorName);
        dialogObj.show()
        var spinnerTarget = document.getElementById('ajax_dialog1')
        ej.popups.createSpinner({
            target: spinnerTarget
        });

        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({

            url: ROOTURL + 'VendorMasters/GetStorevalueforVendor', //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ vendorId: rowData.VendorId, VendorName: rowData.VendorName })
        });
        ajax.send().then((data) => {
            $("#dialogTempData").html(data);

        });
        ej.popups.hideSpinner(spinnerTarget);
    }
}
function actionComplete(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'VendorMasters/Editpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                appendElement(data, args.form); //render the edit form with selected record
                args.form.elements.namedItem('VendorName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {

            args.dialog.header = 'Add';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'VendorMasters/Addpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                args.form.elements.namedItem('VendorName').focus();
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
        var grid = document.getElementById("InlineEditing").ej2_instances[0];
        grid.refresh();
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

        //alert("If you want to any change in this vendor then you must have to active first, otherwise the change will not take place in QuickBooks.");
    }

}
function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('VendorName', { required: [true, "Display Name is required"] });
    //form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}

function SetDepartmentDropDown(e) {
    var dropObj = document.getElementById('MultiStoreId').ej2_instances[0];
    var ddldist = document.getElementById('MultiDepartmentId').ej2_instances[0];
    if (dropObj.value.length != 0) {
        $('#VendorName').attr('disabled', false);
        $('#CompanyName').attr('disabled', false);
        $('#PrintOnCheck').attr('disabled', false);
        $('#PhoneNumber').attr('disabled', false);
        $('#Address').attr('disabled', false);
        $('#Address2').attr('disabled', false);
        $('#City').attr('disabled', false);
        $('#PostalCode').attr('disabled', false);
        $('#EMail').attr('disabled', false);
        $('#Instruction').attr('disabled', false);
        $('#Country').attr('disabled', false);
        $('#AccountNumber').attr('disabled', false);
        var PhoneNumber = document.getElementById('PhoneNumber').ej2_instances[0];
        var state = document.getElementById('State').ej2_instances[0];
        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        state.enabled = true;
        PhoneNumber.enabled = true;
        IsActive.disabled = false;
        ddldist.enabled = true;
    } else {
        $('#VendorName').attr('disabled', true);
        $('#CompanyName').attr('disabled', true);
        $('#PrintOnCheck').attr('disabled', true);
        $('#PhoneNumber').attr('disabled', true);
        $('#Address').attr('disabled', true);
        $('#Address2').attr('disabled', true);
        $('#City').attr('disabled', true);
        $('#PostalCode').attr('disabled', true);
        $('#EMail').attr('disabled', true);
        $('#Instruction').attr('disabled', true);
        $('#Country').attr('disabled', true);
        $('#AccountNumber').attr('disabled', true);
        var PhoneNumber = document.getElementById('PhoneNumber').ej2_instances[0];
        var state = document.getElementById('State').ej2_instances[0];
        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        state.enabled = false;
        PhoneNumber.enabled = false;
        IsActive.disabled = true;
        ddldist.enabled = false;
    }
    $.ajax({
        url: ROOTURL + 'VendorMasters/SetDepartment',
        type: "POST",
        data: JSON.stringify({ Mult: dropObj.value }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            ddldist.dataSource = response;
            ddldist.dataBind();
        },
        error: function (response) {
        }
    });
}
function SetCompanyName() {
    $("#CompanyName").val($("#VendorName").val());
}
function SetPrintOnCheck() {
    $("#PrintOnCheck").val($("#CompanyName").val());
}

function toolbarClick2(args) {
    var gridObj = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.item.id === 'InlineEditing_pdfexport') {
        gridObj.serverPdfExport("/VendorMasters/PdfExport");
    }
    if (args.item.id === 'InlineEditing_excelexport') {
        gridObj.serverPdfExport("/VendorMasters/ExcelExport");
    }
}