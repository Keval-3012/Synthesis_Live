

function populatedata() {
    
    var Month = document.getElementById('txtstartdate').value;
    var InvoiceType = $("#drpInvoiceType").val();

    $.ajax({
        url: '/Invoices/GetInvoiceCountData',
        type: 'GET',
        data: { Date: Month, InvoiceType: InvoiceType },
        success: function (data) {
            var tbody = $('.tablecolor tbody');
            if (data && data.length > 0 && data != "[]") {
                tbody.empty(); 

                $.each(JSON.parse(data), function (index, item) {
                    tbody.append('<tr>' +
                        '<td style="text-align:left;width:15%">' + item.StoreName + '</td>' +
                        '<td style="text-align:center;width:10%">' + item.TotalCount + '</td>' +
                        '</tr>');
                });
                $('.no-data-message').hide();
            } else {
                
                tbody.empty(); 
                $('.no-data-message').show();
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error occurred while fetching data:', errorThrown);
        }
    });

}
$('#exportPdfButton').on('click', function () {
    var Month = document.getElementById('txtstartdate').value;
    var InvoiceType = $("#drpInvoiceType").val();
    if (Month != "") {
        location.href = '/Invoices/DownloadInvoiceCountPDF?Date=' + Month + '&InvoiceType=' + InvoiceType;
    }
});

$('#exportExcelButton').on('click', function () {
    var Month = document.getElementById('txtstartdate').value;
    var InvoiceType = $("#drpInvoiceType").val();
    if (Month != "") {
        location.href = '/Invoices/DownloadInvoiceCountExcel?Date=' + Month + '&InvoiceType=' + InvoiceType;
    }
});