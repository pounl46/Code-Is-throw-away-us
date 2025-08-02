using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour
{
    private void Start()
    {
        transform.DOScale(1f, 0.5f);
    }

    public void SelectedButton()
    {
        transform.DOScale(1.2f, 0.5f);
    }

    public void ExitButton()
    {
        transform.DOScale(1f, 0.5f);
    }
}
