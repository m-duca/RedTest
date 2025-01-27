using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXLoopPlayer : MonoBehaviour
{
    [SerializeField] private string sfxName;

    private void Start() => AudioManager.Instance.PlaySFX(sfxName, true);
}