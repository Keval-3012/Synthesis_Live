
$(".Remember").click(function () {

    if ($(this).prop('checked') == true) {
        $("#checktext").text("After");
    }
    else {
        $("#checktext").text("Before");
    }
})

$("#btnSave").click(function () {

    var d = new Date();
    var ds = new Date();
    var days = parseInt($("#datecount").val());
    $("#Displaydate").text("");
    if ($("#Remember").prop('checked') == true) {
        d.setDate(ds.getDate() + days);
        var Edate = d.getFullYear() + '-' + parseInt(d.getMonth() + 1) + '-' + d.getDate();

        $("#Displaydate").text(Edate);
    }
    else {
        d.setDate(ds.getDate() - days);
        var Edate = d.getFullYear() + '-' + parseInt(d.getMonth() + 1) + '-' + d.getDate();
        $("#Displaydate").text(Edate);
    }

})

$(document).ready(function () {
    $("#dept").attr('disabled', 'disabled')
    getProductPrice();
})

function getProductPrice() {

    $(".loading-containers").attr("style", "display:block");
    var flag = true;
    var upcCode = $("#sSearchUpc").val();
    var txtfromdate = $("#txtfromdate").val();
    var txttodate = $("#txttodate").val();

    if (txtfromdate == "") {
        if (txttodate != "") {
            $("#errorfrom").text("Select From Date..");
            flag = false;
        }
        else {
            $("#errorfrom").text("");
        }
    }
    if (txttodate == "") {
        if (txtfromdate != "") {
            $("#errorto").text("Select To Date..");
            flag = false;
        }
        else {
            $("#errorto").text();
        }
    }

    if (txtfromdate != "" && txttodate != "") {
        if (Date.parse(txtfromdate) > Date.parse(txttodate)) {
            $("#errorto").text("Please Enter To Date should be greater than From Date.")
            flag = false;
        } else {
            $("#errorto").text("");
        }
    }
    if (flag == true) {
        $.ajax({
            url: ROOTURL + "/ProductPrice/getProductPriceList",
            type: "GET",
            datatype: 'application/json',
            contentType: 'application/json',
            data: { "Upccode": upcCode, "FromDate": txtfromdate, "ToDate": txttodate },
            success: function (data) {

                var parsed = $.parseJSON(data);
                var thed = $("#theadj").html("");
                var tbod = $("#tbodyj").html("");
                if (parsed.length > 0) {
                    var columnsIn = parsed[0];
                    var thead = "";
                    var tbody = "";
                    thead += "<tr>";
                    thead += "<th style='padding: 0px 0px;'><div class='searchbox'><input class='form-control' type='text' placeholder='Search by item' aria-label='Search' id='sSearchUpc'>";
                    thead += "<span id='searchby'> <img src='../../Content/productprice/images/searchicon.svg' alt='searchicon' /></span></div></th>";
                    for (var key in columnsIn) {
                        if (key == "UPC" || key == "Descritpion") {

                        }
                        else {
                            thead += "<th> " + key + " </th>";
                        }
                    }
                    thead += "</tr>";
                    thed.append(thead);
                    for (var i = 0; i < parsed.length; i++) {
                        // get i-th object in the results array
                        var columnsIn = parsed[i];
                        // loop through every key in the object
                        tbody += "<tr>";
                        tbody += "<td>";
                        var getsd = "";
                        var uc = "";
                        for (var key in columnsIn) {

                            if (key == "UPC") {
                                getsd += "<p>" + parsed[i][key] + "<p>";
                                uc = parsed[i][key];
                            }
                            if (key == "Descritpion") {
                                getsd += url = ROOTURL + "/ProductPrice/ProductDetail?UPC=" + uc + "&Desc=" + parsed[i][key] + "' style='text-decoration: none !important;'>" + parsed[i][key] + "<a>";
                            }
                        }
                        tbody += getsd;
                        tbody += "</td>";

                        for (var key in columnsIn) {

                            if (key == "UPC" || key == "Descritpion") {
                            }
                            else {
                                tbody += "<td>";
                                tbody += "<ul class='gridboxs'>";
                                var valusa = parsed[i][key];
                                var valu = parsed[i][key];
                                if (valusa == null) {
                                    valu = "";
                                }

                                var get = valu.split("$");

                                get.forEach(function (item) {

                                    if (item != "") {
                                        var getval = item.split("^");
                                        if (getval[1] == "0") {

                                            tbody += "<li><a href='/Invoices/Details/" + getval[2] + "' class='Green02box change tooltipTop' style='text-decoration: unset;' target='_blank'>" + getval[0] + "'<div class='top'><p>InvoiceDate:" + getval[3] + "<br>InvoiceNumber:" + getval[4] + "<br>VendorName:" + getval[5] + "</p><i></i></div></a></li>";

                                        }
                                        else if (getval[1] == "1") {
                                            tbody += "<li><a href='/Invoices/Details/" + getval[2] + "' class='Green01box change tooltipTop' style='text-decoration: unset;' target='_blank'>" + getval[0] + "<div class='top'><p>InvoiceDate:" + getval[3] + "<br>InvoiceNumber:" + getval[4] + "<br>VendorName:" + getval[5] + "</p><i></i></div></a></li>";

                                        }
                                        else if (getval[1] == "2") {
                                            tbody += "<li><a href='/Invoices/Details/" + getval[2] + "'  class='greybox change tooltipTop' style='text-decoration: unset;' target='_blank'>" + getval[0] + "<div class='top'><p>InvoiceDate:" + getval[3] + "<br>InvoiceNumber:" + getval[4] + "<br>VendorName:" + getval[5] + "</p><i></i></div></a></li>";

                                        }
                                        else if (getval[1] == "3") {
                                            tbody += "<li><a href='/Invoices/Details/" + getval[2] + "' class='Red01box change tooltipTop' style='text-decoration: unset;' target='_blank'>" + getval[0] + "<div class='top'><p>InvoiceDate:" + getval[3] + "<br>InvoiceNumber:" + getval[4] + "<br>VendorName:" + getval[5] + "</p><i></i></div></a></li>";

                                        }
                                        else if (getval[1] == "4") {
                                            tbody += "<li><a href='/Invoices/Details/" + getval[2] + "' class='Red02box change tooltipTop' style='text-decoration: unset;' target='_blank'>" + getval[0] + "<div class='top'><p>InvoiceDate:" + getval[3] + "<br>InvoiceNumber:" + getval[4] + "<br>VendorName:" + getval[5] + "</p><i></i></div></a></li>";

                                        }
                                    }
                                    else {
                                        tbody += "<li>-<li>";
                                    }
                                });
                                tbody += "</ul>";
                                tbody += "</td>";
                            }
                        }
                        tbody += "</tr>";

                    }
                    tbod.append(tbody);
                }
                $(".loading-containers").attr("style", "display:none");
            },
            error: function (Result) {
                $(".loading-containers").attr("style", "display:none");
            }
        });
    }
}

