using UnityEngine;
using TMPro;

public class UIStationHealth : MonoBehaviour
{
    public static UIStationHealth Instance;

    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = "ESTAÇÃO: " + currentHealth + " / " + maxHealth;
    }
}