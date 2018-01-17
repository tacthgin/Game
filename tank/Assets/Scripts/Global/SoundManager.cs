using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource effectSource;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioClip buttonClip;

    private bool isPlayingMusic = true;
    private bool isPlayingEffect = true;

    static private SoundManager instance = null;

    static public SoundManager MyInstance
    {
        set { }
        get { return instance;  }
    }


    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}

    public void PlayMusic(AudioClip clip)
    {
        if (!isPlayingMusic)
            return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (!isPlayingEffect)
            return;
        effectSource.clip = clip;
        effectSource.Play();
    }

    public void PlayButtonAudio()
    {
        PlayAudioClip(buttonClip);
    }

    public void SetAudioEffectEnable(bool enable)
    {
        isPlayingMusic = enable;
    }

    public void SetAudioMusicEnable(bool enable)
    {
        isPlayingEffect = enable;
    }
}