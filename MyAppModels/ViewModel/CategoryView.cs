using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModel
{
    public class CategoryView
    {
        public Category Category { get; set; } = new Category();
        public IEnumerable<Category> categories { get; set; }=new List<Category>();//create karwana k time categories to properties ha wo null arha 
                                                                                    //tha tu humna is k liya new list banyi is ma add hota rha ga
    }
}
