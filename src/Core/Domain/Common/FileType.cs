using System.ComponentModel;

namespace MyReliableSite.Domain.Common;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image,
    [Description(".csv")]
    CSV,
    [Description(".dll")]
    Dll
}