$(document).on('click', '#searchby', function () {
    getProductPrice();
})

function dataBound(e) {

    var grid = document.getElementsByClassName('e-grid')[0].ej2_instances[0];
    //  checks whether the cancel icon is already present or not
    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
        var span = ej.base.createElement('span', {
            id: grid.element.id + '_searchcancelbutton',
            className: 'e-clear-icon'
        });
        span.addEventListener('click', (args) => {
            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
            grid.search("");
        });
        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
    }

}

function onfiltering(e) {

    var CBObj = document.getElementById("Department").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('DepartmentName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}

function onfilteringvendor(e) {

    var CBObj = document.getElementById("Vendor").ej2_instances[0];
    var query = new ej.data.Query();
    query = (e.text !== '') ? query.where('VendorName', 'contains', e.text, true) : query;
    e.updateData(CBObj.dataSource, query)
}

function statusDetail(e) {

    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var ul = document.createElement('ul');
    ul.className = 'gridboxs';
    if (e[e.column.field] != null) {
        var value = e[e.column.field];
        var get = value.split("$");
        var li;
        var a;
        var divch;
        var p;
        get.forEach(function (item) {

            li = document.createElement('li');
            a = document.createElement('a');
            divch = document.createElement('div');
            if (item != "") {
                var getval = item.split("^");

                if (getval[0] !== "0.00") {
                    if (getval[1] == "|0|") {

                        a.href = '/Invoices/Details/' + getval[2];
                        a.className = 'Green02box change tooltipTop';
                        a.style = 'text-decoration: unset;';
                        a.target = '_blank';
                        a.textContent = "$ " + getval[0];
                        divch.className = 'top';
                        divch.innerHTML = "<p>" + getval[3] + "<br>" + getval[5] + "<br> Qty: " + getval[6] + "</p><i></i>";
                        a.appendChild(divch);
                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                    else if (getval[1] == "|1|") {

                        a.href = '/Invoices/Details/' + getval[2];
                        a.className = 'Green01box change tooltipTop';
                        a.style = 'text-decoration: unset;';
                        a.target = '_blank';
                        a.innerHTML = "<div class='top'><p>" + getval[3] + "<br>" + getval[5] + "<br> Qty: " + getval[6] + "</p><i></i></div>" + "$ " + getval[0];

                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                    else if (getval[1] == "|2|") {

                        a.href = '/Invoices/Details/' + getval[2];
                        a.className = 'greybox change tooltipTop';
                        a.style = 'text-decoration: unset;';
                        a.target = '_blank';
                        a.innerHTML = "<div class='top'><p>" + getval[3] + "<br>" + getval[5] + "<br> Qty: " + getval[6] + "</p><i></i></div>" + "$ " + getval[0];
                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                    else if (getval[1] == "|3|") {

                        a.href = '/Invoices/Details/' + getval[2];
                        a.className = 'Red01box change tooltipTop';
                        a.style = 'text-decoration: unset;';
                        a.target = '_blank';
                        a.innerHTML = "<div class='top'><p>" + getval[3] + "<br>" + getval[5] + "<br> Qty: " + getval[6] + "</p><i></i></div>" + "$ " + getval[0];

                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                    else if (getval[1] == "|4|") {

                        a.href = '/Invoices/Details/' + getval[2];
                        a.className = 'Red02box change tooltipTop';
                        a.style = 'text-decoration: unset;';
                        a.target = '_blank';
                        a.innerHTML = "<div class='top'><p>" + getval[3] + "<br>" + getval[5] + "<br> Qty: " + getval[6] + "</p><i></i></div>" + "$ " + getval[0];

                        li.appendChild(a);
                        ul.appendChild(li);
                    }
                }

            }
            else {
                li.textContent = '-';
                ul.appendChild(li);
            }
        });

    }
    else {
        li = document.createElement('li');
        li.textContent = '-';
        ul.appendChild(li);
    }
    div.appendChild(ul);

    return div.outerHTML;
}

function Getdatabycolor(args) {
    showloader();

    var radio = $('input[name="payment"]:checked').val();

    var ColorFilter = "";
    $(".e-checkbox").each(function (index, obj) {

        var x = $(obj).is(":checked");
        if (x == true) {
            ColorFilter += obj.value + ",";
        }
    })
    ColorFilter = ColorFilter.trim();
    $('#gridvaluechange').html('');
    var vendor = document.getElementById('Vendor').ej2_instances[0];
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/ProductPrice/ProductPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, vendorname: vendor.value, ColorFilter: ColorFilter, Radio: radio }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Item', width: 350, template: '#template', minWidth: 300, },
                { field: 'Descritpion', headerText: 'Descritpion', width: 120, visible: false },
                { field: 'Brand', headerText: 'Brand', width: 120, visible: false },
                { field: 'UPC', headerText: 'Upc', width: 100, visible: false },
                { field: 'InvoiceUpc', headerText: 'InvoiceUpc', width: 100, visible: false })

            for (var key in columnsIn) {
                if (key == "UPC") {

                } else if (key == "Descritpion") {
                } else if (key == "InvoiceUpc") {
                } else if (key == "Brand") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, width: 100, template: '#boxItem', textAlign: 'Center', minWidth: 100 })
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 100 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                searchSettings: { fields: ['Descritpion', 'UPC', 'Brand'], operator: 'contains', ignoreCase: true },
                dataBound: function () {
                    grid.autoFitColumns(Field)
                    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
                        var span = ej.base.createElement('span', {
                            id: grid.element.id + '_searchcancelbutton',
                            className: 'e-clear-icon'
                        });
                        span.addEventListener('click', (args) => {
                            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
                            grid.search("");
                        });
                        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
                        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
                    }
                },
                resizeSettings: { mode: "Auto" },
                //pdfQueryCellInfo: pdfQueryCellInfo,
            });
            grid.appendTo('#gridvaluechange');
            hideloader();
            grid.toolbarClick = function (args) {
                
                if (args.item.id === 'gridvaluechange_pdfexport') {
                    showloader();
                    for (var i = 0; i < grid.columns.length; i++) {
                        if (grid.columns[i].headerText == "Item") {
                            grid.columns[i].width = 275;
                        }
                        else {
                            grid.columns[i].width = 130;
                        }
                    }
                    let exportProperties = {
                        fileName: "PriceComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "PriceComparison.xlsx"
                    };
                    grid.excelExport(exportExProperties);
                }
            };

            grid.pdfQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + "\n";
                        }

                    });
                }
            }
            grid.excelQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + ", ";
                        }

                    });
                    args.value = args.value.slice(0, -2);
                }
            }
            grid.pdfExportComplete = () => {
                hideloader();

            }
            grid.excelExportComplete = () => {
                hideloader();
            }
        },
        error: function (response) {
            hideloader();
        }
    });
}

