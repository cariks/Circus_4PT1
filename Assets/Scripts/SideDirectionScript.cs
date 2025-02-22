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

        Debug.Log($"[SideDirectionScript] Found {allFaces.Length} faces.");
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

                if (!string.IsNullOrEmpty(landedFace) && diceRollScript.firstThrow)
                {
                    diceRollScript.difeFaceNum = landedFace;
                    Debug.Log($"[SideDirectionScript] Dice landed on: {landedFace}");
                }
                else
                {
                    diceRollScript.difeFaceNum = "?";
                    Debug.LogError("[SideDirectionScript] ERROR: Failed to determine landed face.");
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
                Debug.Log($"[GetLowestFace] Checking face {face.name} at Y={faceY}");

                if (faceY < lowestY)
                {
                    lowestY = faceY;
                    lowestFace = face.name;
                }
            }
            else
            {
                Debug.LogWarning($"[GetLowestFace] Face {face.name} does not have tag 'DiceFace'!");
            }
        }

        Debug.Log($"[GetLowestFace] Determined lowest face: {lowestFace}");
        return lowestFace;
    }
}
