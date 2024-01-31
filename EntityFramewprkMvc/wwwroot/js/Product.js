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
   dbtable= $('#myTable').dataTable({
       "ajax": {
           "url":"/Admin/Product/AllProducts/"
       },
       "columns": [
           { "data": "name" },
           { "data": "description" },
           { "data": "price" },
           { "data": "category.name" },
           {
               "data": "id",
               "render": function (data) {
                   return `
                    <a href="/Admin/Product/CreateUpdate?id=${data}"><i class="bi bi-pencil-square"></i></a>
                    <a onClick=RemoveProduct("/Admin/Product/Delete/${data}")><i class="bi bi-trash"></i></a>
                    `
               }
           }
       ]
    });
});
function RemoveProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Delete!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                tpye: 'Delete',
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}