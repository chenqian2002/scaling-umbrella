using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPerfab;
    // Start is called before the first frame update
    public Transform barPoint;

    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;

    Image healthSlider;

    Transform UIbar;

    Transform cam;

    CharacterStats currentStats;

     void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //��Ѫ�������������ʾ
                UIbar = Instantiate(healthUIPerfab, canvas.transform).transform;
                //��ȡѪ������������ɫѪ��
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                //����Ѫ��һ��ʼ�Ƿ�ɼ�
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
        {

        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);
         
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

    }
    private void LateUpdate()
    {
        if(UIbar !=null)
        {
            UIbar.position = barPoint.position; 
            UIbar.forward = -cam.forward; 

            if(timeLeft<=0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;

        }
    }
}
