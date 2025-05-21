using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importar TextMeshPro
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    private Vector2 _startMousePosition;
    private float _holdTime;
    private bool _isCharging = false;
    private bool _canShoot = true;

    public float forceMultiplier = 10f;
    public float maxHoldTime = 1f;
    private Rigidbody2D rb;
    public float minSpeedToWin = 3f;

    public Slider forceSlider;
    public LineRenderer directionLine;
    public float lineLength = 2f;
    private Vector2 _launchDirection;

    public TextMeshProUGUI strokeCounterText;  // Texto para el contador de golpes
    private int strokeCount = 0;

    public TextMeshProUGUI timerText;          // Texto para el temporizador
    private float timeRemaining = 60f;         // 1 minuto en segundos

    private Vector3 ballStartPosition; // Para guardar la posición inicial de la pelota

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener la posición inicial de la pelota al inicio del nivel
        ballStartPosition = transform.position;

        // Volver a encontrar las referencias de la UI y la línea si se cambió de escena
        if (directionLine == null)
        {
            directionLine = GetComponentInChildren<LineRenderer>();
            if (directionLine == null)
            {
                Debug.LogError("LineRenderer no encontrado. Asegúrate de que esté asignado correctamente.");
            }
        }

        if (strokeCounterText == null || timerText == null || forceSlider == null)
        {
            strokeCounterText = FindObjectOfType<TextMeshProUGUI>();
            timerText = FindObjectOfType<TextMeshProUGUI>();
            forceSlider = FindObjectOfType<Slider>();
        }

        // Inicializar el contador de golpes y temporizador
        UpdateStrokeCounter();
        UpdateTimerText();
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_isCharging && directionLine != null)
        {
            ShowDirectionLine(mousePosition);
        }

        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            _isCharging = true;
            _startMousePosition = mousePosition;
            _holdTime = 0f;

            if (forceSlider != null)
            {
                forceSlider.value = 0f;
                forceSlider.gameObject.SetActive(true);
            }

            directionLine.enabled = true;
        }

        if (_isCharging)
        {
            if (_holdTime < maxHoldTime)
            {
                _holdTime += Time.deltaTime;
            }

            if (forceSlider != null)
            {
                forceSlider.value = _holdTime;
            }
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            Vector2 direction = _launchDirection.normalized;
            float force = _holdTime * forceMultiplier;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            _isCharging = false;
            _canShoot = false;

            Invoke("HideSlider", 2f);
            directionLine.enabled = false;

            strokeCount++;
            UpdateStrokeCounter();

            // Actualizar los strokes totales en GameManager
            GameManager.Instance.AddStrokes(1); // Solo añadir 1 stroke cada vez
        }

        if (!_canShoot && rb.velocity.magnitude < 0.1f)
        {
            ShowSlider();
        }

        // Actualiza el temporizador
        UpdateTimer();
    }

    void ShowDirectionLine(Vector2 targetPosition)
    {
        _launchDirection = ((Vector2)transform.position - targetPosition).normalized;
        directionLine.SetPosition(0, transform.position);
        directionLine.SetPosition(1, (Vector2)transform.position + _launchDirection * lineLength);
    }

    void HideSlider()
    {
        if (forceSlider != null)
        {
            forceSlider.gameObject.SetActive(false);
        }
    }

    void ShowSlider()
    {
        _canShoot = true;
        if (forceSlider != null)
        {
            forceSlider.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            if (rb.velocity.magnitude < minSpeedToWin)
            {
                // Esperar 2 segundos antes de informar al GameManager
                Invoke("NotifyGameManagerAndLoadNextLevel", 2f);
            }
        }
    }

    private void NotifyGameManagerAndLoadNextLevel()
    {
        // Reiniciar el contador de strokes y el temporizador
        strokeCount = 0;
        timeRemaining = 60f; // Resetear el temporizador

        // Cargar el siguiente nivel con la posición de la pelota
        GameManager.Instance.LoadLevel2(ballStartPosition);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateStrokeCounter()
    {
        if (strokeCounterText != null)
        {
            strokeCounterText.text = "Strokes: " + strokeCount.ToString();
        }
    }

    private void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerText();

            // Si el tiempo llega a 0, mostrar pantalla de derrota y detener el juego
            GameManager.Instance.uiCanvas.SetActive(true);
            GameManager.Instance.uiCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Time's Up!"; // Texto de derrota
            Time.timeScale = 0f;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = (int)(timeRemaining / 60); // Calcula los minutos
            int seconds = (int)(timeRemaining - (minutes * 60)); // Calcula los segundos restantes
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }
}
