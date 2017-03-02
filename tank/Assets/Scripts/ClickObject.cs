using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickObject : MonoBehaviour {
    private string _currentSoundBtnState = "normal";
	// Use this for initialization
	void Start ()
    {
        string[] btnNameList = {
            "playBtn",
            "moreBtn",
            "soundBtn"
        };

        for (int i = 0; i < btnNameList.Length; ++i)
        {
            GameObject gameObject = GameObject.Find(btnNameList[i]);
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(delegate ()
            {
                onClick(gameObject);
            });
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void onClick(GameObject gameObject)
    {
        SoundManager.instance.playButtonAudio();
        if(gameObject.name == "soundBtn")
        {
            _currentSoundBtnState = (_currentSoundBtnState == "normal") ? "select" : "normal";
            string btnName = (_currentSoundBtnState == "normal") ? "sound_normal" : "sound_select";
            Image btnImage = gameObject.GetComponent<Image>();
            btnImage.sprite = Resources.Load("Material/Button/" + btnName, typeof(Sprite)) as Sprite;
            SoundManager.instance.setAudioEffectEnable(_currentSoundBtnState == "normal");
            SoundManager.instance.setAudioMusicEnable(_currentSoundBtnState == "normal");
        }
    }
}
