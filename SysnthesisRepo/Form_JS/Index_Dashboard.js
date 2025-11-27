$(document).ready(function () {
    var startdate = Startdate;
    $('#Startdate').val(startdate);

    $('#Startdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#Startdate').attr('autocomplete', 'off');
});

$(function () {
    $('#Startdate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#Startdate').on('dp.change', function (e) {
        var date = $(this).val();
        GetData(date);
        $(".other-deposite").hide();
    });
});

function GetData(Currentdate) {
    $.ajax({
        method: "GET",
        url: ROOTURL + 'Dashboard/getDashboardDailyData',
        data: { date: Currentdate },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {

            $("#CustomerCount").text(response.CustomerCount);
            $("#AllVoids").text("$ " + response.AllVoids.toFixed(2));
            $("#ItemCorrects").text("$ " + response.ItemCorrects.toFixed(2));
            $("#ItemReturns").text("$ " + response.ItemReturns.toFixed(2));
            $("#TotalCash").text("$ " + response.TotalCash.toFixed(2));
            $("#AverageSale").text("$ " + response.AverageSale.toFixed(2));
            $("#CashPayout").text("$ " + response.CashPayout.toFixed(2));
            $("#Over").text("$ " + response.Over.toFixed(2));
            $("#TotalSalesCurrentDay").text("$ " + response.TotalSalesCurrentDay.toFixed(2));
            $("#TotalSalesLastWeek").text("$ " + response.TotalSalesLastWeek.toFixed(2));
            $("#SalesGrowth").text(response.SalesGrowth);
            $("#Offline").text(response.Offline);
            $("#Online").text(response.Online);
            $("#InvoicesAdded").text(response.InvoicesAdded);
            $("#InvoicesApproved").text(response.InvoicesApproved);
            //Loader(0);
        }
    });
}

function closemodal() {
    $(".divIDClass").hide();
    //$(".overlaybox").hide();
}