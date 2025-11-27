
function BankAccountDetail() {
    $.ajax({
        method: "GET",
        url: ROOTURL + '/dashboard/BankAccountDetailValue',
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                $("#BankAccountDetail").html("");
                var htm = "";
                $.each(response, function (j, data) {
                    var currentTime = new Date(data.LastSyncDate.match(/\d+/)[0] * 1);
                    var month = currentTime.getMonth() + 1;
                    var day = currentTime.getDate();
                    var year = currentTime.getFullYear();
                    var hours = currentTime.getHours();
                    var minutes = String(currentTime.getMinutes()).padStart(2, '0');
                    var ampm = hours >= 12 ? 'PM' : 'AM';
                    hours = hours % 12;
                    hours = hours ? hours : 12; // the hour '0' should be '12'
                    var myDate = month + "/" + day + "/" + year + " " + hours + ":" + minutes + " " + ampm;
                    //var myDate = month + "/" + day + "/" + year + " " + currentTime.getHours() + ":" + currentTime.getMinutes();
                    if (data.AccountNo != 'Uncleared Checks') {
                        htm += "<li><section><img src= '" + ROOTURL + "Content/images/bank.svg' /> <span>" + data.AccountNo + "</span></section ><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section></li > ";
                    }
                    else {
                        //htm += "<li class='uncleared'><section><img src='" + ROOTURL + "Content/images/bank-check.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></li >";
                        if (data.IsVisible != 0) {
                            htm += "<li class='uncleared'><section><img src='" + ROOTURL + "Content/images/bank-check.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section><div class='mailedOutContainer'><div class='mailedOutChecks'><span>Mailed</span><span>$ " + data.Mailed6month.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek1.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></div><div class='mailedOutChecks'><span>Unmailed</span><span>$ " + data.UnMailed6month.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek1.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek2.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek3.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek4.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek4.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></div></div></li>";
                        }
                        else {
                            htm += "<li class='uncleared'><section><img src='" + ROOTURL + "Content/images/bank-check.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section></li>";
                        }
                    }
                })
                $(htm).appendTo($('#BankAccountDetail'));
                $('#Show').attr('hidden', true);
                $('#Hide').removeAttr('hidden');
                $('#Refresh').removeAttr('hidden');
            }
        }
    });
}

