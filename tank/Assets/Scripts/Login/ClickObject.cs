using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickObject : MonoBehaviour {
    private string _currentSoundBtnState = "normal";

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

    void onClick(GameObject gameObject)
    {
        SoundManager.instance.PlayButtonAudio();
        string name = gameObject.name;
        if(name == "soundBtn")
        {
            _currentSoundBtnState = (_currentSoundBtnState == "normal") ? "select" : "normal";
            string btnName = (_currentSoundBtnState == "normal") ? "sound_normal" : "sound_select";
            Image btnImage = gameObject.GetComponent<Image>();
            btnImage.sprite = Resources.Load("Material/Button/" + btnName, typeof(Sprite)) as Sprite;
            SoundManager.instance.SetAudioEffectEnable(_currentSoundBtnState == "normal");
            SoundManager.instance.SetAudioMusicEnable(_currentSoundBtnState == "normal");
        }else if (name == "playBtn")
        {
            SceneManager.LoadScene(1);
        }else
        {
            GameManager.instance.dialogManager.ShowDialog(DialogManager.StoreDialog);
        }
    }
}