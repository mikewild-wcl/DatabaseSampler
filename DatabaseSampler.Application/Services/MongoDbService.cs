using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Services;

public class MongoDbService : IMongoDbService
{
    public async Task<IList<Teacher>> GetTeachersAsync(string queryString)
    {
        return [];
    }
}
