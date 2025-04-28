using Bogus;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.DataGenerator
{
    public class BogusDataGenerator : IDataGenerator
    {
        public BogusDataGenerator()
        {
        }

        public Student CreateStudent()
        {
            var student = new Faker<Student>()
                //Id should be 0 as it will be assigned when added to a data collection
                .RuleFor(s => s.Id, f => 0)
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.Created, f => f.Date.Past())
                .FinishWith((f, s) => Console.WriteLine($"Student created. Id={s.Id}, Name {s.FirstName} {s.LastName}"));

            return student;
        }
    }
}