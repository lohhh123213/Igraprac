using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI playerNameText;
    public Button clickButton;
    public Button backToMenuButton;
    public Button saveButton;
    public Button loadButton;
    public UpgradeManager upgradeManager;

    [Header("FX")]
    public AudioSource audioSource;
    public AudioClip clickSfx;
    public AudioClip purchaseSfx;

    private int coins;
    private string playerName = "Player";
    private int clickMultiplier = 1;

    private void Start()
    {
        clickButton?.onClick.AddListener(OnClick);
        backToMenuButton?.onClick.AddListener(BackToMenu);
        saveButton?.onClick.AddListener(SaveProgress);
        loadButton?.onClick.AddListener(LoadProgress);

        // Добавляем звуки на кнопки
        AddButtonSounds();

        // Запускаем фоновую музыку игры
        if (AudioManager.Instance.gameMusic != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
        }

        // Загружаем данные при старте игры
        LoadProgress();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void BackToMenu()
    {
        // Сохраняем прогресс перед возвратом в меню
        SaveProgress();
        SceneManager.LoadScene("Menu");
    }

    private void Update()
    {
        // Выход по клавише Escape (возврат в меню)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    private void AddButtonSounds()
    {
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (saveButton != null)
            saveButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (loadButton != null)
            loadButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
    }

    public void OnClick()
    {
        coins += clickMultiplier;
        UpdateUI();
        
        // Используем AudioManager для звука клика, если есть, иначе старый способ
        if (AudioManager.Instance.buttonClickSfx != null)
            AudioManager.Instance.PlayButtonClick();
        else
            PlaySfx(clickSfx);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public int GetCoins() => coins;

    public void SetClickMultiplier(int multiplier)
    {
        clickMultiplier = multiplier;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText) coinsText.text = $"Очки: {coins}";
        if (playerNameText) playerNameText.text = $"Игрок: {playerName}";
    }

    public void SaveProgress()
    {
        var data = new GameData
        {
            playerName = playerName,
            coins = coins,
            ownedUpgrades = upgradeManager?.GetOwnedUpgrades() ?? new System.Collections.Generic.List<string>()
        };
        SaveManager.Instance.SaveGame(data);
    }

    public void LoadProgress()
    {
        var data = SaveManager.Instance.LoadGame();
        playerName = data.playerName;
        coins = data.coins;
        
        if (upgradeManager != null)
        {
            upgradeManager.LoadUpgrades(data.ownedUpgrades);
            clickMultiplier = upgradeManager.GetTotalMultiplier();
        }
        UpdateUI();
    }

    public void PlayPurchaseSfx()
    {
        PlaySfx(purchaseSfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
    }

}