$(document).ready(function () {
    showloader();
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    var radio = $('input[name="payment"]:checked').val();

    $.ajax({
        url: ROOTURL + "/ProductPrice/ProductPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, Radio: radio }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Item', width: 350, template: '#template', minWidth: 300, },
                { field: 'Descritpion', headerText: 'Descritpion', width: 120, visible: false },
                { field: 'Brand', headerText: 'Brand', width: 120, visible: false },
                { field: 'UPC', headerText: 'Upc', width: 100, visible: false },
                { field: 'InvoiceUpc', headerText: 'InvoiceUpc', width: 100, visible: false })

            for (var key in columnsIn) {
                if (key == "UPC") {

                } else if (key == "Descritpion") {
                } else if (key == "InvoiceUpc") {
                } else if (key == "Brand") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, width: 100, template: '#boxItem', textAlign: 'Center', minWidth: 100 })
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 100 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                searchSettings: { fields: ['Descritpion', 'UPC', 'Brand'], operator: 'contains', ignoreCase: true },
                dataBound: function () {
                    grid.autoFitColumns(Field)
                    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
                        var span = ej.base.createElement('span', {
                            id: grid.element.id + '_searchcancelbutton',
                            className: 'e-clear-icon'
                        });
                        span.addEventListener('click', (args) => {
                            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
                            grid.search("");
                        });
                        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
                        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
                    }
                },
                resizeSettings: { mode: "Auto" },
                // pdfQueryCellInfo: pdfQueryCellInfo,
            });
            grid.appendTo('#gridvaluechange');
            hideloader();
            grid.toolbarClick = function (args) {
                
                if (args.item.id === 'gridvaluechange_pdfexport') {
                    showloader();
                    for (var i = 0; i < grid.columns.length; i++) {
                        if (grid.columns[i].headerText == "Item") {
                            grid.columns[i].width = 275;
                        }
                        else {
                            grid.columns[i].width = 130;
                        }
                    }
                    let exportProperties = {
                        fileName: "PriceComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "PriceComparison.xlsx"
                    };
                    grid.excelExport(exportExProperties);
                    //this.excelExport({ hierarchyExportMode: "All" });
                }
            };

            grid.pdfQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                    console.log(args.value);
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + "\n";                        }

                    });
                }
            }
            grid.excelQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + ", ";
                        }

                    });
                    args.value = args.value.slice(0, -2);
                }
            }
            grid.pdfExportComplete = () => {
                hideloader();
            }
            grid.excelExportComplete = () => {
                hideloader();
            }
        },
        error: function (response) {
            hideloader();
        }
    });
});

