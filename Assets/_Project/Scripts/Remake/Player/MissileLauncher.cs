using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileLauncher : MonoBehaviour
{
    [Header("Missile")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float missileSearchRange = 100f;

    [Header("Input")]
    [SerializeField] private InputActionReference powerUpAction;

    [Header("BG Effect")]
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private bool hasPowerUp = false;
    private bool active = false;

    private float duration;
    private float fireRate;

    private GameObject missilePrefab;

    public void StorePowerUp(
        float newDuration,
        float newFireRate,
        GameObject newMissilePrefab
    )
    {
        duration = newDuration;
        fireRate = newFireRate;
        missilePrefab = newMissilePrefab;

        hasPowerUp = true;

        Debug.Log(
            "POWER-UP DE MÍSSEIS COLETADO!"
        );

        Debug.Log(
            "Pressione BOTÃO DIREITO do mouse para ativar."
        );
    }

    private void Update()
    {
        if (!hasPowerUp || active)
            return;

        if (powerUpAction != null &&
            powerUpAction.action.WasPressedThisFrame())
        {
            ActivateMissiles();
            backgroundMaterial.SetColor("_Color_1", color1);
            backgroundMaterial.SetColor("_Color_2", color2);
            backgroundMaterial.SetFloat("_Power_Enable", 1);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPowerUpSound();
            }
        }

    }

    private void OnEnable()
    {
        if (powerUpAction != null)
            powerUpAction.action.Enable();
    }

    private void OnDisable()
    {
        if (powerUpAction != null)
            powerUpAction.action.Disable();
    }

    private void ActivateMissiles()
    {
        hasPowerUp = false;
        active = true;

        Debug.Log(
            "=== MODO MÍSSIL ATIVADO ==="
        );

        if (PowerUpDurationUI.Instance != null)
        {
            PowerUpDurationUI.Instance.StartPowerUp(
                "MÍSSEIS",
                duration
            );
        }

        StartCoroutine(
            MissileRoutine()
        );
    }

    private IEnumerator MissileRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            FireMissile();

            yield return new WaitForSeconds(
                fireRate
            );

            timer += fireRate;
        }

        active = false;

        backgroundMaterial.SetFloat("_Power_Enable", 0);

        Debug.Log(
            "=== MODO MÍSSIL TERMINOU ==="
        );
    }

    private void FireMissile()
    {
        if (missilePrefab == null)
        {
            Debug.LogError(
                "MissileLauncher: " +
                "MISSILE PREFAB NÃO FOI DEFINIDO!"
            );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogError(
                "MissileLauncher: " +
                "FIRE POINT NÃO FOI DEFINIDO!"
            );

            return;
        }

        GameObject target =
            FindClosestTarget();

        if (target == null)
        {
            Debug.Log(
                "Míssil: nenhum alvo encontrado."
            );

            return;
        }

        Debug.Log(
            "Míssil disparado contra: " +
            target.name
        );

        GameObject missile =
            Instantiate(
                missilePrefab,
                firePoint.position,
                firePoint.rotation
            );

        Missile missileScript =
            missile.GetComponent<Missile>();

        if (missileScript == null)
        {
            Debug.LogError(
                "O Missile Prefab não possui " +
                "o script Missile!"
            );

            Destroy(missile);

            return;
        }

        missileScript.SetTarget(
            target.transform
        );
    }

    private GameObject FindClosestTarget()
    {
        GameObject closestTarget = null;

        float closestDistance =
            missileSearchRange;

        // Procura asteroides
        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag(
                "Asteroide"
            );

        foreach (GameObject asteroid in asteroids)
        {
            if (asteroid == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    asteroid.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = asteroid;
            }
        }

        // Procura naves inimigas
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(
                "Inimigo"
            );

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = enemy;
            }
        }

        return closestTarget;
    }
}