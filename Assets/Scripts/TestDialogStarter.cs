using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class TestDialogStarter : MonoBehaviour
{
    [SerializeField] private InputField inputField;

    public int scriptGroupID = -1;

    private void Awake()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onEndEdit.AddListener(text => {
            int scriptID = int.Parse(text);

            if(GameConstants.isEditorMode == false)
            {
                scriptGroupID = ScriptManager.GetGroupID(scriptID);
            }
            else
            {
                scriptGroupID = scriptID;
            }

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
