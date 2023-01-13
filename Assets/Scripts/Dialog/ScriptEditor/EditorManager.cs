using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UniRx;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

    private DialogManager dialogMgr;

    private EventManager eventMgr;

    private GameObject scriptListPref;

    [SerializeField] private GameObject scrollViewContent;
    private Dictionary<int, ScriptList> scriptLists; //scriptID : ScriptList

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        scriptListPref = Resources.Load<GameObject>("Prefabs/ScriptListPref");
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogMgr = DialogManager.instance;
        eventMgr = FindObjectOfType<EventManager>();

        //테스트 코드
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => eventMgr.RemoveAllCharacter());
    }

    public void LoadScript(int targetScriptGroupID)
    {
        EditorScriptManager.SetScript(targetScriptGroupID);

        dialogMgr.ExecuteMoveTo(EditorScriptManager.scripts[0].scriptID, EditorScriptManager.SetCurrentScript);

        RefreshScriptScrollView();

        int firstScriptID = EditorScriptManager.GetFirstScriptIDFromGroupID(targetScriptGroupID);
        SetCurrentScript(firstScriptID);
    }

    private void SetCurrentScript(int scriptID)
    {
        EditorScriptManager.SetCurrentScript(scriptID);

        foreach (var kvp in scriptLists)
        {
            kvp.Value.SetHighlight(false);
        }

        scriptLists[scriptID].SetHighlight(true);

        dialogMgr.ExecuteMoveTo(scriptID, _ => { });
    }

    private void RefreshScriptScrollView()
    {
        scriptLists = new();
        scrollViewContent.transform.DestroyAllChildren();

        foreach (var script in EditorScriptManager.scripts)
        {
            GameObject obj = Instantiate(scriptListPref);
            ScriptList scriptList = obj.GetComponent<ScriptList>();

            obj.transform.SetParent(scrollViewContent.transform);
            obj.transform.localScale = Vector3.one;
            obj.name = script.scriptID.ToString();

            scriptList.script = script;
            scriptList.SetText(script.scriptID.ToString());
            scriptList.SetHighlight(false);

            ScriptObject prevScript = EditorScriptManager.GetPrevScriptFromID(script.scriptID);

            if (prevScript != null && prevScript.linkEvent == true)
            {
                scriptList.scriptText.color = Color.gray;
                scriptList.selectButton.interactable = false;
            }
            else
            {
                scriptList.SetButtonCallback(() => {
                    SetCurrentScript(scriptList.script.scriptID);
                    ("클릭 : " + scriptList.script.scriptID).Log();
                });
            }



            scriptLists.Add(script.scriptID, scriptList);
        }
    }
}
