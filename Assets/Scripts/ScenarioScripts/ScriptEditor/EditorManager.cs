using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UniRx;

public class EditorManager : Singleton<EditorManager>
{
    [SerializeField] private NodeGraph nodeGrp;
    public Canvas canvas;

    private ScriptManager scriptMgr => RuntimeData.scriptMgr;

    public void Start()
    {
        
    }

    public void EditorStart(int scriptGroupId)
    {
        string path = PathManager.CreateScriptPath(scriptGroupId);
        bool isExists = File.Exists(path);

        if(isExists == true)
        {
            RuntimeData.scriptMgr = CSVReader.ReadScript(scriptGroupId);

            nodeGrp.CreateNodeGraph();
            LoadScript();
        }
        else
        {
            RuntimeData.scriptMgr = new()
            {
                scriptGroupId = scriptGroupId
            };

            nodeGrp.CreateNodeGraph();
            nodeGrp.inputType = InputType.Select;
        }
    }

    public void LoadScript()
    {
        nodeGrp.inputType = InputType.Load;
        LoadScript(scriptMgr.firstScript, null);

        nodeGrp.SelectNode(nodeGrp.head);

        new RemoveNode().Execute();

        Observable.TimerFrame(1)
            .Subscribe(_ => {
                nodeGrp.RefreshAll();

                nodeGrp.TraversalNode(true, nodeGrp.head, (index, branchIndex, depth, node) =>
                {
                    EventData eventData = node.script.eventData;

                    if (node.script.scriptType == ScriptType.Event)
                    {
                        if (eventData.eventType == EventType.BranchEnd)
                        {
                            if (eventData.eventParam[0] == node.parent.nextNode.script.scriptId.ToString())
                            {
                                eventData.eventParam[0] = ScriptVariable.autoTracking;
                            }
                        }
                    }
                });
            });

        nodeGrp.inputType = InputType.Select;
    }

    private void LoadScript(ScriptObject script, Func<ScriptObject, bool> stopCondition = null)
    {
        scriptMgr.SetCurrentScript(script);

        nodeGrp.inputType = InputType.Load;

        for(; scriptMgr.currentScript != null; scriptMgr.Next())
        {
            if(scriptMgr.currentScript.scriptType == ScriptType.Event
                && (scriptMgr.currentScript.eventData.eventType == EventType.Branch || scriptMgr.currentScript.eventData.eventType == EventType.Choice))
            {
                CreateBranch(scriptMgr.currentScript);
            }
            else
            {
                CreateNode(scriptMgr.currentScript);
            }

            if (stopCondition?.Invoke(scriptMgr.currentScript) == true)
            {
                break;
            }
        }
    }

    private Node CreateNode(ScriptObject script)
    {
        var command = new CreateNextNode();
        command.SetScript(script).Execute();

        return command.createdNode;
    }

    private void CreateBranch(ScriptObject parentScript)
    {
        Node parent = CreateNode(parentScript);

        List<BranchBase> branchInfos = new();

        if(parentScript.eventData.eventType == EventType.Branch)
        {
            List<BranchInfo> temp = BranchInfo.CreateBranchInfo(parentScript);

            for(int i = 0; i < temp.Count; ++i)
            {
                branchInfos.Add(temp[i]);
            }
        }
        else if(parentScript.eventData.eventType == EventType.Choice)
        {
            List<ChoiceInfo> temp = ChoiceInfo.CreateChoiceInfo(parentScript);

            for (int i = 0; i < temp.Count; ++i)
            {
                branchInfos.Add(temp[i]);
            }
        }

        for (int i = 0; i < branchInfos.Count; ++i)
        {
            nodeGrp.SelectNode(parent);

            var branchInfo = branchInfos[i];

            scriptMgr.SetCurrentScript(branchInfo.scriptId); //���⼭ scriptId�� ���� ��ũ��Ʈ�� ���������� �ϸ� scriptId�� ����. //��ũ��Ʈ�� ���׹����� �Ǿ�����. �Ϻ� Id�� ���ǵ�.

            nodeGrp.SelectNode(parent);

            var createBranch = new CreateBranch();
            createBranch.SetScript(scriptMgr.currentScript).Execute();
            GameObject.Destroy(createBranch.branchEnd.gameObject);
            GameObject.Destroy(createBranch.nextNode?.gameObject);

            scriptMgr.Next();

            LoadScript(scriptMgr.currentScript, (script) => {
                bool stop = script.scriptType == ScriptType.Event && script.eventData.eventType == EventType.BranchEnd;

                if(stop == true)
                {
                    nodeGrp.SelectNode(parent);
                }

                return stop;
            });
        }
    }

