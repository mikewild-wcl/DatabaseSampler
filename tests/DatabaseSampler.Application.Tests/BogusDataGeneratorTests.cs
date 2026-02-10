using DatabaseSampler.Application.DataGenerator;
using DatabaseSampler.Application.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace DatabaseSampler.Application.Tests;

public class BogusDataGeneratorTests
{
    [Fact]
    public void Create_One_Expense_Returns_Expected_Result()
    {
        var generator = new BogusDataGenerator(NullLogger<BogusDataGenerator>.Instance);
        var result = generator.CreateExpense();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldNotBeNull();
        result.Amount.ShouldBeGreaterThan(0);
        result.Frequency.ShouldNotBeNull();
        Frequencies.All.ShouldContain(result.Frequency);
    }

    [Fact]
    public void Create_One_Student_Returns_Expected_Result()
    {
        var generator = new BogusDataGenerator(NullLogger<BogusDataGenerator>.Instance);

        var result = generator.CreateStudent();

        result.ShouldNotBeNull();

        result.Id.ShouldBe(0);
        result.FirstName.ShouldNotBeNull();
        result.LastName.ShouldNotBeNull();
    }

    [Fact]
    public void Create_One_Teacher_Returns_Expected_Result()
    {
        var generator = new BogusDataGenerator(NullLogger<BogusDataGenerator>.Instance);

        var result = generator.CreateTeacher();

        result.ShouldNotBeNull();

        result.Id.ShouldBe(0);
        result.FirstName.ShouldNotBeNull();
        result.LastName.ShouldNotBeNull();
        result.SpecialistSubject.ShouldNotBeNull();
        SpecialistSubjects.All.ShouldContain(result.SpecialistSubject);
    }
}
