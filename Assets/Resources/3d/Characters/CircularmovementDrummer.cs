using UnityEngine;

public class CircularmovementDrummer : MonoBehaviour
{
    public float turnMultiplier = 1.6f;
    public float movementMultiplier = 0;
    public float rotationCounter = 0;
    public float movementCounter = 0;
    public float radius = 0;
    float rotationOffset = 0;
    public float movementOffset;
    Vector3 rotVect;
    Vector3 movementVect;


    void Start()
    {
        rotationOffset = this.transform.rotation.eulerAngles.y;;
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

