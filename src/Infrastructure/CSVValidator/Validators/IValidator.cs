namespace MyReliableSite.Infrastructure.CSVValidator.Validators;

internal interface IValidator
{
    bool IsValid(string toCheck);

    IList<ValidationError> GetErrors();

    void ClearErrors();
}
