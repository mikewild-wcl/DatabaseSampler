using DatabaseSampler.Application.DataGenerator;
using Microsoft.Extensions.Logging.Abstractions;

namespace DatabaseSampler.Application.Tests;

public class BogusDataGeneratorTests
{
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
}
