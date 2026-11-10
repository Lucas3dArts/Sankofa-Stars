using System.Collections;
using UnityEngine;

public class PowerLiquidFill : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Renderer liquidRenderer;

    [Header("Shader")]
    [SerializeField] private string fillProperty = "_Fill";

    [Header("Configuração inicial")]
    [SerializeField] private bool startEmpty = true;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine drainCoroutine;

    private int fillPropertyID;
    private float currentFill;

    public float CurrentFill => currentFill;
    public bool IsFull => currentFill >= 1f;
    public bool IsEmpty => currentFill <= 0f;
    public bool IsDraining => drainCoroutine != null;

    private void Awake()
    {
        Initialize();
        SetFill(startEmpty ? 0f : 1f);
    }

    private void Initialize()
    {
        if (liquidRenderer == null)
        {
            liquidRenderer = GetComponent<Renderer>();
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        fillPropertyID = Shader.PropertyToID(fillProperty);
    }

    public void CollectPower()
    {
        Initialize();
        EnsureObjectIsActive();
        StopDrain();
        SetFill(1f);
    }

    public void ActivatePower(float duration)
    {
        Initialize();
        EnsureObjectIsActive();

        Debug.Log(
            "PowerLiquidFill ActivatePower | " +
            "Objeto: " + gameObject.name +
            " | Cena: " + gameObject.scene.name +
            " | activeSelf: " + gameObject.activeSelf +
            " | activeInHierarchy: " + gameObject.activeInHierarchy +
            " | componente enabled: " + enabled,
            gameObject
        );

        StopDrain();

        if (duration <= 0f)
        {
            SetFill(0f);
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError(
                "PowerLiquidFill: '" +
                gameObject.name +
                "' continua inativo na Hierarchy. " +
                "Verifique os objetos pais ou outro script " +
                "que esteja usando SetActive(false).",
                gameObject
            );

            return;
        }

        if (!isActiveAndEnabled)
        {
            Debug.LogError(
                "PowerLiquidFill está desabilitado em '" +
                gameObject.name +
                "'.",
                gameObject
            );

            return;
        }

        drainCoroutine = StartCoroutine(
            DrainRoutine(duration)
        );
    }

    public void PauseDrain()
    {
        StopDrain();
    }

    public void Empty()
    {
        StopDrain();
        SetFill(0f);
    }

    public void SetFill(float value)
    {
        Initialize();

        currentFill = Mathf.Clamp01(value);

        if (liquidRenderer == null)
        {
            Debug.LogError(
                "PowerLiquidFill: nenhum Renderer foi encontrado em '" +
                gameObject.name +
                "'.",
                this
            );

            return;
        }

        liquidRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(
            fillPropertyID,
            currentFill
        );

        liquidRenderer.SetPropertyBlock(
            propertyBlock
        );
    }

    private void EnsureObjectIsActive()
    {
        Transform[] hierarchy =
            GetComponentsInParent<Transform>(true);

        // Ativa primeiro os pais e depois os filhos.
        for (int i = hierarchy.Length - 1; i >= 0; i--)
        {
            if (!hierarchy[i].gameObject.activeSelf)
            {
                hierarchy[i].gameObject.SetActive(true);
            }
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!enabled)
        {
            enabled = true;
        }
    }

    private IEnumerator DrainRoutine(float duration)
    {
        float initialFill = currentFill;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(
                elapsedTime / duration
            );

            float newFill = Mathf.Lerp(
                initialFill,
                0f,
                normalizedTime
            );

            SetFill(newFill);

            yield return null;
        }

        SetFill(0f);
        drainCoroutine = null;
    }

    private void StopDrain()
    {
        if (drainCoroutine == null)
            return;

        StopCoroutine(drainCoroutine);
        drainCoroutine = null;
    }

    private void OnDisable()
    {
        drainCoroutine = null;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (liquidRenderer == null)
        {
            liquidRenderer = GetComponent<Renderer>();
        }

        if (string.IsNullOrWhiteSpace(fillProperty))
        {
            fillProperty = "_Fill";
        }

        fillPropertyID = Shader.PropertyToID(fillProperty);
    }

#endif
}