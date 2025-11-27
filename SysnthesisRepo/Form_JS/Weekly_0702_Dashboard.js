$(document).ready(function () {

    var startdate = Startdate;
    $('#DailyChartDate').val(startdate);

    $('#DailyChartDate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#DailyChartDate').attr('autocomplete', 'off');

    GetWeeklyData($('#WeeklyPeriodId').val());
    GetWeeklyComparisionData();
    GetWeeklyComparisionDataCCount();
    GetWeeklyComparisionDataAverage();
    GetWeeklyDoughnutChart(); 
    $(".drpcls option").each(function () {
        if ($(this).val() == "0") {
            $(this).attr('disabled', 'disabled');
        }
    });
});

$(function () {
    $('#DailyChartDate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#WeeklyPeriodId').on('change', function (e) {

        var WeeklyPeriodId = $(this).val();
        GetWeeklyData(WeeklyPeriodId);
        GetWeeklyComparisionData();
        GetWeeklyComparisionDataCCount();
        GetWeeklyComparisionDataAverage();
        GetWeeklyDoughnutChart();
        $(".other-deposite").hide();
    });
});
//clsdailychartstores


function GetWeeklyData(WeeklyPeriodId) {
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/getDashboardWeeklyData',
        data: { WeeklyPeriodId: WeeklyPeriodId },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {

            GetChartDetails();
            GetHourlyChartDetails();
            GetSalesOneHourWorkedData(WeeklyPeriodId);
            GetPayrollSalesBoxesData();

            if (response.AllVoidsAmt < 100000) {
                $("#AllVoidsAmt").text("$ " + response.AllVoidsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            } else {
                $("#AllVoidsAmt").text("#####");
            }

            if (response.ItemCorrectsAmt < 100000) {
                $("#ItemCorrectsAmt").text("$ " + response.ItemCorrectsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            } else {
                $("#ItemCorrectsAmt").text("#####");
            }

            if (response.ItemReturnsAmt < 100000) {
                $("#ItemReturnsAmt").text("$ " + response.ItemReturnsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            } else {
                $("#ItemReturnsAmt").text("#####");
            }
            
            $("#AllVoids").text(response.AllVoids.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
            $("#ItemCorrects").text(response.ItemCorrects.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
            $("#ItemReturns").text(response.ItemReturns.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
            $("#CashPayout").text("$ " + response.CashPayout.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            if (response.Over < 0) {
                $("#Over").css('color', 'red');
            } else if (response.Over > 0) {
                $("#Over").css('color', 'green');
            }
            else {
                $("#Over").css('color', 'gray');
            }
            $("#Over").text("$ " + response.Over.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#TotalSalesCurrentDay").text("$ " + response.TotalSalesCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#SalesGrowth").text(response.SalesGrowth);
            $("#CustomerCountCurrentDay").text(response.CustomerCountCurrentDay.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
            $("#CustomerCountGrowth").text(response.CustomerCountGrowth);
            $("#AverageSaleCurrentDay").text("$ " + response.AverageSaleCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#AveragesalesGrowth").text(response.AveragesalesGrowth);
            $("#TotalSalesLastWeek").text("$ " + response.TotalSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#CustomerCountLastWeek").text(response.CustomerCountLastWeek.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
            $("#AverageSalesLastWeek").text("$ " + response.AverageSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#Offline").text(response.Offline + "%");
            $("#Online").text(response.Online + "%");
            $("#InvoicesAdded").text(response.InvoicesAdded);
            $("#InvoicesApproved").text(response.InvoicesApproved);
            
        }
    });
}


function GetSalesOneHourWorkedData(WeeklyPeriodId) {
    var Departments = "";
    $(".clshourlychartdepartments").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });

    //checkedId = checkedId.slice(0, -1);
    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetSalesOneHourWorkedDataWeekly',
        data: { WeeklyPeriodId: WeeklyPeriodId, department: Departments },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                $(".departmentWiseSalesWidgets").html("");
                var htm = "";
                $.each(response, function (j, data) {
                    htm += "<div class='clsstr'><span>" + data.StoreName + "</span><span>$ " + data.Amount + "</span></div>";
                })
                $(htm).appendTo($('.departmentWiseSalesWidgets'));
            }
            //Loader(0);
        }
    });
}


function GetWeeklySalesTotalDetailsdata() {
    var checkedId = "";
    var radioId = "1";
    var Departments = "";
    $(".clsdailychartstores").each(function (index, obj) {

        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });

    $(".clsRadio").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();

        }
    });

    $(".clsdailychartdepartments").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });

    checkedId = checkedId.slice(0, -1);

    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    var div = document.getElementsByClassName("clsTotalDetail");
    var Length = div.length;
    for (var i = 0; i < Length; i++) {
        div[0].remove();
    }
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();

    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetWeeklySalesTotalDetailsdata',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                var htm = "";
                var htmDept = "";

                htm += "<h2 class='clsTotalDetail'>Stores</h2>";
                htmDept += "<h2 class='clsTotalDetail'>Departments</h2>";
                htmDept += "<ul class='clsTotalDetail'>";
                var CurSign = "";
                var Decimalval = 0;
                if (radioId != "2") {
                    CurSign = "$";
                    Decimalval = 2;
                }
                $.each(response, function (j, data) {
                    if (data.Type == "S") {

                        if (data.StoreId == '1') {
                            htm += "<span class='store74-84 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        } else if (data.StoreId == '5') {
                            htm += "<span class='store2589 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '6') {
                            htm += "<span class='store14Street clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '2') {
                            htm += "<span class='storeMayWood clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '7') {
                            htm += "<span class='store2840 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '3') {
                            htm += "<span class='store1407 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '4') {
                            htm += "<span class='store180 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                        else if (data.StoreId == '9') {
                            htm += "<span class='store170 clsTotalDetail'>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span>";
                        }
                    } else if (data.Type == "D") {
                        htmDept += "<li><span > " + data.StoreId + "</span> <span>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></li>";
                    }
                })
                htmDept += "</ul>";
                $(htm).appendTo($('#dvTotalStore'));
                $(htmDept).appendTo($('#dvTotalDepartment'));
            }
            GetWeeklySalesDailyTotalDetailsdata();
            //Loader(0);
        }
    });
}

