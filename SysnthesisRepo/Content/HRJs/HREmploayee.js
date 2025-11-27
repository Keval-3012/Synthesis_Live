function SSNTempCreate(data) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var a;
    a = document.createElement('a');
    a.href = "#";
    a.setAttribute('onclick', 'GetDataForSSN(' + data.EmployeeId+ ')');
    //a.setAttribute('data-toggle', 'modal');
    //a.setAttribute('data-target', '#ValidateRateSSN');
    var text1 = document.createTextNode('xxx-xx-');
    var text2 = document.createTextNode(data.SSN.substring(data.SSN.length - 4));
    a.appendChild(text1);
    a.appendChild(text2);
    div.appendChild(a);
    return div.outerHTML;
}

function TraningTempCreate(data) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var a = document.createElement('a');
    var span = document.createElement('span');
    //if (data.IsTraningCompleted == true) {
    //    a.href = '/HREmployee/DownLoadFile?TraningFilePath=' + data.TraningContent;
    //    a.className = 'training-status certificate';
    //    span.textContent = 'Certificate';

    //    // Create the <img> element
    //    var img = document.createElement('img');
    //    img.alt = '';
    //    img.src = '/images/download_ico.png';

    //    a.appendChild(span);

    //    a.appendChild(img);
    //}
    if (data.LastSlidename === '.q14Data' || data.LastSlidename === '.q14DataSp') {     
        a.className = 'training-status completed';
        span.textContent = 'Completed';
        a.appendChild(span);
    }
    else {
        
        a.className = 'training-status';
        span.textContent = 'Pending';
        a.appendChild(span);
    }
    div.appendChild(a);
    return div.outerHTML;
}

function  StatusName(data)  {
  
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var a = document.createElement('a');
    var span = document.createElement('span');

    if (data.Status == 1) {
        span.textContent = 'Active';
        a.className = 'status-active';
        a.appendChild(span);
    }
    else if (data.Status == 2) {
        a.className = 'status-inactive';
        span.textContent = 'Terminated';
        a.appendChild(span);
    }
    else if (data.Status == 3) {
        a.className = 'status-inactive';
        span.textContent = 'Deceased';
        a.appendChild(span);
    }
    else {
        a.className = 'status-inactive';
        span.textContent = 'Resigned';
        a.appendChild(span);
    }
      
    div.appendChild(a);
    return div.outerHTML;
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
    var grid = document.querySelector('#HREmployee').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployee/UrlDatasource",
        //insertUrl: "/HRDepartmentMasters/InsertDepartment",
        //updateUrl: "/HRDepartmentMasters/UpdateDepartment",
        //removeUrl: "/HRDepartmentMasters/RemoveDepartment",
        adaptor: new CustomAdaptor()
    });
};

function clickHandler(args) {
    if (args.item.id === "Click") {
        alert("Cutom toolbar click...");
    }
} 

