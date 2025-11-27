$(document).ready(function () {
    $("#Title").autocomplete(false);
    var uploadField = document.getElementById("AttachFile");
    uploadField.onchange = function () {
        var extension = uploadField.value.substr((uploadField.value.lastIndexOf('.') + 1));

        switch (extension.toLowerCase()) {
            case 'jpg':
                removestyle();
                break;
            case 'jpeg':
                removestyle();
                break;
            case 'png':
                removestyle();
                break;
            case 'gif':
                removestyle();
                break;
            /*alert('was jpg png gif');*/  // There's was a typo in the example where
            // the alert ended with pdf instead of gif.
            case 'pdf':
                removestyle();
                break;
            case 'doc':
                removestyle();
                break;
            case 'docx':
                removestyle();
                break;
            case 'xls':
                removestyle();
                break;
            case 'xlsx':
                removestyle();
                break;
            case 'mp4':
                removestyle();
                break;
            case 'avi':
                removestyle();
                break;
            case 'wmv':
                removestyle();
                break;
            case 'mov':
                removestyle();
                break;
            case 'mkv':
                removestyle();
                break;
            case '':
                $(".blankfileupload").hide();
                break;
            default:
                uploadField.value = '';
                $("#popupSizeValidFileExtenstion").show();

        };
        if (this.files.length > 0) {
            if (this.files[0].size > 26214400) {
                //alert("File is too big!");
                $("#popupSize").show();
                this.value = "";
            };
            if (this.files[0].size <= 0) {
                //alert("File is too big!");
                $("#popupSizeBlankFile").show();
                this.value = "";
            };
        }
    };
});

$('#Title').bind('blur change', function (e) {
    if ($('#Title').val() === null || $('#Title').val() === '') {

        $('#Title').addClass('redborderbox');
    }
    else {
        $('#Title').removeClass('redborderbox');
    }
})

function validatedrptext(myVald) {
    $('.drpcls').each(function () {
        if ($(this).val() === null || $(this).val() === '') {
            $(this).addClass('input-validation-error');
            //alert();
            evt.preventDefault();
        }
        else {
            $(this).removeClass('input-validation-error');
            $(this).addClass('input-validation-valid');
        }
    });

    if ($('#Title').val() === null || $('#Title').val() === '') {

        $('#Title').addClass('redborderbox');
        evt.preventDefault();
    }
    else {
        $('#Title').removeClass('redborderbox');
    }

    if (myVald == 1) {
        var AttachFile = $("#AttachFile").val();
        if (AttachFile == "") {
            $('#popupid').show();
            return false;
        }
        else {
            $('#popupid').hide();

        }
    }
    $("#hdnreqval").val("1");
} 

var uploadField = document.getElementById("AttachFile");

uploadField.onchange = function () {
    var extension = uploadField.value.substr((uploadField.value.lastIndexOf('.') + 1));
    switch (extension.toLowerCase()) {
        case 'jpg':
            removestyle();
            break;
        case 'jpeg':
            removestyle();
            break;
        case 'png':
            removestyle();
            break;
        case 'gif':
            removestyle();
            break;
        /*alert('was jpg png gif');*/  // There's was a typo in the example where
        // the alert ended with pdf instead of gif.
        case 'pdf':
            removestyle();
            break;
        case 'doc':
            removestyle();
            break;
        case 'docx':
            removestyle();
            break;
        case 'xls':
            removestyle();
            break;
        case 'xlsx':
            removestyle();
            break;
        case 'mp4':
            removestyle();
            break;
        case 'avi':
            removestyle();
            break;
        case 'wmv':
            removestyle();
            break;
        case 'mov':
            removestyle();
            break;
        case 'mkv':
            removestyle();
            break;
        case '':
            $(".blankfileupload").hide();
            break;
        default:
            uploadField.value = '';
            $("#popupSizeValidFileExtenstion").show();

    };

    if (this.files.length > 0) {
        //if (this.files[0].size > 20971520) {
        if (this.files[0].size > 26214400) {
            //alert("File is too big!");
            $("#popupSize").show();
            this.value = "";
        };
        if (this.files[0].size <= 0) {
            //alert("File is too big!");
            $("#popupSizeBlankFile").show();
            this.value = "";
        };
    }
};

