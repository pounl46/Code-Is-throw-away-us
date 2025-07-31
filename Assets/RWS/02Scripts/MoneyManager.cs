using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    [field: SerializeField] public int Money { get; private set; } = 0;
    [field: SerializeField] public TextMeshPro MoneyText { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ModifyMoney(int value)
    {
        Money += value;
        Debug.Log(value);
        MoneyText.text = Money.ToString();
    }
}
