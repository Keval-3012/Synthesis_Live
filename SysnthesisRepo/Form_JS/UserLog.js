$(document).ready(function () {
    var currentDate = new Date();
    var txtfromdate = Startdate; //'@DateTime.Now.ToString("MM-DD-yyyy")';
    $('#txtfromdate').val(txtfromdate);
    $('#txtfromdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true,
        maxDate: currentDate
    });

    $('#txtfromdate').attr('autocomplete', 'off');

});
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
function Filter() {
    showloader();
    $('#gridvaluechange').html('');
    var User = document.getElementById('User').ej2_instances[0];
    var Module = document.getElementById('Module').ej2_instances[0];
    var ActionValue = document.getElementById('ActionValue').ej2_instances[0];

    var datepicker = document.getElementById('txtfromdate');

    var ajax = new ej.base.Ajax({
        url: ROOTURL + "/UserLog/UserLogList",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ value: datepicker.value, UserName: User.value, ModuleName: Module.value, ActionName: ActionValue.value })
    });
    ajax.send().then((data) => {
        $("#gridvaluechange").html(data);
        hideloader();
    });
}
function rowData(args) {
    if (args.data["ModuleName"].toLowerCase() == "login") {
        args.row.classList.add('Loginrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "dashboard") {
        args.row.classList.add('Dashboardrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "invoice") {
        args.row.classList.add('Invoicerow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "inventory") {
        args.row.classList.add('Inventoryrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "expenses") {
        args.row.classList.add('Expensesrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "payroll") {
        args.row.classList.add('Payrollrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "reports") {
        args.row.classList.add('Reportsrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "registers") {
        args.row.classList.add('Registersrow');
    }
    else if (args.data["ModuleName"].toLowerCase() == "documents") {
        args.row.classList.add('Documentsrow');
    }
}
function customiseCell() {

}

function toolbarClick(args) {
    var gridObj = document.getElementById("InlineEditingCopy").ej2_instances[0];
    if (args.id === 'pdf') {
        gridObj.serverPdfExport("/UserLog/PdfExport");
    }
    if (args.id === 'excel') {
        gridObj.serverPdfExport("/UserLog/ExcelExport");
    }
}
