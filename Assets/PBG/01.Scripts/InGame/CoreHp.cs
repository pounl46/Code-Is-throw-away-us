using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using DG.Tweening;

public class CoreHp : MonoBehaviour
{
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpImage1;
    [SerializeField] private Image hpImage2;
    [SerializeField] private Image hpImage3;
    [SerializeField] private Image hpImage4;

    [SerializeField] private Image lowHpImage;
    [SerializeField] private Image defaultHpImage;

    [SerializeField] private Camera cam;

    

    private int hp = 5;

    void Awake()
    {
        hp = 5;
        hpImage = defaultHpImage;
        hpImage1 = defaultHpImage;
        hpImage2 = defaultHpImage;
        hpImage3 = defaultHpImage;
        hpImage4 = defaultHpImage;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Invoke("CoreHearted", 0.2f);
        }
    }




    private void CoreHearted()
    {
        cam.transform.DOShakePosition(2, 0.2f);

        if (hp == 5)
        {
            TowerGridManager.Instance.EasyCreate();
            TowerGridManager.Instance.HardCreate();
            hpImage4 = lowHpImage;
            hp--;
        }

        else if (hp == 4)
        {
            TowerGridManager.Instance.EasyCreate();
            TowerGridManager.Instance.HardCreate();
            hpImage3 = lowHpImage;
            hp--;
        }

        else if (hp == 3)
        {
            TowerGridManager.Instance.HardCreate();
            TowerGridManager.Instance.EasyLimit();
            hpImage2 = lowHpImage;
            hp--;
        }

        else if (hp == 2)
        {
            TowerGridManager.Instance.HardCreate();
            TowerGridManager.Instance.EasyLimit();
            hpImage1 = lowHpImage;
            hp--;
        }

        else if (hp == 1)
        {
            TowerGridManager.Instance.EasyLimit();
            TowerGridManager.Instance.HardLimit();
            hpImage = lowHpImage;
            hp--;
            End();
        }
    }


    public void End()
    {
        //엔딩 호출
    }

    public void HpAdd()
    {
        hp++;
        CoreHearted();
    }





}
