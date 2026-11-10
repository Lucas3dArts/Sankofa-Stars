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
    private PowerLiquidFill liquidFill;

    private void Awake()
    {
        FindMissileLiquid();
    }

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

        // Enche o líquido quando o poder é coletado.
        PowerLiquidFill currentLiquid =
            GetMissileLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.CollectPower();
        }

        Debug.Log(
            "POWER-UP DE MÍSSEIS COLETADO!"
        );

        Debug.Log(
            "Pressione BOTÃO DIREITO do mouse para ativar."
        );
    }

    private void Update()
    {
        // Só permite ativar se o poder estiver disponível
        // e não estiver sendo usado.
        if (!hasPowerUp || active)
            return;

        if (
            powerUpAction != null &&
            powerUpAction.action.WasPressedThisFrame()
        )
        {
            ActivateMissiles();

            if (backgroundMaterial != null)
            {
                backgroundMaterial.SetColor(
                    "_Color_1",
                    color1
                );

                backgroundMaterial.SetColor(
                    "_Color_2",
                    color2
                );

                backgroundMaterial.SetFloat(
                    "_Power_Enable",
                    1f
                );
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPowerUpSound();
            }
        }
    }

    private void OnEnable()
    {
        if (powerUpAction != null)
        {
            powerUpAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (powerUpAction != null)
        {
            powerUpAction.action.Disable();
        }
    }

    private void FindMissileLiquid()
    {
        GameObject liquidObject =
            GameObject.Find("Liquido_Missiles");

        if (liquidObject == null)
        {
            liquidFill = null;

            Debug.LogWarning(
                "MissileLauncher: não foi encontrado um objeto ativo " +
                "chamado 'Liquido_Missiles' na cena.",
                this
            );

            return;
        }

        liquidFill =
            liquidObject.GetComponent<PowerLiquidFill>();

        if (liquidFill == null)
        {
            Debug.LogError(
                "MissileLauncher: o objeto 'Liquido_Missiles' " +
                "não possui o componente PowerLiquidFill.",
                liquidObject
            );

            return;
        }

        Debug.Log(
            "Líquido dos mísseis encontrado: " +
            GetFullPath(liquidObject.transform) +
            " | activeInHierarchy: " +
            liquidObject.activeInHierarchy,
            liquidObject
        );
    }

    private PowerLiquidFill GetMissileLiquid()
    {
        // Se a referência não existe ou ficou inválida,
        // procura novamente na cena.
        if (
            liquidFill == null ||
            !liquidFill.gameObject.activeInHierarchy
        )
        {
            FindMissileLiquid();
        }

        if (liquidFill == null)
        {
            Debug.LogError(
                "MissileLauncher: não foi possível localizar " +
                "o PowerLiquidFill dos mísseis.",
                this
            );

            return null;
        }

        if (!liquidFill.isActiveAndEnabled)
        {
            Debug.LogError(
                "MissileLauncher: o PowerLiquidFill do objeto " +
                "'Liquido_Missiles' está inativo.",
                liquidFill.gameObject
            );

            return null;
        }

        return liquidFill;
    }

    private string GetFullPath(Transform target)
    {
        string path = target.name;

        while (target.parent != null)
        {
            target = target.parent;

            path =
                target.name +
                "/" +
                path;
        }

        return path;
    }

    private void ActivateMissiles()
    {
        hasPowerUp = false;
        active = true;

        Debug.Log(
            "=== MODO MÍSSIL ATIVADO ==="
        );

        // Começa a esvaziar o líquido durante
        // o mesmo tempo de duração dos mísseis.
        PowerLiquidFill currentLiquid =
            GetMissileLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.ActivatePower(
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

        // Garante que o líquido fique totalmente vazio
        // quando a rotina terminar.
        PowerLiquidFill currentLiquid =
            GetMissileLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.SetFill(0f);
        }

        if (backgroundMaterial != null)
        {
            backgroundMaterial.SetFloat(
                "_Power_Enable",
                0f
            );
        }

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

        GameObject missile = Instantiate(
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

        // Procura asteroides.
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

        // Procura naves inimigas.
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