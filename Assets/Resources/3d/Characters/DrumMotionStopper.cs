using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumMotionStopper : MonoBehaviour
{

    public CircularmovementDrummer script;

    void DrummerStop()
    {
        script.enabled = false;
    }

    void DrummerContinue()
    {
        script.enabled = true;
    }
}
