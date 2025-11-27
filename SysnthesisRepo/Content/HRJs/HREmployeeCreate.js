var toastObj = document.getElementById('toast_type').ej2_instances[0];
function setSelect2() {
    $(".select2").select2({ closeOnSelect: true });

    if ($("#Status").val() == 0) {
        $("#terminationDateContainer").hide();
    }
    else if ($("#Status").val() == 1) {
        $("#terminationDateContainer").hide();
    }
    else {
        $("#terminationDateContainer").show();
    }
}
$(function () {
    setSelect2();
});


function GetEmployeeUSerName() {
    var LastName = $('#LastName').val();
    if (LastName.toString() == '') {
        toastObj.content = "LastName Blank Not Allowed.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
        return false;
    }
    var FirstName = $('#FirstName').val();
    if (FirstName.toString() == '') {
        toastObj.content = "FirstName Blank Not Allowed.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
        return false;
    }
    var SSNvalue = document.getElementById('SSN').ej2_instances[0].value;
    if (SSNvalue != "") {
        var SSN = document.getElementById('SSN').ej2_instances[0].preEleVal;
        console.log(SSN.length);
        if (SSN.length == 11) {
            if (LastName.length == 1) {
                LastName = LastName.slice(0, 1);
            }
            else {
                LastName = LastName.slice(0, 2);
            }


            if (FirstName.length == 1) {
                FirstName = FirstName.slice(0, 1);
            }
            else {
                FirstName = FirstName.slice(0, 2);
            }

            SSN = SSN.substr(SSN.length - 4);
            var EmpUSerName = FirstName.toLowerCase() + LastName.toLowerCase() + SSN.toLowerCase();
            $('#EmployeeUserName').val(EmpUSerName);
        }
        else {
            if (SSN.toString() == '') {
                toastObj.content = "SSN Blank Not Allowed.";
                toastObj.target = document.body;
                toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
                return false;
            }
            toastObj.content = "Please Enter Valid SSN.";
            toastObj.target = document.body;
            toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
            return false;
        }
    }
    else {
        toastObj.content = "SSN Blank Not Allowed.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
        return false;
    }
}


$('#FrmEmp').submit(function (event) {
    GetEmployeeUSerName();
    var SSNvalue = document.getElementById('SSN').ej2_instances[0].value;
    if (!SSNvalue) {
        // Prevent form submission
        event.preventDefault();

        // Add the input-error class to the MaskedTextBox
        $('#SSN').closest('.e-control-wrapper').addClass('input-error');

        // Show the validation message
        $('.SSNerror').text('SSN is required.').show();
    } else {
        // Remove the input-error class and hide the validation message if SSN is provided
        $('#SSN').closest('.e-control-wrapper').removeClass('input-error');
        $('.SSNerror').hide();
    }
});


function daysdifference(startDay, endDay) {

    var millisBetween = startDay.getTime() - endDay.getTime();
    var days = millisBetween / (1000 * 3600 * 24);

    return Math.round(Math.abs(days));
}


$('#frmID').submit(function (event) {
    var BDate = document.getElementById('DateofBirth').ej2_instances[0].value; 
    var HireDate = document.getElementById('sHireDate').ej2_instances[0].value;

    if (BDate != '') {
        var age =
            daysdifference(HireDate, BDate);
        console.log(age);
        if (parseInt(age) > 5844) {
            console.log(2);
        }
        else {
            toastObj.content = "The Employee must be over 16 years old To HireDate.";
            toastObj.target = document.body;
            toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
            return false;
        }
    }
    
    if ($("#Status").val() != 0 && $("#Status").val() != 'Active') {
        var TerminationDate = $('#sTerminationDate').val();
        if (aa1.test(TerminationDate)) {
            if (new Date($('#sHireDate').val()) > new Date(TerminationDate)) {
                toastObj.content = "Termination Date Should Be Greater than Or Equal To Hire Date.";
                toastObj.target = document.body;
                toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
                //$('#TerminationDate').datepicker('setValue', new Date());
                return false;
            }
        }

    }
});


function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

$("#SSN").on("blur", function () {
    var SSNvalue = document.getElementById('SSN').ej2_instances[0].value;
    if (SSNvalue.length >= 1 && SSNvalue.length < 9) {
        toastObj.content = "Please Enter a Valid SSN.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
    }
});
$("#Phone").on("blur", function () {
    var phone = document.getElementById('Phone').ej2_instances[0].value;
    if (phone.length >= 1 && phone.length < 10) {
        toastObj.content = "Please Enter a Valid Home Phone.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
    }
});
$("#MobileNo").on("blur", function () {
    var Mobile = document.getElementById('MobileNo').ej2_instances[0].value;
    if (Mobile.length >= 1 && Mobile.length < 10) {
        toastObj.content = "Please Enter a Valid Mobile.";
        toastObj.target = document.body;
        toastObj.show({ title: 'Information!', cssClass: 'e-toast-info', icon: 'e-info toast-icons' });
    }
});


