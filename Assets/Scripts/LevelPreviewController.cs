using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelPreviewController : MonoBehaviour
{
    public Image levelImage;          
    public Sprite[] levelSprites;     // Array amb les imatges de previsualitzaci� dels nivells
    public float displayTime = 2f;    
    public float fadeDuration = 0.5f; 

    private CanvasGroup canvasGroup;  // Component per controlar la transpar�ncia

    void Awake()
    {
        if (levelImage != null)
            canvasGroup = levelImage.GetComponent<CanvasGroup>();
    }

    // M�tode per mostrar la imatge del nivell actual
    public void ShowLevelImage()
    {
        if (levelImage == null || levelSprites == null || levelSprites.Length == 0 || canvasGroup == null) return;

        // Obtenir l'�ndex del nivell actual des del GameManager
        int index = GameManager.Instance != null ? GameManager.Instance.GetCurrentLevelIndex() : 0;

        // Assegurar-se que l'�ndex �s v�lid
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

        // Fade In (aparici� progressiva)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Assegurar opacitat completa
        yield return new WaitForSeconds(displayTime);

        // Fade Out (desaparici� progressiva)
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f; // Assegurar transpar�ncia completa
        levelImage.gameObject.SetActive(false);
    }
}