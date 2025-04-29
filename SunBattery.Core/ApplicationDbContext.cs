using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SunBattery.Core.Entities;


namespace SunBattery_Api
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ProtocolData> ProtocolDatas { get; set; }
        public DbSet<InfoTime> InfoTimes { get; set; }
    }
    //public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    //{

    //    public DbSet<ProtocolData> ProtocolDatas { get; set; }
    //    public DbSet<InfoTime> InfoTimes { get; set; }
    //}
}
