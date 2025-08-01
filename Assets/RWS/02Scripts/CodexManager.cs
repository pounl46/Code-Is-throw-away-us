using UnityEngine;
using UnityEngine.UI;

public class CodexManager : MonoBehaviour
{
    [SerializeField] private GameObject codex;

    public void CodexOnOff()
    {
        codex.SetActive(!codex.activeSelf);
    }
    public void CodexOnOff(bool value)
    {
        codex.SetActive(value);
    }
}
