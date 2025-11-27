function BindtheDocument() {
    $("#Document").html("");
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
    EmployeeChildId = $('#EmployeeChildId').val();
    
    $.ajax({
        url: '/HREmployeeProfile/HRDocumnetsUrlDatasource',
        type: "get",
        data: { EmployeeId: EmployeeId, StoreId: StoreId, EmployeeChildId: EmployeeChildId },
        contentType: "application/json;",
        success: function (data) {
            if (data.result != null) {
                const eCard1 = createECardDiv("401(K) Plan", data.result.File401KPlan);
                const eCard2 = createECardDiv("Mobile Insurance", data.result.MobileInsurance);
                const eCard3 = createECardDiv("Essential Documents", data.result.EssentialDocuments);
                const eCard6 = createECardDiv("Warning", data.result.Warning);
                const eCard7 = createECardDiv("Termination", data.result.Termination);
                const eCard4 = createECardAnotherDiv("Signed Forms", data.result.SignedForms);
                const eCard5 = createECardAnotherDiv("Vaccination Info", data.result.VaccinationInfo);
                if (eCard1 != null) {
                    $("#Document").append(eCard1);
                }
                if (eCard2 != null) {
                    $("#Document").append(eCard2);
                }
                if (eCard3 != null) {
                    $("#Document").append(eCard3);
                }
                if (eCard6 != null) {
                    $("#Document").append(eCard6);
                }
                if (eCard7 != null) {
                    $("#Document").append(eCard7);
                }
                if (eCard4 != null) {
                    $("#Document").append(eCard4);
                }
                if (eCard5 != null) {
                    $("#Document").append(eCard5);
                }
            }
        }
    });
}

