using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TitleMove : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    private void Start()
    {
        transform.DOMoveY(920, 1.5f).SetEase(Ease.OutBounce);
        button1.transform.DOMoveX(390, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.15f);
        button2.transform.DOMoveX(390, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.25f);
        button3.transform.DOMoveX(390, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.35f);
    }

    
    
}
