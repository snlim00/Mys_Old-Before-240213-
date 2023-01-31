using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Object/Test Data", order = int.MaxValue)]
public class Test : ScriptableObject
{
    public int a;
    public float b;
    public string c;

    [SerializeField] public List<Vector2> list;
}