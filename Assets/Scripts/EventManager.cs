using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System;
using System.Reflection;
using System.Linq;
using UnityEditor;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject characters;
    private GameObject characterPref;
    private Dictionary<string, Image> characterList;

    private void Awake()
    {
        characterList = new Dictionary<string, Image>();
        characterPref = Resources.Load<GameObject>("Prefabs/CharacterPref");
    }

    public Sequence CreateEventSequence(ScriptObject script)
    {
        Sequence eventSequence = DOTween.Sequence();

        CallEvent(script, ref eventSequence);

        //루프 처리
        if(script.eventData.loopCount != 0)
        {
            if(script.eventData.loopDelay != 0)
            {
                eventSequence.AppendInterval(script.eventData.loopDelay);
                "set loop delay".Log();
            }

            eventSequence.SetLoops(script.eventData.loopCount);
            "set loop count".Log();
        }
        else
        {
            ("Not Loop : "+script.eventData.loopCount + " " + script.scriptID).Log();
        }

        return eventSequence;
    }

    public void CallEvent(ScriptObject script, ref Sequence sequence)
    {
        Event_CreateCharacter(sequence, script.eventData);

        switch(script.eventData.eventType)
        {
            case EventType.None:
                ("이벤트가 존재하지 않습니다. ScriptID : " + script.scriptID).LogError();
                break;

            case EventType.CreateCharacter:

                break;
        }
    }

    public void Event_CreateCharacter(Sequence sequence, in EventData eventData)
    {
        string resource = eventData.eventParam[0];
        string index = eventData.eventParam[1];
        //캐릭터 위치 조정 관련된 파라미터 및 코드 필요.
        //캐릭터 방향 관련 파라미터 및 코드 필요

        Sprite sprite = Resources.Load<Sprite>("Images/" + resource);

        Image character = Instantiate(characterPref).GetComponent<Image>();
        characterList[index] = character;

        character.sprite = sprite;
        character.transform.SetParent(characters.transform);
        character.transform.localPosition = new Vector2(0, 0);
        character.color = new Color(255, 255, 255, 0); //테스트 코드

        sequence.Append(character.DOFade(1, eventData.eventDuration));
    }
}