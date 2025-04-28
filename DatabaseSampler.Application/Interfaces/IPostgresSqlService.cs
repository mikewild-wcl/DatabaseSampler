using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IPostgresSqlService
{
    Task<int> AddStudentAsync(Student student);
    Task<IList<Student>> GetStudentsAsync();
}
