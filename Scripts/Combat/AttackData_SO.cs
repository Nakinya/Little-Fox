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
    public float skillRange;//远程攻击距离
    public float coolDown;//冷却时间
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;//暴击加成
    public float criticalChance;//暴击几率
}