function createECardDiv(title, data) {
    if (data.length == 0) return null; // Exit early if data is null

    const eCardDiv = document.createElement("div");
    eCardDiv.classList.add("e-card");
    eCardDiv.id = "basic";
    eCardDiv.style = "margin-top: 10px";

    const titleDiv = document.createElement("div");
    titleDiv.classList.add("e-card-title");
    titleDiv.textContent = title;

    const separatorDiv = document.createElement("div");
    separatorDiv.classList.add("e-card-separator");

    const contentDiv = document.createElement("div");
    contentDiv.classList.add("e-card-content");
    contentDiv.classList.add("essential-c-row");

    $.each(data, function (key, val) {
        const button = document.createElement("button");
        button.classList.add("btn-pdf");

        const anchor = document.createElement("a");
        if (title == "401(K) Plan") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=1`;
        }
        else if (title == "Mobile Insurance") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=2`;
        }
        else if (title == "Essential Documents") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=3`;
        }
        else if (title == "Warning") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=6`;
        }
        else if (title == "Termination") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=7`;
        }
       

        const icon = document.createElement("i");
        const img = document.createElement("img");
        img.src = "/Content/Admin/images/pdf.png";
        icon.appendChild(img);
        anchor.appendChild(icon);
        anchor.appendChild(document.createTextNode(val.FileName));
        const span = document.createElement("span");

        var closeAnchor = document.createElement('a');
        closeAnchor.className = 'close-btn';
        closeAnchor.href = '#';
        if (title == "401(K) Plan") {
            closeAnchor.setAttribute('onclick', 'return ConfirmEsseDialog(' + val.DocId + ',' + val.EmployeeId + ',"' + val.FileName + '",1);');
        }
        else if (title == "Mobile Insurance") {
            closeAnchor.setAttribute('onclick', 'return ConfirmEsseDialog(' + val.DocId + ',' + val.EmployeeId + ',"' + val.FileName + '",2);');
        }
        else if (title == "Essential Documents") {
            closeAnchor.setAttribute('onclick', 'return ConfirmEsseDialog(' + val.DocId + ',' + val.EmployeeId + ',"' + val.FileName + '",3);');
        }
        else if (title == "Warning") {
            closeAnchor.setAttribute('onclick', 'return ConfirmEsseDialog(' + val.DocId + ',' + val.EmployeeId + ',"' + val.FileName + '",6);');
        }
        else if (title == "Termination") {
            closeAnchor.setAttribute('onclick', 'return ConfirmEsseDialog(' + val.DocId + ',' + val.EmployeeId + ',"' + val.FileName + '",7);');
        }

        // Create image element for close icon
        var closeImg = document.createElement('img');
        closeImg.src = '/Content/Admin/images/close.png';

        // Append close image to close anchor
        closeAnchor.appendChild(closeImg);

        span.appendChild(closeAnchor);

        button.appendChild(anchor);
        button.appendChild(span);
        contentDiv.appendChild(button);
    });

    eCardDiv.appendChild(titleDiv);
    eCardDiv.appendChild(separatorDiv);
    eCardDiv.appendChild(contentDiv);

    return eCardDiv;
}

function createECardAnotherDiv(title, data) {
    if (data.length == 0) return null; // Exit early if data is null

    const eCardDiv = document.createElement("div");
    eCardDiv.classList.add("e-card");
    eCardDiv.id = "basic";
    eCardDiv.style = "margin-top: 10px";

    const titleDiv = document.createElement("div");
    titleDiv.classList.add("e-card-title");
    titleDiv.textContent = title;

    const separatorDiv = document.createElement("div");
    separatorDiv.classList.add("e-card-separator");

    const contentDiv = document.createElement("div");
    contentDiv.classList.add("e-card-content");
    contentDiv.classList.add("essential-c-row");

    $.each(data, function (key, val) {
        var cardDiv = document.createElement('div');
        cardDiv.id = 'Card';

        // Create the card container div with class "e-card" and "profile"
        var cardContainerDiv = document.createElement('div');
        cardContainerDiv.className = 'e-card profile';
        cardContainerDiv.setAttribute('tabindex', '0');
        cardContainerDiv.style.justifyContent = 'flex-start';

        // Create the anchor element
        var anchor = document.createElement('a');
        if (title == "Signed Forms") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=4`;
        }
        else if (title == "Vaccination Info") {
            anchor.href = `/HREmployeeProfile/DownloadDocument?DocumentFileName=${val.FileName}&EmployeeId=${val.EmployeeId}&Type=5`;
        }
        // Create the first card header div with class "e-card-header"
        var cardHeader1Div = document.createElement('div');
        cardHeader1Div.className = 'e-card-header';
        cardHeader1Div.style = 'justify-content: center !important;';


        var cardHeaderImageDiv = document.createElement('div');
        cardHeaderImageDiv.className = 'e-card-header-image e-card-corner';

        cardHeader1Div.appendChild(cardHeaderImageDiv);

        // Create the second card header div with class "e-card-header"
        var cardHeader2Div = document.createElement('div');
        cardHeader2Div.className = 'e-card-header';
        cardHeader2Div.style = 'justify-content: center !important;';

        // Create the header caption div with class "e-card-header-caption" and "center"
        var headerCaptionDiv = document.createElement('div');
        headerCaptionDiv.className = 'e-card-header-caption center';

        // Create the header title div with class "e-card-header-title"
        var headerTitleDiv = document.createElement('div');
        headerTitleDiv.className = 'e-card-header-title';
        if (title == "Signed Forms") {
            if (val.DocumentType == 2) {
                headerTitleDiv.textContent = 'Schedule Change';
            }
            else {
                headerTitleDiv.textContent = 'Consent form';
            }
        }
        else if (title == "Vaccination Info") {
            headerTitleDiv.textContent = 'Vaccine Certificate';
        }

        // Create the separator div with class "e-card-separator"
        var separatorDiv1 = document.createElement('div');
        separatorDiv1.className = 'e-card-separator';

        // Create the content div with class "e-card-content" and style background
        var contentDiv1 = document.createElement('div');
        contentDiv1.className = 'e-card-content';
        contentDiv1.style.background = '#ffb3b3';
        if (title == "Signed Forms") {
            contentDiv1.textContent = 'Signed On: ' + val.CreatedOn;
        }
        else if (title == "Vaccination Info") {
            contentDiv1.textContent = 'Uploaded on: ' + val.CreatedOn;
        }

        // Append all elements in the correct hierarchy
        headerCaptionDiv.appendChild(headerTitleDiv);
        cardHeader2Div.appendChild(headerCaptionDiv);
        anchor.appendChild(cardHeader1Div);
        anchor.appendChild(cardHeader2Div);
        anchor.appendChild(separatorDiv1);
        anchor.appendChild(contentDiv1);
        cardContainerDiv.appendChild(anchor);
        cardDiv.appendChild(cardContainerDiv);
        contentDiv.appendChild(cardDiv);
    });

    eCardDiv.appendChild(titleDiv);
    eCardDiv.appendChild(separatorDiv);
    eCardDiv.appendChild(contentDiv);

    return eCardDiv;
}