function removestyle() {
    $(".blankfileupload").removeAttr("style");
}

$(".blankfileupload a").click(function () {
    $(".blankfileupload").hide();
    $("#AttachFile").val('');
});

function closemodal() {
    $(".divIDClass").hide();
}

function ComfirmFav1(ID) {

    var DivId = "#" + ID + "F1";
    $(DivId).show();
}

function ComfirmFav(ID) {
    var DivId = "#" + ID + "F";
    $(DivId).show();

}

function AddToFavorite(ID) {

    $.ajax({
        url: ROOTURL + '/Documents/AddToFavorite',
        data: { DocId: ID },
        async: false,
        success: function (response) {
            if (response.sucess == 1) {
                closemodal();
                if (response.msg != "") {
                    MyCustomToster(response.msg);
                }
                window.location = ROOTURL + 'Documents/DetailDocument?Id=' + ID;
            }
            else {
                MyCustomToster(response.msg);
                closemodal();
                //MyfunError();
            }

            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

function ComfirmDelete() {
    $("#ConfirmDeleteId").show();
}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + '/Documents/delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            if (response.sucess == 1) {
                window.location.href = ROOTURL + '/Documents/Index'
            }

            else {
                MyfunError();
            }
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {

    var container = document.getElementById("container").ej2_instances[0];
    if (container.documentEditor.documentName === '') {
        container.documentEditor.documentName = 'Untitled';
    }
    var documentTitleContentEditor = document.getElementById('DocumentName');
    documentTitleContentEditor.textContent = container.documentEditor.documentName;
    container.documentChange = function () {
        documentTitleContentEditor.textContent = container.documentEditor.documentName;
    };
})

function onPrintFile() {
    document.getElementById("container").ej2_instances[0].documentEditor.print();
}

function saveFile() {
    var container = document.getElementById("container").ej2_instances[0];
    container.documentEditor.save(container.documentEditor.documentName === '' ? 'sample' : container.documentEditor.documentName, 'Docx');
}

function EditDocument() {

    var DocumentId = $('#DocumentId').val();
    var DocumentCategoryId = $('#DocumentCategoryId').val();
    var KeyWords = $('#KeyWords').val();
    var Notes = $('#Notes').val();
    var chkFav = $('#chkFav').val();
    var Title = $('#Title').val();
    $.ajax({
        url: '/Documents/EditDocumentsDetaliValue',
        type: "post",
        cache: false,
        data: { DocumentId: DocumentId, DocumentCategoryId: DocumentCategoryId, KeyWords: KeyWords, Notes: Notes, chkFav: chkFav, Title: Title },
        success: function (states) {

            if (states == "Completed") {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": null,
                    "showDuration": "3000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
                toastr.success('Successfully Edit Document Data.');
                $('#EditModel').modal('hide');
            }
            else {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": null,
                    "showDuration": "3000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }
                toastr.error('Something went to wrong');
            }
        }
    });
}

var fileName = filePath;

function openDocument(obj) {
    openFile(obj.dataset.link);
}

document.getElementById("savebtn").addEventListener('click', function () {
    saveDocument()
});

var documenteditor;

function onCreatedFile() {

    $('#HeaderStoreId').prop('disabled', true);
    var items = ["Undo", "Redo", "Comments", "Image", "Table", "Hyperlink", "Bookmark", "TableOfContents", "Header", "Footer", "PageSetup", "PageNumber", "Break", "Find", "LocalClipboard", "RestrictEditing"];
    var container = document.getElementById("container").ej2_instances[0];
    /* container.toolbarItems = items;*/
    var i = 0;
    $('.e-toolbar-items').find('div').each(function (i1, obj) {
        var row = $(this);
        if (i < 3) {
            row.hide();
        }
        i++;
    });
    var documentTitleContentEditor = document.getElementById('DocumentName');
    documentTitleContentEditor.textContent = container.documentEditor.documentName;
    documenteditor = container.documentEditor;
    documenteditor.resize();
    openFile(fileName);
}

function openFile(fileName) {

    var httpRequest = new XMLHttpRequest();
    httpRequest.open('Post', '/api/DocumentEditor/Import?fileName=' + fileName, true);
    httpRequest.onreadystatechange = function () {
        if (httpRequest.readyState === 4) {
            if (httpRequest.status === 200 || httpRequest.status === 304) {
                console.log(httpRequest.responseText);
                documenteditor.open(httpRequest.responseText);
            } else {
                alert(FailLoad);
            }
        }
    };
    documenteditor.documentName = fileName.substr(0, fileName.lastIndexOf('.'));
    httpRequest.send();
}

function saveDocument() {
    documenteditor.saveAsBlob("Docx").then(function (blob) {
        var fileReader = new FileReader();

        fileReader.onload = function () {
            var fileName = documenteditor.documentName ? documenteditor.documentName : 'Untitled';
            var documentData = {
                fileName: fileName + '.docx',
                documentData: fileReader.result
            }
            var httpRequest = new XMLHttpRequest();
            httpRequest.open('Post', '/api/DocumentEditor/Save', true);
            httpRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            httpRequest.onreadystatechange = function () {
                if (httpRequest.readyState === 4) {
                    if (httpRequest.status === 200 || httpRequest.status === 304) {
                        toastr.options = {
                            "closeButton": true,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": false,
                            "positionClass": "toast-top-right",
                            "preventDuplicates": false,
                            "onclick": null,
                            "showDuration": "3000",
                            "hideDuration": "1000",
                            "timeOut": "5000",
                            "extendedTimeOut": "10000",
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }
                        toastr.success(SavedWord);
                    } else {
                        toastr.options = {
                            "closeButton": true,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": false,
                            "positionClass": "toast-top-right",
                            "preventDuplicates": false,
                            "onclick": null,
                            "showDuration": "3000",
                            "hideDuration": "1000",
                            "timeOut": "5000",
                            "extendedTimeOut": "10000",
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }

                        toastr.error(FailTosave);
                    }
                }
            };
            httpRequest.send(JSON.stringify(documentData));
        };

        fileReader.readAsDataURL(blob);
    });
}

$(".SaveDocumodel").click(function () {

    for (var i = 0; i < $("#FileUpload1")[0].files.length; i++) {
        alert($("#FileUpload1")[0].files[i].name);
    }
})

$(document).ready(function () {
    $(".filtermenulink").click(function () {
        $(".filtermenulink").removeClass("activemenu");
        $(this).addClass("activemenu");
        FunSearchRecord();
    });
    $("#txtSearchTitle").autocomplete(false);

});

$(".filtermenulink").click(function () {
    $(".filtermenulink").removeClass("activemenu");
    $(this).addClass("activemenu");
    FunSearchRecord();
});

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

var pageSize = 20;
var pageIndex = 1;
var pageCount = totalcount;

$(window).scroll(function () {
    if (Math.round($(window).scrollTop(), 0) == Math.round(($(document).height() - $(window).height()), 0)) {
        myCustomFn();
    }
});

function myCustomFn() {

    var page_loadsize = parseInt(PageSize) + parseInt(20);
    document.getElementById('hdnpagevalue').value = page_loadsize;

    GetData(pageSize, pageIndex);
    $("#hdnpagevalue").val(page_loadsize);
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
    bind();
}

function bind() {

    var dept = CategoryId;
    if (dept != '0') {
        document.getElementById('DrpCategory').value = CategoryId;
    }
    document.getElementById('txtstartdate').value = startdate;
    document.getElementById('txtenddate').value = enddate;
    document.getElementById('txtSearchTitle').value = searchTitle;
    document.getElementById('chkImages').checked = chkImages == 'True' ? true : false;
    document.getElementById('chkEmail').checked = chkEmail == 'True' ? true : false;
    document.getElementById('chkDoc').checked = chkDoc == 'True' ? true : false;
    document.getElementById('chkSheet').checked = chkSheet == 'True' ? true : false;
    document.getElementById('chkOther').checked = chkOther == 'True' ? true : false;

    $('#txtstartdate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false
    });
    $('#txtenddate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false
    });

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

}

