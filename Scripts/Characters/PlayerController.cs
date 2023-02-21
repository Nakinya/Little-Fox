using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///<summary>
///
///</summary>
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    private GameObject attackTarget;//被攻击的对象
    private float lastAttackTime;//上一次攻击的时间，用来设置攻击间隔

    private bool isDead;

    private float stopDistance;//用来保存初始导航的停止距离

    private void Awake()
    {
        Debug.Log("Player awake");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable() //同一个物体是awake->enable->start，start在所有物体的awake、enable执行后执行，但不能确定不同物体awake和enable的执行顺序
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//事件委托,注册方法到事件中
        MouseManager.Instance.OnEnemyCliked += EventAttack;

        GameManager.Instance.RegisterPlayer(characterStats);//给GameManager注册player
    }
    private void Start()
    {
        //MouseManager.Instance.OnMouseClicked += MoveToTarget;//事件委托,注册方法到事件中
        //MouseManager.Instance.OnEnemyCliked += EventAttack;

        //GameManager.Instance.RegisterPlayer(characterStats);//给GameManager注册player
        SaveManager.Instance.LoadPlayerData();//每次加载player时都读取数据(会覆盖CharacterStats awake里初始化的数据)
    }
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;//加载新场景时移除这个方法
        MouseManager.Instance.OnEnemyCliked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)//玩家死亡后广播，也可以在GameManager里的update实时检测玩家生命
        {
            agent.isStopped = true;//玩家在导航途中死亡,停止移动
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;//缩减冷却时间
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//取消所有协程，否则只有等一次攻击结束后才能移动
        if (isDead) return;//防止玩家死亡后还能进行操作

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;//保证不会在第一次攻击后停止移动
        agent.destination = target; //移动人物到target
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;//防止玩家死亡后还能进行操作

        if (target != null)//防止敌人死后销毁报错
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;//计算是否暴击
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()//协程  伴随一些事件同步进行的我们一般用协程
    {
        transform.LookAt(attackTarget.transform);//首先转向目标

        agent.isStopped = false;
        //攻击敌人时，当敌人进入角色攻击范围就会停下，避免因为敌人体积过大而导致角色无法停止导航（动态更改导航停止距离）
        agent.stoppingDistance = characterStats.attackData.attackRange;
        //攻击范围根据武器长度修改
        //FIXME:范围还要考虑被攻击角色半径(半径+攻击距离)
        while (Vector3.Distance(this.transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;//agent停止移动
        //attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockState == Rock.RockState.HitNothing)//只有当对象为石头且石头在地上时才能攻击
            {
                attackTarget.GetComponent<Rock>().rockState = Rock.RockState.HitEnemy;//切换到攻击石头人的状态
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;//给一个初始速度为1，防止石头直接就进入HitNothing的状态
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else//攻击对象为敌人
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }

    }
}
