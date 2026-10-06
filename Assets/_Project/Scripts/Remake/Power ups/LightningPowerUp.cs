using UnityEngine;

public class LightningPowerUp : MonoBehaviour
{
    [Header("Lightning Settings")]
    [SerializeField] private float duration = 8f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float range = 80f;
    [SerializeField] private float damage = 100f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Player entrou no Power-Up
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            Debug.Log(
                "Player coletou o Power-Up de Iansã!"
            );

            CollectPowerUp();
            return;
        }

        // Tiro atingiu o Power-Up
        Projectile projectile =
            other.GetComponentInParent<Projectile>();

        if (projectile != null)
        {
            Debug.Log(
                "Tiro atingiu o Power-Up de Iansã!"
            );

            CollectPowerUp();

            Destroy(projectile.gameObject);
            return;
        }
    }

    private void CollectPowerUp()
    {
        if (collected)
            return;

        collected = true;

        LightningAttack lightning =
            FindObjectOfType<LightningAttack>();

        if (lightning != null)
        {
            lightning.StorePowerUp(
                duration,
                fireRate,
                range,
                damage
            );

            Debug.Log(
                "⚡ PODER DE IANSÃ COLETADO!"
            );
        }
        else
        {
            Debug.LogError(
                "LightningPowerUp: " +
                "LightningAttack não encontrado no Player!"
            );
        }

        Destroy(gameObject);
    }
}