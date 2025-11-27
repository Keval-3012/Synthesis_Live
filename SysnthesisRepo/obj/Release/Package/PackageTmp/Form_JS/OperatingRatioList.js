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
        var tablinks = document.getElementsByClassName("w3-red");
        var StoreId = 0;
        for (i = 0; i < tablinks.length; i++) {
            StoreId = tablinks[i].value;
        }

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
                Loader(0);
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