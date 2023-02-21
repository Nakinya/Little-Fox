using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///用来管理ScriptObject数据模板
///</summary>
public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;//攻击更新敌人的血条

    public CharacterData_SO templateData;//模板数据 如果直接复制场景中的物体他们公用的是同一套数据，会出现同生共死的情况，所以先引入一套模板数据
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]//不在inspector中显示
    public bool isCritical;

    private void Awake()
    {
        if (templateData != null)//将模板数据复制一份到此物体中，这个物体就拥有了单独的characterStats
        {
            characterData = Instantiate(templateData);
        }
    }

    #region Read From characterData_SO
    public int MaxHealth
    {
        get { return characterData != null ? characterData.maxHealth : 0; }
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { return characterData != null ? characterData.currentHealth : 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { return characterData != null ? characterData.baseDefence : 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { return characterData != null ? characterData.currentDefence : 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion
    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);//实际攻击数值不能小于0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)//注意是攻击者暴击了
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");//防御者播放被暴击动画
        }

        //update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //经验update
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);//给攻击者加经验值
        }
    }

    public void TakeDamage(int damage, CharacterStats defender)//重载石头造成的伤害
    {
        int currentDamge = Mathf.Max(damage - defender.CurrentDefence, 0);
        defender.CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)//石头人的生命
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);//给玩家加经验
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！：" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