function BankAccountDetailValueRefresh() {
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/BankAccountDetailValueRefresh',
        beforeSend: function () {
            Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                $("#BankAccountDetail").html("");
                var htm = "";
                $.each(response, function (j, data) {
                    var currentTime = new Date(data.LastSyncDate.match(/\d+/)[0] * 1);
                    var month = currentTime.getMonth() + 1;
                    var day = currentTime.getDate();
                    var year = currentTime.getFullYear();
                    var myDate = month + "/" + day + "/" + year + " " + currentTime.getHours() + ":" + currentTime.getMinutes();
                    if (data.AccountNo != 'Uncleared Checks') {
                        htm += "<li><section><img src='" + ROOTURL + "Content/images/bank.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section></li >";
                    }
                    else {
                        if (data.IsVisible != 0) {
                            htm += "<li class='uncleared'><section><img src='" + ROOTURL + "Content/images/bank-check.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section><div class='mailedOutContainer'><div class='mailedOutChecks'><span>Mailed</span><span>$ " + data.Mailed6month.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek1.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></div><div class='mailedOutChecks'><span>Unmailed</span><span>$ " + data.UnMailed6month.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span> <span>$ " + data.UnMailedLastWeek.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek1.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek2.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek3.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.UnMailedLastWeek4.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><span>$ " + data.MailedLastWeek4.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></div></div></li >";
                        }
                        else {
                            htm += "<li class='uncleared'><section><img src='" + ROOTURL + "Content/images/bank-check.svg' /><span>" + data.AccountNo + "</span></section><span>$ " + data.Balance.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span><section style='background: transparent !important;display: flex;gap: 5px;color: black !important;align-items: center;justify-content: center;margin-bottom:20px;margin-top:-10px'><span style='font-size: 10px !important;background: gray !important;color: white;padding: 2px 5px !important;border-radius: 3px !important;'>Last Updated On</span><span style='font-size: 11px !important;font-weight: normal !important;margin: 0px !important;color:black !important'>" + myDate + "</span></section></li>";
                        }
                    }
                })
                $(htm).appendTo($('#BankAccountDetail'));
                $('#Show').attr('hidden', true);
                $('#Hide').removeAttr('hidden');
                $('#Refresh').removeAttr('hidden');
            }
            Loader(0);
        }
    });
}
function HideBankAccountDetail() {
    $("#BankAccountDetail").html("");
    $('#Hide').attr('hidden', true);
    $('#Refresh').attr('hidden', true);
    $('#Show').removeAttr('hidden');
}
$(document).ready(function () {
    /*BankAccountDetail();*/

    $("#loadingMessage").show();
    $("#loading2").show();
    var currentDate = new Date();
    var startdate = Startdate;
    $('#DailyChartDate').val(startdate);

    $('#DailyChartDate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true,
        maxDate: currentDate
    });
    GetData($('#DailyChartDate').val());
    DisableOtherStores();
    $('#DailyChartDate').attr('autocomplete', 'off');

    if (storeid == '0') {
        $('.clshide').show();
    }
    else {
        $('.clshide').hide();
    }

    //Himanshu
    $('#exportPdfButton').on('click', function () {
        var StoreID = "";
        var checkedId = "";
        var Departments = "";
        $(".clsdailystores").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                StoreID += $(obj).val() + ",";
            }
        });
        $(".clsdailychartstores").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                checkedId += $(obj).val() + ",";
            }
        });
        checkedId = checkedId.slice(0, -1);

        $(".clsdailychartdepartments").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                Departments += $(obj).val() + ",";
            }
        });
        if (Departments != "") {
            Departments = Departments.slice(0, -1);
        } else {
            Departments = "ALL";
        }

        var StateId = $('#StateId').val();
        var GroupID = $('#GroupWiseStateStoreId').val();
        var Startdate = $('#DailyChartDate').val();
        location.href = '/Dashboard/DownloadDashboardDailyPDF?date=' + Startdate + '&StateId=' + StateId + '&GroupID=' + GroupID + '&StoreId=' + StoreID + '&SelectedStoreId=' + checkedId + '&Departments=' + Departments;
    });

    //Himanshu
    $('#exportExcelButton').on('click', function () {
        var StoreID = "";
        $(".clsdailystores").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                StoreID += $(obj).val() + ",";
            }
        });

        var StateId = $('#StateId').val();
        var GroupID = $('#GroupWiseStateStoreId').val();
        var Startdate = $('#DailyChartDate').val();
        location.href = '/Dashboard/DownloadDashboardDailyExcel?date=' + Startdate + '&StateId=' + StateId + '&GroupID=' + GroupID + '&StoreId=' + StoreID + '&SelectedStoreId=' + 0 + '&Departments=' + 0;
    });

    //Himanshu 04-02-2025
    $('#salesgraphexport').on('click', function () {
        $('#ExportSalesModal').modal('show');
        $('.modal-backdrop').hide();
        $('#SalesStartDate').datetimepicker({
            format: 'MM-DD-YYYY',
            useCurrent: false,
            maxDate: moment().subtract(1, 'days')
        });
        $('#SalesEndDate').datetimepicker({
            format: 'MM-DD-YYYY',
            useCurrent: false,
            maxDate: moment().subtract(1, 'days')
        });
    });

    $('#salesgraphexportexcel').on('click', function () {
        var Startdate = $('#SalesStartDate').val();
        var Enddate = $('#SalesEndDate').val();
        var stateid = $("#StateId").val();

        var isValid = true;

        if (!Startdate) {
            $('#SalesStartDate').css("border", "2px solid red");
            isValid = false;
        } else {
            $('#SalesStartDate').css("border", "1px solid #ccc");
        }

        if (!Enddate) {
            $('#SalesEndDate').css("border", "2px solid red");
            isValid = false;
        } else {
            $('#SalesEndDate').css("border", "1px solid #ccc");
        }

        if (!isValid) {
            return false;
        }


        $('#ExportSalesModal').modal('hide');

        var StoreID = "";
        $(".clsdailystores").each(function (index, obj) {
            if ($(obj).is(':checked') == true) {

                StoreID += $(obj).val() + ",";
            }
        });

        StoreID = StoreID.replace(/,$/, ''); 

        //remove val from date
        $('#SalesStartDate').val('');
        $('#SalesEndDate').val('');
        $('#SalesStartDate').css("border", "1px solid #ccc");
        $('#SalesEndDate').css("border", "1px solid #ccc");

        location.href = '/Dashboard/ExportSalesExcelData?startdate=' + Startdate + '&enddate=' + Enddate + '&stateid=' + stateid + '&storeids=' + StoreID;

    });

    $('.btnnetsalecancle').on('click', function () {
        $('#ExportSalesModal').modal('hide');
        $('#SalesStartDate').val('');
        $('#SalesEndDate').val('');
        $('#SalesStartDate').css("border", "1px solid #ccc");
        $('#SalesEndDate').css("border", "1px solid #ccc");
    });

});