function OnChange(args) {
    showloader();
    var ia = 1;
    var radio = $('input[name="payment"]:checked').val();
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    var ColorFilter = "";
    $(".e-checkbox").each(function (index, obj) {

        var x = $(obj).is(":checked");
        if (x == true) {
            ColorFilter += obj.value + ",";
        }
    })


    var one = formatDate(new Date(daterangepicker.value[0].toDateString()));
    var two = formatDate(new Date(daterangepicker.value[1].toDateString()));
    var onClickText = "Price Comparison " + one + " To " + two;

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("Date", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }

    ColorFilter = ColorFilter.trim();
    $('#gridvaluechange').html('');
    var vendor = document.getElementById('Vendor').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/ProductPrice/ProductPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, vendorname: vendor.value, ColorFilter: ColorFilter, Radio: radio }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Item', width: 350, template: '#template', minWidth: 300, },
                { field: 'Descritpion', headerText: 'Descritpion', width: 120, visible: false },
                { field: 'Brand', headerText: 'Brand', width: 120, visible: false },
                { field: 'UPC', headerText: 'Upc', width: 100, visible: false },
                { field: 'InvoiceUpc', headerText: 'InvoiceUpc', width: 100, visible: false })

            for (var key in columnsIn) {
                if (key == "UPC") {

                } else if (key == "Descritpion") {
                } else if (key == "InvoiceUpc") {
                } else if (key == "Brand") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, template: '#boxItem', textAlign: 'Center', minWidth: 100, width: 150 })
                    //    Field.push(key);
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 100 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                searchSettings: { fields: ['Descritpion', 'UPC', 'Brand'], operator: 'contains', ignoreCase: true },
                dataBound: function () {
                    grid.autoFitColumns(Field)
                    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
                        var span = ej.base.createElement('span', {
                            id: grid.element.id + '_searchcancelbutton',
                            className: 'e-clear-icon'
                        });
                        span.addEventListener('click', (args) => {
                            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
                            grid.search("");
                        });
                        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
                        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
                    }
                },
                resizeSettings: { mode: "Auto" },
                //pdfQueryCellInfo: pdfQueryCellInfo,
            });
            grid.appendTo('#gridvaluechange');

            hideloader();
            grid.toolbarClick = function (args) {
                
                if (args.item.id === 'gridvaluechange_pdfexport') {
                    showloader();
                    for (var i = 0; i < grid.columns.length; i++) {
                        if (grid.columns[i].headerText == "Item") {
                            grid.columns[i].width = 275;
                        }
                        else {
                            grid.columns[i].width = 130;
                        }
                    }
                    let exportProperties = {
                        fileName: "PriceComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "PriceComparison.xlsx"
                    };
                    grid.excelExport(exportExProperties);
                }
            };

            grid.pdfQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + "\n";
                        }

                    });
                }
            }
            grid.excelQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + ", ";
                        }

                    });
                    args.value = args.value.slice(0, -2);
                }
            }
            grid.pdfExportComplete = () => {

                hideloader();
            }
            grid.excelExportComplete = () => {
                hideloader();
            }
        },
        error: function (response) {
            hideloader();
        }
    });
}

