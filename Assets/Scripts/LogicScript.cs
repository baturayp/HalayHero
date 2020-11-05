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

	//activate songboard action
	public delegate void OpenSongBoardAction();
	public static event OpenSongBoardAction OpenSongBoardEvent;
	public delegate void CloseSongBoardAction();
	public static event CloseSongBoardAction CloseSongBoardEvent;

	[Header("Virtual cameras and pins")]
	public GameObject[] virtCam;
	public GameObject[] mapPin;
	public GameObject[] focusVirtCam;
	private int activeFocus = 0;

	//Collection Frames
	public GameObject[] frames;

	// song selector controls
	public SongCollection[] songCollections;

	public Button[] BalkanFrameButtons;
	public Button[] DugunFrameButtons;
	public Button[] HalayFrameButtons;

    private void Start()
    {
		//get progress
		int balkanProgress = PlayerPrefs.GetInt("0", 1);
		int dugunProgress = PlayerPrefs.GetInt("1", 1);
		int halayProgress = PlayerPrefs.GetInt("2", 1);

		//activate balkan buttons
		for (int i = 0; i < balkanProgress; i++)
        {
			BalkanFrameButtons[i].interactable = true;
        }
		//activate dugun buttons
		for (int i = 0; i < dugunProgress; i++)
		{
			DugunFrameButtons[i].interactable = true;
		}
		//activate halay buttons
		for (int i = 0; i < halayProgress; i++)
		{
			HalayFrameButtons[i].interactable = true;
		}

		//focus to and open songboard (if returning from gameplay)
		if (SingletonScript.instance.collNumber == 1) FocusTo(1);
		if (SingletonScript.instance.collNumber == 2) FocusTo(2);

		if (SingletonScript.instance.openFrame && SingletonScript.instance.collNumber < 3)
		{
			frames[SingletonScript.instance.collNumber].SetActive(true);
		}
	}

    void Update()
	{
		if (!SongPickingControl.settingsIsActive && !SongPickingControl.songBoardIsActive)
		{
			//swipe controls
			if (SwipeInput.swipedRight)
			{
				if (activeFocus == 0) { return; }
				else { FocusTo(activeFocus-1); }
			}
			if (SwipeInput.swipedLeft)
			{
				if (activeFocus == 2) { return; }
				else { FocusTo(activeFocus+1); }
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
			foreach (GameObject frame in frames) { frame.SetActive(false); }
			frames[selected].SetActive(true);
			OpenSongBoardEvent?.Invoke();
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
			SceneManager.LoadSceneAsync("Street");
		}
		if (scene == 1)
		{
			SceneManager.LoadSceneAsync("Gameplay");
		}
		if (scene == 2)
		{
			SceneManager.LoadSceneAsync("Forest");
		}
	}

	public void SelectSong(int songnumber)
	{
		currSong = songCollections[activeFocus].songSets[songnumber].song;
		SongInfoMessenger.Instance.currentSong = currSong;
		SongInfoMessenger.Instance.currentCollection = songCollections[activeFocus];
		SongInfoMessenger.Instance.currSongNumber = songnumber;
		SongInfoMessenger.Instance.currCollNumber = activeFocus;
		CloseSongBoardEvent?.Invoke();
		foreach (GameObject camera in focusVirtCam) { camera.SetActive(false); }
		focusVirtCam[activeFocus].SetActive(true);
		StartCoroutine(WaitBeforeLoad(activeFocus));
	}
}
