using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Node : MonoBehaviour
{
    private EditorManager editorMgr;

    [SerializeField] private Button button;
    [SerializeField] private Text text;
    [SerializeField] private UILineRenderer lineRdr;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        editorMgr = EditorManager.instance;

        transform.SetParent(editorMgr.graph.transform);
        transform.localScale = Vector2.one;

        int index = editorMgr.nodeList.IndexOf(this);
        transform.localPosition = new Vector2(0, index * -30);

        lineRdr.Points = new Vector2[2];

        Observable.EveryLateUpdate()
            .Subscribe(_ =>
            {
                lineRdr.transform.position = editorMgr.graph.transform.position;

                lineRdr.Points[0] = rect.anchoredPosition;


                int index = editorMgr.nodeList.IndexOf(this);

                if (editorMgr.nodeList.Count <= index + 1)
                {
                    return;
                }

                lineRdr.Points[1] = editorMgr.nodeList[index + 1].rect.anchoredPosition;
                lineRdr.SetAllDirty();
            });
    }
}
