using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public DialogManager _dialogManager;

	void Awake ()
    {
	    if(instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}

    public DialogManager dialogManager
    {
        get { return _dialogManager; }
        set { }
    }
}
