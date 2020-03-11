using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
	private SongInfo currSong;
	RaycastHit hit;
	Ray ray;

	[Header("Virtual cameras and pins")]
	public GameObject[] virtCam;
	public GameObject[] mapPin;
	public GameObject[] focusVirtCam;
	private int activeFocus = 0;
	public SongCollection[] songCollections;

	void Update()
	{
		if (!SongPickingControl.settingsIsActive)
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
							SelectSong(0);
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
							SelectSong(1);
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
							SelectSong(2);
						}
						else
						{
							activeFocus = 2; FocusTo(2);
						}
					}
				}
			}
		}
	}

	//focus to corresponding item such balkan or halay
	void FocusTo(int selected)
	{
		foreach (GameObject camera in virtCam) { camera.SetActive(false); }
		virtCam[selected].SetActive(true);
		foreach (GameObject pin in mapPin) { pin.GetComponent<Animator>().enabled = false; }
		mapPin[selected].GetComponent<Animator>().enabled = true;
	}

	IEnumerator waiter()
	{
		yield return new WaitForSeconds(0.6f);
		SceneManager.LoadSceneAsync("Gameplay");
	}

	void SelectSong(int song)
	{
		currSong = songCollections[0].songSets[0].song;
		SongInfoMessenger.Instance.currentSong = currSong;
		//select camera focus
		foreach (GameObject camera in focusVirtCam) { camera.SetActive(false); }
		focusVirtCam[song].SetActive(true);
		//wait briefly before loading gameplay scene
		StartCoroutine(waiter());
	}
}
