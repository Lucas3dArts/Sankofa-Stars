using UnityEngine;
using UnityEngine.UI;

public class MovementModeUI : MonoBehaviour
{
    [Header("Ship")]
    [SerializeField] private ShipController shipController;

    [Header("Buttons")]
    [SerializeField] private Button wsButton;
    [SerializeField] private Button scrollButton;

    [Header("Button Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;

    private const string MOVEMENT_MODE_KEY = "MOVEMENT_MODE";

    private void Start()
    {
        UpdateButtonColors();
    }

    public void SetWS()
    {
        shipController.SetMovementMode(
            ShipController.MovementMode.WS
        );

        SaveMovementMode(
            ShipController.MovementMode.WS
        );

        UpdateButtonColors();
    }

    public void SetScroll()
    {
        shipController.SetMovementMode(
            ShipController.MovementMode.Scroll
        );

        SaveMovementMode(
            ShipController.MovementMode.Scroll
        );

        UpdateButtonColors();
    }

    private void SaveMovementMode(
        ShipController.MovementMode mode)
    {
        PlayerPrefs.SetInt(
            MOVEMENT_MODE_KEY,
            (int)mode
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Modo de movimento salvo: " + mode
        );
    }

    private void UpdateButtonColors()
    {
        if (shipController == null)
            return;

        if (shipController.CurrentMovementMode ==
            ShipController.MovementMode.WS)
        {
            wsButton.image.color = activeColor;
            scrollButton.image.color = inactiveColor;
        }
        else
        {
            wsButton.image.color = inactiveColor;
            scrollButton.image.color = activeColor;
        }
    }
}