using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script.SO;
using System.Linq;
using UnityEditor;

public class TowerAttack : MonoBehaviour
{
    public static List<GameObject> AllTowers = new List<GameObject>();

    [SerializeField] private ThrowObjectSO _objectSO;
    [SerializeField] private GameObject _baseObj;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask towerLayer;
    public AttakTowerSetting attakTower;
    private TowerSetting attakTowerSetting;
    [SerializeField] private List<SynergyGroup> mySynergyGroups = new();
    private float fireSpeed;
    private float count;
    private string nameObj;
    private bool isCanAttack = true;
    private AudioSource fire;
    public float damage;
    public float BulletSpeed;
    public bool isSlow;
    public bool isSynergyAlready = false;

    public Stack<GameObject> throws = new();

    private void Awake()
    {
        fire = GetComponent<AudioSource>();
        attakTowerSetting = GetComponent<TowerSetting>();
        attakTower = attakTowerSetting._attakTowerSetting;

        fireSpeed = _objectSO.Speed;
        count = _objectSO.ThrowCount;
        nameObj = _objectSO.Name;
        AllTowers.Add(gameObject);

        for (int i = 0; i < 10; i++)
        {
            GameObject throwObj = Instantiate(_baseObj, transform);
            throwObj.name = nameObj;
            throwObj.SetActive(false);
            throwObj.GetComponent<SpriteRenderer>().sprite = _objectSO.Sprite;
            throws.Push(throwObj);
        }
    }
    private void FixedUpdate()
    {
        Collider2D hits = Physics2D.OverlapCircle(transform.position, attakTowerSetting.attackDistance, _targetMask);
        if (hits && isCanAttack)
        {
            isCanAttack = false;
            StartCoroutine(Attacking(hits));
        }
    }

    private IEnumerator Attacking(Collider2D collider)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject throwObj = throws.Pop();
            throwObj.SetActive(true);
            throwObj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            throwObj.transform.position = transform.position;
            throwObj.transform.rotation = Quaternion.LookRotation(collider.transform.position - transform.position) * Quaternion.Euler(0, 90, 90);
            throwObj.GetComponent<Rigidbody2D>().AddForce((Vector2)(collider.transform.position - transform.position) * fireSpeed, ForceMode2D.Impulse);
            throwObj.GetComponent<AudioSource>().clip = _objectSO.Audio;
            _objectSO.HitEffect.GetComponent<AudioSource>().clip = _objectSO.Audio;
            throwObj.GetComponent<ThrowScript>().hitEffect = _objectSO.HitEffect;
            fire.Play();
            yield return new WaitForSeconds(i <= count ? attakTowerSetting.attackDelay / 2 : attakTowerSetting.attackDelay);
        }
        isCanAttack = true;
    }
}