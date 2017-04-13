using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public GameObject scriptsLoader = null;
    private DialogManager _dialogManager;
    private ReadJson _readJson;

	void Awake ()
    {
	    if(instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _dialogManager = scriptsLoader.GetComponent<DialogManager>();
        _readJson = scriptsLoader.GetComponent<ReadJson>();
	}

    public DialogManager dialogManager
    {
        get { return _dialogManager; }
        set { }
    }

    public ReadJson readJson
    {
        get { return _readJson; }
        set { }
    }
}