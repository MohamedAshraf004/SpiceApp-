using Microsoft.EntityFrameworkCore;
using SpiceApp.Data;
using SpiceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _dbContext;

        public CouponService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<bool> AddCoupon(Coupon Coupon)
        {
            await _dbContext.Coupons.AddAsync(Coupon);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<Coupon>> GetAllCoupons()
        {
            return await _dbContext.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetCouponById(int id)
        {
            return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Coupon> GetCouponByName(string couponName)
        {
            return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Name.ToLower() == couponName.ToLower());
        }
        public async Task<bool> DeleteCoupon(int id)
        {
            var Coupon = await _dbContext.Coupons.FindAsync(id);
            _dbContext.Coupons.Remove(Coupon);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateCoupon(Coupon updatedCoupon)
        {
            var Coupon = _dbContext.Coupons.Attach(updatedCoupon);
            Coupon.State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
