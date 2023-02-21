using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

///<summary>
///
///</summary>
public class SaveManager : Singleton<SaveManager>
{
    private string sceneName = "level";//只作为Key来使用
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

    private void Save(object data, string key)//存储数据
    {
        var jsonData = JsonUtility.ToJson(data,true);//先将monobehavior或者scriptable类型序列化为json格式
        PlayerPrefs.SetString(key, jsonData);//键值对，用key标识存入PlayerPrefs
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);//保存当前场景名
        PlayerPrefs.Save();//将修改写入磁盘
    }
    private void Load(object data,string key)//读取数据
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);//从playerPrefs里读取key所标识的内容反序列化后写入data中
        }
    }
}
