using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoes.Models
{
    public class MultipleTableClass
    {
        public Models.Billing bill { get; set; }
        public IEnumerable<Cart> cart { get; set; }
    }
}