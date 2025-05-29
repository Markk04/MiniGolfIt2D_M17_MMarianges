using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelPreviewController : MonoBehaviour
{
    public Image levelImage;
    public Sprite[] levelSprites;
    public float displayTime = 2f;
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (levelImage != null)
            canvasGroup = levelImage.GetComponent<CanvasGroup>();
    }

    public void ShowLevelImage()
    {
        if (levelImage == null || levelSprites == null || levelSprites.Length == 0 || canvasGroup == null) return;

        int index = GameManager.Instance != null ? GameManager.Instance.GetCurrentLevelIndex() : 0;

        if (index >= 0 && index < levelSprites.Length)
        {
            levelImage.sprite = levelSprites[index];
            StopAllCoroutines();
            StartCoroutine(ShowWithFade());
        }
    }

    private IEnumerator ShowWithFade()
    {
        canvasGroup.alpha = 0f;
        levelImage.gameObject.SetActive(true);

        // Fade In
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
        levelImage.gameObject.SetActive(false);
    }
}
