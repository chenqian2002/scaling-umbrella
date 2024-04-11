using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_So templateData;

    public CharacterData_So characterData;

    public AttackData_So attackData;

    [HideInInspector]
    public bool isCrititalc;
    #region Read from Data_SO

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int CurentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.curentDefence;
            else return 0;
        }
        set
        {
            characterData.curentDefence = value;
        }
    }
    #endregion


    #region
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        
        
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCrititalc)
        {   
            //被攻击者的受伤动画
            defener.GetComponent<Animator>().SetTrigger("Hit");

        }
        //TODO:UPDATA UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);




        //经验updata
        //如果enemy的生命为0，将自己的经验加给击杀者
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);

    }


    public void TakeDamage(int damage, CharacterStats defener)
    {
        int curremtDamage = Math.Max(damage - defener.CurentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - curremtDamage);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint); 

    }




    public int CurrentDamage()
    {
        float corDamage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamge);
        if (isCrititalc)
        {   //如果暴击了 基础伤害*暴击伤害
            corDamage *= attackData.criticalMultiplier; 
            Debug.Log("暴击" + corDamage);
        }
        return (int)corDamage;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
