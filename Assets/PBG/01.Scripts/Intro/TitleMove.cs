using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class TitleMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Button button1;
    [SerializeField] private Transform button1Target;
    [SerializeField] private Button button2;
    [SerializeField] private Transform button2Target;
    [SerializeField] private Button button3;
    [SerializeField] private Transform button3Target;

    private void Start()
    {
        StopButtons();
        Buttons();
    }

    private void Buttons()
    {
        transform.DOMove(target.position, 1.5f).SetEase(Ease.OutBounce);
        button1.transform.DOMove(button1Target.position, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.15f);
        button2.transform.DOMove(button2Target.position, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.25f);
        button3.transform.DOMove(button3Target.position, 1.5f).SetEase(Ease.InOutCubic).SetDelay(0.35f);
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
        SceneLoadManager.Instance.LoadScene1();
    }
}
