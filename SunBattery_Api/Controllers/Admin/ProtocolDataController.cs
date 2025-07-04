using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunBattery.Core.Entities;
using SunBattery_Api.Models.Dtos.ProtocolData;
using SunBattery_Api.Models.ProtocolDatas;

namespace SunBattery_Api.Controllers.Admin
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProtocolDataController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProtocolDataController(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<ProtocolDataOutput>> GetAsync([FromQuery] ProtocolDataInput input)
        {
            var protocolDatas = await _dbContext.ProtocolDatas.AsNoTracking()
                .OrderByDescending(d => d.Date).Take(30).ToListAsync();

            var mapItems =  _mapper.Map<List<ProtocolDataDto>>(protocolDatas);


            return Ok(new ProtocolDataOutput { ProtocolData = mapItems, Total = mapItems.Count});
        }
    }


}
