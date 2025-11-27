function closemodal() {
    $(".divIDClass").hide();
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
    mywindow.document.write('<h3>Payroll Analysis Report</h3>');
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

$(document).ready(function () {
    var currentDate = new Date();
    var formattedDate = moment(currentDate).format('MMM DD, YYYY');
    $('#txtstartdate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false,
        maxDate: currentDate
    });
    $('#txtenddate').datetimepicker({
        format: 'MMM DD, YYYY',
        useCurrent: false,
        maxDate: currentDate
    });
    $('#txtstartdate').val(formattedDate);
    $('#txtenddate').val(formattedDate);

});

function ManageQuarterlyData(iii) {

    var today = new Date();
    var Year = today.getFullYear();
    var Day = today.getDate();
    var hs = today.toLocaleString('default', { month: 'short' })
    //d = d || new Date();
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
        //$('#txtenddate').val(hs + " " + today.toString("dd, yyy"));
        $('#txtenddate').val(hs + " " + (Day.toString().length < 2 ? "0" + Day : Day) + ", " + Year);

    }
    getRatioList();
    //return m > 4 ? m - 4 : m;
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
        url: '/Report/getPayrollList',
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

function getalldatacollapse1(obj) {

    Loader(1);
    if (getalldatacollapse(obj) == true) {
        setTimeout(function () {
            Loader(0);
        }, 10);
    }
}
function getalldatacollapse(obj) {

    var tablinks = document.getElementsByClassName("w3-red");
    var StoreId = 0;
    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();

    for (i = 0; i < tablinks.length; i++) {
        StoreId = tablinks[i].value;
    }

    var div = document.getElementsByClassName("childrow");
    var Length = div.length;
    for (var k = 0; k < Length; k++) {
        div[0].remove();
    }

    if ($("#MainCollapse")[0].textContent == "-") {
        var txtSpn = document.getElementsByClassName("clsbutton");
        for (var k = 0; k < txtSpn.length; k++) {
            txtSpn[k].textContent = "-";
        }
    }
    else if ($("#MainCollapse")[0].textContent == "+") {
        var txtSpn = document.getElementsByClassName("clsbutton");
        for (var k = 0; k < txtSpn.length; k++) {
            txtSpn[k].textContent = "+";
        }
    }

    var rowCount = $('#ratioTbl tr').length;
    for (var i = 1; i < rowCount - 1; i++) {
        var row = i;
        var img = document.getElementById('img' + row);
        if (img.textContent == "+") {
            img.textContent = "+"
            var Department = document.getElementsByClassName('row' + row)[0].cells[0].innerText.replace('+', '').trim();
            $.ajax({
                url: '/Report/getStoreWisePayrollanalysisData',
                type: "get",
                cache: false,
                async: false,
                data: { FromDate: FromDate, ToDate: ToDate, StoreId: StoreId, Department: Department },
                beforeSend: function () {
                },
                success: function (states) {
                    if (states.length > 0) {
                        $.each(states, function (j, data) {
                            var htm = "<tr class='childrow child" + row + "'>";
                            htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.DepartmentName.toString() + "</td>";
                            htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PayrollAmount.toString()) + "</td>";
                            htm += "<td style='font-weight: normal;' class='childtd'>" + (data.PayrollPercentage == null ? "" : data.PayrollPercentage).toString() + " %</td>";
                            htm += "</tr>";
                            $(htm).insertAfter($('.row' + row).closest('tr'));
                        })
                    }
                    obj.title = "Expanded";
                },
                error: function () {
                    Loader(0);
                }
            });
            img.textContent = "-"
            $("#MainCollapse")[0].textContent = "-"
        }
        else {
            img.textContent = "+"
            var div = document.getElementsByClassName("child" + row);
            var Length = div.length;
            for (var j = 0; j < Length; j++) {
                div[0].remove();
            }
            $("#MainCollapse")[0].textContent = "+"
        }
        //if (i == rowCount -2) {
        //    Loader(0);
        //}
    }
    return true;
}

function getStoreWiseTotalData(row, obj) {
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
                    $.each(states, function (j, data) {
                        var htm = "<tr class='childrow1 invoiceamt" + row + "'>";
                        htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.DepartmentName.toString() + " - $" + data.PayrollAmount.toLocaleString() + " - " + data.PayrollPercentage.toLocaleString() + "%</td>";
                        htm += "</tr>";
                        $(htm).insertAfter($('#tbl11'));
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
        var div = document.getElementsByClassName("invoiceamt" + row);
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            div[0].remove();
        }
    }
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
        var tablinks = document.getElementsByClassName("w3-red");
        var StoreId = 0;
        for (i = 0; i < tablinks.length; i++) {
            StoreId = tablinks[i].value;
        }


        $.ajax({
            url: '/Report/getStoreWisePayrollanalysisData',
            type: "get",
            cache: false,
            data: { FromDate: FromDate, ToDate: ToDate, StoreId: StoreId, Department: Department },
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {
                if (states.length > 0) {
                    $.each(states, function (j, data) {
                        var htm = "<tr class='childrow child" + row + "'>";
                        htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.DepartmentName.toString() + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PayrollAmount.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + (data.PayrollPercentage == null ? "" : data.PayrollPercentage).toString() + " %</td>";
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

function ExportExcelData() {
    var tablinks = document.getElementsByClassName("w3-red");
    var StoreId = 0;
    for (i = 0; i < tablinks.length; i++) {
        StoreId = tablinks[i].value;
    }
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    if (element_txtstartdate != "" || element_txtenddate != "") {
        let a = document.createElement('a');
        a.target = '_blank';
        a.href = ROOTURL + 'Report/ExportExcelPayrollAnalysisData?StartDate=' + element_txtstartdate + '&EndDate=' + element_txtenddate + '&StoreId=' + StoreId;
        a.click();
    }
}

function ExportPDFData() {
    var tablinks = document.getElementsByClassName("w3-red");
    var StoreId = 0;
    for (i = 0; i < tablinks.length; i++) {
        StoreId = tablinks[i].value;
    }
    var element_txtstartdate = document.getElementById('txtstartdate').value;
    var element_txtenddate = document.getElementById('txtenddate').value;
    if (element_txtstartdate != "" || element_txtenddate != "") {
        let a = document.createElement('a');
        a.target = '_blank';
        a.href = ROOTURL + 'Report/ExportPDFPayrollAnalysisData?StartDate=' + element_txtstartdate + '&EndDate=' + element_txtenddate + '&StoreId=' + StoreId;
        a.click();
    }
}