function GetWeeklySalesDailyTotalDetailsdata() {
    var checkedId = "";
    var Departments = "";


    $(".clsdailychartdepartments1").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });

    checkedId = checkedId.slice(0, -1);

    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    var div = document.getElementsByClassName("clsTotalDetail1");
    var Length = div.length;
    for (var i = 0; i < Length; i++) {
        div[0].remove();
    }
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();

    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetWeeklySalesDailyTotalDetailsdata',
        data: { StoreId: 0, Department: Departments, Type: 4, WeeklyPeriodId: WeeklyPeriodId },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                var htm = "";
                var htmDept = "";

                htm += "<h2 class='clsTotalDetail1'>Purchase</h2>";
                htm += "<ul class='clsTotalDetail1'>";
                htmDept += "<h2 class='clsTotalDetail1'>Payroll</h2>";
                htmDept += "<ul class='clsTotalDetail1'>";
                var CurSign = "";
                var Decimalval = 0;

                $.each(response, function (j, data) {
                    if (data.Type == "C") {

                        htm += "<li><span > " + data.StoreId + "</span> <span>" + data.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></li>";
                    } else if (data.Type == "P") {
                        htmDept += "<li><span > " + data.StoreId + "</span> <span>" + data.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></li>";
                    }
                })
                htmDept += "</ul>";
                htm += "</ul>";
                $(htm).appendTo($('#dvTotalStore2'));
                $(htmDept).appendTo($('#dvTotalDepartment2'));
            }
            //Loader(0);
        }
    });
}

$(".clsdailychartstores").change(function () {
    GetChartDetails();

})
$(".clsRadio").change(function () {
    GetChartDetails();

})
//$(".clsRadioWC").change(function () {
//    //GetWeeklyComparisionBarChart();
//    GetWeeklyComparisionData();
//})
$(".clsdailychartdepartments").change(function () {
    GetChartDetails();
})
$(".clsdailychartdepartments1").change(function () {
    GetWeeklyDoughnutChart();
    GetWeeklySalesDailyTotalDetailsdata();
    GetPayrollSalesBoxesData();

})

