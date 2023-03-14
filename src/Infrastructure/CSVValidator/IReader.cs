namespace MyReliableSite.Infrastructure.CSVValidator;

internal interface IReader
{
    ValidatorConfiguration Read(string content);
}
