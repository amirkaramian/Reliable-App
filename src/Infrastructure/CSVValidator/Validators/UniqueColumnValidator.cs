namespace MyReliableSite.Infrastructure.CSVValidator.Validators;

internal class UniqueColumnValidator : ValidationEntry
{
    private List<string> _entries;

    public UniqueColumnValidator()
    {
        _entries = new List<string>();
    }

    public override bool IsValid(string toCheck)
    {
        bool isValid = !_entries.Contains(toCheck);

        if (isValid)
        {
            _entries.Add(toCheck);
        }
        else
        {
            Errors.Add(new ValidationError(0, string.Format("Duplicate ID {0} found.", toCheck)));
        }

        return isValid;
    }
}
