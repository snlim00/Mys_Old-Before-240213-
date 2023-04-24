using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MysObject : MonoBehaviour
{
    private NewEventManager eventMgr;

    public Image image;

    private void Awake()
    {
        eventMgr = FindObjectOfType<NewEventManager>();
    }

    public void SetPosition(int position)
    {
        transform.position = eventMgr.objectPositions[position].transform.position;
    }
}