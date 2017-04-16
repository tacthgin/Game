using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {
    public int tankId;
    public GameObject[] bulletArray;
    public GameObject[] propArray;

    private TankInfo _tankInfo = null;
    private BulletInfo _bulletInfo = null;
    private GameObject _bullet = null;
	void Start ()
    {
        ReadJson readJson = GameManager.instance.readJson;
        _tankInfo = (TankInfo)readJson.getInfo<TankInfo>(DataType.TANK_INFO, tankId).Clone();
        _bulletInfo = (BulletInfo)readJson.getInfo<BulletInfo>(DataType.BULLET_INFO, _tankInfo.bulletType).Clone();
	}
	
	void Update ()
    {
		
	}
}
