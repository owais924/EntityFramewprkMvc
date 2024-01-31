using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.Models.ViewModel
{
    public class ProductView
    {
        public Product? Product { get; set; }
        [ValidateNever]
        public IEnumerable<Product> Products { get; set; }= new List<Product>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }//categories ka liya ek dropdown list chya  ha
                                                                   //sari categories product ma aska
    }
}
