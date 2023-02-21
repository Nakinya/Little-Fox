using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
[CreateAssetMenu(fileName ="New AttackData",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("AttackData Info")]
    public float attackRange;
    public float skillRange;//Զ�̹�������
    public float coolDown;//��ȴʱ��
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;//�����ӳ�
    public float criticalChance;//��������
}
