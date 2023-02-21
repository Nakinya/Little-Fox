using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//场景加载
using UnityEngine.AI;
///<summary>
///SceneManager是unity自带的类型
///</summary>
public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    GameObject player;
    NavMeshAgent playerAgent;

    private bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);//不要在加载新场景中删除此脚本
    }
    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)//判断是同场景还是不同场景
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene://同场景传送
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.scenceName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)//目的是异步加载场景
    {
        //保存数据
        SaveManager.Instance.SavePlayerData();//切换场景前先保存数据
        if (SceneManager.GetActiveScene().name != sceneName)//加载不同场景
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//异步加载场景
            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);//传送到该场景目标点
            SaveManager.Instance.LoadPlayerData();//切换场景加载player后读取数据
            yield break;//中断协程
        }
        else//同一场景传送
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;//传送前先关闭agent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)//找到该场景中要传送到的位置
    {
        var entrance = FindObjectsOfType<TransitionDestination>();//找到该场景中所有的TransitionDestiontion
        foreach (var enterPoint in entrance)
        {
            if(enterPoint.destinationTag == destinationTag)
                return enterPoint;
        }
        return null;
    }

    public void TransitionToMenu()//Back to Menu
    {
        StartCoroutine(LoadMenu());
    }
    public void TransitionToLoadGame()//Menu LoadGame
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));//这里只加载场景，加载玩家数据在player controller里进行
    }
    public void TransitionToFirstLevel()//Menu NewGame
    {
        StartCoroutine(LoadLevel("Scene1"));
    }
    IEnumerator LoadLevel(string scene)//加载场景和人物
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != null)
        {
            yield return StartCoroutine(fade.FadeOut(2f));//加载场景前先白屏
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GetDestination(TransitionDestination.DestinationTag.ENTER).transform.position, GetDestination(TransitionDestination.DestinationTag.ENTER).transform.rotation);
            //保存数据
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2f));//加载完场景后恢复
            yield break;
        }
    }

    IEnumerator LoadMenu()//按esc退到主菜单
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Menu");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()//注意，因为NotifyObservers是在update里调用的，频繁调用这个endnotify方法可能会导致游戏崩溃,所以加入一个布尔值来判断协程是否开始
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMenu());
        }
    }
}
