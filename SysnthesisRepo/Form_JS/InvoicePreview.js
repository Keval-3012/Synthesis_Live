$(document).ready(function () {

    $("#VendorN").val('All');

    var ajax = new ej.base.Ajax({
        url: ROOTURL + "InvoicePreview/GetListUnmapped",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: 'All' })
    });
    ajax.send().then((data) => {
        $("#GridJ3").html(data);
    });
});


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
var dialogObj;
var dialogObj1;
function rowDataBound(args) {
    //check the condition
    if (!ej.base.isNullOrUndefined(args.data.ProductId)) {
        args.row.querySelector('[title="Linked"]').classList.add('e-success')
        args.row.querySelector('[title="Link"]').classList.add('e-hide')
        args.row.querySelector('[title="Add"]').classList.add('e-hide')
    }
    else {
        args.row.querySelector('[title="Linked"]').classList.add('e-hide')
        args.row.querySelector('[title="Link"]').classList.add('e-primary')
        args.row.querySelector('[title="Add"]').classList.add('e-flat')
    }
}

function GetGridValue(args) {

    $("#VendorN").val(args.value);

    var ajax = new ej.base.Ajax({
        url: ROOTURL + "InvoicePreview/GetListUnmapped",  //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: args.value })
    });
    ajax.send().then((data) => {

        $("#GridJ3").html(data);
    });
}
function created() {
    dialogObj = this;
    dialogObj.hide();
}
function created1() {
    dialogObj1 = this;
    dialogObj1.hide();
}

function toolbarClick(args) {
    var gridObj = document.getElementById("InlineEditingU").ej2_instances[0];
    if (args.item.id === 'InlineEditingU_excelexport') {
        gridObj.serverPdfExport("/InvoicePreview/ExcelExport?vendorname=" + $("#VendorName").val());
      
    }
    else if (args.item.id === 'InlineEditingU_pdfexport') {
        gridObj.serverPdfExport("/InvoicePreview/PdfExport?vendorname=" + $("#VendorName").val());
    }
    else if (args.item.id === 'InlineEditingU_delete') {

        DeleteAll();
    }
}
function dlgButtonClick(args) {
    if (args.action != "enter") {

        var i = 0;
        var count = localStorage.getItem("Count");
        if (count != '0') {
            var grid1 = document.getElementById("InlineEditing1").ej2_instances[0];
            var selectedrowindex1 = grid1.getSelectedRowIndexes();
            var selectedrecords1 = grid1.getSelectedRecords();
            i = 1;
        }
        var grid2 = document.getElementById("InlineEditing2").ej2_instances[0];
        var selectedrowindex2 = grid2.getSelectedRowIndexes();// get the selected row indexes.
        var selectedrecords2 = grid2.getSelectedRecords();

        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var vendorname = $("#VendorN").val();
        var grid4 = document.getElementById('InlineEditingU').ej2_instances[0];
        if (i == 1) {
            if (selectedrowindex1.length != 0) {
                var ajax = new ej.base.Ajax({
                    url: ROOTURL + "InvoicePreview/AssignProduct", //render the partial view
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ value: selectedrecords1, VendorN: vendorname })
                });
                ajax.send().then(function (data) {
                    if (data == "\"Success\"") {
                        dialogObj.hide();
                        toastObj.content = Successmapped;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });

                    }
                    if (data == "\"Error\"") {
                        dialogObj.hide();
                        toastObj.content = "Something went wrong!";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }
                    /* grid3.refresh();*/

                    grid4.refresh();
                });
            }
        }
        else if (selectedrowindex2.length != 0) {
            var ajax = new ej.base.Ajax({
                url: ROOTURL + "InvoicePreview/AssignProduct", //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: selectedrecords2, VendorN: vendorname })
            });
            ajax.send().then(function (data) {

                if (data == "\"Success\"") {
                    dialogObj.hide();
                    toastObj.content = Successmapped;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });

                }
                if (data == "\"Error\"") {
                    dialogObj.hide();
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
                /* grid3.refresh();*/

                grid4.refresh();
            });
        }
        else {
            dialogObj.hide();
        }
    }
}


