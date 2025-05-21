using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject ball;
    public GameObject uiCanvas;

    private int totalStrokes = 0;
    private int currentLevelIndex = 0;

    public string[] levelNames; // Lista de nombres de niveles (Level2, Level3, etc.)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (uiCanvas != null)
            {
                DontDestroyOnLoad(uiCanvas);
            }

            if (ball != null)
            {
                DontDestroyOnLoad(ball);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddStrokes(int strokes)
    {
        totalStrokes += strokes;
    }

    public int GetTotalStrokes()
    {
        return totalStrokes;
    }

    public void LoadNextLevel(Vector3 defaultBallPosition)
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelNames.Length)
        {
            string nextLevel = levelNames[currentLevelIndex];
            SceneManager.LoadScene(nextLevel);

            // Al cargar un nuevo nivel, colocar la pelota en la posición deseada
            if (ball != null)
            {
                Vector3 startPosition = defaultBallPosition;

                // Verificar si estamos cargando el nivel 3 para asignar una posición específica
                if (nextLevel == "Level3")
                {
                    startPosition = new Vector3(3.8499999f, 1.10000002f, -0.31f); // Ajustar el Z a 0
                }

                ball.transform.position = startPosition;
            }
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
            // Lógica para finalizar el juego (pantalla de victoria)
            uiCanvas.SetActive(true);
            uiCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "¡Ganaste!";
        }
    }

    public void ResetGame()
    {
        totalStrokes = 0;
        currentLevelIndex = 0;
        SceneManager.LoadScene(levelNames[0]); // Reiniciar desde el primer nivel
    }
}
