function ErrorPopup(ID) {
    var DivId = ID;
    $("#" + DivId).show();
    $(".overlaytransparent").show();
}

function CheckExistence(value) {
    var files = $("#file-input").get(0).files;
    var formData = new FormData();
    formData.append('postedFile', files[0]);
    var Redio = $('input[name="FileFormat"]:checked').val();
    formData.append('Redio', Redio);
    $.ajax({
        url: ROOTURL + '/PDFRead/CheckExistence',
        data: formData,
        type: 'POST',
        contentType: false,
        processData: false,
        success: function (data) {
            if (data == 1) {
                event.preventDefault();
                $("#popupcheckExistsagain").show();
                $('#FormData').submit(false);
                return false;
            }
            else {
                document.getElementById('FormData').submit();
            }
        },
        error: function (ex) {
        }
    });
}
function Save() {
    $("#popupcheckExistsagain").hide();
    document.getElementById('FormData').submit();
}
function SubDuplicatepopupclose() {
    $("#popupcheckExistsagain").hide();
}
function Delete(ID) {

    $('.modal-backdrop').hide();
    $.ajax({
        url: ROOTURL + '/PDFRead/delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            window.location.reload();
            return true;
        },
        error: function () {
            return false;
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
}

$(document).ready(function () {
    $(".myval").select2({
        allowclear: true,
    });
    if (document.getElementById('SelectRecords') != null) {
        document.getElementById('SelectRecords').value = PageSize;
    }
    $("select").select2({
        width: "100%",
        formatResult: function (state) {
            if (!state.id) return state.text;
            if ($(state.element).data('active') == "0") {
                return state.text + "<i class='fa fa-dot-circle-o'></i>";
            } else {
                return state.text;
            }
        },
        formatSelection: function (state) {
            if ($(state.element).data('active') == "0") {
                return state.text + "<i class='fa fa-dot-circle-o'></i>";
            } else {
                return state.text;
            }
        }
    });
});

function FunSearchRecord()//Search
{
    window.FunSearchRecord = FunSearchRecord
    var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, '', '', element_txtSearchTitle);
}

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val) {
    $.ajax({
        url: ROOTURL + '/PDFRead/grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val },
        beforeSend: function () {
            Loader(1);
            $("#preloader").show();
            $("#status").show();
        },
        success: function (response) {
            $('#grddata').empty();
            $('#grddata').append(response);
            $("#preloader").hide();
            $("#status").hide();
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
}
let isStatusColumnHidden = false; 

//syncfusion grid code start from here
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
    var allRows = grid.getCurrentViewRecords();
    if (allRows.length > 0) {
        var allIsReadTrue = allRows.every(function (row) {
            return row.IsRead === true;
        });
        if (allIsReadTrue && !isStatusColumnHidden) {
            grid.hideColumns('Status');  
            isStatusColumnHidden = true; 
        } else if (!allIsReadTrue && isStatusColumnHidden) {
            grid.showColumns('Status');  
            isStatusColumnHidden = false;
        }        
    }    
}


function refreshGrid() {
    var grid = document.getElementById("PayrollFilesGrid").ej2_instances[0];
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
    var grid = document.querySelector('#PayrollFilesGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/PDFRead/UrlDatasource",
        adaptor: new CustomAdaptor()
    });
};


function ComfirmDeleteej2(ID) {    
    var DivId = "#" + ID;
    $(DivId).show();
}

function Deleteej2(ID) {    
    $.ajax({
        url: '/PDFRead/Delete',
        data: { id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("PayrollFilesGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/PDFRead/UrlDatasource",
                adaptor: new ej.data.UrlAdaptor()
            });
            //return true;
            toastr.success('Payroll data deleted successfully.');
        },
        error: function () {
            Loader(0);
        }
    });
}
