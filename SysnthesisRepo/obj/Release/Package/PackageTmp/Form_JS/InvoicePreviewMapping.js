$("body").on("click", "#InlineEditing2_searchbutton", function () {

    var grid = document.getElementById('InlineEditing2').ej2_instances[0];
    var value = document.getElementById("InlineEditing2_searchbar").value;
    grid.search(value);
});

$("body").on("click", "#InlineEditing1_searchbutton", function () {

    var grid = document.getElementById('InlineEditing1').ej2_instances[0];
    var value = document.getElementById("InlineEditing1_searchbar").value;
    grid.search(value);
});


function dataBound(e) {

    var grid = document.getElementById("InlineEditing2").ej2_instances[0];
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
function dataBound01(e) {

    var grid = document.getElementById("InlineEditing1").ej2_instances[0];
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