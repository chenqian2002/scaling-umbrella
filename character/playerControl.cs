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
    //���ù�������
    private GameObject attackTarget;
    //���ù�������ȴʱ��
    private float lastAttackTime;

    //����״̬�ж�
    private bool isDeath;


    //�ж�ֹͣ����
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
    {   //��ǰѪ���Ƿ�Ϊ0 
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
        //�ճ��ƶ�
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
    //Э��
    IEnumerator MoveToAttactTarget()
    {

        agent.isStopped = false;

        agent.stoppingDistance = characterStats.attackData.attackRange;

        //��playerת�򹥻�Ŀ��
        transform.LookAt(attackTarget.transform);
        //��������player�ľ��볬��1
        //FIXME:�޸Ĺ�����Χ����
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            // player��
            agent.destination = attackTarget.transform.position;
            //ѭ��
            yield return null;
        }
        agent.isStopped = true;
        //attack
        if (lastAttackTime < 0)
        { //�жϱ���
            anim.SetBool("Critical", characterStats.isCrititalc);
            //��Ŀ�깥��
            anim.SetTrigger("Attack");
            //TODO:������ȴʱ��
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
                //�����
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
        // ���ȼ�� attackTarget �Ƿ����
        if (attackTarget != null)
        {
            // ��� attackTarget �Ƿ��� Rock �����������״̬Ϊ HitNothing
            if (attackTarget.GetComponent<Rock>() != null && attackTarget.GetComponent<Rock>().rockStats == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStats = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                // �����
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
            else
            {
                // ��� attackTarget �Ƿ��� CharacterStats ���
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    targetStats.TakeDamage(characterStats, targetStats);
                }
                else
                {
                    // ��� targetStats Ϊ null����¼������Ϣ
                    Debug.LogError("The attack target does not have a CharacterStats component.");
                }
            }
        }
        else
        {
            // ��� attackTarget Ϊ null����¼������Ϣ
            Debug.LogError("attackTarget is null when trying to call Hit()");
        }
    }
}
