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

$(document).ready(function () {
    showloader();

    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/TopsellerPrice/TopSellerPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];

            var columnsIn = parsed[0];
            console.log(columnsIn);
            console.log("22");
            ColumeGrid.push({ headerText: 'Image', width: 60, template: '#ImageDefault', minWidth: 60 },
                { field: 'RowType', headerText: 'RowType', width: 120, visible: false },
                { field: 'Department', headerText: 'Department', width: 70, minWidth: 100 },
                { headerText: 'Item', width: 350, template: '#template', minWidth: 350 },
            )
            for (var key in columnsIn) {
                if (key == "HighLowQty") {
                    ColumeGrid.push({ field: key, headerText: 'Top Seller in', width: 60, template: '#boxItem', textAlign: 'Center', minWidth: 60 })
                    ColumeGrid.push({ field: 'RowType', headerText: '', width: 20, template: '#ListNameBox', textAlign: 'Center', minWidth: 10 })

                }
            }
            for (var key in columnsIn) {
                if (key == "ItemCode") {
                }
                else if (key == "Department") {
                }
                else if (key == "HighQty") {
                }
                else if (key == "RowType") {
                }
                else if (key == "LowQty") {
                }
                else if (key == "AvgQtySold") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. Qty Sold', width: 80, textAlign: 'Center', minWidth: 70 })
                } else if (key == "PerformanceGap") {
                    ColumeGrid.push({ field: key, headerText: 'Performance Gap', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                } else if (key == "AvgPrice") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. price', width: 100, format: 'C2', textAlign: 'Center', minWidth: 100 })
                } else if (key == "LostRevenue") {
                    ColumeGrid.push({ field: key, headerText: 'Total Lost Revenues', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                }
                else if (key == "ItemName") { }
                else {
                    if (key != "HighLowQty") {
                        ColumeGrid.push({ field: key, headerText: key, width: 100, textAlign: 'Center', minWidth: 100 })
                    }
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 99 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                queryCellInfo: QueryCellEvent,
                searchSettings: { fields: ['ItemCode', 'Department'], operator: 'contains', ignoreCase: true },
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
                    //showloader();
                    //for (var i = 0; i < grid.columns.length; i++) {
                    //    if (grid.columns[i].headerText == "Item") {
                    //        grid.columns[i].width = 275;
                    //    }
                    //    else {
                    //        grid.columns[i].width = 130;
                    //    }
                    //}
                    let exportProperties = {
                        fileName: "TopSeller.pdf",
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
                //if (args.column.headerText === 'Item') {
                //    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("'", "`").replace("'", "`").replace("’", "`");
                //}
                //else {
                //    var get = args.value.split("$");
                //    args.value = "";
                //    get.forEach(function (item) {
                //        if (item != "") {
                //            var getval = item.split("^");
                //            if (getval[0] !== "0.00") {
                //                if (getval[1] == "|0|") {
                //                    //args.className = 'Green02box';
                //                    //args.classList.add("Green02box");
                //                }
                //                else if (getval[1] == "|1|") {
                //                    //args.className = 'Green01box';
                //                    //args.classList.add("Green01box");
                //                }
                //                else if (getval[1] == "|2|") {
                //                    //args.className = 'greybox';
                //                    //args.classList.add("greybox");
                //                }
                //                else if (getval[1] == "|3|") {
                //                    //args.className = 'Red01box';
                //                    //args.classList.add("Red01box");
                //                }
                //                else if (getval[1] == "|4|") {
                //                    //args.className = 'Red02box';
                //                    //args.classList.add("Red02box");
                //                }
                //            }
                //            args.value += "$ " + getval[0] + "   ";
                //        }

                //    });
                //}
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

function TypeDetail(e) {
    var span = document.createElement('span');
    span.className = 'VerticleLable';
    if (e[e.column.field] != null) {
        var value = e[e.column.field];
        if (value == 'A') {
            span.textContent = 'Quantities';
        }
        else if (value == 'B') {
            span.textContent = 'Average Price';
        }
        else {
            span.textContent = 'Lost Revenue';
        }
    }
    return span.outerHTML;
}

function statusDetail(e) {

    var grid = document.querySelector(".e-grid").ej2_instances[0]
    var div = document.createElement('div');
    var ul = document.createElement('ul');
    ul.className = 'gridboxs';
    if (e[e.column.field] != null) {
        var value = e[e.column.field];
        var get = value.split("/");
        var li;
        var a;
        var divch;
        var p;

        console.log(value);
        li = document.createElement('li');
        a = document.createElement('a');
        divch = document.createElement('div');
        if (value != "") {
            a.href = '#';
            a.className = 'highValSpan';
            a.style = 'text-decoration: unset;';
            a.target = '_blank';
            a.textContent = get[0];

            li.appendChild(a);
            ul.appendChild(li);
        }
        else {
            li.textContent = item;
            ul.appendChild(li);
        }
    }
    else {
        li = document.createElement('li');
        li.textContent = '-';
        ul.appendChild(li);
    }
    div.appendChild(ul);
    return div.outerHTML;
}

function QueryCellEvent(args) {
    if (args.column.headerText === 'Image') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Department') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Item') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'High Seller Qty') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Low Seller Qty') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Top Seller in') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Avg. Qty Sold') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Performance Gap') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Avg. price') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'Total Lost Revenues') {
        args.rowSpan = 3;
    }
    else if (args.column.headerText === 'RowType') {
    }
    else if (args.column.headerText === '') {
    }
    else {
        if (args.data.RowType.toString() === 'A') {
            if (args.data[args.column.field].toString() == "0") {
                var num = args.data[args.column.field].toLocaleString(undefined, { minimumFractionDigits: 2 });
                args.cell.innerHTML = '<span class="zeroQty"> ' + num + '</span>';
                args.cell.style.textAlign = "center";
            }
        }
        else if (args.data.RowType.toString() === 'B') {
            if (args.data[args.column.field].toString().split('-').length == 2) {
                var num = args.data[args.column.field].toLocaleString(undefined, { minimumFractionDigits: 2 });
                args.cell.innerText = '-$' + num.split('-')[1];
                args.cell.style.textAlign = "center";
            }
            else {
                var num = args.data[args.column.field].toLocaleString(undefined, { minimumFractionDigits: 2 });
                args.cell.innerText = '$' + num;
                args.cell.style.textAlign = "center";
            }
        }
        else if (args.data.RowType.toString() === 'C') {
            if (args.data[args.column.field].toString().split('-').length == 2) {
                var num = args.data[args.column.field].toLocaleString(undefined, { minimumFractionDigits: 2 });
                args.cell.innerHTML = '<span class="lostRevenueRed"> -$' + num.split('-')[1] + '</span>';
                args.cell.style.textAlign = "center";
            }
            else {
                var num = args.data[args.column.field].toLocaleString(undefined, { minimumFractionDigits: 2 });
                args.cell.innerHTML = ' <span class="lostRevenueGreen"> $' + num + '</span>';
                args.cell.style.textAlign = "center";
            }
        }
    }
}

function OnChange(args) {
    showloader();
    var ColorFilter = "";
    $('#gridvaluechange').html('');
    var Department = document.getElementById('Department').ej2_instances[0];

    var one = formatDate(new Date(args.value[0].toDateString()));
    var two = formatDate(new Date(args.value[1].toDateString()));
    var onClickText = "Top Seller Items " + one + " To " + two;

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
        url: ROOTURL + "/TopSellerPrice/TopSellerPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: args.value, Departmentname: Department.value, ColorFilter: ColorFilter }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Image', width: 60, template: '#ImageDefault', minWidth: 60 },
                { field: 'RowType', headerText: 'RowType', width: 120, visible: false },
                { field: 'Department', headerText: 'Department', width: 70, minWidth: 100 },
                { headerText: 'Item', width: 350, template: '#template', minWidth: 350 },
            )
            for (var key in columnsIn) {
                if (key == "HighLowQty") {
                    ColumeGrid.push({ field: key, headerText: 'Top Seller in', width: 100, template: '#boxItem', textAlign: 'Center', minWidth: 100 })
                    ColumeGrid.push({ field: 'RowType', headerText: '', width: 20, template: '#ListNameBox', textAlign: 'Center', minWidth: 10 })

                }
            }
            for (var key in columnsIn) {
                if (key == "ItemCode") {
                }
                else if (key == "Department") {
                }
                else if (key == "HighQty") {
                }
                else if (key == "RowType") {
                }
                else if (key == "LowQty") { }
                else if (key == "ItemName") { }
                else if (key == "AvgQtySold") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. Qty Sold', width: 80, textAlign: 'Center', minWidth: 70 })
                } else if (key == "PerformanceGap") {
                    ColumeGrid.push({ field: key, headerText: 'Performance Gap', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                } else if (key == "AvgPrice") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. price', width: 100, format: 'C2', textAlign: 'Center', minWidth: 100 })
                } else if (key == "LostRevenue") {
                    ColumeGrid.push({ field: key, headerText: 'Total Lost Revenues', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                }
                else {
                    if (key != "HighLowQty") {
                        ColumeGrid.push({ field: key, headerText: key, width: 100, textAlign: 'Center', minWidth: 100 })
                    }
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 99 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                queryCellInfo: QueryCellEvent,
                searchSettings: { fields: ['ItemCode', 'Department'], operator: 'contains', ignoreCase: true },
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
                    //showloader();
                    //for (var i = 0; i < grid.columns.length; i++) {
                    //    if (grid.columns[i].headerText == "Item") {
                    //        grid.columns[i].width = 275;
                    //    }
                    //    else {
                    //        grid.columns[i].width = 130;
                    //    }
                    //}
                    let exportProperties = {
                        fileName: "TopSeller.pdf",
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
                //if (args.column.headerText === 'Item') {
                //    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("'", "`").replace("'", "`").replace("’", "`");
                //}
                //else {
                //    var get = args.value.split("$");
                //    args.value = "";
                //    get.forEach(function (item) {
                //        if (item != "") {
                //            var getval = item.split("^");
                //            if (getval[0] !== "0.00") {
                //                if (getval[1] == "|0|") {
                //                    //args.className = 'Green02box';
                //                    //args.classList.add("Green02box");
                //                }
                //                else if (getval[1] == "|1|") {
                //                    //args.className = 'Green01box';
                //                    //args.classList.add("Green01box");
                //                }
                //                else if (getval[1] == "|2|") {
                //                    //args.className = 'greybox';
                //                    //args.classList.add("greybox");
                //                }
                //                else if (getval[1] == "|3|") {
                //                    //args.className = 'Red01box';
                //                    //args.classList.add("Red01box");
                //                }
                //                else if (getval[1] == "|4|") {
                //                    //args.className = 'Red02box';
                //                    //args.classList.add("Red02box");
                //                }
                //            }
                //            args.value += "$ " + getval[0] + "   ";
                //        }

                //    });
                //}
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
    $('#gridvaluechange').html('');
    var daterangepicker = document.getElementById('daterangepicker').ej2_instances[0];
    var Department = document.getElementById('Department').ej2_instances[0];
    $.ajax({
        url: ROOTURL + "/TopSellerPrice/TopSellerPriceGrid",
        type: "POST",
        data: JSON.stringify({ value: daterangepicker.value, Departmentname: Department.value, ColorFilter: ColorFilter }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            var parsed = $.parseJSON(response);
            var ColumeGrid = [];
            var Field = [];
            var columnsIn = parsed[0];
            ColumeGrid.push({ headerText: 'Image', width: 60, template: '#ImageDefault', minWidth: 60 },
                { field: 'RowType', headerText: 'RowType', width: 120, visible: false },
                { field: 'Department', headerText: 'Department', width: 120, minWidth: 100 },
                { headerText: 'Item', width: 350, template: '#template', minWidth: 350 },
            )
            for (var key in columnsIn) {
                if (key == "HighLowQty") {
                    ColumeGrid.push({ field: key, headerText: 'Top Seller in', width: 100, template: '#boxItem', textAlign: 'Center', minWidth: 100 })
                    ColumeGrid.push({ field: 'RowType', headerText: '', width: 20, template: '#ListNameBox', textAlign: 'Center', minWidth: 10 })
                }
            }
            for (var key in columnsIn) {
                if (key == "ItemCode") {
                }
                else if (key == "Department") {
                }
                else if (key == "HighQty") {
                }
                else if (key == "RowType") {
                }
                else if (key == "LowQty") { }
                else if (key == "ItemName") { }
                else if (key == "AvgQtySold") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. Qty Sold', width: 80, textAlign: 'Center', minWidth: 70 })
                } else if (key == "PerformanceGap") {
                    ColumeGrid.push({ field: key, headerText: 'Performance Gap', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                } else if (key == "AvgPrice") {
                    ColumeGrid.push({ field: key, headerText: 'Avg. price', width: 100, format: 'C2', textAlign: 'Center', minWidth: 100 })
                } else if (key == "LostRevenue") {
                    ColumeGrid.push({ field: key, headerText: 'Total Lost Revenues', format: 'C2', width: 100, textAlign: 'Center', minWidth: 100 })
                }
                else {
                    if (key != "HighLowQty") {
                        ColumeGrid.push({ field: key, headerText: key, width: 100, textAlign: 'Center', minWidth: 100 })
                    }
                }
            }
            /*var parsed = JSON.stringify(response);*/
            var grid = new ej.grids.Grid({
                dataSource: parsed,
                columns: ColumeGrid,
                gridLines: 'Both',
                enableStickyHeader: true,
                allowPaging: true,
                pageSettings: { pageSize: 99 },
                toolbar: ['Search', 'PdfExport', 'ExcelExport'],
                allowPdfExport: true,
                allowExcelExport: true,
                queryCellInfo: QueryCellEvent,
                searchSettings: { fields: ['ItemCode', 'Department'], operator: 'contains', ignoreCase: true },
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
                    //showloader();
                    //for (var i = 0; i < grid.columns.length; i++) {
                    //    if (grid.columns[i].headerText == "Item") {
                    //        grid.columns[i].width = 275;
                    //    }
                    //    else {
                    //        grid.columns[i].width = 130;
                    //    }
                    //}
                    let exportProperties = {
                        fileName: "TopSeller.pdf",
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
                //if (args.column.headerText === 'Item') {
                //    args.value = args.data["UPC"] + " / " + (args.data["Descritpion"]).replace("'", "`").replace("'", "`").replace("’", "`");
                //}
                //else {
                //    var get = args.value.split("$");
                //    args.value = "";
                //    get.forEach(function (item) {
                //        if (item != "") {
                //            var getval = item.split("^");
                //            if (getval[0] !== "0.00") {
                //                if (getval[1] == "|0|") {
                //                    //args.className = 'Green02box';
                //                    //args.classList.add("Green02box");
                //                }
                //                else if (getval[1] == "|1|") {
                //                    //args.className = 'Green01box';
                //                    //args.classList.add("Green01box");
                //                }
                //                else if (getval[1] == "|2|") {
                //                    //args.className = 'greybox';
                //                    //args.classList.add("greybox");
                //                }
                //                else if (getval[1] == "|3|") {
                //                    //args.className = 'Red01box';
                //                    //args.classList.add("Red01box");
                //                }
                //                else if (getval[1] == "|4|") {
                //                    //args.className = 'Red02box';
                //                    //args.classList.add("Red02box");
                //                }
                //            }
                //            args.value += "$ " + getval[0] + "   ";
                //        }

                //    });
                //}
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
