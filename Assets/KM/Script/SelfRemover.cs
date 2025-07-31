using System.Collections;
using UnityEngine;

public class SelfRemover : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Deleteeee());
    }

    private IEnumerator Deleteeee()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
