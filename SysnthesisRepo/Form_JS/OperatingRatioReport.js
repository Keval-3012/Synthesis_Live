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

function closemodal() {
    $(".divIDClass").hide();
}



var top = 0;
//function Loader(val) {
//    var doc = document.documentElement;
//    $("[data-toggle=tooltip]").tooltip();
//    if (val == 1) {
//        $(".loading-container").attr("style", "display:block;")
//    }
//    else {
//        $(".loading-container").attr("style", "display:none;")
//    }

//}
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
    mywindow.document.write('<h3>Operating Ratio Report</h3>');
    var str = '';

    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    var ShiftId = $("#ShiftId option:selected").text();
    if (FromDate != "") {
        str = "FromDate :" + FromDate + " ";
    }
    if (ToDate != "") {
        str += "ToDate :" + ToDate + " ";
    }
    if (ShiftId != "") {
        str += "Shift :" + ShiftId + " ";
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
    var mon = today.getMonth() + 1;
    var hs = today.toLocaleString('default', { month: 'short' })
    //d = d || new Date();
    var quarter;
    if (iii == 1) {
        $('#txtstartdate').val("01/01/" + Year);
        $('#txtenddate').val("03/31/" + Year);
    }
    else if (iii == 2) {
        $('#txtstartdate').val("04/01/" + Year);
        $('#txtenddate').val("06/30/" + Year);
    }
    else if (iii == 3) {
        $('#txtstartdate').val("07/01/" + Year);
        $('#txtenddate').val("09/30/" + Year);
    }
    else if (iii == 4) {
        $('#txtstartdate').val("10/01/" + Year);
        $('#txtenddate').val("12/31/" + Year);
    }
    else if (iii == 'YTD') {
        $('#txtstartdate').val("01/01/" + Year);
        //$('#txtenddate').val(hs + " " + today.toString("dd, yyy"));
        $('#txtenddate').val((mon.toString().length < 2 ? "0" + mon : mon) + "/" + (Day.toString().length < 2 ? "0" + Day : Day) + "/" + Year);
    }
    else if (iii == 'LFY') {
        $('#txtstartdate').val("01/01/" + (Year - 1));
        //$('#txtenddate').val(hs + " " + today.toString("dd, yyy"));
        $('#txtenddate').val("12/31/" + (Year - 1));

    }
    getRatioList();
    //return m > 4 ? m - 4 : m;
}
function getRatioList(obj, StoreId) {
    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    var ShiftId = $('#ShiftId').val();
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
        url: '/Report/getRatioList',
        type: "post",
        cache: false,
        data: { FromDate: FromDate, ToDate: ToDate, ShiftId: ShiftId, StoreId: StoreId },
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
//$(document).ready(function () {
//    var JumpTo = 'AtBottomOfForm';
//    if (JumpTo != "") {
//        $(this).scrollTop($('#' + JumpTo).position().top);
//    }
//});

// Harsh's Code start for Expand ALL
function getStoreWiseDataALL() {
    
    var rows = $('#ratioTbl tbody tr');
    var image = document.getElementById('allrow');
    var i = 0;
    var Departments = [];
    rows.each(function (index) {
        var department = $(this).find('td:first').text().trim().replace(/^\s*\+/, '').trim();
        var spanValue = $(this).find('td:first span.clsbutton').text().trim();
        Departments.push(department);
        i++;
    });

    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    var ShiftId = $('#ShiftId').val();
    if (image.textContent == "+") {
        Loader(1);
        image.textContent = "-"
        $.ajax({
            url: '/Report/getStoreWiseDataForAllDep',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: "Post",
            cache: false,
            data: JSON.stringify({ Department: Departments, FromDate: FromDate, ToDate: ToDate, ShiftId: ShiftId }),
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {
                var k = 0;
                Loader(1);
                rows.each(function (index) {
                    Loader(1);
                    var department = $(this).find('td:first').text().trim().replace(/^\s*\+/, '').trim();
                    var depData = states.filter(function (item) {
                        return item.MasterDepartment === department;
                    });
                    getStoreWiseDataCopy(department, index + 1, this, depData);
                    k++;
                });
                
                if (rows.length == k) {
                    Loader(0);
                }
                
            },
            error: function () {
                Loader(0);
            }
        });
    }
    else {
        image.textContent = "+"
        var f = 0;
        rows.each(function (index) {
            var department = $(this).find('td:first').text().trim().replace(/^\s*\+/, '').trim();
            getStoreWiseDataCopy(department, index + 1, this, "No data");
            f++;
        });
    }
    
}
function getStoreWiseDataCopy(Department, row, obj, states) {
    var img = document.getElementById('img' + row);
    if (img.textContent == "+") {

        var div = document.getElementsByClassName("childrow");
        var Length = div.length;
        
        var txtSpn = document.getElementsByClassName("clsbutton");
        var txtlength = txtSpn.length;
        
        if (states.length > 0) {
            $.each(states, function (j, data) {
                var Sales = data.Sales.toString();
                var htm = "<tr class='childrow child" + row + "'>";
                htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.Department.toString() + "</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.Sales.toString()) + "</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>" + data.TotalSalPercentage.toString() + " %</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.COgs.toString()) + "</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>" + data.SalPercentage.toString() + " %</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PDFAmount.toString()) + "</td>";
                htm += "<td style='font-weight: normal;' class='childtd'>" + data.PDFPercentage.toString() + " %</td>";
                htm += "</tr>";
                $(htm).insertAfter($(obj).closest('tr'));
            })
        }
        obj.title = "Expanded";
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
//Harsh' code End
function getStoreWiseData(Department, row, obj) {
    
    var img = document.getElementById('img' + row);
    if (img.textContent == "+") {

        var div = document.getElementsByClassName("childrow");
        var Length = div.length;
        for (var i = 0; i < Length; i++) {
            
            div[0].remove();
        }
        var txtSpn = document.getElementsByClassName("clsbutton");
        var txtlength = txtSpn.length;
        for (var i = 0; i < txtlength; i++) {
            
            txtSpn[i].textContent = "+";
        }
        var FromDate = $('#txtstartdate').val();
        var ToDate = $('#txtenddate').val();
        var ShiftId = $('#ShiftId').val();

        $.ajax({
            url: '/Report/getStoreWiseData',
            type: "get",
            cache: false,
            data: { Department: Department, FromDate: FromDate, ToDate: ToDate, ShiftId: ShiftId },
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
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.SalPercentage.toString() + " %</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PDFAmount.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.PDFPercentage.toString() + " %</td>";
                        htm += "</tr>";
                        $(htm).insertAfter($(obj).closest('tr'));
                    })
                }
                //window.alert(Department + " - expanded.");
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

function GetStoreExpenseData(row, obj) {
    
    var img = document.getElementById('img' + row);
    var FromDate = $('#txtstartdate').val();
    var ToDate = $('#txtenddate').val();
    var ShiftId = $('#ShiftId').val();
    var StoreId = $('.w3-red').val();
    if (img.textContent == "+") {
        $.ajax({
            url: '/Report/getstorewiseExpenseDetail',
            type: "get",
            cache: false,
            data: { FromDate: FromDate, ToDate: ToDate, StoreId: StoreId, ShiftId: ShiftId },
            beforeSend: function () {
                Loader(1);
            },
            success: function (states) {
                
                if (states.length > 0) {
                    $.each(states, function (j, data) {
                        
                        var htm = "<tr class='childrow child" + row + "'>";
                        htm += "<td style='font-weight: normal;' class='childtd'> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + data.Department.toString() + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.Sales.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.TotalSalPercentage.toString() + " %</td>";
                        /*htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.COgs.toString()) + "</td>";*/
                        if (parseFloat(data.COgs) < 0) {
                            htm += "<td style='font-weight: normal; color: red;' class='childtd'>$" + Comasaperatedvalue(data.COgs.toString()) + "</td>";
                        } else {
                            htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.COgs.toString()) + "</td>";
                        }
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.SalPercentage.toString() + " %</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>$" + Comasaperatedvalue(data.PDFAmount.toString()) + "</td>";
                        htm += "<td style='font-weight: normal;' class='childtd'>" + data.PDFPercentage.toString() + " %</td>";
                        htm += "</tr>";
                        $(htm).insertAfter($(obj).closest('tr'));
                    })
                }
                //window.alert(Department + " - expanded.");
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
}