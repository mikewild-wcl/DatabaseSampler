using DatabaseSampler.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSampler.Application.Data;

public class StudentDbContext : DbContext
{
    //public StudentDbContext()
    //{
    //}

    public StudentDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
}
