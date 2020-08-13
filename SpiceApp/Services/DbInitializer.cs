﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpiceApp.Data;
using SpiceApp.Models;
using SpiceApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> logger;

        public DbInitializer(ApplicationDbContext dbContext,UserManager<IdentityUser> userManager
                            ,RoleManager<IdentityRole> roleManager,ILogger<DbInitializer> logger)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this.logger = logger;
        }
        
        public async void InitializeAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (_dbContext.Roles.Any(r => r.Name == SD.ManagerUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.FrontDeskUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.KitchenUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Mohamed Ash",
                EmailConfirmed = true,
                PhoneNumber = "1201339358"
            }, "Admin@123").GetAwaiter().GetResult();

            IdentityUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");

            await _userManager.AddToRoleAsync(user, SD.ManagerUser);

        }
    }
}