using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour {
    public int tankId;
    private TankInfo _tankInfo = null;

	void Start ()
    {
        _tankInfo = GameManager.instance.readJson.getInfo<TankInfo>(ReadJson.DataType.TANK_INFO, tankId);
	}
	
	void Update ()
    {
		
	}
}
