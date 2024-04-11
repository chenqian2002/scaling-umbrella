using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class Enemycontrol1 : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyStates enemyStates;

    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    //怪物可视范围
    public float sightRadius;
    //生成敌人攻击目标
    protected GameObject attackTarget;
    // Start is called before the first frame update
    //是否追击状态
    public bool isGuard;
    //设置怪物的速度
    private float speed;
    //
    private Animator anim;

    //获取碰撞体控制权
    private Collider coll;

    public float lookAtTime;
    public float remainlookAtTime;
    private float lastAttackTime;
    //怪物回起始点的朝向
    private Quaternion guradRotation;

    [Header("Basic Settings")]
    public float patrolRange;
    private Vector3 wayPoint;
    //获取原始enemy的坐标
    private Vector3 guardPos;

    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guradRotation = transform.rotation;
     
        remainlookAtTime = lookAtTime;
         }
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME:场景切换时修改掉
        GameManager.Instance.AddObserver(this);

    }

    //启用

    /*void OnEnable()
    {
      
    }*/
    //禁用
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if (!playerDead) { 
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
        }
    }
    //设置动画播放
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCrititalc);
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()
    {   //
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        
        //追击player
        //拉托回上一个状态
        //配合动画
        else if (FoundPlayer()) 
        {
            enemyStates = EnemyStates.CHASE;
           
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    //如果guardPos的坐标-目标坐标小于agant的停止目标
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guradRotation,0.01f); 
                    }
                }
                break;
            case EnemyStates.PATROL:


                isChase = false;
                agent.speed = speed * 0.5f;
                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint,transform.position)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainlookAtTime > 0)
                    
                        remainlookAtTime -= Time.deltaTime;
                    
                        
                    GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }


                break;
            case EnemyStates.CHASE:
                agent.speed = speed;
                //配合动画
                isWalk = false;
                isChase = true;
                //拉托回上一个状态
                if (!FoundPlayer())
                {
                    
                    isFollow = false;
                    if (remainlookAtTime > 0) { 
                        agent.destination = transform.position;
                        remainlookAtTime -= Time.deltaTime;
                    }else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;

                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position; 
                }
                //在攻击范围内则攻击
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    //如果最后攻击时间小于0
                    if (lastAttackTime < 0)
                    {  //攻击
                        lastAttackTime = characterStats.attackData.collDown;
                        //暴击判断
                        characterStats.isCrititalc = Random.value < characterStats.attackData.crititalChance;
                       //攻击判断   
                        Attack();
                    }

                }
                break;
            case EnemyStates.DEAD:
                //代码有顺序
                coll.enabled = false;
                /* agent.enabled = false;*/
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");

        }
        if (TargetInSkillRange())
        {
            //技能动画
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()
    {
        var colliders  = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {   //如果怪物找到player
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {

        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillkRange;
        else
            return false;
    }


    void GetNewWayPoint()
    {
        remainlookAtTime = lookAtTime;
        //生成随机的x，z轴值
        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //生成随机的三维坐标
        Vector3 randomPoint = new Vector3(guardPos.x + randomX,
             transform.position.y + guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
       
    }

     void OnDrawGizmosSelected()
    {   //用蓝色的线画区域
        Gizmos.color = Color.blue;
        //在当前位置画可见视野的区域画
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
     //Animation Event
     void Hit()
    {
        if (attackTarget != null  && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
      
    }

    public void EndNotify()
    {
        //获胜动画
        anim.SetBool("Win", true);
        playerDead = true;
        //停止所有移动
        //停止Agent
        isWalk = false;
        isChase = false;
   
       
        attackTarget = null;
    }
}
