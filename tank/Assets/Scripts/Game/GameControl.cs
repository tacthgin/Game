using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameControl : MonoBehaviour{

    private int _level = 0;
    private int _life = 3;
    private int _enemy = 20;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void OnJoystickBgDown()
    {
        Debug.Log("down");
    }

    public void OnJoystickDrag()
    {
        Debug.Log("move");
    }

    public void OnJoystickBgUp()
    {
        Debug.Log("up");
    }
}