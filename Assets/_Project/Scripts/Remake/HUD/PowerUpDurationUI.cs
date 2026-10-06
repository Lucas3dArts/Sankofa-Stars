using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpDurationUI : MonoBehaviour
{
    public static PowerUpDurationUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject powerUpPanel;
    [SerializeField] private Slider durationSlider;
    [SerializeField] private TMP_Text powerUpNameText;

    private float duration;
    private float remainingTime;
    private bool active;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (!active)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            UpdateBar();
            Hide();

            return;
        }

        UpdateBar();
    }

    public void StartPowerUp(
        string powerUpName,
        float powerUpDuration
    )
    {
        duration = powerUpDuration;
        remainingTime = powerUpDuration;

        active = true;

        if (powerUpPanel != null)
            powerUpPanel.SetActive(true);

        if (powerUpNameText != null)
            powerUpNameText.text = powerUpName;

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (durationSlider == null)
            return;

        if (duration <= 0f)
        {
            durationSlider.value = 0f;
            return;
        }

        durationSlider.value =
            remainingTime / duration;
    }

    public void Hide()
    {
        active = false;

        if (powerUpPanel != null)
            powerUpPanel.SetActive(false);
    }
}