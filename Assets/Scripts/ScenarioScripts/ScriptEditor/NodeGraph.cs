using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum InputType
{
    Load,

    EditInputField,

    Playing,

    Select,
    Script,
    Object,
}

public class NodeGraph : Singleton<NodeGraph>
{
    private DialogManager dialogMgr;
    private EditorManager editorMgr;
    private ScriptInspector inspector;
    private ObjectList objList;

    private ScriptManager scriptMgr => RuntimeData.scriptMgr;

    [SerializeField] private GameObject nodePref;

    [SerializeField] private RectTransform scrollViewContent;

    public Node head = null;

    public Node selectedNode = null;

    public InputType inputType = InputType.Select;

    private Stack<EditorCommand> undoCommand = new Stack<EditorCommand>();
    private Stack<EditorCommand> redoCommand = new Stack<EditorCommand>();

    public event Action<Node> OnSelectScript;
    public event Action<string> OnSelectObject;

    #region commandButtons
    [SerializeField] private Button createNextNodeBtn;
    [SerializeField] private Button createPrevNodeBtn;
    [SerializeField] private Button changeNodeTypeBtn;
    [SerializeField] private Button removeNodeBtn;
    [SerializeField] private Button playBtn;
    #endregion

    private void Start()
    {
        dialogMgr = DialogManager.Instance;
        editorMgr = EditorManager.Instance;
        inspector = ScriptInspector.Instance;
        objList = ObjectList.Instance;

        createNextNodeBtn.onClick.AddListener(CreateNextNode);
        createPrevNodeBtn.onClick.AddListener(CreatePrevNode);
        changeNodeTypeBtn.onClick.AddListener(ChangeNodeType);
        removeNodeBtn.onClick.AddListener(RemoveNode);
        playBtn.onClick.AddListener(Play);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HideInspector();
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            if(inputType != InputType.EditInputField)
            {
                Play();
            }
        }

