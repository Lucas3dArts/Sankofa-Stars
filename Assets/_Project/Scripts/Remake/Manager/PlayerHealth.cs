using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private bool shieldActive = false;
    private bool lightningProtectionActive = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (UIHealth.Instance != null)
        {
            UIHealth.Instance.UpdateHealth(currentHealth);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (lightningProtectionActive)
        {
            Asteroide protectedAsteroid =
                other.GetComponentInParent<Asteroide>();

            if (protectedAsteroid != null)
            {
                Debug.Log(
                    "Tempestade de Iansã protegeu o Player!"
                );

                return;
            }
        }

        Asteroide asteroid =
            other.GetComponentInParent<Asteroide>();

        if (asteroid != null)
        {
            Debug.Log(
                "Player atingido! Dano: " +
                asteroid.Damage
            );

            TakeDamage(asteroid.Damage);

            Destroy(asteroid.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (shieldActive)
        {
            Debug.Log("Escudo bloqueou o dano do Player!");

            return;
        }

        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0);

        if (UIHealth.Instance != null)
        {
            UIHealth.Instance.UpdateHealth(currentHealth);
        }

        Debug.Log(
            "Vida do Player: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (UIHealth.Instance != null)
        {
            UIHealth.Instance.UpdateHealth(currentHealth);
        }

        Debug.Log(
            "Player ganhou vida! Vida atual: " +
            currentHealth
        );
    }

    public void ActivateShield(float duration)
    {
        shieldActive = true;

        Debug.Log("Escudo do Player ativado!");

        CancelInvoke(nameof(DeactivateShield));

        Invoke(
            nameof(DeactivateShield),
            duration
        );
    }

    private void DeactivateShield()
    {
        shieldActive = false;

        Debug.Log("Escudo do Player acabou!");
    }

    private void Die()
    {
        Debug.Log("Player morreu!");

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }

        gameObject.SetActive(false);
    }

    public void ActivateLightningProtection(float duration)
    {
        lightningProtectionActive = true;

        CancelInvoke(nameof(DeactivateLightningProtection));

        Invoke(
            nameof(DeactivateLightningProtection),
            duration
        );

        Debug.Log(
            "⚡ Proteção de Iansã ativada!"
        );
    }

    private void DeactivateLightningProtection()
    {
        lightningProtectionActive = false;

        Debug.Log(
            "⚡ Proteção de Iansã terminou!"
        );
    }
}