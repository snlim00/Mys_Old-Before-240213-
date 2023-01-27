using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniRx;
using System.IO;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

    private DialogManager dialogMgr;

    private EventManager eventMgr;

    [SerializeField] private GameObject nodePref;
    public GameObject graph;
    [SerializeField] private GameObject scrollViewContent;
    private GameObject scriptGraph;

    public List<Node> nodeList;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogMgr = DialogManager.instance;
        eventMgr = FindObjectOfType<EventManager>();


        //test code
        nodeList = new();

        {
            Node node = Instantiate(nodePref).GetComponent<Node>();
            nodeList.Add(node);
            node.Init();
        }
        {
            Node node = Instantiate(nodePref).GetComponent<Node>();
            nodeList.Add(node);
            node.Init();
        }
        {
            Node node = Instantiate(nodePref).GetComponent<Node>();
            nodeList.Add(node);
            node.Init();
        }
    }

    private void LoadScriptGraph(int groupID)
    {
        string graphPath = Application.dataPath + "/Assets/Resources/Prefabs/ScriptGraph/ScriptGraph" + groupID + ".prefab";

        if (!File.Exists(graphPath))
        {
            CreateNewScriptTree();
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(graphPath);

        GameObject scriptGraph = Instantiate(prefab); 
        scrollViewContent.transform.DestroyAllChildren();

        scriptGraph.transform.SetParent(scrollViewContent.transform);

        this.scriptGraph = scriptGraph;
    }

    private void CreateNewScriptTree()
    {
        
    }
}
