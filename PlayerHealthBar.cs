using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI lifePercentage;

    [Header("Damage Overlay")]
    public Image overlay; // our damage overlay game object
    public float duration; // how long the image stays fully opaque
    public float fadeSpeed; // how quickly the image will fade

    private float durationTimer; // timer to check against the duration

    [Header("XP Bar")]
    public Image frontXPBar;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI xpText;
    public int maxXPOne = 500;
    public int maxXPTwo = 100000;
    public int xp;
    bool level2 = false;


    // Start is called before the first frame update
    void Start()
    {
        // Check if UserScore exists in PlayerPrefs
        if (PlayerPrefs.HasKey("UserScore"))
        {
            // Load the UserScore from PlayerPrefs
            xp = PlayerPrefs.GetInt("UserScore");
        }
        else
        {
            // UserScore not found, set it to 0
            xp = 0;
        }

        if (xp >= maxXPOne)
        {
            level2 = true;
        }

        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        xp = Mathf.Clamp(xp, 0, maxXPTwo);

        if (xp >= maxXPOne)
        {
            level2 = true;
        }
        else
        {
            level2 = false;
        }

        UpdateHealthUI();
        UpdateXPUI();

        if (overlay.color.a > 0)
        {
            if (health < 30)
                return;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                // fade the image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        // Take damage
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        // Restore health
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }

        // Update life percentage
        float lifePercentageValue = (health / maxHealth) * 100;
        lifePercentage.text = Mathf.RoundToInt(lifePercentageValue) + "%";
    }

    public void UpdateXPUI()
    {
        int currentLevel = level2 ? 2 : 1;
        Level.text = "Level " + currentLevel;
        xpText.text = xp + " XP";

        float fillXP = frontXPBar.fillAmount;
        float xpFraction = (float)xp / maxXPOne;

        frontXPBar.fillAmount = xpFraction;

        if (xp >= maxXPOne && !level2)
        {
            // Level up to 2
            LevelUp();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    public void AddXP(int XPAmount)
    {
        xp += XPAmount;
        lerpTimer = 0f;
    }

    public void SubtractXP(int XPAmount)
    {
        xp -= XPAmount;
        lerpTimer = 0f;
    }

    private void LevelUp()
    {
        // Level up logic here
        level2 = true;
        maxXPOne = maxXPTwo;
        xp = maxXPOne;
    }
}
