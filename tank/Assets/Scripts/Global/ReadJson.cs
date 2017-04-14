using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadJson : MonoBehaviour {
    public enum DataType
    {
        TANK_INFO,
        BULLET_INFO,
        LEVEL_TANK_INFO
    }

    [System.Serializable]
    public class TankInfo
    {
        public int id = 0;
        public int speed = 0;
        public int blood = 0;
        public int shootTime = 0;
        public int bulletType = 0;
        public int score = 0;
    }

    [System.Serializable]
    public class BulletInfo
    {
        public int id = 0;
        public int power = 0;
        public int speed = 0;
        public bool armor = false;
        public int imageId = 0;
    }

    [System.Serializable]
    public class LevelTankInfo
    {
        public int[] array = null;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array = null;
    }

    private Dictionary<DataType, ArrayList> _arrayDictionary = new Dictionary<DataType, ArrayList>();

    void Start ()
    {
        TankInfo[] tankArray = readFile<TankInfo>("tank");
        _arrayDictionary.Add(DataType.TANK_INFO, ArrayList.Adapter(tankArray));

        BulletInfo[] bulletArray = readFile<BulletInfo>("bullet");
        _arrayDictionary.Add(DataType.BULLET_INFO, ArrayList.Adapter(bulletArray));

        LevelTankInfo[] levelArray = readFile<LevelTankInfo>("level");
        _arrayDictionary.Add(DataType.LEVEL_TANK_INFO, ArrayList.Adapter(levelArray));
    }

    T[] readFile<T>(string relativePath)
    {
        TextAsset asset = Resources.Load<TextAsset>("Json/" + relativePath);
        if (asset)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(asset.text);
            return wrapper.array;
        }

        return null;
    }

    public T getInfo<T>(DataType type, int index)
    {
        ArrayList array = _arrayDictionary[type];
        if(array != null)
        {
            if (index >= 0 && index < array.Count)
                return (T)array[index];
        }

        return default(T);
    }
}
