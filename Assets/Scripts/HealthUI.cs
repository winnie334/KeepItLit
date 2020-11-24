using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(float maxHealth) {
        slider.maxValue = maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health) {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}

