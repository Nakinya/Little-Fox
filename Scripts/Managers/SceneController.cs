using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//��������
using UnityEngine.AI;
///<summary>
///SceneManager��unity�Դ�������
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
        DontDestroyOnLoad(this);//��Ҫ�ڼ����³�����ɾ���˽ű�
    }
    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)//�ж���ͬ�������ǲ�ͬ����
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene://ͬ��������
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.scenceName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)//Ŀ�����첽���س���
    {
        //��������
        SaveManager.Instance.SavePlayerData();//�л�����ǰ�ȱ�������
        if (SceneManager.GetActiveScene().name != sceneName)//���ز�ͬ����
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//�첽���س���
            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);//���͵��ó���Ŀ���
            SaveManager.Instance.LoadPlayerData();//�л���������player���ȡ����
            yield break;//�ж�Э��
        }
        else//ͬһ��������
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;//����ǰ�ȹر�agent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)//�ҵ��ó�����Ҫ���͵���λ��
    {
        var entrance = FindObjectsOfType<TransitionDestination>();//�ҵ��ó��������е�TransitionDestiontion
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
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));//����ֻ���س������������������player controller�����
    }
    public void TransitionToFirstLevel()//Menu NewGame
    {
        StartCoroutine(LoadLevel("Scene1"));
    }
    IEnumerator LoadLevel(string scene)//���س���������
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != null)
        {
            yield return StartCoroutine(fade.FadeOut(2f));//���س���ǰ�Ȱ���
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GetDestination(TransitionDestination.DestinationTag.ENTER).transform.position, GetDestination(TransitionDestination.DestinationTag.ENTER).transform.rotation);
            //��������
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2f));//�����곡����ָ�
            yield break;
        }
    }

    IEnumerator LoadMenu()//��esc�˵����˵�
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Menu");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()//ע�⣬��ΪNotifyObservers����update����õģ�Ƶ���������endnotify�������ܻᵼ����Ϸ����,���Լ���һ������ֵ���ж�Э���Ƿ�ʼ
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMenu());
        }
    }
}
