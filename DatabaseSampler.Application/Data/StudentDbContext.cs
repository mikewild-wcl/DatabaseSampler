using DatabaseSampler.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSampler.Application.Data;

public class StudentDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Student> Students { get; set; }
}
