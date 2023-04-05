using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void Open_YoutubeURL()
    {
        Application.OpenURL("https://www.youtube.com/");
        Debug.Log("현재 공식 채널이 개설되지 않았습니다! \n빠른시일내로 개설하겠습니다!");
    }
    public void Open_TwitterURL()
    {
        Application.OpenURL("https://twitter.com/");
        Debug.Log("현재 공식 채널이 개설되지 않았습니다! \n빠른시일내로 개설하겠습니다!");
    }
    public void Open_InstagramURL()
    {
        Application.OpenURL("https://www.instagram.com/");
        Debug.Log("현재 공식 채널이 개설되지 않았습니다! \n빠른시일내로 개설하겠습니다!");
    }

    public void Open_BlogURL()
    {
        Application.OpenURL("https://blog.naver.com/kaelo1205");
    }

    public void Open_NaverCafeURL()
    {
        Application.OpenURL("https://www.naver.com/");
        Debug.Log("현재 공식 채널이 개설되지 않았습니다! \n빠른시일내로 개설하겠습니다!");
    }

}//end class