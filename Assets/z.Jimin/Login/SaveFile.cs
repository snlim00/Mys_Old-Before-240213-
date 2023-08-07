using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
	public Dictionary<string, int> chapterData;

	public Dictionary<string, int> status;

	public int money;

	public int date;
}