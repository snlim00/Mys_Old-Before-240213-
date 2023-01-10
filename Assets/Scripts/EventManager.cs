using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public GameObject[] characterSet;
    private GameObject characterPref;
    private Dictionary<int, Character> characterList;

    private void Awake()
    {
        characterList = new();

        characterPref = Resources.Load<GameObject>("Prefabs/CharacterPref");
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

            case EventType.Goto:
                Event_Goto(script, ref sequence);
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

        void CreateCharacter()
        {
            characterList[index] = character;

            character.SetPosition(index);
            character.image.sprite = sprite;

            character.image.SetAlpha(0);
        }

        sequence.AppendCallback(CreateCharacter);
        sequence.Append(character.image.DOFade(1, eventData.eventDuration));
    }

    public void Event_RemoveCharacter(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        int index = int.Parse(eventData.eventParam[0]);

        Character character = characterList[index];

        void RemoveCharacter()
        {
            characterList.Remove(index);

            Destroy(character);
        }

        sequence.Append(character.image.DOFade(0, eventData.eventDuration));
        sequence.AppendCallback(RemoveCharacter);
    }

    public void Event_Goto(ScriptObject script, ref Sequence sequence)
    {
        EventData eventData = script.eventData;

        int scriptID = int.Parse(eventData.eventParam[0]);

        DialogManager dialogMgr = FindObjectOfType<DialogManager>();

        dialogMgr.Goto(scriptID);
    }
}
