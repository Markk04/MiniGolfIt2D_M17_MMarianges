using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuScreen : MonoBehaviour
{
    // Configuració dels panels del menú principal
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuPanel;     
    [SerializeField] private GameObject settingsPanel;      
    [SerializeField] private GameObject playerNamePanel;   
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button playButton;            

    // Configuració dels botons principals
    [Header("Buttons")]
    [SerializeField] private Button startButton;   
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;    

    // Altres components necessaris
    [Header("Other References")]
    [SerializeField] private LevelPreviewController previewController; 
    [SerializeField] private AudioClip buttonClickSound;

    private AudioSource audioSource; 
    public static string PlayerName { get; private set; } = "Player";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (playerNamePanel != null) playerNamePanel.SetActive(false);
    }

    void Start()
    {
        startButton?.onClick.AddListener(OnStartClicked);
        settingsButton?.onClick.AddListener(OnSettingsClicked);
        quitButton?.onClick.AddListener(OnQuitClicked);
        playButton?.onClick.AddListener(OnPlayClicked);

        Time.timeScale = 0f;
    }

    // Gestiona el clic al botó d'inici
    private void OnStartClicked()
    {
        PlayClickSound();

        mainMenuPanel?.SetActive(false);
        playerNamePanel?.SetActive(true);

        // Selecciona automàticament el camp de text
        playerNameInput?.Select();
    }

    private void OnPlayClicked()
    {
        PlayClickSound();

        // Guarda el nom si s'ha introduït
        if (!string.IsNullOrWhiteSpace(playerNameInput?.text))
        {
            PlayerName = playerNameInput.text;
            PlayerPrefs.SetString("LastPlayerName", PlayerName);
            PlayerPrefs.Save();
        }

        playerNamePanel?.SetActive(false);
        Time.timeScale = 1f;

        previewController?.ShowLevelImage();
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

    void PlayClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}