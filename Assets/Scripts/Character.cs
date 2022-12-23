using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

//캐릭터들을 클래스로 만들까 말까 고민 중. 221223
public class Character : MonoBehaviour
{
    public Image image;

    public int latest = ScriptObject.UNVALID_ID; //마지막으로 호출된 scriptID

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Init(ScriptObject script)
    {
        
    }
}
