using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
///<summary>
///
///</summary>
public enum EnemyState { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyState enemyState;
    private Animator anim;
    protected CharacterStats characterStats;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;//这个敌人是不是站桩的敌人
    private float speed;//敌人移动速度，巡逻与追击状态速度不同
    protected GameObject attackTarget;

    public float lookAtTime;//站在原地停留观察的时间
    private float remainLookAtTime;
    private Quaternion guardRotation;//物体站桩时面朝的方向

    [Header("Patrol State")]
    public float patrolRange;//巡逻的范围
    private Vector3 wayPoint;//巡逻点
    private Vector3 guardPos;//初始刷新的坐标
    private float lastAttackTime;

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool PlayerDead;

    private void Awake()
    {
        Debug.Log("enemy awake");
        agent = this.GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = this.GetComponent<Animator>();
        characterStats = this.GetComponent<CharacterStats>();
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
        guardRotation = transform.rotation;
        coll = this.GetComponent<Collider>();
    }
    private void Start()
    {
        Debug.Log("enemy start");
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetWayPoint();//初始化一个巡逻点
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }

    //切换场景时启用
    //private void OnEnable()//场景加载 生成对象时加入列表，如果不是场景加载时调用则会报空指针异常
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    private void OnDisable()//销毁后移除列表 正常打包游戏时不会报错，而在编辑器中游戏停止时也会调用一次，会导致编辑器的额外报错
    {
        if (!GameManager.IsInitialized) return;//解决编辑器中的报错
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        if (characterStats.CurrentHealth <= 0)
            isDead = true;
        if (!PlayerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    private void SwitchStates()
    {
        if (isDead)
        {
            enemyState=EnemyState.DEAD;
        }
        //如果发现player，切换到CHASE
        else if (FindPlayer())
        {
            enemyState = EnemyState.CHASE;
        }

        switch (enemyState)
        {
            case EnemyState.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//敌人回到站桩位置后缓慢旋转归位
                    }
                }

                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)//停下来的距离 判断是否到了随机巡逻点
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                        GetWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyState.CHASE:
                //播放攻击动画
                agent.speed = speed;
                isWalk = false;
                isChase = true;
                //脱战回到上一个状态
                if (!FindPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;//刚脱战时移动有延迟，将其位置固定在脱战位置停留一段时间
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyState = EnemyState.GUARD;
                    }
                    else
                    {
                        enemyState = EnemyState.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //在攻击范围内执行攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;//先让敌人停下来
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击判断
                        characterStats.isCritical = UnityEngine.Random.value <= characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }

                }
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//后面会修改 
                agent.radius = 0;//范围为0则物体也不会被视为障碍物，解决StopAgent的一个bug
                Destroy(gameObject, 2f);
                break;
        }
    }
    private void Attack()
    {
        transform.LookAt(attackTarget.transform.position);
        if (TargetInAttackRange())
        {
            //执行攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //执行远程攻击动画
            anim.SetTrigger("Skill");
        }
    }
    private bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }
    private bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    private bool FindPlayer()
    {
        var colliders = Physics.OverlapSphere(this.transform.position, sightRadius);//一定范围内所有碰撞体的数组
        foreach (Collider target in colliders)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                this.attackTarget = target.gameObject;
                return true;
            }
        }
        return false;
    }

    private void GetWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float VectorX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float VectorZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + VectorX, transform.position.y, guardPos.z + VectorZ);//保留y是因为如果有坑洼的地方，用初始位置的y值可能会让enemy在半空移动
        //让敌人在一定范围内巡逻
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? randomPoint : transform.position;//判断随机点是否在可导航的范围
    }

    private void OnDrawGizmosSelected()//在scene面板现实范围,Selected是指选中后才显示
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);//是对目标的数值进行操作,所以为targetStats
        }
    }

    public void EndNotify()//订阅者接收函数
    {
        //获胜动画
        //停止所有移动
        //停止agent
        anim.SetBool("Win", true);
        PlayerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
