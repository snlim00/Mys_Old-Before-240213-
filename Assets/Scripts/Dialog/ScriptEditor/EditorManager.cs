using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniRx;
using System.IO;
using System;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

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
        }
        instance = this;

        nodeGraph = FindObjectOfType<NodeGraph>();
    }
    private void Start()
    {
        //test code
        LoadGraph(1);
    }

    public void LoadGraph(int scriptGroupID)
    {
        this.scriptGroupID = scriptGroupID;

        string path = Application.dataPath + "/Assets/Resources/Prefabs/ScriptGraph/ScriptGraph" + scriptGroupID + ".prefab";
        bool isExists = File.Exists(path);

        if(isExists)
        {
            nodeGraph.LoadGraph(path);
        }
        else
        {
            nodeGraph.CreateGraph();
        }
    }
}
