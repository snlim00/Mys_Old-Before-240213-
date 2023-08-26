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
            startIndex += sectionStart.Length; // 섹션 이름 다음 인덱스로 이동
            int endIndex = input.IndexOf("[", startIndex); // 다음 섹션 시작 위치를 찾음

            if (endIndex == -1)
            {
                endIndex = input.Length;
            }

            // 다음 섹션이 없으면 전체 텍스트의 끝까지 추출
            string sectionValue = input.Substring(startIndex, endIndex - startIndex).Trim(); // 좌우 공백 제거
            
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
