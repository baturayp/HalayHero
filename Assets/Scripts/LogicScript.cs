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
	public CanvasGroup selectionPanel;
	public GameObject songSelector;
	public GameObject songTitle;
	public GameObject songDuration;
	public GameObject songIcon;
	public GameObject songLock;
	public GameObject[] songStars;

	//some values needed for animations
	private int activeSong = 0;
	private int oldActive = 0;
	private bool songSelectorAnimating = false;
	private bool selectionPanelAnimating = false;
	Coroutine panelAnimCoroutine;
	private Vector3 songIconPos;
	private Vector3 songLockPos;

	void Start()
	{
		songIconPos = songIcon.transform.position;
		songLockPos = songLock.transform.position;
		SetSongValues(activeFocus, activeSong);
		panelAnimCoroutine = StartCoroutine(SongSelectorAnim());
	}

	void Update()
	{
		if (!SongPickingControl.settingsIsActive)
		{
			//swipe controls
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
			if (SwipeInput.swipedUp)
			{
				UpDownButtonPress(1);
			}
			if (SwipeInput.swipedDown)
			{
				UpDownButtonPress(-1);
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
						activeFocus = 0; FocusTo(0);
					}
					if (obj == "PinPointText1" || obj == "PinPoint1")
					{
						activeFocus = 1; FocusTo(1);
					}
					if (obj == "PinPointText2" || obj == "PinPoint2")
					{
						activeFocus = 2; FocusTo(2);
					}
				}
			}
		}
	}

	void SetSongValues(int coll, int song)
	{
		int songId = songCollections[coll].songSets[song].song.songID;
		
		//set title and time
		songTitle.GetComponent<TMPro.TextMeshProUGUI>().text = songCollections[coll].songSets[song].song.songTitle;
		songDuration.GetComponent<TMPro.TextMeshProUGUI>().text = songCollections[coll].songSets[song].song.songDuration;
		
		//set icons
		float iconMargin = songCollections[coll].songSets[song].song.iconMargin;
		//ensure icons at original position
		songIcon.transform.position = songIconPos;
		songLock.transform.position = songLockPos;
		if (iconMargin != 0)
		{
			//set margins
			songIcon.transform.position -= new Vector3(iconMargin, 0, 0);
			songLock.transform.position -= new Vector3(iconMargin, 0, 0);
		}
		
		//determine if locked or not
		bool defaultLocked = songCollections[coll].songSets[song].song.isLocked;
		int prefunLocked = PlayerPrefs.GetInt("Unlocked" + songId, 0);
		if (!defaultLocked)
		{
			songLock.SetActive(false);
			songIcon.SetActive(true);
		}
		if (defaultLocked)
		{
			songLock.SetActive(true);
			songIcon.SetActive(false);
			if (prefunLocked == 1)
			{
				songLock.SetActive(false);
				songIcon.SetActive(true);
			}
		}
		
		//set stars
		int prefStars = PlayerPrefs.GetInt("Stars" + songId, 0);
		if (prefStars > 0)
		{
			songStars[0].GetComponent<Image>().color = Color.yellow;
			if (prefStars > 1)
			{
				songStars[1].GetComponent<Image>().color = Color.yellow;
				if (prefStars > 2)
				{
					songStars[2].GetComponent<Image>().color = Color.yellow;
				}
			}
		}
	}

	//focus to corresponding item such balkan or halay
	void FocusTo(int selected)
	{
		foreach (GameObject camera in virtCam) { camera.SetActive(false); }
		virtCam[selected].SetActive(true);
		StartCoroutine(PinResetter(selected));
		if (!selectionPanelAnimating) panelAnimCoroutine = StartCoroutine(SongSelectorAnim());
		if (selectionPanelAnimating) StopCoroutine(panelAnimCoroutine); panelAnimCoroutine = StartCoroutine(SongSelectorAnim());
	}

	//song selector animation
	IEnumerator SongSelectorAnim()
	{
		selectionPanelAnimating = true;
		selectionPanel.alpha = 0;
		yield return new WaitForSeconds(1f);
		float elapsedTime = 0.0f;
		while (elapsedTime < 0.2f)
		{
			elapsedTime += Time.deltaTime;
			selectionPanel.alpha = elapsedTime * 5;
			yield return null;
		}
		selectionPanelAnimating = false;
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

	public void SelectSong(int collection)
	{
		currSong = songCollections[0].songSets[0].song;
		SongInfoMessenger.Instance.currentSong = currSong;
		//select camera focus
		foreach (GameObject camera in focusVirtCam) { camera.SetActive(false); }
		focusVirtCam[collection].SetActive(true);
		StartCoroutine(WaitBeforeLoad(collection));
	}

	public void UpDownButtonPress(int i)
	{
		if (!songSelectorAnimating && i == 1)
		{
			oldActive = activeSong;
			activeSong += 1;
			if(activeSong == songCollections[activeFocus].songSets.Length)
			{
				activeSong = 0;
			}
			StartCoroutine(LabelAnimation());
		}
		if (!songSelectorAnimating && i == -1)
		{
			oldActive = activeSong;
			activeSong -= 1;
			if (activeSong == -1)
			{
				activeSong = songCollections[activeFocus].songSets.Length - 1;
			}
			StartCoroutine(LabelAnimation());
		}
	}

	IEnumerator LabelAnimation()
	{
		songSelectorAnimating = true;
		float elapsedTime = 0.0f;
		while (elapsedTime < 0.1f)
		{
			elapsedTime += Time.deltaTime;
			songSelector.transform.rotation = Quaternion.Euler(elapsedTime * 900, 0, 0);
			yield return null;
		}
		SetSongValues(activeFocus, activeSong);
		while (elapsedTime < 0.1f)
		{
			elapsedTime += Time.deltaTime;
			songSelector.transform.rotation = Quaternion.Euler(90 - elapsedTime * 900, 0, 0);
			yield return null;
		}
		songSelector.transform.rotation = Quaternion.Euler(0, 0, 0);
		songSelectorAnimating = false;
	}
}
