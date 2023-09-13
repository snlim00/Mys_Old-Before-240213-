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

            scriptMgr.SetCurrentScript(branchInfo.scriptId); //여기서 scriptId를 통해 스크립트를 가져오려고 하면 scriptId가 없음. //스크립트가 뒤죽박죽이 되어있음. 일부 Id는 유실됨.

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

            //스크립트 정보 처리
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

            //스크립트 처리
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

                        if (paramInfo == null) //사용하지 않는 파라미터라면
                        {
                            if (ScriptInfo.scriptParamInfos.TryGetValue(key, out var scriptParam)) //공용 파라미터에 있는 지 확인
                            {
                                paramInfo = scriptParam;

                                if (scriptInfo.excludedKeys.Contains(key) == true) //공용 파라미터에 있지만 제외되는 키라면 값을 기본값으로 변경.
                                {
                                    value = paramInfo.defaultValue;
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(value)) //공용 파라미터면서 사용되는 값이지만 입력되지 않은 경우 기본값으로 설정.
                                    {
                                        value = paramInfo.defaultValue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value)) //사용되는 파라미터임에도 값이 없다면 기본 값을 적용.
                            {
                                (node.script.scriptId + " | " + paramInfo.paramName + " : 해당 값이 입력되지 않았습니다.").LogError();
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

            //스크립트 정보 처리
            {
                string[] keys = Enum.GetNames(typeof(ScriptDataKey));

                colums.AddRange(keys);
                writer.WriteRow(colums);
                colums.Clear();
            }

            //스크립트 처리
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
