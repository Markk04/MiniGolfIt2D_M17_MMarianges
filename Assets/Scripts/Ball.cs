using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Ball : MonoBehaviour
{
    // Variables de control del llançament
    private Vector2 _startMousePosition;  
    private float _holdTime;              
    private bool _isCharging = false;     
    private bool _canShoot = true;        

    // Configuració física
    public float forceMultiplier = 10f;   
    public float maxHoldTime = 1f;        
    private Rigidbody2D rb;              
    public float minSpeedToWin = 3f;     

    // Elements d'UI
    public Slider forceSlider;            
    public LineRenderer directionLine;    
    public float lineLength = 2f;         
    private Vector2 _launchDirection;     

    // Control de posicions
    private Vector3 lastPositionBeforeShot; // Última posició abans de llançar

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPositionBeforeShot = transform.position;
    }

    private IEnumerator Start()
    {
        yield return null; // Espera un frame per carregar la UI

        if (directionLine == null)
        {
            directionLine = GetComponentInChildren<LineRenderer>();
            if (directionLine == null)
                Debug.LogError("LineRenderer no trobat.");
        }

        if (forceSlider == null)
            forceSlider = FindObjectOfType<Slider>();

        if (forceSlider != null)
        {
            forceSlider.value = 0f;
            forceSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Pausa o temps esgotat
        if (Time.timeScale == 0f) return;
        if (GameManager.Instance != null && GameManager.Instance.GetTimeRemaining() <= 0f)
            return;

        // Obtenció de la posició del ratolí
        Vector2 mousePosition = GetValidMousePosition();
        if (mousePosition == Vector2.zero) return;

        // Mostra la línia de direcció mentre es carrega
        if (_isCharging && directionLine != null)
        {
            ShowDirectionLine(mousePosition);
        }

        // Inici de càrrega
        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            StartCharging(mousePosition);
        }

        // Control del temps de càrrega
        if (_isCharging)
        {
            UpdateChargeTime();
        }

        // Alliberament i llançament
        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            LaunchBall();
        }

        // Permetre nou llançament quan la bola està quieta
        if (!_canShoot && rb.velocity.magnitude < 0.1f)
        {
            ShowSlider();
        }
    }

    Vector2 GetValidMousePosition()
    {
        if (Camera.main != null && Input.mousePresent)
        {
            Vector3 mousePos = Input.mousePosition;
            // Comprova si el ratolí està dins de la pantalla
            if (mousePos.x >= 0 && mousePos.y >= 0 &&
                mousePos.x <= Screen.width && mousePos.y <= Screen.height)
            {
                return Camera.main.ScreenToWorldPoint(mousePos);
            }
        }
        return Vector2.zero;
    }

    void StartCharging(Vector2 mousePosition)
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

    void UpdateChargeTime()
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

    void LaunchBall()
    {
        Vector2 direction = _launchDirection.normalized;
        float force = _holdTime * forceMultiplier;

        lastPositionBeforeShot = transform.position;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        _isCharging = false;
        _canShoot = false;

        Invoke("HideSlider", 2f);
        directionLine.enabled = false;

        GameManager.Instance.AddStrokes(1);
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
            forceSlider.value = 0f;
            forceSlider.gameObject.SetActive(false);
        }
    }

    void ShowSlider()
    {
        _canShoot = true;
        if (forceSlider != null)
        {
            forceSlider.value = 0f;
            forceSlider.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            if (rb.velocity.magnitude < minSpeedToWin)
            {
                Invoke("NotifyGameManagerAndLoadNextLevel", 2f);
            }
        }
    }

    private void NotifyGameManagerAndLoadNextLevel()
    {
        if (GameManager.Instance.GetCurrentLevelIndex() >= GameManager.Instance.levelNames.Length - 1)
        {
            // Últim nivell
            GameManager.Instance.ShowGameEndScreen();
        }
        else
        {
            GameManager.Instance.ShowHoleImageAndLoadNext();
        }
    }

    public void ResetLevelDataWithoutMovingBall()
    {
        _canShoot = true;
        _isCharging = false;

        if (forceSlider != null)
        {
            forceSlider.value = 0f;
            forceSlider.gameObject.SetActive(false);
        }
    }

    public void ResetToLastPosition()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = lastPositionBeforeShot;
    }
}