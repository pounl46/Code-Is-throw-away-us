using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class MoneyTower : MonoBehaviour
{
    [field : SerializeField, Header("SO")]
    public MoneyTowerSO TowerSO { get; private set; }

    [Header("조건 만족 시 버는 돈 증가")]
    [SerializeField] private bool isMoneyMulty;
    [Header("조건 만족 시 쿨타임 감소")]
    [SerializeField] private bool isShorterTime;

    [field : SerializeField, Header("Itself")] public SpriteRenderer Renderer { get; private set; }
    [field : SerializeField] public LayerMask WhatIsTower { get; private set; }

    [Header("Event")]
    public UnityEvent OnGainMoney;

    [field: SerializeField, Header("Else"), Range(0,4)]
    //public Transform CoolTime { get; private set; }
    //[field: SerializeField] public float CoolTimeXSize { get; private set; }


    public int Level { get; private set; } = 0;
    [field: SerializeField] public int[] LevelCost { get; private set; } = new int[4];
    [field: SerializeField] public int[] LevelUpMoney { get; private set; } = new int[5];

    public bool IsEnabled { get; private set; }

    public float CurrentTime { get; private set; } = 0;
    private List<Vector2> _dirs = new();
    private int _detectCount = 0;
    private bool _isDetected = false;
    private Coroutine _moneyCoroutine;
    private LineRenderer _line;
    private List<Transform> _detectedObj = new();

    private void OnEnable()
    {
        MoneyManager.Instance.AddTowerList(this);

        InitTower();
        _dirs = GetDirectionVectors();

        SetEnabled(MoneyManager.Instance.OnOff);
        _line = GetComponentInChildren<LineRenderer>();
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
        _detectedObj.Clear();
        _detectCount = 0;
        foreach (Vector2 vector in _dirs)
        {
            Vector2 startPos = (Vector2)transform.position + vector.normalized * 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(startPos, vector.normalized, vector.magnitude - 0.1f, WhatIsTower);

            if (hit)
            {
                _detectCount++;
                _detectedObj.Add(hit.transform);
            }

            Debug.DrawRay(transform.position, vector.normalized * vector.magnitude, Color.red, 2f);
        }
        _isDetected = _detectCount == TowerSO.GetDirectionCount();
        if (_isDetected)
        {
            List<Vector3> vs = new();
            foreach (Transform v in _detectedObj)
            {
                vs.Add(Vector3.zero);
                vs.Add(transform.InverseTransformPoint(v.position));

            }
            _line.positionCount = vs.Count;
            _line.SetPositions(vs.ToArray());
            CodexManager.Instance.AddToDict(TowerSO);
        }
    }

    public void LevelUp()
    {
        Level++;
        Level = Math.Clamp(Level, 0, 4);
    }

    public void GainMoney()
    {
        OnGainMoney?.Invoke();
        MoneyManager.Instance.ModifyMoney(Mathf.FloorToInt((TowerSO.Money + LevelUpMoney[Level]) * (_isDetected && isMoneyMulty ? TowerSO.MoneyMultiplier : 1)));
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

            //float t = Mathf.InverseLerp(0f, _isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime, CurrentTime);
            //CoolTime.localScale = new Vector3(Mathf.Lerp(0, CoolTimeXSize, t), 0.2f, 1);

            if (CurrentTime >= (_isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime))
            {
                CurrentTime -= _isDetected && isShorterTime ? TowerSO.ShorterWaitTime : TowerSO.WaitTime;
                //CoolTime.localScale = Vector3.zero;
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
