using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using System;

///<summary>
///
///</summary>

//[System.Serializable]//���л���û�й���monobehavior��EventVector3��ʾ������� 
//public class EventVector3 : UnityEvent<Vector3> { } //������ק��ʽ��ʹ��.Net�Լ���event
public class MouseManager : Singleton<MouseManager>
{
    //public static MouseManager Instance;//����ģʽ

    //public EventVector3 OnMouseClicked;//������¼�

    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;//����Physics.Raycast����ײ��Ϣ
    //�¼� vector3�ǽ��յĲ�������,MouseManager���յ���vector3�����ᴫ��ע�ᵽ���¼��еķ�����
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyCliked;
    //void Awake()
    //{
    //    if (instance != null)
    //        Destroy(gameObject);
    //    instance = this;
    //}
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()//�޸����ָ��
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//ray��Ҫʵʱ�޸�
        if (Physics.Raycast(ray, out hitInfo))
        {
            //�޸����ָ��
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//�ڶ�������Ϊƫ��ֵ����Ϊ���һ��ֻ������һ�������ж�������Ҫ��������ƶ���ͼƬ�м�
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;

            }
        }
    }
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)//�ж��Ƿ�������Ч����
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //�����ô˺���ʱ������ע��������¼��ķ������ᱻ���� ?��ʾOnMouseClicked�Ƿ�Ϊ��
                OnMouseClicked?.Invoke(hitInfo.point);//OnMouseClicked�¼���Ϊ��ʱִ��
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                //�����ô˺���ʱ������ע��������¼��ķ������ᱻ����
                OnEnemyCliked?.Invoke(hitInfo.collider.gameObject);//�������������gameObject����ȥ
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyCliked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}

