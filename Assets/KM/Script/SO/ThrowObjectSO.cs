using UnityEngine;

[CreateAssetMenu(fileName = "ThrowObjectSO", menuName = "enemyTypeSetting/ThrowObjectSO")]
public class ThrowObjectSO : ScriptableObject
{
    public string Name;
    public float Speed;
    public Sprite Sprite;
    public float ThrowCount;
    public AudioClip Audio;
    public GameObject HitEffect;
}
