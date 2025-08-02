using UnityEngine;
using DG.Tweening;
using TMPro;


public class PopupShop : MonoBehaviour
{
    [SerializeField] private GameObject popUP;
    [SerializeField] private TMP_Text open;
    private bool isOpend = false;

    public void OpenShop()
    {
        if (!isOpend)
        {
            popUP.GetComponent<RectTransform>().DOMoveY(1, 1f);
            open.text = "Close";
            isOpend = true;
        }
        else
        {
             popUP.GetComponent<RectTransform>().DOMoveY(-1f, 1f);
            open.text = "Open";
            isOpend = false;
        }
    }
}
