using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IObserver
{
    public void AddObserver(IObserver observer);
    public void RemoveObserver(IObserver observer);
    public void OnNotify();
}

public interface Subject
{
    void Notify();
}

public class UI_System : MonoBehaviour
{
    protected virtual void PushUI()
    {
        UI_Manager.Instance.InsertStack(this);
        Debug.Log(this.name + " 오브젝트가 생성혹은 활성화 되었습니다.");
    }

    protected virtual void DeleteUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI_Manager.Instance.DeleteStack();
        }
    }
    
}//end class