$(function () {
    $('#DailyChartDate').datetimepicker({
        format: 'MM-DD-YYYY',
        useCurrent: true
    });

    $('#DailyChartDate').on('dp.change', function (e) {
        var date = $(this).val();
        GetData(date);
        $(".other-deposite").hide();
    });
});
//clsdailychartstores

function nFormatter(num, digits) {

    const lookup = [
        { value: 1, symbol: "" },
        { value: 1e3, symbol: "k" },
        { value: 1e6, symbol: "M" },
        { value: 1e9, symbol: "B" },
        { value: 1e12, symbol: "T" },
        { value: 1e15, symbol: "P" },
        { value: 1e18, symbol: "E" }
    ];
    const rx = /\.0+$|(\.[0-9]*[1-9])0+$/;
    var item = lookup.slice().reverse().find(function (item) {
        return num >= item.value;
    });
    return item ? (num / item.value).toFixed(digits).replace(rx, "$1") + item.symbol : "0.00";
}
function GetStateWisedata() {
    var Currentdate = $('#DailyChartDate').val();
    var stateDropdown = $('#StateId');
    //if (stateDropdown.val()) {
    //    $('#select2-StateId-container').closest('.select2-selection').css('background-color', '#dff4fb');
    //} else {
    //    $('#select2-StateId-container').closest('.select2-selection').css('background-color', '');
    //}

    $('#GroupWiseStateStoreId').val('').select2();
    GetData(Currentdate);
}

//himanshu 27-02-2025
function GetGroupWisedata() {
    var Currentdate = $('#DailyChartDate').val();
    var groupDropdown = $('#GroupWiseStateStoreId');

    //if (groupDropdown.val()) {
    //    $('#select2-GroupWiseStateStoreId-container').closest('.select2-selection').css('background-color', '#dff4fb');
    //} else {
    //    $('#select2-GroupWiseStateStoreId-container').closest('.select2-selection').css('background-color', '');
    //}
    $('#StateId').val('').select2();
    GetData(Currentdate);
}
function GetDatastatewise() {
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/Daily',
        data: { StateId: StateId, GroupID: GroupID },

        success: function (response) {

        }
    });
}