function toolbarClick(args) {
    if (args.item.id === "Add") {
        window.location.href = '/HREmployee/Create';
    }
    if (args.item.id === "Edit") {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            showCustomDialog("No records selected for edit operation");
        } else {
            // Assuming "EmployeeId" is the primary key field
            var selectedPrimaryKey = selectedRecords[0]["EmployeeId"];
            // Now you have the primary key of the selected row
            console.log("Selected primary key:", selectedPrimaryKey);
            // Perform your edit operation here using the selected primary key
            window.location.href = "/HREmployee/Edit?EmployeeId=" + selectedPrimaryKey;
        }
    }
    if (args.item.id === "delete") {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            showCustomDialog("No records selected for delete operation.");
        } else {
            var selectedPrimaryKey = selectedRecords[0]["EmployeeId"];
            showConfirmationDialog('Are you sure you want to delete this record?', function (result) {
                if (result) {
                    $.ajax({
                        url: '/HREmployee/Delete', // URL to your server-side delete endpoint
                        type: 'POST', // or 'GET', depending on your server-side implementation
                        data: {
                            EmployeeId: selectedPrimaryKey
                            }, // Data to send to the server
                        success: function (response) {
                            // Handle success response from the server
                            var toastObj = document.getElementById('toast_type').ej2_instances[0];
                            if (response.success) {
                                // Reload the page or update UI as needed
                                var grid = document.getElementById("HREmployee").ej2_instances[0];
                                grid.refresh();
                                toastObj.content = response.message;
                                toastObj.target = document.body;
                                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                            } else {
                                // Handle error or display error message
                                toastObj.content = response.message;
                                toastObj.target = document.body;
                                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                            }
                        },
                        error: function (xhr, status, error) {
                            // Handle AJAX call error
                            console.error(xhr.responseText);
                        }
                    });
                } else {
                    console.log("Closed");
                    // User clicked Cancel or closed the dialog
                    // Handle the cancellation or take appropriate action
                }
            });
        }
    }
    if (args.item.id === 'Reset') {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            showCustomDialog("No records selected for Reset Password operation");
        } else {
            var selectedPrimaryKey = selectedRecords[0]["EmployeeId"];
            showConfirmationDialog('Are you sure want to reset this employee Password?', function (result) {
                if (result) {
                    $.ajax({
                        url: '/HREmployee/ResetPassword', // URL to your server-side delete endpoint
                        type: 'POST', // or 'GET', depending on your server-side implementation
                        data: {
                            EmployeeId: selectedPrimaryKey
                        }, // Data to send to the server
                        success: function (response) {
                            // Handle success response from the server
                            var toastObj = document.getElementById('toast_type').ej2_instances[0];
                            if (response.success) {
                                // Reload the page or update UI as needed
                                toastObj.content = response.message;
                                toastObj.target = document.body;
                                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                            } else {
                                // Handle error or display error message
                                toastObj.content = response.message;
                                toastObj.target = document.body;
                                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                            }
                        },
                        error: function (xhr, status, error) {
                            // Handle AJAX call error
                            console.error(xhr.responseText);
                        }
                    });
                } else {
                    console.log("Closed");
                    // User clicked Cancel or closed the dialog
                    // Handle the cancellation or take appropriate action
                }
            });
        }
    }
    if (args.item.id === 'HREmployee_excelexport') {
        var gridObj = document.getElementById('HREmployee').ej2_instances[0];
        //gridObj.serverPdfExport("/HREmployee/ExcelExport");
        var storedFilters = sessionStorage.getItem('employeeFilters');
        gridObj.serverExcelExport("/HREmployee/ExcelExport?filterss=" + encodeURIComponent(storedFilters));
    }
    if (args.item.id === 'HREmployee_pdfexport') {
        var gridObj = document.getElementById('HREmployee').ej2_instances[0];
        //gridObj.serverPdfExport("/HREmployee/PdfExport");
        var storedFilters = sessionStorage.getItem('employeeFilters');
        gridObj.serverExcelExport("/HREmployee/PdfExport?filterss=" + encodeURIComponent(storedFilters));
    }
    if (args.item.id === 'Filter') {
        createdFilterDialog();
    }
    if (args.item.id === 'ResetTraining') {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            var confirmation = window.confirm("No records selected for Reset Training operation");
        } else {
            //var selectedPrimaryKey = selectedRecords[0]["EmployeeId"];
            var selectedIds = selectedRecords.map(function (record) {
                return record["EmployeeId"];
            });
            var confirmation = window.confirm('Are you sure want to reset this employee Training?');
            if (confirmation) {
                $.ajax({
                    url: '/HRTraining/ResetTraining',
                    type: 'POST',
                    data: {
                        selectedIds: selectedIds
                    },
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "deleted") {
                            toastObj.content = response.message;
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Employee Training Reset Successfully.', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                            applyFilters();
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

//for cleasesession filter partial view
window.addEventListener('beforeunload', clearSessionStorage);
function clearSessionStorage() {
    sessionStorage.removeItem('employeeFilters');
}


var dialogObjemployee;

function createdFilter(){
    dialogObjemployee = this;
    dialogObjemployee.hide();
}

function createdFilterDialog() {
    scrollVal = $(window).scrollTop();
    dialogObjemployee.show();
    $(".e-dlg-overlay").remove();
    var spinnerTarget = document.getElementById('filterDialog')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployee/GetFilterPartial', //render the partial view
        type: "POST",
        contentType: "application/json",
    });
    ajax.send().then((data) => {
        $("#filterDialogContent").html(data);

        // Retrieve and apply the filters from session storage
        var storedFilters = sessionStorage.getItem('employeeFilters');
        if (storedFilters) {
            updateFilterView(JSON.parse(storedFilters));
        }
    });

    ej.popups.hideSpinner(spinnerTarget);
}

//function applyFilters(args) {
//    var filters = {
//        HiringFromDate: $("#HiringFromDate").val(),
//        HiringToDate: $("#HiringToDate").val(),
//        TStatusComplete: $("#TStatusComplete").is(':checked'),
//        TStatusPending: $("#TStatusPending").is(':checked'),
//        EStatusDeceased: $("#EStatusDeceased").is(':checked'),
//        EStatusActive: $("#EStatusActive").is(':checked'),
//        EStatusOnTerminated: $("#EStatusOnTerminated").is(':checked'),
//        EStatusResigned: $("#EStatusResigned").is(':checked'),
//        PStatusNotUsed: $("#PStatusNotUsed").is(':checked'),
//        PStatusOptIn: $("#PStatusOptIn").is(':checked'),
//        PStatusOptOut: $("#PStatusOptOut").is(':checked'),
//        MIStatusNotUsed: $("#MIStatusNotUsed").is(':checked'),
//        MIStatusUsed: $("#MIStatusUsed").is(':checked'),
//        DepartmentId: $("#DepartmentId").val()
//    };

//    // Store the filters in session storage
//    sessionStorage.setItem('employeeFilters', JSON.stringify(filters));

//    $.ajax({
//        type: "POST",
//        url: ROOTURL + 'HREmployee/GetEmployeeFilterData',
//        data: filters,
//        success: function (response) {
//            updateGrid(response);
//            updateFilterView(response.filters);
//        },
//        error: function (error) {
//            console.log("Error in fetching filtered data: ", error);
//        }
//    });
//}



function applyFilters(args) {
    var filters = {
        HiringFromDate: $("#HiringFromDate").val(),
        HiringToDate: $("#HiringToDate").val(),
        TStatusComplete: $("#TStatusComplete").is(':checked'),
        TStatusPending: $("#TStatusPending").is(':checked'),
        EStatusDeceased: $("#EStatusDeceased").is(':checked'),
        EStatusActive: $("#EStatusActive").is(':checked'),
        EStatusOnTerminated: $("#EStatusOnTerminated").is(':checked'),
        EStatusResigned: $("#EStatusResigned").is(':checked'),
        PStatusNotUsed: $("#PStatusNotUsed").is(':checked'),
        PStatusOptIn: $("#PStatusOptIn").is(':checked'),
        PStatusOptOut: $("#PStatusOptOut").is(':checked'),
        MIStatusNotUsed: $("#MIStatusNotUsed").is(':checked'),
        MIStatusUsed: $("#MIStatusUsed").is(':checked'),
        DepartmentId: $("#DepartmentId").val()
    };

    // Store the filters in session storage
    sessionStorage.setItem('employeeFilters', JSON.stringify(filters));


    var grid = document.getElementById("HREmployee").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/HREmployee/UrlDatasource?filterss=" + encodeURIComponent(JSON.stringify(filters)),
        adaptor: new ej.data.UrlAdaptor()
    });

    dialogObjemployee.hide();
    var storedFilters = sessionStorage.getItem('employeeFilters');
    if (storedFilters) {
        updateFilterView(JSON.parse(storedFilters));
    }
}

