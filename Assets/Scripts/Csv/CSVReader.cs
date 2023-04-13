using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

//CSVReader.ReadClass의 TValue로 쓰이는 클래스는 IData를 상속받아야 함.
public interface IData
{
    public void Init(object[] param);
}

public static class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<ScriptObject> ReadScript(string path)
    {
        string filePath = Application.dataPath + "/Data/" + path;
        
        StreamReader file = new StreamReader(filePath);
        //path.Log();

        var list = new List<ScriptObject>();

        var lines = Regex.Split(file.ReadToEnd(), LINE_SPLIT_RE);

        file.Close();

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

            ScriptObject entry = new ScriptObject(values);

            list.Add(entry);
        }
        return list;
    }
}
