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

            if (scriptID - (ScriptManager.GetPrefixID(scriptID) * 10000) == 1)
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                    FindObjectOfType<DialogManager>().DialogStart(scriptID)
                );
            }
            else
            {
                Observable.TimerFrame(1).Subscribe(_ =>
                    FindObjectOfType<DialogManager>().ExecuteMoveTo(scriptID)
                );
            }
        });
    }
}
