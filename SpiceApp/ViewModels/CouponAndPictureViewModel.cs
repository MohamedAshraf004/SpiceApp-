using SpiceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.ViewModels
{
    public class CouponAndPictureViewModel
    {
        public Coupon Coupon{ get; set; }
        public byte[] ExistingPhoto { get; set; }

    }
}