function GetDataStorewise() {
    var StateId = "";
    var GroupID = "";
    var StoreId = "";
    var Currentdate = $('#DailyChartDate').val();
    $(".clsdailystores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            StoreId += $(obj).val() + ",";
        }
    });
    if (StoreId != "" && StoreId != null) {
        $('.clshide').hide();
    }
    else {
        StateId = $('#StateId').val();
        GroupID = $('#GroupWiseStateStoreId').val();
        $('.clshide').show();
    }

    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/getDashboardDailyDataStoreWise',
        data: { date: Currentdate, StateId: StateId, StoreId: StoreId, GroupID: GroupID },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {

            $("#AllVoidsAmt").text("$ " + nFormatter(response.AllVoidsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#ItemCorrectsAmt").text("$ " + nFormatter(response.ItemCorrectsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#ItemReturnsAmt").text("$ " + nFormatter(response.ItemReturnsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            $("#AllVoids").text(response.AllVoids.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#ItemCorrects").text(response.ItemCorrects);
            $("#ItemReturns").text(response.ItemReturns);
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

            $("#SalesGrowth").text(response.SalesGrowth + "%");

            $("#CustomerCountCurrentDay").text(response.CustomerCountCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#CustomerCountGrowth").text(response.CustomerCountGrowth + "%");
            $("#AverageSaleCurrentDay").text("$ " + response.AverageSaleCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#AveragesalesGrowth").text(response.AveragesalesGrowth + "%");
            $("#TotalSalesLastWeek").text("$ " + response.TotalSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#CustomerCountLastWeek").text(response.CustomerCountLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#AverageSalesLastWeek").text("$ " + response.AverageSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#Offline").text(response.Offline.toFixed(2) + "%");
            $("#Online").text(response.Online.toFixed(2) + "%");
            $("#InvoicesAdded").text(response.InvoicesAdded);
            $("#InvoicesApproved").text(response.InvoicesApproved);

            $("#currentday").text("Current " + response.DayName);
            $("#currenttime").text(response.HourTime);
            $("#lastday").text("Last " + response.DayName);
            $("#lasttime").text(response.HourTime);



            if (response.SalesGrowth < 0) {
                $("#clsvalup").removeClass('valueUpCell');
                $("#clsvalup").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup").removeClass('valueDownCell');
                $("#clsvalup").addClass('valueUpCell');
            }
            if (response.CustomerCountGrowth < 0) {
                $("#clsvalup1").removeClass('valueUpCell');
                $("#clsvalup1").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup1").removeClass('valueDownCell');
                $("#clsvalup1").addClass('valueUpCell');
            }
            if (response.AveragesalesGrowth < 0) {
                $("#clsvalup2").removeClass('valueUpCell');
                $("#clsvalup2").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup2").removeClass('valueDownCell');
                $("#clsvalup2").addClass('valueUpCell');
            }
            //Loader(0);
        }
    });
}

function GetData(Currentdate) {
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    var StoreId = "";
    $(".clsdailystores").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            StoreId += $(obj).val() + ",";
        }
    });
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/getDashboardDailyData',
        data: { date: Currentdate, StateId: StateId, StoreId: StoreId, GroupID: GroupID },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {

            checkAllStore();
            //if ($("#HeaderStoreId option:selected").val() != "") {
            GetChartDetails();
            //}
            GetHourlyChartDetails();
            GetSalesOneHourWorkedData(Currentdate);
            $("#AllVoidsAmt").text("$ " + nFormatter(response.AllVoidsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#ItemCorrectsAmt").text("$ " + nFormatter(response.ItemCorrectsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#ItemReturnsAmt").text("$ " + nFormatter(response.ItemReturnsAmt, 2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            //if (response.AllVoidsAmt < 100000) {
            //    //$("#AllVoidsAmt").text("$ " + response.AllVoidsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));

            //} else {
            //    $("#AllVoidsAmt").text("#####");
            //}

            //if (response.ItemCorrectsAmt < 100000) {
            //    $("#ItemCorrectsAmt").text("$ " + response.ItemCorrectsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            //} else {
            //    $("#ItemCorrectsAmt").text("#####");
            //}

            //if (response.ItemReturnsAmt < 100000) {
            //    $("#ItemReturnsAmt").text("$ " + response.ItemReturnsAmt.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            //} else {
            //    $("#ItemReturnsAmt").text("#####");
            //}

            $("#AllVoids").text(response.AllVoids.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#ItemCorrects").text(response.ItemCorrects);
            $("#ItemReturns").text(response.ItemReturns);
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

            $("#SalesGrowth").text(response.SalesGrowth + "%");

            $("#CustomerCountCurrentDay").text(response.CustomerCountCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#CustomerCountGrowth").text(response.CustomerCountGrowth + "%");
            $("#AverageSaleCurrentDay").text("$ " + response.AverageSaleCurrentDay.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#AveragesalesGrowth").text(response.AveragesalesGrowth + "%");
            $("#TotalSalesLastWeek").text("$ " + response.TotalSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#CustomerCountLastWeek").text(response.CustomerCountLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            $("#AverageSalesLastWeek").text("$ " + response.AverageSalesLastWeek.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            $("#Offline").text(response.Offline.toFixed(2) + "%");
            $("#Online").text(response.Online.toFixed(2) + "%");
            $("#InvoicesAdded").text(response.InvoicesAdded);
            $("#InvoicesApproved").text(response.InvoicesApproved);

            $("#currentday").text("Current " + response.DayName);
            $("#currenttime").text(response.HourTime);
            $("#lastday").text("Last " + response.DayName);
            $("#lasttime").text(response.HourTime);



            if (response.SalesGrowth < 0) {
                $("#clsvalup").removeClass('valueUpCell');
                $("#clsvalup").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup").removeClass('valueDownCell');
                $("#clsvalup").addClass('valueUpCell');
            }
            if (response.CustomerCountGrowth < 0) {
                $("#clsvalup1").removeClass('valueUpCell');
                $("#clsvalup1").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup1").removeClass('valueDownCell');
                $("#clsvalup1").addClass('valueUpCell');
            }
            if (response.AveragesalesGrowth < 0) {
                $("#clsvalup2").removeClass('valueUpCell');
                $("#clsvalup2").addClass('valueDownCell');
                //$("#clsvalup").attr('valueUpCell', 'valueDownCell');
            } else {
                $("#clsvalup2").removeClass('valueDownCell');
                $("#clsvalup2").addClass('valueUpCell');
            }
            //Loader(0);

            //GET STATEWISE STORE CHECKBOX CONDITION (HIMANSHU 31-12-2024)
            GetStatetWiseStore();
        }
    });
}

function GetSalesOneHourWorkedData(Currentdate) {

    var Departments = "";
    $(".clshourlychartdepartments").each(function (index, obj) {
        if ($(obj).is(':checked') == true) {

            Departments += $(obj).val() + ",";
        }
    });

    if (Departments != "") {
        Departments = Departments.slice(0, -1);
    } else {
        Departments = "ALL";
    }
    var div = document.getElementsByClassName("clsstr");
    var Length = div.length;
    for (var i = 0; i < Length; i++) {
        div[0].remove();
    }
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetSalesOneHourWorkedData',
        data: { date: Currentdate, department: Departments, StateId: StateId, GroupID: GroupID },
        beforeSend: function () {
            //Loader(1);
        },
        success: function (response) {
            if (response.length > 0) {
                $(".departmentWiseSalesWidgets").html("");
                var htm = "";
                $.each(response, function (j, data) {
                    htm += "<div class='clsstr'><span>" + data.StoreName + "</span><span>$ " + data.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></div>";
                })
                $(htm).appendTo($('.departmentWiseSalesWidgets'));
            }
            //Loader(0);
        }
    });
}


function GetSalesTotalDetailsdata() {
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
    var Date = $('#DailyChartDate').val();
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    $.ajax({
        method: "GET",
        url: ROOTURL + 'dashboard/GetSalesTotalDetailsdata',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, Date: Date, StateId: StateId, GroupID: GroupID },
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
                    CurSign = "$ ";
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
                        htmDept += "<li><span> " + data.StoreId + "</span> <span>" + CurSign + " " + data.Amount.toFixed(Decimalval).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "</span></li>";
                    }
                })
                htmDept += "</ul>";
                $(htm).appendTo($('#dvTotalStore'));
                $(htmDept).appendTo($('#dvTotalDepartment'));
            }
            //Loader(0);
        }
    });
}

