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
    var grid = document.getElementById("MyDocumentGrid").ej2_instances[0];
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
    var grid = document.querySelector('#MyDocumentGrid').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Documents/UrlDatasource",
        //insertUrl: "/HRStoreManagers/InsertStoreManager",
        //updateUrl: "/HRStoreManagers/UpdateStoreManager",
        //removeUrl: "/HRStoreManagers/RemoveDepartment",
        adaptor: new CustomAdaptor()
    });
};

function ComfirmDeleteej2(ID) {
    var DivId = "#" + ID;
    var divmrg = DivId + "_delete";
    $(divmrg).show();
}

function Deleteej2(ID) {
    var startdate = $("#txtstartdate").val();
    var enddate = $("#txtenddate").val();
    $.ajax({
        url: '/Documents/Delete',
        data: { id: ID },
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("MyDocumentGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/Documents/UrlDatasource?startdate=" + startdate + "&enddate=" + enddate,
                adaptor: new ej.data.UrlAdaptor()
            });
            //return true;
            toastr.success('Document deleted successfully.');
        },
        error: function () {
            Loader(0);
        }
    });
}
function closemodal() {
    $(".divIDClass").hide();
}
function CloseDocumentModel() {
    $("#previews").html('');
    $("#DocumentModal").modal('hide');
}
$('#DocumentFU').on('change', function () {

    if ($("#DocumentFU")[0].files.length > 20) {
        alert("You can select only 20 Files");
        $("#DocumentFU")[0].val('');
    } else {

        images(this, '#previews');
    }

});
var images = function (input, imgPreview) {
    if (input.files) {

        var filesAmount = input.files.length;
        for (i = 0; i < filesAmount; i++) {
            var reader = new FileReader();
            var filename = input.files[i];
            $("<li><strong>" + filename.name + "</strong>&nbsp;&nbsp;<a style=\"color: #428bca;\" class=\"remove\" href=\"#\">Remove</a>&nbsp;&nbsp;<input type='checkbox' id='chkFav' name='chkFav'><label for='chkFav' style='font-size:16px;'>&nbsp;Favourite</label>&nbsp;&nbsp;<input type='checkbox' id='chkPrivate' name='chkPrivate'><label for='chkPrivate' style='font-size:16px;'>&nbsp;Private</label></li>").appendTo(imgPreview);
            $(".remove").click(function () {
                $(this).parent().remove();
            });
        }
    }
};

function SaveDocumentModel() {

    var model = new FormData();
    var check = "jpg,jpeg,png,gif,pdf,doc,docx,xls,xlsx,mp4,avi,wmv,mov,mkv";
    var mesg = "";
    for (var i = 0; i < $("#DocumentFU")[0].files.length; i++) {
        if (check.includes($("#DocumentFU")[0].files[i].name.split('.')[1])) {

            model.append("File", $("#DocumentFU")[0].files[i]);

            var preview = document.getElementById("previews");
            for (var j = 0; j < preview.childNodes.length; j++) {

                if (preview.childNodes[j].childNodes[0].innerText === $("#DocumentFU")[0].files[i].name) {

                    model.append("item", preview.childNodes[j].children[0].innerText + '^' + preview.childNodes[j].children[2].checked + '^' + preview.childNodes[j].children[4].checked);
                }
            }
        }
        else {
            if (mesg == "") {
                mesg = "Extension not supported : " + $("#DocumentFU")[0].files[i].name.split('.')[1];
            }
            else {
                mesg += "," + $("#DocumentFU")[0].files[i].name.split('.')[1];
            }
        }
    }
    if (mesg != "") {
        alert(mesg);
        return false;
    }
    $.ajax({
        url: ROOTURL + '/Documents/SaveDocumentsDetail',
        beforeSend: function () {
            $('.ajax-loader').css("visibility", "visible");
        },
        type: "POST",
        data: model,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.Result == 'success') {
                $("input[name=DocumentFU]").val("");
                var grid = document.getElementById("MyDocumentGrid").ej2_instances[0];
                grid.dataSource = new ej.data.DataManager({
                    url: "/Documents/UrlDatasource",
                    adaptor: new ej.data.UrlAdaptor()
                });
                $("#txtstartdate").val('');
                $("#txtenddate").val('');
                return false;
            }
            else {
                showAlert(data.Result, "danger", 5000);
            }
        },
        complete: function () {
            $('.ajax-loader').css("visibility", "hidden");
        },
        error: function (Result) {
        }
    });
    $("#previews").html('');
    $("#DocumentModal").modal('hide');
}
function SearchFromDate() {
    var startdate = $("#txtstartdate").val();
    var enddate = $("#txtenddate").val();
    var grid = document.getElementById("MyDocumentGrid").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Documents/UrlDatasource?startdate=" + startdate + "&enddate=" + enddate,
        adaptor: new ej.data.UrlAdaptor()
    });
}

function RefreshMyDocument() {
    location.reload();
}

function FavoriteStatusDetails(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.IsFavorite) {
        span.textContent = "Favorite"
    }
    else {
        span.textContent = "UnFavorite"
    }
    div.appendChild(span);
    return div.outerHTML;
}

function TypeStatusDetails(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.AttachExtention == ".png" || e.AttachExtention == ".jpg") {
        span.textContent = "Images"
    }
    else if (e.AttachExtention == ".pdf" || e.AttachExtention == ".docx") {
        span.textContent = "Documents"
    }
    else if (e.AttachExtention == ".xlsx") {
        span.textContent = "Sheets"
    }
    div.appendChild(span);
    return div.outerHTML;
}

function ComfirmUnfavorite(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
    $(".CheckFavModalbody").text("Are you sure to add Document in Unfavorite?");
}

function Comfirmfavorite(ID) {
    var DivId = "#" + ID;
    $(DivId).show();
    $(".CheckFavModalbody").text("Are you sure to add Document in favorite?");
}

function IsFavoriteUpdate(ID) {
    var startdate = $("#txtstartdate").val();
    var enddate = $("#txtenddate").val();
    $.ajax({
        url: '/Documents/AddToFavorite',
        data: { DocId: ID },
        success: function (response) {            
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("MyDocumentGrid").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/Documents/UrlDatasource?startdate=" + startdate + "&enddate=" + enddate,
                adaptor: new ej.data.UrlAdaptor()
            });
            toastr.success(response.msg);
        },
        error: function () {
            Loader(0);
        }
    });
}