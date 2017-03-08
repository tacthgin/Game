using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseDialog : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Close()
    {
        GameManager.instance.dialogManager.CloseDialog();
    }

    public void OnPointerClick(PointerEventData data)
    {

    }
}
