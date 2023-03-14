namespace MyReliableSite.Domain.Common;

public static class StringExtensions
{
    public static string NullToString(this object value)
    {
        return value == null ? string.Empty : value.ToString();
    }
}
