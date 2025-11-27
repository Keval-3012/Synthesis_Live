function load() {
    var grid = document.getElementById('Grid').ej2_instances[0];
    var rowHeight = grid.getRowHeight();  //height of the each row
    var gridHeight = grid.height;  //grid height
    var pageSize = grid.pageSettings.pageSize;   //initial page size
    var pageResize = (gridHeight - (pageSize * rowHeight)) / rowHeight; //new page size is obtained here
    grid.pageSettings.pageSize = pageSize + Math.round(pageResize);
}

function closemodal() {
    $(".divIDClass").hide();
}

function toolbarClick(args) {
    var gridObj = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.item.id === 'InlineEditing_pdfexport') {

        
        gridObj.serverPdfExport(ROOTURL + "VendorMasters/PdfExport");
    }
    if (args.item.id === 'InlineEditing_excelexport') {
        gridObj.serverExcelExport(ROOTURL + "VendorMasters/ExcelExport");
    }
    if (args.item.id === 'InlineEditing_print') {
        this.columns[0].visible = false;
    }
}

//function toolbarClick2(args) {
//    var list = args.item.id.replace("_excelexport", "");
//    var gridObj = document.getElementById(list).ej2_instances[0];
//    var excelExportProperties = {
//        fileName: "ItemsMovementReports.xlsx"
//    };
//    gridObj.excelExport(excelExportProperties);
//}
function toolbarClick2(args) {
    var list = args.item.id.replace("_excelexport", "").replace("_pdfexport", "");
    var gridObj = document.getElementById(list).ej2_instances[0];
    if (args.item.id.endsWith("_excelexport")) {
        var excelExportProperties = {
            fileName: "ItemsMovementReports.xlsx"
        };
        gridObj.excelExport(excelExportProperties);
    }
    else if (args.item.id.endsWith("_pdfexport")) {
        var pdfExportProperties = {
            fileName: "ItemsMovementReports.pdf"
        };
        gridObj.pdfExport(pdfExportProperties);
    }
}
$(document).ready(function () {
    

    var ajax = new ej.base.Ajax({
        url: ROOTURL + "ItemMovement/GetList", //render the partial view
        type: "POST",
        contentType: "application/json"
    });
    ajax.send().then((data) => {
        $("#GridValue").html(data);
    });
});

function ComfirmDelete(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
}

function Delete(ID) {
    $.ajax({
        url: '/ItemMovement/DeleteItemMovement',
        data: { ItemMovementID: ID },
        async: false,
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("InlineEditing").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/ItemMovement/UrlDatasource",
                adaptor: new ej.data.UrlAdaptor()
            });
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}