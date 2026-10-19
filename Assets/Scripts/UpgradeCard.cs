using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private UpgradeData data;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    public UpgradeData Data => data;

    public void Setup(System.Action<UpgradeData> onClick)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data));
    }

    public void Refresh(int level, double cost, bool canBuy)
    {
        nameText.text = data.DisplayName + " Lv." + level;
        costText.text = cost.ToString("N0");
        button.interactable = canBuy;
    }
}