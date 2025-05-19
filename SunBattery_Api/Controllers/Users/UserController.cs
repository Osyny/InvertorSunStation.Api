using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using SunBattery.Core.Entities;
using System.Data;
using SunBattery.Core.Extentions;
using SunBattery.Core.Helpers.SelectList;

namespace SunBattery_Api.Controllers.Users
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;



        public UserController(
            ApplicationDbContext dbContext, 
            RoleManager<IdentityRole> roleManager)
        {

            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        [HttpGet("getRoles")]
        public async Task<List<SelectListItem>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var selectList = new List<SelectListItem>();
            for (var i = 1; i < roles.Count; i++)
            {
                var item = new SelectListItem()
                {
                    Id = Convert.ToInt32(i),
                    Name = roles[i].Name,
                };
                selectList.Add(item);
            }
            return selectList;
        }
    }
}
