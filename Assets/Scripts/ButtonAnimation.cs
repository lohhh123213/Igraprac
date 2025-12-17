using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAnimation : MonoBehaviour
{
    [Range(1f, 1.3f)] public float animScale = 1.08f;
    [Range(0.03f, 0.2f)] public float animDuration = 0.08f;

    private Button button;
    private Vector3 originalScale;
    private bool isAnimating;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (button != null)
        {
            // Сохраняем оригинальный scale при включении
            originalScale = transform.localScale;
        }
    }

    private void Start()
    {
        if (button != null && !isAnimating)
        {
            // Добавляем анимацию к существующим listeners
            button.onClick.AddListener(Animate);
        }
    }

    private void Animate()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateButton());
            // Воспроизводим звук клика кнопки
            AudioManager.Instance?.PlayButtonClick();
        }
    }

    private IEnumerator AnimateButton()
    {
        var endScale = originalScale * animScale;
        float t = 0f;
        
        // Увеличение
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;
            transform.localScale = Vector3.Lerp(originalScale, endScale, lerp);
            yield return null;
        }
        
        // Уменьшение
        t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float lerp = t / animDuration;
            transform.localScale = Vector3.Lerp(endScale, originalScale, lerp);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
}

