using DatabaseSampler.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSampler.Application.Data;

public class LocationDbContext : DbContext
{
    public LocationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Location> Locations { get; set; }
}
