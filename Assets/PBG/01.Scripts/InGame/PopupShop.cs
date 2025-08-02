using UnityEngine;
using DG.Tweening;


public class PopupShop : MonoBehaviour
{
    [SerializeField] private GameObject popUP;
    private bool isOpend = false;

    public void OpenShop()
    {
        if (!isOpend)
        {
            popUP.GetComponent<RectTransform>().DOMoveY(1, 1f);
            isOpend = true;
        }
        else
        {
             popUP.GetComponent<RectTransform>().DOMoveY(-1f, 1f);
            isOpend = false;
        }
    }
}
