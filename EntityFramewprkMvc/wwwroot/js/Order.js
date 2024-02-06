//const { Toast } = require("../lib/bootstrap/dist/js/bootstrap.bundle");

var dbtable;
//$(document).ready(function () {
//    dbtable = $('#myTable').Datatable({
//        "ajax": {
//            "url": "/Admin";
//        },
//        "columns": [
//            { "data": "name" },
//            { "data": "description" },
//            { "data": "price" }

//        ]

//    });
//});
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pending")) {
        OrderTable("pending");
    }
    else {
        if (url.includes("approved")) {
            OrderTable("approved");
        }
    }
});
function OrderTable(status) {
    dbtable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Order/AllOrders?status=" + status
        },
        "columns": [
            { "data": "name" },
            { "data": "phone" },
            { "data": "orderStatus" },
            { "data": "orderTotal" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a href="/Admin/order/OrderDetails?id=${data}"><i class="bi bi-pencil-square"></i></a>
                   
                    `
                }
            }

        ]
    });
}
