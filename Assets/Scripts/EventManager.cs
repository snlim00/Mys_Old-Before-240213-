using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance = null;

    void Awake()
    {
        if (instance != null) Destroy(this.gameObject);

        instance = this;
    }
}
