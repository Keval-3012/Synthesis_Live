function createFormGroup(labelText, textContent) {
    // Create the form group div
    var formGroup = document.createElement('div');
    formGroup.classList.add('form-g');

    // Create the label element
    var label = document.createElement('label');
    label.classList.add('col-form-label');
    label.textContent = labelText;

    // Create the form text element
    var formText = document.createElement('div');
    formText.classList.add('form-text');
    formText.textContent = textContent;

    // Append label and formText to formGroup
    formGroup.appendChild(label);
    formGroup.appendChild(formText);

    // Return the created form group
    return formGroup;
}
function GetStorWiseData() {
    var storeId = $('#StoreId').val();
    var employeeId = $('#EmployeeId').val();
    $.ajax({
        url: '/HREmployeeProfile/GetStoerWiseDetail',
        type: "get",
        data: { StoreId: storeId, EmployeeId: employeeId },
        success: function (data) {
            if (data != null) {
               
                document.getElementById('EmployeeChildDetail').innerHTML = '';
                var container = document.getElementById('EmployeeChildDetail');
                // Call the createFormGroup function for each data
                var employeeIdFormGroup = createFormGroup('Employee ID', data.OfficeEmployeeID);
                var hireDateFormGroup = createFormGroup('Hire date', data.sHireDate);
                var employmentTypeFormGroup = createFormGroup('Employment Type', data.EmployeementTypeStatusName);
                var storeFormGroup = createFormGroup('Store', data.StoreName);

                // Append the form groups to the container
                container.appendChild(employeeIdFormGroup);
                container.appendChild(hireDateFormGroup);
                container.appendChild(employmentTypeFormGroup);
                container.appendChild(storeFormGroup);
                EmployeeChildId = data.EmployeeChildId;
                $('#EmployeeChildId').val(data.EmployeeChildId);
                GetTabingView()
            }
        }
    })
}

function GetTabingView(){
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/BindTabingView', //render the partial view
        type: "POST",
    });
    ajax.send().then(function (data) {
        $("#ProfileTabing").html(data);
        BindtheDocument();
        BindtheWarning();
        BindtheTermination();
        console.log(data);
    }).catch(function (xhr) {
        console.log(xhr);
    });
}
