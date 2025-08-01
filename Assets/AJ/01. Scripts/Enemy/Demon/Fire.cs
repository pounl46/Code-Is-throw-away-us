using UnityEngine;

public class Fire : MonoBehaviour
{
    public ParticleSystem particle;


    private void OnEnable()
    {
        if (particle != null)
        {
            particle.Stop();
            particle.Play();
        }
    }
    private void OnDisable()
    {
        particle.Pause();
    }
    private void OnDestroy()
    {
        particle.Stop();
    }
}
