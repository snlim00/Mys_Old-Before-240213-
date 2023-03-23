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

    public GameObject grpahPanel;

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
        //EditorStart(1);
    }

    public void EditorStart(int scriptGroupID)
    {
        "EDITOR START".Log();
        this.scriptGroupID = scriptGroupID;

        string path = Application.dataPath + "/Data/ScriptTable" + scriptGroupID + ".CSV";
        bool isExists = File.Exists(path);

        nodeGrp = Instantiate(graphPref).GetComponent<NodeGraph>();

        nodeGrp.CreateGraph();

        nodeGrp.transform.SetParent(scrollViewContent.transform);
        nodeGrp.transform.localScale = Vector3.one;
        nodeGrp.transform.localPosition = new Vector2(50, -50);

        if (isExists)
        {
            LoadScript(path);
        }
    }

    private ScriptManager scriptMgr = new();

    private void LoadScript(string path)
    {
        "Load Script".Log();
        scriptMgr = new();

        scriptMgr.ReadScript("ScriptTable" + scriptGroupID + ".CSV");

        //시작 스크립트 설정
        int firstScriptID = ScriptManager.GetFirstScriptIDFromGroupID(scriptGroupID);

        bool hasBranch = false;

        while(scriptMgr.currentScript != null)
        {
            if(scriptMgr.currentScript.scriptType == ScriptType.Event && scriptMgr.currentScript.eventData.eventType == EventType.Branch)
            {
                hasBranch = true;
                CreateBranch();
            }
            else
            {
                CreateNode(scriptMgr.currentScript);
            }

            if(scriptMgr.GetNextScript() != null)
            {
                scriptMgr.Next();
            }
            else
            {
                break;
            }
        }

        //브랜치 생성 시 다음 노드가 자동 생성되므로 해당 노드 제거.
        if(hasBranch == true)
        {
            nodeGrp.SelectLastNode();
            new RemoveNode().Execute();
        }

        //그래프 생성 시 첫 노드가 자동 생성되므로 해당 노드 제거.
        nodeGrp.SelectNode(nodeGrp.head);
        new RemoveNode().Execute();
    }

    private Node CreateNode(ScriptObject script)
    {
        var command = new CreateNextNode();
        command.SetScript(script);
        command.Execute();

        script.scriptID.Log("CreateNode");

        return command.GetCreatedNode();
    }

    private void CreateBranch()
    {
        scriptMgr.currentScript.scriptID.Log("CreateBranch");
        ScriptObject parentScript = scriptMgr.currentScript;
        Node parent = CreateNode(scriptMgr.currentScript);

        parentScript.scriptID.Log("Parent");

        BranchInfo branchInfo = parentScript.GetBranchInfo();

        for(int i = 0; i < branchInfo.Count; ++i)
        {
            scriptMgr.SetCurrentScript(branchInfo.targetID[i]);

            nodeGrp.SelectNode(parent);

            var createBranch = new CreateBranchNode();
            createBranch.SetScript(scriptMgr.currentScript);
            createBranch.Execute();

            scriptMgr.Next();

            while (true)
            {
                if(scriptMgr.currentScript.scriptType == ScriptType.Event && scriptMgr.currentScript.eventData.eventType == EventType.Goto)
                {
                    break;
                }

                if (scriptMgr.currentScript.scriptType == ScriptType.Event && scriptMgr.currentScript.eventData.eventType == EventType.Branch)
                {
                    CreateBranch();
                }
                else
                {
                    CreateNode(scriptMgr.currentScript);
                }

                scriptMgr.Next();
            }
        }

        nodeGrp.SelectNode(parent);
    }
}
