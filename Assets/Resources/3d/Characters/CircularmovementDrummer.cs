using UnityEngine;

public class CircularmovementDrummer : MonoBehaviour
{
    public float turnMultiplier = 2.4f;
    public float movementMultiplier = 0.0405f;
    public float rotationCounter = 0;
    public float movementCounter = 0;
    public float radius = 3.2f;
    float rotationOffset = -2.2f;
    public float movementOffset;
    Vector3 rotVect;
    Vector3 movementVect;


    void Start()
    {
        rotationOffset = this.transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        {

            {
                rotationCounter += Time.deltaTime * turnMultiplier;
                rotVect = new Vector3(0, rotationOffset - rotationCounter, 0);
                transform.rotation = Quaternion.Euler(rotVect);
            }

            {

                movementCounter += Time.deltaTime * movementMultiplier;

                float x = -0.1f + Mathf.Cos(movementCounter+ movementOffset) *radius;
                float z = -0.5f + Mathf.Sin(movementCounter+ movementOffset) *radius;

                movementVect = new Vector3(x, 0, z);
                transform.position = movementVect;
            }



        }
    }
}

