using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour
{
    private void Start()
    {
        transform.DOScaleY(1f, 0.5f);
    }

    public void SelectedButton()
    {
        transform.DOScaleY(1.5f, 0.5f);
    }

    public void ExitButton()
    {
        transform.DOScaleY(1f, 0.5f);
    }
}
