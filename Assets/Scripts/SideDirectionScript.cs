using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDirectionScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    private Collider[] allFaces;
    private string landedFace = "";

    private bool hasCheckedLanded = false;

    private void Awake()
    {
        diceRollScript = FindObjectOfType<DiceRollScript>();
        allFaces = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        if (diceRollScript == null) return;

        Rigidbody rb = diceRollScript.GetComponent<Rigidbody>();

        if (rb.velocity.sqrMagnitude < 0.01f && rb.angularVelocity.sqrMagnitude < 0.01f)
        {
            if (!hasCheckedLanded)
            {
                diceRollScript.isLanded = true;
                landedFace = GetLowestFace();

                // Проверяем выпавшую грань только если кубик был брошен вручную
                if (!string.IsNullOrEmpty(landedFace) && diceRollScript.firstThrow)
                {
                    diceRollScript.difeFaceNum = landedFace;
                    Debug.Log("Dice: " + landedFace);
                }
                else
                {
                    diceRollScript.difeFaceNum = "?";
                }

                    hasCheckedLanded = true;
            }
        }
        else
        {
            diceRollScript.isLanded = false;
            hasCheckedLanded = false;
        }
    }

    private string GetLowestFace()
    {
        float lowestY = float.MaxValue;
        string lowestFace = "";

        foreach (Collider face in allFaces)
        {
            if (face.CompareTag("DiceFace"))
            {
                float faceY = face.transform.position.y;

                if (faceY < lowestY)
                {
                    lowestY = faceY;
                    lowestFace = face.name;
                }
            }
        }

        return lowestFace;
    }
}
