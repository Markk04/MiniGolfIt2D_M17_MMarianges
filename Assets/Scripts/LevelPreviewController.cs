using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelPreviewController : MonoBehaviour
{
    public Image levelImage;          
    public Sprite[] levelSprites;     // Array amb les imatges de previsualització dels nivells
    public float displayTime = 2f;    
    public float fadeDuration = 0.5f; 

    private CanvasGroup canvasGroup;  // Component per controlar la transparència

    void Awake()
    {
        if (levelImage != null)
            canvasGroup = levelImage.GetComponent<CanvasGroup>();
    }

    // Mètode per mostrar la imatge del nivell actual
    public void ShowLevelImage()
    {
        if (levelImage == null || levelSprites == null || levelSprites.Length == 0 || canvasGroup == null) return;

        // Obtenir l'índex del nivell actual des del GameManager
        int index = GameManager.Instance != null ? GameManager.Instance.GetCurrentLevelIndex() : 0;

        // Assegurar-se que l'índex és vàlid
        if (index >= 0 && index < levelSprites.Length)
        {
            levelImage.sprite = levelSprites[index];
            StopAllCoroutines(); 
            StartCoroutine(ShowWithFade()); 
        }
    }

    // Coroutine per mostrar la imatge amb efecte de fade
    private IEnumerator ShowWithFade()
    {
        // Inicialitzar transparent
        canvasGroup.alpha = 0f;
        levelImage.gameObject.SetActive(true);

        // Fade In (aparició progressiva)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Assegurar opacitat completa
        yield return new WaitForSeconds(displayTime);

        // Fade Out (desaparició progressiva)
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f; // Assegurar transparència completa
        levelImage.gameObject.SetActive(false);
    }
}