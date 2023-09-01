using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiableObject : MonoBehaviour
{
    [SerializeField] protected GameObject noti;

    public void ShowNoti(bool? doShow)
    {
        noti.SetActive(doShow ?? !noti.activeSelf);
    }
}
