using UnityEngine;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int startingHealth = 50;
    [SerializeField] private TextMeshProUGUI healthText;

    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = Mathf.Clamp(startingHealth, 0, maxHealth);
        UpdateHealthText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if(currentHealth  >  maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthText();
    }

    public void UpdateHealthText()
    {
        healthText.text = "HP" + currentHealth + "/" + maxHealth;
    }

}
