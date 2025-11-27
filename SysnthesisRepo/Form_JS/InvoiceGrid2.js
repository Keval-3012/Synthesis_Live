function undofilters() {
    var grid = document.getElementById("InlineInvoiceEditing").ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/Invoices/UrlDatasourceInvoice",
        adaptor: new ej.data.UrlAdaptor()
    });
};

function getQuery() {
    var departmentId = document.getElementById("departmentid")?.value || localStorage.getItem('getdeptid') || '';
    return new ej.data.Query().addParams('deptid', departmentId);
}

function getInvoiceData() {
    var grid = document.getElementById("InlineInvoiceEditing").ej2_instances[0];
    localStorage.setItem('getdeptid', $("#departmentid").val());
    var deptid = $("#departmentid").val();
    grid.dataSource = new ej.data.DataManager({
        url: "/Invoices/UrlDatasourceInvoice",
        adaptor: new ej.data.UrlAdaptor(),
    });
    grid.query = new ej.data.Query().addParams('deptid', deptid);
}

function querycellinfo(args) {
    if (args.column.headerText === 'Amount') {

        if (args.data.InvoiceTypeId.toString() === '2') {
            var num = args.data.TotalAmount.toLocaleString(undefined, { minimumFractionDigits: 2 });

            args.cell.innerText = '- $ ' + num;
            args.cell.style.color = "red";
            args.cell.style.textAlign = "right";

        }
        else {
            var num = args.data.TotalAmount.toLocaleString(undefined, { minimumFractionDigits: 2 });

            args.cell.innerText = '$ ' + num;
            args.cell.style.textAlign = "right";
        }
    }
}

function dataBound(e) {
    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    //  checks whether the cancel icon is already present or not
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
        $("#InlineInvoiceEditing_searchbar").removeAttr("placeholder");
        $("#InlineInvoiceEditing_searchbar").attr('placeholder', 'Search by Vendor, Invoice #, Amount');
    }

    var dropdownHtml = document.getElementById('departmentDropdownTemplate').innerHTML;
    document.getElementById('departmentDropdownContainer').innerHTML = dropdownHtml;

    var deptid = $("#departmentid").val();
    var getdeptid = localStorage.getItem('getdeptid');
    if (deptid == "" && getdeptid != "") {
        $("#departmentid").val(getdeptid);
    }

    attachMouseOverEvent();
}

