using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoes.Models
{
    public class Cart
    {
        public int P_ID { get; set; }
        public string P_Name { get; set; }
        public string P_Pic { get; set; }
        public int qty { get; set; }
        public int stock { get; set; }
        public int P_Price { get; set; }
        public int P_Totalprice { get; set; }

    }
}