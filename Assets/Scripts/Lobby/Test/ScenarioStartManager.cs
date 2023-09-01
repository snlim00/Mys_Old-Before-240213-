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

            RuntimeData.isEditorMode = toggle.isOn;

            if (RuntimeData.isEditorMode == true)
            {
                MysSceneManager.LoadScenarioScene(() =>
                {
                    EditorManager.Instance.EditorStart(scriptGroupId);
                });
            }
            else
            {
                MysSceneManager.LoadScenarioScene(() =>
                {
                    EditorManager.Instance.gameObject.SetActive(false);
                    DialogManager.Instance.StartDialog(scriptGroupId);
                });
            }
        });
    }
}
