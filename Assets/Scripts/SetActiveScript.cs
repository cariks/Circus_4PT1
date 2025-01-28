using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveScript : MonoBehaviour
{
    public GameObject gameObject2;

   public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCoroutine(delay));
    }

    private IEnumerator ToggleActiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject2.SetActive(!gameObject2.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
