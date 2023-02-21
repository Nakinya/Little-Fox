using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

///<summary>
///
///</summary>
public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;//��Ϊpublic�����������������playStats��ʱ��ͨ��GameManager�����ʣ����㼯�й���

    private CinemachineFreeLook followCamera;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();//���б�洢�۲��� ���涩�����б�

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    //���ù۲���ģʽ����ע��ķ�������GameManager����playerStats
    public void RegisterPlayer(CharacterStats Player)
    {
        playerStats = Player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();//�л�����ʱ���ø������
        if (followCamera != null)
        {
            followCamera.Follow =playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }

    }

    public void AddObserver(IEndGameObserver observer)//�ڵ�������ʱ��ӵ��б���
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)//��������ʱ���б����Ƴ�
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//�㲥 ÿһ��������GameManager�Ķ��󶼻�ִ��EndNotify�������
    {
        foreach (IEndGameObserver observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    //ֱ����SceneController�����
    //public Transform GetEntrance()//��GameManager���ҵ������˵�new gameʱ������ҵ�λ��
    //{
    //    foreach (var item in FindObjectsOfType<TransitionDestination>())
    //    {
    //        if(item.destinationTag == TransitionDestination.DestinationTag.ENTER)
    //            return item.transform;
    //    }
    //    return null;
    //}
}
