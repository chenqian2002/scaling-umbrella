using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack", menuName = "Attcak /Attack Data")]
public class AttackData_So : ScriptableObject
{
    //�����빥��
    public float attackRange;
    //Զ�̹���
    public float skillkRange;
    //cdʱ��
    public float collDown;
    //��С����
    public int minDamge;
    //��󹥻�  
    public int maxDamge;
    //�����ӳ�
    public float criticalMultiplier;
    //������
    
    public float crititalChance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
