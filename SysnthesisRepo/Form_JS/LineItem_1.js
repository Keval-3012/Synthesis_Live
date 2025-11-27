function updatestatus(iii, obj) {

    $.ajax({
        url: ROOTURL + 'Invoices/UpdateStatus',
        type: "POST",
        data: JSON.stringify({ id: iii, value: obj.checked }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            if (response.message == "success") {
                if (obj.checked == true) {
                    toastr.success(response.approvermsg);
                }
                else {
                    toastr.success(response.approvermsg);
                }

            }
            else {
                toastObj.content = "Something went wrong!";
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }

        },
        error: function (response) {
        }
    });

}

function approveAllStatus(iii) {

    var result = confirm("Are you sure Want to approve all status?");
    if (result) {
        $.ajax({
            url: ROOTURL + 'Invoices/ApproveAllStatus',
            type: "POST",
            data: JSON.stringify({ id: iii }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                if (response == "success") {

                    var grid = document.getElementById("InlineEditingValue").ej2_instances[0];
                    grid.dataSource = new ej.data.DataManager({
                        url: "/LineItem/UrlDatasourceValue",
                        adaptor: new ej.data.UrlAdaptor()
                    });
                    toastr.success(ApproveAll);
                }
                else {
                    toastObj.content = "Something went wrong!";
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            },
            error: function (response) {
            }
        });
    }

}

