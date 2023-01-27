using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI.Extensions;


public class Human
{
    public string name;

    public int age;
    public int height;
    public int weight;

    public int iq;

    //override : 덮어쓰기
    public virtual string Introduce()
    {
        return "저는 " + name + "입니다.";
    }
}

public class Student : Human
{
    public string school;

    public override string Introduce()
    {
        string prevIntroduce = base.Introduce();

        return prevIntroduce + school + "에 다니고 있습니다.";
    }
}

public class Test : MonoBehaviour
{
    // main 함수
    private void Start()
    {
        Student uiman = new();
        uiman.name = "최의만";
        uiman.school = "한세사이버보안고등학교";
    }
}
