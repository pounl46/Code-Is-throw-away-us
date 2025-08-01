using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexManager : MonoBehaviour
{
    public static CodexManager Instance { get; private set; }

    [field : SerializeField] public GameObject Codex { get; private set; }
    [field : SerializeField] public GameObject StuffPrefab { get; private set; }
    [field: SerializeField] public List<CodexStruct> CodexDictionary { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToDict(MoneyTowerSO so)
    {
        bool isInDict = false;
        foreach (CodexStruct @struct in CodexDictionary)
        {
            if (so.name == @struct.key.name)
            {
                isInDict = true;
                break;
            }
        }

        if (!isInDict)
        {
            CodexStruct s = new CodexStruct();
            s.key = so;
            s.obj = Instantiate(StuffPrefab, Codex.GetComponentInChildren<ContentSizeFitter>().transform);
            s.stuff = s.obj.GetComponent<CodexStuff>();
            s.stuff.SO = s.key;
            CodexDictionary.Add(s);
        }
    }

    public void CodexOnOff()
    {
        Codex.SetActive(!Codex.activeSelf);
    }
    public void CodexOnOff(bool value)
    {
        Codex.SetActive(value);
    }
}

[Serializable]
public struct CodexStruct
{
    public MoneyTowerSO key;
    public GameObject obj;
    public CodexStuff stuff;
}