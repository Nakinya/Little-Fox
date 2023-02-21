using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///<summary>
///
///</summary>
public class Rock : MonoBehaviour
{
    public enum RockState { HitPlayer, HitEnemy, HitNothing }

    private Rigidbody rb;
    public RockState rockState;

    [Header("Basic Settings")]
    public float force;//�ƿ�����
    public int damage;
    [HideInInspector]
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;

    private void Start()
    {
        rockState = RockState.HitPlayer;//��ʼ״̬ΪHitPlayer
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;//����ʱ��ʼ�ٶȶ�Ϊ1
        FlyToTarget();
    }
    private void FixedUpdate()//�����жϷ���FixedUpdate����
    {
        if (rb.velocity.sqrMagnitude < 1f)//�ٶȵ�ƽ��С��1ʱ�л���HitNothing����Ҫע��һ��ʼ����ʯͷʱ�����ٶ���0����������ʱ��ʼ�ٶ���Ϊ1
        {
            rockState=RockState.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        if(target == null)//��������ʯͷ�������սʱ��Ȼ�ӳ�ʯͷ�������
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;//��һ�����ϵķ��򣬱���ʯͷֱֱ������ҷɹ�ȥ
        rb.AddForce(direction*force,ForceMode.Impulse);//��ʯͷһ������ģʽΪ�����
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockState)//ʲôʱ��ʯͷ״̬�л���HitNothing
        {
            case RockState.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");

                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStats>());
                    rockState = RockState.HitNothing;
                }
                break;

            case RockState.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect,transform.position,Quaternion.identity);//������ʯ

                    Destroy(gameObject);
                }
                break;
        }
    }
}
