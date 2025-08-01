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
            popUP.transform.DOMoveY(250, 1f);
            isOpend = true;
        }
        else
        {
            popUP.transform.DOMoveY(0, 1f);
            isOpend = false;
        }
    }
}
