using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCWEF.Models;

namespace MVCWEF.Models
{
    public class MyViewModel
    {
        public Order order { get; set; }
        public OrderDetail orderdetails { get; set; }
        public Product products { get; set; }
        public User users { get; set; }
    }
}