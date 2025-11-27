function actionBegin(args) {
    var storeid = Expensestoreid;

    var requiredRoles = ["Administrator", "AddExpenseExpenseCheck", "AddCheckExpenseCheck"];
    // Check if the user has at least one required role
    var hasRole = userRoles.some(role => requiredRoles.includes(role));
    var isAdmin = userRoles.includes("Administrator");

    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();

        if (!isAdmin) {
            if (storeid === "0" || storeid === "") {
                $('#popupStoreAlert').show();
                args.cancel = true;
            }
        }
    }


    if (args.requestType == "add") {
        if (!hasRole) {
            // Redirect to the Error.cshtml page if the user lacks the required roles
            window.location.href = '/AdminInclude/Error';
            args.cancel = true;
            return;
        }
        if (storeid === "0" || storeid === "") {
            $('#popupStoreAlert').show();
            args.cancel = true;
        }
    }
    if (args.requestType == "save") {
        var editedData = args.data;
        var errorElement = document.getElementById("totalamt-error");
        if (errorElement) {
            errorElement.remove();
        }
        if (editedData.TotalAmt < 0) {
            var errorMessage = document.createElement("div");
            errorMessage.id = "totalamt-error";
            errorMessage.style.color = "red";
            errorMessage.style.marginTop = "10px";
            errorMessage.textContent = "Enter a Transaction amount that is 0 or greater.";

            // Append the error message below the "Amount" section
            var amountSection = document.querySelector(".amount-sec");
            amountSection.appendChild(errorMessage);
            args.cancel = true;
        }
        // Get all rows in the Department Breakdown section
        var departmentRows = document.querySelectorAll("#department-rows .department-row");
        var isValid = true;
        var firstInvalidElement = null;

        // Loop through each row and validate inputs
        departmentRows.forEach(function (row, index) {
            var departmentDropdown = row.querySelector(".department-select");
            var amountInput = row.querySelector(".amount-input");
            var errorMessage;

            departmentDropdown.style.border = "";
            amountInput.style.border = "";
            var existingError = row.querySelector(".validation-error");
            if (existingError) {
                existingError.remove();
            }

            // Validate Department Dropdown
            if (!departmentDropdown.value || departmentDropdown.value.trim() === "") {
                errorMessage = "Please select a department.";
                departmentDropdown.style.border = "2px solid red";
                if (!firstInvalidElement) firstInvalidElement = departmentDropdown;
                isValid = false;
            }

            // Validate Amount Input
            if (!amountInput.value || amountInput.value.trim() === "") {
                errorMessage = "Please enter an amount.";
                amountInput.style.border = "2px solid red";
                if (!firstInvalidElement) firstInvalidElement = amountInput;
                isValid = false;
            }

            // If there's an error, display it below the row
            //if (errorMessage) {
            //    var errorDiv = document.createElement("div");
            //    errorDiv.className = "validation-error";
            //    errorDiv.style.color = "red";
            //    errorDiv.style.marginTop = "5px";
            //    errorDiv.textContent = errorMessage;
            //    row.appendChild(errorDiv);
            //}
        });

        if (firstInvalidElement) {
            firstInvalidElement.focus();
        }

        if (!isValid) {
            args.cancel = true;
        }
    }

    //var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
}
function actionComplete(args) {
    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit Expense/Check';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'ExpenseAccounts/Editpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {

                appendElement(data, args.form); //render the edit form with selected record
                ej.popups.hideSpinner(args.dialog.element);
            }).catch(function (xhr) {
                console.log(xhr);
                ej.popups.hideSpinner(args.dialog.element);
            });
        }
        if (args.requestType === 'add') {
            args.dialog.header = 'Add Expense/Check';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'ExpenseAccounts/Addpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({ value: args.rowData })
            });
            ajax.send().then(function (data) {
                appendElement(data, args.form); //Render the edit form with selected record
                //args.form.elements.namedItem('FirstName').focus();
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
        var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
        grid.refresh();
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

        //alert("If you want to any change in this vendor then you must have to active first, otherwise the change will not take place in QuickBooks.");
    }

}

