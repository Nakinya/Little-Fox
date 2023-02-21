using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
using System;

///<summary>
///
///</summary>

//[System.Serializable]//序列化将没有挂载monobehavior的EventVector3显示在面板中 
//public class EventVector3 : UnityEvent<Vector3> { } //舍弃拖拽方式后，使用.Net自己的event
public class MouseManager : Singleton<MouseManager>
{
    //public static MouseManager Instance;//单例模式

    //public EventVector3 OnMouseClicked;//鼠标点击事件

    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;//接收Physics.Raycast的碰撞信息
    //事件 vector3是接收的参数类型,MouseManager就收到的vector3参数会传入注册到此事件中的方法中
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
    void SetCursorTexture()//修改鼠标指针
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//ray需要实时修改
        if (Physics.Raycast(ray, out hitInfo))
        {
            //修改鼠标指针
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//第二个参数为偏移值，因为鼠标一般只有左上一个点有判定，我们要把这个点移动到图片中间
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
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)//判断是否点击到有效物体
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //当启用此函数时，所有注册了这个事件的方法都会被调用 ?表示OnMouseClicked是否为空
                OnMouseClicked?.Invoke(hitInfo.point);//OnMouseClicked事件不为空时执行
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                //当启用此函数时，所有注册了这个事件的方法都会被调用
                OnEnemyCliked?.Invoke(hitInfo.collider.gameObject);//将被攻击对象的gameObject传过去
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

