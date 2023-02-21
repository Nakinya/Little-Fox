using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

///<summary>
///
///</summary>
public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;//作为public是想其他对象想访问playStats的时候都通过GameManager来访问，方便集中管理

    private CinemachineFreeLook followCamera;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();//用列表存储观察者 保存订阅者列表

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    //想用观察者模式反向注册的方法告诉GameManager我是playerStats
    public void RegisterPlayer(CharacterStats Player)
    {
        playerStats = Player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();//切换场景时设置跟踪相机
        if (followCamera != null)
        {
            followCamera.Follow =playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }

    }

    public void AddObserver(IEndGameObserver observer)//在敌人生成时添加到列表中
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)//敌人销毁时从列表中移除
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//广播 每一个订阅了GameManager的对象都会执行EndNotify这个方法
    {
        foreach (IEndGameObserver observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    //直接在SceneController里操作
    //public Transform GetEntrance()//在GameManager里找到从主菜单new game时生成玩家的位置
    //{
    //    foreach (var item in FindObjectsOfType<TransitionDestination>())
    //    {
    //        if(item.destinationTag == TransitionDestination.DestinationTag.ENTER)
    //            return item.transform;
    //    }
    //    return null;
    //}
}
