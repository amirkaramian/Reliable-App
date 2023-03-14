using MyReliableSite.Infrastructure.CSVValidator.Validators;

namespace MyReliableSite.Infrastructure.CSVValidator;

internal class ConvertedValidators
{
    public ConvertedValidators()
    {
        Columns = new Dictionary<int, List<IValidator>>();
    }

    public Dictionary<int, List<IValidator>> Columns { get; private set; }

    public string RowSeperator { get; set; }

    public string ColumnSeperator { get; set; }

    public bool HasHeaderRow { get; set; }
    public bool HasTitleRow { get; set; }
}
