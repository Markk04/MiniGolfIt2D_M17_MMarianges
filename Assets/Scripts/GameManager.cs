using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject uiCanvas;
    public TextMeshProUGUI strokeCounterText;
    public TextMeshProUGUI timerText;

    public string[] levelNames;

    private int totalStrokes = 0;
    private int currentLevelStrokes = 0;
    private int currentLevelIndex = 0;

    private float timeRemaining = 90f;
    private bool timerRunning = false;

    public LevelPreviewController levelPreviewController; // Asigna esto desde el Inspector

    private GameObject currentLevelBall = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (uiCanvas != null)
                DontDestroyOnLoad(uiCanvas);

            GameObject playerBall = GameObject.FindGameObjectWithTag("Player");
            if (playerBall != null)
                DontDestroyOnLoad(playerBall);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destruye la pelota del nivel anterior si existe
        if (currentLevelBall != null)
        {
            Destroy(currentLevelBall);
            currentLevelBall = null;
        }

        // Busca la pelota del nivel actual
        GameObject playerBall = GameObject.FindGameObjectWithTag("Player");

        if (playerBall == null)
        {
            Debug.LogError("No se encontró la pelota con tag 'Player' en la escena.");
            return;
        }

        currentLevelBall = playerBall; // Guarda referencia para destruirla al cargar siguiente nivel

        // Buscar slider por tag
        GameObject sliderGO = GameObject.FindGameObjectWithTag("Slider");
        if (sliderGO != null)
        {
            Slider sliderComponent = sliderGO.GetComponent<Slider>();
            if (sliderComponent != null)
            {
                Ball ballScript = playerBall.GetComponent<Ball>();
                if (ballScript != null)
                {
                    ballScript.forceSlider = sliderComponent;
                    sliderComponent.gameObject.SetActive(false); // Asegura que esté oculto al inicio
                }
            }
            else
            {
                Debug.LogWarning("El objeto con tag 'Slider' no tiene componente Slider.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con tag 'Slider'.");
        }

        Ball ballScript2 = playerBall.GetComponent<Ball>();
        if (ballScript2 != null)
            ballScript2.ResetLevelDataWithoutMovingBall();

        currentLevelStrokes = 0;
        timeRemaining = 90f;
        timerRunning = true;

        UpdateStrokeUI();
        UpdateTimerUI();

        if (levelPreviewController != null)
        {
            levelPreviewController.ShowLevelImage();
        }
    }




    void Update()
    {
        if (!timerRunning) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0;
                timerRunning = false;
                OnTimeUp();
            }
        }

    }
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }


    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void AddStrokes(int strokes)
    {
        totalStrokes += strokes;
        currentLevelStrokes += strokes;
        UpdateStrokeUI();
        Debug.Log($"Level Strokes: {currentLevelStrokes} | Total Strokes: {totalStrokes}");
    }

    private void UpdateStrokeUI()
    {
        if (strokeCounterText == null)
            strokeCounterText = FindObjectOfType<TextMeshProUGUI>();

        if (strokeCounterText != null)
            strokeCounterText.text = $"{currentLevelStrokes}";
    }

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

    private void OnTimeUp()
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
            var text = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = "Time's Up!";
        }
        Time.timeScale = 0f;
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Length)
        {
            SceneManager.LoadScene(levelNames[currentLevelIndex]);
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(true);
                var text = uiCanvas.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = $"¡Ganaste!\nTotal Strokes: {totalStrokes}";
            }
            Time.timeScale = 0f;
        }
    }

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
