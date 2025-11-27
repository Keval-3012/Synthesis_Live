function ShowdatabyCheck() {   

    checkBox = document.getElementById('Remember');
    var AccountTypeId = document.getElementById('AccountTypeId').ej2_instances[0];
    var AccountDetailTypeId = document.getElementById('AccountDetailTypeId').ej2_instances[0];
    var IsSubAccounts = document.getElementById('IsSubAccounts').ej2_instances[0];
    if (checkBox.checked) {
        $('#DepartmentName').attr('disabled', false);
        $('#AccountNumber').attr('disabled', false);
        AccountTypeId.enabled = true;
        AccountDetailTypeId.enabled = true;
        IsSubAccounts.enabled = true;
        $('#Description').attr('disabled', false);

        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        IsActive.disabled = false;
        $("#StoreId").val("");
        $("#StoreId").show();
        $(".e-multi-select-wrapper").hide();

    }
    else {
        $('.IsSubRemember :input').attr("disabled", true);
        $("#StoreId").val("");

        $("#StoreId").hide();
        $(".e-multi-select-wrapper").show();
        IsSubAccounts.enabled = false;
    }
}
function SetStoreDropDown(e) {

    var dropObj = document.getElementById('MultiStoreId').ej2_instances[0];
    var AccountTypeId = document.getElementById('AccountTypeId').ej2_instances[0];
    var AccountDetailTypeId = document.getElementById('AccountDetailTypeId').ej2_instances[0];

    if (dropObj.value.length != 0) {
        $('#DepartmentName').attr('disabled', false);
        $('#AccountNumber').attr('disabled', false);
        AccountTypeId.enabled = true;
        AccountDetailTypeId.enabled = true;

        $('#Description').attr('disabled', false);

        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        IsActive.disabled = false;

    } else {
        $('#DepartmentName').attr('disabled', true);
        $('#AccountNumber').attr('disabled', true);

        AccountTypeId.enabled = true;
        AccountDetailTypeId.enabled = true;
        $('#Description').attr('disabled', true);
        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        IsActive.disabled = true;


    }

}
function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    var storeId = storeid;
    if (storeId == '') {
        storeId = '0';
    }
    var grid = document.getElementById("DepartmentIndex").ej2_instances[0];
    var column = grid.getColumnByField('StoreName');
    if (storeId != '0') {
        if (column.visible == true) {
            column.visible = false;
            grid.refresh();
        }
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
function SetdataByAccountTypeID() {
    

    var GetTypes = document.getElementById('AccountTypeId').ej2_instances[0].text;
    var AccountDetailTypeId = document.getElementById('AccountDetailTypeId').ej2_instances[0];

    $("#AccountDetailTypeId").val(document.getElementById('AccountTypeId').ej2_instances[0].text);

    $.ajax({
        url: ROOTURL + "QBConfiguration/GetDetailAccountType",
        type: "POST",
        data: { "GetAccountType": GetTypes },
        success: function (response) {
            if (response.length > 0) {
                $('#AccountDetailTypeId').html('');
                AccountDetailTypeId.dataSource = response;
                AccountDetailTypeId.dataBind();
            }
            else {

                $('#AccountDetailTypeId').html('');
                var options = '';
                options += '<option value="0">Select</option>';
                AccountDetailTypeId.dataSource = options;
                AccountDetailTypeId.dataBind();
            }
        },
        error: function (response) {
        }
    });

    var IsSubAccounts = document.getElementById('IsSubAccounts').ej2_instances[0];
    var GetTypeIDtexr = document.getElementById('AccountTypeId').ej2_instances[0].text;
    var storId = $("#StoreId").find("option:selected").val();
    if (storId != "") {
        $.ajax({
            url: ROOTURL + "QBConfiguration/IsSubAccounttype",
            type: "POST",
            data: { "GetAccountType": GetTypeIDtexr, "StoreID": storId },
            success: function (response) {
                
                if (response.length > 0) {
                    $('#IsSubAccounts').html('');
                    IsSubAccounts.dataSource = response;
                    IsSubAccounts.dataBind();
                }
                else {

                    $('#IsSubAccounts').html('');
                    var options = '';
                    options += '<option value="0">Select</option>';
                    $('#IsSubAccounts').append(options);

                }
            },
            error: function (response) {
            }
        });
    }
}
$("#IsSubAccounts").change(function () {
    $("#IsSubAccount").val($('#IsSubAccounts').find("option:selected").val());
})
function SetdatabyStoreID() {
    

    var optionSelected = $("#StoreId").find("option:selected").val();
    if (optionSelected == "") {
        $('#DepartmentName').attr('disabled', true);
        $('#AccountNumber').attr('disabled', true);
        $('#AccountTypeId').attr('disabled', true);
        $('#AccountDetailTypeId').attr('disabled', true);
        $('#Description').attr('disabled', true);
        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        IsActive.disabled = true;
        var dropObj = document.getElementById('MultiStoreId').ej2_instances[0];
        dropObj.value = null;
    }
    else {
        $('#DepartmentName').attr('disabled', false);
        $('#AccountNumber').attr('disabled', false);
        $('#AccountTypeId').attr('disabled', false);
        $('#AccountDetailTypeId').attr('disabled', false);

        $('#Description').attr('disabled', false);

        var IsActive = document.getElementById('IsActive').ej2_instances[0];
        IsActive.disabled = false;
        var dropObj = document.getElementById('MultiStoreId').ej2_instances[0];
        dropObj.value = optionSelected;

    }

    $.ajax({
        url: ROOTURL + 'QBConfiguration/GetQBTypeAndFlag',
        type: "POST",
        data: { "StoreID": optionSelected },
        success: function (response) {
            
            if (response.QBType == "Online") {
                if (response.DesktopTrue == "False") {
                    $("#DepartmentName").attr('maxlength', 500);
                    $("#StoreID").val(response.StoreID);
                    $("#QBType").val(response.QBType);
                    getAccountType(response.QBType);
                    $('#AccountDetailTypeId option').map(function () {
                        if ($(this).text() == "Select") return this;
                    }).attr('selected', 'selected');
                }
                else {
                    $("#DepartmentName").attr('maxlength', 41);
                    $("#StoreID").val(response.StoreID);
                    $("#QBType").val(response.QBType);
                    getAccountType(response.QBType);
                    $('#AccountDetailTypeId option').map(function () {
                        if ($(this).text() == "Select") return this;
                    }).attr('selected', 'selected');
                }
            }
            else if (response.QBType == "Desktop") {
                $("#DepartmentName").attr('maxlength', 41);
                $("#StoreID").val(response.StoreID);
                $("#QBType").val(response.QBType);
                getAccountType(response.QBType);
                $('#AccountDetailTypeId option').map(function () {
                    if ($(this).text() == "Select") return this;
                }).attr('selected', 'selected');

            }
            else {
                $("#DepartmentName").attr('maxlength', 41);
                $("#StoreID").val(response.StoreID);
                $("#QBType").val(response.QBType);
                getAccountType(response.QBType);
                $('#AccountDetailTypeId option').map(function () {
                    if ($(this).text() == "Select") return this;
                }).attr('selected', 'selected');

            }

        },
        error: function (Result) {
        }
    });
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
function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('DepartmentName', { required: [true, "Display Name is required"] });
    //form.ej2_instances[0].addRules('CustomerID', { required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    $("#StoreId").hide();
    serverScript.remove();
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
    var grid = document.querySelector('#DepartmentIndex').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/ChartofAccounts/UrlDatasource",
        insertUrl: "/ChartofAccounts/InsertDepartment",
        updateUrl: "/ChartofAccounts/UpdateDepartments",
        adaptor: new CustomAdaptor()
    });
};
function getAccountType(iType) {
    

    $.ajax({
        url: ROOTURL + "QBConfiguration/GetAccountType",
        type: "POST",
        data: { "Type": iType },
        success: function (response) {
            if (response.length > 0) {
                $('#AccountTypeId').html('');
                var options = '';
                options += '<option value="0">Select</option>';
                for (var i = 0; i < response.length; i++) {
                    options += '<option value="' + response[i].AccountTypeId + '">' + response[i].CommonType + '</option>';
                }
                $('#AccountTypeId').append(options);
            }
            else {
                $('#AccountTypeId').html('');
                var options = '';
                options += '<option value="0">Select</option>';
                $('#AccountTypeId').append(options);

            }
        },
        error: function (response) {
        }
    });
}
function actionComplete(args) {
    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + "ChartofAccounts/Editpartial",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                appendElement(data, args.form); //render the edit form with selected record
                args.form.elements.namedItem('DepartmentName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {
            args.dialog.header = 'Add';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + "ChartofAccounts/Addpartial",
                type: "POST",
                contentType: "application/json",
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                args.form.elements.namedItem('DepartmentName').focus();
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
    }
    if (args.requestType == 'save') {
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    }
}
function ActiveStatus(iii) {

    $.ajax({
        url: ROOTURL + 'ChartofAccounts/ActiveStatus',
        data: { deptid: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {

            if (states == "OK") {
                
                toastr.success(ActiveStatus);
                var grid = document.getElementById("DepartmentIndex").ej2_instances[0];
                grid.dataSource = new ej.data.DataManager({
                    url: "/ChartofAccounts/UrlDatasource",
                    adaptor: new ej.data.UrlAdaptor()
                });
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}
function InActiveStatus(iii) {

    $.ajax({
        url: ROOTURL + 'ChartofAccounts/InActiveStatus',
        data: { deptid: iii },
        beforeSend: function () { Loader(1); },
        async: false,
        success: function (states) {

            if (states == "OK") {
                
                toastr.success(InactiveStatus);
                var grid = document.getElementById("DepartmentIndex").ej2_instances[0];
                grid.dataSource = new ej.data.DataManager({
                    url: "/ChartofAccounts/UrlDatasource",
                    adaptor: new ej.data.UrlAdaptor()
                });
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

function toolbarClick(args) {
    var gridObj = document.getElementById("DepartmentIndex").ej2_instances[0];
    if (args.item.id === 'DepartmentIndex_pdfexport') {
        gridObj.serverPdfExport("/ChartofAccounts/PdfExport");
    }
    if (args.item.id === 'DepartmentIndex_excelexport') {
        gridObj.serverExcelExport("/ChartofAccounts/ExcelExport");
    }
    if (args.item.id === 'DepartmentIndex_csvexport') {
        gridObj.serverCsvExport("/ChartofAccounts/CsvExport");
    }
}