$(".clshourlychartdepartments").change(function () {

    GetHourlyChartDetails();
    GetSalesOneHourWorkedData($('#WeeklyPeriodId').val());

})

var charts;
function GetChartDetails() {
    GetWeeklySalesTotalDetailsdata();
    //$("#bar-chartcanvas").load('~/Dashboard/GetChartData');
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var checkedId = "";
    var radioId = "1";
    var Departments = "";
    $(".clsdailychartstores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });

    $(".clsRadio").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();
        }
    });

    $(".clsdailychartdepartments").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });
    if (radioId == "1") {
        $('#graphtitle').text("Daily Sales Graph");
        $("input.clsdailychartdepartments").removeAttr("disabled");
    } else if (radioId == "2") {
        $('#graphtitle').text("Customer Count Graph");
        $("input.clsdailychartdepartments").attr("disabled", true);
    } else {
        $('#graphtitle').text("Average Sale Graph");
        $("input.clsdailychartdepartments").attr("disabled", true);
    }
    checkedId = checkedId.slice(0, -1);
    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    var ctx = $("#bar-chartcanvas");

    var options = {
        maintainAspectRatio: false,
        responsive: true,
        title: {
            display: true,
            position: "top",
            text: "",
            fontSize: 18,
            fontColor: "#000"
        },
        tooltips: {
            callbacks: {
                title: function (t, d) {
                    return d.labels[t[0].index];
                },
                label: function (context, lbldata) {
                    var arr = [];
                    var StoreCode = lbldata.datasets[context.datasetIndex].code;
                    $.ajax({
                        async: false,
                        type: "POST",
                        url: '/Dashboard/getToolTipData',
                        data: { StoreId: StoreCode, Department: Departments, Type: radioId, Date: context.xLabel, Mode: "W" },
                        //contentType: "application/json",
                        //dataType: "json",
                        success: function (chData) {
                            aa = chData;
                            var strDept = "";
                            $.each(aa, function (index, obje) {
                                strDept = "";
                                if (radioId != "2") {
                                    strDept = obje.Department + " - " + " $" + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                } else {
                                    strDept = obje.Department + " - " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                    strDept = obje.Department + " : " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                }
                                arr.push(strDept);
                            })
                        }
                    });

                    //arr.push("----------------------");
                    if (radioId != "2") {
                        arr.push("Total : " + new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(context.yLabel));
                    } else {
                        arr.push("Total : " + context.yLabel.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
                    }
                    return arr;
                }
            }
        },
        legend: {
            display: false,
        },
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true,
                    min: 0,
                    callback: function (value, index, values) {
                        if (parseInt(value) >= 1000) {
                            if (radioId != "2") {
                                return '$' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            } else {
                                return value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", "");
                            }
                        } else {
                            if (radioId != "2") {
                                return '$' + value.toFixed(2);
                            } else {
                                return value.toFixed(0);
                            }
                        }
                    }
                },
            }],
            //xAxes: [{
            //    ticks: {
            //        beginAtZero: true,
            //        min: 0
            //    }
            //}]
        }
    };
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    if (chart2) {
        chart2.destroy();
    }
    if (charts) {
        charts.destroy();
    }
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetWeeklyChartData',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData) {
            var aData = chData;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetLists"];
            var Total = aData["Total"];
            if (radioId == "3") {
                //Total = Total / 24;
                //$("#graphTotal").text("$" + Total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $(".totals").hide();
            }
            else if (radioId == "1") {
                $(".totals").show();
                $("#graphTotal").text("$" + Total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            }
            else {
                $(".totals").show();
                $("#graphTotal").text(Total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            }

            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            if (charts != undefined)
                charts.destroy();

            charts = new Chart(ctx, {
                type: "line",
                data: data,
                options: options
            });

        }
    });
    //GetWeeklyDoughnutChart();

    //var ctx1 = $("#bar-chartcanvasDoughnut");
    //var ctxv2 = $("#bar-chartcanvasDoughnut2");
    //var ctxv3 = $("#bar-chartcanvasDoughnut3");

    //var options1 = {
    //    title: {
    //        display: true,
    //        position: "top",
    //        text: "",
    //        fontSize: 18,
    //        fontColor: "#000"
    //    },
    //    legend: {
    //        display: false,
    //    },

    //};
    //$.ajax({

    //    type: "POST",
    //    url: '/Dashboard/GetChartDataDoughnut',
    //    data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 1 },
    //    //contentType: "application/json",
    //    //dataType: "json",
    //    success: function (chData) {

    //        var aData = chData;
    //        var aLabels = aData["Labels"];
    //        var aDatasets1 = aData["DatsetListDoughnutLists"];
    //        var data = {
    //            labels: aLabels,
    //            datasets: aDatasets1,
    //        };

    //        var chart = new Chart(ctx1, {
    //            type: "doughnut",
    //            data: data,
    //            options: options1
    //        });
    //    }
    //});


    //$.ajax({

    //    type: "POST",
    //    url: '/Dashboard/GetChartDataDoughnut',
    //    data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 2 },
    //    //contentType: "application/json",
    //    //dataType: "json",
    //    success: function (chData) {

    //        var aData = chData;
    //        var aLabels = aData["Labels"];
    //        var aDatasets1 = aData["DatsetListDoughnutLists"];
    //        var data = {
    //            labels: aLabels,
    //            datasets: aDatasets1,
    //        };

    //        var chart = new Chart(ctxv2, {
    //            type: "doughnut",
    //            data: data,
    //            options: options1
    //        });

    //    }
    //});

    //$.ajax({

    //    type: "POST",
    //    url: '/Dashboard/GetChartDataDoughnut',
    //    data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 3 },
    //    //contentType: "application/json",
    //    //dataType: "json",
    //    success: function (chData) {

    //        var aData = chData;
    //        var aLabels = aData["Labels"];
    //        var aDatasets1 = aData["DatsetListDoughnutLists"];
    //        var data = {
    //            labels: aLabels,
    //            datasets: aDatasets1,
    //        };

    //        var chart3 = new Chart(ctxv3, {
    //            type: "doughnut",
    //            data: data,
    //            options: options1
    //        });
    //    }
    //});

    var chartdatabar;
    var ctx3 = $("#bar-chartcanvasbar");
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetChartDataBar',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, Date: Date },
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

            if (chartdatabar != undefined)
                chartdatabar.destroy();

            chartdatabar = new Chart(ctx3, {
                type: "bar",
                data: data,
                //options: options
            });
        }
    });
};