function actionCompleteU(args) {

    var grid4 = document.getElementById('InlineEditingU').ej2_instances[0];
    if ($("#hdnindex")[0] !== undefined) {

        var b = parseInt($("#hdnindex")[0].outerText);
        grid4.selectRow(b);
    }
}
function CancelBouttonClick(args) {
    if (args.action != "enter") {
        dialogObj.hide();
    }
}
function isreturned() {
    var flag = true;
    var VendorName = $("#Vendors").val();
    var File = $("#fileinput").val();
    if (VendorName == "") {
        flag = false;
        $("#vendorerror").text(VendorName1);
    }
    if (File == "") {
        flag = false;
        $("#fileerror").text(File);
    }
    if (flag == true) {
        return true;
    }
    else {
        return false;
    }
}
function RemoveButtonClick(args) {

    if (args.action != "enter") {

        var vendorname = $("#VendorN").val();
        var ProductVendorId = $("#ProductVendorId").val();
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var grid3 = document.getElementById('InlineEditingU').ej2_instances[0];
        var ajax = new ej.base.Ajax({
            url: ROOTURL + "InvoicePreview/RemoveProductMapping", //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ProductVendorId: ProductVendorId, VendorN: vendorname })
        });
        ajax.send().then(function (data) {
            if (data == "\"Success\"") {
                dialogObj1.hide();
                toastObj.content = RemoveMap;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (data == "\"Error\"") {
                dialogObj1.hide();
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            grid3.refresh();
        });
    }
}
function LinkedCancelBouttonClick(args) {
    if (args.action != "enter") {
        dialogObj1.hide();
    }
}

function headerCellInfo(args) {
}
function loads() {


    this.columns[2].commands[0].buttonOption.click = function (args) {

        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var grid = document.getElementById('InlineEditingU').ej2_instances[0];
        var rowData = grid.currentViewData[index];
        $("#txtid").val(rowData.ProductVendorId);
        dialogObj.show()
        var spinnerTarget = document.getElementById('ajax_dialog')
        ej.popups.createSpinner({
            target: spinnerTarget
        });
        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({
            url: ROOTURL + "InvoicePreview/GetProductsData", //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ value: rowData })
        });
        ajax.send().then((data) => {

            $("#dialogTemp").html(data);

            $.ajax({
                url: ROOTURL + "InvoicePreview/SetPopupHeader",
                type: "POST",
                data: JSON.stringify({ value: rowData }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    var html = "<span class='storeNameforPopup'>" + response.store + "</span>" +
                        "<div class='itemArea'>" +
                        "<span class='itemSectionTitle'>Mapping Item</span>" +
                        "<span class='itemSectionName'>" + response.Item + "</span>" +
                        "<span class='itemSectionName' style='font-size:15px'>" + (response.upc == null ? "" : "UPC Code : " + response.upc) + "</span>" +
                        "<span id='hdnindex' hidden>" + index + "</span>" +
                        "</div>" +
                        "<div class='storeVendorArea'>" +
                        "<span class='vendorTitle'>Vendor</span>" +
                        "<span class='vendorNameTitle 7484Store'>" + response.vendorname + "</span>" +
                        "</div>";
                    $(".itemDetailsSection").html(html);
                },
                error: function (response) {
                }
            });

        });
        ej.popups.hideSpinner(spinnerTarget);
    }
    this.columns[2].commands[1].buttonOption.click = function (args) {


        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var grid = document.getElementById('InlineEditingU').ej2_instances[0];
        var rowData = grid.currentViewData[index];
        $("#ProductVendorId").val(rowData.ProductVendorId);
        dialogObj1.show()
        var spinnerTarget = document.getElementById('ajax_dialog1')
        ej.popups.createSpinner({
            target: spinnerTarget
        });
        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({
            url: ROOTURL + "InvoicePreview/GetProductMasterData", //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ value: rowData })
        });
        ajax.send().then((data) => {

            $("#dialogTempData").html(data);

        });
        ej.popups.hideSpinner(spinnerTarget);
    }
    this.columns[2].commands[2].buttonOption.click = function (args) {

        scrollVal = $(window).scrollTop();
        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var grid = document.getElementById('InlineEditingU').ej2_instances[0];
        var rowData = grid.currentViewData[index];
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        $.ajax({
            url: ROOTURL + "InvoicePreview/AddVendorItem",
            type: "POST",
            data: JSON.stringify({ value: rowData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response == "Success") {
                    toastObj.content = AddVendor;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                }
                if (response == "Error") {
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

                grid.refresh();
            },
            error: function (response) {
            }
        });
    }



    this.columns[2].commands[3].buttonOption.click = function (args) {

        scrollVal = $(window).scrollTop();
        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var grid = document.getElementById('InlineEditingU').ej2_instances[0];
        var rowData = grid.currentViewData[index];
        var toastObj = document.getElementById('toast_type').ej2_instances[0];

        const response = confirm(Suredelete);

        if (response) {
            $.ajax({
                url: ROOTURL + "InvoicePreview/DeleteVendorItem",
                type: "POST",
                data: JSON.stringify({ value: rowData }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response == "Success") {
                        toastObj.content = DeleteSuccess;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    }
                    if (response == "Error") {
                        toastObj.content = "Something went wrong!";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }

                    grid.refresh();
                },
                error: function (response) {
                }
            });
        }
    }
}
function setitems() {
    var startdate = document.getElementById('txtstartdate').value;
    var enddate = document.getElementById('txtenddate').value;
    var Store = document.getElementById('DrpLstStore').value;
    var Vendors = document.getElementById('DrpLstVendor').value;
    localStorage.setItem("PriceFilter_StartDate", startdate);
    localStorage.setItem("PriceFilter_EndDate", enddate);
    localStorage.setItem("PriceFilter_Store", Store);
    localStorage.setItem("PriceFilter_Vendors", Vendors);
}

function DeleteAll() {

    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    var grid = document.getElementById('InlineEditingU').ej2_instances[0];
    var x = 0;
    $("#InlineEditingU tr").each(function () {
        var $checkBox = $(this).find("input[type='checkbox']");
        if ($checkBox.is(':checked')) {
            x = 1;
        }
    });

    if (x == 1) {
        const response = confirm("Are you sure you want to delete?");
        $('#loading').css('display', 'block');

        if (response) {
            var vendorid = null; let k = 0;
            $("#InlineEditingU tr").each(function () {
                var $checkBox = $(this).find("input[type='checkbox']");
                if ($checkBox.is(':checked')) {

                    if (k == 0) {
                        vendorid = $(this)[0].cells[0].innerText + ',';
                    }
                    else {
                        vendorid += $(this)[0].cells[0].innerText + ',';
                    }
                    k = 1;
                }
            });

            if (vendorid != '' && vendorid != null) {

                $.ajax({
                    url: ROOTURL + "InvoicePreview/DeleteVendorItembyselect",
                    type: "POST",
                    data: JSON.stringify({ value: vendorid }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var grid1 = document.getElementById('InlineEditingU').ej2_instances[0];
                        grid1.refresh();
                        $('#loading').css('display', 'none');
                    },
                    error: function (response) {
                        $('#loading').css('display', 'none');
                    }
                });
            }
        }
        else {
            $('#loading').css('display', 'none');
        }
    }
    else {
        confirm("Select record first..!!");
    }
}