$(".clsdailychartstores").change(function () {

    if ($(this).is(':checked') == true) {
        if (this.value == "0") {

            $(".clsdailychartstores").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', true);
                } else {
                    $(this).prop('checked', true);
                }
            });
        }
        else {
            $(".clsdailychartstores").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetChartDetails();
    }
    else {
        if (this.value == "0") {

            $(".clsdailychartstores").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', false);
                } else {
                    $(this).prop('checked', false);
                }
            });
        }
        else {
            $(".clsdailychartstores").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetChartDetails();
    }

});
$(".clsdailystores").change(function () {
    var currentValue = this.value;

    if ($(this).is(':checked') == true) {
        if (this.value == "0") {

            $(".clsdailystores").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', true);
                } else {
                    $(this).prop('checked', true);
                }
            });
        }
        else {
            $(".clsdailystores").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetDataStorewise();
    }
    else {
        if (this.value == "0") {

            $(".clsdailystores").each(function (index, obj) {
                if (this.value != "0") {
                    $(this).prop('checked', false);
                } else {
                    $(this).prop('checked', false);
                }
            });
        }
        else {
            $(".clsdailystores").each(function (index, obj) {
                if (this.value == "0") {
                    $(this).prop('checked', false);
                }
            });
        }
        GetDataStorewise();
    }
});
//else {
//    if (this.value == "0") {
//        $(".clsdailychartstores").each(function (index, obj) {
//            if ($(obj).val != "0") {
//                $(this).prop('checked', false);
//            }
//        });
//    }
//  //  GetChartDetails();
//}

