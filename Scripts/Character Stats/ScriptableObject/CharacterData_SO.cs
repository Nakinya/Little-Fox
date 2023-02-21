using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// ScriptObject可以通过一个脚本创建多个人物的数值,并且可以在此文件里更改内容应用到其他模板中,并且方便单独的脚本化对象更改数值
///</summary>
[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]//可以在右键菜单中选择
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

    public float LevelMultiplier//每升一级，升级所需经验乘以LevelMultiplier
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
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//限制等级在0到maxLevel之间
        baseExp += (int)(currentExp * LevelMultiplier);//保留每个等级累计的经验

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LevelUp!:" + currentLevel + "baseExp:" + baseExp);
    }
}
