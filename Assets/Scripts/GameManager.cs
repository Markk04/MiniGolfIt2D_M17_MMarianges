using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton pattern per accés global
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject uiCanvas;                   
    public TextMeshProUGUI strokeCounterText;    
    public TextMeshProUGUI timerText;           

    [Header("Level Management")]
    public string[] levelNames;                   
    private int currentLevelIndex = 0;            

    [Header("Game Stats")]
    private int totalStrokes = 0;               
    private int currentLevelStrokes = 0;         
    private float timeRemaining = 90f;          
    private bool timerRunning = false;           

    [Header("References")]
    public LevelPreviewController levelPreviewController;  // Controlador de previsualització de nivell
    public CanvasGroup holeWinGroup;            
    private GameObject currentLevelBall = null;   

    [Header("Game End Screen")]
    public GameObject gameEndScreen;              
    public TextMeshProUGUI finalTimeText;         
    public TextMeshProUGUI finalStrokesText;     
    public Button returnToMenuButton;             

    void Awake()
    {
        // Implementació del patró Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Manté els objectes necessaris entre escenes
            if (uiCanvas != null)
                DontDestroyOnLoad(uiCanvas);

            GameObject playerBall = GameObject.FindGameObjectWithTag("Player");
            if (playerBall != null)
                DontDestroyOnLoad(playerBall);

            // Subscripció a l'event de càrrega d'escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Gestiona la càrrega de cada escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Neteja la bola del nivell anterior
        if (currentLevelBall != null)
        {
            Destroy(currentLevelBall);
            currentLevelBall = null;
        }

        // Troba la bola del nou nivell
        GameObject playerBall = GameObject.FindGameObjectWithTag("Player");
        if (playerBall == null)
        {
            Debug.LogError("No s'ha trobat la bola amb tag 'Player' a l'escena.");
            return;
        }

        currentLevelBall = playerBall;

        // Reinicia les dades de la bola
        Ball ballScript = playerBall.GetComponent<Ball>();
        if (ballScript != null)
            ballScript.ResetLevelDataWithoutMovingBall();

        // Reinicia estadístiques del nivell
        currentLevelStrokes = 0;
        timeRemaining = 90f;
        timerRunning = true;

        UpdateStrokeUI();
        UpdateTimerUI();

        // Mostra la previsualització del nivell
        if (levelPreviewController != null)
        {
            levelPreviewController.ShowLevelImage();
        }
    }

    void Update()
    {
        // Actualitza el temporitzador si està actiu
        if (!timerRunning) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0;
                timerRunning = false;
                OnTimeUp(); // Gestiona el final del temps
            }
        }
    }

    // Mètodes públics per accedir a dades del joc
    public int GetCurrentLevelIndex() => currentLevelIndex;
    public float GetTimeRemaining() => timeRemaining;

    public void AddStrokes(int strokes)
    {
        totalStrokes += strokes;
        currentLevelStrokes += strokes;
        UpdateStrokeUI();
    }

    // Actualitza la UI dels strokes
    private void UpdateStrokeUI()
    {
        if (strokeCounterText == null)
            strokeCounterText = FindObjectOfType<TextMeshProUGUI>();

        if (strokeCounterText != null)
            strokeCounterText.text = $"{currentLevelStrokes}";
    }

    // Actualitza la UI del temporitzador
    private void UpdateTimerUI()
    {
        if (timerText == null)
            timerText = FindObjectOfType<TextMeshProUGUI>();

        if (timerText != null)
        {
            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Gestiona quan s'acaba el temps
    private void OnTimeUp()
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
            var text = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = "Temps esgotat!";
        }
        Time.timeScale = 0f;
    }

    // Mostra l'efecte de victòria i carrega el següent nivell
    public void ShowHoleImageAndLoadNext()
    {
        StartCoroutine(FadeInHoleImageThenLoad());
    }

    // Coroutine per l'efecte de fade al completar nivell
    private IEnumerator FadeInHoleImageThenLoad()
    {
        if (holeWinGroup != null)
        {
            holeWinGroup.gameObject.SetActive(true);
            float duration = 0.5f;
            float elapsed = 0f;

            // Fade-in progressiu
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                holeWinGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            holeWinGroup.alpha = 1f;
            yield return new WaitForSeconds(0.7f); // Espera abans de canviar de nivell

            holeWinGroup.gameObject.SetActive(false);
            holeWinGroup.alpha = 0f;
        }

        LoadNextLevel();
    }

    // Carrega el següent nivell o mostra la pantalla final
    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Length)
        {
            SceneManager.LoadScene(levelNames[currentLevelIndex]);
        }
        else
        {
            ShowGameEndScreen(); // Tots els nivells completats
        }
    }

    // Mostra la pantalla de fi del joc
    public void ShowGameEndScreen()
    {
        Time.timeScale = 0f;

        if (gameEndScreen != null)
        {
            gameEndScreen.SetActive(true);

            // Calcula i mostra el temps total
            if (finalTimeText != null)
            {
                int minutes = (int)(90f - timeRemaining) / 60;
                int seconds = (int)(90f - timeRemaining) % 60;
                finalTimeText.text = $"Temps Total: {minutes:00}:{seconds:00}";
            }

            // Mostra els strokes totals
            if (finalStrokesText != null)
            {
                finalStrokesText.text = $"Total Strokes: {totalStrokes}";
            }

            // Configura el botó de tornada al menú
            if (returnToMenuButton != null)
            {
                returnToMenuButton.onClick.RemoveAllListeners();
                returnToMenuButton.onClick.AddListener(() =>
                {
                    if (gameEndScreen != null)
                        gameEndScreen.SetActive(false);

                    Time.timeScale = 1f;
                    ResetGame(); // Reinicia el joc
                });
            }
        }
    }

    // Reinicia totes les dades del joc
    public void ResetGame()
    {
        currentLevelIndex = 0;
        totalStrokes = 0;
        currentLevelStrokes = 0;
        timeRemaining = 60f;
        timerRunning = true;

        if (levelNames.Length > 0)
            SceneManager.LoadScene(levelNames[0]);

        if (uiCanvas != null)
            uiCanvas.SetActive(false);

        Time.timeScale = 1f;
    }
}