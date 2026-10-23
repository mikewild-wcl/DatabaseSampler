using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IMongoDbService
{
    Task AddTeacherAsync(Teacher teacher);

    Task<IList<Teacher>> GetTeachersAsync(string queryString);
}
