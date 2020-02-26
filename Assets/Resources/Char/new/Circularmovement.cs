using UnityEngine;

public class Circularmovement : MonoBehaviour
{
    public float turnMultiplier = 1.6f;
    public float timeCounter = 0;
    public Vector3 vec;

    void Start()
    {

    }

    void Update()
    {
        {

            timeCounter += Time.deltaTime* turnMultiplier ;

            vec = new Vector3(0, -timeCounter, 0);

            transform.rotation = Quaternion.Euler(vec);

        }
    }
}