//Doughnut Chart Data
function GetWeeklyDoughnutChart() {
    $("#Doughnut").html("");
    $("#Doughnut").append('<canvas id="bar-chartcanvasDoughnut"></canvas>');
    $("#Doughnut2").html("");
    $("#Doughnut2").append('<canvas id="bar-chartcanvasDoughnut2"></canvas>');
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var Departments = "";
    var checkedId = "1";
    $(".clsdailychartdepartments1").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });
    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }


    var ctx1 = $("#bar-chartcanvasDoughnut");
    var ctxv2 = $("#bar-chartcanvasDoughnut2");
    var ctxv3 = $("#bar-chartcanvasDoughnut3");
    var chart;
    var chart2;

    var options1 = {
        responsive: true,
        maintainAspectRatio: true,
        title: {
            display: true,
            position: "top",
            text: "",
            fontSize: 18,
            fontColor: "#000"
        },
        legend: {
            display: false,
        },
    };
    //if (Departments != "ALL") {
    //    chart.destroy();
    //    chart2.destroy();
    //}
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetChartDataDoughnut',
        data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 1 },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData) {
            var aData = chData;
            $("#PurchaseToSales").text(aData["Labels"][0] + "%");
            $("#PurchaseToSales2").text(aData["Labels"][2] + "%");

            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetListDoughnutLists"];
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };
            console.log(data);

            if (chart != undefined)
                chart.destroy();

            chart = new Chart(ctx1, {
                type: "doughnut",
                data: data,
                options: options1
            });

            chart.data.labels.splice(2, 1);
            chart.data.datasets[0].data.splice(2, 1);
            chart.update();
            // chart.destroy();
        }
    });


    $.ajax({

        type: "POST",
        url: '/Dashboard/GetChartDataDoughnut',
        data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 2 },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData) {
            var aData = chData;
            //$("#PayrollToToSales").text("Current Week :" + aData["Labels"][0] + "% , Last Week :" + aData["Labels"][2] + "%");
            $("#PayrollToToSales").text(aData["Labels"][0] + "%");
            $("#PayrollToToSales2").text(aData["Labels"][2] + "%");

            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetListDoughnutLists"];
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            if (chart2 != undefined)
                chart2.destroy();

            chart2 = new Chart(ctxv2, {
                type: "doughnut",
                data: data,
                options: options1
            });

            chart2.data.labels.splice(2, 1);
            chart2.data.datasets[0].data.splice(2, 1);
            chart2.update();
        }
    });

    //$.ajax({

    //    type: "POST",
    //    url: '/Dashboard/GetChartDataDoughnut',
    //    data: { StoreId: checkedId, Department: Departments, WeekNo: WeeklyPeriodId, Type: 3 },
    //    //contentType: "application/json",
    //    //dataType: "json",
    //    success: function (chData) {

    //        var aData = chData;
    //        var aLabels = aData["Labels"];
    //        var aDatasets1 = aData["DatsetListDoughnutLists"];
    //        var data = {
    //            labels: aLabels,
    //            datasets: aDatasets1,
    //        };

    //        var chart3 = new Chart(ctxv3, {
    //            type: "doughnut",
    //            data: data,
    //            options: options1
    //        });
    //    }
    //});
    //---------------------------------

}
var chart2;
function GetWeeklyComparisionBarChart() {

    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var checkedId = "";
    var radioId = "5";

    $(".clsdailychartstores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });
    $(".clsRadioWC").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();
        }
    });
    checkedId = checkedId.slice(0, -1);
    var options = {
        maintainAspectRatio: false,
        responsive: true,
        //tooltips: { enabled: false },
        //hover: { mode: null },
        title: {
            display: true,
            position: "top",
            text: "",
            fontSize: 18,
            fontColor: "#000"
        },
        legend: {
            display: true,
        },
        scales: {
            yAxes: [{
                ticks: {
                    min: 0,
                    callback: function (value, index, values) {
                        if (parseInt(value) >= 1000) {
                            if (radioId != "6") {
                                return '$' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            } else {
                                return value.toFixed(0).toString();
                            }
                        } else {
                            if (radioId != "6") {
                                return '$' + value.toFixed(2);
                            } else {
                                return value.toFixed(0);
                            }
                        }
                    }
                },
            }]
        }
    };

    //Bar Chart Data

    var ctxH2 = $("#bar-chartcanvasH2");

    if (chart2) {
        chart2.destroy();
    }
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetWeeklyBarChartData',
        data: { StoreId: checkedId, Department: "ALL", Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData2) {
            var aData = chData2;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetLists"];
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            if (chart2 != undefined)
                chart2.destroy();

            chart2 = new Chart(ctxH2, {
                type: "bar",
                data: data,
                options: options
            });
        }
    });
    //---------------------------------

}

