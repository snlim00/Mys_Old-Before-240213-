using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_System_Child : UI_System
{
    private void OnEnable()
    {
        PushUI();
    }

    protected override void PushUI()
    {
        base.PushUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))    //�� �̵� �׽�Ʈ
        {
            SceneLoader.Instance.LoadScene("Project_Test");
        }
    }
}
