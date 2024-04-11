using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealthUI : MonoBehaviour
{
    //等级
    Text levelText;
    //生命值
    Image healthSilder;
    //经验值
    Image expSilder;


     void Update()
    {
        levelText.text = "Level  " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }
    void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();

        healthSilder = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        expSilder = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

     void UpdateHealth()
    {
        float silderPercent =(float) GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;

        healthSilder.fillAmount = silderPercent;
    }

    void UpdateExp()
    {

        float silderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;

        expSilder.fillAmount = silderPercent;
    }
}
