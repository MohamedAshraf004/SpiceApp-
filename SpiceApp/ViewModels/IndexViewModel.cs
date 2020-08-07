using SpiceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<MenuItem> MenuItems{ get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Coupon> Coupons { get; set; }

    }
}
