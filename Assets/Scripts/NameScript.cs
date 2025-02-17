using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    TextMeshPro tMP;


    void Awake()
    {
        tMP = transform.Find("NameField").gameObject.GetComponent<TMPro.TextMeshPro>();
    }

    public void SetPlayerName(string name)
    {
        tMP.text = name;
        tMP.color = new Color32((byte)Random.Range(0, 255),
            (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    public string GetPlayerName()
    {
        return tMP.text;
    }
}