function GetWeeklyComparisionData() {
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var checkedId = "";
    var radioId = "5";

    $(".clsdailychartstores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });
    $(".clsRadioWC").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();
        }
    });
    checkedId = checkedId.slice(0, -1);
    var Sign = "$";
    if (radioId == "6") {
        Sign = "";
    }
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetWeeklyComparisionData',
        data: { StoreId: checkedId, Department: "ALL", Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",

        success: function (wcdata) {
            //WCSalesAmt, WCSalesSameWKLYearPer, WCSalesCurYearPer
            $("#WCSalesAmt").text(Sign + wcdata.WCCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#WCLastWeekAmt").text(Sign + wcdata.WCLastWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#WCSameWeekLastYearAmt").text(Sign + wcdata.WCLYearCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#WCCurrentYearAvergaeAmt").text(Sign + wcdata.WCAWeekThisYearAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#WCLastWeekPer").text(wcdata.WCCurWeekPer.toFixed(2) + "%");
            $("#WCSalesSameWKLYearPer").text(wcdata.WCLYearCurWeekPer.toFixed(2) + "%");
            $("#WCSalesCurYearPer").text(wcdata.WCAWeekThisYearPer.toFixed(2) + "%");
            //$("#WCCurWeekAmt").text(Sign + wcdata.WCCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            if (wcdata.WCCurWeekPer < 0) {
                $("#clsvalup0").removeClass('valueUpCell');
                $("#clsvalup0").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup0").removeClass('valueDownCell');
                $("#clsvalup0").addClass('valueUpCell');
            }


            if (wcdata.WCLYearCurWeekPer < 0) {
                $("#clsvalup1").removeClass('valueUpCell');
                $("#clsvalup1").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup1").removeClass('valueDownCell');
                $("#clsvalup1").addClass('valueUpCell');
            }
            //$("#WCAWeekThisYearAmt").text(Sign + wcdata.WCAWeekThisYearAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            //$("#WCAWeekThisYearPer").text(wcdata.WCAWeekThisYearPer.toFixed(2) + "%");

            if (wcdata.WCAWeekThisYearPer < 0) {
                $("#clsvalup2").removeClass('valueUpCell');
                $("#clsvalup2").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup2").removeClass('valueDownCell');
                $("#clsvalup2").addClass('valueUpCell');
            }

        }
    });
    //---------------------------------

}