$(".clsRadio").change(function () {
    GetChartDetails();
})
$(".clsdailychartdepartments").change(function () {
    GetChartDetails();
})

$(".clshourlychartdepartments").change(function () {

    GetHourlyChartDetails();
    GetSalesOneHourWorkedData($('#DailyChartDate').val());

})

var chart2;
function GetChartDetails() {
    $("#loadingMessage").show();
    //$("#bar-chartcanvas").load('~/Dashboard/GetChartData');
    GetSalesTotalDetailsdata();
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
    if (radioId == "1") {
        $('#graphtitle').text("Hourly Sales Graph");
        $("input.clsdailychartdepartments").removeAttr("disabled");
        $("input.clsdailychartstores").attr("detail", "Hourly Sales Graph - Stores");
        $("input.clsdailychartdepartments").attr("detail", "Hourly Sales Graph - Departments");
    } else if (radioId == "2") {
        $('#graphtitle').text("Hourly Customer Count Graph");
        $("input.clsdailychartdepartments").attr("disabled", true);
        $("input.clsdailychartstores").attr("detail", "Hourly Customer Count Graph - Stores");
        $("input.clsdailychartdepartments").attr("detail", "Hourly Customer Count Graph - Departments");
    } else {
        $('#graphtitle').text("Hourly Average Sale Graph", false);
        $("input.clsdailychartdepartments").removeAttr("disabled");
        $("input.clsdailychartstores").attr("detail", "Hourly Average Sale Graph - Stores");
        $("input.clsdailychartdepartments").attr("detail", "Hourly Average Sale Graph - Departments");
        //$("input.clsdailychartdepartments").attr("disabled", true);
    }
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
    var Date = $('#DailyChartDate').val();
    var ctx = $("#bar-chartcanvas");

    var options = {

        tooltips: {
            callbacks: {
                title: function (t, d) {
                    return "Time : " + d.labels[t[0].index];
                    //return ['Hours : ' + d.labels[t[0].index], 'another thing', 'and another one'];
                },
                label: function (context, lbldata) {
                    var arr = [];
                    var StoreCode = lbldata.datasets[context.datasetIndex].code;
                    $.ajax({
                        async: false,
                        type: "POST",
                        url: '/Dashboard/getToolTipData',
                        data: { StoreId: StoreCode, Department: Departments, Type: radioId, Date: Date, Hours: context.xLabel, Mode: "D" },
                        //contentType: "application/json",
                        //dataType: "json",

                        success: function (chData) {
                            aa = chData;
                            var strDept = "";
                            $.each(aa, function (index, obje) {
                                strDept = "";
                                if (radioId == "1") {
                                    strDept = obje.Department + " - [" + obje.Hour1 + "] - " + " $ " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " ------ [" + obje.TillTime + "] $ " + obje.TAmount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                    arr.push(strDept);
                                } else if (radioId == "2") {
                                    strDept = obje.Department + " - [" + obje.Hour1 + "] - " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " ------ [" + obje.TillTime + "] $ " + obje.TAmount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                    strDept = obje.Department + " : " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                    arr.push(strDept);
                                }

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
        maintainAspectRatio: false,
        responsive: true,
        title: {
            display: true,
            position: "top",
            text: "",
            fontSize: 18,
            fontColor: "#000"
        },
        //interaction: {
        //    intersect: false,
        //},
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
                                return '$ ' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            } else {
                                return value.toFixed(0).toString();
                            }
                        } else {
                            if (radioId != "2") {
                                return '$ ' + value.toFixed(2);
                            } else {
                                return value.toFixed(0);
                            }
                        }
                    }
                },
            }],
            xAxes: [{
                ticks: {
                    beginAtZero: true,
                    min: 0
                }
            }]
        }
    };


    if (chart2) {
        chart2.destroy();
    }
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    $.ajax({

        type: "POST",
        url: ROOTURL + 'Dashboard/GetChartData',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, Date: Date, StateId: StateId, GroupID: GroupID },
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
                $("#graphTotal").text("$ " + Total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,"));
            }
            else {
                $(".totals").show();
                $("#graphTotal").text(Total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, "$&,").replace(".00", ""));
            }
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            if (chart2 != undefined)
                chart2.destroy();

            chart2 = new Chart(ctx, {
                type: "line",
                data: data,
                options: options,

            });
            $("#loadingMessage").hide();
        }
    });


    var ctx3 = $("#bar-chartcanvasbar");
    $.ajax({

        type: "POST",
        url: ROOTURL + 'Dashboard/GetChartDataBar',
        data: { StoreId: checkedId, Department: Departments, Type: radioId, Date: Date, StateId: StateId, GroupID: GroupID },
        success: function (chData) {

            var aData = chData;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["DatsetLists"];
            var data = {
                labels: aLabels,
                datasets: aDatasets1,
            };

            var chart = new Chart(ctx3, {
                type: "bar",
                data: data,
                //options: options

            });

        }
    });
};

