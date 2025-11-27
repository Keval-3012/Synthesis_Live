$(document).ready(function () {
    //$("#bar-chartcanvas").load('~/Dashboard/GetChartData');
    var ctx = $("#bar-chartcanvas");

    var options = {
        responsive: true,
        title: {
            display: false,
            position: "top",
            fontSize: 18,
            fontColor: "#111"
        },
        legend: {
            display: false,
        },
        scales: {
            yAxes: [{
                ticks: {
                    min: 0
                }
            }]
        },

    };
    var Date = $('#DailyChartDate').val();
    var HeaderStoreId = $("#HeaderStoreId option:selected").val();
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetChartData',
        data: { StoreId: HeaderStoreId, Department: 'ALL', Type:'1',Date: Date },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData) {
            var aData = chData;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetLists"];
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            var chart = new Chart(ctx, {
                type: "line",
                data: data,
                options: options
            });
        }
    });


});