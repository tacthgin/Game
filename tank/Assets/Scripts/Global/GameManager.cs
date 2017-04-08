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

        GameObject dialogManager = (GameObject)Instantiate(Resources.Load("Prefabs/Dialogs/DialogManager"));
        dialogManager.transform.parent = instance.transform;
        _dialogManager = dialogManager.GetComponent<DialogManager>();
	}

    public DialogManager dialogManager
    {
        get { return _dialogManager; }
        set { }
    }
}