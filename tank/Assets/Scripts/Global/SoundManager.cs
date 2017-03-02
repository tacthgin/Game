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

    public void playMusic(AudioClip clip)
    {
        if (!_isPlayingMusic)
            return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void playAudioClip(AudioClip clip)
    {
        if (!_isPlayingEffect)
            return;
        effectSource.clip = clip;
        effectSource.Play();
    }

    public void playButtonAudio()
    {
        playAudioClip(buttonClip);
    }

    public void setAudioEffectEnable(bool enable)
    {
        _isPlayingEffect = enable;
    }

    public void setAudioMusicEnable(bool enable)
    {
        _isPlayingMusic = enable;
    }
}