function appendElement(elementString, form) {
    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('VendorIdstr', {
        required: [true, "Payee is required"],
        custom: [function (args) {
            if (args.value === '0') {
                return false;
            }
            return true;
        }, "Payee is required"]
    });
    form.ej2_instances[0].addRules('PaymentTypeId', {
        required: [true, "PaymentType is required"],
        custom: [function (args) {
            if (args.value === '0') {
                return false;
            }
            return true;
        }, "Payment Type is required"]
    });
    form.ej2_instances[0].addRules('BankAccountId', {
        required: [true, "Payment Account is required"],
        custom: [function (args) {
            if (args.value === '0') {
                return false;
            }
            return true;
        }, "Payment Account is required"]
    });
    form.ej2_instances[0].addRules('TxnDate', { required: [true, "Payment Date is required"] });
    //form.ej2_instances[0].addRules('PaymentTypeId', { required: [true, "Payment Method is required"] });
    //form.ej2_instances[0].addRules('DocNumber', { required: [true, "Ref no. is required"] });
    //form.ej2_instances[0].addRules('UploadFiles', { required: [true, "File is required"] });
    //form.ej2_instances[0].addRules('Memo', { required: [true, "Note is required"] });
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);

    //for paymenttype check and hide/show
    var paymentMethodValue = $("#PaymentTypeId").val();
    if (paymentMethodValue === "Check") {
        // Define the required roles for the "add" action
        var requiredRoles = ["Administrator", "UpdateCheckExpenseCheck"];
        // Check if the user has at least one required role
        var hasRole = userRoles.some(role => requiredRoles.includes(role));
        if (!hasRole) {
            // Redirect to the Error.cshtml page if the user lacks the required roles
            window.location.href = '/AdminInclude/Error';
            args.cancel = true;
            return;
        }

        $(".date-check-sec").addClass("showcheckcls");
        $(".mailing-address").show();
        $(".form-check").show();
        $(".refcheckno").text('Check No.');
        $(".payment-method").hide();
        $(".payacctlbl").text('Bank Account');

        //enable/disable ref/check no
        const printLaterCheckbox = document.querySelector('input[name="PrintLater"]');
        toggleDocNumber(printLaterCheckbox.checked);


        var storeid = $("#StoreId").val();

        $.ajax({
            url: ROOTURL + "ExpenseAccounts/GetExpenseCheckPaymentAccount",
            type: 'POST',
            data: { paytype: 1, editstoreid: storeid },
            success: function (response) {
                if (response && Array.isArray(response)) {
                    var dropdownObj = document.getElementById('BankAccountId').ej2_instances[0];
                    dropdownObj.dataSource = response;
                    dropdownObj.dataBind();
                } else {
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
    else if (paymentMethodValue === "Expense") {
        // Define the required roles for the "add" action
        var requiredRoles = ["Administrator", "UpdateExpenseExpenseCheck"];
        // Check if the user has at least one required role
        var hasRole = userRoles.some(role => requiredRoles.includes(role));
        if (!hasRole) {
            // Redirect to the Error.cshtml page if the user lacks the required roles
            window.location.href = '/AdminInclude/Error';
            args.cancel = true;
            return;
        }
    }
    serverScript.remove();
}
function refreshGrid() {
    var grid = document.getElementById("InlineExpenseEditing").ej2_instances[0];
    if (grid) {
        grid.refresh();
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
    var grid = document.querySelector('#InlineExpenseEditing').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/ExpenseAccounts/UrlDatasourceExpense",
        insertUrl: "/ExpenseAccounts/InsertExpenseCheck",
        updateUrl: "/ExpenseAccounts/UpdateExpenseCheck",
        removeUrl: "/ExpenseAccounts/RemoveExpenseCheck",
        adaptor: new CustomAdaptor()
    });
};




//let rowIndex = 0;

//$(document).on('click', '.add-row', function () {
//    rowIndex++;
//    const newRow = $('#row-template').clone();
//    newRow.find('.id-input').attr('name', `ExpenseCheckDetails[${rowIndex}].ExpenseCheckDetailId`).val('');
//    newRow.find('.department-select').attr('name', `ExpenseCheckDetails[${rowIndex}].DepartmentId`).val('');
//    newRow.find('.description-input').attr('name', `ExpenseCheckDetails[${rowIndex}].Description`).val('');
//    newRow.find('.amount-input').attr('name', `ExpenseCheckDetails[${rowIndex}].Amount`).val('');
//    $('#department-rows').append(newRow);
//    updateTotalAmount();
//});

//$(document).on('click', '.remove-row', function () {
//    if ($('.department-row').length > 1) {
//        $(this).closest('.department-row').remove();
//        updateTotalAmount();
//    }
//});

$(document).on('click', '.add-row', function () {
    const rowIndex = $('#department-rows .department-row').length;

    const newRow = $('#row-template').clone();
    newRow.find('.id-input').attr('name', `ExpenseCheckDetails[${rowIndex}].ExpenseCheckDetailId`).val('');
    newRow.find('.department-select').attr('name', `ExpenseCheckDetails[${rowIndex}].DepartmentId`).val('').css('border', '');
    newRow.find('.description-input').attr('name', `ExpenseCheckDetails[${rowIndex}].Description`).val('');
    newRow.find('.amount-input').attr('name', `ExpenseCheckDetails[${rowIndex}].Amount`).val('').css('border', '');

    $('#department-rows').append(newRow);
    updateTotalAmount();
});

$(document).on('click', '.remove-row', function () {
    if ($('.department-row').length > 1) {
        $(this).closest('.department-row').remove();
        updateRowIndices();
        updateTotalAmount();
    }
});

function updateRowIndices() {
    $('#department-rows .department-row').each(function (index) {
        $(this).find('.id-input').attr('name', `ExpenseCheckDetails[${index}].ExpenseCheckDetailId`);
        $(this).find('.department-select').attr('name', `ExpenseCheckDetails[${index}].DepartmentId`);
        $(this).find('.description-input').attr('name', `ExpenseCheckDetails[${index}].Description`);
        $(this).find('.amount-input').attr('name', `ExpenseCheckDetails[${index}].Amount`);
    });
}


function updateTotalAmount() {
    let totalAmount = 0;
    $('.amount-input').each(function () {
        let value = parseFloat($(this).val());
        if (!isNaN(value)) {
            totalAmount += value;
        }
    });

    let formattedAmount = totalAmount.toFixed(2);
    if (formattedAmount < 0) {
        formattedAmount = formattedAmount.replace('-', '');
        formattedAmount = '-$' + formattedAmount;
    } else {
        formattedAmount = '$' + formattedAmount;
    }

    $('.amount').text(formattedAmount);

    $('input[name="TotalAmt"]').val(totalAmount);
}


$(document).on('input', '.amount-input', function () {
    if ($(this).val().trim() !== "") {
        $(this).css("border", "");
    }
    updateTotalAmount();
});

$(document).on('blur', '.amount-input', function () {
    if (this.value) {
        const value = parseFloat(this.value).toFixed(2);
        this.value = value;
    }
});

$(document).on('change', '.department-select', function () {
    if ($(this).val().trim() !== "") {
        $(this).css("border", "");
    }
});

function onImageUploadSuccess(args) {
    var uploadedFileName = args.e.currentTarget.getResponseHeader('name');
    if (uploadedFileName != null) {
        var currentFileNames = $("#FileName").val();
        if (currentFileNames) {
            $("#FileName").val(currentFileNames + ',' + uploadedFileName);
        } else {
            $("#FileName").val(uploadedFileName);
        }
    }
}

function OnPayeeChange(args) {
    var payeeid = args.value;
    $.ajax({
        url: ROOTURL + "ExpenseAccounts/GetPayeeMailingAddress",
        type: 'POST',
        data: { payeeidstr: payeeid },
        success: function (response) {
            if (response.mailingaddress != "") {
                var mailingadrs = response.mailingaddress;
                $("#MailingAddress").val(mailingadrs);
            }
            else {
                $("#MailingAddress").val("");
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

function OnPaymentType(args) {
    if (args.value == 1) {


        // Define the required roles for the "add" action
        var requiredRoles = ["Administrator", "AddCheckExpenseCheck"];
        // Check if the user has at least one required role
        var hasRole = userRoles.some(role => requiredRoles.includes(role));
        if (!hasRole) {
            // Redirect to the Error.cshtml page if the user lacks the required roles
            window.location.href = '/AdminInclude/Error';
            args.cancel = true;
            return;
        }

        $(".date-check-sec").addClass("showcheckcls");
        $(".mailing-address").show();
        $(".form-check").show();
        $(".refcheckno").text('Check No.');
        $(".payment-method").hide();
        $(".payacctlbl").text('Bank Account');

        //enable/disable ref/check no
        const printLaterCheckbox = document.querySelector('input[name="PrintLater"]');
        toggleDocNumber(printLaterCheckbox.checked);


        const storeid = parseInt($("#StoreId").val() || 0, 10);
        $.ajax({
            url: ROOTURL + "ExpenseAccounts/GetExpenseCheckPaymentAccount",
            type: 'POST',
            data: { paytype: 1, editstoreid: storeid },
            success: function (response) {
                if (response && Array.isArray(response)) {
                    var dropdownObj = document.getElementById('BankAccountId').ej2_instances[0];
                    dropdownObj.dataSource = response;
                    dropdownObj.dataBind();
                } else {
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
    else {


        // Define the required roles for the "add" action
        var requiredRoles = ["Administrator", "AddExpenseExpenseCheck"];
        // Check if the user has at least one required role
        var hasRole = userRoles.some(role => requiredRoles.includes(role));
        if (!hasRole) {
            // Redirect to the Error.cshtml page if the user lacks the required roles
            window.location.href = '/AdminInclude/Error';
            args.cancel = true;
            return;
        }


        $(".date-check-sec").removeClass("showcheckcls");
        $(".mailing-address").hide();
        $(".form-check").hide();
        $(".refcheckno").text('Ref No.');
        $(".payment-method").show();
        $(".payacctlbl").text('Payment Account');

        //remove refno disabled
        const docNumberInput = document.getElementById("DocNumber");
        docNumberInput.disabled = false;

        const storeid = parseInt($("#StoreId").val() || 0, 10);
        $.ajax({
            url: ROOTURL + "ExpenseAccounts/GetExpenseCheckPaymentAccount",
            type: 'POST',
            data: { paytype: 2, editstoreid: storeid },
            success: function (response) {
                if (response && Array.isArray(response)) {
                    var dropdownObj = document.getElementById('BankAccountId').ej2_instances[0];
                    dropdownObj.dataSource = response;
                    dropdownObj.dataBind();
                } else {
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
}

$(document).on('click', '#PrintLater', function () {
    const printLaterCheckbox = document.querySelector('input[name="PrintLater"]');
    if (printLaterCheckbox.checked) {
        printLaterCheckbox.checked = false;
    } else {
        printLaterCheckbox.checked = true;
    }
    toggleDocNumber(printLaterCheckbox.checked);
});



function toggleDocNumber(isChecked) {
    const docNumberInput = document.getElementById("DocNumber");
    if (isChecked) {
        docNumberInput.disabled = true;
        docNumberInput.value = "";
    } else {
        docNumberInput.disabled = false;
    }
}

function statusQbStatusDetail(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.IsSync === true) {
        span.textContent = "Synced"
    }
    else {
        span = document.createElement('span');
        span.textContent = "No Sync"
    }
    div.appendChild(span);
    return div.outerHTML;
}