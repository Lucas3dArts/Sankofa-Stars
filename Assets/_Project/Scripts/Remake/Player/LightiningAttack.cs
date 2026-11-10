using System.Collections;
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

    [Header("BG Effect")]
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private bool powerUpAvailable = false;
    private bool powerActive = false;

    private float duration;
    private float fireRate;
    private float range;
    private float damage;

    private PowerLiquidFill liquidFill;

    private void Awake()
    {
        // Usa os valores padrão até o power-up
        // fornecer novos valores.
        duration = defaultDuration;
        fireRate = defaultFireRate;
        range = defaultRange;
        damage = defaultDamage;

        FindLightningLiquid();
    }

    private void Update()
    {
        // Só permite ativar se o jogador tiver coletado
        // o poder e ele ainda não estiver ativo.
        if (!powerUpAvailable || powerActive)
            return;

        if (
            powerUpAction != null &&
            powerUpAction.action.WasPressedThisFrame()
        )
        {
            ActivatePower();

            if (BackgroundPowerEffectManager.Instance != null)
            {
                BackgroundPowerEffectManager.Instance.ActivateEffect(
                    this,
                    color1,
                    color2
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

        // Enche o líquido quando o poder é coletado.
        PowerLiquidFill currentLiquid =
            GetLightningLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.CollectPower();
        }

        Debug.Log(
            "TEMPESTADE DE IANSÃ DISPONÍVEL!"
        );

        Debug.Log(
            "Pressione o BOTÃO DIREITO para ativar."
        );
    }

    private void ActivatePower()
    {
        powerUpAvailable = false;
        powerActive = true;

        Debug.Log(
            "TEMPESTADE DE IANSÃ ATIVADA!"
        );

        // Começa a esvaziar durante a duração
        // real do poder.
        PowerLiquidFill currentLiquid =
            GetLightningLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.ActivatePower(
                duration
            );
        }

        ShipController ship =
            GetComponent<ShipController>();

        if (ship != null)
        {
            ship.ActivateStorm(duration);
        }

        StartCoroutine(
            LightningRoutine()
        );
    }

    private void FindLightningLiquid()
    {
        GameObject liquidObject =
            GameObject.Find("Liquido_Lightning");

        if (liquidObject == null)
        {
            liquidFill = null;

            Debug.LogWarning(
                "LightningAttack: não foi encontrado um objeto ativo " +
                "chamado 'Liquido_Lightning' na cena.",
                this
            );

            return;
        }

        liquidFill =
            liquidObject.GetComponent<PowerLiquidFill>();

        if (liquidFill == null)
        {
            Debug.LogError(
                "LightningAttack: o objeto 'Liquido_Lightning' " +
                "não possui o componente PowerLiquidFill.",
                liquidObject
            );

            return;
        }

        Debug.Log(
            "Líquido da tempestade encontrado: " +
            GetFullPath(liquidObject.transform) +
            " | activeInHierarchy: " +
            liquidObject.activeInHierarchy,
            liquidObject
        );
    }

    private PowerLiquidFill GetLightningLiquid()
    {
        // Caso a referência não tenha sido localizada no Awake,
        // procura novamente quando o poder for coletado ou ativado.
        if (
            liquidFill == null ||
            !liquidFill.gameObject.activeInHierarchy
        )
        {
            FindLightningLiquid();
        }

        if (liquidFill == null)
        {
            Debug.LogError(
                "LightningAttack: não foi possível localizar " +
                "o PowerLiquidFill da tempestade.",
                this
            );

            return null;
        }

        if (!liquidFill.isActiveAndEnabled)
        {
            Debug.LogError(
                "LightningAttack: o componente PowerLiquidFill do objeto " +
                "'Liquido_Lightning' está inativo.",
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

        // Garante que o líquido termine completamente vazio.
        PowerLiquidFill currentLiquid =
            GetLightningLiquid();

        if (currentLiquid != null)
        {
            currentLiquid.SetFill(0f);
        }

        if (BackgroundPowerEffectManager.Instance != null)
        {
            BackgroundPowerEffectManager.Instance.DeactivateEffect(
                this
            );
        }

        Debug.Log(
            "TEMPESTADE DE IANSÃ TERMINOU!"
        );
    }

    private void FireLightning()
    {
        GameObject target =
            FindClosestTarget();

        if (target == null)
        {
            Debug.Log(
                "Nenhum alvo próximo para o raio."
            );

            return;
        }

        Debug.Log(
            "RAIO ATINGIU: " +
            target.name
        );

        CreateLightningEffect(
            target.transform
        );

        DamageTarget(target);
    }

    private GameObject FindClosestTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = range;

        // Procura asteroides.
        GameObject[] asteroids =
            GameObject.FindGameObjectsWithTag(
                "Asteroide"
            );

        foreach (GameObject asteroid in asteroids)
        {
            if (asteroid == null)
                continue;

            float distance = Vector3.Distance(
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

            float distance = Vector3.Distance(
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
        // Asteroide.
        Asteroide asteroid =
            target.GetComponentInParent<Asteroide>();

        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
            return;
        }

        // Nave inimiga.
        EnemyShip enemy =
            target.GetComponentInParent<EnemyShip>();

        if (enemy != null)
        {
            enemy.TakeDamage(
                Mathf.RoundToInt(damage)
            );

            return;
        }

        // Nave inimiga da estação.
        EnemyShipStation stationEnemy =
            target.GetComponentInParent<EnemyShipStation>();

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
        {
            Debug.LogWarning(
                "LightningAttack: Lightning Prefab " +
                "não foi configurado.",
                this
            );

            return;
        }

        Vector3 direction =
            (
                target.position -
                transform.position
            ).normalized;

        Vector3 startPosition =
            transform.position +
            direction * 5f;

        GameObject lightning = Instantiate(
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