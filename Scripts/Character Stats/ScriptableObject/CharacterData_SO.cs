using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// ScriptObject����ͨ��һ���ű���������������ֵ,���ҿ����ڴ��ļ����������Ӧ�õ�����ģ����,���ҷ��㵥���Ľű������������ֵ
///</summary>
[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]//�������Ҽ��˵���ѡ��
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;

    public float LevelMultiplier//ÿ��һ�����������辭�����LevelMultiplier
    {
        get { return 1+(currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//���Ƶȼ���0��maxLevel֮��
        baseExp += (int)(currentExp * LevelMultiplier);//����ÿ���ȼ��ۼƵľ���

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LevelUp!:" + currentLevel + "baseExp:" + baseExp);
    }
}
