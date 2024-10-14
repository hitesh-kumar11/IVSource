var dataTable;

jQuery(document).ready(function ($) {
    loadDataTable;
});

function loadDataTable()
{
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/editcountry/getall/",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "SerialNum", "width": "20%" },
            { "data": "CountryName", "width": "20%" },
            { "data": "CountryISO", "width": "20%" },
            { "data": "IsEnable", "width": "20%" }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}