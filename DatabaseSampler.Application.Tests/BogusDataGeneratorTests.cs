using DatabaseSampler.Application.DataGenerator;

namespace DatabaseSampler.Application.Tests;

public class BogusDataGeneratorTests
{
    [Fact]
    public void Create_One_Student_Returns_Expected_Result()
    {
        var generator = new BogusDataGenerator();

        var result = generator.CreateStudent();

        result.Should().NotBeNull();

        result.Id.Should().Be(0);
        result.FirstName.Should().NotBeNull();
        result.LastName.Should().NotBeNull();
    }
}
