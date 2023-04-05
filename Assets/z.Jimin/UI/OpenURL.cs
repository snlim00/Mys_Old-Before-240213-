using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public void Open_YoutubeURL()
    {
        Application.OpenURL("https://www.youtube.com/");
        Debug.Log("���� ���� ä���� �������� �ʾҽ��ϴ�! \n�������ϳ��� �����ϰڽ��ϴ�!");
    }
    public void Open_TwitterURL()
    {
        Application.OpenURL("https://twitter.com/");
        Debug.Log("���� ���� ä���� �������� �ʾҽ��ϴ�! \n�������ϳ��� �����ϰڽ��ϴ�!");
    }
    public void Open_InstagramURL()
    {
        Application.OpenURL("https://www.instagram.com/");
        Debug.Log("���� ���� ä���� �������� �ʾҽ��ϴ�! \n�������ϳ��� �����ϰڽ��ϴ�!");
    }

    public void Open_BlogURL()
    {
        Application.OpenURL("https://blog.naver.com/kaelo1205");
    }

    public void Open_NaverCafeURL()
    {
        Application.OpenURL("https://www.naver.com/");
        Debug.Log("���� ���� ä���� �������� �ʾҽ��ϴ�! \n�������ϳ��� �����ϰڽ��ϴ�!");
    }

}//end class