using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem :Enemycontrol1
{
    private Rigidbody rb;
    [Header("Skill")]
    public float kickForce = 25;

    //获取石头
    public GameObject rockPerfab;
    //获取手的坐标
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

            //设置方向=攻击坐标的方向值-当前坐标方向值
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //打断移动
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            //击退=方向*距离
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //使目标眩晕
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
