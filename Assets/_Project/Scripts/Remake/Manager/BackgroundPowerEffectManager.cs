using System.Collections.Generic;
using UnityEngine;

public class BackgroundPowerEffectManager : MonoBehaviour
{
    public static BackgroundPowerEffectManager Instance { get; private set; }

    [Header("Material do Background")]
    [SerializeField] private Material backgroundMaterial;

    [Header("Combinação de poderes")]
    [Tooltip("Se ativado, mistura as cores de todos os poderes ativos.")]
    [SerializeField] private bool blendActivePowers = true;

    private static readonly int PowerEnableID =
        Shader.PropertyToID("_Power_Enable");

    private static readonly int Color1ID =
        Shader.PropertyToID("_Color_1");

    private static readonly int Color2ID =
        Shader.PropertyToID("_Color_2");

    private class ActivePowerEffect
    {
        public Object owner;
        public Color color1;
        public Color color2;
        public int activationOrder;
    }

    private readonly Dictionary<Object, ActivePowerEffect> activeEffects =
        new Dictionary<Object, ActivePowerEffect>();

    private int currentActivationOrder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DisableMaterialEffect();
    }

    /// <summary>
    /// Ativa ou atualiza o efeito visual de um poder.
    /// O owner normalmente deve ser "this", vindo do script do poder.
    /// </summary>
    public void ActivateEffect(
        Object owner,
        Color color1,
        Color color2
    )
    {
        if (owner == null)
        {
            Debug.LogWarning(
                "Não é possível ativar o efeito sem informar o objeto responsável."
            );

            return;
        }

        currentActivationOrder++;

        if (activeEffects.TryGetValue(owner, out ActivePowerEffect effect))
        {
            effect.color1 = color1;
            effect.color2 = color2;
            effect.activationOrder = currentActivationOrder;
        }
        else
        {
            activeEffects.Add(
                owner,
                new ActivePowerEffect
                {
                    owner = owner,
                    color1 = color1,
                    color2 = color2,
                    activationOrder = currentActivationOrder
                }
            );
        }

        UpdateMaterialEffect();
    }

    /// <summary>
    /// Remove somente o efeito pertencente ao poder informado.
    /// O background continuará ativo caso existam outros poderes funcionando.
    /// </summary>
    public void DeactivateEffect(Object owner)
    {
        if (owner == null)
            return;

        if (activeEffects.Remove(owner))
        {
            UpdateMaterialEffect();
        }
    }

    private void UpdateMaterialEffect()
    {
        RemoveDestroyedOwners();

        if (backgroundMaterial == null)
        {
            Debug.LogWarning(
                "O material do background não foi atribuído.",
                gameObject
            );

            return;
        }

        if (activeEffects.Count == 0)
        {
            DisableMaterialEffect();
            return;
        }

        if (blendActivePowers)
        {
            ApplyBlendedColors();
        }
        else
        {
            ApplyMostRecentPower();
        }

        backgroundMaterial.SetFloat(PowerEnableID, 1f);
    }

    private void ApplyBlendedColors()
    {
        Color blendedColor1 = Color.clear;
        Color blendedColor2 = Color.clear;

        foreach (ActivePowerEffect effect in activeEffects.Values)
        {
            blendedColor1 += effect.color1;
            blendedColor2 += effect.color2;
        }

        float effectCount = activeEffects.Count;

        blendedColor1 /= effectCount;
        blendedColor2 /= effectCount;

        backgroundMaterial.SetColor(Color1ID, blendedColor1);
        backgroundMaterial.SetColor(Color2ID, blendedColor2);
    }

    private void ApplyMostRecentPower()
    {
        ActivePowerEffect mostRecentEffect = null;

        foreach (ActivePowerEffect effect in activeEffects.Values)
        {
            if (mostRecentEffect == null ||
                effect.activationOrder > mostRecentEffect.activationOrder)
            {
                mostRecentEffect = effect;
            }
        }

        if (mostRecentEffect == null)
            return;

        backgroundMaterial.SetColor(
            Color1ID,
            mostRecentEffect.color1
        );

        backgroundMaterial.SetColor(
            Color2ID,
            mostRecentEffect.color2
        );
    }

    private void RemoveDestroyedOwners()
    {
        if (activeEffects.Count == 0)
            return;

        List<Object> destroyedOwners = new List<Object>();

        foreach (KeyValuePair<Object, ActivePowerEffect> effect
                 in activeEffects)
        {
            if (effect.Key == null)
            {
                destroyedOwners.Add(effect.Key);
            }
        }

        foreach (Object owner in destroyedOwners)
        {
            activeEffects.Remove(owner);
        }
    }

    private void DisableMaterialEffect()
    {
        if (backgroundMaterial == null)
            return;

        backgroundMaterial.SetFloat(PowerEnableID, 0f);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        DisableMaterialEffect();

        activeEffects.Clear();
        Instance = null;
    }
}