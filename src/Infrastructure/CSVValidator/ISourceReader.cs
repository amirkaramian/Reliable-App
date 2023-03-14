namespace MyReliableSite.Infrastructure.CSVValidator;

public interface ISourceReader
{
    IEnumerable<string> ReadLines(string rowSeperator, int hedaercount);
}