var myChartHOur;
function GetHourlyChartDetails() {

    $("#loading2").show();
    var checkedId = "";
    var radioId = "1";
    var Departments = "";

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

    var Date = $('#DailyChartDate').val();
    var StateId = $('#StateId').val();
    var GroupID = $('#GroupWiseStateStoreId').val();
    if (myChartHOur != undefined)
        myChartHOur.destroy();

    $.ajax({

        type: "POST",
        url: ROOTURL + 'Dashboard/GetHourlyChartData',
        data: { StoreId: checkedId, Department: Departments, Type: "4", Date: Date, StateId: StateId, GroupID: GroupID },
        success: function (chData) {

            var aData = chData;
            var aLabels = aData["Labels"];
            var aDatasets1 = aData["HourlyDatsetList"];
            var data = {
                labels: aLabels[0],
                datasets: aDatasets1,
            };

            var options = {
                tooltips: {
                    callbacks: {
                        title: function (t, d) {
                            return "Payroll Hours : " + t[0].xLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            //return ['Hours : ' + d.labels[t[0].index], 'another thing', 'and another one'];
                        },
                        label: function (context, lbldata) {
                            var arr = [];
                            var StoreCode = lbldata.datasets[context.datasetIndex].label;
                            $.ajax({
                                async: false,
                                type: "POST",
                                url: ROOTURL + 'Dashboard/getHourlyChartToolTipData',
                                data: { StoreId: StoreCode, Department: Departments, Type: "4", Date: Date },
                                success: function (chData) {
                                    aa = chData;
                                    var strDept = "";
                                    $.each(aa, function (index, obje) {
                                        strDept = "";
                                        if (radioId != "2") {
                                            strDept = obje.Department + " Sales :  $ " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                        } else {
                                            strDept = obje.Department + " Sales : " + obje.Amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                        }
                                        arr.push(strDept);
                                    })
                                }
                            });
                            arr.push("Total Sales : $ " + context.yLabel.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",").replace(".00", ""));
                            return arr;
                        }
                    }
                },
                responsive: true, // Instruct chart js to respond nicely.
                maintainAspectRatio: false, // Add to prevent default behaviour of full-width/height
                showLines: false,
                legend: {
                    display: false
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            callback: function (value, index, values) {
                                if (parseInt(value) >= 1000) {
                                    return '$ ' + value.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                } else {
                                    return '$ ' + value.toFixed(2);
                                }
                            },

                        }
                    }],
                    xAxes: [{
                        ticks: {
                            beginAtZero: true,
                            min: 0
                        }
                    }]
                },
            };
            var ctx1 = document.getElementById("bar-chartcanvas2").getContext('2d');
            // End Defining data


            myChartHOur = new Chart(ctx1, {
                type: 'scatter',
                data: data,
                options: options,

            });
            $("#loading2").hide();
        }
    });
};

