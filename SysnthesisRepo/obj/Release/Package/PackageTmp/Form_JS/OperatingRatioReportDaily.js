function closemodal() {
    $(".divIDClass").hide();
}
if (ShiftName == "0") {
    $('#DrpLstShift').val('0');
}
else {
    $('#DrpLstShift').val(ShiftName);
}
$("#ratioTbl").tablesorter();
$('.stop-propagation').on('click', function (e) {
    e.stopPropagation();
});

Loader(0);

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    $("[data-toggle=tooltip]").tooltip();
    if (val == 1) {
        $(".loading-container").attr("style", "display:block;")
    }
    else {
        $(".loading-container").attr("style", "display:none;")
    }

}

function cleartextsearch() {
    FunSearchRecord();
}

function printData() {
    var htmlToPrint = '' +
        '<style type="text/css">' +
        'table th, table td {' +
        'border:1px solid #000;' +
        'padding;0.5em;' +
        '}' +
        '</style>';
    var divContents = $("#ratioTbl")[0].outerHTML;//document.getElementById("dvBody").innerHTML;
    htmlToPrint += divContents;
    var mywindow = window.open('', 'PRINT', "FullScreen");
    mywindow.document.write('<html><head><title></title>');

    mywindow.document.write('</head><body >');
    mywindow.document.write('<h3>Operating Ratio Daily Report</h3>');
    var str = '';

    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    if (FromDate != "") {
        str = "FromDate :" + FromDate + " ";
    }
    if (ToDate != "") {
        str += "ToDate :" + ToDate + " ";
    }
    if (str != "") {
        mywindow.document.write('<h5>' + str + '</h5>');
    }
    mywindow.document.write(htmlToPrint);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    mywindow.print();
    mywindow.close();

    return true;

}

var top = 0;
function Loader(val) {
    var doc = document.documentElement;
    if (val == 1) {
        $("#secloader").removeClass('pace-active1');
        $("#secloader").addClass('pace-active');
        $("#dvloader").removeClass('bak1');
        $("#dvloader").addClass('bak');
        $("#globalFooter").addClass('LoaderFooter');
        top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
    }
    else {
        $("#secloader").removeClass('pace-active');
        $("#secloader").addClass('pace-active1');
        $("#dvloader").removeClass('bak');
        $("#dvloader").addClass('bak1');
        $("#globalFooter").removeClass('LoaderFooter');
        doc.scrollTop = top;
    }
}

function ManageQuarterlyData(iii) {

    var today = new Date();
    var Year = today.getFullYear();
    var Day = today.getDate();
    var hs = today.toLocaleString('default', { month: 'short' })
    var quarter;
    if (iii == 1) {
        $('#txtstartdate').val("Jan 01, " + Year);
        $('#txtenddate').val("Mar 31, " + Year);
    }
    else if (iii == 2) {
        $('#txtstartdate').val("Apr 01, " + Year);
        $('#txtenddate').val("Jun 30, " + Year);
    }
    else if (iii == 3) {
        $('#txtstartdate').val("Jul 01, " + Year);
        $('#txtenddate').val("Sep 30, " + Year);
    }
    else if (iii == 4) {
        $('#txtstartdate').val("Oct 01, " + Year);
        $('#txtenddate').val("Dec 31, " + Year);
    }
    else if (iii == 'YTD') {
        $('#txtstartdate').val("Jan 01, " + Year);
        $('#txtenddate').val(hs + " " + (Day.toString().length < 2 ? "0" + Day : Day) + ", " + Year);

    }
    getRatioList();
}
function getRatioList(obj, StoreId) {
    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    if (obj != undefined) {
        tablinks = document.getElementsByClassName("tablink");
        for (i = 0; i < tablinks.length; i++) {
            tablinks[i].className = tablinks[i].className.replace(" w3-red", "");
        }
        obj.className += " w3-red";
    } else {
        tablinks = document.getElementsByClassName("w3-red");
        for (i = 0; i < tablinks.length; i++) {
            StoreId = tablinks[i].value;
        }
    }

    $.ajax({
        url: '/Report/getRatioDailyList',
        type: "post",
        cache: false,
        data: { FromDate: FromDate, ToDate: ToDate, StoreId: StoreId },
        beforeSend: function () {
            Loader(1);
        },
        success: function (states) {

            $("#dvBody").html(states);
            $("#ratioTbl").tablesorter();

            Loader(0);
        },
        error: function () {
            Loader(0);
        }
    });
};
function Comasaperatedvalue(ii) {
    var x = "2";
    var AmtVal = Number(ii).toFixed(x);
    if (AmtVal.indexOf("-") != "-1") {
        AmtVal = "(" + AmtVal.replace("-", "") + ")";
    }
    var FnlVal = AmtVal.toString().split(".");
    return FnlVal[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (FnlVal[1] ? "." + FnlVal[1] : "");
}

function getStoreWiseData(Department, row, obj) {


    var img = document.getElementById('img' + row);
    if (img.textContent == "+") {

        var div = document.getElementsByClassName("childrow");
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            div[0].remove();
        }

        var txtSpn = document.getElementsByClassName("clsbutton");
        for (var i = 0; i < txtSpn.length; i++) {
            txtSpn[i].textContent = "+";
        }

        var FromDate = $('#txtstartdate').val();
        var ToDate = $('#txtenddate').val();


        $.ajax({
            url: '/Report/getStoreWiseDailyData',
            type: "get",
            cache: false,
            data: { Department: Department, FromDate: FromDate, ToDate: ToDate },
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {
                if (states.length > 0) {
                    $.each(states, function (j, data) {
                        var Sales = data.Sales.toString();
                        var htm = "<tr class='childrow child" + row + "'>";
                        htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.Department.toString() + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.Sales.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.TotalSalPercentage.toString() + " %</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.COgs.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.CogsPe.toString() + " %</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PDFAmount.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.PDFPercentage.toString() + " %</td>";
                        htm += "</tr>";
                        $(htm).insertAfter($('.row' + row).closest('tr'));
                    })
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
        var div = document.getElementsByClassName("child" + row);
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            div[0].remove();
        }
    }
};