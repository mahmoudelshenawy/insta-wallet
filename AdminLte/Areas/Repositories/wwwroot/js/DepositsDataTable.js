$(document).ready(function () {
    var datatable = $('#datatable').dataTable({
        "dom": "<'row'<'col-sm-6 offset-3 col-offset-3'B>>" + "<'d-flex mb-2 justify-content-between mt-4'<''f><''l>>" + "<'row'<'col-sm-12'tr>>" +
            "<'row mt-2'<'col-sm-5'i><'col-sm-7'p>>",
        "processing": true,
        "serverSide": true,
        "filter": true,

        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"],
        "ajax": {
            "url": "/admin/deposits/datatable",
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
            {
                "render": function (data, type, row) { return '<span>' + row.createdAt.split('T')[0] + '</span>' },
                "name": "CreatedAt"
            },
            {
                "render": function (data, type, row) { return '<span>' + row.user + '</span>' },
                "name": "user"
            },

            { "data": "amount", "name": "Amount", orderable: true },
            { "data": "fees", "name": "Fees", orderable: true },
            { "data": "totalAmount", "name": "TotalAmount", orderable: true },
            { "data": "currency", "name": "Currency", orderable: true },
            { "data": "paymentType", "name": "PaymentType", orderable: true },
            {
                "render": function (data, type, row) {
                    if (row.status == "Success") {
                        return "<span class='badge badge-success'>Success</span>"
                    } else if (row.status == "Pending") {
                        return "<span class='badge badge-info'>Pending</span>"
                    } else if (row.status == "Blocked") {
                        return "<span class='badge badge-danger'>Blocked</span>"
                    }
                }, "name": "status", orderable: false
            },
            {
                "render": function (data, type, row) { return '<a href="#" class="btn btn-xs btn-info" onclick=DeleteCustomer("' + row.id + '"); > <i class="fa fa-edit"></i> </a>' },
                "orderable": false
            },
        ]
    });
});