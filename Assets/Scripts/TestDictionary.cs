using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Newtonsoft.Json;

[System.Serializable]
public class TestData
{
    public Dictionary<int, string> dictionaryData = new();
    public int id = 12389238;
    public string userName = "MINJUN";
}

public class TestDictionary : MonoBehaviour
{
    public TestData testData = new();

    string json = "{\r\n  \"dictionaryData\": {\r\n    \"1\": \"66\",\r\n    \"2\": \"67\",\r\n    \"3\": \"68\",\r\n    \"4\": \"69\"\r\n  },\r\n  \"id\": 0,\r\n  \"userName\": \"MINJUN\"\r\n}";

    int idx = 0;
    int alphabet = 'A';

    // Start is called before the first frame update
    void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ => testData.dictionaryData.Add(++idx, (++alphabet).ToString()));

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.L))
            .Subscribe(_ => {
                var newDic = JsonConvert.DeserializeObject<TestData>(json);
                JsonConvert.SerializeObject(newDic).Log();
                newDic.userName.Log();
                newDic.id.Log();
            });
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ASDF");
    }
}
