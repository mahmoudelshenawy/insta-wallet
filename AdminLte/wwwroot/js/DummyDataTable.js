$(document).ready(function () {
    var datatable = $('#example1').dataTable({
        //"dom": 'QBfrtip',
        // "dom": 'Bfl<"top"r>rt<"bottom"p><"clear">',
        // "dom": 'Bfltpi', المعتمد
        "dom": "<'row'<'col-sm-6 offset-3 col-offset-3'B>>" + "<'d-flex mb-2 justify-content-between mt-4'<''f><''l>>" + "<'row'<'col-sm-12'tr>>" +
            "<'row mt-2'<'col-sm-5'i><'col-sm-7'p>>",
        "processing": true,
        "serverSide": true,
        "filter": true,

        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],
        "ajax": {
            "url": "/admin/dummies",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "defaultContent": "-",
            "targets": "_all",
        },

        ],
        "columns": [
            { "data": "id", "name": "Id", "visible": false, "searchable": false },
            { "data": "firstName", "name": "FirstName", orderable: true },
            { "data": "lastName", "name": "LastName", orderable : true },
            { "data": "contact", "name": "Contact", orderable: true },
            { "data": "email", "name": "Email", orderable: true },
            {
                "render": function (data, type, row) { return '<span>' + row.dateOfBirth.split('T')[0] + '</span>' },
                "name": "DateOfBirth"
            },
            {
                "render": function (data, type, row) { return '<a href="#" class="btn btn-danger" onclick=DeleteCustomer("' + row.id + '"); > Delete </a>' },
                "orderable": false
            },
        ]
    });


    //datatable.buttons(["copy", "csv", "excel", "pdf", "print", "colvis"]).container().appendTo('#headerTable');
    //.buttons().container().appendTo('#headerTable');
});