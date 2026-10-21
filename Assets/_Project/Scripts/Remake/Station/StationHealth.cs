using UnityEngine;

public class StationHealth : MonoBehaviour
{
    public static StationHealth Instance;

    [Header("Station Health")]
    [SerializeField] private int maxHealth = 1000;

    [SerializeField] private GameObject shieldVisual;

    [Header("BG Effect")]
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private int currentHealth;
    private bool shieldActive = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (UIStationHealth.Instance != null)
        {
            UIStationHealth.Instance.UpdateHealth(
                currentHealth,
                maxHealth
            );
        }

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Asteroide asteroid =
            other.GetComponentInParent<Asteroide>();

        if (asteroid != null)
        {
            Debug.Log("Asteroide atingiu a estação!");

            TakeDamage(asteroid.StationDamage);

            Destroy(asteroid.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (shieldActive)
        {
            Debug.Log("Escudo bloqueou o dano da Estação!");
            return;
        }

        currentHealth -= damage;

        currentHealth =
            Mathf.Max(currentHealth, 0);

        if (UIStationHealth.Instance != null)
        {
            UIStationHealth.Instance.UpdateHealth(
                currentHealth,
                maxHealth
            );
        }

        Debug.Log(
            "Vida da estação: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        // Aumenta a vida máxima
        maxHealth += amount;

        // Aumenta a vida atual na mesma quantidade
        currentHealth += amount;

        if (UIStationHealth.Instance != null)
        {
            UIStationHealth.Instance.UpdateHealth(
                currentHealth,
                maxHealth
            );
        }

        Debug.Log(
            "Estação recebeu +" +
            amount +
            " de vida!"
        );

        Debug.Log(
            "Vida da estação: " +
            currentHealth +
            " / " +
            maxHealth
        );
    }

    public void ActivateShield(float duration)
    {
        shieldActive = true;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        Debug.Log(
            "Escudo da Estação ativado!"
        );

        CancelInvoke(
            nameof(DeactivateShield)
        );

        Invoke(
            nameof(DeactivateShield),
            duration
        );

        backgroundMaterial.SetColor("_Color_1", color1);
        backgroundMaterial.SetColor("_Color_2", color2);
        backgroundMaterial.SetFloat("_Power_Enable", 1);
    }

    private void DeactivateShield()
    {
        shieldActive = false;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        Debug.Log(
            "Escudo da Estação acabou!"
        );

        backgroundMaterial.SetFloat("_Power_Enable", 0);
    }

    private void Die()
    {
        Debug.Log(
            "A estação espacial foi destruída!"
        );

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }
    }
}