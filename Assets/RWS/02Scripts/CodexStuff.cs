using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexStuff : MonoBehaviour
{
    [field : SerializeField] public GameObject UpLeft { get; private set; }
    [field : SerializeField] public GameObject Up { get; private set; }
    [field : SerializeField] public GameObject UpRight { get; private set; }
    [field : SerializeField] public GameObject Left { get; private set; }
    [field : SerializeField] public GameObject Right { get; private set; }
    [field : SerializeField] public GameObject DownLeft { get; private set; }
    [field : SerializeField] public GameObject Down { get; private set; }
    [field : SerializeField] public GameObject DownRight { get; private set; }
    [field : SerializeField] public GameObject Center { get; private set; }
    [field : SerializeField] public TextMeshProUGUI Name { get; set; }
    [field : SerializeField] public TextMeshProUGUI Cost { get; set; }
    [field : SerializeField] public TextMeshProUGUI Time { get; set; }
    [field : SerializeField] public TextMeshProUGUI Time2 { get; set; }
    [field : SerializeField] public TextMeshProUGUI Money { get; set; }
    [field : SerializeField] public TextMeshProUGUI Money2 { get; set; }
    [field : SerializeField] public MoneyTowerSO SO { get; set; }

    private void OnEnable()
    {
        UpLeft.GetComponent<Image>().sprite = SO.Sprite;
        Up.GetComponent<Image>().sprite = SO.Sprite;
        UpRight.GetComponent<Image>().sprite = SO.Sprite;
        Left.GetComponent<Image>().sprite = SO.Sprite;
        Right.GetComponent<Image>().sprite = SO.Sprite;
        DownLeft.GetComponent<Image>().sprite = SO.Sprite;
        Down.GetComponent<Image>().sprite = SO.Sprite;
        DownRight.GetComponent<Image>().sprite = SO.Sprite;
        Center.GetComponent<Image>().sprite = SO.Sprite;

        if ((SO.Direction & Direction8.Up) == 0) Up.SetActive(false);
        if ((SO.Direction & Direction8.UpRight) == 0) UpRight.SetActive(false);
        if ((SO.Direction & Direction8.Right) == 0) Right.SetActive(false);
        if ((SO.Direction & Direction8.DownRight) == 0) DownRight.SetActive(false);
        if ((SO.Direction & Direction8.Down) == 0) Down.SetActive(false);
        if ((SO.Direction & Direction8.DownLeft) == 0) DownLeft.SetActive(false);
        if ((SO.Direction & Direction8.Left) == 0) Left.SetActive(false);
        if ((SO.Direction & Direction8.UpLeft) == 0) UpLeft.SetActive(false);

        Name.text = SO.TowerName;
        Cost.text = $"Cost : {SO.Cost}";
        Time.text = $"Time : {SO.WaitTime}";
        Money.text = $"Money : {SO.Money}";
        Time2.text = $"Time when activated : {SO.ShorterWaitTime}";
        Money2.text = $"MoneyMultiply when activated : {SO.MoneyMultiplier}";
    }
}
