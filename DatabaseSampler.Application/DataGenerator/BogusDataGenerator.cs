using Bogus;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace DatabaseSampler.Application.DataGenerator;

public partial class BogusDataGenerator(ILogger<BogusDataGenerator> logger) : IDataGenerator
{
    [LoggerMessage(
        EventId = 101,
        Level = LogLevel.Information,
        Message = "Bogus expense created with Name {Name}, Amount {Amount}, MonthlyCost {MonthlyCost}, Frequency {Frequency}, StartDate {StartDate}.")]
    private static partial void LogBogusExpenseCreated(ILogger logger, string name, decimal amount, decimal monthlyCost, string frequency, DateTime startDate);

   [LoggerMessage(
        EventId = 102,
        Level = LogLevel.Information,
        Message = "Bogus student created with Id={Id}, Name {FirstName} {LastName}.")]
    private static partial void LogBogusStudentCreated(ILogger logger, int id, string firstName, string lastName);

    [LoggerMessage(
        EventId = 103,
        Level = LogLevel.Information,
        Message = "Bogus teacher created with Id={Id}, Name {FirstName} {LastName}, Subject {Subject}, Joined {Joined}.")]
    private static partial void LogBogusTeacherCreated(ILogger logger, int id, string firstName, string lastName, string subject, DateTime joined);

    public Expense CreateExpense() =>
        new Faker<Expense>()
            .RuleFor(e => e.Id, f => Guid.NewGuid())
            .RuleFor(e => e.Name, f => f.Commerce.ProductName())
            .RuleFor(e => e.Amount, f => decimal.Parse(f.Commerce.Price(), CultureInfo.CurrentCulture))
            .RuleFor(e => e.MonthlyCost, f => decimal.Parse(f.Commerce.Price(), CultureInfo.CurrentCulture))
            .RuleFor(s => s.Frequency, f => f.PickRandom<string>(Frequencies.All))
            .RuleFor(e => e.StartDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(e => e.Created, f => f.Date.Past().ToUniversalTime())
            .FinishWith((_, e) =>
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    LogBogusExpenseCreated(logger, e.Name, e.Amount, e.MonthlyCost, e.Frequency, e.StartDate);
                }
            });
        
    public Student CreateStudent() =>
        new Faker<Student>()
            .RuleFor(s => s.Id, f => 0) // Use zero - Id will be assigned when added to a data collection
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Created, f => f.Date.Past().ToUniversalTime())
            .FinishWith((_, s) =>
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    LogBogusStudentCreated(logger, s.Id, s.FirstName, s.LastName);
                }
            });

    public Teacher CreateTeacher() =>
        new Faker<Teacher>()
            .RuleFor(t => t.Id, f => 0) // Use zero - Id will be assigned when added to a data collection
            .RuleFor(t => t.FirstName, f => f.Name.FirstName())
            .RuleFor(t => t.LastName, f => f.Name.LastName())
            .RuleFor(t => t.SpecialistSubject, f => f.PickRandom<string>(SpecialistSubjects.All))
            .RuleFor(t => t.Joined, f => f.Date.Past().ToUniversalTime())
            .RuleFor(t => t.Created, f => f.Date.Past().ToUniversalTime())
            .FinishWith((_, t) =>
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    LogBogusTeacherCreated(logger, t.Id, t.FirstName, t.LastName, t.SpecialistSubject, t.Joined);
                }
            });
}