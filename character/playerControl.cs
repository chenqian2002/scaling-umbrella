using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class playerControl : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    //设置攻击对象
    private GameObject attackTarget;
    //设置攻击的冷却时间
    private float lastAttackTime;

    //死亡状态判断
    private bool isDeath;


    //判断停止距离
    private float stopDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

     
        characterStats = GetComponent<CharacterStats>();


        stopDistance = agent.stoppingDistance;
    }
    // Start is called before the first frame update

     void OnEnable()
    {
        MouseManger.Instance.OnMouseClicked += MoveToTarget;
        MouseManger.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    void Start()
    {
     
     
        
        SaveManger.Instance.LoadPlayerData();

    }
    void OnDisable()
    {   if (!MouseManger.IsInitialized) return;
        MouseManger.Instance.OnMouseClicked -= MoveToTarget;
        MouseManger.Instance.OnEnemyClicked -= EventAttack;
    }



    // Update is called once per frame
    void Update()
    {   //当前血量是否为0 
        isDeath = characterStats.CurrentHealth == 0;
        if (isDeath)
            GameManager.Instance.NotifyObserver();

        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDeath);
    }
    public void MoveToTarget(Vector3 target)
    {

        StopAllCoroutines();
        if (isDeath) return;
        //日常移动
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;

        agent.destination = target;

    }
    private void EventAttack(GameObject target)
    {
        if (isDeath) return;
        if (target != null)
        {

            attackTarget = target;
            characterStats.isCrititalc = UnityEngine.Random.value < characterStats.attackData.crititalChance;
            StartCoroutine(MoveToAttactTarget());
        }
    }
    //协程
    IEnumerator MoveToAttactTarget()
    {

        agent.isStopped = false;

        agent.stoppingDistance = characterStats.attackData.attackRange;

        //将player转向攻击目标
        transform.LookAt(attackTarget.transform);
        //如果怪物和player的距离超过1
        //FIXME:修改攻击范围参数
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            // player跑
            agent.destination = attackTarget.transform.position;
            //循环
            yield return null;
        }
        agent.isStopped = true;
        //attack
        if (lastAttackTime < 0)
        { //判断暴击
            anim.SetBool("Critical", characterStats.isCrititalc);
            //对目标攻击
            anim.SetTrigger("Attack");
            //TODO:重置冷却时间
            lastAttackTime = characterStats.attackData.collDown;
        }
    }

    /*void Hit()
    {

        if (attackTarget.CompareTag("AttackAble"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStats==Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStats = Rock.RockStates.HitEnemy;
                //冲击力
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
            else
            {

                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
            }
        }
    }*/
    void Hit()
    {
        // 首先检查 attackTarget 是否存在
        if (attackTarget != null)
        {
            // 检查 attackTarget 是否有 Rock 组件，并且其状态为 HitNothing
            if (attackTarget.GetComponent<Rock>() != null && attackTarget.GetComponent<Rock>().rockStats == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStats = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                // 冲击力
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
            else
            {
                // 检查 attackTarget 是否有 CharacterStats 组件
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    targetStats.TakeDamage(characterStats, targetStats);
                }
                else
                {
                    // 如果 targetStats 为 null，记录错误信息
                    Debug.LogError("The attack target does not have a CharacterStats component.");
                }
            }
        }
        else
        {
            // 如果 attackTarget 为 null，记录错误信息
            Debug.LogError("attackTarget is null when trying to call Hit()");
        }
    }
}
