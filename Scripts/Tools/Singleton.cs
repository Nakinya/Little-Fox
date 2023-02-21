using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///һ����Ϸ�кܶ��manager��Ҫ�õ���ģʽ���鷳�����ǲ��÷��͵���ģʽ
///</summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>//���͵���ģʽ  T��singleton������(�̳�)
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
