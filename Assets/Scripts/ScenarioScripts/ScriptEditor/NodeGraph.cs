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
            "Branch End 뒤에는 노드를 배치할 수 없습니다.".Log();
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
            "Branch 뒤에 배치된 노드는 항상 존재해야 합니다.".Log();
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
            //노드의 scriptId 설정
            node.script.scriptId = scriptMgr.scriptGroupId * 10000 + index;

            node.RefreshNodeType();

            //노드 이름 설정
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

            //노드 포지션 설정
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

    //Automathic으로 설정된 BranchEnd를 정확한 스크립트 아이디로 변경합니다.
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
        //width의 수급처들을 구분해서 확인해보기.

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
    /// 노드를 순회합니다.
    /// </summary>
    /// <param name="includeBranch">false라면 노드의 순회에서 브랜치에 속한 노드들이 제외됩니다.</param>
    /// <param name="head">순회를 시작하는 첫 번째 노드입니다.</param>
    /// <param name="action">모든 노드를 대상으로 실행되는 콜백 함수입니다.<br></br><br></br>
    /// index, branchIndex, depth, node<br></br><br></br>
    /// index : 부모가 없는 노드들만 나열했을 때, 해당 노드의 번호<br></br>
    /// branchIndex : 해당 브랜치가 속한 부모의 branch의 index. 부모가 없다면 null<br></br>
    /// depth : 해당 브랜치에 속한 노드들만 나열했을 때, 해당 노드의 해당 브랜치에서의 번호.</param>
    /// <param name="index">순회 횟수를 이어서 카운트하고 싶다면 값을 입력합니다.</param>
    /// <returns>노드를 순회한 총 횟수를 반환합니다.</returns>
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

            branchIndex = _branchIndex != -1 ? _branchIndex : null; //branchIndex가 -1이라면 null로 변경

            action?.Invoke(index, branchIndex, depth, node);

            if(includeBranch == true)
            {
                if (node.branch.Count > 0)
                {
                    foreach(var branch in node.branch)
                    {
                        index = TraversalNode(true, branch, action, index); //index를 전달해서 index가 계속해서 증가하도록 함.
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
