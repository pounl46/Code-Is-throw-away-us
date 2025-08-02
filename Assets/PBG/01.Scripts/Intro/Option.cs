using UnityEngine;
using DG.Tweening;
public class Option : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Ease easeOn;
    [SerializeField] private Ease easeOff;

    private bool _onOff = false;

    public void ClickOption()
    {
        if (_onOff) return;
        panel.transform.DOScale(Vector2.one, 0.6f).SetEase(easeOn);
        _onOff = true;
    }

    public void ExitOption()
    {
        if (!_onOff) return;
        panel.transform.DOScale(Vector2.zero, 0.6f).SetEase(easeOff);
        _onOff = false;
    }
}
