using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MysObject : MonoBehaviour
{
    private EventManager eventMgr;

    public Image image;

    public int position = 0;

    private void Awake()
    {
        eventMgr = FindObjectOfType<EventManager>();
    }

    public void SetPosition(int position)
    {
        Transform parent = eventMgr.characterSet[position].transform;

        if(parent.childCount > 0)
        {
            ("해당 위치에 이미 캐릭터가 있습니다 : " + position).Log();
            parent.DestroyAllChildren();
        }

        transform.SetParent(parent);

        transform.localPosition = Vector3.zero;
    }
}
