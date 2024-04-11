using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Rock : MonoBehaviour
{   
    public enum RockStates { HitPlayer, HitEnemy,HitNothing}
    //石头状态
    public RockStates rockStats;

  private Rigidbody rb;

    [Header("Basic Settings")]

    public float force;


    public int damage;

    public GameObject target;

    private Vector3 direction;

    public GameObject breakEffect;

    // Start is called before the first frame update

    void Start()
    {
        // 检查Rigidbody组件是否存在
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing on " + gameObject.name);
        }
        else
        {
            Debug.Log("Rigidbody component found on " + gameObject.name);
            // 现在可以安全地使用Rigidbody组件
            // ... 其他代码 ...
        }
        rb.velocity = Vector3.one; // 您的原始代码
        rockStats = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<playerControl>().gameObject;

        direction = (target.transform.position - transform.position + Vector3.up).normalized;

        rb.AddForce(direction * force, ForceMode.Impulse); 

    }
 
    // Update is called once per frame
    void FixeUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStats = RockStates.HitNothing;
        }
        Debug.Log("FixedUpdate called on " + gameObject.name);
    }
    private void OnCollisionEnter(Collision other)
    {
       
        switch (rockStats)
        { 
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {   //player停止移动
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    //击退
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    //播放眩晕动画
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");


                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,
                        other.gameObject.GetComponent<CharacterStats>());


                    rockStats = RockStates.HitNothing;
                
                
                }
                break;


            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                   //获取 CharacterStats的组件
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    //调用CharacterStats的方法
                    otherStats.TakeDamage(damage,otherStats);
                    //石头破碎特效
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    //销毁石头
                    Destroy(gameObject);

                }
                break;

        }
        Debug.Log("Collision detected with " + other.gameObject.name);
    }
}
