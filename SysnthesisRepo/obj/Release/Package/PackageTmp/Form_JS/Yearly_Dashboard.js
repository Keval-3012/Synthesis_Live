
$(document).ready(function () {
    $("#loading").show();
    GetYearlySales();
});
$(".YearlyDashboardDepartment").change(function () {

    if ($(this).is(':checked') == true) {
        if (this.value == "0") {

            $(".YearlyDashboardDepartment").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', true);
                } else {
                    $(this).prop('checked', true);
                }
            });
        }
        else {
            $(".YearlyDashboardDepartment").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetYearlySales();
    }
    else {
        if (this.value == "0") {

            $(".YearlyDashboardDepartment").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', false);
                } else {
                    $(this).prop('checked', false);
                }
            });
        }
        else {
            $(".YearlyDashboardDepartment").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetYearlySales();
    }

});

function formatDateWithSuffix(date) {
    const day = date.getDate();
    let suffix = 'th';

    if (day === 1 || day === 21 || day === 31) {
        suffix = 'st';
    } else if (day === 2 || day === 22) {
        suffix = 'nd';
    } else if (day === 3 || day === 23) {
        suffix = 'rd';
    } else if (day >= 4 && day <= 20 || day >= 24 && day <= 30) {
        suffix = 'th';
    }
    const month = date.toLocaleDateString('en-US', { month: 'short' });
    return `${month} ${day}${suffix}`;
}

