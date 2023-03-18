using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] InputField inputField;

    private void Start()
    {
        inputField.onValueChanged.AddListener(_ => {
            });
    }
}