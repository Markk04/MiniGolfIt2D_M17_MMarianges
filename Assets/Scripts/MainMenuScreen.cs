using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    public LevelPreviewController previewController;


    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        Time.timeScale = 0f; // Pausar el juego mientras se muestra el menú
    }

    void OnStartClicked()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        Time.timeScale = 1f; // Reanudar el juego

        if (previewController != null)
            previewController.ShowLevelImage();
    }

    void OnSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
