using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject camCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SwipeInput.swipedLeft)
        {
            camCam.transform.Translate(0,0,1, Space.World);
        }
        if (SwipeInput.swipedRight)
        {
            camCam.transform.Translate(0, 0, -1, Space.World);
        }
        if (SwipeInput.swipedUp)
        {
            camCam.transform.Translate(Vector3.up, Space.World);
        }
        if (SwipeInput.swipedDown)
        {
            camCam.transform.Translate(Vector3.down, Space.World);
        }
    }
}
