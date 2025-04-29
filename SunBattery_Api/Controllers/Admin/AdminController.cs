using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunBattery.Core.Entities;
using SunBattery_Api.Services.Commands;

namespace SunBattery_Api.Controllers.Admin
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IParseCommand _parseCommand;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(IParseCommand parseCommand,
            ApplicationDbContext dbContext)
        {
            _parseCommand = parseCommand;
            _dbContext = dbContext;
        }

        [HttpGet("start-command")]
        public async Task StartCommand()
        {
            while (true)
            {
               await _parseCommand.ParseCommandStrAsync();
                await Task.Delay(20000);
              //  return "in development ...!";
            }

        }

        [HttpGet("protocol-datas")]
        public async Task<List<ProtocolData>> GetAsync()
        {
            return await _dbContext.ProtocolDatas.AsNoTracking().ToListAsync();
        }
    }
}
