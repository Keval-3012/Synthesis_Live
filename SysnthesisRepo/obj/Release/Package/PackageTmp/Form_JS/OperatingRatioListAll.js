function getStoreWiseTotalData(row, obj, Mode) {
    var img = document.getElementById('FinalRows');
    if (img.textContent == "+") {
        var div = document.getElementsByClassName("childrow1");
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            div[0].remove();
        }

        var txtSpn = document.getElementsByClassName("clsbutton2");
        for (var i = 0; i < txtSpn.length; i++) {
            txtSpn[i].textContent = "+";
        }

        var FromDate = $('#txtstartdate').val();
        var ToDate = $('#txtenddate').val();
        var StoreId = 0;

        $.ajax({
            url: '/Report/getStoreWiseOperatingRationDepAllTotal',
            type: "get",
            cache: false,
            data: { Department: 'Total', FromDate: FromDate, ToDate: ToDate },
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {

                if (states.Lst.length > 0) {
                    var htm = "<div class='totals'>"
                    htm += "<ul class='ttl'>";
                    $.each(states.Lst, function (j, val) {

                        htm += "<li><span>[" + val.Department.toString() + "]</span> <span>$" + val.Sales.toLocaleString() + "</span> <span>" + val.TotalSalPercentage.toLocaleString() + "%</span> </li>";
                    })
                    htm += "</ul>";
                    htm += "</div>";
                    $(htm).appendTo($('#tblSalesTotal'));
                }
                if (states.Lst2.length > 0) {
                    htm = "";
                    htm = "<div class='totals'>"
                    htm += "<ul class='ttl'>";
                    $.each(states.Lst2, function (j, Lst2) {

                        htm += "<li><span>[" + Lst2.Department.toString() + "]</span> <span>$" + Lst2.COgs.toLocaleString() + "</span> <span>" + Lst2.SalPercentage.toLocaleString() + "%</span> </li>";
                    })
                    htm += "</ul>";
                    htm += "</div>";
                    $(htm).appendTo($('#tblCogsTotal'));
                }
                if (states.Lst3.length > 0) {
                    htm = "";
                    htm = "<div class='totals'>"
                    htm += "<ul class='ttl'>";
                    $.each(states.Lst3, function (j, Lst3) {

                        htm += "<li><span>[" + Lst3.Department.toString() + "]</span> <span>$" + Lst3.PDFAmount.toLocaleString() + "</span> <span>" + Lst3.PDFPercentage.toLocaleString() + "%</span> </li>";
                    })
                    htm += "</ul>";
                    htm += "</div>";
                    $(htm).appendTo($('#tblPayrollStoreTotal'));
                }
                obj.title = "Expanded";
                Loader(0);
            },
            error: function () {
                Loader(0);
            }
        });

        $.ajax({
            url: '/Report/getStoreWisePayrollanalysisTotal',
            type: "get",
            cache: false,
            data: { FromDate: FromDate, ToDate: ToDate, StoreId: StoreId },
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {
                if (states.length > 0) {
                    var htm = "<div class='totals'>"
                    htm += "<ul class='ttl'>";
                    $.each(states, function (j, data) {

                        htm += "<li><span>[" + data.DepartmentName.toString() + "]</span> <span>$" + data.PayrollAmount.toLocaleString() + "</span> <span>" + data.PayrollPercentage.toLocaleString() + "%</span> </li>";

                    })
                    htm += "</ul>";
                    htm += "</div>";
                    $(htm).appendTo($('#tblPayrollTotal'));
                }

                obj.title = "Expanded";
            },
            error: function () {
                Loader(0);
            }
        });

        img.textContent = "-"
    }
    else {
        img.textContent = "+"
        var div = document.getElementsByClassName("totals");
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            div[0].remove();
        }
    }
}