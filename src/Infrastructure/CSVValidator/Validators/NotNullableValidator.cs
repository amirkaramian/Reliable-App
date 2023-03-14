namespace MyReliableSite.Infrastructure.CSVValidator.Validators;

internal class NotNullableValidator : ValidationEntry
{
    public override bool IsValid(string toCheck)
    {
        bool isValid = !string.IsNullOrEmpty(toCheck);
        if (!isValid)
        {
            Errors.Add(new ValidationError(0, "Expected a value but none was provided."));
        }

        return isValid;
    }
}
