using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem :Enemycontrol1
{
    private Rigidbody rb;
    [Header("Skill")]
    public float kickForce = 25;

    //��ȡʯͷ
    public GameObject rockPerfab;
    //��ȡ�ֵ�����
    public Transform handPos;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
 
    }
    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            //���÷���=��������ķ���ֵ-��ǰ���귽��ֵ
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //����ƶ�
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            //����=����*����
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //ʹĿ��ѣ��
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStats, targetStats); 
        }
    }




    //Animation Event

    public void ThrowRock()
    {
        if(attackTarget != null)
        {   
            var rock = Instantiate(rockPerfab, handPos.position, Quaternion.identity);
            rock = GetComponent<Rock>().target = attackTarget;
        }
    }

}
