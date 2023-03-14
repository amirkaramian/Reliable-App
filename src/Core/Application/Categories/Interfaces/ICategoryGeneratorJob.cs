using MyReliableSite.Application.Common.Interfaces;
using System.ComponentModel;

namespace MyReliableSite.Application.Categories.Interfaces;

public interface ICategoryGeneratorJob : IScopedService
{
    [DisplayName("Generate Random Category example job on Queue notDefault")]
    Task GenerateAsync(int nSeed);

    [DisplayName("removes all radom categories created example job on Queue notDefault")]
    Task CleanAsync();
}