function updateGrid(data) {
    var grid = document.querySelector("#HREmployee").ej2_instances[0];
    grid.dataSource = data;
    grid.refresh();
    dialogObjemployee.hide();
}

function updateFilterView(filters) {
    $("#HiringFromDate").val(filters.HiringFromDate);
    $("#HiringToDate").val(filters.HiringToDate);
    $("#TStatusComplete").prop('checked', filters.TStatusComplete);
    $("#TStatusPending").prop('checked', filters.TStatusPending);
    $("#EStatusDeceased").prop('checked', filters.EStatusDeceased);
    $("#EStatusActive").prop('checked', filters.EStatusActive);
    $("#EStatusOnTerminated").prop('checked', filters.EStatusOnTerminated);
    $("#EStatusResigned").prop('checked', filters.EStatusResigned);
    $("#PStatusNotUsed").prop('checked', filters.PStatusNotUsed);
    $("#PStatusOptIn").prop('checked', filters.PStatusOptIn);
    $("#PStatusOptOut").prop('checked', filters.PStatusOptOut);
    $("#MIStatusNotUsed").prop('checked', filters.MIStatusNotUsed);
    $("#MIStatusUsed").prop('checked', filters.MIStatusUsed);
    $("#DepartmentId").val(filters.DepartmentId).trigger('change');
}

