﻿namespace MyReliableSite.Infrastructure.CSVValidator.Validators;

internal abstract class ValidationEntry : IValidator
{
    private List<ValidationError> _errors;

    public ValidationEntry()
    {
        _errors = new List<ValidationError>();
    }

    public abstract bool IsValid(string toCheck);

    public virtual IList<ValidationError> GetErrors()
    {
        return _errors;
    }

    public virtual void ClearErrors()
    {
        _errors.Clear();
    }

    protected List<ValidationError> Errors
    {
        get { return _errors; }
    }
}
