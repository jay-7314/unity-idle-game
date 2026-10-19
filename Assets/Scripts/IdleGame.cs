using UnityEngine;
using TMPro;

public class IdleGame : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    private double gold;

    private void Start()
    {
        Refresh();
    }

    public void OnTap()
    {
        gold += 1;
        Refresh();
    }

    private void Refresh()
    {
        goldText.text = "°ñµå : " + gold.ToString("N0");
    }
}