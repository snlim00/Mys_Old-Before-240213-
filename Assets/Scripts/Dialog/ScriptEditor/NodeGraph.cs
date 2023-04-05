using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeGraph : MonoBehaviour
{
    public static NodeGraph instance = null;

    private EditorManager editorMgr;

    private ObjectList objectList;

    public Node head;

    private GameObject nodePref;

    public Node selectedNode = null;

    private RectTransform rect;

    private Stack<EditorCommand> commands = new(); //����� Ŀ�ǵ�
    private Stack<EditorCommand> redoCommands = new(); //�𵵵� Ŀ�ǵ�

    public bool isPlaying { get; private set; } = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        editorMgr = EditorManager.instance;

        nodePref = Resources.Load<GameObject>("Prefabs/ScriptEditor/Node");
        rect = GetComponent<RectTransform>();
        objectList = FindObjectOfType<ObjectList>();
    }

    private void Start()
    {
        var commandStream = Observable.EveryUpdate()
            .Where(_ =>
            {
                GameObject current = EventSystem.current.currentSelectedGameObject;

                if (current == null || current?.tag == Tag.Node)
                {
                    return true;
                }

                return false;
            });

        commandStream
            .Where(_ => Input.GetKeyDown(KeyCode.H))
            .Subscribe(_ =>
            {
                HideInspector();
            });

        commandStream
            .Where(_ => Input.GetKeyDown(KeyCode.P))
            .Subscribe(_ =>
            {
                DialogPlay();
            });


        commandStream
            .Where(_ => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ =>
            {
                Undo();
            });

        commandStream
            .Where(_ => Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
            .Subscribe(_ =>
            {
                Redo();
            });

        commandStream
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ => CreateNextNode());

        commandStream
            .Where(_ => Input.GetKeyDown(KeyCode.Delete))
            .Subscribe(_ => RemoveNode());

        commandStream
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => Save());

        commandStream
           .Where(_ => Input.GetKeyDown(KeyCode.E))
           .Subscribe(_ =>
           {
               ToggleScriptType();
           });
    }

    #region Keyboard commands
    private void ToggleScriptType()
    {
        if (selectedNode.nodeType == Node.NodeType.BranchEnd)
        {
            return;
        }

        if (selectedNode.script.scriptType == ScriptType.Text)
        {
            selectedNode.script.scriptType = ScriptType.Event;
        }
        else
        {
            selectedNode.script.scriptType = ScriptType.Text;
        }

        RefreshInspector();
        selectedNode.RefreshNodeType();
    }

    private void HideInspector(bool? doHide = null)
    {
        bool isActive = ScriptInspector.instance.gameObject.activeSelf;

        ScriptInspector.instance.gameObject.SetActive(!doHide ?? !isActive);
        editorMgr.grpahPanel.SetActive(!doHide ?? !isActive);
        objectList.gameObject.SetActive(!doHide ?? !isActive);
    }

    private void DialogPlay()
    {
        if (isPlaying == false)
        {
            isPlaying = true;

            HideInspector(true);

            Save();
            DialogManager.instance.ReadScript(editorMgr.scriptGroupID);
            DialogManager.instance.ExecuteMoveTo(selectedNode.script.scriptID, DialogManager.instance.DialogStart);
        }
        else
        {
            isPlaying = false;

            HideInspector(false);

            DialogManager.instance.StopDialog();
        }
    }

    private void Undo()
    {
        EditorCommand cmd;

        if (commands.TryPop(out cmd) == true)
        {
            cmd.Undo();
            redoCommands.Push(cmd);
        }
        else
        {
            "�ǵ��� �۾��� �����ϴ�".Log("Undo");
        }
    }

    private void Redo()
    {
        EditorCommand cmd;

        if (redoCommands.TryPop(out cmd) == true)
        {
            cmd.Execute();
            commands.Push(cmd);
        }
        else
        {
            "�ǵ��� �۾��� �����ϴ�".Log("Redo");
        }
    }
    #endregion

    public void RefreshInspector()
    {
        ScriptInspector.instance.ApplyInspector();
        ScriptInspector.instance.SetInspector(selectedNode);  
    }

    #region Save / Export
    public void Save()
    {
        RefreshInspector();

        ScriptExport();
        "Save".Log();
    }

    private void ScriptExport()
    {
        RefreshAllNode();

        string path = (Application.dataPath + "/Data/ScriptTable" + editorMgr.scriptGroupID + ".CSV");

        using (var writer = new CsvFileWriter(path))
        {
            List<string> colums = new List<string>();
            string[] keys = Enum.GetNames(typeof(ScriptDataKey));

            colums.AddRange(keys);
            writer.WriteRow(colums);
            colums.Clear();

            TraversalNode(true, this.head, (index, branchIndex, depth, node) =>
            {
                //branch �Ķ���� ó��
                if(node.script.eventData.eventType == EventType.Branch)
                {
                    for(int i = 0; i < Node.maxBranchCount; ++i)
                    {
                        //�귣ġ�� ������ �ش� �κ��� eventParam ����
                        if (node.branch[i] == null)
                        {
                            for(int j = i; j < Node.maxBranchCount; ++j)
                            {
                                node.script.eventData.eventParam[i * 2 + 1] = null;
                                node.script.eventData.eventParam[i * 2 + 2] = null;
                            }

                            break;
                        }

                        node.script.eventData.eventParam[i * 2 + 2] = node.branch[i].script.scriptID.ToString();
                    }
                }

                //�� ���
                foreach (ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
                {
                    string value = node.script.GetVariableFromKey(key) ?? "";

                    var script = node.script;
                    var eventData = script.eventData;

                    if(script.scriptType == ScriptType.Event)
                    {
                        EventInfo eventInfo = EventInfo.GetEventInfo(node.script.eventData.eventType);

                        //�Ұ����� �� ó��
                        {
                            if (eventInfo.canUseLoop == false)
                            {
                                if (key == ScriptDataKey.LoopCount)
                                {
                                    value = EventData.DEFAULT_LOOP_COUNT.ToString();
                                }
                            }

                            if (eventInfo.canUseLinkEvent == false)
                            {
                                if (key == ScriptDataKey.LinkEvent)
                                {
                                    value = ScriptObject.DEFAULT_LINK_EVENT.ToString();
                                }
                            }

                            if (eventInfo.canUseDurationTurn == false)
                            {
                                if (key == ScriptDataKey.DurationTurn)
                                {
                                    value = EventData.DEFAULT_DURATION_TURN.ToString();
                                }
                            }
                        }
                        

                        if(script.linkEvent == true && node.nextNode != null && node.nextNode.script.scriptType == ScriptType.Text)
                        {
                            value = ScriptObject.DEFAULT_LINK_EVENT.ToString();
                        }
                    }

                    colums.Add(value);
                }

                writer.WriteRow(colums);
                colums.Clear();
            });
        } 
    }
    #endregion

    public void CreateGraph()
    {
        int scriptGroupID = editorMgr.scriptGroupID;

        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;
        node.transform.localPosition = Vector3.zero;

        head = node;

        SelectNode(node);

        RefreshAllNode();
    }

    public Node CreateNode()
    {
        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.transform.SetParent(transform);
        node.transform.localScale = Vector3.one;

        return node;
    }

    #region commands
    public void ExecuteCommand(EditorCommand cmd)
    {
        cmd.Execute();
        commands.Push(cmd);
    }

    public void CreateBranch()
    {
        if (selectedNode.nodeType == Node.NodeType.Goto)
        {
            return;
        }

        if (selectedNode.nodeType == Node.NodeType.BranchEnd)
        {
            return;
        }

        if (selectedNode.GetBranchCount() >= Node.maxBranchCount)
        {
            return;
        }

        EditorCommand command = new CreateBranchNode();
        ExecuteCommand(command);
    }

    public void CreateNextNode()
    {
        if (selectedNode.nodeType == Node.NodeType.Goto)
        {
            return;
        }

        if(selectedNode.nodeType == Node.NodeType.BranchEnd)
        {
            return;
        }

        EditorCommand command = new CreateNextNode();
        ExecuteCommand(command);
    }

    public void RemoveNode()
    {
        if (selectedNode.nextNode == null && selectedNode.prevNode == null) //������ �ϳ� ���� ����� ������ ����
        {
            "������ �ϳ� ���� ���� ������ �� �����ϴ�".LogWarning();
            return;
        }

        //���� ��尡 Branch�̸� �ش� ����� ���� ��尡 ���ٸ� ������ �� ����.
        if (selectedNode.prevNode != null && selectedNode.prevNode.nodeType == Node.NodeType.Branch && selectedNode.nextNode == null)
        {
            "�귣ġ�� ���� ���� �׻� �����ؾ� �մϴ�.".LogWarning();
            return;
        }

        if(selectedNode.parent != null)
        {
            if (selectedNode.prevNode == null && selectedNode.nextNode.nodeType == Node.NodeType.BranchEnd)
            {
                "�귣ġ�� ������ �ϳ� ���� ���� ������ �� �����ϴ�.".LogWarning();
                return;
            }
            else if(selectedNode.nodeType == Node.NodeType.BranchEnd)
            {
                EditorCommand cmd = new RemoveBranch();
                ExecuteCommand(cmd);
                return;
            }
        }

        EditorCommand command = new RemoveNode();
        ExecuteCommand(command);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeBranch"></param>
    /// <param name="action">index, branchIndex, depth, Node<br></br><br></br>
    /// index : �θ� ���� ���鸸 �������� ��, �ش� ����� ��ȣ<br></br>
    /// branchIndex : �ش� �귣ġ�� ���� �θ��� branch�� index. �θ� ���ٸ� null<br></br>
    /// depth : �ش� �귣ġ�� ���� ���鸸 �������� ��, �ش� ����� �ش� �귣ġ������ ��ȣ.
    /// </param>
    /// <returns></returns>
    public int TraversalNode(bool includeBranch, Node head, Action<int, int?, int, Node> action, int index = 0)
    {
        Node node = head;
        int? branchIndex = null;
        int depth = 0;

        while(node != null)
        {
            ++index;
            ++depth;

            branchIndex = node.GetBranchIndex() != -1 ? node.GetBranchIndex() : null;

            if (action != null)
            {
                action(index, branchIndex, depth, node);
            }

            if(includeBranch == true)
            {
                if (node.branch.Count > 0)
                {
                    foreach (var branch in node.branch)
                    {
                        index = TraversalNode(true, branch, action, index);
                    }
                }
            }

            node = node.nextNode;
        }

        return index;
    }

    public int GetNodeCount()
    {
        return TraversalNode(true, head, null);
    }

    private void SetScriptID()
    {
        TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            node.SetScriptID(index);
        });
    }

    public void RefreshAllNode()
    {
        SetScriptID();

        int loopCount = TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            //��� �̸� ����
            {
                string name = "";
                
                if (node.parent == null)
                {
                    name += depth.ToString();
                }
                else
                {
                    if (node.isHead == true)
                    {
                        name += node.parent.name;
                        name += " - " + branchIndex;
                    }
                    else
                    {
                        name += depth;
                    }
                }

                node.SetName(name);
            }

            //��� ������ ����
            {
                if (node.isHead == true)
                {
                    if (node.parent == null)
                    {
                        node.transform.localPosition = Vector2.zero;
                    }
                    else
                    {
                        Vector2 pos = node.parent.transform.localPosition;
                        pos.y += Node.interval.y;
                        pos.x += (Node.interval.x * (branchIndex + 1) ?? 1) - Node.interval.x * 0.5f;

                        node.transform.localPosition = pos;
                    }
                }
                else if (node.prevNode != null)
                {

                    Vector2 pos = node.prevNode.transform.localPosition;

                    int childCount = node.prevNode.GetChildCount() + 1;
                    pos.y += Node.interval.y * childCount;
                    node.transform.localPosition = pos;
                }
            }

            //��� ��������
            {
                node.Refresh();

                //BranchEnd ó��
                if (node.nodeType == Node.NodeType.BranchEnd)
                {
                    node.SetName("-");

                    node.script.eventData.eventParam[0] = node?.parent?.nextNode.script.scriptID.ToString();
                }
            }
        });

        RefreshContentSize();

        RefreshObjectList();
    }

    private void RefreshObjectList()
    {
        List<string> objectList = new();

        bool passed = false;

        TraversalNode(true, head, (_, _, _, node) =>
        {
            if (passed == true) return;

            if(node.script.scriptType == ScriptType.Event && node.script.eventData.eventType == EventType.CreateObject)
            {
                objectList.Add(node.script.eventData.eventParam[1]);
            }

            if (node.script.scriptType == ScriptType.Event && node.script.eventData.eventType == EventType.RemoveObject)
            {
                objectList.Remove(node.script.eventData.eventParam[0]);
            }

            if (node == selectedNode) passed = true;
        });

        this.objectList.RefreshList(objectList);
    }

    private void RefreshContentSize()
    {
        int nodeCount = GetNodeCount();

        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = nodeCount * Mathf.Abs(Node.interval.y) + 100;
        rect.sizeDelta = sizeDelta;

        editorMgr.scrollViewContent.sizeDelta = new Vector2(editorMgr.scrollViewContent.sizeDelta.x, sizeDelta.y);
    }

    #region ��� ����
    public void SelectNode(Node node)
    {
        if(node == selectedNode)
        {
            return;
        }

        selectedNode?.SetColorDeselect();

        selectedNode = node;
        node.SetColorSelect();
        ScriptInspector.instance.SetInspector(node);

        RefreshObjectList();
    }

    public void SelectLastNode()
    {
        Node lastNode = head;

        TraversalNode(false, head, (_, _, _, node) =>
        {
            lastNode = node;
        });

        SelectNode(lastNode);
    }
    #endregion
}
