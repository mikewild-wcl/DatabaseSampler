using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSampler.Application.Data;

public class PostgresSqlRepository : IPostgresSqlRepository
{
    private readonly StudentDbContext _studentContext;

    public PostgresSqlRepository(StudentDbContext studentContext)
    {
        _studentContext = studentContext;
    }

    public async Task<int> AddStudentAsync(Student student)
    {
        await _studentContext.AddAsync(student);

        await _studentContext.SaveChangesAsync();

        return student.Id;
    }

    public async Task<IList<Student>> GetStudentsAsync()
    {
        return await _studentContext.Students.ToListAsync();
    }
}