function ConfirmEsseDialog(DocId, EmployeeId, FileName, Type) {
    showConfirmationDialog('Are you sure you want to delete this record?', function (result) {
        if (result) {
            $.ajax({
                url: '/HREmployeeProfile/DeleteEmployeeDocument', // URL to your server-side delete endpoint
                type: 'POST', // or 'GET', depending on your server-side implementation
                data: {
                    DocId: DocId, EmployeeId: EmployeeId, FileName: FileName, Type: Type
                }, // Data to send to the server
                success: function (response) {
                    // Handle success response from the server
                    var toastObj = document.getElementById('toast_type').ej2_instances[0];
                    if (response.success) {
                        // Reload the page or update UI as needed
                        toastObj.content = response.success;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                        BindtheDocument();

                    } else {
                        // Handle error or display error message
                        toastObj.content = response.Error;
                        toastObj.target = document.body;
                        toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                    }
                },
                error: function (xhr, status, error) {
                    // Handle AJAX call error
                    console.error(xhr.responseText);
                }
            });
        } else {
            console.log("Closed");
            // User clicked Cancel or closed the dialog
            // Handle the cancellation or take appropriate action
        }
    });
}

function showConfirmationDialog(message, callback) {
    // Create a new Dialog instance
    var dialog = new ej.popups.Dialog({
        header: 'Confirmation', // Set dialog header
        content: message, // Set dialog content to the message
        width: '300px', // Set dialog width
        animationSettings: { effect: 'Zoom' }, // Set animation effect
        buttons: [
            {
                buttonModel: {
                    content: 'OK',
                    isPrimary: true,
                },
                click: function () {
                    if (typeof callback === 'function') {
                        callback(true); // Invoke the callback function with true
                    }
                    dialogButtonClick.call(this); // Hide and destroy the dialog
                }
            },
            {
                buttonModel: {
                    content: 'Cancel'
                },
                click: function () {
                    if (typeof callback === 'function') {
                        callback(false); // Invoke the callback function with false
                    }
                    dialogButtonClick.call(this); // Hide and destroy the dialog
                }
            }
        ] // Add OK and Cancel buttons
    });

    // Render the dialog
    dialog.appendTo('#customDialog');
    dialog.show(); // Show the dialog

    var gridContainer = document.getElementById('EmployeeContent');
    gridContainer.classList.add('blur-background');
}

function dialogButtonClick() {
    this.hide(); // Hide the dialog when OK button is clicked
    this.destroy(); // Destroy the dialog instance
    document.getElementById('customDialog').innerHTML = '';

    var gridContainer = document.getElementById('EmployeeContent');
    gridContainer.classList.remove('blur-background');
}

var AddDocument;
function createdAddDocument() {

    AddDocument = this;
    AddDocument.hide();
}

function GetAddDocument() {
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
    EmployeeChildId = $('#EmployeeChildId').val();

    openDialogBoxAddDocument(EmployeeId, EmployeeChildId, StoreId);
}

function openDialogBoxAddDocument(EmployeeId, EmployeeChildId, StoreId) {
    scrollVal = $(window).scrollTop();
    AddDocument.show();
    $(".e-dlg-overlay").remove();
    $("#Upload401plan").html('');
    var spinnerTarget = document.getElementById('ajax_dialogAddDocument')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/HRDocumentsAddPartial', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ EmployeeId: EmployeeId, EmployeeChildId: EmployeeChildId, StoreId: StoreId })
    });
    ajax.send().then((data) => {
        $("#AddDocument").html(data);
    });

    ej.popups.hideSpinner(spinnerTarget);
}