function FunSearchRecord()//Search
{
    //window.FunSearchRecord=FunSearchRecord
    var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
    var element_categoryId = document.getElementById('DrpCategory').value;
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    var tabListing = $("a.activemenu").attr('id');

    var element_chkImages = document.getElementById('chkImages').checked;
    var element_chkEmail = document.getElementById('chkEmail').checked;
    var element_chkDoc = document.getElementById('chkDoc').checked;
    var element_chkSheet = document.getElementById('chkSheet').checked;
    var element_chkOther = document.getElementById('chkOther').checked;

    GetData(1, 1, OrderByVal, 0, PageSize, '', '', element_txtSearchTitle, element_categoryId, element_txtstartdate, element_txtenddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
}

function FunPageIndex(pageindex)//grid pagination
{
    GetData(0, pageindex, OrderByVal, IsAscVal, PageSize, '', '', SearchTitle, CategoryId, startdate, enddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
}

function FunSortData(SortData)//Grid header sorting
{

    var element_chkImages = document.getElementById('chkImages').checked;
    var element_chkEmail = document.getElementById('chkEmail').checked;
    var element_chkDoc = document.getElementById('chkDoc').checked;
    var element_chkSheet = document.getElementById('chkSheet').checked;
    var element_chkOther = document.getElementById('chkOther').checked;
    if (OrderByVal != SortData) {
        GetData(0, CurrentPageIndex, SortData, 1, PageSize, '', '', SearchTitle, CategoryId, startdate, enddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
    }
    else {
        GetData(0, CurrentPageIndex, SortData, IsAscVal, PageSize, '', '', SearchTitle, CategoryId, startdate, enddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
    }

}

function FunPageRecord(PageRecord)//Grid Page per record
{
    var element_chkImages = document.getElementById('chkImages').checked;
    var element_chkEmail = document.getElementById('chkEmail').checked;
    var element_chkDoc = document.getElementById('chkDoc').checked;
    var element_chkSheet = document.getElementById('chkSheet').checked;
    var element_chkOther = document.getElementById('chkOther').checked;
    GetData(0, 1, OrderByVal, IsAscVal, PageSize, '', '', SearchTitle, CategoryId, startdate, enddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
}

function FunAlphaSearchRecord(alpha)//Alpha Search
{
    GetData(1, 1, OrderByVal, IsAscVal, PageSize, alpha, '', SearchTitle, CategoryId, startdate, enddate, element_chkImages, element_chkEmail, element_chkDoc, element_chkSheet, element_chkOther, tabListing);
}
//For Search Button

function GetData(IsBindData_val, PageIndex, orderby_val, isAsc_val, PageSize_val, alpha_val, SearchRecords_val, SearchTitle_val, CategoryId_val, startdate_val, enddate_val, chkImages_val, chkEmail_val, chkDoc_val, chkSheet_val, chkOther_val, tabListing) {

    $.ajax({
        url: '/Documents/Grid',
        data: { IsBindData: IsBindData_val, currentPageIndex: PageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val, CategoryId: CategoryId_val, startdate: startdate_val, enddate: enddate_val, chkImages: chkImages_val, chkEmail: chkEmail_val, chkImages: chkImages_val, chkDoc: chkDoc_val, chkSheet: chkSheet_val, chkOther: chkOther_val, tabListing },

        beforeSend: function () { Loader(1); },
        // async: false,
        cache: false,
        success: function (response) {
            Loader(0);

            //    $("body").html(response);
            $('#grddata').html('');
            $('#grddata').append(response);

            Loader(0);
        },
        error: function () {
            Loader(0);
        }

    });

}

document.getElementById('txtSearchTitle').onkeypress = function (e) {
    if (e.keyCode == 13) {
        document.getElementById('btnSearch').click();
    }
}

function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + 'Documents/Delete',
        data: { Id: ID },
        async: false,
        success: function (response) {
            if (IsAscVal == 1) {
                GetData(1, 1, OrderByVal, 0, PageSize, '', '', SearchTitle
                    , CategoryId, startdate, enddate, chkImages, chkEmail
                    , chkDoc, chkSheet, chkOther);
            }
            else {
                GetData(1, 1, OrderByVal, 1, PageSize, '', '', SearchTitle
                    , CategoryId, startdate, enddate, chkImages, chkEmail
                    , chkDoc, chkSheet, chkOther);
            }
            if (response.sucess == 1)
                FunSearchRecord();
            else {
                MyfunError();
            }
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

function ComfirmFav1(ID) {
    var DivId = "#" + ID + "F1";
    $(DivId).show();
}

function ComfirmFav(ID) {
    var DivId = "#" + ID + "F";
    $(DivId).show();
}

var pageSize = 20;
var pageIndex = 1;
var pageCount = totalcount;

$(window).scroll(function () {
    if (Math.round($(window).scrollTop(), 0) == Math.round(($(document).height() - $(window).height()), 0)) {
        myCustomFn();
    }
});

function myCustomFn() {

    var page_loadsize = parseInt(PageSize) + parseInt(20);
    document.getElementById('hdnpagevalue').value = page_loadsize;

    GetData12(pageSize, pageIndex);
    $("#hdnpagevalue").val(page_loadsize);
}

function GetData12(PageSize, CurrentPageIndex) {

    var element_txtSearchTitle = document.getElementById('txtSearchTitle').value;
    var element_categoryId = document.getElementById('DrpCategory').value;
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    var tabListing = $("a.activemenu").attr('id');

    var element_chkImages = document.getElementById('chkImages').checked;
    var element_chkEmail = document.getElementById('chkEmail').checked;
    var element_chkDoc = document.getElementById('chkDoc').checked;
    var element_chkSheet = document.getElementById('chkSheet').checked;
    var element_chkOther = document.getElementById('chkOther').checked;

    var IsBindData_val = 1;
    //var PageIndex=1;
    var orderby_val = OrderByVal;
    var isAsc_val = IsAscVal;
    var PageSize_val = PageSize;
    var alpha_val = '';
    var SearchRecords_val = element_txtSearchTitle;
    var CategoryId_val = element_categoryId;
    var startdate_val = element_txtstartdate;
    var enddate_val = element_txtenddate;
    var chkImages_val = element_chkImages;
    var chkEmail_val = element_chkEmail;
    var chkDoc_val = element_chkDoc;
    var chkSheet_val = element_chkSheet;
    var chkOther_val = element_chkOther;
    var SearchTitle_val = element_txtSearchTitle;

    pageIndex++;
    if (pageIndex == 2 || pageIndex <= pageCount) {
        $.ajax({
            type: 'GET',
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            url: 'Document/GridScroll',
            cache: false,
            data: { IsBindData: IsBindData_val, currentPageIndex: CurrentPageIndex, orderby: orderby_val.trim(), IsAsc: isAsc_val, PageSize: PageSize_val, SearchRecords: SearchRecords_val, Alpha: alpha_val, SearchTitle: SearchTitle_val, CategoryId: CategoryId_val, startdate: startdate_val, enddate: enddate_val, chkImages: chkImages_val, chkEmail: chkEmail_val, chkImages: chkImages_val, chkDoc: chkDoc_val, chkSheet: chkSheet_val, chkOther: chkOther_val, tabListing: tabListing },

            beforeSend: function () {

                Loader(1);
            },
            timeout: 300000,
            success: function (data) {


                Loader(0);
                var CurrentUserId = 10;
                if (data != null && data.length != 0) {
                    for (var i = 0; i < data.length; i++) {

                        var table = $("#test table tbody tr").eq(0).clone(true);

                        $("#RowId", table).html(data[i].Id);

                        var ImageIdhtml = "<a target='_blank' href=" + data[i].AttachLink + "><img src='Content/Admin/images/" + data[i].FileTypeImage + "' alt='' style='height:34px;' /></a>";
                        $("#ImageId", table).html(ImageIdhtml);

                        if (data[i].isprivate) {
                            var isPrivateHtml = "<img src = 'Content/Admin/images/lock-document.svg' class='iconprivatelock' alt='' style='height:20px;' />";

                        }
                        else {
                            var isPrivateHtml = "";
                        }

                        $("#isprivateId", table).html(isPrivateHtml);

                        var DocTitleIdHtml = '<span class="documentttl"> <strong><a href="/Documents/DetailDocument?Id=' + data[i].Id + '" style="text-transform: uppercase;">' + data[i].DocTitle + '</a> </strong></span>';
                        $("#DocTitleId", table).html(DocTitleIdHtml);
                        $("#storeNameId", table).html(data[i].storeName);


                        $("#CreatedDateId", table).html(data[i].CreatedDateFormated);

                        if (data[i].Notes != "" && data[i].Notes != null) {

                            var nnotes = '<img id="ok" src="Content/Admin/images/icons/icon_notes.png" style="height:30px;" title="' + (data[i].Notes) + '">';

                            $("#NotesId", table).html(nnotes);
                        }
                        else {
                            $("#NotesId", table).html("");
                        }

                        var isFavoriteIdHtml = "";

                        var isFavHtml = "";
                        var sparkHtml = ' <div class="Fav-bloom"></div>';
                        sparkHtml += '<div class="Fav-sparkle">';
                        sparkHtml += '<div class="Fav-sparkle-line"></div>';
                        sparkHtml += '<div class="Fav-sparkle-line"></div>';
                        sparkHtml += '<div class="Fav-sparkle-line"></div>';
                        sparkHtml += '<div class="Fav-sparkle-line"></div>';
                        sparkHtml += '<div class="Fav-sparkle-line"></div>';
                        sparkHtml += ' </div>';
                        sparkHtml += '<svg class="Fav-star" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">';
                        sparkHtml += '<title>Star Icon</title>';
                        sparkHtml += '<path d="M36.14,3.09l5.42,17.78H59.66a4.39,4.39,0,0,1,2.62,7.87L47.48,40.14,53,58.3a4.34,4.34,0,0,1-6.77,4.78L32,52l-14.26,11A4.34,4.34,0,0,1,11,58.27l5.55-18.13L1.72,28.75a4.39,4.39,0,0,1,2.62-7.87h18.1L27.86,3.09A4.32,4.32,0,0,1,36.14,3.09Z" />';
                        sparkHtml += ' </svg>';

                        var unFavHtml = ' <label for="fav-checkbox" onclick="return ComfirmFav1(' + data[i].Id + ');" class="Fav-label"><span class="Fav-label-text">Favourite</span></label>';
                        var UserIdF1 = data[i].Id + "F1";

                        unFavHtml += '<div id="' + UserIdF1 + '" class="divIDClass modal-popup modal-danger modal-message" style="position: fixed;  left:45%; width:350px; top: 10px; display:none">';
                        unFavHtml += '<div class="modal-content ">';
                        unFavHtml += '<div class="modal-header text-center">';
                        unFavHtml += ' <img src="Content/Admin/images/star.svg"/>';
                        unFavHtml += '</div>';
                        unFavHtml += '<div class="modal-title">Message</div>';
                        unFavHtml += '<div class="modal-body ">Are you sure to Unfavorite this Document?</div>';
                        unFavHtml += '<div class="modal-footer" style="text-align:center">';
                        unFavHtml += '<a class="btn btn-danger" onclick="AddToFavorite(' + data[i].Id + ');">Ok </a>';
                        unFavHtml += '<a class="btn" data-dismiss="modal" onclick="closemodal()">Cancel</a>';
                        unFavHtml += '</div></div></div>';

                        var DoFavHtml = ' <label for="fav-checkbox" onclick="return ComfirmFav(' + data[i].Id + ');" class="Fav-label"><span class="Fav-label-text">Favourite</span></label>';
                        var UserIdF = data[i].Id + "F";

                        DoFavHtml += '<div id="' + UserIdF + '" class="divIDClass modal-popup modal-danger modal-message" style="position: fixed;  left:45%; width:350px; top: 10px; display:none">';
                        DoFavHtml += '<div class="modal-content ">';
                        DoFavHtml += '<div class="modal-header text-center">';
                        DoFavHtml += ' <img src="Content/Admin/images/star.svg"/>';
                        DoFavHtml += '</div>';
                        DoFavHtml += '<div class="modal-title">Message</div>';
                        DoFavHtml += '<div class="modal-body ">Are you sure to Unfavorite this Document?</div>';
                        DoFavHtml += '<div class="modal-footer" style="text-align:center">';
                        DoFavHtml += '<a class="btn btn-danger" onclick="AddToFavorite(' + data[i].Id + ');">Ok </a>';
                        DoFavHtml += '<a class="btn" data-dismiss="modal" onclick="closemodal()">Cancel</a>';
                        DoFavHtml += '</div></div></div>';

                        if ((storeMgrRole == 'true' && data[i].IsStatus_id == 3) || (dataappRole == 'true' && data[i].IsStatus_id == 2) || adminRole == 'true') {
                            if (data[i].FavId > 0) {
                                isFavHtml += '<div class="Fav"><input id="fav-checkbox2" class="Fav-checkbox" type="checkbox" checked>';

                                if (data[i].isFavorite) {
                                    isFavHtml += unFavHtml;
                                }
                                else {
                                    isFavHtml += DoFavHtml;
                                }

                                isFavHtml += sparkHtml;
                                isFavHtml += '</div>';

                            }
                            else {
                                isFavHtml += '<div class="Fav"><input id="fav-checkbox" class="Fav-checkbox" type="checkbox">';

                                if (data[i].isFavorite) {
                                    isFavHtml += unFavHtml;
                                }
                                else {
                                    isFavHtml += DoFavHtml;
                                }

                                isFavHtml += sparkHtml;
                            }
                        }
                        else {
                            if (data[i].FavId > 0) {
                                isFavHtml += '<div class="Fav"> <input id="fav-checkbox2" class="Fav-checkbox" type="checkbox" checked>';

                                if (data[i].isFavorite) {
                                    isFavHtml += unFavHtml;
                                }
                                else {
                                    isFavHtml += DoFavHtml;
                                }
                                isFavHtml += sparkHtml;
                            }
                            else {
                                isFavHtml += '<div class="Fav"> <input id="fav-checkbox" class="Fav-checkbox" type="checkbox">';

                                if (data[i].isFavorite) {
                                    isFavHtml += unFavHtml;
                                }
                                else {
                                    isFavHtml += DoFavHtml;
                                }
                                isFavHtml += sparkHtml;
                                isFavHtml += '</div>';

                            }
                        }
                        $("#isFavoriteId", table).html(isFavHtml);

                        var editdeleteidHtml = "";
                        if (data[i].IsStatus_id == CurrentUserId) {
                            editdeleteidHtml = '<input type="hidden" id="itemhiddenID" name="ID" value="' + data[i].Id + '" />';
                            editdeleteidHtml += '<input type="hidden" id="ID" name="ID" value="' + data[i].StoreId + '" />'

                            var UserIdD = data[i].Id + "D";
                            editdeleteidHtml += '<a href="#" onclick="return ComfirmDelete(' + data[i].Id + ');" data-toggle="tooltip" data-placement="top" data-original-title="Delete"><img src="/Content/Admin/images/trash-2.svg" alt="" /> </a>';

                            editdeleteidHtml += '<div id="' + UserIdD + '" class="divIDClass modal-popup modal-danger modal-message" style="position: fixed;  left:45%; width:350px; top: 10px; display:none">';
                            editdeleteidHtml += '<div class="modal-content ">';
                            editdeleteidHtml += '<div class="modal-header text-center">';
                            editdeleteidHtml += '  <i class="glyphicon glyphicon-trash"></i>';
                            editdeleteidHtml += '</div>';
                            editdeleteidHtml += '<div class="modal-title">Message</div>';
                            editdeleteidHtml += '<div class="modal-body ">Are you sure want to delete this Document?</div>';
                            editdeleteidHtml += '<div class="modal-footer" style="text-align:center">';
                            editdeleteidHtml += ' <a class="btn btn-danger" onclick="Delete(' + data[i].Id + ');">Ok </a>';
                            editdeleteidHtml += '<a class="btn" data-dismiss="modal" onclick="closemodal()">Cancel</a>';
                            editdeleteidHtml += '</div></div></div>'

                        }
                        else {
                            editdeleteidHtml = ' <a class="disabled" href="#" onclick="return ComfirmDelete(' + data[i].Id + ');" data-toggle="tooltip" data-placement="top" data-original-title="Delete"><img src="/Content/Admin/images/trash-2.svg" alt="" /> </a>';
                        }

                        if (data[i].IsStatus_id == CurrentUserId || adminRole == true) {
                            editdeleteidHtml += '<a href="Documents/Edit?Id=' + data[i].Id + '" data-toggle="tooltip" data-placement="top" data-original-title="Edit"><img src="Content/Admin/images/edit-2.svg" alt="" /></a>';
                        }
                        else {
                            editdeleteidHtml += '<a href="Edit?Id=' + data[i].Id + '" data-toggle="tooltip" data-placement="top" data-original-title="Edit"><img src="Content/Admin/images/edit-2.svg" alt="" /></a>';
                        }


                        $("#editdeleteid", table).html(editdeleteidHtml);
                        $("#test table tbody").append("<tr class='even'> " + table.html() + "</tr>")
                    }
                }

                $(".loading-container").hide();
            },

            complete: function () {
                Loader(0);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                Loader(0);
            }
        });
    }
}

$(window).scroll(function () {
    var scroll = $(window).scrollTop();

    //>=, not <=
    if (scroll >= 20) {
        //clearHeader, not clearheader - caps H

        $(".page-header-main").addClass("stikyheader");
        $(".documentgridtable").addClass("topsearcharea");

    } else {
        $(".page-header-main").removeClass("stikyheader");
        $(".documentgridtable").removeClass("topsearcharea");
    }
}); //missing );

$(document).ready(function () {
    ; (function ($) {
        $.fn.fixMe = function () {
            return this.each(function () {
                var $this = $(this),
                    $t_fixed;
                function init() {
                    $this.wrap('<div class="" />');
                    $t_fixed = $this.clone();
                    $t_fixed.find("tbody").remove().end().addClass("fixed").insertBefore($this);
                    resizeFixed();
                }
                function resizeFixed() {
                    $t_fixed.find("th").each(function (index) {
                        $(this).css("width", $this.find("th").eq(index).outerWidth() + "px");
                    });
                }
                function scrollFixed() {
                    var offset = $(this).scrollTop(),
                        tableOffsetTop = $this.offset().top,
                        tableOffsetBottom = tableOffsetTop + $this.height() - $this.find("thead").height();
                    if (offset < tableOffsetTop || offset > tableOffsetBottom)
                        $t_fixed.hide();
                    else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && $t_fixed.is(":hidden"))
                        $t_fixed.show();
                }
                $(window).resize(resizeFixed);
                $(window).scroll(scrollFixed);
                init();
            });
        };
    })(jQuery);

    $(document).ready(function () {
        $("table").fixMe();


    });
});

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
        url: ROOTURL + 'Documents/SaveDocumentsDetail',
        beforeSend: function () {
            $('.ajax-loader').css("visibility", "visible");
        },
        type: "POST",
        data: model,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.Result == 'success') {
                /*$("#txtDcomment").val('');*/
                $("input[name=DocumentFU]").val("");

                GetData(1, 1, OrderByVal, 0, PageSize, '', '', SearchTitle
                    , CategoryId, startdate, enddate, chkImages, chkEmail
                    , chkDoc, chkSheet, chkOther);

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
    $("#DocumentModal").modal('hide');
}

function undofilters() {

    GetData(1, 1, OrderByVal, 0, PageSize, '', '', '', '', '', '', '', '', '', '', '', '');
}

function DownloadPdf() {

    var pdfViewer = document.getElementById('PdfViewer').ej2_instances[0];
    pdfViewer.download();
}

function PrintPdf() {

    var pdfViewer = document.getElementById('PdfViewer').ej2_instances[0];
    pdfViewer.print.print();
}