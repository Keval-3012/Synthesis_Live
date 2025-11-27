$(document).ready(function () {
    localStorage.removeItem("checkvalue");
    AddEditRemove();
    $("#InlineEditing_toolbarItems").find(".e-toolbar-items.e-tbar-pos").addClass("gridToolbar");
    $("#InlineEditing_toolbarItems").find(".e-toolbar-items.e-tbar-pos").find(".e-toolbar-left").addClass("gridButtonContainer");
});

function fileupload() {
    var fileUpload = $("#fileinput").get(0);
    let formData = new FormData();
    formData.append("postedFile", fileUpload.files[0]);
    $.ajax({
        url: ROOTURL + "/ProductMappings/ImportProductExcel",
        type: "POST",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: formData,
        success: function (data) {
            window.location.href = '/ProductMappings/ImportProductExcel';
        }
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

var elem;
var dObj;
function create(args) {
    elem = document.createElement('input');
    return elem;
}

function write(args) {
    let multiselectDatasource = [
        { Country: 'France', Id: '1' },
        { Country: 'Germany', Id: '2' },
        { Country: 'Brazil', Id: '3' },
        { Country: 'Switzerland', Id: '4' },
        { Country: 'Venezuela', Id: '5' },
    ];
    dObj = new ej.dropdowns.MultiSelect({
        value: args.rowData[args.column.field] ? args.rowData[args.column.field].split(',') : [],
        placeholder: 'Add KeyWord',
        floatLabelType: 'Never',
        mode: 'Box',
        allowCustomValue: true
    });
    dObj.appendTo(elem);
}

function destroy() {
    dObj.destroy();
}

function read(args) {
    return dObj.value.join(',');
}

function closemodal() {
    $(".divIDClass").hide();
}

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }

}

function GetLinkedItems(args) {
    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    var checked = document.getElementById('checkedLinked').ej2_instances[0];
    var checkvalue = checked.checked;
    localStorage.setItem("checkvalue", checkvalue);
    AddEditRemove();
    /*grid.refresh();*/
}

function rowPositionChange(args) {
    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    grid.editSettings.newRowPosition = this.value;
}

function actionBegin(args) {
    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.requestType === 'add') {
        $.ajax({
            url: ROOTURL + "/ProductMappings/GetSynthesisId",
            type: "GET",
            success: function (data) {
                console.log(data);
                if (data != null) {
                    $("#InlineEditingSynthesisId").val(data);
                }
            }
        });
    }
}

function actionComplete(args) {

}

function closemodal() {
    $(".divIDClass").hide();
}

    Loader(0);

//$(document).ready(function () {
//    $("#productTbl").tablesorter();
//});

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }

}

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    if (val == 1) {
        $("#secloader").removeClass('pace-active1');
        $("#secloader").addClass('pace-active');
        $("#dvloader").removeClass('bak1');
        $("#dvloader").addClass('bak');
        $("#globalFooter").addClass('LoaderFooter');
        top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
    }
    else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
}
function toolbarClick(args) {
    var gridObj = document.getElementById("InlineEditing").ej2_instances[0];
    if (args.item.id === 'InlineEditing_pdfexport') {

        gridObj.serverPdfExport("/ProductMappings/PdfExport");
    }
    else if (args.item.id === 'InlineEditing_excelexport') {
        gridObj.serverPdfExport("/ProductMappings/ExcelExport");
    }
    //if (args.id === 'pdf') {
    //    gridObj.serverPdfExport("/ProductMappings/PdfExport");
    //}
    //if (args.id === 'excel') {
    //    gridObj.serverPdfExport("/ProductMappings/ExcelExport");
    //}
}