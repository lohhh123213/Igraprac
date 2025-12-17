using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Upgrade
{
    public string id;
    public string name;
    public int cost;
    public int multiplier;
}

public class UpgradeManager : MonoBehaviour
{
    public Upgrade[] upgrades;
    public Transform upgradeContainer;
    public GameObject upgradeButtonPrefab;
    public AudioSource audioSource;
    public AudioClip purchaseSfx;

    private Dictionary<string, Upgrade> upgradeDict;
    private HashSet<string> ownedUpgrades = new HashSet<string>();
    private Dictionary<string, GameObject> buttonDict = new Dictionary<string, GameObject>();

    private void Start()
    {
        upgradeDict = new Dictionary<string, Upgrade>();
        foreach (var upgrade in upgrades)
            upgradeDict[upgrade.id] = upgrade;
        CreateUpgradeButtons();
    }

    private void CreateUpgradeButtons()
    {
        if (upgradeContainer == null || upgradeButtonPrefab == null) return;

        foreach (var upgrade in upgrades)
        {
            var buttonObj = Instantiate(upgradeButtonPrefab, upgradeContainer);
            buttonDict[upgrade.id] = buttonObj;

            var texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = upgrade.name;
                texts[1].text = $"Стоимость: {upgrade.cost}";
            }

            buttonObj.GetComponent<Button>()?.onClick.AddListener(() => BuyUpgrade(upgrade.id));
            UpdateButton(upgrade.id);
        }
    }

    public void BuyUpgrade(string upgradeId)
    {
        if (!upgradeDict.TryGetValue(upgradeId, out var upgrade) || ownedUpgrades.Contains(upgradeId))
            return;

        var controller = FindObjectOfType<GameController>();
        if (controller == null || !controller.SpendCoins(upgrade.cost)) return;

        ownedUpgrades.Add(upgradeId);
        controller.SetClickMultiplier(GetTotalMultiplier());
        UpdateButton(upgradeId);
        PlayPurchaseSfx();
    }

    private void UpdateButton(string upgradeId)
    {
        if (!buttonDict.TryGetValue(upgradeId, out var buttonObj)) return;

        var button = buttonObj.GetComponent<Button>();
        var isOwned = ownedUpgrades.Contains(upgradeId);
        
        if (button) button.interactable = !isOwned;

        var texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2 && isOwned)
            texts[1].text = "Куплено";
    }

    public List<string> GetOwnedUpgrades() => new List<string>(ownedUpgrades);

    public void LoadUpgrades(List<string> upgradeIds)
    {
        ownedUpgrades = new HashSet<string>(upgradeIds);
        foreach (var id in upgradeIds)
            UpdateButton(id);

        var controller = FindObjectOfType<GameController>();
        controller?.SetClickMultiplier(GetTotalMultiplier());
    }

    public int GetTotalMultiplier()
    {
        int total = 1;
        foreach (var id in ownedUpgrades)
        {
            if (upgradeDict.TryGetValue(id, out var upgrade))
                total *= upgrade.multiplier;
        }
        return total;
    }

    private void PlayPurchaseSfx()
    {
        if (purchaseSfx == null) return;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(purchaseSfx);
    }
}

