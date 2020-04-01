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
	
	// song selector controls
	public SongCollection[] songCollections;
	public GameObject[] songLabel;
	
	private int activeSong = 0;
	private int oldActive = 0;
	private bool animating = false;

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

	//focus to corresponding item such balkan or halay
	void FocusTo(int selected)
	{
		foreach (GameObject camera in virtCam) { camera.SetActive(false); }
		virtCam[selected].SetActive(true);
		foreach (GameObject pin in mapPin) { pin.GetComponent<Animator>().enabled = false; }
		mapPin[selected].GetComponent<Animator>().enabled = true;
	}

	IEnumerator waiter(int scene)
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
		StartCoroutine(waiter(collection));
	}

	public void UpDownButtonPress(int i)
	{
		if (!animating && i == 1)
		{
			oldActive = activeSong;
			activeSong += 1;
			if(activeSong == songLabel.Length)
			{
				activeSong = 0;
			}
			StartCoroutine(LabelAnimation());
		}
		if (!animating && i == -1)
		{
			oldActive = activeSong;
			activeSong -= 1;
			if (activeSong == -1)
			{
				activeSong = songLabel.Length - 1;
			}
			StartCoroutine(LabelAnimation());
		}
	}

	IEnumerator LabelAnimation()
	{
		animating = true;
		float elapsedTime = 0.0f;
		songLabel[activeSong].SetActive(true);
		while (elapsedTime < 0.2f)
		{
			elapsedTime += Time.deltaTime;
			songLabel[activeSong].transform.rotation = Quaternion.Euler(90 - elapsedTime * 450, 0, 0);
			songLabel[oldActive].transform.rotation = Quaternion.Euler(elapsedTime * 450, 0, 0);
			yield return null;
		}
		songLabel[oldActive].SetActive(false);
		songLabel[activeSong].transform.rotation = Quaternion.Euler(0, 0, 0);
		songLabel[oldActive].transform.rotation = Quaternion.Euler(0, 0, 0);
		animating = false;
	}
}