function GetWeeklyComparisionDataCCount() {

    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var checkedId = "";
    var radioId = "6";

    $(".clsdailychartstores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });
    $(".clsRadioWC").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();
        }
    });
    checkedId = checkedId.slice(0, -1);
    var Sign = "$";
    if (radioId == "6") {
        Sign = "";
    }
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetWeeklyComparisionData',
        data: { StoreId: checkedId, Department: "ALL", Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",

        success: function (wcdata) {
            $("#WCCCount").text(Sign + wcdata.WCCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));

            $("#WCCCountLastWeekAmt").text(Sign + wcdata.WCLastWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#WCCCountSameWeekLastYearAmt").text(Sign + wcdata.WCLYearCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#WCCCountCurrentYearAvergaeAmt").text(Sign + wcdata.WCAWeekThisYearAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));

            $("#WCCCountLastWeekPer").text(wcdata.WCCurWeekPer.toFixed(2) + "%");
            $("#WCCCountSameWKLYearPer").text(wcdata.WCLYearCurWeekPer.toFixed(2) + "%");
            $("#WCCCountCurYearPer").text(wcdata.WCAWeekThisYearPer.toFixed(2) + "%");

            if (wcdata.WCCurWeekPer < 0) {
                $("#clsvalup01").removeClass('valueUpCell');
                $("#clsvalup01").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup01").removeClass('valueDownCell');
                $("#clsvalup01").addClass('valueUpCell');
            }

            if (wcdata.WCLYearCurWeekPer < 0) {
                $("#clsvalup3").removeClass('valueUpCell');
                $("#clsvalup3").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup3").removeClass('valueDownCell');
                $("#clsvalup3").addClass('valueUpCell');
            }

            if (wcdata.WCAWeekThisYearPer < 0) {
                $("#clsvalup4").removeClass('valueUpCell');
                $("#clsvalup4").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup4").removeClass('valueDownCell');
                $("#clsvalup4").addClass('valueUpCell');
            }
        }
    });
    //---------------------------------

}
//WCAverageAmt, WCAverageSameWKLYearPer, WCAverageCurYearPer
function GetWeeklyComparisionDataAverage() {
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var checkedId = "";
    var radioId = "7";

    $(".clsdailychartstores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            checkedId += $(obj).val() + ",";
        }
    });
    $(".clsRadioWC").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            radioId = $(obj).val();
        }
    });
    checkedId = checkedId.slice(0, -1);
    var Sign = "$";
    if (radioId == "6") {
        Sign = "";
    }
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetWeeklyComparisionData',
        data: { StoreId: checkedId, Department: "ALL", Type: radioId, WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",

        success: function (wcdata) {
            $("#WCAverageAmt").text(Sign + wcdata.WCCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#WCAverageLastWeekAmt").text(Sign + wcdata.WCLastWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#WCAverageSameWKLYearAmt").text(Sign + wcdata.WCLYearCurWeekAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#WCAverageCurYearAmt").text(Sign + wcdata.WCAWeekThisYearAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#WCAverageLastWeekPer").text(wcdata.WCCurWeekPer.toFixed(2) + "%");
            $("#WCAverageSameWKLYearPer").text(wcdata.WCLYearCurWeekPer.toFixed(2) + "%");
            $("#WCAverageCurYearPer").text(wcdata.WCAWeekThisYearPer.toFixed(2) + "%");

            if (wcdata.WCCurWeekPer < 0) {
                $("#clsvalupAvg0").removeClass('valueUpCell');
                $("#clsvalupAvg0").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalupAvg0").removeClass('valueDownCell');
                $("#clsvalupAvg0").addClass('valueUpCell');
            }

            if (wcdata.WCLYearCurWeekPer < 0) {
                $("#clsvalup5").removeClass('valueUpCell');
                $("#clsvalup5").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup5").removeClass('valueDownCell');
                $("#clsvalup5").addClass('valueUpCell');
            }

            if (wcdata.WCAWeekThisYearPer < 0) {
                $("#clsvalup6").removeClass('valueUpCell');
                $("#clsvalup6").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup6").removeClass('valueDownCell');
                $("#clsvalup6").addClass('valueUpCell');
            }
        }
    });
    //---------------------------------

}



