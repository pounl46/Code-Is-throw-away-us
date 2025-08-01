using UnityEngine;
using UnityEngine.UI;

public class HpBarView : MonoBehaviour
{
    public float steeringValue = 0f;
    private RectTransform mViewRectTransform;
    private Slider hpSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;

    private void Awake()
    {
        mViewRectTransform = GetComponent<RectTransform>();

        if (hpSlider == null)
            hpSlider = GetComponentInChildren<Slider>();

        if (hpSlider != null && fillImage == null)
            fillImage = hpSlider.fillRect.GetComponent<Image>();
    }

    public void Start()
    {
        if (hpSlider != null)
        {
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f;
            hpSlider.value = 1f;
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (mViewRectTransform != null)
        {
            mViewRectTransform.position = position + Vector3.up * steeringValue;
        }
    }

    public void SetSliderRatio(float ratio)
    {
        if (hpSlider != null)
        {
            hpSlider.value = Mathf.Clamp01(ratio);
        }
    }
}
