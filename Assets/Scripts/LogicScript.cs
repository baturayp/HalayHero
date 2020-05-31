using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
	
	// song selector controls
	public SongCollection[] songCollections;

	void Start()
	{

	}

	void Update()
	{
		if (!SongPickingControl.settingsIsActive)
		{
			//swipe controls
			if (SwipeInput.swipedRight)
			{
				if (activeFocus == 0) { return; }
				else { FocusTo(--activeFocus); }
			}
			if (SwipeInput.swipedLeft)
			{
				if (activeFocus == 2) { return; }
				else { FocusTo(++activeFocus); }
			}
			if (SwipeInput.swipedUp)
			{
				return;
			}
			if (SwipeInput.swipedDown)
			{
				return;
			}

			//set focus when click on pins or labels
			if (Input.GetMouseButtonDown(0))
			{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit))
				{
					string obj = hit.collider.gameObject.name;
					if (obj == "PinPointText0" || obj == "PinPoint0")
					{
						FocusTo(0);
					}
					if (obj == "PinPointText1" || obj == "PinPoint1")
					{
						FocusTo(1);
					}
					if (obj == "PinPointText2" || obj == "PinPoint2")
					{
						FocusTo(2);
					}
				}
			}
		}
	}


	//focus to corresponding item such balkan or halay
	void FocusTo(int selected)
	{
		if (activeFocus == selected) 
		{ 
			SelectSong(selected); 
		}
		else
		{
			foreach (GameObject camera in virtCam) { camera.SetActive(false); }
			virtCam[selected].SetActive(true);
			StartCoroutine(PinResetter(selected));
			activeFocus = selected;
		}
	}

	//ensure pin animations reset to their initial states
	IEnumerator PinResetter(int selected)
	{
		foreach (GameObject pin in mapPin)
		{
			pin.GetComponent<Animator>().Play(0, -1, 0);
			yield return new WaitForSeconds(0.01f);
			pin.GetComponent<Animator>().enabled = false;
		}
		//animate selected pin
		mapPin[selected].GetComponent<Animator>().enabled = true;
	}

	//wait for focus effect before loading scene
	IEnumerator WaitBeforeLoad(int scene)
	{
		yield return new WaitForSeconds(0.6f);
		if (scene == 0)
		{
			SceneManager.LoadSceneAsync("Gameplay");
		}
		if (scene == 1)
		{
			SceneManager.LoadSceneAsync("Forest");
		}
		if (scene == 2)
		{
			SceneManager.LoadSceneAsync("Street");
		}
	}

	public void SelectSong(int selected)
	{
		currSong = songCollections[0].songSets[selected].song;
		SongInfoMessenger.Instance.currentSong = currSong;
		foreach (GameObject camera in focusVirtCam) { camera.SetActive(false); }
		focusVirtCam[selected].SetActive(true);
		StartCoroutine(WaitBeforeLoad(selected));
	}
}
