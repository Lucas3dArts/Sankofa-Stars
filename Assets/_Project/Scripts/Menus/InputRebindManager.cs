using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebindManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    private const string REBINDS_KEY = "INPUT_REBINDS";

    private void Awake()
    {
        LoadBindings();
    }

    public void SaveBindings()
    {
        if (inputActions == null)
            return;

        string rebinds =
            inputActions.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(
            REBINDS_KEY,
            rebinds
        );

        PlayerPrefs.Save();

        Debug.Log("Controles salvos!");
    }

    public void LoadBindings()
    {
        if (inputActions == null)
            return;

        if (!PlayerPrefs.HasKey(REBINDS_KEY))
            return;

        string rebinds =
            PlayerPrefs.GetString(REBINDS_KEY);

        inputActions.LoadBindingOverridesFromJson(
            rebinds
        );

        Debug.Log("Controles carregados!");
    }

    public void ResetBindings()
    {
        if (inputActions == null)
        {
            Debug.LogError(
                "InputRebindManager: Input Actions não foi definido!"
            );

            return;
        }

        // Remove todas as alterações feitas pelo jogador
        inputActions.RemoveAllBindingOverrides();

        // Apaga as configurações salvas
        PlayerPrefs.DeleteKey(REBINDS_KEY);
        PlayerPrefs.Save();

        Debug.Log("Controles restaurados para o padrão!");

        // Atualiza os textos dos botões
        KeyRebindUI[] rebindButtons =
            FindObjectsOfType<KeyRebindUI>(true);

        foreach (KeyRebindUI rebindButton in rebindButtons)
        {
            rebindButton.RefreshDisplay();
        }
    }
}