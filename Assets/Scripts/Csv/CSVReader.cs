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

    public static (Dictionary<int, Dictionary<TKey, TValue>>, Dictionary<TName, Dictionary<TKey, TValue>>) ReadTable<TKey, TValue, TName>(string path)
    {
        string filePath = Application.dataPath + "/" + path;

        StreamReader file = new StreamReader(filePath);
        Debug.Log(filePath);

        var idList = new Dictionary<int, Dictionary<TKey, TValue>>();
        var nameList = new Dictionary<TName, Dictionary<TKey, TValue>>();

        string[] lines = Regex.Split(file.ReadToEnd(), LINE_SPLIT_RE);

        file.Close();

        TValue ConvertValue(string value)
        {
            return (TValue)Convert.ChangeType(value, typeof(TValue));
        }

        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for (int i = 1; i < lines.Length; i++)
        {
            //i.Log();
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            //values.Log();
            if (values.Length == 0 || values[0] == "")
            {
                continue;
            }

            var idEntry = new Dictionary<TKey, TValue>();
            var nameEntry = new Dictionary<TKey, TValue>();

            for (int j = Constants.essentialKeyCount; j < header.Length && j < values.Length; j++)
            {
                //i.Log();
                string _value = values[j];
                _value = _value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                _value = _value.Replace("<br>", "\n");
                _value = _value.Replace("<c>", ",");

                TKey key = (TKey)(object)j;

                TValue value = ConvertValue(_value);

                idEntry.Add(key, value);
                nameEntry.Add(key, value);
            }
            int id = Convert.ToInt32(values[Constants.idKey]); //��� ���̺��� ù ��° ���� ���� id���� ��.
            TName name = (TName)Enum.Parse(typeof(TName), values[Constants.nameKey]); //��� ���̺��� �� ��° ���� ���� name�̾�� ��.

            //idEntry.Add((TKey)(object)Constants.nameKey, ConvertValue(values[Constants.nameKey]));
            nameEntry.Add((TKey)(object)Constants.idKey, ConvertValue(values[Constants.idKey]));

            idList.Add(id, idEntry);
            nameList.Add(name, nameEntry);

            //List<object> tableList = new List<object>();
            //foreach (var key in entry.Keys)
            //{
            //    TValue value = entry[key];
            //    tableList.Add(value);
            //}

            //TableManager.table.Add(id, tableList);
        }

        return (idList, nameList);
    }

    public static Dictionary<TKey, TValue> ReadClass<TKey, TValue>(string path) where TValue : IData, new()
    {
        string filePath = Application.dataPath + '/' + path;

        StreamReader file = new StreamReader(filePath);

        var list = new Dictionary<TKey, TValue>();

        string[] lines = Regex.Split(file.ReadToEnd(), LINE_SPLIT_RE);

        file.Close();

        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for (int i = 1; i < lines.Length; ++i) //0��° �ε����� header�� ����.
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);

            if (values[0] == "") continue;
            TKey key = (TKey)Enum.Parse(typeof(TKey), values[0]);

            //j�� 1���� �����ϹǷ� 1�� �� ��. �׷��� ������ �迭 �� ĭ�� ���� ��.
            int paramSize = Math.Max(header.Length, values.Length) - 1;
            object[] paramArr = new object[paramSize];
            for (int j = 0; j < paramSize; ++j)
            {
                string param = values[j + 1]; //0��° �ε����� Key�� ���ǹǷ� j�� 1�� ���ؼ� ����� ������.
                param = param.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                param = param.Replace("<br>", "\n");
                param = param.Replace("<c>", ",");

                //j�� 1���� �����ؼ� 1�� �� ��. �׷��� 0��° �迭���� ����ϰ� ��.
                paramArr[j] = param;
            }

            paramArr.Log();
            TValue value = new TValue();
            value.Init(paramArr);

            list.Add(key, value);
        }

        return list;
    }

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
            if (values.Length == 0 || values[0] == "") continue;

            ScriptObject entry = new ScriptObject(values);

            list.Add(entry);
        }
        return list;
    }
}
