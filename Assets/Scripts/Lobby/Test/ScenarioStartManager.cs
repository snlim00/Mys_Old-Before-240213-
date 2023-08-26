using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenarioStartManager : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        inputField.onEndEdit.AddListener(input =>
        {
            if(int.TryParse(inputField.text, out int scriptGroupId) == false)
            {
                return;
            }

            bool isEditorMode = toggle.isOn;

            if (isEditorMode == true)
            {
                MysSceneManager.LoadDialogScene(() =>
                {

                });
            }
            else
            {
                MysSceneManager.LoadDialogScene(() =>
                {
                    DialogManager.Instance.StartDialog(scriptGroupId);
                });
            }
        });
    }
}
