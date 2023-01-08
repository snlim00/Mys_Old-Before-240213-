using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private EventManager eventMgr;

    public Image image;

    private void Awake()
    {
        eventMgr = FindObjectOfType<EventManager>();
    }

    public void SetIndex(int index)
    {
        Transform parent = eventMgr.characterSet[index].transform;

        transform.SetParent(parent);

        transform.localPosition = Vector3.zero;
    }
}
