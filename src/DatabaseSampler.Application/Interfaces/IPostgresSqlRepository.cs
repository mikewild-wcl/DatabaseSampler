using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IPostgresSqlRepository
{
    Task<int> AddStudentAsync(Student student);
    Task<IList<Student>> GetStudentsAsync();
}
