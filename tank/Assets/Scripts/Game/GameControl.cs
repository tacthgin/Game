using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameControl : MonoBehaviour{

    private int _level = 0;
    private int _life = 3;
    private int _enemy = 20;

    private GameObject _joysitckClickPanel = null;
    private GameObject _joystickBg = null;
    private GameObject _joystickCenter = null;
    private Vector2 _joystickBeginPos = new Vector2();

	void Start ()
    {
        _joysitckClickPanel = GameObject.Find("joystickClickPanel");
        _joystickBg = GameObject.Find("joystickBg");
        _joystickBeginPos = ((RectTransform)_joystickBg.transform).anchoredPosition;
        _joystickCenter = GameObject.Find("joystickCenter");
    }
	
	void Update ()
    {
		
	}

    public void OnJoystickBgDown()
    {
        setClickPos(_joysitckClickPanel, _joystickBg);
    }

    public void OnJoystickDrag()
    {
        setClickPos(_joystickBg, _joystickCenter);
    }

    public void OnJoystickBgUp()
    {
        ((RectTransform)_joystickBg.transform).anchoredPosition = _joystickBeginPos;
        ((RectTransform)_joystickCenter.transform).anchoredPosition = new Vector2(0, 0);
    }

    void setClickPos(GameObject parent, GameObject moveObject)
    {
        RectTransform parentTransform = (RectTransform)parent.transform;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, Input.mousePosition, Camera.main, out pos);
        RectTransform childTransform = (RectTransform)moveObject.transform;
        childTransform.anchoredPosition = pos;
    }

    void getDirection()
    {

    }
}