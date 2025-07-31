using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MoneyTower : MonoBehaviour
{
    [SerializeField] private MoneyTowerSO towerSO;
    [field : SerializeField] public SpriteRenderer Renderer { get; private set; }

    [field : SerializeField] public float WaitTime { get; private set; }
    public float CurrentTime { get; private set; } = 0;

    private void Awake()
    {
        InitTower();
    }

    private void Update()
    {
        if (CurrentTime >= WaitTime)
        {
            CurrentTime = 0;
            GainMoney();
        }
        else
        {
            CurrentTime += Time.deltaTime;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Vector2 vector in GetDirectionVectors())
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)vector);
        }
    }
#endif

    public void GainMoney()
    {
        MoneyManager.Instance.GainMoney(towerSO.Money);
    }

    public List<Vector2> GetDirectionVectors()
    {
        List<Vector2> vectors = new();

        if ((towerSO.Direction & Direction8.Up) != 0) vectors.Add(Vector2.up * towerSO.Distance);
        if ((towerSO.Direction & Direction8.UpRight) != 0) vectors.Add(new Vector2(1, 1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Right) != 0) vectors.Add(Vector2.right * towerSO.Distance);
        if ((towerSO.Direction & Direction8.DownRight) != 0) vectors.Add(new Vector2(1, -1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Down) != 0) vectors.Add(Vector2.down * towerSO.Distance);
        if ((towerSO.Direction & Direction8.DownLeft) != 0) vectors.Add(new Vector2(-1, -1) * towerSO.Distance);
        if ((towerSO.Direction & Direction8.Left) != 0) vectors.Add(Vector2.left * towerSO.Distance);
        if ((towerSO.Direction & Direction8.UpLeft) != 0) vectors.Add(new Vector2(-1, 1) * towerSO.Distance);

        return vectors;
    }

    [ContextMenu("InitTower")]
    public void InitTower()
    {
        try
        {
            Renderer.sprite = towerSO.Sprite;
        }

        catch
        {
            Debug.Log("error");
        }
    }
}
