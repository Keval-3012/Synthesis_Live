$(document).ready(function () {
    localStorage.removeItem("checkvalue");
    AddEditRemove();
    $("#InlineEditing_toolbarItems").find(".e-toolbar-items.e-tbar-pos").addClass("gridToolbar");
    $("#InlineEditing_toolbarItems").find(".e-toolbar-items.e-tbar-pos").find(".e-toolbar-left").addClass("gridButtonContainer");
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

var elem;
var dObj;
function create(args) {
    elem = document.createElement('input');
    return elem;
}

function write(args) {
    let multiselectDatasource = [
        { Country: 'France', Id: '1' },
        { Country: 'Germany', Id: '2' },
        { Country: 'Brazil', Id: '3' },
        { Country: 'Switzerland', Id: '4' },
        { Country: 'Venezuela', Id: '5' },
    ];
    dObj = new ej.dropdowns.MultiSelect({
        value: args.rowData[args.column.field] ? args.rowData[args.column.field].split(',') : [],
        placeholder: 'Add KeyWord',
        floatLabelType: 'Never',
        mode: 'Box',
        allowCustomValue: true
    });
    dObj.appendTo(elem);
}

function destroy() {
    dObj.destroy();
}

function read(args) {
    return dObj.value.join(',');
}

function closemodal() {
    $(".divIDClass").hide();
}

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

function GetLinkedItems(args) {
    var grid = document.getElementById("InlineEditing").ej2_instances[0];
    var checked = document.getElementById('checkedLinked').ej2_instances[0];
    var checkvalue = checked.checked;
    localStorage.setItem("checkvalue", checkvalue);
    AddEditRemove();
}

function AddEditRemove(args) {
    // extending the default UrlAdaptor
    var toastObj = document.getElementById('toast_type').ej2_instances[0];
    class CustomAdaptor extends ej.data.UrlAdaptor {
        processResponse(data, ds, query, xhr, request, changes) {
            if (!ej.base.isNullOrUndefined(data.success)) {

                toastObj.content = data.success;
                toastObj.target = document.body;
                toastObj.show({ title: 'Success!', cssClass: 'e-toast-success', icon: 'e-success toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.Error)) {

                toastObj.content = data.Error;
                toastObj.target = document.body;
                toastObj.show({ title: 'Error!', cssClass: 'e-toast-danger', icon: 'e-error toast-icons' });
            }
            if (!ej.base.isNullOrUndefined(data.data))
                return data.data;
            else
                return data;
        }
    }
    var grid = document.querySelector('#InlineEditing').ej2_instances[0];
    grid.dataSource = new ej.data.DataManager({
        url: "/ProductMappingLog/UrlDatasource",
        adaptor: new CustomAdaptor()
    });
};
