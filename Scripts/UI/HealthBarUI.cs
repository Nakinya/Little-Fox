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
    public Transform barPoint;//�����UI����λ��
    public bool alwaysVisible;//Ѫ���Ƿ��ǳ��ÿɼ���
    public float visibleTime;//�ɼ�ʱ��
    private float timeLeft;

    Image healthSlider;//��ʵѪ��
    Transform UIBar;//Ѫ������ʱ�����λ�ã��������λ�ñ�ΪbarPoint��λ��
    Transform cam;//�������λ�ã�Ѫ������תΪ�������ǰ���ķ�������Ѫ��һֱ���������

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
                UIBar = Instantiate(HealthUIPrefab, canvas.transform).transform;//һ��Instantiate�����أ�����Ϊcanvas������
                healthSlider = UIBar.transform.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(alwaysVisible);//���õ�ʱ���ж�Ѫ���Ƿ��ǳ��ÿɼ���
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (UIBar != null)
        {
            if (currentHealth <= 0)
                Destroy(UIBar.gameObject);

            UIBar.gameObject.SetActive(true);//ÿ�θ��µ�ʱ��Ҫ����Ϊ�ɼ�
            timeLeft = visibleTime;

            float sliderPercent = (float)currentHealth / maxHealth;
            healthSlider.fillAmount = sliderPercent;
        }
    }

    private void LateUpdate()//���涼Ӧ����LateUpate�У��������λ�ñ�����˸
    {
        if (UIBar != null)
        {
            UIBar.position = barPoint.position;//�������
            UIBar.forward = -cam.forward;//Ѫ�����������

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
