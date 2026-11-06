using UnityEngine;

public class ExplosionEffectManager : MonoBehaviour
{
    public static ExplosionEffectManager Instance { get; private set; }

    [Header("Explosion Prefab")]
    [SerializeField] private GameObject explosionEffectPrefab;

    [Header("Default Settings")]
    [SerializeField]
    private Vector3 defaultParticleScale =
        Vector3.one;

    [SerializeField] private float defaultSizeMultiplier = 1f;

    [SerializeField] private float defaultLifetime = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreateExplosion(Vector3 position)
    {
        CreateExplosion(
            position,
            Quaternion.identity,
            defaultParticleScale,
            defaultSizeMultiplier
        );
    }

    public void CreateExplosion(
        Vector3 position,
        float sizeMultiplier
    )
    {
        CreateExplosion(
            position,
            Quaternion.identity,
            defaultParticleScale,
            sizeMultiplier
        );
    }

    public void CreateExplosion(
        Vector3 position,
        Quaternion rotation,
        Vector3 particleScale,
        float sizeMultiplier
    )
    {
        if (explosionEffectPrefab == null)
        {
            Debug.LogWarning(
                "O prefab de explosão não foi atribuído " +
                "no ExplosionEffectManager.",
                gameObject
            );

            return;
        }

        GameObject explosionObject = Instantiate(
            explosionEffectPrefab,
            position,
            rotation
        );

        // Mantém o Empty principal com escala normal.
        explosionObject.transform.localScale = Vector3.one;

        ParticleSystem[] particleSystems =
            explosionObject.GetComponentsInChildren<ParticleSystem>(
                true
            );

        float calculatedLifetime = defaultLifetime;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ConfigureParticleSystem(
                particleSystem,
                particleScale,
                sizeMultiplier
            );

            float particleLifetime =
                CalculateParticleLifetime(particleSystem);

            if (particleLifetime > calculatedLifetime)
            {
                calculatedLifetime = particleLifetime;
            }

            particleSystem.Play();
        }

        Destroy(
            explosionObject,
            calculatedLifetime + 0.2f
        );
    }

    private void ConfigureParticleSystem(
        ParticleSystem particleSystem,
        Vector3 particleScale,
        float sizeMultiplier
    )
    {
        // A escala é aplicada diretamente no objeto
        // que possui o Particle System.
        particleSystem.transform.localScale =
            Vector3.Scale(
                particleSystem.transform.localScale,
                particleScale
            );

        ParticleSystem.MainModule main =
            particleSystem.main;

        main.startSizeMultiplier *= sizeMultiplier;
    }

    private float CalculateParticleLifetime(
        ParticleSystem particleSystem
    )
    {
        ParticleSystem.MainModule main =
            particleSystem.main;

        float maximumStartLifetime =
            GetMaximumValue(main.startLifetime);

        return main.duration + maximumStartLifetime;
    }

    private float GetMaximumValue(
        ParticleSystem.MinMaxCurve curve
    )
    {
        switch (curve.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return curve.constant;

            case ParticleSystemCurveMode.TwoConstants:
                return curve.constantMax;

            case ParticleSystemCurveMode.Curve:
                return curve.curveMultiplier;

            case ParticleSystemCurveMode.TwoCurves:
                return curve.curveMultiplier;

            default:
                return 1f;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
