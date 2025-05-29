using UnityEngine;

public class BadHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Ball ballScript = collision.GetComponent<Ball>();
            if (ballScript != null)
            {
                ballScript.ResetToLastPosition();
            }
        }
    }
}