    public void ExportScript()
    {
        nodeGrp.RefreshAll();
        nodeGrp.SetAutomaticBranch();

        string textPath = PathManager.CreateScriptTextPath(RuntimeData.scriptMgr.scriptGroupId);
        string eventPath = PathManager.CreateScriptPath(RuntimeData.scriptMgr.scriptGroupId);

        using (var writer = new CsvFileWriter(eventPath))
        {
            List<string> colums = new List<string>();

            //��ũ��Ʈ ���� ó��
            {
                writer.WriteString("[" + MysSection.chapter + "]\n" + RuntimeData.scriptMgr.chapter + "\n");
                writer.WriteString("[" + MysSection.character + "]\n" + RuntimeData.scriptMgr.character + "\n");
                writer.WriteString("[" + MysSection.title + "]\n" + RuntimeData.scriptMgr.title + "\n");
                writer.WriteString("[" + MysSection.explain + "]\n" + RuntimeData.scriptMgr.explain + "\n");

                List<int> requireStatValues = new();

                foreach (var kvp in RuntimeData.scriptMgr.requiredStat)
                {
                    requireStatValues.Add(kvp.Value);
                }
                writer.WriteString("[" + MysSection.requiredStat + "]\n" + string.Join(",", requireStatValues) + "\n");
                writer.WriteString("[" + MysSection.script + "]");

                string[] keys = Enum.GetNames(typeof(ScriptDataKey));

                colums.AddRange(keys);
                writer.WriteRow(colums);
                colums.Clear();
            }

            //��ũ��Ʈ ó��
            {
                nodeGrp.TraversalNode(true, nodeGrp.head, (index, branchIndex, depth, node) =>
                {
                    var script = node.script;
                    var eventData = node.script.eventData;

                    if (eventData.eventType == EventType.Branch)
                    {
                        for (int i = 0; i < node.GetBranchCount(); ++i)
                        {
                            eventData.eventParam[3 + (i * 2)] = node.branch[i].script.scriptId.ToString();
                        }
                    }

                    if (eventData.eventType == EventType.Choice)
                    {
                        for (int i = 0; i < node.GetBranchCount(); ++i)
                        {
                            eventData.eventParam[4 + (i * 5)] = node.branch[i].script.scriptId.ToString();
                        }
                    }

                    if (script.scriptType != ScriptType.Event) { return; }

                    foreach (ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
                    {
                        string value = node.script.GetVariableFromKey(key) ?? string.Empty;

                        ScriptInfo scriptInfo = ScriptInfo.eventInfos[eventData.eventType];
                        ParamInfo paramInfo = scriptInfo.GetParamInfo(key);

                        if (paramInfo == null) //������� �ʴ� �Ķ���Ͷ��
                        {
                            if (ScriptInfo.scriptParamInfos.TryGetValue(key, out var scriptParam)) //���� �Ķ���Ϳ� �ִ� �� Ȯ��
                            {
                                paramInfo = scriptParam;

                                if (scriptInfo.excludedKeys.Contains(key) == true) //���� �Ķ���Ϳ� ������ ���ܵǴ� Ű��� ���� �⺻������ ����.
                                {
                                    value = paramInfo.defaultValue;
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(value)) //���� �Ķ���͸鼭 ���Ǵ� �������� �Էµ��� ���� ��� �⺻������ ����.
                                    {
                                        value = paramInfo.defaultValue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value)) //���Ǵ� �Ķ�����ӿ��� ���� ���ٸ� �⺻ ���� ����.
                            {
                                (node.script.scriptId + " | " + paramInfo.paramName + " : �ش� ���� �Էµ��� �ʾҽ��ϴ�.").LogError();
                                value = paramInfo.defaultValue;
                            }
                        }

                        if (value == string.Empty)
                        {
                            value = "";
                        }

                        colums.Add(value);
                    }


                    writer.WriteRow(colums);
                    colums = new();
                });
            }
        }

        using (var writer = new CsvFileWriter(textPath))
        {
            List<string> colums = new List<string>();

            //��ũ��Ʈ ���� ó��
            {
                string[] keys = Enum.GetNames(typeof(ScriptDataKey));

                colums.AddRange(keys);
                writer.WriteRow(colums);
                colums.Clear();
            }

            //��ũ��Ʈ ó��
            {
                nodeGrp.TraversalNode(true, nodeGrp.head, (index, branchIndex, depth, node) =>
                {
                    var script = node.script;

                    if(script.scriptType != ScriptType.Text) { return; }

                    foreach (ScriptDataKey key in Enum.GetValues(typeof(ScriptDataKey)))
                    {
                        string value = node.script.GetVariableFromKey(key) ?? string.Empty;

                        if (value == string.Empty)
                        {
                            value = "";
                        }

                        colums.Add(value);
                    }

                    writer.WriteRow(colums);
                    colums = new();
                });
            }
        }
    }
}
