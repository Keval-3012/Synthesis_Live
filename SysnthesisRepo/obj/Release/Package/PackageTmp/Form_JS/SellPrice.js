$(window).scroll(function () {

    var scroll = $(window).scrollTop();
    if (scroll >= 50) {
        $(".page-header-main").addClass("stikyheader");
    } else {
        $(".page-header-main").removeClass("stikyheader");
    }
});

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

function undofilters() {
    localStorage.removeItem("Colorvalue");
    window.location.href = ROOTURL + "/SellPrice/SellPrice";
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

            console.log(item);
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
    var ColorFilter = "";
    $(".e-checkbox").each(function (index, obj) {

        var x = $(obj).is(":checked");
        if (x == true) {
            ColorFilter += obj.value + ",";
        }
    })
    ColorFilter = ColorFilter.trim();
    $('#gridvaluechange').html('');
    var Department = document.getElementById('Department').ej2_instances[0];
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/SellPrice/SellPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, Departmentname: Department.value, ColorFilter: ColorFilter }),
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
                } else if (key == "A7484Qty") {
                } else if (key == "AMaywoodQty") {
                } else if (key == "A1407Qty") {
                } else if (key == "A2840Qty") {
                } else if (key == "A180Qty") {
                } else if (key == "A14StreetQty") {
                } else if (key == "A170Qty") {
                } else if (key == "A2589Qty") {
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
                        fileName: "SellComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "SellComparison.xlsx"
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
    $.ajax({
        url: ROOTURL + "/SellPrice/SellPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Item', width: 350, template: '#template', minWidth: 300 },
                { field: 'Descritpion', headerText: 'Descritpion', width: 120, visible: false },
                { field: 'Brand', headerText: 'Brand', width: 120, visible: false },
                { field: 'UPC', headerText: 'Upc', width: 100, visible: false },
                { field: 'InvoiceUpc', headerText: 'InvoiceUpc', width: 100, visible: false })

            for (var key in columnsIn) {
                if (key == "UPC") {

                } else if (key == "Descritpion") {
                } else if (key == "InvoiceUpc") {
                } else if (key == "Brand") {
                } else if (key == "A7484Qty") {
                } else if (key == "AMaywoodQty") {
                } else if (key == "A1407Qty") {
                } else if (key == "A2840Qty") {
                } else if (key == "A180Qty") {
                } else if (key == "A14StreetQty") {
                } else if (key == "A170Qty") {
                } else if (key == "A2589Qty") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, width: 100, template: '#boxItem', textAlign: 'Center', minWidth: 100 })
                }
            }
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
                        fileName: "SellComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "SellComparison.xlsx"
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
});

function gridComplete(args) {

    var grid4 = document.getElementById('InlineEditingU').ej2_instances[0];
    if ($("#hdnindex")[0] !== undefined) {

        var b = parseInt($("#hdnindex")[0].outerText);
        grid4.selectRow(b);
    }
}

function OnChange(args) {
    showloader();
    var ColorFilter = "";
    $(".e-checkbox").each(function (index, obj) {

        var x = $(obj).is(":checked");
        if (x == true) {
            ColorFilter += obj.value + ",";
        }
    })
    ColorFilter = ColorFilter.trim();
    $('#gridvaluechange').html('');
    var Department = document.getElementById('Department').ej2_instances[0];

    var one = formatDate(new Date(args.value[0].toDateString()));
    var two = formatDate(new Date(args.value[1].toDateString()));
    var onClickText = "Sales Price Comparison " + one + " To " + two;

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


    $.ajax({
        url: ROOTURL +"/SellPrice/SellPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: args.value, Departmentname: Department.value, ColorFilter: ColorFilter }),
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
                } else if (key == "A7484Qty") {
                } else if (key == "AMaywoodQty") {
                } else if (key == "A1407Qty") {
                } else if (key == "A2840Qty") {
                } else if (key == "A180Qty") {
                } else if (key == "A14StreetQty") {
                } else if (key == "A170Qty") {
                } else if (key == "A2589Qty") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, template: '#boxItem', textAlign: 'Center', minWidth: 100, width: 150 })
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
                        fileName: "SellComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "SellComparison.xlsx"
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
    var Department = document.getElementById('Department').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/SellPrice/SellPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, Departmentname: Department.value, ColorFilter: ColorFilter }),
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
                } else if (key == "A7484Qty") {
                } else if (key == "AMaywoodQty") {
                } else if (key == "A1407Qty") {
                } else if (key == "A2840Qty") {
                } else if (key == "A180Qty") {
                } else if (key == "A14StreetQty") {
                } else if (key == "A170Qty") {
                } else if (key == "A2589Qty") {
                } else {
                    ColumeGrid.push({ field: key, headerText: key, template: '#boxItem', textAlign: 'Center', minWidth: 100, width: 150 })
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
                        fileName: "SellComparison.pdf",
                        pageOrientation: 'Landscape',
                        pageSize: 'A3',
                    };
                    grid.pdfExport(exportProperties);
                } else if (args.item.id === 'gridvaluechange_excelexport') {
                    showloader();
                    let exportExProperties = {
                        fileName: "SellComparison.xlsx"
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
