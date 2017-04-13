using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource effectSource;
    public AudioSource musicSource;
    public AudioClip buttonClip;

    public static SoundManager instance = null;

    private bool _isPlayingMusic = true;
    private bool _isPlayingEffect = true;
    
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}

    public void PlayMusic(AudioClip clip)
    {
        if (!_isPlayingMusic)
            return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (!_isPlayingEffect)
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
        _isPlayingEffect = enable;
    }

    public void SetAudioMusicEnable(bool enable)
    {
        _isPlayingMusic = enable;
    }
}