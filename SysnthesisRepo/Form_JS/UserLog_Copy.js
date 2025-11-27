
function onfilteringAction(e) {

    var CBObj = document.getElementById("ActionValue").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('ActionName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}
function onfilteringModule(e) {

    var CBObj = document.getElementById("Module").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('ModuleName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}
function onfilteringvendor(e) {

    var CBObj = document.getElementById("User").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('UserName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}

$(document).ready(function () {
    Filterdata();
});

function Filterdata() {
    showloader();
    $('#gridvaluechange').html('');
    var User = document.getElementById('User').ej2_instances[0];
    var Module = document.getElementById('Module').ej2_instances[0];
    //var ActionValue = document.getElementById('ActionValue').ej2_instances[0];

    var datepicker = document.getElementById('txtfromdate');

    var ajax = new ej.base.Ajax({
        url: ROOTURL + "/UserLog/UserLogList_New",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ value: datepicker.value, UserName: User.value, ModuleName: Module.value })
    });
    ajax.send().then((data) => {
        $("#gridvaluechange").html(data);
        hideloader();
    });
}

function rowData(args) {
    if (args.data["ModuleName"].toLowerCase().includes("login")) {
        args.row.classList.add('Loginrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("dashboard")) {
        args.row.classList.add('Dashboardrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("invoice")) {
        args.row.classList.add('Invoicerow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("inventory")) {
        args.row.classList.add('Inventoryrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("expenses")) {
        args.row.classList.add('Expensesrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("payroll")) {
        args.row.classList.add('Payrollrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("reports")) {
        args.row.classList.add('Reportsrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("registers")) {
        args.row.classList.add('Registersrow');
    }
    else if (args.data["ModuleName"].toLowerCase().includes("documents")) {
        args.row.classList.add('Documentsrow');
    }
}
function customiseCell() {

}

function OnLoad() {

    this.element.addEventListener(
        'click',
        collapseAll.bind(this),
        true
    );

}

function collapseAll(args) {

    var grid = document.getElementById('InlineEditingCopy').ej2_instances[0];
    var tgt = event.target;
    //let tgt = args.target;
    if (tgt.closest('.e-grid').getAttribute('id') !== grid.element.getAttribute('id')) {
        // for child grid
        if (tgt.classList.contains('e-dtdiagonalright') || tgt.classList.contains('e-detailrowcollapse')) {
            var inst = tgt.closest('.e-grid').ej2_instances[0];
            console.log(inst.getRowObjectFromUID(tgt.closest('.e-row').getAttribute('data-uid')))
            inst.detailRowModule.collapseAll();
        } else if (tgt.classList.contains('e-dtdiagonaldown') || tgt.classList.contains('e-detailrowexpand')) {
            var inst = tgt.closest('.e-grid').ej2_instances[0];
            console.log(inst.getRowObjectFromUID(tgt.closest('.e-row').getAttribute('data-uid')));
        }
    } else if (tgt.classList.contains('e-dtdiagonalright') || tgt.classList.contains('e-detailrowcollapse')) {
        console.log(this.getRowObjectFromUID(tgt.closest('.e-row').getAttribute('data-uid')));
        // for Parent Grid Alone
        grid.detailRowModule.collapseAll();
    } else if (tgt.classList.contains('e-dtdiagonaldown') || tgt.classList.contains('e-detailrowexpand')) {
        console.log(this.getRowObjectFromUID(tgt.closest('.e-row').getAttribute('data-uid')));
    }
}
//function actionBegin(args) {
//    if (args.requestType == "save") {
//        this.query.params = [];
//        this.query.addParams('EmployeeID', args.data.EmployeeID);
//    }
//}

function refreshgrid() {

    var grid = document.getElementById("InlineEditingCopy").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/UserLog/UrlDatasourceUnmapped_New",
        adaptor: new ej.data.UrlAdaptor()
    });
}

function toolbarClicks(args) {
    var gridObj = document.getElementById("InlineEditingCopy").ej2_instances[0];
    if (args.item.id === 'InlineEditingCopy_pdfexport') {
        showloader();
        this.pdfExport({ hierarchyExportMode: "All" });
    }
if (args.item.id === 'InlineEditingCopy_excelexport') {
        showloader();
        this.excelExport({ hierarchyExportMode: "All" });
    }
}

function pdfExportComplete(args) {
    hideloader();
}

function excelExportComplete(args) {
    hideloader();
}