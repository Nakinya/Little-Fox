using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
///<summary>
///
///</summary>
public class HealthBarUI : MonoBehaviour
{
    public GameObject HealthUIPrefab;
    public Transform barPoint;//物体的UI理想位置
    public bool alwaysVisible;//血条是否是长久可见的
    public float visibleTime;//可见时间
    private float timeLeft;

    Image healthSlider;//真实血条
    Transform UIBar;//血条生成时自身的位置，想让这个位置变为barPoint的位置
    Transform cam;//摄像机的位置，血条的旋转为摄像机正前方的反方向，让血条一直面向摄像机

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
        visibleTime = 3f;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.CompareTag("HealthBar"))
            {
                UIBar = Instantiate(HealthUIPrefab, canvas.transform).transform;//一个Instantiate的重载，生成为canvas的子类
                healthSlider = UIBar.transform.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(alwaysVisible);//启用的时候判断血条是否是长久可见的
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (UIBar != null)
        {
            if (currentHealth <= 0)
                Destroy(UIBar.gameObject);

            UIBar.gameObject.SetActive(true);//每次更新的时候要设置为可见
            timeLeft = visibleTime;

            float sliderPercent = (float)currentHealth / maxHealth;
            healthSlider.fillAmount = sliderPercent;
        }
    }

    private void LateUpdate()//跟随都应该在LateUpate中，获得最新位置避免闪烁
    {
        if (UIBar != null)
        {
            UIBar.position = barPoint.position;//跟随敌人
            UIBar.forward = -cam.forward;//血条面向摄像机

            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIBar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
