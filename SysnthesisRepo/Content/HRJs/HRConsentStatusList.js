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

$(document).ready(function () {
    var grid = document.getElementById('HRConsentStatusList');
    var count = grid.ej2_instances[0].dataSource.length;
    var gridCountElement = document.getElementById('gridCount');
    if (count === undefined) {
        count = 0;
    }
    gridCountElement.textContent = 'Signed : ' + count;
});

function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
}

function refreshGrid() {
    var grid = document.getElementById("HRConsentStatusList").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}

function actionComplete(args) {
    if (args.requestType === 'refresh' || args.requestType === 'paging') {
        updateGridCount();
    }
}
function updateGridCount() {
    var grid = document.getElementById('HRConsentStatusList');
    var count = grid.ej2_instances[0].dataSource.length;
    var gridCountElement = document.getElementById('gridCount');
    if (count === undefined) {
        count = 0;
    }
    var signcheck = $("#SignCheck").val();
    if (signcheck == 1) {
        gridCountElement.textContent = 'Signed : ' + count;
    }
    else {
        gridCountElement.textContent = 'UnSigned : ' + count;
    }
    
}
//document.addEventListener('DOMContentLoaded', function () {
//    var grid = document.querySelector('#HRConsentStatusList').ej2_instances[0];
//});

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
    var grid = document.querySelector('#HRConsentStatusList').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        //url: "/HRConsentMasters/UrlDatasource1",
        url: "/HRConsentMasters/UrlDatasource1?fromdate=" + $("#FromDate").val() + "&todate=" + $("#ToDate").val() + "&signcheck=" + $("#SignCheck").val() + "&storeid=" + $("#StoreId").val(),
        insertUrl: "",
        updateUrl: "",
        adaptor: new CustomAdaptor()
    });
};

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("btnSearch").addEventListener("click", function () {
        fromdate = $("#FromDate").val();
        todate = $("#ToDate").val();
        signcheck = $("#SignCheck").val();
        storeid = $("#StoreId").val();

        $.ajax({
            url: ROOTURL + 'HRConsentMasters/UrlDatasource1',
            type: "POST",
            data: JSON.stringify({ fromdate: fromdate, todate: todate, signcheck: signcheck, storeid: storeid }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var grid = document.getElementById("HRConsentStatusList").ej2_instances[0];
                grid.dataSource = response;
                grid.dataBind();

                var columns = grid.columns;
                for (var i = 0; i < columns.length; i++) {
                    if (signcheck == "1") {
                        columns[i].visible = (columns[i].field !== "EmployeeId");
                    } else if (signcheck == "2") {
                        if (columns[i].field !== "EmployeeName" && columns[i].field !== "DepartmentName") {
                            columns[i].visible = false;
                        } else {
                            columns[i].visible = true;
                        }
                    }
                }
                grid.refreshHeader();
            },
            error: function (response) {
            }
        });
    });
});

function ConsentFileDownload(data) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var a = document.createElement('a');
    var span = document.createElement('span');

    a.href = '#';
    a.className = 'training-status filedownload';
    //span.textContent = 'Document';

    // Create the <img> element
    var img = document.createElement('img');
    img.alt = '';
    img.src = '/Content/Admin/images/icon-pdf.svg';
    img.style.height = '34px';

    a.setAttribute('onclick', "downloadFile('" + data.FileName + "', '" + data.EmployeeId + "')");

    //a.appendChild(span);

    a.appendChild(img);

    div.appendChild(a);
    return div.outerHTML;
}

function downloadFile(filename, employeeid) {
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    $.ajax({
        url: ROOTURL + 'HRConsentMasters/ConsentFileDownload',
        type: "POST",
        data: JSON.stringify({ filename: filename, employeeid: employeeid }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            
            if (response == "Error") {
                toastObj.content = "File Not Found..";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                return false;
            }
            else if (response == "Success") {
                var downloadUrl = ROOTURL + 'HRConsentMasters/ConsentFileDownloadSuccess?filename=' + filename + '&employeeid=' + employeeid;
                window.location = downloadUrl;
            }
        },
        error: function (response) {
        }
    });
}

function setSelect2() {
    $(".select2").select2({ closeOnSelect: true });
}
$(function () {
    setSelect2();
});

function toolbarClick(args) {
    var list = args.item.id.replace("_excelexport", "").replace("_pdfexport", "");
    
    if (args.item.id.endsWith("_excelexport")) {
        var excelExportProperties = {
            fileName: "EmployeeDocument.xlsx"
        };
        var gridObj = document.getElementById(list).ej2_instances[0];
        gridObj.excelExport(excelExportProperties);
    }
    else if (args.item.id.endsWith("_pdfexport")) {
        var pdfExportProperties = {
            fileName: "EmployeeDocument.pdf"
        };
        var gridObj = document.getElementById(list).ej2_instances[0];
        gridObj.pdfExport(pdfExportProperties);
    }
}