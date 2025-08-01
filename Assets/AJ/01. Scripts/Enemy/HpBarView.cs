using UnityEngine;
using UnityEngine.UI;

public class HpBarView : MonoBehaviour
{
    public float steeringValue;

    private RectTransform mViewRectTransform;
    private Slider mSlider;

    public void Start()
    {
        gameObject.SetActive(false);
        mViewRectTransform = GetComponent<RectTransform>();
        mSlider = GetComponent<Slider>();
    }
    public void SetPosition(Vector3 position)
    {
        mViewRectTransform.position = position + Vector3.up * steeringValue;
    }

    public void SetSliderRatio(float ratio)
    {
        mSlider.value = ratio;
    }
}
