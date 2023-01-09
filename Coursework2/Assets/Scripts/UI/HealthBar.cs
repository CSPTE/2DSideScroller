using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Vector3 offset = new Vector3(0, 4, 0);


    public void SetHealth(int health) {
        slider.value = health;
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos + offset;
    }

    public void SetMaxHealth(int health) {
        slider.maxValue = health;
        slider.value = slider.maxValue;
    }

    public void Destroy() {
        Destroy(gameObject);
    }
    
}
