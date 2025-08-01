using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class MoneyTower : MonoBehaviour
{
    [field : SerializeField, Header("SO")] public MoneyTowerSO TowerSO { get; private set; }
    [SerializeField] private int extraMoney = 0;
    [Header("조건 만족 시 버는 돈 증가")]
    [SerializeField] private bool isMoneyMulty;
    [Header("조건 만족 시 쿨타임 감소")]
    [SerializeField] private bool isShorterTime;

    [field : SerializeField, Header("Itself")] public SpriteRenderer Renderer { get; private set; }
    [field : SerializeField] public LayerMask WhatIsTower { get; private set; }

    [Header("Event")]
    public UnityEvent OnGainMoney;

    [field: SerializeField, Header("Else")] public Transform CoolTime { get; private set; }
    [field: SerializeField] public float CoolTimeXSize { get; private set; }
    [field: SerializeField] public bool IsEnabled { get; private set; }

    public float CurrentTime { get; private set; } = 0;
    private List<Vector2> _dirs = new();
    private int _detectCount = 0;
    private bool _isDetected = false;
    private Coroutine _moneyCoroutine;

    private void OnEnable()
    {
        MoneyManager.Instance.AddTowerList(this);

        InitTower();
        _dirs = GetDirectionVectors();
        Detect();

        SetEnabled(MoneyManager.Instance.OnOff);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color color = Gizmos.color;
        Gizmos.color = Color.red;
        foreach (Vector2 vector in GetDirectionVectors())
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)vector);
        }
        Gizmos.color = color;
    }
#endif

    [ContextMenu("Detect")]
    public void Detect()
    {
        _detectCount = 0;
        foreach (Vector2 vector in _dirs)
        {
            Vector2 startPos = (Vector2)transform.position + vector.normalized * 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(startPos, vector.normalized, vector.magnitude - 0.1f, WhatIsTower);

            if (hit)
            {
                _detectCount++;
            }

            Debug.DrawRay(transform.position, vector.normalized * vector.magnitude, Color.red, 2f);
        }
        _isDetected = _detectCount == TowerSO.GetDirectionCount();

    }

    public void GainMoney()
    {
        OnGainMoney?.Invoke();
        MoneyManager.Instance.ModifyMoney(Mathf.FloorToInt((TowerSO.Money + extraMoney) * (_isDetected && isMoneyMulty ? TowerSO.MoneyMultiplier : 1)));
    }

    public List<Vector2> GetDirectionVectors()
    {
        _dirs.Clear();

        if ((TowerSO.Direction & Direction8.Up)         != 0) _dirs.Add(Vector2.up * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.UpRight)    != 0) _dirs.Add(new Vector2(1, 1) * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.Right)      != 0) _dirs.Add(Vector2.right * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.DownRight)  != 0) _dirs.Add(new Vector2(1, -1) * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.Down)       != 0) _dirs.Add(Vector2.down * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.DownLeft)   != 0) _dirs.Add(new Vector2(-1, -1) * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.Left)       != 0) _dirs.Add(Vector2.left * TowerSO.Distance);
        if ((TowerSO.Direction & Direction8.UpLeft)     != 0) _dirs.Add(new Vector2(-1, 1) * TowerSO.Distance);

        return _dirs;
    }

    private IEnumerator MoneyLoop()
    {
        Detect();
        //CurrentTime = 0;

        while (IsEnabled)
        {
            CurrentTime += Time.deltaTime;

            float t = Mathf.InverseLerp(0f, _isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime, CurrentTime);
            CoolTime.localScale = new Vector3(Mathf.Lerp(0, CoolTimeXSize, t), 0.2f, 1);

            if (CurrentTime >= (_isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime))
            {
                CurrentTime -= _isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime;
                CoolTime.localScale = Vector3.zero;
                GainMoney();
            }

            yield return null;
        }
    }


    public void SetEnabled(bool value)
    {
        if (IsEnabled == value) return;

        IsEnabled = value;

        if (value)
        {
            _moneyCoroutine = StartCoroutine(MoneyLoop());
        }
        else if (_moneyCoroutine != null)
        {
            StopCoroutine(_moneyCoroutine);
            _moneyCoroutine = null;
            //CoolTime.localScale = Vector3.zero;
        }
    }

    [ContextMenu("InitTower")]
    public void InitTower()
    {
        Renderer.sprite = TowerSO.Sprite;
    }
}
