using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniRx;
using System.IO;
using System;
using UnityEditor.SceneManagement;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

    private GameObject graphPref;

    private DialogManager dialogMgr;
    private EventManager eventMgr;

    private NodeGraph nodeGraph;

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

        string fileName = "ScriptGraph" + scriptGroupID;
        string path = Application.dataPath + "/Resources/Prefabs/ScriptGraph/" + fileName + ".prefab";
        bool isExists = File.Exists(path);

        //(path + " : " + isExists).Log();

        if(isExists)
        {
            GameObject pref = Resources.Load<GameObject>("Prefabs/ScriptGraph/" + fileName);

            nodeGraph = Instantiate(pref).GetComponent<NodeGraph>();

            nodeGraph.SelectNode(nodeGraph.selectedNode);
        }
        else
        {
            nodeGraph = Instantiate(graphPref).GetComponent<NodeGraph>();

            nodeGraph.CreateGraph();
        }

        nodeGraph.transform.SetParent(scrollViewContent.transform);
        nodeGraph.transform.localScale = Vector3.one;
        nodeGraph.transform.localPosition = new Vector2(0, -50);
    }
}
