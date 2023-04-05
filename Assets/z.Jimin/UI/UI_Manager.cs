using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manager : Singleton<UI_Manager>
{
    Stack<UI_System> ui_Stack = new Stack<UI_System>(); //���Լ���� ����

    private void Start()
    {
        SceneManager.sceneLoaded += LoadedsceneEvent;
        DontDestroyOnLoad(this);
    }

    private void LoadedsceneEvent(Scene scene, LoadSceneMode mode)  //���� �ٲ� ���� ȣ��
    {
        Debug.Log(scene.name + "���� ����Ǿ����ϴ�.");
        ui_Stack = null;    //UI Stack �ʱ�ȭ
    }

   

    #region UI Stack ����
    public void InsertStack(UI_System systemUI) //���ÿ� �߰��ϱ�
    {
        ui_Stack.Push(systemUI);
    }

  
    public void DeleteStack()   //���� �����
    {
        if(ui_Stack.Count > 0)
        {
            ui_Stack.Peek().gameObject.SetActive(false);
            ui_Stack.Pop();
        }
        else
            return;
    }
    #endregion

}//end class