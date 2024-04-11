using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Rock : MonoBehaviour
{   
    public enum RockStates { HitPlayer, HitEnemy,HitNothing}
    //ʯͷ״̬
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
        // ���Rigidbody����Ƿ����
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing on " + gameObject.name);
        }
        else
        {
            Debug.Log("Rigidbody component found on " + gameObject.name);
            // ���ڿ��԰�ȫ��ʹ��Rigidbody���
            // ... �������� ...
        }
        rb.velocity = Vector3.one; // ����ԭʼ����
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
                {   //playerֹͣ�ƶ�
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    //����
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    //����ѣ�ζ���
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");


                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,
                        other.gameObject.GetComponent<CharacterStats>());


                    rockStats = RockStates.HitNothing;
                
                
                }
                break;


            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                   //��ȡ CharacterStats�����
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    //����CharacterStats�ķ���
                    otherStats.TakeDamage(damage,otherStats);
                    //ʯͷ������Ч
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    //����ʯͷ
                    Destroy(gameObject);

                }
                break;

        }
        Debug.Log("Collision detected with " + other.gameObject.name);
    }
}
