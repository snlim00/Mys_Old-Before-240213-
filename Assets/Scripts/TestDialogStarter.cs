using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class TestDialogStarter : MonoBehaviour
{
    [SerializeField] private InputField inputField;

    private void Awake()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onEndEdit.AddListener(text => {
            int scriptGroupID = int.Parse(text);
            int scriptID = ScriptManager.GetFirstScriptIDFromGroupID(scriptGroupID);

            SceneManager.LoadScene("DialogScene");

            if (GameConstants.isEditorMode == true)
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                    EditorManager.instance.EditorStart(scriptGroupID)
                );
            }
            else
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                {
                    DialogManager.instance.ReadScript(scriptGroupID);
                    DialogManager.instance.ExecuteMoveTo(scriptID, DialogManager.instance.DialogStart);
                });
            }
        });
    }
}
