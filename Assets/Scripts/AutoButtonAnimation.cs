using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Автоматически добавляет анимацию ко всем кнопкам в сцене
/// Добавьте этот компонент на Canvas или любой GameObject в сцене
/// </summary>
public class AutoButtonAnimation : MonoBehaviour
{
    [Range(1f, 1.3f)] public float animScale = 1.08f;
    [Range(0.03f, 0.2f)] public float animDuration = 0.08f;

    private void Start()
    {
        // Находим все кнопки в сцене
        Button[] buttons = FindObjectsOfType<Button>(true);
        
        foreach (Button button in buttons)
        {
            // Проверяем, есть ли уже ButtonAnimation
            if (button.GetComponent<ButtonAnimation>() == null)
            {
                // Добавляем компонент анимации
                ButtonAnimation anim = button.gameObject.AddComponent<ButtonAnimation>();
                anim.animScale = animScale;
                anim.animDuration = animDuration;
            }
        }
    }
}


