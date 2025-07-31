using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class MoneyTower : MonoBehaviour
{
    [Header("SO")]
    [SerializeField] private MoneyTowerSO towerSO;
    [SerializeField] private int extraMoney = 0;

    [Header("Itself")]
    [field : SerializeField] public SpriteRenderer Renderer { get; private set; }
    [field : SerializeField] public LayerMask WhatIsTower { get; private set; }

    [Header("Event")]
    public UnityEvent OnGainMoney;

    [Header("Else")]
    [field: SerializeField] public Transform CoolTime { get; private set; }
    [field: SerializeField] public float CoolTimeXSize { get; private set; }

    public float CurrentTime { get; private set; } = 0;
    private List<Vector2> _dirs = new();
    private int _detectCount = 0;
    private bool _isDetected = false;

    private void Awake()
    {
        InitTower();
        _dirs = GetDirectionVectors();
    }

    private void Start()
    {
        Detect();
    }

    private void Update()
    {
        if (CurrentTime >= towerSO.WaitTime)
        {
            CurrentTime -= towerSO.WaitTime;
            CoolTime.localScale = Vector3.zero;
            GainMoney();
        }
        else
        {
            CurrentTime += Time.deltaTime;
            CoolTime.localScale = new Vector3(Mathf.Lerp(0, CoolTimeXSize, Mathf.InverseLerp(0f, towerSO.WaitTime, CurrentTime)), 0.2f, 1);
        }
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
        _isDetected = _detectCount == towerSO.GetDirectionCount();

    }
    public void GainMoney()
    {
        OnGainMoney?.Invoke();
        MoneyManager.Instance.ModifyMoney(Mathf.FloorToInt((towerSO.Money + extraMoney) * (_isDetected ? towerSO.MoneyMultiplier : 1)));
    }

    public List<Vector2> GetDirectionVectors()
    {
        _dirs.Clear();

        if ((towerSO.Direction & Direction8.Up)         != 0) _dirs.Add(Vector2.up * towerSO.Distance);
        if ((towerSO.Direction & Direction8.UpRight)    != 0) _dirs.Add(new Vector2(1, 1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Right)      != 0) _dirs.Add(Vector2.right * towerSO.Distance);
        if ((towerSO.Direction & Direction8.DownRight)  != 0) _dirs.Add(new Vector2(1, -1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Down)       != 0) _dirs.Add(Vector2.down * towerSO.Distance);
        if ((towerSO.Direction & Direction8.DownLeft)   != 0) _dirs.Add(new Vector2(-1, -1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Left)       != 0) _dirs.Add(Vector2.left * towerSO.Distance);
        if ((towerSO.Direction & Direction8.UpLeft)     != 0) _dirs.Add(new Vector2(-1, 1) * towerSO.Distance);

        return _dirs;
    }

    [ContextMenu("InitTower")]
    public void InitTower()
    {
        Renderer.sprite = towerSO.Sprite;
    }
}
