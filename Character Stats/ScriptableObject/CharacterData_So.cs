using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="New Data",menuName ="Character State/Data")]
public class CharacterData_So : ScriptableObject
{
    [Header("Stats Info")]

    public int maxHealth;
    //��ǰѪ��
    public int currentHealth;
    public int baseDefence;
    //��ǰ����
    public int curentDefence;

    [Header("Kill")]

    public int killPoint;

    // Start is called before the first frame
    // Start is called before the first frame update
    [Header("Level")]

    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;

    public float levelBuff;

    public float LevelMultiplier
    {
        get{ return 1 + (currentLevel - 1) * levelBuff; }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    public  void UpdateExp(int point)
    {
          currentExp+=point;
        if (currentExp >= baseExp)
            LeveluUp();
    }

    private void LeveluUp()
    {
        //�жϵȼ������Ƿ�ﵽ���ֵ
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp +=(int) (baseExp * LevelMultiplier);
        maxHealth= (int)(maxHealth*LevelMultiplier);
    }
}