function OnChangeVendor() {
    showloader();

    var radio = $('input[name="payment"]:checked').val();

    var ColorFilter = "";
    $(".e-checkbox").each(function (index, obj) {

        var x = $(obj).is(":checked");
        if (x == true) {
            ColorFilter += obj.value + ",";
        }
    })
    ColorFilter = ColorFilter.trim();
    $('#gridvaluechange').html('');
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    var vendor = document.getElementById('Vendor').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/ProductPrice/ProductPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, vendorname: vendor.value, ColorFilter: ColorFilter, Radio: radio }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Item', width: 350, template: '#template', minWidth: 300, },
                { field: 'Descritpion', headerText: 'Descritpion', width: 120, visible: false },
                { field: 'Brand', headerText: 'Brand', width: 120, visible: false },
                { field: 'UPC', headerText: 'Upc', width: 100, visible: false },
                { field: 'InvoiceUpc', headerText: 'InvoiceUpc', width: 100, visible: false })

            for (var key in columnsIn) {
                if (key == "UPC") {

                } else if (key == "Descritpion") {
                } else if (key == "InvoiceUpc") {
                } else if (key == "Brand") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, template: '#boxItem', textAlign: 'Center', minWidth: 100, width: 150 })
                    //    Field.push(key);
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 100 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                searchSettings: { fields: ['Descritpion', 'UPC', 'Brand'], operator: 'contains', ignoreCase: true },
                dataBound: function () {
                    grid.autoFitColumns(Field)
                    if (!grid.element.getElementsByClassName('e-search')[0].classList.contains('clear')) {
                        var span = ej.base.createElement('span', {
                            id: grid.element.id + '_searchcancelbutton',
                            className: 'e-clear-icon'
                        });
                        span.addEventListener('click', (args) => {
                            document.querySelector('.e-search').getElementsByTagName('input')[0] = "";
                            grid.search("");
                        });
                        grid.element.getElementsByClassName('e-search')[0].appendChild(span);
                        grid.element.getElementsByClassName('e-search')[0].classList.add('clear');
                    }
                },
                resizeSettings: { mode: "Auto" },
                //pdfQueryCellInfo: pdfQueryCellInfo,
            });
            grid.appendTo('#gridvaluechange');
            hideloader();
            grid.toolbarClick = function (args) {
                
                if (args.item.id === 'gridvaluechange_pdfexport') {
                    showloader();
                    for (var i = 0; i < grid.columns.length; i++) {
                        if (grid.columns[i].headerText == "Item") {
                            grid.columns[i].width = 275;
                        }
                        else {
                            grid.columns[i].width = 130;
                        }
                    }
                    let exportProperties = {
                        fileName: "PriceComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "PriceComparison.xlsx"
                    };
                    grid.excelExport(exportExProperties);
                    //this.excelExport({ hierarchyExportMode: "All" });
                }
            };

            grid.pdfQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + "\n";                        }

                    });
                }
            }
            grid.excelQueryCellInfo = (args) => {
                if (args.column.headerText === 'Item') {
                    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("’", "'");
                }
                else {
                    var get = args.value.split("$");
                    args.value = "";
                    get.forEach(function (item) {
                        if (item != "") {
                            var getval = item.split("^");
                            if (getval[0] !== "0.00") {
                                if (getval[1] == "|0|") {
                                    //args.className = 'Green02box';
                                    //args.classList.add("Green02box");
                                }
                                else if (getval[1] == "|1|") {
                                    //args.className = 'Green01box';
                                    //args.classList.add("Green01box");
                                }
                                else if (getval[1] == "|2|") {
                                    //args.className = 'greybox';
                                    //args.classList.add("greybox");
                                }
                                else if (getval[1] == "|3|") {
                                    //args.className = 'Red01box';
                                    //args.classList.add("Red01box");
                                }
                                else if (getval[1] == "|4|") {
                                    //args.className = 'Red02box';
                                    //args.classList.add("Red02box");
                                }
                            }
                            args.value += "$ " + getval[0] + ", ";
                        }

                    });
                    args.value = args.value.slice(0, -2);
                }
            }
            grid.pdfExportComplete = () => {
                hideloader();
            }
            grid.excelExportComplete = () => {
                hideloader();
            }
        },
        error: function (response) {
            hideloader();
        }
    });
}

