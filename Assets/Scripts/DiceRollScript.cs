using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DiceRollScript : MonoBehaviour
{

    Rigidbody rigidbody;
    Vector3 position;
    [SerializeField]
    private float maxRandForceVal, startRollingForce;
    float forceX, forceY, forceZ;
    public string difeFaceNum;
    public bool firstThrow = false;
    public bool isLanded = false;

    [SerializeField] private Button rollButton;

    private void Awake()
    {
        Initialize();
        rollButton.onClick.AddListener(RollDice);
    }

    private void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        position = transform.position;
        transform.rotation = new Quaternion(Random.Range(0, 360),
            Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    public void RollDice()
    {
        if (!isLanded)
        {
            if (firstThrow || isLanded)
                return;

            rigidbody.isKinematic = false;
            forceX = Random.Range(0, maxRandForceVal);
            forceY = Random.Range(0, maxRandForceVal);
            forceZ = Random.Range(0, maxRandForceVal);
            rigidbody.AddForce(Vector3.up * Random.Range(800, startRollingForce));
            rigidbody.AddTorque(forceX, forceY, forceZ);

            firstThrow = true;
        }
    }

    public void ResetDice()
    {
        rigidbody.isKinematic = true;
        firstThrow = false;
        isLanded = false;
        transform.position = position;
    }

    private void Update()
    {
        if (rigidbody != null && !isLanded && firstThrow)
        {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null &&
                        hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                            firstThrow = true;
                        RollDice();
                    }

                }
            }
    }
}