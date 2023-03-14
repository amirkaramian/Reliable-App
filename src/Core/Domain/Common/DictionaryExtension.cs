using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Domain.Common;

public static class DictionaryExtension
{
    public static string DictionaryToString<Tkey, TValue>(this IDictionary<Tkey, TValue> dictionary)
    {
        string dictionaryString = "{";
        foreach (KeyValuePair<Tkey, TValue> keyValues in dictionary)
        {
            dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
        }

        return dictionaryString.TrimEnd(',', ' ') + "}";
    }
}
