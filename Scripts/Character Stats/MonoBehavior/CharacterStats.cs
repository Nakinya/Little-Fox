using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///��������ScriptObject����ģ��
///</summary>
public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;//�������µ��˵�Ѫ��

    public CharacterData_SO templateData;//ģ������ ���ֱ�Ӹ��Ƴ����е��������ǹ��õ���ͬһ�����ݣ������ͬ�����������������������һ��ģ������
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]//����inspector����ʾ
    public bool isCritical;

    private void Awake()
    {
        if (templateData != null)//��ģ�����ݸ���һ�ݵ��������У���������ӵ���˵�����characterStats
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
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);//ʵ�ʹ�����ֵ����С��0
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)//ע���ǹ����߱�����
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");//�����߲��ű���������
        }

        //update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //����update
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);//�������߼Ӿ���ֵ
        }
    }

    public void TakeDamage(int damage, CharacterStats defender)//����ʯͷ��ɵ��˺�
    {
        int currentDamge = Mathf.Max(damage - defender.CurrentDefence, 0);
        defender.CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)//ʯͷ�˵�����
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);//����ҼӾ���
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("��������" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
