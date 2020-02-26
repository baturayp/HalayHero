using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GameObject tissue;
    public GameObject gun;
    public GameObject flash;
    public ParticleSystem debris;

    void HideTissue()
    {
        tissue.SetActive(false);
    }

    void ShowTissue()
    {
        tissue.SetActive(true);
    }

    void HideGun()
    {
        gun.SetActive(false);
    }

    void ShowGun()
    {
        gun.SetActive(true);
    }

    void ShowFlash()
    {
        flash.SetActive(true);
    }

    void HideFlash()
    {
        flash.SetActive(false);
    }

    void TriggerDebris()
    {
        debris.Play();
    }
}
