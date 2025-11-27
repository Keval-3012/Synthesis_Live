function BindtheTermination() {
    $("#TerminationDocument").html("");
    var StoreId = $('#StoreId').val();
    var EmployeeId = $('#EmployeeId').val();
    EmployeeChildId = $('#EmployeeChildId').val();

    $.ajax({
        url: '/HREmployeeProfile/HRTerminationUrlDatasource',
        type: "get",
        data: { EmployeeId: EmployeeId, StoreId: StoreId, EmployeeChildId: EmployeeChildId },
        contentType: "application/json;",
        success: function (data) {
            if (data.result != null) {
                const eCard1 = createTCardDiv("Termination", data.result.terminationfile);
                if (eCard1 != null) {
                    $("#TerminationDocument").append(eCard1);
                }
            }
        }
    });
}
function createTCardDiv(title, data) {
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
        if (title == "Termination") {
            anchor.href = `/HREmployeeProfile/DownloadTermination?DocumentFileName=${val.DocFileName}&EmployeeId=${val.EmployeeId}`;
        }


        const icon = document.createElement("i");
        const img = document.createElement("img");
        img.src = "/Content/Admin/images/pdf.png";
        icon.appendChild(img);
        anchor.appendChild(icon);
        anchor.appendChild(document.createTextNode(val.DocFileName));
        const span = document.createElement("span");

        // Create image element for close icon
        var closeImg = document.createElement('img');
        closeImg.src = '/Content/Admin/images/close.png';

        button.appendChild(anchor);
        button.appendChild(span);
        contentDiv.appendChild(button);
    });

    eCardDiv.appendChild(titleDiv);
    eCardDiv.appendChild(separatorDiv);
    eCardDiv.appendChild(contentDiv);

    return eCardDiv;
}