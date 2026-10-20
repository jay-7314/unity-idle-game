using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public class IdleGame : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private RectTransform spawnParent;
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private UpgradeCard[] cards;
    [SerializeField] private GameObject offlinePopup;
    [SerializeField] private TMP_Text offlineRewardText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tapClip;
    [SerializeField] private AudioClip buyClip;

    private double gold;
    private double tapPower = 1;
    private double incomePerSecond;
    private float incomeTimer;
    private float saveTimer;
    private const string SaveKey = "idle_save";
    private const long MaxOfflineSeconds = 8 * 60 * 60;

    private readonly Dictionary<UpgradeData, int> levels = new Dictionary<UpgradeData, int>();
    private ObjectPool<FloatingText> pool;

    private void Awake()
    {
        pool = new ObjectPool<FloatingText>(
            createFunc: () => Instantiate(floatingTextPrefab, spawnParent),
            actionOnGet: t => t.gameObject.SetActive(true),
            actionOnRelease: t => t.gameObject.SetActive(false),
            actionOnDestroy: t => Destroy(t.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 30);
    }

    private void Start()
    {
        offlinePopup.SetActive(false);
        foreach (UpgradeCard card in cards)
        {
            levels[card.Data] = 0;
            card.Setup(TryBuy);
        }
        Load();
        Refresh();
    }

    private void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= 10f)
        {
            saveTimer = 0f;
            Save();
        }
        if (incomePerSecond <= 0) return;

        incomeTimer += Time.deltaTime;
        if (incomeTimer >= 1f)
        {
            incomeTimer -= 1f;
            gold += incomePerSecond;
            Refresh();
        }
    }

    public void OnTap()
    {
        gold += tapPower;
        audioSource.PlayOneShot(tapClip, 0.6f);
        Refresh();
        SpawnFloatingText("+" + tapPower.ToString("N0"));
    }

    private void TryBuy(UpgradeData data)
    {
        int level = levels[data];
        double cost = data.GetCost(level);
        if (gold < cost) return;

        gold -= cost;
        audioSource.PlayOneShot(buyClip);
        levels[data] = level + 1;

        if (data.Type == UpgradeType.TapPower)
            tapPower += data.ValuePerLevel;
        else
            incomePerSecond += data.ValuePerLevel;

        Refresh();
    }

    private void SpawnFloatingText(string text)
    {
        FloatingText floating = pool.Get();
        Vector2 start = spawnPoint.anchoredPosition + new Vector2(Random.Range(-80f, 80f), 120f);
        floating.Play(text, start, pool.Release);
    }

    private void Refresh()
    {
        goldText.text = "골드: " + gold.ToString("N0");

        foreach (UpgradeCard card in cards)
        {
            int level = levels[card.Data];
            double cost = card.Data.GetCost(level);
            card.Refresh(level, cost, gold >= cost);
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void Save()
    {
        SaveData data = new SaveData();
        data.gold = gold;
        foreach (KeyValuePair<UpgradeData, int> pair in levels)
        {
            data.upgradeKeys.Add(pair.Key.name);
            data.upgradeLevels.Add(pair.Value);
        }
        data.lastSavedUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
        gold = data.gold;

        for (int i = 0; i < data.upgradeKeys.Count; i++)
        {
            foreach (UpgradeCard card in cards)
            {
                if (card.Data.name == data.upgradeKeys[i])
                    levels[card.Data] = data.upgradeLevels[i];
            }
        }

        tapPower = 1;
        incomePerSecond = 0;
        foreach (KeyValuePair<UpgradeData, int> pair in levels)
        {
            double total = pair.Key.ValuePerLevel * pair.Value;
            if (pair.Key.Type == UpgradeType.TapPower) tapPower += total;
            else incomePerSecond += total;
        }
        long elapsed = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() - data.lastSavedUnix;
        if (elapsed < 10 || incomePerSecond <= 0) return;

        elapsed = System.Math.Min(elapsed, MaxOfflineSeconds);
        double reward = incomePerSecond * elapsed;
        gold += reward;
        offlineRewardText.text = "자리를 비운 동안\n+" + reward.ToString("N0") + " 골드";
        offlinePopup.SetActive(true);
    }
    public void ClosePopup()
    {
        offlinePopup.SetActive(false);
    }
}