using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Services;

public class PostgresSqlService(IPostgresSqlRepository repository) : IPostgresSqlService
{
    private readonly IPostgresSqlRepository _repository = repository;

    public async Task<int> AddStudentAsync(Student student)
    {
        return await _repository.AddStudentAsync(student);
    }

    public async Task<IList<Student>> GetStudentsAsync()
    {
        return await _repository.GetStudentsAsync();
    }
}
