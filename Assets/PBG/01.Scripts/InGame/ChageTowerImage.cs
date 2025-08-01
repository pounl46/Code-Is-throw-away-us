using UnityEngine;

public class ChageTowerImage : MonoBehaviour
{
    [SerializeField] private GameObject attackTower;
    [SerializeField] private GameObject moneyTower;

    void Start()
    {
        moneyTower.SetActive(false);
        attackTower.SetActive(true);
    }


    public void ChangeAttack()
    {
        moneyTower.SetActive(false);
        attackTower.SetActive(true);
    }

    public void ChangeMoney()
    {
        attackTower.SetActive(false);
        moneyTower.SetActive(true);
    }
}
