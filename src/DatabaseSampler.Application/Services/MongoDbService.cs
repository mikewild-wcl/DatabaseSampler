using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using DatabaseSampler.Shared;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace DatabaseSampler.Application.Services;

public partial class MongoDbService(
    IMongoClient mongoClient,
    ILogger<MongoDbService> logger) : IMongoDbService
{
    private readonly IMongoClient _mongoClient = mongoClient;
    private readonly ILogger<MongoDbService> _logger = logger;

    [LoggerMessage(
        EventId = 501,
        Level = LogLevel.Information,
        Message = "Teacher created with Id={Id}, Name {FirstName} {LastName}, Subject {Subject}, Joined {Joined}.")]
    private static partial void LogTeacherCreated(ILogger logger, Guid id, string firstName, string lastName, string subject, DateTime joined);

    public async Task AddTeacherAsync(Teacher teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);

        var collection = GetTeachersCollection();
        await collection.InsertOneAsync(teacher);

        LogTeacherCreated(_logger, teacher.Id, teacher.FirstName, teacher.LastName, teacher.SpecialistSubject, teacher.Joined);
    }

    public async Task<IList<Teacher>> GetTeachersAsync(string queryString)
    {
        var collection = GetTeachersCollection();

        var results =
            await (await collection.FindAsync(new BsonDocument()))
            .ToListAsync();

        return [.. results];
    }

    private IMongoCollection<Teacher> GetTeachersCollection()
    {
        var database = _mongoClient.GetDatabase(DatabaseNames.Teachers);
        return database.GetCollection<Teacher>("teacher_details");
    }
}
