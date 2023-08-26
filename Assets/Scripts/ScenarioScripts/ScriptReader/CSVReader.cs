using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

//CSVReader.ReadClass�� TValue�� ���̴� Ŭ������ IData�� ��ӹ޾ƾ� ��.
public interface IData
{
    public void Init(object[] param);
}

public static class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static ScriptManager ReadScript(int scriptGroupID)
    {
        string filePath = PathManager.CreateScriptPath(scriptGroupID);

        StreamReader file = new(filePath);

        string scriptFile = file.ReadToEnd();

        file.Close();

        ScriptManager scriptMgr = new()
        {
            scriptGroupId = scriptGroupID,
            scripts = CreateScriptList(scriptFile),
            chapter = int.Parse(GetSectionValue(scriptFile, MysSection.chapter)),
            title = GetSectionValue(scriptFile, MysSection.title),
            explain = GetSectionValue(scriptFile, MysSection.explain)
        };

        string statStr = GetSectionValue(scriptFile, MysSection.requiredStat);
        string[] stats = Regex.Split(statStr, SPLIT_RE);

        for (int i = 0; i < stats.Length; ++i)
        {
            string statName = StatInfo.GetStatName(i);

            scriptMgr.requiredStat[statName] = int.Parse(stats[i]);
        }

        return scriptMgr;
    }

    private static string GetSectionValue(string input, string sectionName)
    {
        string sectionStart = "[" + sectionName + "]";
        int startIndex = input.IndexOf(sectionStart);

        if (startIndex != -1)
        {
            startIndex += sectionStart.Length; // ���� �̸� ���� �ε����� �̵�
            int endIndex = input.IndexOf("[", startIndex); // ���� ���� ���� ��ġ�� ã��

            if (endIndex == -1)
            {
                endIndex = input.Length;
            }

            // ���� ������ ������ ��ü �ؽ�Ʈ�� ������ ����
            string sectionValue = input.Substring(startIndex, endIndex - startIndex).Trim(); // �¿� ���� ����
            
            return sectionValue;
        }

        return string.Empty;
    }

    public static List<ScriptObject> CreateScriptList(string input)
    {
        string scriptCsv = GetSectionValue(input, MysSection.script);

        var list = new List<ScriptObject>();

        var lines = Regex.Split(scriptCsv, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || string.IsNullOrEmpty(values[0])) { continue; }

            ScriptObject entry = new ScriptObject(values);

            list.Add(entry);
        }

        return list;
    }

    public static List<ScriptObject> CreateScriptList(int scriptGroupID)
    {
        string filePath = PathManager.CreateScriptPath(scriptGroupID);
        
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
