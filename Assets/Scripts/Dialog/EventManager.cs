using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    private DialogManager dialogMgr;

    public GameObject[] characterSet;
    private GameObject characterPref;
    private Dictionary<int, Character> characterList;

    private void Awake()
    {
        characterList = new();

        characterPref = Resources.Load<GameObject>("Prefabs/CharacterPref");
    }

    private void Start()
    {
        dialogMgr = DialogManager.instance;
    }

    public Sequence CreateEventSequence(ScriptObject script)
    {
        Sequence seq = DOTween.Sequence();

        //선딜 처리
        if (script.eventData.eventDelay > 0)
        {
            seq.AppendInterval(script.eventData.eventDelay);
        }

        CallEvent(script, ref seq);

        //루프 처리
        if (script.eventData.loopCount != 0)
        {
            if (script.eventData.loopDelay != 0)
            {
                seq.AppendInterval(script.eventData.loopDelay);
            }

            seq.SetLoops(script.eventData.loopCount, script.eventData.loopType);
        }

        return seq;
    }


    public void CallEvent(ScriptObject script, ref Sequence sequence)
    {
        switch(script.eventData.eventType)
        {
            case EventType.None:
                ("이벤트가 존재하지 않습니다. ScriptID : " + script.scriptID).LogError();
                break;

            case EventType.CreateCharacter:
                Event_CreateCharacter(script, ref sequence);
                break;

            case EventType.RemoveCharacter:
                Event_RemoveCharacter(script, ref sequence);
                break;

            case EventType.RemoveAllCharacter:
                Event_RemoveAllCharacter(script, ref sequence);
                break;

            case EventType.Goto:
                Event_Goto(script, ref sequence);
                break;

            case EventType.Branch:
                Event_Branch(script, ref sequence);
                break;
        }
    }

    public void Event_CreateCharacter(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string resource = eventData.eventParam[0];
        int index = int.Parse(eventData.eventParam[1]);
        
        Sprite sprite = Resources.Load<Sprite>("Images/" + resource);

        Character character = Instantiate(characterPref).GetComponent<Character>();
        characterList[index] = character;

        void CreateCharacter()
        {
            character.SetPosition(index);
            character.image.sprite = sprite;

            character.image.SetAlpha(0);
        }

        sequence.AppendCallback(CreateCharacter);
        sequence.Append(character.image.DOFade(1, eventData.eventDuration));
    }

    public void RemoveCharacter(int index)
    {
        Character character = characterList[index];

        characterList.Remove(index);

        Destroy(character.gameObject);
    }

    public void Event_RemoveCharacter(ScriptObject script, ref Sequence sequence)
    {
   
        EventData eventData = script.eventData;

        int index = int.Parse(eventData.eventParam[0]);

        script.scriptID.Log();
        characterList.Count.Log();

        Character character = characterList[index];

        sequence.Append(character.image.DOFade(0, eventData.eventDuration));
        sequence.AppendCallback(() => RemoveCharacter(index));
    }

    public void RemoveAllCharacter()
    {
        Dictionary<int, Character>.KeyCollection keys = characterList.Keys;

        List<int> keyList = new();

        foreach(int key in keys)
        {
            keyList.Add(key);
        }

        for(int i = 0; i < keyList.Count; ++i)
        {
            RemoveCharacter(keyList[i]);
        }
    }

    public void Event_RemoveAllCharacter(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        sequence.AppendCallback(RemoveAllCharacter);
    }

    public void Event_AddLovePoint(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string target = eventData.eventParam[0];
        int amount = int.Parse(eventData.eventParam[1]);

        sequence.AppendCallback(() => ProgressData.AddLovePoint(script, target, amount));
    }

    public void Goto(int scriptID)
    {
        dialogMgr.Goto(scriptID);
    }

    public void Event_Goto(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        int scriptID = int.Parse(eventData.eventParam[0]);

        sequence.AppendCallback(() => Goto(scriptID));
    }

    public void Event_Branch(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        string target = eventData.eventParam[0];
        int lovePoint = ProgressData.GetLovePointFromCharacter(script, target);

        List<int> requiredValue = new();
        List<int> targetScriptID = new();

        for(int i = 1; i < eventData.eventParam.Count; ++i)
        {
            if(i % 2 == 0)
            {
                int param = int.Parse(eventData.eventParam[i]);
                targetScriptID.Add(param);
            }
            else
            {
                int param = int.Parse(eventData.eventParam[i]);
                requiredValue.Add(param);
            }
        }

        int branch = -1;

        for(int i = requiredValue.Count - 1; i >= 0; --i)
        {
            if (requiredValue[i] <= lovePoint)
            {
                branch = i;
                break;
            }
        }

        if (branch == -1)
        {
            ("분기를 찾을 수 없습니다. ScriptID : " + script.scriptID).Log();
            return;
        }

        int scriptID = targetScriptID[branch];

        sequence.AppendCallback(() => Goto(scriptID));
    }
}
