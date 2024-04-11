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
    //������ӷ�Χ
    public float sightRadius;
    //���ɵ��˹���Ŀ��
    protected GameObject attackTarget;
    // Start is called before the first frame update
    //�Ƿ�׷��״̬
    public bool isGuard;
    //���ù�����ٶ�
    private float speed;
    //
    private Animator anim;

    //��ȡ��ײ�����Ȩ
    private Collider coll;

    public float lookAtTime;
    public float remainlookAtTime;
    private float lastAttackTime;
    //�������ʼ��ĳ���
    private Quaternion guradRotation;

    [Header("Basic Settings")]
    public float patrolRange;
    private Vector3 wayPoint;
    //��ȡԭʼenemy������
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
        //FIXME:�����л�ʱ�޸ĵ�
        GameManager.Instance.AddObserver(this);

    }

    //����

    /*void OnEnable()
    {
      
    }*/
    //����
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
    //���ö�������
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
        
        //׷��player
        //���л���һ��״̬
        //��϶���
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
                    //���guardPos������-Ŀ������С��agant��ֹͣĿ��
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
                //�ж��Ƿ������Ѳ�ߵ�
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
                //��϶���
                isWalk = false;
                isChase = true;
                //���л���һ��״̬
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
                //�ڹ�����Χ���򹥻�
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    //�����󹥻�ʱ��С��0
                    if (lastAttackTime < 0)
                    {  //����
                        lastAttackTime = characterStats.attackData.collDown;
                        //�����ж�
                        characterStats.isCrititalc = Random.value < characterStats.attackData.crititalChance;
                       //�����ж�   
                        Attack();
                    }

                }
                break;
            case EnemyStates.DEAD:
                //������˳��
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
            //����������
            anim.SetTrigger("Attack");

        }
        if (TargetInSkillRange())
        {
            //���ܶ���
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()
    {
        var colliders  = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {   //��������ҵ�player
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
        //���������x��z��ֵ
        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //�����������ά����
        Vector3 randomPoint = new Vector3(guardPos.x + randomX,
             transform.position.y + guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
       
    }

     void OnDrawGizmosSelected()
    {   //����ɫ���߻�����
        Gizmos.color = Color.blue;
        //�ڵ�ǰλ�û��ɼ���Ұ������
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
        //��ʤ����
        anim.SetBool("Win", true);
        playerDead = true;
        //ֹͣ�����ƶ�
        //ֹͣAgent
        isWalk = false;
        isChase = false;
   
       
        attackTarget = null;
    }
}
