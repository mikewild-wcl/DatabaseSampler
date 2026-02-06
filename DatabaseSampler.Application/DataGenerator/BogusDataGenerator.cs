using Bogus;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using Microsoft.Extensions.Logging;

namespace DatabaseSampler.Application.DataGenerator;

public partial class BogusDataGenerator(ILogger<BogusDataGenerator> logger) : IDataGenerator
{
    [LoggerMessage(
        EventId = 101, 
        Level = LogLevel.Information, 
        Message = "Bogus student created with Id={Id}, Name {FirstName} {LastName}.")]
    public static partial void LogBogusStudentCreated(ILogger logger, int id, string firstName, string lastName);

    public Student CreateStudent()
    {
        var student = new Faker<Student>()
            //Id should be 0 as it will be assigned when added to a data collection
            .RuleFor(s => s.Id, f => 0)
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Created, f => f.Date.Past().ToUniversalTime())
            .FinishWith((f, s) => {
                if (logger.IsEnabled(LogLevel.Trace)) 
                {
                    LogBogusStudentCreated(logger, s.Id, s.FirstName, s.LastName);
                }
            });

        return student;
    }
}