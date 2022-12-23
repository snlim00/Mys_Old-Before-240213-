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
        Sequence eventSequence = DOTween.Sequence().Pause();

        CallEvent(script, eventSequence);

        return eventSequence;
    }

    public void CallEvent(ScriptObject script, Sequence sequence)
    {
        Event_CreateCharacter(sequence, 10, script.eventParam);

        switch(script.eventType)
        {
            //case EventType.
        }
    }

    public void Event_CreateCharacter(Sequence sequence, float duration, string[] parameter)
    {
        //작동 확인함. 확장하여 다양하게 사용할 방법 고려할 것. 221222

        string resource = parameter[0];
        string index = parameter[1];

        Sprite sprite = Resources.Load<Sprite>("Images/" + resource);

        Image character = Instantiate(characterPref).GetComponent<Image>();
        characterList[index] = character;

        character.sprite = sprite;
        character.transform.SetParent(characters.transform);
        character.transform.localPosition = new Vector2(0, 0);
        character.color = new Color(255, 255, 255, 0); //테스트 코드

        sequence.Append(character.DOFade(1, 2));
    }
}