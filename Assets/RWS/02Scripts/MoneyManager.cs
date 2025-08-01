using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    [field: SerializeField] public int Money { get; private set; } = 0;
    [field: SerializeField] public TextMeshProUGUI MoneyText { get; private set; }  //test
    [field: SerializeField] public AudioClip MoneySound { get; private set; }

    public bool OnOff { get; private set; } = true;
    public List<MoneyTower> MoneyTowers { get; private set; } = new();

    private AudioSource _source;
    private float _lastPlayTime = 0f;
    private float _playCooldown = 0.1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _source = GetComponent<AudioSource>();
    }

    public void AddTowerList(MoneyTower tower)
    {
        if (!MoneyTowers.Contains(tower))
            MoneyTowers.Add(tower);

        foreach (MoneyTower t in MoneyTowers)
        {
            t.Detect();
        }
    }

    public void ModifyMoney(int value)
    {
        Debug.Log($"+ {value}!");
        Money += value;

        if (value > 0)
        {
            PlayMoneySound();
        }

        if (MoneyText != null)
        {
            MoneyText.text = Money.ToString();
        }
    }

    public void MoneyTowerOnOff()
    {
        OnOff = !OnOff;
        foreach (MoneyTower t in MoneyTowers)
        {
            t.SetEnabled(OnOff);
        }
    }

    public void MoneyTowerOnOff(bool value)
    {
        OnOff = value;
        foreach (MoneyTower t in MoneyTowers)
        {
            t.SetEnabled(value);
        }
    }

    public void PlayMoneySound()
    {
        if (Time.time - _lastPlayTime < _playCooldown)
            return;

        _source.pitch = Random.Range(1.4f, 1.2f);
        _source.PlayOneShot(MoneySound);

        _lastPlayTime = Time.time;
    }
}
