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

    private double gold;
    private double tapPower = 1;
    private double incomePerSecond;
    private float incomeTimer;

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
        foreach (UpgradeCard card in cards)
        {
            levels[card.Data] = 0;
            card.Setup(TryBuy);
        }
        Refresh();
    }

    private void Update()
    {
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
        Refresh();
        SpawnFloatingText("+" + tapPower.ToString("N0"));
    }

    private void TryBuy(UpgradeData data)
    {
        int level = levels[data];
        double cost = data.GetCost(level);
        if (gold < cost) return;

        gold -= cost;
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
        goldText.text = "°ñµå: " + gold.ToString("N0");

        foreach (UpgradeCard card in cards)
        {
            int level = levels[card.Data];
            double cost = card.Data.GetCost(level);
            card.Refresh(level, cost, gold >= cost);
        }
    }
}