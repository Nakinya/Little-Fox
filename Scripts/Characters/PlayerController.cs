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

    private GameObject attackTarget;//�������Ķ���
    private float lastAttackTime;//��һ�ι�����ʱ�䣬�������ù������

    private bool isDead;

    private float stopDistance;//���������ʼ������ֹͣ����

    private void Awake()
    {
        Debug.Log("Player awake");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable() //ͬһ��������awake->enable->start��start�����������awake��enableִ�к�ִ�У�������ȷ����ͬ����awake��enable��ִ��˳��
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//�¼�ί��,ע�᷽�����¼���
        MouseManager.Instance.OnEnemyCliked += EventAttack;

        GameManager.Instance.RegisterPlayer(characterStats);//��GameManagerע��player
    }
    private void Start()
    {
        //MouseManager.Instance.OnMouseClicked += MoveToTarget;//�¼�ί��,ע�᷽�����¼���
        //MouseManager.Instance.OnEnemyCliked += EventAttack;

        //GameManager.Instance.RegisterPlayer(characterStats);//��GameManagerע��player
        SaveManager.Instance.LoadPlayerData();//ÿ�μ���playerʱ����ȡ����(�Ḳ��CharacterStats awake���ʼ��������)
    }
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;//�����³���ʱ�Ƴ��������
        MouseManager.Instance.OnEnemyCliked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)//���������㲥��Ҳ������GameManager���updateʵʱ����������
        {
            agent.isStopped = true;//����ڵ���;������,ֹͣ�ƶ�
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;//������ȴʱ��
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//ȡ������Э�̣�����ֻ�е�һ�ι�������������ƶ�
        if (isDead) return;//��ֹ����������ܽ��в���

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;//��֤�����ڵ�һ�ι�����ֹͣ�ƶ�
        agent.destination = target; //�ƶ����ﵽtarget
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;//��ֹ����������ܽ��в���

        if (target != null)//��ֹ�����������ٱ���
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;//�����Ƿ񱩻�
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()//Э��  ����һЩ�¼�ͬ�����е�����һ����Э��
    {
        transform.LookAt(attackTarget.transform);//����ת��Ŀ��

        agent.isStopped = false;
        //��������ʱ�������˽����ɫ������Χ�ͻ�ͣ�£�������Ϊ���������������½�ɫ�޷�ֹͣ��������̬���ĵ���ֹͣ���룩
        agent.stoppingDistance = characterStats.attackData.attackRange;
        //������Χ�������������޸�
        //FIXME:��Χ��Ҫ���Ǳ�������ɫ�뾶(�뾶+��������)
        while (Vector3.Distance(this.transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;//agentֹͣ�ƶ�
        //attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //������ȴʱ��
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockState == Rock.RockState.HitNothing)//ֻ�е�����Ϊʯͷ��ʯͷ�ڵ���ʱ���ܹ���
            {
                attackTarget.GetComponent<Rock>().rockState = Rock.RockState.HitEnemy;//�л�������ʯͷ�˵�״̬
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;//��һ����ʼ�ٶ�Ϊ1����ֹʯͷֱ�Ӿͽ���HitNothing��״̬
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else//��������Ϊ����
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }

    }
}