//function pdfQueryCellInfo(args) {
//    

//    if (args.column.headerText === "Item") {

//        args.column.width = 10;

//    }

//}

//this.columns[10].commands[1].buttonOption.click = function (args) {
//    scrollVal = $(window).scrollTop();
//    var row = new ej.base.closest(evt.target, '.e-row'); // get row element
//    var index = row.getAttribute('aria-rowindex')
//    var id = new ej.base.closest(evt.target, '.e-detailcell').firstChild.id
//    grid4 = document.getElementById(id).ej2_instances[0];
//    var rowData = grid4.currentViewData[index];
//    $("#InvoiceProductId").val(rowData.InvoiceProductId);
//    //dialogObj1.show()
//    var spinnerTarget = document.getElementById('ajax_dialog1')
//    ej.popups.createSpinner({
//        target: spinnerTarget
//    });
//    ej.popups.showSpinner(spinnerTarget);
//    var ajax = new ej.base.Ajax({
//        url: ROOTURL + 'ProductPrice/Unlinkitem',  //render the partial view
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify({ value: rowData })
//    });
//    ajax.send().then((data) => {
//        var toastObj = document.getElementById('toast_type').ej2_instances[0];
//        if (data == "\"Success\"") {
//            toastObj.content = unlinked;
//            toastObj.target = document.body;
//            toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
//        }
//        if (data == "\"Error\"") {
//            toastObj.content = "Something went wrong!";
//            toastObj.target = document.body;
//            toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
//        }
//        grid4.refresh();

//        //$("#dialogTempData").html(data);
//    });
//    ej.popups.hideSpinner(spinnerTarget);
//}

