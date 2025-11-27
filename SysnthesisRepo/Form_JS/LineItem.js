var Oldview = "";
var OldIndex = "";

function refreshgrid() {

    var grid = document.getElementById("InlineEditingValue").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/LineItem/UrlDatasourceValue",
        adaptor: new ej.data.UrlAdaptor()
    });
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

$("body").on("click", "#InlineEditing2_searchbutton", function () {

    var grid = document.getElementById('InlineEditing2').ej2_instances[0];
    var value = document.getElementById("InlineEditing2_searchbar").value;
    grid.search(value);
});

$("body").on("click", "#InlineEditing1_searchbutton", function () {

    var grid = document.getElementById('InlineEditing1').ej2_instances[0];
    var value = document.getElementById("InlineEditing1_searchbar").value;
    grid.search(value);
});


function customiseCell(args) {   
    if (args.column.headerText === 'Item') {
        if (args.data['Approved'] == false) {
            if (args.data['Description_Accuracy'] != null) {
                if (args.data['Description_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Description_Accuracy'] > 85 && args.data['Description_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Description_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'Qty') {
        if (args.data['Approved'] == false) {
            if (args.data['Qty_Accuracy'] != null) {
                if (args.data['Qty_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Qty_Accuracy'] > 85 && args.data['Qty_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Qty_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'Total') {
        if (args.data['Approved'] == false) {
            if (args.data['Total_Accuracy'] != null) {
                if (args.data['Total_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['Total_Accuracy'] > 85 && args.data['Total_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['Total_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }
    else if (args.column.field === 'UnitPrice') {
        if (args.data['Approved'] == false) {
            if (args.data['UnitPrice_Accuracy'] != null) {
                if (args.data['UnitPrice_Accuracy'] < 85) {
                    args.cell.classList.add('below-85');
                } else if (args.data['UnitPrice_Accuracy'] > 85 && args.data['UnitPrice_Accuracy'] < 95) {
                    args.cell.classList.add('below-95');
                } else if (args.data['UnitPrice_Accuracy'] > 95) {
                    args.cell.classList.add('above-95');
                }
            }
        }
        else {
            args.cell.classList.add('above-95');
        }
    }

}
var scrollVal = 0;
function getfile(args) {

    var id = args.id;
    $.ajax({
        url: ROOTURL + 'LineItem/GetInvoiceFilePath',
        type: "POST",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {


            $('#file').attr('src', response);
        },
        error: function (response) {
        }
    });
}
var dialogObj;
var dialogObj1;
var dialogObj2;
var grid4;
function created() {

    dialogObj = this;
    dialogObj.hide();
}
function created1() {

    dialogObj1 = this;
    dialogObj1.hide();
}
function created2() {

    dialogObj2 = this;
    dialogObj2.hide();
}
function dlgButtonClick(args) {
    
    if (args.action != "enter") {
        var grid1 = document.getElementById("InlineEditing1");
        var selectedrowindex1;
        var selectedrecords1;
        var datalist = 0;
        if (grid1 != null) {
            datalist = 1;
            grid1 = grid1.ej2_instances[0];
            selectedrowindex1 = grid1.getSelectedRowIndexes();
            selectedrecords1 = grid1.getSelectedRecords();
        }

        var grid2 = document.getElementById("InlineEditing2").ej2_instances[0];
        var selectedrowindex2 = grid2.getSelectedRowIndexes();// get the selected row indexes.
        var selectedrecords2 = grid2.getSelectedRecords();

        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        //var vendorname = $("#VendorN").val();
        //var grid3 = document.getElementById('InlineEditingValue').ej2_instances[0];
        if (datalist != 0) {
            if (selectedrowindex1.length != 0) {
                var ajax = new ej.base.Ajax({
                    url: ROOTURL + 'LineItem/AssignProduct', //render the partial view
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ value: selectedrecords1 })
                });
                ajax.send().then(function (data) {
                    if (data == "\"Success\"") {
                        dialogObj.hide();
                        toastObj.content = mapped;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    }
                    if (data == "\"Error\"") {
                        dialogObj.hide();
                        toastObj.content = "Something went wrong!";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }
                    grid4.refresh();
                    $(window).scrollTop(scrollVal);
                    scrollVal = 0;
                });
            }
        }
        else if (datalist == 0) {
            if (selectedrowindex2.length != 0) {
                var ajax = new ej.base.Ajax({
                    url: ROOTURL + 'LineItem/AssignProduct', //render the partial view
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ value: selectedrecords2 })
                });
                ajax.send().then(function (data) {
                    if (data == "\"Success\"") {
                        dialogObj.hide();
                        toastObj.content = mapped;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    }
                    if (data == "\"Error\"") {
                        dialogObj.hide();
                        toastObj.content = "Something went wrong!";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }
                    grid4.refresh();
                    $(window).scrollTop(scrollVal);
                    scrollVal = 0;
                });
            }
        }
        else {
            dialogObj.hide();
        }
    }
}
function CancelBouttonClick(args) {

    if (args.action != "enter") {
        dialogObj.hide();
    }
}
function actionBegin(args) {
    if (args.requestType == "save") {
        if (args.data.InvoiceId == null) {
            args.data.InvoiceId = this.parentDetails.parentRowData.InvoiceId
        }
    }
    if (args.requestType == "beginEdit" || args.requestType == "add") {
        var id = new ej.base.closest(event.target, '.e-detailcell').firstChild.id
        var grid = document.getElementById(id).ej2_instances[0];
        grid.columns[3].visible = false;
        grid.columns[4].visible = true;

        for (var i = 0; i < this.columns.length; i++) {

            if (this.columns[i].field == "Approved") {

                this.columns[i].visible = false;
            }
        }

    }

}


function actionComplete(args) {

}


function collapseAll(args) {    
    var grid = document.getElementById('InlineEditingValue').ej2_instances[0];
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
function OnLoad() {

    this.element.addEventListener(
        'click',
        collapseAll.bind(this),
        true
    );

}

function AddtoLibrary(Type) {
    //dialogObj2.show()
    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/GetAddLibrary', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val() })
    });
    ajax.send().then((data) => {
        //Oldview = $("#MapContent").html();
        //$("#MapContent").html(data);
        //$("#MapContent").removeClass("itemsGridPlaceHolder");
        //$("#MapContent").addClass("addItemToLibrary");
        $("#AddnewItem").html(data);
        if (Type == "Master") {
            $("#AddnewItem").addClass("saveToMaster");
            var MUPCCODE = $("#upccode").val();
            var SUPCCODE = $("#hdnselectedUPC").text();
            if ((MUPCCODE == null || MUPCCODE == "" || MUPCCODE == undefined) && (SUPCCODE != null && SUPCCODE != "" && SUPCCODE != undefined)) {
                $("#AddUPCCode").val(SUPCCODE);
            }
        }
        else {
            $("#AddnewItem").addClass("saveToVendor");
            var MUPCCODE = $("#upccode").val();
            var SUPCCODE = $("#hdnMselectedUPC").text();
            if ((MUPCCODE == null || MUPCCODE == "" || MUPCCODE == undefined) && (SUPCCODE != null && SUPCCODE != "" && SUPCCODE != undefined)) {
                $("#AddUPCCode").val(SUPCCODE);
            }
        }
        $(".masterItemsGrid").hide();
        $("#btnSaveandClose").hide();
        $("#btnSaveandMap").hide();
        $("#MapContent").hide();
        $("#AddnewItem").show();
        $("#hdnItemType").val(Type);
    });
    ej.popups.hideSpinner(spinnerTarget);
}

function changethetab() {

    var rowData = grid4.currentViewData[OldIndex];
    var VendorName = $("#VendorList_hidden").val();
    FillSearchMasterItemData(rowData, VendorName);
    FillSearchVendorItemData(rowData, VendorName);

    $(".masterItemsGrid").show();
    $("#btnSaveandMap").show();
    $("#MapContent").show();
    $("#btnSaveandClose").show();
    $("#AddnewItem").html('');
    $("#AddnewItem").hide();

}

function load(args) {
	VFile = 0;   
    this.columns[10].commands[0].buttonOption.click = function (args) {
        scrollVal = $(window).scrollTop();
        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        OldIndex = row.getAttribute('aria-rowindex')
        var id = new ej.base.closest(event.target, '.e-detailcell').firstChild.id
        grid4 = document.getElementById(id).ej2_instances[0];
        var rowData = grid4.currentViewData[index];
        //$("#txtid").val(rowData.ProductVendorId);
        dialogObj.show()
        $(".e-dlg-overlay").remove();
        var spinnerTarget = document.getElementById('ajax_dialog')
        ej.popups.createSpinner({
            target: spinnerTarget
        });
        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({
            url: ROOTURL + 'LineItem/GetInvoiceProductData', //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ value: rowData, VendorName: $("#VendorList_hidden").val() })
        });
        ajax.send().then((data) => {

            $("#dialogTemp").html(data);

            FillVendorGrid(rowData, VendorName);
            FillMasterGrid(rowData, VendorName);
            $("#AddnewItem").hide();


        });
        ej.popups.hideSpinner(spinnerTarget);
    }

    this.columns[10].commands[1].buttonOption.click = function (args) {
        scrollVal = $(window).scrollTop();
        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var id = new ej.base.closest(event.target, '.e-detailcell').firstChild.id
        grid4 = document.getElementById(id).ej2_instances[0];
        var rowData = grid4.currentViewData[index];
        $("#InvoiceProductId").val(rowData.InvoiceProductId);
        //dialogObj1.show()
        var spinnerTarget = document.getElementById('ajax_dialog1')
        ej.popups.createSpinner({
            target: spinnerTarget
        });
        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({
            url: ROOTURL + 'LineItem/Unlinkitem',  //render the partial view
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ value: rowData })
        });
        ajax.send().then((data) => {
            var toastObj = document.getElementById('toast_type').ej2_instances[0];
            if (data == "\"Success\"") {
                toastObj.content = unlinked;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (data == "\"Error\"") {
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            grid4.refresh();

            //$("#dialogTempData").html(data);
        });
        ej.popups.hideSpinner(spinnerTarget);
    }

    this.columns[10].commands[2].buttonOption.click = function (args) {

        scrollVal = $(window).scrollTop();
        var row = new ej.base.closest(event.target, '.e-row'); // get row element
        var index = row.getAttribute('aria-rowindex')
        var id = new ej.base.closest(event.target, '.e-detailcell').firstChild.id
        grid4 = document.getElementById(id).ej2_instances[0];
        var rowData = grid4.currentViewData[index];
        var toastObj = document.getElementById('toast_type').ej2_instances[0];

        $.ajax({
            url: ROOTURL + 'LineItem/AddVendorLineItem',
            type: "POST",
            data: JSON.stringify({ value: rowData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response == "Success") {
                    toastObj.content = AddVendorItem;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                }
                if (response == "Error") {
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            },
            error: function (response) {
            }
        });
    }


}
var VFile = 0;
function rowDataBound(args) {
    
    args.row.querySelector('[title="Add to vendor library"]').classList.add('e-hide')

    //check the condition
    if (!ej.base.isNullOrUndefined(args.data.ProductId)) {
        args.row.querySelector('[title="Linked"]').classList.add('e-success')
        args.row.querySelector('[title="Link"]').classList.add('e-hide')
    }
    else {
        args.row.querySelector('[title="Linked"]').classList.add('e-hide')
        args.row.querySelector('[title="Link"]').classList.add('e-primary')
    }

	if(VFile == 0){
    var id = args.data.InvoiceId;
    $.ajax({
        url: ROOTURL + 'LineItem/GetInvoiceFilePath',
        type: "POST",
        data: JSON.stringify({ id: id }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {


            $('#file').attr('src', response);
        },
        error: function (response) {
        }
    });
	VFile = 1;
	}

}



function rowselected(args) {


    if (args.target.id != 'btnapproveall' && args.target.id != 'imgReset') {
        var id = args.data.InvoiceId;
        $.ajax({
            url: ROOTURL + 'LineItem/GetInvoiceFilePath',
            type: "POST",
            data: JSON.stringify({ id: id }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {


                $('#file').attr('src', response);
            },
            error: function (response) {
            }
        });
    }

}

$(document).ready(function () {
    $("#AddnewItem").hide();

    var vendorId = document.getElementById('VendorList').value;
    var InvoiceDate = new Date($("#InvoiceDateValue").val());
    var checked = document.getElementById('checked').ej2_instances[0];
    var checkvalue = checked.checked;
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/GetList', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: vendorId, Date: InvoiceDate, checkvalue: checkvalue })
    });
    ajax.send().then((data) => {
        $("#GridValue").html(data);

    });
});
function FillSearchVendorItemData() {

    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);

    $.ajax({
        type: "POST",
        url: ROOTURL + 'LineItem/GetVendorProductDatawithDesc',
        contentType: "application/json",
        data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val(), search: $("#searchVendorItemData").val(), InvoiceId: $("#hdnInvoiceId").val(), VendorName: $("#VendorList_hidden").val(), search2: $("#searchMasterItemData").val(), VendorName: $("#VendorList_hidden").val() }),
        success: function (response) {
            $("#VendorItemGrid").html(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

    ej.popups.hideSpinner(spinnerTarget);
}
function FillSearchMasterItemData() {

    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    $.ajax({
        type: "POST",
        url: ROOTURL + 'LineItem/GetInvoiceProductDatawithDesc',
        contentType: "application/json",
        data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val(), search: $("#searchMasterItemData").val(), InvoiceId: $("#hdnInvoiceId").val(), VendorName: $("#VendorList_hidden").val(), search2: $("#searchVendorItemData").val(), VendorName: $("#VendorList_hidden").val() }),
        success: function (response) {
            $("#MasterItemGrid").html(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
    ej.popups.hideSpinner(spinnerTarget);
}
function GetUnmappedvalue(args) {
    
    //$("#VendorN").val(args.value);
    var VendorName = document.getElementById('VendorList').ej2_instances[0].itemData.VendorName;
    var checked = document.getElementById('checked').ej2_instances[0];
    var checkvalue = checked.checked;
    var InvoiceDate = new Date($("#InvoiceDateValue").val());
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/GetList',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: VendorName, Date: InvoiceDate, checkvalue: checkvalue })
    });
    ajax.send().then((data) => {
        $("#GridValue").html(data);
    });

}

function GetGridValue(args) {
    
    var InvoiceDate = new Date($("#InvoiceDateValue").val());
    var checked = document.getElementById('checked').ej2_instances[0];
    var checkvalue = checked.checked;
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/GetList',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: args.value, Date: InvoiceDate, checkvalue: checkvalue })
    });
    ajax.send().then((data) => {
        $("#GridValue").html(data);
    });

}
function GetValueUsingDate(args) {

    $("#InvoiceDateValue").val(args.value);
    var checked = document.getElementById('checked').ej2_instances[0];
    var checkvalue = checked.checked;
    var VendorName = document.getElementById('VendorList').ej2_instances[0].itemData.VendorName;
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/GetList',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ vendorname: VendorName, Date: args.value, checkvalue: checkvalue })
    });
    ajax.send().then((data) => {
        $("#GridValue").html(data);
    });

}
function LinkedCancelBouttonClick(args) {

    if (args.action != "enter") {
        dialogObj1.hide();
    }
}
function RemoveButtonClick(args) {

    if (args.action != "enter") {
        var InvoiceProductId = $("#InvoiceProductId").val();
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var ajax = new ej.base.Ajax({
            url: ROOTURL + 'LineItem/RemoveProductMapping',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ InvoiceProductId: InvoiceProductId })
        });
        ajax.send().then(function (data) {
            if (data == "\"Success\"") {
                dialogObj1.hide();
                toastObj.content = removemapping;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (data == "\"Error\"") {
                dialogObj1.hide();
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            grid4.refresh();
            $(window).scrollTop(scrollVal);
            scrollVal = 0;
        });
    }

}

function AssignProduct() {
    var hdnddlProductID = $("#hdnddlProductID").val();
    var hdnddlProductVendorID = $("#hdnddlProductVendorID").val();
    if (hdnddlProductID != null && hdnddlProductID != "" && hdnddlProductID != undefined && hdnddlProductVendorID != null && hdnddlProductVendorID != "" && hdnddlProductVendorID != undefined) {

        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var spinnerTarget = document.getElementById('ajax_dialog')
        ej.popups.createSpinner({
            target: spinnerTarget
        });
        ej.popups.showSpinner(spinnerTarget);
        var ajax = new ej.base.Ajax({
            url: ROOTURL + 'LineItem/AssignProduct',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val(), search: $("#search").val(), InvoiceId: $("#hdnInvoiceId").val(), ProductId: hdnddlProductID, ProductVendorId: hdnddlProductVendorID })
        });
        ajax.send().then(function (data) {
            if (data == "\"Success\"") {
                dialogObj.hide();
                toastObj.content = mapped;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                ej.popups.hideSpinner(spinnerTarget);
            }
            if (data == "\"Error\"") {
                dialogObj.hide();
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                ej.popups.hideSpinner(spinnerTarget);
            }
            grid4.refresh();
            $(window).scrollTop(scrollVal);
            scrollVal = 0;
        });
        grid4.refresh();
    }
    else {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        toastObj.content = SelectBoth;
        toastObj.target = document.body;
        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
    }
}

function MapandsyncItem(ProductId, Type) {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'LineItem/syncLineItem',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val(), search: $("#search").val(), InvoiceId: $("#hdnInvoiceId").val(), ProductId: ProductId, Type: Type, VendorName: $("#VendorList_hidden").val() })
    });
    ajax.send().then(function (data) {
        if (data.split('|')[0] == "\"Success") {
            var ajax2 = new ej.base.Ajax({
                url: ROOTURL + 'LineItem/AssignProduct',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ InvoiceProductId: $("#hdnInvoiceProductId").val(), Description: $("#Description").val(), UPCCode: $("#upccode").val(), ItemNo: $("#itemno").val(), search: $("#search").val(), InvoiceId: $("#hdnInvoiceId").val(), ProductId: data.split('|')[1].replace('"', "") })
            });
            ajax2.send().then(function (data2) {
                if (data2 == "\"Success\"") {
                    dialogObj.hide();
                    toastObj.content = mapped;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    ej.popups.hideSpinner(spinnerTarget);
                }
                if (data2 == "\"Error\"") {
                    dialogObj.hide();
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    ej.popups.hideSpinner(spinnerTarget);
                }
                grid4.refresh();
                $(window).scrollTop(scrollVal);
                scrollVal = 0;
            });
        }
        if (data == "\"Error\"") {
            dialogObj.hide();
            toastObj.content = "Something went wrong!";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            ej.popups.hideSpinner(spinnerTarget);
        }
        grid4.refresh();
        $(window).scrollTop(scrollVal);
        scrollVal = 0;
    });
    grid4.refresh();
}

