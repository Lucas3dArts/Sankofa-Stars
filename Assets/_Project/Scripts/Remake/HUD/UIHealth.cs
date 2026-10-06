using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour
{
    public static UIHealth Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "Vida: " + currentHealth;
    }
}