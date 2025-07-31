using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;
public class Option : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject buttons;
    // private Material material;
    private void Start()
    {
        panel.SetActive(false);
        // material = GetComponent<Material>();
    }
    public void ClickOption()
    {
        panel.SetActive(true);
        panel.transform.DOScaleX(4.8f, 0.6f);
        panel.transform.DOScaleY(14f, 0.6f);
    }

    public void ExitOption()
    {
        panel.transform.DOScaleX(2f, 0.6f);
        panel.transform.DOScaleY(1f, 0.6f);
        Invoke("ViewButton", 0.5f);
    }

    private void ViewButton()
    {
        buttons.transform.DOScaleY(1f, 0.5f);
        panel.SetActive(false);
    }
}
