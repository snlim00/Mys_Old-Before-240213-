using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtension
{
    public static bool ContainsComma(this string str)
    {
        return str.Contains(",");
    }

    public static bool EnclosedInQutoes(this string str)
    {
        return str[0] == '\"' && str[^1] == '\"';
    }
}
