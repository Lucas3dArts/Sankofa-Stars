using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightningAttack : MonoBehaviour
{
    [Header("Lightning")]
    [SerializeField] private GameObject lightningPrefab;

    [Header("Power Settings")]
    [SerializeField] private float defaultDuration = 8f;
    [SerializeField] private float defaultFireRate = 0.5f;
    [SerializeField] private float defaultRange = 80f;
    [SerializeField] private float defaultDamage = 100f;

    [Header("Input")]
    [SerializeField] private InputActionReference powerUpAction;

    private bool powerUpAvailable = false;
    private bool powerActive = false;

    private float duration;
    private float fireRate;
    private float range;
    private float damage;

    private void Update()
    {
        if (!powerUpAvailable || powerActive)
            return;

        if (powerUpAction != null &&
            powerUpAction.action.WasPressedThisFrame())
        {
            ActivatePower();
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

    public void StorePowerUp(
        float newDuration,
        float newFireRate,
        float newRange,
        float newDamage
    )
    {
        duration = newDuration;
        fireRate = newFireRate;
        range = newRange;
        damage = newDamage;

        powerUpAvailable = true;

        Debug.Log(
            "⚡ TEMPESTADE DE IANSÃ DISPONÍVEL!"
        );

        Debug.Log(
            "Pressione o BOTÃO DIREITO para ativar."
        );
    }

    private void ActivatePower()
    {
        powerUpAvailable = false;
        powerActive = true;

        Debug.Log("⚡ TEMPESTADE DE IANSÃ ATIVADA!");

        ShipController ship =
            GetComponent<ShipController>();

        if (ship != null)
        {
            ship.ActivateStorm(duration);
        }

        if (PowerUpDurationUI.Instance != null)
        {
            PowerUpDurationUI.Instance.StartPowerUp(
                "Tempestade de Iansã",
                duration
            );
        }

        StartCoroutine(LightningRoutine());
    }
    private IEnumerator LightningRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            FireLightning();

            yield return new WaitForSeconds(
                fireRate
            );

            timer += fireRate;
        }

        powerActive = false;

        Debug.Log(
            "⚡ TEMPESTADE DE IANSÃ TERMINOU!"
        );
    }

    private void FireLightning()
    {
        GameObject target =
            FindClosestTarget();

        if (target == null)
        {
            Debug.Log(
                "⚡ Nenhum alvo próximo para o raio."
            );

            return;
        }

        Debug.Log(
            "⚡ RAIO ATINGIU: " +
            target.name
        );

        // Cria o efeito visual do raio
        CreateLightningEffect(target.transform);

        // Causa dano
        DamageTarget(target);
    }

    private GameObject FindClosestTarget()
    {
        GameObject closestTarget = null;

        float closestDistance = range;

        // =========================
        // ASTEROIDES
        // =========================

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

        // =========================
        // NAVES
        // =========================

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

    private void DamageTarget(GameObject target)
    {
        // ASTEROIDE
        Asteroide asteroid =
            target.GetComponentInParent<Asteroide>();

        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
            return;
        }

        // NAVE
        EnemyShip enemy =
            target.GetComponentInParent<EnemyShip>();

        if (enemy != null)
        {
            enemy.TakeDamage(
                Mathf.RoundToInt(damage)
            );

            return;
        }

        // NAVE DA ESTAÇÃO
        EnemyShipStation stationEnemy =
            target.GetComponentInParent<
                EnemyShipStation
            >();

        if (stationEnemy != null)
        {
            stationEnemy.TakeDamage(
                Mathf.RoundToInt(damage)
            );
        }
    }

    private void CreateLightningEffect(Transform target)
    {
        if (lightningPrefab == null)
            return;

        Vector3 direction =
            (target.position - transform.position).normalized;

        // Posição inicial do raio um pouco à frente da nave
        Vector3 startPosition =
            transform.position + direction * 5f;

        GameObject lightning =
            Instantiate(
                lightningPrefab,
                startPosition,
                Quaternion.identity
            );

        LightningEffect effect =
            lightning.GetComponent<LightningEffect>();

        if (effect != null)
        {
            effect.SetTarget(
                startPosition,
                target
            );
        }
        else
        {
            Destroy(
                lightning,
                0.2f
            );
        }
    }
}