// Define a function to create and show the custom dialog
function showCustomDialog(message) {
    // Create a new Dialog instance
    var dialog = new ej.popups.Dialog({
        header: 'Alert', // Set dialog header
        content: message, // Set dialog content to the message
        width: '300px', // Set dialog width
        animationSettings: { effect: 'Zoom' }, // Set animation effect
        buttons: [{ buttonModel: { content: 'OK', isPrimary: true }, click: dialogButtonClick }], // Add OK button
    });

    // Render the dialog
    dialog.appendTo('#customDialog');
    dialog.show(); // Show the dialog

    var gridContainer = document.getElementById('grddata');
    gridContainer.classList.add('blur-background');
}

function showConfirmationDialog(message, callback) {
    // Create a new Dialog instance
    var dialog = new ej.popups.Dialog({
        header: 'Confirmation', // Set dialog header
        content: message, // Set dialog content to the message
        width: '300px', // Set dialog width
        animationSettings: { effect: 'Zoom' }, // Set animation effect
        buttons: [
            {
                buttonModel: {
                    content: 'OK',
                    isPrimary: true,
                },
                click: function () {
                    if (typeof callback === 'function') {
                        callback(true); // Invoke the callback function with true
                    }
                    dialogButtonClick.call(this); // Hide and destroy the dialog
                }
            },
            {
                buttonModel: {
                    content: 'Cancel'
                },
                click: function () {
                    if (typeof callback === 'function') {
                        callback(false); // Invoke the callback function with false
                    }
                    dialogButtonClick.call(this); // Hide and destroy the dialog
                }
            }
        ] // Add OK and Cancel buttons
    });

    // Render the dialog
    dialog.appendTo('#customDialog');
    dialog.show(); // Show the dialog

    var gridContainer = document.getElementById('grddata');
    gridContainer.classList.add('blur-background');
}

// Define a function to handle button click in the dialog
function dialogButtonClick() {
    this.hide(); // Hide the dialog when OK button is clicked
    this.destroy(); // Destroy the dialog instance
    document.getElementById('customDialog').innerHTML = '';

    var gridContainer = document.getElementById('grddata');
    gridContainer.classList.remove('blur-background');
}

function GetDataForSSN(employeeId) {
    openDialogBoxSSN(employeeId);
}
var dialogObj;
var dialogObjSSN;

function createdSSN() {

    dialogObjSSN = this;
    dialogObjSSN.hide();
}
function openDialogBoxSSN(employeeId) {
    scrollVal = $(window).scrollTop();
    dialogObjSSN.show();
    $(".e-dlg-overlay").remove();
    var spinnerTarget = document.getElementById('ajax_dialogSSN')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/GetValidatePasswordForSSN', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ EmployeeId: employeeId })
    });
    ajax.send().then((data) => {
        $("#ValidateRateSSN").html(data);
    });

    ej.popups.hideSpinner(spinnerTarget);
}
function GetSSNDetail(args) {
    if (args.action != "enter") {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var EmployeeId = document.getElementById('EmployeeId').value;
        var password = document.getElementById('password').value;
        if (password == '') {
            toastObj.content = "Password is required ";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            return false;
        }

        $.ajax({
            url: ROOTURL + 'HREmployeeProfile/GetSSN',
            type: "POST",
            data: {
                "EmployeeId": EmployeeId, "Password": password
            },
            "success": function (data) {
                if (!ej.base.isNullOrUndefined(data.success)) {

                    toastObj.content = data.success;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });

                    $("#SSN").val(data.data);
                }
                if (!ej.base.isNullOrUndefined(data.Error)) {

                    toastObj.content = data.Error;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            }

        });
        event.preventDefault();

        console.log(EmployeeId);
        console.log(password);
    }
}

function SSNpromptBtnClick() {
    dialogObjSSN.hide();
    $(window).scrollTop(scrollVal);
    scrollVal = 0;
}

function statusTrainingDetail(e) {
    debugger;
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.LastSlidename === '.q14Data' || e.LastSlidename === '.q14DataSp') {
        span.textContent = "Completed"
    }
    else{
        span = document.createElement('span');
        span.textContent = "Pending"
    }
    div.appendChild(span);
    return div.outerHTML;
}