function GetHourlyChartDetails() {
    //$("#bar-chartcanvas").load('~/Dashboard/GetChartData');

    var checkedId = "";
    var radioId = "1";
    var Departments = "";
    //$(".clsdailychartstores").each(function (index, obj) {
    //    if ($(obj).is(':checked') == true) {

    //        checkedId += $(obj).val() + ",";
    //    }
    //});

    //$(".clsRadio").each(function (index, obj) {
    //    if ($(obj).is(':checked') == true) {

    //        radioId = $(obj).val();
    //    }
    //});

    $(".clshourlychartdepartments").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });

    checkedId = checkedId.slice(0, -1);
    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    var ctx = $("#bar-chartcanvas2");

    var options = {
        responsive: true,
        maintainAspectRatio: false,
        //title: {
        //    display: true,
        //    position: "top",
        //    text: "Multicolor Bubble Graph",
        //    fontSize: 18,
        //    fontColor: "#111"
        //},
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Multicolor Bubble Graph'
            }
        },
        //legend: {
        //    display: false,
        //},

        scales: {
            yAxes: [{
                ticks: {
                    min: 0,
                    beginAtZero: true,
                    callback: function (value, index, values) {
                        if (parseInt(value) >= 1000) {
                            return '$' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        } else {
                            return '$' + value.toFixed(2);
                        }
                    }
                }
            }],
            xAxes: [{
                type: 'linear',
                position: 'bottom',
                ticks: {
                    beginAtZero: true,
                    min: 0
                }
            }]
        }
    };
    //var Date = $('#DailyChartDate').val();
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();

    var HourlyChart;
    $.ajax({

        type: "POST",
        url: '/Dashboard/GetHourlyChartDataWeek',
        data: { StoreId: checkedId, Department: Departments, Type: "4", WeeklyPeriodId: WeeklyPeriodId },
        //contentType: "application/json",
        //dataType: "json",
        success: function (chData) {

            //var datasets = "";
            var aData = chData;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["HourlyDatsetList"];
            var data = {
                labels: aLabels[0],
                datasets: aDatasets1,
            };


            //var aData = chData;
            //var aLabels = aData["Labels"];
            //var aDatasets1 = null;
            //var datas = null;

            // Add data values to array
            // End Defining data
            var options = {

                responsive: true, // Instruct chart js to respond nicely.
                maintainAspectRatio: false, // Add to prevent default behaviour of full-width/height
                pointStyle: 'circle',
                radius: 7,
                borderWidth: 1.2,
                showLines: false,
                legend: {
                    display: false
                },
                tooltips: {
                    callbacks: {
                        title: function (t, d) {
                            return "Payroll Hours : " + t[0].xLabel;
                            //return ['Hours : ' + d.labels[t[0].index], 'another thing', 'and another one'];
                        },
                        label: function (context, lbldata) {
                            var arr = [];
                            var StoreCode = lbldata.datasets[context.datasetIndex].label;
                            $.ajax({
                                async: false,
                                type: "POST",
                                url: '/Dashboard/getWeeklyHourlyChartToolTipData',
                                data: { StoreId: StoreCode, Department: Departments, Type: "5", WeeklyPeriodId: WeeklyPeriodId },
                                //data: { StoreId: StoreCode, Department: Departments, Type: "4", Date: Date, Hours: context.xLabel },
                                //contentType: "application/json",
                                //dataType: "json",
                                success: function (chData) {
                                    aa = chData;
                                    var strDept = "";
                                    $.each(aa, function (index, obje) {
                                        strDept = "";
                                        //if (radioId != "2") {
                                        if (obje.Department != "") {
                                            strDept = obje.Department + " :  $" + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

                                            arr.push(strDept);
                                        }
                                    })
                                }
                            });

                            //arr.push("----------------------");
                            //if (radioId != "2") {
                            //    arr.push("Total : " + new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(context.yLabel));
                            //} else {
                            //    arr.push("Total : " + context.yLabel.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
                            //}
                            arr.push("Total : $" + context.yLabel.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
                            return arr;
                        }
                    }
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            callback: function (value, index, values) {
                                if (parseInt(value) >= 1000) {
                                    return '$' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                } else {
                                    return '$' + value.toFixed(2);
                                }
                            }
                        }
                    }],
                    xAxes: [{
                        ticks: {
                            beginAtZero: true,
                            min: 0
                        }
                    }]
                }
            };
            var ctxscatter = document.getElementById("bar-chartcanvas2").getContext('2d');
            // End Defining data

            if (HourlyChart != undefined)
                HourlyChart.destroy();

            HourlyChart = new Chart(ctxscatter, {

                type: 'scatter',
                data: data,
                options: options
            });
        }
    });
};