function AddAndMapBtn() {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    var spinnerTarget = document.getElementById('ajax_dialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var AddDescription = $("#AddDescription").val();
    var hdnItemType = $("#hdnItemType").val();
    if (AddDescription != "" && AddDescription != undefined && AddDescription != null) {
        if (hdnItemType == "Vendor") {

            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'LineItem/syncLineItem',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ InvoiceProductId: $("#AddInvoiceProductId").val(), Description: $("#AddDescription").val(), UPCCode: $("#AddUPCCode").val(), ItemNo: $("#AddItemNo").val(), search: "", InvoiceId: 0, ProductId: 0, Type: 0, Brand: $("#AddBrand").val(), Size: $("#AddSize").val(), VendorName: $("#VendorList_hidden").val() })
            });
            ajax.send().then(function (data3) {
                if (data3.split('|')[0] == "\"Success") {
                    var rowData = grid4.currentViewData[OldIndex];
                    var VendorName = $("#VendorList_hidden").val();
                    SelectVendorRow(data3.split('|')[1].replace('"', ""), $("#AddUPCCode").val(), $("#AddItemNo").val(), $("#AddDescription").val(), $("#AddBrand").val());
                    FillSearchVendorItemData(rowData, VendorName);
                    //dialogObj.hide();
                    changethetab();
                    toastObj.content = VendorItem;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                }
                if (data3 == "\"Error\"") {
                    //dialogObj.hide();
                    changethetab();
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
                grid4.refresh();
                $(window).scrollTop(scrollVal);
                scrollVal = 0;
                ej.popups.hideSpinner(spinnerTarget);


            });
            grid4.refresh();
        }
        else {
            var ajax3 = new ej.base.Ajax({
                url: ROOTURL + 'LineItem/syncLineItem',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ InvoiceProductId: $("#AddInvoiceProductId").val(), Description: $("#AddDescription").val(), UPCCode: $("#AddUPCCode").val(), ItemNo: $("#AddItemNo").val(), search: "", InvoiceId: 0, ProductId: 0, Type: 1, Brand: $("#AddBrand").val(), Size: $("#AddSize").val(), VendorName: $("#VendorList_hidden").val() })
            });
            ajax3.send().then(function (data3) {

                if (data3.split('|')[0] == "\"Success") {
                    var rowData = grid4.currentViewData[OldIndex];
                    var VendorName = $("#VendorList_hidden").val();
                    SelectMasterRow(data3.split('|')[1].replace('"', ""), $("#AddUPCCode").val(), $("#AddItemNo").val(), $("#AddDescription").val(), $("#AddBrand").val());
                    FillSearchMasterItemData(rowData, VendorName);
                    //dialogObj.hide();
                    changethetab();
                    toastObj.content = MasterItem;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                }
                if (data3 == "\"Error\"") {
                    //dialogObj.hide();
                    changethetab();
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }
                grid4.refresh();
                $(window).scrollTop(scrollVal);
                scrollVal = 0;
                ej.popups.hideSpinner(spinnerTarget);
            });
            grid4.refresh();

        }
    }
    else {
        toastObj.content = "Please enter Item Name!";
        toastObj.target = document.body;
        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
        ej.popups.hideSpinner(spinnerTarget);
    }
    grid4.refresh();
}
function FNselectProduct(ProductID, VendorProductID, type) {
    if (type == 0) {
        $(".TempProduct").removeAttr("selectedrow");
        $("#recommendedItemlst_" + ProductID).attr("selectedrow", "yes");
        //$("#hdnddlProductID").val(ProductID);

        $(".TempVendorProduct").removeAttr("selectedrow");
        $("#recommendedVendorItemlst_" + VendorProductID).attr("selectedrow", "yes");
        //$("#hdnddlProductVendorID").val(VendorProductID);
    }
    else {
        $(".TempVendorProduct").removeAttr("selectedrow");
        $("#recommendedVendorItemlst_" + VendorProductID).attr("selectedrow", "yes");
        //$("#hdnddlProductVendorID").val(VendorProductID);

        $(".TempProduct").removeAttr("selectedrow");
        $("#recommendedItemlst_" + ProductID).attr("selectedrow", "yes");
        //$("#hdnddlProductID").val(ProductID);
    }
}

