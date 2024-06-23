using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CooldownManager : MonoBehaviour
{
    public Image potionCooldownImage;
    public TMP_Text potionCooldownText;

    public Image hammerCooldownImage;
    public TMP_Text hammerCooldownText;

    public Image amuletCooldownImage;
    public TMP_Text amuletCooldownText;

    private float potionDuration = 5f; // Example duration
    private float potionCooldownDuration = 10f;
    private float hammerDuration = 5f; // Example duration
    private float hammerCooldownDuration = 5f;
    private float amuletDuration = 10f; // Example duration
    private float amuletCooldownDuration = 20f;

    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Darker color
    private Color activeColor = Color.white; // Active color (fully colored)

    // Track cooldown states
    private bool isPotionOnCooldown = false;
    private bool isHammerOnCooldown = false;
    private bool isAmuletOnCooldown = false;

    void Start()
    {
        ResetCooldownUI(potionCooldownImage, potionCooldownText);
        ResetCooldownUI(hammerCooldownImage, hammerCooldownText);
        ResetCooldownUI(amuletCooldownImage, amuletCooldownText);
    }

    void ResetCooldownUI(Image cooldownImage, TMP_Text cooldownText)
    {
        cooldownImage.fillAmount = 0;
        cooldownImage.color = inactiveColor;
        cooldownText.text = "";
    }

    public bool IsPotionOnCooldown()
    {
        return isPotionOnCooldown;
    }

    public bool IsHammerOnCooldown()
    {
        return isHammerOnCooldown;
    }

    public bool IsAmuletOnCooldown()
    {
        return isAmuletOnCooldown;
    }

    public void StartPotionDuration()
    {
        if (!isPotionOnCooldown)
        {
            isPotionOnCooldown = true;
            StartDuration(potionCooldownImage, potionCooldownText, potionDuration, potionCooldownDuration, () => isPotionOnCooldown = false);
        }
    }

    public void StartHammerDuration()
    {
        if (!isHammerOnCooldown)
        {
            isHammerOnCooldown = true;
            StartDuration(hammerCooldownImage, hammerCooldownText, hammerDuration, hammerCooldownDuration, () => isHammerOnCooldown = false);
        }
    }

    public void StartAmuletDuration()
    {
        if (!isAmuletOnCooldown)
        {
            isAmuletOnCooldown = true;
            StartDuration(amuletCooldownImage, amuletCooldownText, amuletDuration, amuletCooldownDuration, () => isAmuletOnCooldown = false);
        }
    }

    private void StartDuration(Image cooldownImage, TMP_Text cooldownText, float duration, float cooldownDuration, System.Action onCooldownEnd)
    {
        cooldownImage.color = activeColor; // Set to active color when duration starts
        StartCoroutine(DurationCoroutine(cooldownImage, cooldownText, duration, cooldownDuration, onCooldownEnd));
    }

    private IEnumerator DurationCoroutine(Image cooldownImage, TMP_Text cooldownText, float duration, float cooldownDuration, System.Action onCooldownEnd)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            cooldownImage.fillAmount = timeRemaining / duration;
            cooldownText.text = Mathf.Ceil(timeRemaining).ToString();
            yield return null;
        }

        StartCooldown(cooldownImage, cooldownText, cooldownDuration, onCooldownEnd);
    }

    private void StartCooldown(Image cooldownImage, TMP_Text cooldownText, float cooldownDuration, System.Action onCooldownEnd)
    {
        cooldownImage.color = inactiveColor; // Darken image when cooldown starts
        StartCoroutine(CooldownCoroutine(cooldownImage, cooldownText, cooldownDuration, onCooldownEnd));
    }

    private IEnumerator CooldownCoroutine(Image cooldownImage, TMP_Text cooldownText, float cooldownDuration, System.Action onCooldownEnd)
    {
        float timeRemaining = cooldownDuration;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            cooldownImage.fillAmount = timeRemaining / cooldownDuration;
            cooldownText.text = Mathf.Ceil(timeRemaining).ToString();
            yield return null;
        }

        ResetCooldownUI(cooldownImage, cooldownText);
        onCooldownEnd?.Invoke();
    }
}
