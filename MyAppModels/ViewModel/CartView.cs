﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.ViewModel
{
    public class CartView
    {
        public IEnumerable<Cart> ListOfCart { get;  set; }
       public OrderHeader OrderHeader { get; set; }
    }
}
