using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public GameObject[] virtCam;
    public GameObject[] mapPin;
    private int activeFocus = 0;

    RaycastHit hit;
    Ray ray;

    public SongCollection[] songCollections;
    private SongInfo currSong;

    void Update()
    {
        if (SwipeInput.swipedRight)
        {
            if (activeFocus == 0) { return; }
            else { activeFocus -= 1; FocusTo(activeFocus); }
        }
        if (SwipeInput.swipedLeft)
        {
            if (activeFocus == 2) { return; }
            else { activeFocus += 1; FocusTo(activeFocus); }
        }

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                string obj = hit.collider.gameObject.name;
                if (obj == "PinPointText0" || obj == "PinPoint0")
                {
                    if (activeFocus == 0)
                    {
                        SelectSong();
                    }
                    else
                    {
                        activeFocus = 0; FocusTo(0);
                    }
                }
                if (obj == "PinPointText1" || obj == "PinPoint1")
                {
                    if (activeFocus == 1)
                    {
                        SelectSong();
                    }
                    else
                    {
                        activeFocus = 1; FocusTo(1);
                    }
                }
                if (obj == "PinPointText2" || obj == "PinPoint2")
                {
                    if (activeFocus == 2)
                    {
                        SelectSong();
                    }
                    else
                    {
                        activeFocus = 2; FocusTo(2);
                    }
                }
            }
        }
    }

    void FocusTo(int selected)
    {
        foreach(GameObject camera in virtCam) { camera.SetActive(false); }
        virtCam[selected].SetActive(true);
        foreach(GameObject pin in mapPin) { pin.GetComponent<Animator>().enabled = false; }
        mapPin[selected].GetComponent<Animator>().enabled = true;
    }

    void SelectSong()
    {
        currSong = songCollections[0].songSets[0].easy;
        SongInfoMessenger.Instance.characterIndex = 0;
        SongInfoMessenger.Instance.currentSong = currSong;
        SceneManager.LoadSceneAsync("Gameplay");
    }
}
