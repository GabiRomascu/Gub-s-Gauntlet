using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarController : MonoBehaviour
{
    public Slider healthSlider;
    public GameObject healthBarUI;

    private void Start()
    {
        // Hide the health bar at the start
        healthBarUI.SetActive(false);
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void ShowHealthBar()
    {
        healthBarUI.SetActive(true);
    }

    public void HideHealthBar()
    {
        healthBarUI.SetActive(false);
    }
}
