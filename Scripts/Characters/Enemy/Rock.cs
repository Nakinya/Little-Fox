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
    public float force;//推开的力
    public int damage;
    [HideInInspector]
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;

    private void Start()
    {
        rockState = RockState.HitPlayer;//初始状态为HitPlayer
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;//生成时初始速度定为1
        FlyToTarget();
    }
    private void FixedUpdate()//物理判断放在FixedUpdate里面
    {
        if (rb.velocity.sqrMagnitude < 1f)//速度的平方小于1时切换到HitNothing，但要注意一开始生成石头时它的速度是0，所以生成时初始速度设为1
        {
            rockState=RockState.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        if(target == null)//当生成了石头但玩家脱战时依然扔出石头砸向玩家
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;//加一个向上的方向，避免石头直直地向玩家飞过去
        rb.AddForce(direction*force,ForceMode.Impulse);//给石头一个力，模式为冲击力
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockState)//什么时候将石头状态切换到HitNothing
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
                    Instantiate(breakEffect,transform.position,Quaternion.identity);//生成碎石

                    Destroy(gameObject);
                }
                break;
        }
    }
}
