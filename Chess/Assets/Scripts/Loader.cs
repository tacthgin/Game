using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private SoundManager soundManager;

    void Awake()
    {
        if(SoundManager.MyInstance == null)
        {
            Instantiate(soundManager);
        }
    }
}
