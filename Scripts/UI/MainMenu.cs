using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

///<summary>
///
///</summary>
public class MainMenu : MonoBehaviour
{
    private Button newGameBtn;
    private Button continueBtn;
    private Button quitBtn;
    private PlayableDirector director; //控制TimeLine
    private void Awake()
    {
        newGameBtn =transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;//一个action事件,结束播放后执行

        newGameBtn.onClick.AddListener(PlayTimeLine);//点击开始按钮先播放开场动画
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    private void PlayTimeLine()
    {
        director.Play();//播放动画
    }
    private void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }
    private void ContinueGame()
    {
        //转换场景，读取玩家数据
        if (SaveManager.Instance.SceneName != "")//没有对应的key时读取到的数据为空字符串，避免首次进入游戏没有存档但continue game时报错
            SceneController.Instance.TransitionToLoadGame();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
