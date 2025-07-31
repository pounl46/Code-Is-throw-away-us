using UnityEngine;

public class Tower : MonoBehaviour
{
    public int gridX;
    public int gridY;

    void Start()
    {
        Debug.Log($"포탑이 ({gridX}, {gridY})에 설치되었습니다.");
    }
}
