using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXButton : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    private void Start()
    {
        if (TryGetComponent<Button>(out Button btn))
        {
            btn.onClick.AddListener(() => SoundManager.PlaySFX(clip));
        }
    }
}
