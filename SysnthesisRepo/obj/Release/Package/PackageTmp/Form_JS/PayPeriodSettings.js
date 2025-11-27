$('#chkemail').change(function () {
    var a = $(this).is(':checked');
    var toastObj = document.getElementById('toast_type').ej2_instances[0];

    if (confirm("Are you sure?") == true) {
        var ajax = new ej.base.Ajax({
            url: ROOTURL + "/ManageIPAddress/IsMailSendAllow",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ value: a })
        });
        ajax.send().then(function (data) {

            if (data == "\"Success\"") {
                toastObj.content = '@ViewBag.EmailSuccess';
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (data == "\"Error\"") {
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
        });
    }
});
