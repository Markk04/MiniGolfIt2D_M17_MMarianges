using UnityEngine;
using UnityEngine.UI;

public class SettingsP : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(CloseSettings);
    }

    void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
