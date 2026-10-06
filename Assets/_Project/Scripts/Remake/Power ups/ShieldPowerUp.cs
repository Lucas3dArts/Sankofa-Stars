using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 10f;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        // Player encostou no poder
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            ActivateShield();
            return;
        }

        // Tiro do Player atingiu o poder
        Projectile projectile =
            other.GetComponentInParent<Projectile>();

        if (projectile != null)
        {
            ActivateShield();
            Destroy(projectile.gameObject);
        }
    }

    private void ActivateShield()
    {
        if (activated)
            return;

        activated = true;

        Debug.Log(
            "ESCUDO ATIVADO por " +
            shieldDuration +
            " segundos!"
        );

        PlayerHealth player =
            FindObjectOfType<PlayerHealth>();

        if (player != null)
            player.ActivateShield(shieldDuration);

        StationHealth station =
            FindObjectOfType<StationHealth>();

        if (station != null)
            station.ActivateShield(shieldDuration);

        // Mostra o nome e a duração na HUD
        if (PowerUpDurationUI.Instance != null)
        {
            PowerUpDurationUI.Instance.StartPowerUp(
                "Escudo de Obaluaê",
                shieldDuration
            );
        }

        Destroy(gameObject);
    }
}