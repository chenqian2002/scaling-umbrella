 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using UnityEngine.Events;*/
using System;

/*[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }*/
public class MouseManger : Singleton<MouseManger>
{   //����ģʽ

    //�����ģʽ
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    //������ײ
    RaycastHit hitinfo;
    //���ָ��
    public Texture2D point;



    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();

        OnMouseControl();
    }

    
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hitinfo))
        {
            //�л������ͼ 
            switch (hitinfo.collider.gameObject.tag)
            {
            /*    case "Group":
                    Cursor.SetCursor(point,new Vector2(16,16),CursorMode.Auto);
                    break;*/
                case "enemy":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }

    }
    void OnMouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null)
        { 

            if (hitinfo.collider.gameObject.CompareTag("Group")) 
            OnMouseClicked?.Invoke(hitinfo.point);
            if (hitinfo.collider.gameObject.CompareTag("enemy")) 
            OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);

            if (hitinfo.collider.gameObject.CompareTag("AttackAble"))
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);

            if (hitinfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitinfo.point);
        }
    }
}


