using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour {
    public int tankId;
    private TankInfo _tankInfo = null;
    private BulletInfo _bulletInfo = null;
	void Start ()
    {
        ReadJson readJson = GameManager.instance.readJson;
        _tankInfo = (TankInfo)readJson.getInfo<TankInfo>(DataType.TANK_INFO, tankId).Clone();
	}
	
	void Update ()
    {
		
	}
}
