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
    public bool isGuard;//��������ǲ���վ׮�ĵ���
    private float speed;//�����ƶ��ٶȣ�Ѳ����׷��״̬�ٶȲ�ͬ
    protected GameObject attackTarget;

    public float lookAtTime;//վ��ԭ��ͣ���۲��ʱ��
    private float remainLookAtTime;
    private Quaternion guardRotation;//����վ׮ʱ�泯�ķ���

    [Header("Patrol State")]
    public float patrolRange;//Ѳ�ߵķ�Χ
    private Vector3 wayPoint;//Ѳ�ߵ�
    private Vector3 guardPos;//��ʼˢ�µ�����
    private float lastAttackTime;

    //bool��϶���
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
            GetWayPoint();//��ʼ��һ��Ѳ�ߵ�
        }
        //FIXME:�����л����޸ĵ�
        GameManager.Instance.AddObserver(this);
    }

    //�л�����ʱ����
    //private void OnEnable()//�������� ���ɶ���ʱ�����б�������ǳ�������ʱ������ᱨ��ָ���쳣
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    private void OnDisable()//���ٺ��Ƴ��б� ���������Ϸʱ���ᱨ�����ڱ༭������ϷֹͣʱҲ�����һ�Σ��ᵼ�±༭���Ķ��ⱨ��
    {
        if (!GameManager.IsInitialized) return;//����༭���еı���
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
        //�������player���л���CHASE
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
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//���˻ص�վ׮λ�ú�����ת��λ
                    }
                }

                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)//ͣ�����ľ��� �ж��Ƿ������Ѳ�ߵ�
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
                //���Ź�������
                agent.speed = speed;
                isWalk = false;
                isChase = true;
                //��ս�ص���һ��״̬
                if (!FindPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;//����սʱ�ƶ����ӳ٣�����λ�ù̶�����սλ��ͣ��һ��ʱ��
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
                //�ڹ�����Χ��ִ�й���
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;//���õ���ͣ����
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //�����ж�
                        characterStats.isCritical = UnityEngine.Random.value <= characterStats.attackData.criticalChance;
                        //ִ�й���
                        Attack();
                    }

                }
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//������޸� 
                agent.radius = 0;//��ΧΪ0������Ҳ���ᱻ��Ϊ�ϰ�����StopAgent��һ��bug
                Destroy(gameObject, 2f);
                break;
        }
    }
    private void Attack()
    {
        transform.LookAt(attackTarget.transform.position);
        if (TargetInAttackRange())
        {
            //ִ�й�������
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //ִ��Զ�̹�������
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
        var colliders = Physics.OverlapSphere(this.transform.position, sightRadius);//һ����Χ��������ײ�������
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

        Vector3 randomPoint = new Vector3(guardPos.x + VectorX, transform.position.y, guardPos.z + VectorZ);//����y����Ϊ����п��ݵĵط����ó�ʼλ�õ�yֵ���ܻ���enemy�ڰ���ƶ�
        //�õ�����һ����Χ��Ѳ��
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? randomPoint : transform.position;//�ж�������Ƿ��ڿɵ����ķ�Χ
    }

    private void OnDrawGizmosSelected()//��scene�����ʵ��Χ,Selected��ָѡ�к����ʾ
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
            targetStats.TakeDamage(characterStats, targetStats);//�Ƕ�Ŀ�����ֵ���в���,����ΪtargetStats
        }
    }

    public void EndNotify()//�����߽��պ���
    {
        //��ʤ����
        //ֹͣ�����ƶ�
        //ֹͣagent
        anim.SetBool("Win", true);
        PlayerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
