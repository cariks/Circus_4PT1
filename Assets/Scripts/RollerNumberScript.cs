using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using TMPro;


public class RollerNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    [SerializeField]
    TMP_Text rolledNumberText;

    void Awake()
    {
        diceRollScript = FindObjectOfType<DiceRollScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (diceRollScript != false)
        {
            if (diceRollScript.isLanded)
                rolledNumberText.text = diceRollScript.difeFaceNum;
            else
                rolledNumberText.text = "?";
        }
        else
            Debug.LogError("DiceRollScript not found in a scene!");
    }
}
