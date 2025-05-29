using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private Vector3 lastPositionBeforeShot;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPositionBeforeShot = transform.position;

        if (directionLine == null)
        {
            directionLine = GetComponentInChildren<LineRenderer>();
            if (directionLine == null)
            {
                Debug.LogError("LineRenderer no encontrado.");
            }
        }

        if (forceSlider == null)
            forceSlider = FindObjectOfType<Slider>();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // Consultar el tiempo restante desde GameManager
        if (GameManager.Instance != null && GameManager.Instance.GetTimeRemaining() <= 0f)
        {
            return; // Tiempo terminado, no permitir acciones
        }

        Vector2 mousePosition = Vector2.zero;

        if (Camera.main != null && Input.mousePresent)
        {
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x >= 0 && mousePos.y >= 0 &&
                mousePos.x <= Screen.width && mousePos.y <= Screen.height)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }

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

            lastPositionBeforeShot = transform.position;

            rb.AddForce(direction * force, ForceMode2D.Impulse);
            _isCharging = false;
            _canShoot = false;

            Invoke("HideSlider", 2f);
            directionLine.enabled = false;

            GameManager.Instance.AddStrokes(1);
        }

        if (!_canShoot && rb.velocity.magnitude < 0.1f)
        {
            ShowSlider();
        }
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
            forceSlider.gameObject.SetActive(false);
    }

    void ShowSlider()
    {
        _canShoot = true;
        if (forceSlider != null)
            forceSlider.gameObject.SetActive(true);
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
        GameManager.Instance.LoadNextLevel();
    }


    public void ResetLevelDataWithoutMovingBall()
    {
        _canShoot = true;
        _isCharging = false;
    }

    public void ResetToLastPosition()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = lastPositionBeforeShot;
    }
}
