using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.IO;
using System;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

    private GameObject graphPref;

    private NodeGraph nodeGrp;

    public RectTransform scrollViewContent;

    public int scriptGroupID = -1;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        EventInfo.Init();

        graphPref = Resources.Load<GameObject>("Prefabs/ScriptEditor/Graph");
    }

    private void Start()
    {
        //test code
        EditorStart(1);
    }

    public void EditorStart(int scriptGroupID)
    {
        this.scriptGroupID = scriptGroupID;

        string path = Application.dataPath + "/Data/ScriptTable/" + scriptGroupID + ".CSV";
        bool isExists = File.Exists(path);

        nodeGrp = Instantiate(graphPref).GetComponent<NodeGraph>();

        if(isExists)
        {
            LoadScript(path);
        }

        nodeGrp.CreateGraph();

        nodeGrp.transform.SetParent(scrollViewContent.transform);
        nodeGrp.transform.localScale = Vector3.one;
        nodeGrp.transform.localPosition = new Vector2(0, -50);
    }

    private void LoadScript(string path)
    {
        var scripts = CSVReader.ReadScript(path);

        void Branch(int index)
        {
            var parent = scripts[index];


        }

        for(int i = 0; i < scripts.Count; ++i)
        {
            var script = scripts[i];
            var nextScript = scripts?[i + 1];

            EditorCommand command = null;

            if (script.scriptType == ScriptType.Event)
            {
                if(script.eventData.eventType == EventType.Branch)
                {
                    
                }
            }
            else if(script.scriptType == ScriptType.Text)
            {
                command = new CreateNextNode();
            }

            if(command != null)
            {
                command.SetScript(script);
                command.Execute();
            }
        }
    }
}
