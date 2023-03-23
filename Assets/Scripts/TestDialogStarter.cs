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
            int scriptID = int.Parse(text);

            SceneManager.LoadScene("DialogScene");

            if (GameConstants.isEditorMode == true)
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                    EditorManager.instance.EditorStart(ScriptManager.GetGroupID(scriptID))
                );
            }
            else
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                    DialogManager.instance.ExecuteMoveTo(scriptID, DialogManager.instance.DialogStart)
                );
            }
        });
    }
}
