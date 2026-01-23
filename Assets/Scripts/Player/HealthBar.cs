using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHealth(float health)
    {  
        slider.maxValue = health; 
        slider.value = health;
    }

    public void SetHealth(float health)
    {  
        slider.value = health; 
    }

    public void SetMaxShield(float shield)
    {
        slider.maxValue = shield;
        slider.value = shield;
    }

    public void SetShield(float shield)
    {
        slider.value = shield;
    }
}

