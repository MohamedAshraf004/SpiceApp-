using SpiceApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public interface ICouponService
    {
        Task<bool> AddCoupon(Coupon Coupon);
        Task<bool> DeleteCoupon(int id);
        Task<IEnumerable<Coupon>> GetAllCoupons();
        Task<Coupon> GetCouponById(int id);
        Task<Coupon> GetCouponByName(string couponName);
        Task<bool> UpdateCoupon(Coupon updatedCoupon);
    }
}