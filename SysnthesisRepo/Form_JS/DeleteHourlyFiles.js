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

$(document).ready(function () {
    var startdate = Startdate;
    var currentDate = new Date();
    var formattedDate = moment(currentDate).format('MM-DD-YYYY');


    $('#txtstartdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true,
        maxDate: currentDate
    });
    $('#txtstartdate').val(startdate);
    FunSearchRecord()
    $('#txtstartdate').attr('autocomplete', 'off');


});

$(function () {
    var currentDate = new Date();
    $('#txtstartdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true,
        maxDate: currentDate
    });
    $('#txtstartdate').on('dp.change', function (e) {
        var date = $(this).val();
        var dateParts = date.split('-');
        var formattedDate = dateParts[0] + '-' + dateParts[1] + '-' + dateParts[2];
        //FunSearchRecord(date)
        //$(".other-deposite").hide();
        var grid = document.getElementById("HourlyPOSFeedsGrid").ej2_instances[0];
        grid.dataSource = new ej.data.DataManager({
            url: "/DeleteHourlyFiles/UrlDatasource?startdate=" + encodeURIComponent(formattedDate),
            adaptor: new ej.data.UrlAdaptor()
        });
    });
});

function FunSearchRecord(date)//Search
{
    window.FunSearchRecord = FunSearchRecord
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    //GetData(element_txtstartdate);
}

function GetData(startdate_val) {
    var startdate = startdate_val.replace(" ", "_");

    $.ajax({
        url: ROOTURL + '/DeleteHourlyFiles/Grid',
        post: 'GET',
        data: { startdate: startdate },
        beforeSend: function () { Loader(1); },
        success: function (response) {

            $('#grddata').empty();
            $('#grddata').append(response);
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();

}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + 'DeleteHourlyFiles/Delete',
        data: { id: ID },
        async: false,
        success: function (response) {
            if (response === "Hourly File Deleted Successfully..") {
                MyCustomToster(response);
                FunSearchRecord()
                $(".divIDClass").hide();
            }
            else if (response != "") {
                alert(response);
                $(".divIDClass").hide();
            }
        },
        error: function () {
            Loader(0);
        }
    });
}

        //syncfusion code start from here
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


function refreshGrid() {
    var grid = document.getElementById("HourlyPOSFeedsGrid").ej2_instances[0];
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
    var grid = document.querySelector('#HourlyPOSFeedsGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/DeleteHourlyFiles/UrlDatasource",
        adaptor: new CustomAdaptor()
    });
};

function ComfirmDeleteej2(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
}

function Deleteej2(ID) {
    var startdate = $("#txtstartdate").val();
    var dateParts = startdate.split('-');
    var formattedDate = dateParts[0] + '-' + dateParts[1] + '-' + dateParts[2]; 
    $.ajax({
        url: '/DeleteHourlyFiles/Delete',
        data: { id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("HourlyPOSFeedsGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/DeleteHourlyFiles/UrlDatasource?startdate=" + encodeURIComponent(formattedDate),
                adaptor: new ej.data.UrlAdaptor()
            });
            //return true;
            toastr.success('Hourly File Deleted Successfully..');
        },
        error: function () {
            Loader(0);
        }
    });
}