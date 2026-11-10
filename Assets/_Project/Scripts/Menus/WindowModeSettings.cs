using TMPro;
using UnityEngine;

public class WindowModeSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown modeDropdown;

    private const string WINDOW_MODE_KEY = "WindowMode";

    private void Start()
    {
        int savedMode = PlayerPrefs.GetInt(WINDOW_MODE_KEY, 0);

        modeDropdown.value = savedMode;
        modeDropdown.RefreshShownValue();

        ApplyWindowMode(savedMode);
    }

    public void SetWindowMode(int mode)
    {
        ApplyWindowMode(mode);

        PlayerPrefs.SetInt(WINDOW_MODE_KEY, mode);
        PlayerPrefs.Save();
    }

    private void ApplyWindowMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode =
                    FullScreenMode.ExclusiveFullScreen;
                break;

            case 1:
                Screen.fullScreenMode =
                    FullScreenMode.Windowed;
                break;

            case 2:
                Screen.fullScreenMode =
                    FullScreenMode.FullScreenWindow;
                break;
        }
    }
}