using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebindUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference actionReference;

    [Header("Binding")]
    [SerializeField] private int bindingIndex = 1;

    [Header("UI")]
    [SerializeField] private TMP_Text keyText;

    private InputAction action;

    private void Awake()
    {
        if (actionReference != null)
        {
            action = actionReference.action;
        }
    }

    private void Start()
    {
        LoadBinding();
        RefreshDisplay();
    }

    public void StartRebinding()
    {
        if (action == null)
            return;

        action.Disable();

        keyText.text = "...";

        action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();

                action.Enable();

                SaveBinding();
                RefreshDisplay();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();

                action.Enable();

                RefreshDisplay();
            })
            .Start();
    }

    public void RefreshDisplay()
    {
        if (keyText == null || action == null)
            return;

        keyText.text =
            action.GetBindingDisplayString(bindingIndex);
    }

    private void SaveBinding()
    {
        if (action == null)
            return;

        string rebinds =
            action.actionMap.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(
            "INPUT_REBINDS",
            rebinds
        );

        PlayerPrefs.Save();
    }

    private void LoadBinding()
    {
        if (action == null)
            return;

        if (PlayerPrefs.HasKey("INPUT_REBINDS"))
        {
            string rebinds =
                PlayerPrefs.GetString("INPUT_REBINDS");

            action.actionMap.asset
                .LoadBindingOverridesFromJson(rebinds);
        }
    }
}