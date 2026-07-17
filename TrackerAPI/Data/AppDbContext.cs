using Microsoft.EntityFrameworkCore;
using Shared;

namespace TrackerAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<GpsPosition> GpsPosition {get;set;}
}