var myChart;
    function GetYearlySales() {
        $("#loading").show();
        var Departments = "";
        var StateId = $('#StateId').val();
        var Year = $('#Year').val();
        var currentYear = new Date().getFullYear();
        var yearValue = parseInt(Year, 10);
        if (StateId == "" || StateId == undefined) {
            StateId = null;
        }

        if (Year == null || Year == undefined) {
            const d = new Date();
            Year = d.getFullYear();
        }

        $(".YearlyDashboardDepartment").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                Departments += $(obj).val() + ",";
            }
        });

        if (Departments != "") {
            Departments = Departments.slice(0, -1);
        } else {
            Departments = "ALL";
        }
        var currentDate = new Date();
        var formattedDate = formatDateWithSuffix(currentDate);

        $.ajax({
            method: "GET",
            url: ROOTURL + 'dashboard/GetYearlySalesData',
            data: { Departments: Departments, StateId: StateId, Year: Year },
            contentType: "application/json;",
            success: function (response) {
                if (yearValue !== currentYear) {
                    $("#Fys").text("Full Year Sales " + (Year - 1));
                    $("#Lyy").text((Year-1) + " Year " + " - " + formattedDate);
                    $("#Cy").text("Year " + Year);
                    $("#LastY").text((Year - 1) + " Year " + " - " + formattedDate);
                    $("#CureyearS").text(Year + " Sales");
                    $("#Lasty1").text((Year - 1) + " Year " + " - " + formattedDate);
                    $("#CureYearS1").text(Year + " Sales");
                }
                else {
                    $("#Fys").text("Full Year Sales " + (Year - 1));
                    $("#Lyy").text((Year - 1) + " Year " + " - " + formattedDate);
                    $("#Cy").text("Current Year");
                    $("#LastY").text((Year - 1) + " Year " + " - " + formattedDate);
                    $("#CureyearS").text("Current Year Sales");
                    $("#Lasty1").text((Year - 1) + " Year " + " - " + formattedDate);
                    $("#CureYearS1").text("Current Year Sales");
                }
                
                if (Departments == "ALL") {
                    /*  $('#ExpensesAmount').text("NASSS");*/
                    $('#ExpensesAmount').text("$ " + response.sales.ExpensesAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                }
                else {
                    $('#ExpensesAmount').text("NA");
                }
                $('#SalesAmount').text("$ " + response.sales.SalesAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#CogsAmount').text("$ " + response.sales.CogsAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#GrossMargin').text("$ " + response.sales.GrossMargin.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#PayrollAmount').text("$ " + response.sales.PayrollAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#ProfitAfterPayrollAmount').text("$ " + response.sales.ProfitAfterPayrollAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                /* $('#ExpensesAmount').text("$ " + response.sales.ExpensesAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));*/
                $('#NetProfitAmount').text("$ " + response.sales.NetProfitAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

                $('#SalesPercent').text(response.sales.SalesPercent + " %");
                $('#CogsPercent').text(response.sales.CogsPercent + " %");
                $('#GrossMarginPercent').text(response.sales.GrossMarginPercent + " %");
                $('#PayrollPercent').text(response.sales.PayrollPercent + " %");
                $('#ProfitAfterPayrollPercent').text(response.sales.ProfitAfterPayrollPercent + " %");
                $('#ExpensesPercent').text(response.sales.ExpensesPercent + " %");
                $('#NetProfitPercent').text(response.sales.NetProfitPercent + " %");

                $('#ExpensesAmountL').text("$ " + response.salesL.ExpensesAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#SalesAmountL').text("$ " + response.salesL.SalesAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#CogsAmountL').text("$ " + response.salesL.CogsAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#GrossMarginL').text("$ " + response.salesL.GrossMargin.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#PayrollAmountL').text("$ " + response.salesL.PayrollAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#ProfitAfterPayrollAmountL').text("$ " + response.salesL.ProfitAfterPayrollAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

                $('#NetProfitAmountL').text("$ " + response.salesL.NetProfitAmount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

                $('#SalesAmountYTD').text("$ " + response.sales.SalesAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#CogsAmountYTD').text("$ " + response.sales.CogsAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#GrossMarginYTD').text("$ " + response.sales.GrossMarginYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#PayrollAmountYTD').text("$ " + response.sales.PayrollAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#ProfitAfterPayrollAmountYTD').text("$ " + response.sales.ProfitAfterPayrollAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
                $('#ExpensesAmountYTD').text("$ " + response.sales.ExpensesAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

                $('#NetProfitAmountYTD').text("$ " + response.sales.NetProfitAmountYTD.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

                if (response.sales.SalesPercent < 0) {
                    $("#SalesPercent").removeClass('high');
                    $("#SalesPercent").addClass('low');
                } else {
                    $("#SalesPercent").removeClass('low');
                    $("#SalesPercent").addClass('high');
                }
                if (response.sales.CogsPercent < 0) {
                    $("#CogsPercent").removeClass('high');
                    $("#CogsPercent").addClass('low');
                } else {
                    $("#CogsPercent").removeClass('low');
                    $("#CogsPercent").addClass('high');
                }
                if (response.sales.GrossMarginPercent < 0) {
                    $("#GrossMarginPercent").removeClass('high');
                    $("#GrossMarginPercent").addClass('low');
                } else {
                    $("#GrossMarginPercent").removeClass('low');
                    $("#GrossMarginPercent").addClass('high');
                }
                if (response.sales.PayrollPercent < 0) {
                    $("#PayrollPercent").removeClass('high');
                    $("#PayrollPercent").addClass('low');
                } else {
                    $("#PayrollPercent").removeClass('low');
                    $("#PayrollPercent").addClass('high');
                }
                if (response.sales.ProfitAfterPayrollPercent < 0) {
                    $("#ProfitAfterPayrollPercent").removeClass('high');
                    $("#ProfitAfterPayrollPercent").addClass('low');
                } else {
                    $("#ProfitAfterPayrollPercent").removeClass('low');
                    $("#ProfitAfterPayrollPercent").addClass('high');
                }
                if (response.sales.ExpensesPercent < 0) {
                    $("#ExpensesPercent").removeClass('high');
                    $("#ExpensesPercent").addClass('low');
                } else {
                    $("#ExpensesPercent").removeClass('low');
                    $("#ExpensesPercent").addClass('high');
                }
                if (response.sales.NetProfitPercent < 0) {
                    $("#NetProfitPercent").removeClass('high');
                    $("#NetProfitPercent").addClass('low');
                } else {
                    $("#NetProfitPercent").removeClass('low');
                    $("#NetProfitPercent").addClass('high');
                }

                var htmlT = "";
                var htmlL = "";
                var totalvalue = 0;
                var totalvalueL = 0;
                $.each(response.growt, function (j, data) {
                    if (data.Type == "T") {
                        if (data.SalesWeeks == "Total") {
                            totalvalue = data.Sales.toFixed(2);
                        }
                        else if (data.SalesWeeks == "Total%") {
                            htmlT += "<ul class='grandTotal'>";
                            var salesAmount = parseFloat($('#SalesAmount').text().replace(/[^\d.]/g, ''));
                            var ttlamnt = parseFloat(totalvalue.replace(/[^\d.]/g, ''));
                            var percentage = salesAmount / ttlamnt;
                            htmlT += "<li>Total <span>" + percentage.toFixed(2) + " %" + " of Total</span></li>";
                            htmlT += "<li>$" + totalvalue.replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlT += "</ul>";
                        }
                        else {
                            htmlT += "<ul>";
                            htmlT += "<li>" + data.SalesWeeks + "</li>";
                            htmlT += "<li>$" + data.LastYearSales.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlT += "<li>$" + data.Sales.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlT += "</ul>";
                        }
                    }
                    else if (data.Type == "L") {
                        if (data.SalesWeeks == "Total") {
                            totalvalueL = data.Sales.toFixed(2);
                        }
                        else if (data.SalesWeeks == "Total%") {
                            htmlL += "<ul class='grandTotal'>";
                            var salesAmounts = parseFloat($('#SalesAmount').text().replace(/[^\d.]/g, ''));
                            var ttlamnts = parseFloat(totalvalueL.replace(/[^\d.]/g, ''));
                            var percentages = salesAmounts / ttlamnts;
                            htmlL += "<li>Total <span>" + percentages.toFixed(2) + " %" + " of Total</span></li>";
                            htmlL += "<li>$" + totalvalueL.replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlL += "</ul>";
                        }
                        else {
                            htmlL += "<ul>";
                            htmlL += "<li>" + data.SalesWeeks + "</li>";
                            htmlL += "<li>$" + data.LastYearSales.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlL += "<li>$" + data.Sales.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,") + "</li>";
                            htmlL += "</ul>";
                        }
                    }
                });
                $('#Up').html("");
                $('#Down').html("");
                $(htmlT).appendTo($('#Up'));
                $(htmlL).appendTo($('#Down'));
                if (myChart != undefined)
                    myChart.destroy();
                var ctx2 = document.getElementById('myChart2').getContext('2d');
                myChart = new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: response.result.Labels,
                        datasets: [{
                            label: 'Purchase to Sales %',
                            data: response.result.Data,
                            backgroundColor: response.result.backgroundColor,
                            borderColor: response.result.borderColor,
                            borderWidth: 1
                        }]
                    },
                    options: {
                        maintainAspectRatio: false,
                        responsive: true,
                        legend: {
                            display: false,
                        },
                        scales: {
                            yAxes: [{

                                ticks: {
                                    beginAtZero: true,
                                    min: 0,
                                    fontSize: 14,
                                },
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Percentage of Sales',
                                    fontSize: 16
                                }
                            }],
                            xAxes: [{
                                ticks: {
                                    fontSize: 14, 
                                },
                                scaleLabel: { 
                                    display: true,
                                    labelString: 'Weeks',
                                    fontSize: 16
                                }
                            }]
                        }
                    }
                });
                $("#loading").hide();
            }
        });
    }
