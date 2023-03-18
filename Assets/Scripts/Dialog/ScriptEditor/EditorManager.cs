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
        var scriptMgr = new ScriptManager();

        scriptMgr.ReadScript(path);

        //시작 스크립트 설정
        int firstScriptID = ScriptManager.GetFirstScriptIDFromGroupID(scriptGroupID);
        ScriptObject firstScript = scriptMgr.GetScriptFromID(firstScriptID);
        scriptMgr.SetCurrentScript(firstScript);

        while(true)
        {
            var currentScript = scriptMgr.currentScript;

            CreateNode(ref scriptMgr);

            scriptMgr.Next();
        }
    }

    private void CreateNode(ref ScriptManager scriptMgr)
    {
        var script = scriptMgr.currentScript;

        EditorCommand command = null;

        if (script.scriptType == ScriptType.Text)
        {
            command = new CreateNextNode();
        }
        else if(script.scriptType == ScriptType.Event)
        {
            if(script.eventData.eventType == EventType.Branch)
            {
                Branch(ref scriptMgr);
            }
        }

        if (command != null)
        {
            command.SetScript(script);
            command.Execute();
        }
    }

    private void Branch(ref ScriptManager scriptMgr)
    {
        var parent = scriptMgr.currentScript;

        int branchCount = parent.GetBranchCount;

        for(int i = 0; i < branchCount; ++i)
        {
            //TraversalBranch();
        }
    }

    private void TraversalBranch(ref ScriptManager scriptMgr, ScriptObject script)
    {
        while(scriptMgr.currentScript.scriptType != ScriptType.Event && scriptMgr.currentScript.eventData.eventType != EventType.Goto)
        {
            scriptMgr.Next();
        }
    }
}
