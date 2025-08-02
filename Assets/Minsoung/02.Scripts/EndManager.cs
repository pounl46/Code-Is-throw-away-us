using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject endPanel; // End 패널 GameObject 참조
    public TextMeshProUGUI finalWaveText;

    private void Start()
    {
        // 시작할 때 End 패널 비활성화
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("EndManager: End Panel이 할당되지 않았습니다!");
        }

        if (finalWaveText == null)
        {
            Debug.LogWarning("EndManager: FinalWave Text UI가 할당되지 않았습니다!");
        }
    }
    public void FinalWave(int waveNumber)
    {
        // End 패널 활성화
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("EndManager: End Panel이 할당되지 않았습니다!");
        }

        // FinalWave 텍스트 업데이트 (웨이브 번호 표시)
        if (finalWaveText != null)
        {
            finalWaveText.text = $"Wave {waveNumber}";
        }
        else
        {
            Debug.LogError("EndManager: FinalWave Text UI가 할당되지 않았습니다!");
        }

        Debug.Log($"EndManager: 웨이브 {waveNumber} - End 패널 활성화");
    }
}
