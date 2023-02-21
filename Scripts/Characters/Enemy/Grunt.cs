using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///<summary>
///
///</summary>
public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10f;
    public void KickOff()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform.position);

            Vector3 direction = attackTarget.transform.position - this.transform.position;
            direction.Normalize();//���ɷ���

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction*kickForce;//һ��ʹ�ø����һ����s
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
