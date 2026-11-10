using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 10f;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        // Player encostou no poder.
        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            ActivateShield();
            return;
        }

        // Tiro do Player atingiu o poder.
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
        {
            player.ActivateShield(shieldDuration);
        }

        StationHealth station =
            FindObjectOfType<StationHealth>();

        if (station != null)
        {
            station.ActivateShield(shieldDuration);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUpSound();
        }

        ActivateShieldLiquid();

        Destroy(gameObject);
    }

    private void ActivateShieldLiquid()
    {
        // Procura especificamente o objeto ativo da cena.
        GameObject liquidObject =
            GameObject.Find("Liquido_Shield");

        if (liquidObject == null)
        {
            Debug.LogError(
                "ShieldPowerUp: não foi encontrado um objeto ativo " +
                "chamado 'Liquido_Shield' na cena.",
                this
            );

            return;
        }

        PowerLiquidFill liquidFill =
            liquidObject.GetComponent<PowerLiquidFill>();

        if (liquidFill == null)
        {
            Debug.LogError(
                "ShieldPowerUp: o objeto 'Liquido_Shield' não possui " +
                "o componente PowerLiquidFill.",
                liquidObject
            );

            return;
        }

        Debug.Log(
            "Líquido encontrado na cena: " +
            GetFullPath(liquidObject.transform) +
            " | Cena: " +
            liquidObject.scene.name +
            " | activeSelf: " +
            liquidObject.activeSelf +
            " | activeInHierarchy: " +
            liquidObject.activeInHierarchy,
            liquidObject
        );

        liquidFill.CollectPower();
        liquidFill.ActivatePower(shieldDuration);
    }

    private string GetFullPath(Transform target)
    {
        string path = target.name;

        while (target.parent != null)
        {
            target = target.parent;
            path = target.name + "/" + path;
        }

        return path;
    }
}