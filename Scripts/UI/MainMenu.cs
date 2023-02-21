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
    private PlayableDirector director; //����TimeLine
    private void Awake()
    {
        newGameBtn =transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;//һ��action�¼�,�������ź�ִ��

        newGameBtn.onClick.AddListener(PlayTimeLine);//�����ʼ��ť�Ȳ��ſ�������
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    private void PlayTimeLine()
    {
        director.Play();//���Ŷ���
    }
    private void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //ת������
        SceneController.Instance.TransitionToFirstLevel();
    }
    private void ContinueGame()
    {
        //ת����������ȡ�������
        if (SaveManager.Instance.SceneName != "")//û�ж�Ӧ��keyʱ��ȡ��������Ϊ���ַ����������״ν�����Ϸû�д浵��continue gameʱ����
            SceneController.Instance.TransitionToLoadGame();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
