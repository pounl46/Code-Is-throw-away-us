using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class TitleMove : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private GameObject fadePanel;
    private void Start()
    {
        StopButtons();
        Buttons();
    }

    private void Buttons()
    {
        transform.DOMoveY(3.5f, 1.5f).SetEase(Ease.OutBounce);
        button1.transform.DOMoveX(-6f, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.15f);
        button2.transform.DOMoveX(-6f, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.25f);
        button3.transform.DOMoveX(-6f, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.35f);
    }

    private void StopButtons()
    {
        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
    }

    public void Fade()
    {
        button1.interactable = false;
        button2.interactable = false;
        button3.interactable = false;
        fadePanel.transform.DOMoveX(0.5f, 3f).SetEase(Ease.InExpo);
    }

    
    
}
