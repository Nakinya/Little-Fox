using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
///
///</summary>
public class SaveManager : Singleton<SaveManager>
{
    private string sceneName = "level";//ֻ��ΪKey��ʹ��
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMenu();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.name);
    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.name);
    }

    private void Save(object data, string key)//�洢����
    {
        var jsonData = JsonUtility.ToJson(data,true);//�Ƚ�monobehavior����scriptable�������л�Ϊjson��ʽ
        PlayerPrefs.SetString(key, jsonData);//��ֵ�ԣ���key��ʶ����PlayerPrefs
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);//���浱ǰ������
        PlayerPrefs.Save();//���޸�д�����
    }
    private void Load(object data,string key)//��ȡ����
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);//��playerPrefs���ȡkey����ʶ�����ݷ����л���д��data��
        }
    }
}
