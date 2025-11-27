
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

function actionComplete(args) {
    //var grid = document.getElementById("InlineEditing").ej2_instances[0];

    if (args.requestType === 'beginEdit' || args.requestType === 'add') {
        let spinner = ej.popups.createSpinner({ target: args.dialog.element });
        ej.popups.showSpinner(args.dialog.element);
        if (args.requestType === 'beginEdit') {
            args.dialog.header = 'Edit';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'UserMasters/Editpartial', //render the partial view
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
            args.dialog.header = 'Add User';
            var ajax = new ej.base.Ajax({
                url: ROOTURL + 'UserMasters/Addpartial', //render the partial view
                type: "POST",
                contentType: "application/json",
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
        //var grid6 = document.getElementById("InlineEditing").ej2_instances[0];
        //$(window).scrollTop(scrollVal);
        //scrollVal = 0;
        var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
        grid.refresh();
    }

    if (args.requestType === 'beginEdit' && args.rowData.Status === 'Inactive') {

        //alert("If you want to any change in this vendor then you must have to active first, otherwise the change will not take place in QuickBooks.");
    }

}

function appendElement(elementString, form) {

    form.querySelector("#dialogTemp").innerHTML = elementString;
    form.ej2_instances[0].addRules('FirstName', { required: [true, "Name is required"] });
    form.ej2_instances[0].addRules('UserName', { required: [true, "User Name is required"] });
    //form.ej2_instances[0].addRules('Password', { required: [true, "Password is required"] });
    //form.ej2_instances[0].addRules('ConfirmPassword', { required: [true, "Confirm Password is required"] });
    form.ej2_instances[0].addRules('GroupId', { required: [true, "Group is required"] });
    form.ej2_instances[0].addRules('UserTypeId', { required: [true, "User Role is required"] });
    form.querySelector("#dialogTemp").style.width = "500px";
    //form.ej2_instances[0].addRules('CustomerID', {required: true, minLength: 3 }); //adding the form validation rules
    form.ej2_instances[0].refresh();  // refresh method of the formObj
    var script = document.createElement('script');
    script.type = "text/javascript";
    var serverScript = form.querySelector("#dialogTemp").querySelector('script');
    script.textContent = serverScript.innerHTML;
    document.head.appendChild(script);
    serverScript.remove();
}

function refreshGrid() {
    var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
    if (grid) {
        // Get current GroupId from dropdown
        var groupDropdown = document.getElementById("GroupFilter");
        var currentGroupId = 0;
        if (groupDropdown && groupDropdown.ej2_instances && groupDropdown.ej2_instances[0]) {
            currentGroupId = groupDropdown.ej2_instances[0].value || 0;
        }

        // Update URL with current GroupId before refresh
        grid.dataSource.dataSource.url = "/UserMasters/UrlDatasource?GroupId=" + currentGroupId;
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
    var grid = document.querySelector('#ManageUserGrid').ej2_instances[0];

    // Get default GroupId (All Groups = 0)
    var defaultGroupId = 0;
    var groupDropdown = document.getElementById("GroupFilter");
    if (groupDropdown && groupDropdown.ej2_instances && groupDropdown.ej2_instances[0]) {
        defaultGroupId = groupDropdown.ej2_instances[0].value || 0;
    }
    grid.dataSource = new ej.data.DataManager({
        url: "/UserMasters/UrlDatasource?GroupId=" + defaultGroupId, 
        insertUrl: "/UserMasters/InsertUserManager",
        updateUrl: "/UserMasters/UpdateUserManager",
        adaptor: new CustomAdaptor()
    });
};

function getUserTypeData() {
    var GroupId = $('#GroupId').val();
    if (GroupId == "") {
        GroupId = "0";
    }
    $.ajax({
        url: '/UserMasters/getUserTypeData',
        type: "GET",
        dataType: "JSON",
        data: { GroupId: GroupId },
        success: function (states) {
            $("#UserTypeId").html("");
            $('select#UserTypeId').html('<option value="">--Select--</option>');
            $.each(states, function (data, value) {
                $("#UserTypeId").append(
                    $('<option></option>').val(value.UserTypeId).html(value.UserType));
            });
        }
    });
}
$("body").on('click', '.toggle-password', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") === "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});

function statusTrainingDetail(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.IsActive === true) {
        span.textContent = "Active"
    }
    else {
        span.textContent = "Inactive"
    }
    div.appendChild(span);
    return div.outerHTML;
}

function trackstatusTrainingDetail(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.TrackHours === true) {
        span.textContent = "Active"
    }
    else {
        span.textContent = "Inactive"
    }
    div.appendChild(span);
    return div.outerHTML;
}

function PasswordMatchCheck() {
    let password = $('#Password').val();
    let confirmPassword = $('#ConfirmPassword').val();
    let passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;

    if (!passwordPattern.test(password)) {
        $('#password-regexs').show();
        $('#password-regexs').html('Required at least 8 characters, 1 capital letter, 1 special character and 1 number without spaces.').css('color', 'red');
    } else {
        if (password !== confirmPassword && confirmPassword != "") {
            $('#password-errors').show();
            $('#password-regexs').hide();
            $('#password-errors').html('Passwords do not match.').css('color', 'red');
        } else if (password == confirmPassword && password != "" && confirmPassword != "") {
            $('#password-errors').show();
            $('#password-regexs').hide();
            $('#password-errors').html('Matching').css('color', 'green');
        }
        else {
            $('#password-errors').hide();
            $('#password-regexs').hide();
        }
    }

}

function ResetPasswordMatchCheck() {
    let password = $('#NewPassword').val();
    let confirmPassword = $('#NewConfirmPassword').val();
    let passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;

    if (!passwordPattern.test(password)) {
        $('#password-regex').show();
        $('#password-regex').html('Required at least 8 characters, 1 capital letter, 1 special character and 1 number without spaces.').css('color', 'red');
    } else {
        if (password !== confirmPassword && confirmPassword != "") {
            $('#password-error').show();
            $('#password-regex').hide();
            $('#password-error').html('Passwords do not match.').css('color', 'red');
        } else if (password == confirmPassword && password != "" && confirmPassword != "") {
            $('#password-error').show();
            $('#password-regex').hide();
            $('#password-error').html('Matching').css('color', 'green');
        }
        else {
            $('#password-error').hide();
            $('#password-regex').hide();
        }
    }

}

function toggle(button) {
    var List = button.id.split("^");
    switch (button.value) {
        case "on":
            button.value = "off";
            UserId = List[0];
            IsActive = false;
            var text = "Are you sure you want to deactivate: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'UserMasters/UpdateStatusUserManager',
                    type: "POST",
                    data: JSON.stringify({ UserId: UserId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                grid.refresh();
            }
            break;
        case "off":
            button.value = "on";
            UserId = List[0];
            IsActive = true;
            var text = "Are you sure you want to activate: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'UserMasters/UpdateStatusUserManager',
                    type: "POST",
                    data: JSON.stringify({ UserId: UserId, IsActive: IsActive }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                grid.refresh();
            }
            break;
    }
}

function tracktoggle(button) {
    var List = button.id.split("^");
    switch (button.value) {
        case "on":
            button.value = "off";
            UserId = List[0];
            TrackHours = false;
            var text = "Are you sure you want to deactivate track hours: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'UserMasters/UpdateTrackStatusUserManager',
                    type: "POST",
                    data: JSON.stringify({ UserId: UserId, TrackHours: TrackHours }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Track Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                grid.refresh();
            }
            break;
        case "off":
            button.value = "on";
            UserId = List[0];
            TrackHours = true;
            var text = "Are you sure you want to activate  track hours: " + List[1] + " ?";
            if (confirm(text) == true) {
                $.ajax({
                    url: ROOTURL + 'UserMasters/UpdateTrackStatusUserManager',
                    type: "POST",
                    data: JSON.stringify({ UserId: UserId, TrackHours: TrackHours }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var toastObj = document.getElementById('toast_type').ej2_instances[0];
                        if (response == "Success") {
                            var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                            grid.refresh();
                            toastObj.content = "Track Status Successfully Updated.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        }
                        else {
                            toastObj.content = "Some thing went to wrong.";
                            toastObj.target = document.body;
                            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                        }
                    },
                    error: function (response) {
                    }
                });
            }
            else {
                var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                grid.refresh();
            }
            break;
    }
}

function toolbarClick(args) {
    if (args.item.id === "ManageUserGrid_Reset Password") {
        var selectedRecords = this.getSelectedRecords();
        if (selectedRecords.length === 0) {
            //showCustomDialog("No records selected for Reset Password operation");
        }
        else {
            var selectedPrimaryKey = selectedRecords[0]["UserId"];

            var modalElement = document.getElementById('resetPasswordModal');
            modalElement.style.display = 'block';
            var dialogObj = new ej.popups.Dialog({
                width: '300px'
            });
            $("#HiddenUserId").val(selectedPrimaryKey);
            dialogObj.appendTo('#resetPasswordModal');
        }
    }
}

function ResetPasswordcnf() {
    var UserId = $("#HiddenUserId").val();
    var Password = $("#NewPassword").val();
    var ConfirmPassword = $("#NewConfirmPassword").val();
    let passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;
    if (passwordPattern.test(Password)) {
        if (Password == ConfirmPassword) {
            $.ajax({
                url: ROOTURL + 'UserMasters/UserManagerResetPassowrd',
                type: "POST",
                data: JSON.stringify({ UserId: UserId, Password: Password }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var toastObj = document.getElementById('toast_type').ej2_instances[0];
                    if (response == "Success") {
                        var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
                        $('#resetPasswordModal').css('display', 'none');
                        grid.refresh();
                        toastObj.content = "Password Reset Successfully Updated.";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    }
                    else {
                        $('#resetPasswordModal').css('display', 'none');
                        toastObj.content = "Some thing went to wrong.";
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }
                },
                error: function (response) {
                }
            });
        } else {
            var toastObj = document.getElementById('toast_type').ej2_instances[0];
            $('#resetPasswordModal').css('display', 'none');
            toastObj.content = "Password and confirm password not matched...";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
        }
    }
    else {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        $('#resetPasswordModal').css('display', 'none');
        toastObj.content = "Required at least 8 characters,1 capital and 1 special letter,1 number";
        toastObj.target = document.body;
        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
    }

}

function cancelResetPassword() {
    $('#resetPasswordModal').css('display', 'none');
}

function ProductscanOnchange() {    
    $("#divStoreAccess").show();
   

    var dropdownobj = document.getElementById("MuiltiStoreAccess").ej2_instances[0];
    var MulitStoreData = document.getElementById("MuiltiStoreAccessData").ej2_instances[0]; 
    var UserRightstoreaccess = document.getElementById("UserRightsforStoreAccess").ej2_instances[0];
    if ($('#ProductScanApp input[type="checkbox"]').is(':unchecked')) { 
        $("#divlocationstoreaccess").hide();
        $("#divdatastoreaccess").hide();
        dropdownobj.enabled = false;
        dropdownobj.value = [];
        dropdownobj.dataBind();
        MulitStoreData.enabled = false;
        MulitStoreData.value = [];
        MulitStoreData.dataBind();
        UserRightstoreaccess.enabled = true;
    }
    else
    {
        $("#divStoreAccess").hide();
        $("#divlocationstoreaccess").hide();
        $("#divdatastoreaccess").hide();
        UserRightstoreaccess.enabled = false; 
        UserRightstoreaccess.value = '0';
        UserRightstoreaccess.dataBind();
        dropdownobj.enabled = false;
        dropdownobj.value = [];
        dropdownobj.dataBind();
        MulitStoreData.enabled = false;
        MulitStoreData.value = [];
        MulitStoreData.dataBind();
    }
}

function userrightonchange() {    
    $("#divlocationstoreaccess").show();
    $("#divdatastoreaccess").show();
    var dropdownobj = document.getElementById("MuiltiStoreAccess").ej2_instances[0]; 
    var MulitStoreData = document.getElementById("MuiltiStoreAccessData").ej2_instances[0]; 
    var UserRightstoreaccess = document.getElementById("UserRightsforStoreAccess").ej2_instances[0];
    if (UserRightstoreaccess.value == '2') {
        $("#divlocationstoreaccess").show();
        $("#divdatastoreaccess").show();
        dropdownobj.enabled = true;
        MulitStoreData.enabled = true;
    } else {
        $("#divlocationstoreaccess").hide();
        $("#divdatastoreaccess").show();
        dropdownobj.enabled = false;
        dropdownobj.value = [];
        dropdownobj.dataBind();
        MulitStoreData.enabled = true;       
    }
};

function CustomCompetitorsOnchange() {
    var customCheckbox = document.getElementById("IsCustomCompetitors").ej2_instances[0];

    if (customCheckbox.checked) {
        $("#customCompetitorsDiv").show();
        $("#designatedStoreDiv").hide();
    } else {
        $("#customCompetitorsDiv").hide();
        $("#designatedStoreDiv").show();
    }
}

function onGroupFilterChange(args) {
    var grid = document.getElementById("ManageUserGrid").ej2_instances[0];
    var selectedGroupId = args.value || 0;

    // Update the grid's data source URL with the GroupId parameter
    grid.dataSource.dataSource.url = "/UserMasters/UrlDatasource?GroupId=" + selectedGroupId;

    // Refresh the grid to fetch new data
    grid.refresh();
}