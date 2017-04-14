using UnityEngine;
using System;
using UnityEngine.EventSystems;

<<<<<<< HEAD
public class GameControl : MonoBehaviour{

    public enum Direction
    {
        NONE,
        Left,
        DOWN,
        RIGHT,
        UP
    }

    private int _level = 0;
    private int _life = 3;
    private int _enemy = 20;
    private Direction _currentDirection = Direction.NONE;
    private bool _isShootPressed = false;

    private GameObject _joysitckClickPanel = null;
    private GameObject _joystickBg = null;
    private GameObject _joystickCenter = null;
    private Vector2 _joystickBeginPos = new Vector2();

=======
public class GameControl : MonoBehaviour{

    private int _level = 0;
    private int _life = 3;
    private int _enemy = 20;

    private GameObject _joysitckClickPanel = null;
    private GameObject _joystickBg = null;
    private GameObject _joystickCenter = null;
    private Vector2 _joystickBeginPos = new Vector2();

>>>>>>> 07ebbac221daf9f62e97b50c55590145eae27025
	void Start ()
    {
        _joysitckClickPanel = GameObject.Find("joystickClickPanel");
        _joystickBg = GameObject.Find("joystickBg");
        _joystickBeginPos = ((RectTransform)_joystickBg.transform).anchoredPosition;
        _joystickCenter = GameObject.Find("joystickCenter");
    }
<<<<<<< HEAD

    private void FixedUpdate()
    {
        
    }
=======
	
	void Update ()
    {
		
	}
>>>>>>> 07ebbac221daf9f62e97b50c55590145eae27025

    public void OnJoystickBgDown()
    {
        setClickPos(_joysitckClickPanel, _joystickBg);
    }

    public void OnJoystickDrag()
    {
        setClickPos(_joystickBg, _joystickCenter);
        getDirection();
    }

    public void OnJoystickBgUp()
    {
        ((RectTransform)_joystickBg.transform).anchoredPosition = _joystickBeginPos;
        ((RectTransform)_joystickCenter.transform).anchoredPosition = Vector2.zero;
        _currentDirection = Direction.NONE;
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
        Vector2[] directionList = { Vector2.left, Vector2.down, Vector2.right, Vector2.up };
        Vector2 pos = ((RectTransform)_joystickCenter.transform).anchoredPosition;
        for(int i = 0; i < directionList.Length; ++i)
        {
            if(Vector2.Angle(pos, directionList[i]) <= 45)
            {
                _currentDirection = (Direction)(i + 1);
                return;
            }
        }

        _currentDirection = Direction.NONE;
    }

    public void onShootDown()
    {
        _isShootPressed = true;
    }

    public void onShootUp()
    {
        _isShootPressed = false;
    }
}