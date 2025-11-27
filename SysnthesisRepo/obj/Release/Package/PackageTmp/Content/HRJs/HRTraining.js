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

function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
}

function refreshGrid() {
    var grid = document.getElementById("HRTraining").ej2_instances[0];
    if (grid) {
        grid.refresh();
    }
}

function actionComplete(args) {

}

function TraningTempCreate(data) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var a = document.createElement('a');
    var span = document.createElement('span');

    a.href = '/HRTraining/CerificateDownload?filename=' + data.TrainingContent;
    a.className = 'training-status certificate';
    span.textContent = 'Certificate';

    // Create the <img> element
    var img = document.createElement('img');
    img.alt = '';
    img.src = '/images/download_ico.png';

    a.appendChild(span);

    a.appendChild(img);

    div.appendChild(a);
    return div.outerHTML;
}

document.addEventListener('DOMContentLoaded', function () {
    //var resetButton = document.getElementById('resetButton');
    //resetButton.addEventListener('click', function () {
    //    showConfirmationDialog();
    //});
    var grid = document.querySelector('#HRTraining').ej2_instances[0];

    // Handle click event for download button
    grid.element.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('certificate')) {
            var data = grid.getRowInfo(e.target.closest('.e-row')).rowData;
            var downloadUrl = data.TrainingContent;
            certificatedownload(downloadUrl);
        }
    });
});
function showConfirmationDialog() {

    var grid = document.getElementById('HRTraining').ej2_instances[0];
    var selectedRecords = grid.getSelectedRecords();
    
    var selectedIds = selectedRecords.map(function (record) {
        return record.EmployeeId;
    });

    if (selectedIds.length > 0) {
        var confirmation = window.confirm('Are you sure you want to reset training of this Employee?');
        if (confirmation) {
            resetTraining();
        }
    }
    else {
        var confirmation = window.confirm('Please select atleast one record!');
        if (confirmation) {
            return;
        }
    }


}

function resetTraining() {
    var grid = document.getElementById('HRTraining').ej2_instances[0];
    var selectedRecords = grid.getSelectedRecords();
    
    var selectedIds = selectedRecords.map(function (record) {
        return record.EmployeeId;
    });

    // Make an AJAX request to your controller
    $.ajax({
        type: 'POST',
        url: '/HRTraining/ResetTraining',
        data: { selectedIds: selectedIds },
        success: function (data) {
            var toastObj = document.getElementById('toast_type').ej2_instances[0];
            if (data == "deleted") {
                toastObj.show({ title: 'Employee Training Reset Successfully.', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });


            }
        },
        error: function (error) {
            // Handle error
            console.error(error);
        }
    });

}
function certificatedownload(e) {
    var filename = e;
    
    // Make an AJAX request to your controller
    $.ajax({
        type: 'POST',
        url: '/HRTraining/CerificateDownload',
        data: { filename: filename },
        success: function (data) {
            
            if (data == "Success") {
                
                var toastObj = document.getElementById('toast_type').ej2_instances[0];
                toastObj.content = response.message;
                toastObj.target = document.body;
                toastObj.show({ title: 'File Downloaded Successfully.', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                refreshGrid();
            }
        },
        error: function (error) {
            
            // Handle error
            console.error(error);
        }
    });
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
    var grid = document.querySelector('#HRTraining').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HRTraining/UrlDatasource",
        insertUrl: "",
        updateUrl: "",
        adaptor: new CustomAdaptor()
    });
};
function toolbarClick(args) {
    
    if (args.item.id === 'Reset') {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            var confirmation = window.confirm("No records selected for Reset Training operation");
        } else {
            var selectedPrimaryKey = selectedRecords[0]["EmployeeId"];
            var confirmation = window.confirm('Are you sure want to reset this employee Training?');
            if (confirmation) {
                
                $.ajax({
                    url: '/HRTraining/ResetTraining',
                    type: 'POST',
                    data: {
                        selectedIds: selectedPrimaryKey
                    },
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "deleted") {
                            toastObj.content = response.message;
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Employee Training Reset Successfully.', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                            refreshGrid();
                        } else {
                            toastObj.content = response.message;
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error(xhr.responseText);
                    }
                });
            } else {
                console.log("Closed");
            }
        }
    }
}