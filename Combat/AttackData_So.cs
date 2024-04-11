using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack", menuName = "Attcak /Attack Data")]
public class AttackData_So : ScriptableObject
{
    //近距离攻击
    public float attackRange;
    //远程攻击
    public float skillkRange;
    //cd时间
    public float collDown;
    //最小攻击
    public int minDamge;
    //最大攻击  
    public int maxDamge;
    //暴击加成
    public float criticalMultiplier;
    //暴击率
    
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
