using DatabaseSampler.Application.Models;

namespace DatabaseSampler.Application.Interfaces;

public interface IDataGenerator
{
    Student CreateStudent();
}
