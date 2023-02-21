using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
///<summary>
///
///</summary>
public class PlayerHealthUI : MonoBehaviour
{
    
    private TMP_Text levelText;
    private Image healthSlider;
    private Image expSlider;
    private void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<TMP_Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    private void Update()
    {
        levelText.text = "LEVEL   " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");//¸ñÊ½»¯×Ö·û´®
        UpdatePlayerHealth();
        UpdatePlayerExp();
    }

    private void UpdatePlayerHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void UpdatePlayerExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}
