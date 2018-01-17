using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickObject : MonoBehaviour {
    [SerializeField]
    private Button soundBtn;

    [SerializeField]
    private Sprite soundEnableTexture;

    [SerializeField]
    private Sprite soundDisableTexture;

    private bool soundEnable = true;

	void Start ()
    {
        
	}

    private void setSoundBtnState(bool enable)
    {
        soundBtn.image.sprite = enable ? soundEnableTexture : soundDisableTexture;
    }

    public void SoundClick()
    {
        SoundManager.MyInstance.PlayButtonAudio();
        soundEnable = soundEnable ? false : true;
        setSoundBtnState(soundEnable);
        SoundManager.MyInstance.SetAudioEffectEnable(soundEnable);
        SoundManager.MyInstance.SetAudioMusicEnable(soundEnable);
    }

    public void PlayClick()
    {
        SceneManager.LoadScene(1);
    }
}