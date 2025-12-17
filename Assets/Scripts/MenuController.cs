using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public Button saveButton;
    public Button loadButton;
    public Button quitButton;
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        playButton?.onClick.AddListener(PlayGame);
        saveButton?.onClick.AddListener(SaveProgress);
        loadButton?.onClick.AddListener(LoadProgress);
        quitButton?.onClick.AddListener(QuitGame);

        // Добавляем звуки на кнопки
        AddButtonSounds();

        // Запускаем фоновую музыку меню
        if (AudioManager.Instance.menuMusic != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        }

        if (SaveManager.Instance.SaveFileExists())
        {
            var data = SaveManager.Instance.LoadGame();
            if (playerNameInput && !string.IsNullOrEmpty(data.playerName))
                playerNameInput.text = data.playerName;
        }

        UpdateStatus("Готов к игре");
    }

    private void AddButtonSounds()
    {
        if (playButton != null)
            playButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (saveButton != null)
            saveButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (loadButton != null)
            loadButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (quitButton != null)
            quitButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
    }

    public void QuitGame()
    {
        // Сохраняем прогресс перед выходом
        if (SaveManager.Instance.SaveFileExists())
        {
            var data = SaveManager.Instance.LoadGame();
            if (playerNameInput && !string.IsNullOrEmpty(playerNameInput.text))
                data.playerName = playerNameInput.text;
            SaveManager.Instance.SaveGame(data);
        }

#if UNITY_EDITOR
        // В редакторе Unity
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // В собранной игре
        Application.Quit();
#endif
    }

    public void PlayGame()
    {
        string newPlayerName = playerNameInput && !string.IsNullOrEmpty(playerNameInput.text.Trim())
            ? playerNameInput.text.Trim()
            : "Player";

        GameData data;
        
        // Проверяем, есть ли сохранение и совпадает ли имя
        if (SaveManager.Instance.SaveFileExists())
        {
            data = SaveManager.Instance.LoadGame();
            
            // Если имя игрока изменилось - сбрасываем прогресс
            if (data.playerName != newPlayerName)
            {
                // Новый игрок - создаём новое сохранение с нулевыми значениями
                data = new GameData
                {
                    playerName = newPlayerName,
                    coins = 0,
                    ownedUpgrades = new System.Collections.Generic.List<string>()
                };
            }
            else
            {
                // Тот же игрок - обновляем только имя (на случай, если оно изменилось в поле ввода)
                data.playerName = newPlayerName;
            }
        }
        else
        {
            // Нет сохранения - создаём новое
            data = new GameData
            {
                playerName = newPlayerName,
                coins = 0,
                ownedUpgrades = new System.Collections.Generic.List<string>()
            };
        }
        
        SaveManager.Instance.SaveGame(data);
        SceneManager.LoadScene("Game");
    }

    public void SaveProgress()
    {
        if (!SaveManager.Instance.SaveFileExists())
        {
            UpdateStatus("Нет сохранённого прогресса!");
            return;
        }

        var data = SaveManager.Instance.LoadGame();
        if (playerNameInput && !string.IsNullOrEmpty(playerNameInput.text))
            data.playerName = playerNameInput.text;
        
        SaveManager.Instance.SaveGame(data);
        UpdateStatus("Прогресс сохранён!");
    }

    public void LoadProgress()
    {
        if (!SaveManager.Instance.SaveFileExists())
        {
            UpdateStatus("Файл сохранения не найден!");
            return;
        }

        var data = SaveManager.Instance.LoadGame();
        if (playerNameInput) playerNameInput.text = data.playerName;
        UpdateStatus($"Загружено! Игрок: {data.playerName}, Очки: {data.coins}");
    }

    private void UpdateStatus(string message)
    {
        if (statusText) statusText.text = message;
    }

    private void Update()
    {
        // Выход по клавише Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}

