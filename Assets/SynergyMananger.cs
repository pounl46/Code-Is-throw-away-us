using System.Collections.Generic;
using UnityEngine;

public class SynergyMananger : MonoBehaviour
{
    public static SynergyMananger Instance { get; set; }

    private List<GameObject> synergys = new List<GameObject>();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    public bool isInSynergy(GameObject obj)
    {
        for (int i = 0; i < synergys.Count; i++)
        {
            if (obj == synergys[i])
            {
                return true;
            }
        }
        return false;
    }

    public void AddList(GameObject obj)
    {
        synergys.Add(obj);
    }

    public void RemoveList(GameObject obj)
    {
        synergys.Remove(obj);
    }
}
