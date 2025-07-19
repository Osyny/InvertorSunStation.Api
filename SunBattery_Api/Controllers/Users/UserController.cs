using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Station.Web.Controllers.Users.Dtos;
using SunBattery.Core;
using SunBattery.Core.Entities;
using SunBattery.Core.Extentions;
using SunBattery.Core.Helpers.SelectList;
using SunBattery_Api.Models;
using SunBattery_Api.Models.Authentification.SignUp;
using SunBattery_Api.Models.Dtos.Users;
using SunBattery_Api.Models.EmailSenderModels;
using SunBattery_Api.Services.EmailServices;
using SunBattery_Api.Services.UserManagements;
using System.Data;

namespace SunBattery_Api.Controllers.Users
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUserManagement _userManagement;
        private readonly IEmailService _emailService;

        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(
            ApplicationDbContext dbContext, 
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IUserManagement userManagement,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManagement = userManagement;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpGet("getAllUsers")]
        public async Task<UsersDataOutput> GetAllUsers([FromQuery] UsersInputData input)
        {
            IQueryable<ApplicationUser> query;
            if (!string.IsNullOrWhiteSpace(input.FilterText))
            {
                input.FilterText = input.FilterText?.ToLower();
                var queryFilter = _dbContext.Users.Where(u => u.FirstName.Contains(input.FilterText) ||
                    u.LastName.ToLower().Contains(input.FilterText) ||
                    u.UserName.ToLower().Contains(input.FilterText) ||
                    u.Email.ToLower().Contains(input.FilterText)).AsNoTracking().AsQueryable();

                query = queryFilter;
            }
            else
            {
                query = _dbContext.Users.AsNoTracking().AsQueryable();
            }
            var count = await query.CountAsync();
            IList<ApplicationUser> sortQuery = await GetSortQuery(input, query);

            List<UserDto> mapUsers = new List<UserDto>();
            foreach (var user in sortQuery)
            {
                var userRoles = await _userManagement.GerUserRoleAsync(user);
                var mapUser = MapUserEntityDto(user, (List<string>)userRoles);
                mapUsers.Add(mapUser);
            }


            return new UsersDataOutput() { Users = mapUsers, Total = count };
        }


        [HttpGet("getRoles")]
        public async Task<List<SelectListItem>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var selectList = new List<SelectListItem>();
            for (var i = 0; i < roles.Count; i++)
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

        [HttpPut("{id}/update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This User Doesnot exist!" });
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var existRoles = await _userManagement.GerUserRoleAsync(user);
                if (existRoles != null)
                {
                    foreach (var role in existRoles)
                    {
                        if (!userDto.Roles.Contains(role))
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                    }

                }
                foreach (var newRole in userDto.Roles)
                {
                    await _userManagement.AssignRoleToUserAsync(userDto.Roles, user);
                }

                return StatusCode(StatusCodes.Status200OK,
                    new Response
                    {
                        IsSuccess = true,
                        Message = "user successfully updated!",
                    });
            }
            var res = new Response { IsSuccess = false };
            var errors = "";
            foreach (var error in result.Errors)
            {
                errors += $", {error.Description}";
            }

            return StatusCode(StatusCodes.Status500InternalServerError, res);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This User Doesnot exist!" });
            }
          
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK,
                        new Response
                        {
                            Status = "Ok"
                        });

        }
        private async Task<IList<ApplicationUser>> GetSortQuery(UsersInputData input, IQueryable<ApplicationUser> query)
        {
            var parse = input?.Sorting?.Split(" ");

            IList<ApplicationUser>? sortQuery = null;

            if (parse != null && parse.Count() > 1)
            {
                var type = parse[0].First().ToString().ToUpper() + parse[0].Substring(1);
                var propertyInfo = typeof(ApplicationUser).GetProperty(type);

                switch (parse[1])
                {
                    case "asc":
                        sortQuery = await query.OrderByField(type, true).Skip((int)(input?.Skip)).
                             Take((int)(input?.Rows)).AsNoTracking().ToListAsync();

                        return sortQuery;
                    case "desc":
                        sortQuery = await query.OrderByField(type, false).Skip((int)(input?.Skip)).
                            Take((int)(input?.Rows)).AsNoTracking().ToListAsync();
                        return sortQuery;

                }
            }
            else
            {
                sortQuery = await query.OrderBy(p => p.UserName).Skip((int)(input?.Skip)).
                    Take((int)(input?.Rows)).AsNoTracking().ToListAsync();
            }
            return sortQuery;
        }

        private UserDto MapUserEntityDto(ApplicationUser user, List<string> roles)
        {
            var map = _mapper.Map<UserDto>(user);
            
            map.Roles = roles;
            return map;
        }

    }
}
