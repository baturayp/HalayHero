using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript : MonoBehaviour
{
    //this script persists across scenes
    public static SingletonScript instance;

    public int collNumber = 0;
    public bool openFrame = false;
    
    void Awake()
    {
        //singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