function closemodal() {
    $(".divIDClass").hide();
    //$(".overlaybox").hide();
}

var HeaderStoreId = $("#HeaderStoreId option:selected").val();
if (HeaderStoreId == "") {
    HeaderStoreId = "0";
}
if (HeaderStoreId != "0") {
    $(".clsdailychartstores").each(function (index, obj) {
        var valu = $(obj).val();

        if (HeaderStoreId == valu) {

            $(obj).attr('checked', 'checked')
        }
        else {
            $(obj).attr('checked', false)
        }
    });

    // Call the disable function
    DisableOtherStores();
}

function DisableOtherStores() {
    var HeaderStoreId = $("#HeaderStoreId option:selected").val();

    if (HeaderStoreId != "" && HeaderStoreId != "0") {
        // A specific store is selected
        $(".clsdailychartstores").each(function (index, obj) {
            var valu = $(obj).val();

            if (HeaderStoreId != valu) {
                // Disable other stores
                $(obj).prop('disabled', true);
                $(obj).parent('li').css('opacity', '0.5');
                $(obj).parent('li').css('cursor', 'not-allowed');
            } else {
                // Keep selected store enabled
                $(obj).prop('disabled', false);
                $(obj).parent('li').css('opacity', '1');
                $(obj).parent('li').css('cursor', 'pointer');
            }
        });
    } else {
        // "ALL" is selected, enable all stores
        $(".clsdailychartstores").each(function (index, obj) {
            $(obj).prop('disabled', false);
            $(obj).parent('li').css('opacity', '1');
            $(obj).parent('li').css('cursor', 'pointer');
        });
    }
}

$('#HeaderStoreId').on('change', function () {
    DisableOtherStores();
});

function checkAllStore() {
    var HeaderStoreId = $("#HeaderStoreId option:selected").val();
    if (HeaderStoreId == "") {
        HeaderStoreId = "0";
    }
    if (HeaderStoreId == "0") {
        $(".clsdailychartstores").each(function (index, obj) {
            $(this).prop('checked', true);
        });
        $(".clsdailystores").each(function (index, obj) {
            $(this).prop('checked', true);
        });
    }
}

function GetStatetWiseStore() {
    var HeaderStoreId = $("#HeaderStoreId option:selected").val();
    if (HeaderStoreId == "") {
        HeaderStoreId = "0";
    }
    if (HeaderStoreId == "0") {
        var stateid = $("#StateId").val();
        var GroupID = $('#GroupWiseStateStoreId').val();
        if (stateid != "" || GroupID != "" && GroupID !== undefined) {
            $.ajax({
                url: ROOTURL + "Dashboard/GetStoreBySelectedStateId",
                type: 'POST',
                data: { stateid: stateid, GroupID: GroupID },
                success: function (response) {
                    if (Array.isArray(response)) {
                        $(".clsdailychartstores").prop("checked", false);
                        $(".clsdailychartstores").each(function (index, obj) {
                            var checkboxValue = parseInt($(obj).val(), 10);
                            if (response.includes(checkboxValue)) {
                                $(obj).prop("checked", true);
                            }
                        });

                        $(".clsdailystores").prop("checked", false);
                        $(".clsdailystores").each(function (index, obj) {
                            var checkboxValue = parseInt($(obj).val(), 10);
                            if (response.includes(checkboxValue)) {
                                $(obj).prop("checked", true);
                            }
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error);
                }
            });
        }
    }
}