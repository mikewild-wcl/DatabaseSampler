using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IMongoDbService
{
    Task<IList<Teacher>> GetTeachersAsync(string queryString);
}