function SelectMasterRow(ProductID, UPCCODE, ItemNO, Description, Brand) {
    $("#hdnMselectedUPC").text(((UPCCODE == null || UPCCODE == "" || UPCCODE == undefined) ? "-" : UPCCODE));
    $("#hdnMselectedItemNo").text(((ItemNO == null || ItemNO == "" || ItemNO == undefined) ? "-" : ItemNO));
    $("#hdnMselectedDescription").text(((Description == null || Description == "" || Description == undefined) ? "-" : Description));
    $("#hdnMselectedBrand").text(((Brand == null || Brand == "" || Brand == undefined) ? "-" : Brand));
    $("#hdnddlProductID").val(ProductID);
    $("#MasterrecommendedItemsGridHeader").addClass("displayNone");
    $("#MasterItemGrid").addClass("displayNone");
    $("#MasterSearch").addClass("displayNone");
    $(".masterConfirmedItem").removeClass("displayNone");
}

function SelectVendorRow(ProductID, UPCCODE, ItemNO, Description, Brand) {
    $("#hdnselectedUPC").text(((UPCCODE == null || UPCCODE == "" || UPCCODE == undefined) ? "-" : UPCCODE));
    $("#hdnselectedItemNo").text(((ItemNO == null || ItemNO == "" || ItemNO == undefined) ? "-" : ItemNO));
    $("#hdnselectedDescription").text(((Description == null || Description == "" || Description == undefined) ? "-" : Description));
    $("#hdnselectedBrand").text(((Brand == null || Brand == "" || Brand == undefined) ? "-" : Brand));
    $("#hdnddlProductVendorID").val(ProductID);
    $("#VendorrecommendedItemsGridHeader").addClass("displayNone");
    $("#VendorItemGrid").addClass("displayNone");
    $("#VendorSearch").addClass("displayNone");
    $(".vendorConfirmedItem").removeClass("displayNone");
}

function CancelMasterRow() {
    $("#hdnMselectedUPC").text('');
    $("#hdnMselectedItemNo").text('');
    $("#hdnMselectedDescription").text('');
    $("#hdnMselectedBrand").text('');
    $("#hdnddlProductID").val(0);
    $("#MasterrecommendedItemsGridHeader").removeClass("displayNone");
    $("#MasterItemGrid").removeClass("displayNone");
    $("#MasterSearch").removeClass("displayNone");
    $(".masterConfirmedItem").addClass("displayNone");
}

function CancelVendorRow() {
    $("#hdnselectedUPC").text('');
    $("#hdnselectedItemNo").text('');
    $("#hdnselectedDescription").text('');
    $("#hdnselectedBrand").text('');
    $("#hdnddlProductVendorID").val(0);
    $("#VendorrecommendedItemsGridHeader").removeClass("displayNone");
    $("#VendorItemGrid").removeClass("displayNone");
    $("#VendorSearch").removeClass("displayNone");
    $(".vendorConfirmedItem").addClass("displayNone");
}