        if (inputType == InputType.Select)
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                RefreshAll();
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                ChangeNodeType();
            }

            if(Input.GetKeyDown(KeyCode.C))
            {
                CreateNodeCommand();
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                RemoveNode();
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log(GetBranchLength(selectedNode));
            }

            if(Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log(GetBranchWidth(selectedNode));
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log(selectedNode.GetBranchCount());
            }

            if(Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log(selectedNode.GetBranchIndex());
            }

            if(Input.GetKeyDown(KeyCode.Z))
            {
                if(Input.GetKey(KeyCode.LeftControl))
                {
                    Undo();
                }
            }
        }
    }

    #region Commands
    private void ExecuteCommand(EditorCommand cmd)
    {
        cmd.Execute();

        undoCommand.Push(cmd);
    }

    private void Undo()
    {
        var cmd = undoCommand.Pop();
        cmd?.Undo();
    }

    private void CreateNodeCommand()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CreatePrevNode();
        }
        else
        {
            CreateNextNode();
        }
    }

    private void CreateNextNode()
    {
        if(selectedNode.nodeType == NodeType.BranchEnd)
        {
            "Branch End �ڿ��� ��带 ��ġ�� �� �����ϴ�.".Log();
            return;
        }

        var cmd = new CreateNextNode();
        ExecuteCommand(cmd);
    }

    private void CreatePrevNode()
    {
        var cmd = new CreatePrevNode();
        ExecuteCommand(cmd);
    }

    private void RemoveNode()
    {
        if (selectedNode.nextNode == null && selectedNode.prevNode.nodeType == NodeType.Branch)
        {
            "Branch �ڿ� ��ġ�� ���� �׻� �����ؾ� �մϴ�.".Log();
        }

        EditorCommand cmd = null;

        if(selectedNode.nodeType == NodeType.BranchEnd)
        {
            cmd = new RemoveBranch();
        }
        else
        {
            cmd = new RemoveNode();
        }

        ExecuteCommand(cmd);
        return;
    }

    private void ChangeNodeType()
    {
        if (selectedNode.script.scriptType == ScriptType.Event)
        {
            selectedNode.script.scriptType = ScriptType.Text;
        }
        else
        {
            selectedNode.script.scriptType = ScriptType.Event;
        }

        SetInspector(selectedNode);
    }

    private void Save()
    {
        editorMgr.ExportScript();
    }

    private void Play()
    {
        if (dialogMgr.isPlaying == false)
        {
            HideInspector(true);
            inputType = InputType.Playing;
            editorMgr.ExportScript();
            dialogMgr.StartDialog(RuntimeData.scriptMgr.scriptGroupId);

            dialogMgr.onStop.AddListener(() =>
            {
                HideInspector(false);
                inputType = InputType.Select;
                dialogMgr.onStop.RemoveAllListeners();
            });
        }
        else
        {
            HideInspector(false);
            inputType = InputType.Select;
            dialogMgr.StopDialog();
        }
    }
    #endregion

    public void CreateNodeGraph()
    {
        if(RuntimeData.scriptMgr == null) { return; }

        head = CreateNode();

        SelectNode(head);

        RefreshAll();
    }

    public void CreateHeadNode()
    {
        Node node = CreateNode();

        head = node;

        SelectNode(node);
    }

    public Node CreateNode(ScriptObject script = null)
    {
        Node node = Instantiate(nodePref).GetComponent<Node>();

        node.Init(script ?? new ScriptObject());

        return node;
    }

    public void CreateBranch()
    {
        ExecuteCommand(new CreateBranch());
    }

    public void OnNodeClick(Node node)
    {
        if (inputType == InputType.Select)
        {
            SelectNode(node);
            SetInspector(node);
            objList.RefreshList();
        }
        else if(inputType == InputType.Script)
        {
            OnSelectScript.Invoke(node);
            CancelSelectScript();
        }
    }

    public void CancelSelectScript()
    {
        OnSelectScript = null;
        inputType = InputType.Select;
    }

    public void InvokeSelectObject(string name)
    {
        OnSelectObject.Invoke(name);
    }

    public void CancelSelectObject()
    {
        OnSelectObject = null;
        inputType = InputType.Object;
    }

    public void SelectNode(Node node, bool setInspector = false)
    {
        selectedNode?.DeselectNode();
        node.SelectNode();
        selectedNode = node;

        SetInspector();
    }

    public void HideInspector(bool? doHide = null)
    {
        bool isActive = dialogMgr.canvas.sortingOrder < editorMgr.canvas.sortingOrder;

        if((doHide == null && isActive == false) || (doHide != null && doHide == false))
        {
            dialogMgr.canvas.sortingOrder = 0;
            editorMgr.canvas.sortingOrder = 1;
        }
        else
        {
            dialogMgr.canvas.sortingOrder = 1;
            editorMgr.canvas.sortingOrder = 0;
        }
    }

    public void SetInspector(Node node = null)
    {
        if (inputType != InputType.Load)
        {
            inspector.SetVaraibles(node ?? selectedNode);
        }
    }

    public void RefreshAll()
    {
        RefreshAllNode();

        SetContentSize();
    }

    public void RefreshAllNode()
    {
        TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            //����� scriptId ����
            node.script.scriptId = scriptMgr.scriptGroupId * 10000 + index;

            node.RefreshNodeType();

            //��� �̸� ����
            {
                string name = "";

                if(node.parent != null && node.isHead == true)
                {
                    name += node.parent.name;
                    name += " - " + branchIndex;
                }
                else
                {
                    name += depth.ToString();
                }

                if(node.nodeType == NodeType.BranchEnd)
                {
                    name = "-";
                }

                node.SetName(name);
            }

            //��� ������ ����
            {
                if(node.isHead == true)
                {
                    if(node.parent == null)
                    {
                        node.transform.localPosition = Vector2.zero;
                    }
                    else
                    {
                        Vector2 pos = node.parent.transform.localPosition;

                        pos.y -= 50;

                        if (branchIndex > 0)
                        {
                            float width = GetBranchWidth(node.parent.branch[(branchIndex ?? 1) - 1]);

                            pos.x += (width * 60f) + 30f;
                        }
                        else
                        {
                            pos.x += 30f;
                        }


                        node.transform.localPosition = pos;
                    }
                }
                else if(node.prevNode != null)
                {
                    Vector2 pos = node.prevNode.transform.localPosition;

                    //if(node.prevNode.nodeType == NodeType.Branch)
                    if(node.prevNode.GetBranchCount() > 0)
                    {
                        int length = GetBranchLength(node.prevNode) + 1;

                        pos.y -= ((node.rectTransform.sizeDelta.y / 2) + (node.prevNode.rectTransform.sizeDelta.y / 2) + 5) * length;
                    }
                    else
                    {
                        pos.y -= (node.rectTransform.sizeDelta.y / 2) + (node.prevNode.rectTransform.sizeDelta.y / 2) + 5;
                    }


                    node.transform.localPosition = pos;
                }
            }
        });
    }

    //Automathic���� ������ BranchEnd�� ��Ȯ�� ��ũ��Ʈ ���̵�� �����մϴ�.
    public void SetAutomaticBranch()
    {
        TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            EventData eventData = node.script.eventData;

            if (node.script.scriptType == ScriptType.Event)
            {
                if (eventData.eventType == EventType.BranchEnd)
                {
                    if (eventData.eventParam[0] == ScriptVariable.autoTracking)
                    {
                        eventData.eventParam[0] = node.parent.nextNode.script.scriptId.ToString();
                    }
                }
            }
        });
    }

    public float GetBranchWidth(Node headNode)
    {
        //width�� ����ó���� �����ؼ� Ȯ���غ���.

        float width = 1f;

        int branchCount = headNode.GetBranchCount();

        int headBranchIdx = headNode.GetBranchIndex();
        if (headBranchIdx > 0)
        {
            float temp = GetBranchWidth(headNode.parent.branch[headBranchIdx - 1]);

            width += temp;
        }

        TraversalNode(true, headNode, (index, branchIndex, depth, node) =>
        {
            float branchCount = node.GetBranchCount();

            width += branchCount;
        },
        stopCondition: (_, _, _, node) =>
        {
            if (node.parent == headNode.parent && node.GetBranchIndex() != headBranchIdx)
            {
                return true;
            }
            else if(node == headNode.parent?.nextNode)
            {
                return true;
            }
            else
            {
                return false;
            }
        });

        return width;
    }

    public int GetBranchLength(Node headNode)
    {
        int length = 0;

        int branchCount = headNode.GetBranchCount();

        for (int i = 0; i < branchCount; ++i)
        {
            int tempLength = 0;

            tempLength += TraversalNode(true, headNode.branch[i], (index, branchIndex, depth, node) =>
            {
                if(node.GetBranchCount() > 0)
                {
                    tempLength += GetBranchLength(node);
                }
            },
            stopCondition: (_, _, _, node) =>
            {
                if(node.prevNode == headNode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if(tempLength > length)
            {
                length = tempLength;
            }
        }

        return length;
    }

    public void SetContentSize()
    {
        Vector2 size = new(0, 0);

        TraversalNode(true, head, (index, branchIndex, depth, node) =>
        {
            if(node.transform.localPosition.x > size.x)
            {
                size.x = node.transform.localPosition.x;
            }

            if (Mathf.Abs(node.transform.localPosition.y) > size.y)
            {
                size.y = Mathf.Abs(node.transform.localPosition.y);
            }
        });

        scrollViewContent.sizeDelta = size + (Vector2.one * 100);
    }

    /// <summary>
    /// ��带 ��ȸ�մϴ�.
    /// </summary>
    /// <param name="includeBranch">false��� ����� ��ȸ���� �귣ġ�� ���� ������ ���ܵ˴ϴ�.</param>
    /// <param name="head">��ȸ�� �����ϴ� ù ��° ����Դϴ�.</param>
    /// <param name="action">��� ��带 ������� ����Ǵ� �ݹ� �Լ��Դϴ�.<br></br><br></br>
    /// index, branchIndex, depth, node<br></br><br></br>
    /// index : �θ� ���� ���鸸 �������� ��, �ش� ����� ��ȣ<br></br>
    /// branchIndex : �ش� �귣ġ�� ���� �θ��� branch�� index. �θ� ���ٸ� null<br></br>
    /// depth : �ش� �귣ġ�� ���� ���鸸 �������� ��, �ش� ����� �ش� �귣ġ������ ��ȣ.</param>
    /// <param name="index">��ȸ Ƚ���� �̾ ī��Ʈ�ϰ� �ʹٸ� ���� �Է��մϴ�.</param>
    /// <returns>��带 ��ȸ�� �� Ƚ���� ��ȯ�մϴ�.</returns>
    public int TraversalNode(bool includeBranch, Node head, Action<int, int?, int, Node> action, int index = 0, Func<int, int?, int, Node, bool> stopCondition = null)
    {
        int? branchIndex = null;
        int depth = 0;

        for(Node node = head; node != null; node = node.nextNode)
        {
            if (stopCondition?.Invoke(index, branchIndex, depth, node) == true)
            {
                break;
            }    

            ++index; 
            ++depth;

            int _branchIndex = node.GetBranchIndex();

            branchIndex = _branchIndex != -1 ? _branchIndex : null; //branchIndex�� -1�̶�� null�� ����

            action?.Invoke(index, branchIndex, depth, node);

            if(includeBranch == true)
            {
                if (node.branch.Count > 0)
                {
                    foreach(var branch in node.branch)
                    {
                        index = TraversalNode(true, branch, action, index); //index�� �����ؼ� index�� ����ؼ� �����ϵ��� ��.
                    }
                }
            }
        }

        return index;
    }

    public void LoadScript()
    {
        inputType = InputType.Load;

        while(scriptMgr.currentScript != null)
        {
            CreateNextNode cmd = new();
            cmd.SetScript(scriptMgr.currentScript);
            cmd.Execute();

            scriptMgr.Next();
        }

        SelectNode(head);
        new RemoveNode().Execute();
        SelectNode(head);

        inputType = InputType.Select;
    }
}
