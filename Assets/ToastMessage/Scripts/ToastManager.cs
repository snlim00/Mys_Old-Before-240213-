using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToastManager : MonoBehaviour
{
    public static ToastManager instance;

    public GameObject content;

    private GameObject msgPref;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        msgPref = Resources.Load<GameObject>("Prefabs/MessagePrefab");
    }

    public void ToastMessage(string message)
    {
        ToastMessage toastMsg = Instantiate(msgPref).GetComponent<ToastMessage>();

        toastMsg.Print(message);

        int childCount = content.transform.childCount;
        if(childCount > 2)
        {
            "Destroy".Log();
            content.transform.GetChild(0).GetComponent<ToastMessage>().Remove();
        }
    }

    public static void Print(string message)
    {
        instance.ToastMessage(message);
    }
}
