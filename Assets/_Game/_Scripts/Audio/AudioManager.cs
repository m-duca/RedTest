﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioManager : MonoBehaviour
{
    #region Variáveis
    [Header("Assets:")]
    [SerializeField] private MusicScriptable[] musics;
    [SerializeField] private SFXScriptable[] sfxs;

    // Componentes:
    private FadeVolume _fadeVolume;

    // Música Atual:
    private static float musicCurTime;
    private static string musicCurName;
    private static AudioSource curMusicAudioSource;
    private GameObject musicCurObj;

    public static AudioManager Instance;
    #endregion

    #region Funções Unity
    
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject); // Faz com que o objeto mova entre as scenes
        AudioListener.volume = 1;
        Instance = this;
    }
    

    private void Start() => _fadeVolume = GetComponent<FadeVolume>();

    private void Update()
    {
        if (_fadeVolume.enabled == false)
            AudioListener.volume = 1; // Alterando o masterVolume com base no PlayerPrefs

        // Retomando o progresso da música que estava tocando
        if (curMusicAudioSource != null) musicCurTime = curMusicAudioSource.time;
    }
    #endregion

    #region Funções Próprias
    public void PlaySFX(string name, bool loopable=false)
    {
        // Procure pelo sfx desejado
        foreach (SFXScriptable s in sfxs)
        {
            if (s.Clip.name == name) // Caso achar, instancie um objeto com o componente de audio
            {
                var sfx = new GameObject("SFX " + s.Clip.name);
                var sAudioSource = sfx.AddComponent<AudioSource>();
                sAudioSource.clip = s.Clip;
                sAudioSource.volume = s.Volume;
                sAudioSource.pitch = s.Pitch;
                sAudioSource.Play();


                if (loopable)
                    sAudioSource.loop = true;
                else
                    Destroy(sfx, 5f);

                break;
            }
        }
    }

    public void PlayMusic(string name, bool loopable=true)
    {
        // Procure pela música desejada
        foreach (MusicScriptable m in musics)
        {
            if (m.Clip.name == name) // Caso achar, instancie um objeto com o componente de audio
            {
                var mObj = new GameObject("Music " + m.Clip.name);
                var mAudioSource = mObj.AddComponent<AudioSource>();
                mAudioSource.clip = m.Clip;
                mAudioSource.volume = m.Volume;
                if (musicCurTime != 0 && m.Clip.name == musicCurName)
                    mAudioSource.time = musicCurTime;
                else
                    Destroy(musicCurObj);

                mObj.tag = "Music";
                musicCurObj = mObj;
                musicCurName = m.Clip.name;
                mAudioSource.Play();

                if (loopable)
                    mAudioSource.loop = true;

                curMusicAudioSource = mAudioSource;
            }
        }
    }

    public GameObject PlayMusicGet(string name, bool loopable = true)
    {
        // Procure pela música desejada
        foreach (MusicScriptable m in musics)
        {
            if (m.Clip.name == name) // Caso achar, instancie um objeto com o componente de audio
            {
                var mObj = new GameObject("Music " + m.Clip.name);
                var mAudioSource = mObj.AddComponent<AudioSource>();
                mAudioSource.clip = m.Clip;
                mAudioSource.volume = m.Volume;

                mObj.tag = "Music";
                musicCurObj = mObj;
                musicCurName = m.Clip.name;
                mAudioSource.Play();

                if (loopable)
                    mAudioSource.loop = true;

                curMusicAudioSource = mAudioSource;

                return mObj;
            }
        }

        return null;
    }
    #endregion
}