function closemodal() {
    $(".divIDClass").hide();
    //$(".overlaybox").hide();
}


//var HeaderStoreId = $("#HeaderStoreId option:selected").val();
//$(".clsdailychartstores").each(function (index, obj) {
//    var valu = $(obj).val();
//    if (HeaderStoreId == valu) {
//        $(obj).attr('checked', 'checked')
//    }
//    else{
//        $(obj).attr('checked', false)
//    }
//});

var HeaderStoreId = $("#HeaderStoreId option:selected").val();
if (HeaderStoreId == "") {
    HeaderStoreId = "0";
}
$(".clsdailychartstores").each(function (index, obj) {
    var valu = $(obj).val();

    if (HeaderStoreId == valu) {

        $(obj).attr('checked', 'checked')
    }
    else {
        $(obj).attr('checked', false)
    }


});



function GetPayrollSalesBoxesData() {
    var WeeklyPeriodId = $('#WeeklyPeriodId').val();
    var Departments = "";
    $(".clsdailychartdepartments1").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });
    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }

    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetPayrollSalesBoxesData',
        data: { WeeklyPeriodId: WeeklyPeriodId, Department: Departments },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            $("#PayrollOverTime").text(response.PayrollOverTime + "%");
            $("#PayrollSickPay").text(response.PayrollSickPay + "%");
            $("#PayrollVacation").text(response.PayrollVacation + "%");
            $("#PayrollHolidays").text(response.PayrollHolidays + "%");
            $("#PayrollBonus").text(response.PayrollBonus + "%");
            $("#PayrollRegularPay").text(response.PayrollRegularPay + "%");
            $("#SalesRegularPay").text(response.SalesRegularPay + "%");
            $("#SalesOverTime").text(response.SalesOverTime + "%");
            $("#SalesSalary").text(response.SalesSalary + "%");
            $("#SalesOtherpay").text(response.SalesOtherpay + "%");
        }
    });
}