function AddDocumentpromptBtnClick() {
    AddDocument.hide();
    $(window).scrollTop(scrollVal);
    scrollVal = 0;
}

function InsertDocument(args) {
    if (args.action != "enter") {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var EmployeeId = document.getElementById('EmployeeId').value;
        var EmployeeChildId = document.getElementById('EmployeeChildId').value;
        var FileName = document.getElementById('FileName').value;
        var Comments = document.getElementById('Comments').value;
        if (FileName == '') {
            toastObj.content = "Upload File is required ";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            return false;
        }

        $.ajax({
            url: ROOTURL + 'HREmployeeProfile/HRDocumentsInsert',
            type: "POST",
            data: {
                "EmployeeId": EmployeeId, "EmployeeChildId": EmployeeChildId, "FileName": FileName, "Comments": Comments
            },
            "success": function (data) {
                if (!ej.base.isNullOrUndefined(data.success)) {

                    toastObj.content = data.success;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    BindtheDocument();
                    AddDocument.hide();
                }
                if (!ej.base.isNullOrUndefined(data.Error)) {

                    toastObj.content = data.Error;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            }

        });
        //event.preventDefault();

        console.log(EmployeeId);
        console.log(EmployeeChildId);
        console.log(FileName);
        console.log(Comments);
    }
}


var Upload401plan;

function createdUpload401plan() {

    Upload401plan = this;
    Upload401plan.hide();
}

function Upload401DocumentpromptBtnClick() {
    Upload401plan.hide();
    $(window).scrollTop(scrollVal);
    scrollVal = 0;
}

function Insert401Document(args) {
    if (args.action != "enter") {
        var toastObj = document.getElementById('toast_type').ej2_instances[0];
        var EmployeeId = document.getElementById('EmployeeId').value;
        var EmployeeChildId = document.getElementById('EmployeeChildId').value;
        var FileName = document.getElementById('FileName').value;
        var rdopt = $('input[name="rdopt"]:checked').val();
        if (FileName == '') {
            toastObj.content = "Upload File is required ";
            toastObj.target = document.body;
            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            return false;
        }

        $.ajax({
            url: ROOTURL + 'HREmployeeProfile/HRDocuments401kInsert',
            type: "POST",
            data: {
                "EmployeeId": EmployeeId, "EmployeeChildId": EmployeeChildId, "FileName": FileName, "rdopt": rdopt
            },
            "success": function (data) {
                if (!ej.base.isNullOrUndefined(data.success)) {

                    toastObj.content = data.success;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
                    BindtheDocument();
                    Upload401plan.hide();
                }
                if (!ej.base.isNullOrUndefined(data.Error)) {

                    toastObj.content = data.Error;
                    toastObj.target = document.body;
                    toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
                }

            }

        });
        //event.preventDefault();

        console.log(EmployeeId);
        console.log(EmployeeChildId);
        console.log(FileName);
        console.log(Comments);
    }
}

function GetUpload401plan() {
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
    EmployeeChildId = $('#EmployeeChildId').val();

    openDialogBoxUpload401plan(EmployeeId, EmployeeChildId, StoreId);
}

function openDialogBoxUpload401plan(EmployeeId, EmployeeChildId, StoreId) {
    scrollVal = $(window).scrollTop();
    Upload401plan.show();
    $(".e-dlg-overlay").remove();
    $("#AddDocument").html('');
    var spinnerTarget = document.getElementById('ajax_dialogUpload401plan')
    ej.popups.createSpinner({
        target: spinnerTarget
    });
    ej.popups.showSpinner(spinnerTarget);
    var ajax = new ej.base.Ajax({
        url: ROOTURL + 'HREmployeeProfile/HRDocumentsUpload401planPartial', //render the partial view
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ EmployeeId: EmployeeId, EmployeeChildId: EmployeeChildId, StoreId: StoreId })
    });
    ajax.send().then((data) => {
        $("#Upload401plan").html(data);
    });

    ej.popups.hideSpinner(spinnerTarget);
}