function ComfirmDelete(ID) {
    var Data = '@Session["searchdashbord"]';
    $.ajax({
        url: '/Invoices/CheckInvoiceUSedAnywhere',
        data: { InvoiceID: ID },
        beforeSend: function () { Loader(1); },
        success: function (response) {
            if (response != null) {
                $("p#deleModalbody").html(response);
            }
            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
    var DivId = "#" + ID;
    $(DivId).show();
}

function Delete(ID) {
    var aa = "";
    if ($("#deleModalbody").html().toString().includes("CashPaid Out")) {
        aa = "CashPaidOut";
    }
    $.ajax({
        url: '/Invoices/Delete',
        data: { Id: ID, From: aa },
        async: false,
        success: function (response) {
            $('.divIDClass').css('display', 'none');
            var grid = document.getElementById("InlineInvoiceEditing").ej2_instances[0];
            grid.dataSource = new ej.data.DataManager({
                url: "/Invoices/UrlDatasourceInvoice",
                adaptor: new ej.data.UrlAdaptor()
            });
            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

function Setvalues(invoiceid) {
    localStorage.removeItem("SelectedPersistData");
    localStorage.setItem("FromInvoicePage", "");


    var grid = document.getElementById("InlineInvoiceEditing").ej2_instances[0];
    for (var i = 0; i < grid.columns.length; i++) {
        grid.columns[i].backupHeader = grid.columns[i].headerText;    //take headerText duplicate to store in another property 
    }
    var persistData = JSON.stringify({ persistData: grid.getPersistData(), storeid: selectdstoreid }); // Grid persistData
    localStorage.setItem("SelectedPersistData", persistData);
    window.location.href = "/Invoices/Details?Id=" + invoiceid;
    //var ajax = new ej.base.Ajax({ // used our ajax to send the stored persistData to server
    //    url: "/Invoices/InvoiceGridStorePersistData",
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    datatype: "json",
    //    data: persistData
    //});
    //ajax.send().then(
    //    function () {
    //        // Success callback
    //        window.location.href = "/Invoices/Details?Id=" + invoiceid;
    //    },
    //    function (error) {
    //        // Error callback
    //        console.error("Error sending AJAX request:", error);
    //    }
    //);
}

function closemodal() {
    $(".divIDClass").hide();
}

function toolbarClick(args) {
    var gridObj = document.getElementById("InlineInvoiceEditing").ej2_instances[0];
    if (args.id === 'pdf') {
        gridObj.serverPdfExport("/Invoices/PdfExport");
    }
    if (args.id === 'excel') {
        gridObj.serverPdfExport("/Invoices/ExcelExport");
    }
}

//function ShowDetailsInfo(InvoiceID) {
//    var amountElement = $("#spnAmount_" + InvoiceID);

//    // Check if the tooltip already exists to avoid duplicate calls
//    if (amountElement.find('.tooltipTD').length === 0) {
//        $.ajax({
//            url: '/Invoices/GetInvoiceDetails',
//            type: "GET",
//            dataType: "JSON",
//            data: { InvoiceID: InvoiceID },
//            success: function (states) {
//                var str = '<ul>';
//                $.each(states, function (index, value) {
//                    str += '<li><span>' + value.DepartmentName + '</span><span>$ ' + value.Amount + '</span></li>';
//                });
//                str += '</ul>';
//                // Append the tooltip content to the amount span
//                amountElement.append('<span class="tooltipTD"><div class="depTooltip">' + str + '</div></span>');
//            }
//        });
//    }
//}


//function attachMouseOverEvent() {
//    $(".e-grid .e-rowcell").each(function () {
//        var fieldName = $(this).attr("aria-label");
//        if (fieldName && fieldName.includes("Amount")) {
//            var invoiceId = $(this).closest("tr").find("td[aria-label*='InvoiceId']").text();
//            $(this).on("mouseover", function () {
//                ShowDetailsInfo(invoiceId);
//            });
//        }
//    });
//}

function attachMouseOverEvent() {
    $(".e-grid .e-rowcell").each(function () {
        var fieldName = $(this).attr("aria-label");
        if (fieldName && fieldName.includes("Amount") && !fieldName.includes("-")) {
            var invoiceId = $(this).closest("tr").find("td[aria-label*='InvoiceId']").text();
            var invoiceTypeId = $(this).closest("tr").find("td[aria-label*='InvoiceTypeId']").text();
            $(this).on("click", function () {
                ShowDetailsInfo(invoiceId, invoiceTypeId, $(this));
            });
        }
    });
}

function ShowDetailsInfo(InvoiceID, invoiceTypeId, amountElement) {
    var offset = amountElement.offset();
    var width = amountElement.outerWidth();

    var tooltip = $('<div class="tooltipContainer"></div>');
    tooltip.css({
        position: 'absolute',
        top: offset.top,
        left: offset.left + width,
        //width: '200px',
        backgroundColor: '#333',
        color: '#f9f9f9',
        borderRadius: '6px',
        padding: '15px',
        boxShadow: '0px 0px 10px rgba(0, 0, 0, 0.1)',
        zIndex: 1000
    }).hide();

    $.ajax({
        url: '/Invoices/GetInvoiceDetails',
        type: "GET",
        dataType: "JSON",
        data: { InvoiceID: InvoiceID },
        success: function (states) {
            var str = '<ul>';
            $.each(states, function (index, value) {
                var formattedAmount = parseFloat(value.Amount).toLocaleString('en-US', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });

                if (invoiceTypeId === "1") {
                    //str += '<li><span>' + value.DepartmentName + '</span><span> $ ' + formattedAmount + '</span></li>';
                    str += '<li><span style="white-space: nowrap;">' + value.DepartmentName + ' $' + formattedAmount + '</span></li>';
                }
                else if (invoiceTypeId === "2") {
                    str += '<li><span style="white-space: nowrap;">' + value.DepartmentName + ' -$' + formattedAmount + '</span></li>';
                }
                
            });
            str += '</ul>';
            tooltip.html(str);
            $('body').append(tooltip);
            tooltip.fadeIn();
        }
    });

    amountElement.on('mouseleave', function () {
        tooltip.remove();
    });
}

//function ShowQbPaidPopup(iii) {
//    $('#QbPaidModal').modal('show');
//    $('.modal-backdrop').hide();
//    $.ajax({
//        url: ROOTURL + "Invoices/GetQbPaidStatusDetails",
//        type: 'POST',
//        data: { invoiceid: iii },
//        success: function (response) {
//            if (response.statusmessage == "Success") {
//                $(".qbpaymentbnd").text(response.qbpaymentmethod);
//                let formtqbamount = "$" + response.qbamount;
//                $(".qbamountbnd").text(formtqbamount);

//                let accountPaidText = response.qbaccountpaidname;
//                if (response.qbcheckno && response.qbcheckno.trim() !== "") {
//                    accountPaidText += " | " + "#" + response.qbcheckno;
//                }
//                $(".qbaccountnamebnd").text(accountPaidText);

//                let formtqbcheckamount = "$" + response.qbcheckamount;
//                $(".qbcheckamountbnd").text(formtqbcheckamount);

//                let timestamp = parseInt(response.qbpaymentdate.match(/\d+/)[0]);
//                let date = new Date(timestamp);
//                let formattedDate = (date.getMonth() + 1).toString().padStart(2, '0') + '/' + date.getDate().toString().padStart(2, '0') + '/' + date.getFullYear();
//                $(".qbpaydatebnd").text(formattedDate);
//            }
//            else {
//                //$('#QbPaidModal .modal-body').text("No Error Found!").css('font-size', '15px');
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error('Error:', error);
//        }
//    });
//};

function ShowQbPaidPopup(iii) {
    $('#QbPaidModal').modal('show');
    $('.modal-backdrop').hide();

    $.ajax({
        url: ROOTURL + "Invoices/GetQbPaidStatusDetails",
        type: 'POST',
        data: { invoiceid: iii },
        success: function (response) {
            if (response.statusmessage === "Success") {
                let modalBody = $("#QbPaidModal .modal-body");
                modalBody.empty();

                if (Array.isArray(response.data) && response.data.length > 1) {

                    let paymentsMade = 0;
                    let invoiceAmount = response.data[0].InvoiceAmount;

                    response.data.forEach(item => {
                        paymentsMade += parseFloat(item.QbCheckAmount);

                        let timestamp = parseInt(item.QbPaymentDate.match(/\d+/)[0]);
                        let date = new Date(timestamp);
                        let formattedDate = (date.getMonth() + 1).toString().padStart(2, '0') + '/' +
                            date.getDate().toString().padStart(2, '0') + '/' +
                            date.getFullYear();

                        let accountPaidText = item.QbAccountPaidName;
                        if (item.QbCheckNo && item.QbCheckNo.trim() !== "") {
                            accountPaidText += " | #" + item.QbCheckNo;
                        }

                        let paymentDetails = `
                            <div class="invpayment-entry">
                                <p><strong>Payment Date:</strong> <span>${formattedDate}</span></p>
                                <p><strong>Paid From:</strong> <span>${accountPaidText}</span></p>
                                <p><strong>Paid Amount:</strong> <span>$${item.QbCheckAmount}</span></p>
                                <p><strong>Payment Method:</strong> <span>${item.QbPaymentMethod}</span></p>
                            </div>
                        `;
                        modalBody.append(paymentDetails);
                    });

                    let balanceDue = invoiceAmount - paymentsMade;
                    let summaryDetails = `
                        <hr>
                        <p><strong>Payments Made:</strong> <span>$${paymentsMade.toFixed(2)}</span></p>
                        <p><strong>Balance Due:</strong> <span>$${balanceDue.toFixed(2)}</span></p>
                        <p><strong>Invoice Amount:</strong> <span>$${invoiceAmount.toFixed(2)}</span></p>
                    `;
                    modalBody.append(summaryDetails);
                } else {
                    let item = response;
                    let timestamp = parseInt(item.data[0].QbPaymentDate.match(/\d+/)[0]);
                    let date = new Date(timestamp);
                    let formattedDate = (date.getMonth() + 1).toString().padStart(2, '0') + '/' +
                        date.getDate().toString().padStart(2, '0') + '/' +
                        date.getFullYear();

                    let accountPaidText = item.data[0].QbAccountPaidName;
                    if (item.data[0].QbCheckNo && item.data[0].QbCheckNo.trim() !== "") {
                        accountPaidText += " | #" + item.data[0].QbCheckNo;
                    }

                    let singlePaymentDetails = `
                        <p><strong>Payment Date:</strong> <span class="qbpaydatebnd">${formattedDate}</span></p>
                        <p><strong>Invoice Amount:</strong> <span class="qbamountbnd">$${item.data[0].InvoiceAmount}</span></p>
                        <p><strong>Paid From:</strong> <span class="qbaccountnamebnd">${accountPaidText}</span></p>
                        <p><strong>Paid Amount:</strong> <span class="qbcheckamountbnd">$${item.data[0].QbTotalAmount}</span></p>
                        <p><strong>Payment Method:</strong> <span class="qbpaymentbnd">${item.data[0].QbPaymentMethod}</span></p>
                    `;
                    modalBody.append(singlePaymentDetails);
                }
            } else {
                $("#QbPaidModal .modal-body").text("No Payment Data Found").css('font-size', '15px');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}


function PaidStatusFilterCheck(e) {
    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var span;
    span = document.createElement('span');
    if (e.IsPaid === true) {
        span.textContent = "Paid"
    }
    else {
        span = document.createElement('span');
        span.textContent = "UnPaid"
    }
    div.appendChild(span);
    return div.outerHTML;
}

//function refreshGridData() {
//    var gridObj = document.getElementById('InlineInvoiceEditing').ej2_instances[0];
//    gridObj.refresh();
//}

//setInterval(refreshGridData, 50000);

//document.addEventListener('DOMContentLoaded', function () {
//    refreshGridData(); // Initial load
//});

//var invoicefilteredData = [];

// Fetch designation data using AJAX
//function fetchInvoiceFilteredData() {
//    var invoiceDataManager = new ej.data.DataManager({
//        url: '/Invoices/GetInvoiceFilterdData',
//        adaptor: new ej.data.UrlAdaptor(),
//    });
//    invoiceDataManager.executeQuery(new ej.data.Query()).then(function (data) {
//        invoicefilteredData = data.result.map((item) => ({
//            VendorName: item.VendorName,
//            InvoiceType: item.InvoiceType,
//            InvoiceNumber: item.InvoiceNumber,
//            StoreName: item.StoreName,
//            InvoiceDate: item.InvoiceDate,
//            TotalAmount: item.TotalAmount,
//            PaymentType: item.PaymentType,
//        }));
//    });

//}

//fetchInvoiceFilteredData();

//async function fetchInvoiceFilteredData() {
//    try {
//        var invoiceDataManager = new ej.data.DataManager({
//            url: '/Invoices/GetInvoiceFilterdData',
//            adaptor: new ej.data.UrlAdaptor(),
//        });

//        // Execute the query and wait for the result
//        var data = await invoiceDataManager.executeQuery(new ej.data.Query());

//        // Process the data
//        invoicefilteredData = data.result.map((item) => ({
//            VendorName: item.VendorName,
//            InvoiceType: item.InvoiceType,
//            InvoiceNumber: item.InvoiceNumber,
//            StoreName: item.StoreName,
//            InvoiceDate: item.InvoiceDate,
//            TotalAmount: item.TotalAmount,
//            PaymentType: item.PaymentType,
//        }));

//        console.log(invoicefilteredData); // Log or handle the filtered data
//    } catch (error) {
//        console.error("Error fetching invoice data:", error);
//    }
//}

//// Call the function
//(async () => {
//    await fetchInvoiceFilteredData();
//})();


function actionBegin(args) {
    if (args.requestType == "beginEdit") {
        scrollVal = $(window).scrollTop();
    }
    //if (args.requestType === 'filterbeforeopen') {
    //    args.filterModel.options.dataSource = invoicefilteredData;
    //    args.filterModel.options.filteredColumns = args.filterModel.options.filteredColumns.filter(function (col) {
    //        if (col.field == 'VendorName') {
    //            return true;
    //        }
    //        if (col.field == 'InvoiceType') {
    //            return true;
    //        }
    //        if (col.field == 'InvoiceNumber') {
    //            return true;
    //        }
    //        if (col.field == 'StoreName') {
    //            return true;
    //        }
    //        if (col.field == 'InvoiceDate') {
    //            return true;
    //        }
    //        if (col.field == 'TotalAmount') {
    //            return true;
    //        }
    //        if (col.field == 'PaymentType') {
    //            return true;
    //        }
    //        return false;
    //    });
    //}
}

//function actionBegin(args) {
//    if (args.requestType == "beginEdit") {
//        scrollVal = $(window).scrollTop();
//    }
//    if (args.requestType === "filterbeforeopen") {
//        var ajax = new ej.base.Ajax({
//            url: ROOTURL + 'Invoices/GetInvoiceFilterdData', //render the partial view
//            type: "POST",
//            contentType: "application/json",
//            data: JSON.stringify({ columnName : args.columnName })
//        });
//        ajax.send().then(function (data) {
//            args.filterModel.options.dataSource = [
//                { FirstName: "Test1" },
//                { FirstName: "Test2" },
//                { FirstName: "Test3" },
//                { FirstName: "Test4" }
//            ]
//            /*appendElement(data, args.form); //render the edit form with selected record*/
//            /*ej.popups.hideSpinner(args.dialog.element);*/
//        }).catch(function (xhr) {
//            console.log(xhr);
//            /*ej.popups.hideSpinner(args.dialog.element);*/
//        });
//        //args.filterModel.options.dataSource = [
//        //    { FirstName: "Test1" },
//        //    { FirstName: "Test2" },
//        //    { FirstName: "Test3" },
//        //    { FirstName: "Test4" }
//        //]
//    }
//}