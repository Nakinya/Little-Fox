using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///一个游戏有很多的manager都要用单例模式很麻烦，于是采用泛型单例模式
///</summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>//泛型单例模式  T是singleton的类型(继承)
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }
    
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else {
            instance = (T)this; 
        }
    }
    public static bool IsInitialized
    {
         get{ return instance != null; } 
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
