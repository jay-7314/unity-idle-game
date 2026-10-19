using System;
using UnityEngine;

public enum UpgradeType
{
    TapPower,
    AutoIncome
}

[CreateAssetMenu(menuName = "Idle/Upgrade Data", fileName = "UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private UpgradeType type;
    [SerializeField] private double baseCost = 10;
    [SerializeField] private double costGrowth = 1.15;
    [SerializeField] private double valuePerLevel = 1;

    public string DisplayName => displayName;
    public UpgradeType Type => type;
    public double ValuePerLevel => valuePerLevel;

    public double GetCost(int level)
    {
        return Math.Round(baseCost * Math.Pow(costGrowth, level));
    }
}