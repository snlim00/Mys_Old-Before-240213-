using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance = null;

    private DialogManager dialogMgr;

    private GameObject scriptListPref;

    [SerializeField] private ScrollView scirptScrollView;

    private List<ScriptObject> scripts = new();

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
    }

    public void LoadScript(int targetScriptGroupID)
    {
        scripts = new();

        //같은 그룹의 스크립트를 모두 리스트에 추가.
        foreach (var script in ScriptManager.scripts)
        {
            int groupID = ScriptManager.GetGroupID(script.scriptID);

            if(targetScriptGroupID == groupID)
            {
                scripts.Add(script);
            }
        }

        scripts.Sort((a, b) =>
        {
            return a.scriptID.CompareTo(b.scriptID);
        });

        dialogMgr.ExecuteMoveTo(scripts[0].scriptID, SetCurrentScript);
    }

    private void SetCurrentScript(int scriptID)
    {

    }
}
