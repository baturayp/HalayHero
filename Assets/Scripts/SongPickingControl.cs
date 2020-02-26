using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

// Era Picker & Fuel indicator -> SoundPicker (2 boards) -> Choose Character
public class SongPickingControl : MonoBehaviour
{
	private SongInfo currSong;

	//First Board
	public RectTransform firstBoardTransform;
	public RectTransform settingsBoardTransform;

	public SongCollection[] songCollections;

	//setting objects
	public GameObject aaDroplabel;
	public Dropdown aaDrop;
	public GameObject volumeOn;
	public GameObject volumeOff;
	public GameObject volumeLow;

	//Song Board
	[Serializable]
	public class SongBoard
	{
		public RectTransform rectTransform;
		public Text titleText;
		//public Image backgroundImage;
		public ToggleBar difficultyToggle;
		//public GameObject leftButton;
		//public GameObject rightButton;
	}
	
	public SongBoard currSongBoard;

	//curr song properties
	public int currCollectionIndex;
	public int currSongSetIndex;
	public bool currEasyDifficulty;

	//character checkboxes
	public GameObject characterCheck1;
	public GameObject characterCheck2;

	//selected character which is 0 default
	private int selectedCharacter = 0;

	//the messenger to pass through other scenes
	public GameObject songInfoMessengerPrefab;

	//animation
	private RotationAnimation flipInAnimation;
	private RotationAnimation flipOutAnimation;
	private const float FlipAnimationDuration = 0.25f;
	private bool animating = false;
	//active board; 0 for firstboard, 1 for songboard
	private int activeBoard = 0;
	private bool settingsIsActive = false;

	//rotation
	private Vector3 verticalFlip = new Vector3(90f, 0f, 0f);
	private Vector3 horizontalFlip = new Vector3(0f, 90f, 0f);
	private Vector3 foldFlip = new Vector3(56f, 90f, 0f);

	//pivot
	private Vector2 topEdgePivot = new Vector2(0.5f, 1f);
	private Vector2 bottomEdgePivot = new Vector2(0.5f, 0f);
	private Vector2 leftEdgePivot = new Vector2(0f, 0.5f);
	private Vector2 rightEdgePivot = new Vector2(1f, 0.5f);
	private Vector2 centerPivot = new Vector2(0.5f, 0.5f);

	void Awake()
	{
		//init animation
		flipInAnimation = new RotationAnimation();
		flipOutAnimation = new RotationAnimation();

		//check if song messenger exists (if player returned from other scenes)
		if (SongInfoMessenger.Instance != null)
		{
			//Placeholder. Do something here.
		}
		//if not exist, create new SongInfoMessenger
		else
		{
			Instantiate(songInfoMessengerPrefab);
		}

		//set antialias pref
		int aaPref = PlayerPrefs.GetInt("Antialias", 0);
		AaDropdownChanged(aaPref);

		//set volume pref
		float volPref = PlayerPrefs.GetFloat("Volume", 1f);

		if (volPref == 0f)
		{
			volumeOn.SetActive(false);
			volumeLow.SetActive(false);
			volumeOff.SetActive(true);
		}
		else if (volPref == 0.5f)
		{
			volumeOn.SetActive(false);
			volumeLow.SetActive(true);
			volumeOff.SetActive(false);
		}
		else
		{
			volumeOn.SetActive(true);
			volumeLow.SetActive(false);
			volumeOff.SetActive(false);
		}
	}

	//era board
	public void FirstImageClicked(int songSetIndex)
	{
		if (animating) return;

		//set collection
		currCollectionIndex = songSetIndex;
		currSongSetIndex = 0;
		currEasyDifficulty = true;

		//configure song board
		ConfigureCurrSongBoard();

		//perform board transition animation
		FlipToSongFromFirst();
	}

	//song board
	public void SongBoardBackButtonClicked()
	{
		if (animating) return;

		//perform board transition animation
		FlipToFirstFromSong();

		//reset collection, song, difficulty
		currCollectionIndex = 0;
		currSongSetIndex = 0;
		currEasyDifficulty = true;
	}

	void ConfigureCurrSongBoard()
	{
		currSong = currEasyDifficulty ?
			songCollections[currCollectionIndex].songSets[currSongSetIndex].easy :
			songCollections[currCollectionIndex].songSets[currSongSetIndex].hard;

		//difficulty (left is easy)
		currSongBoard.difficultyToggle.Initialize(inLeft: currSong.easyDifficulty);
	}

	public void SongBoardDifficultyToggled()
	{
		if (animating) return;

		//play animation
		currSongBoard.difficultyToggle.Toggled();

		//change difficulty
		currEasyDifficulty = !currEasyDifficulty;

		//update currSong
		currSong = currEasyDifficulty ?
			songCollections[currCollectionIndex].songSets[currSongSetIndex].easy :
			songCollections[currCollectionIndex].songSets[currSongSetIndex].hard;

		//update combo & perfection
		//
	}


	//TODO: check against bugs
	public void characterSelected(int selectedChar)
	{
		selectedCharacter = selectedChar;
		if (selectedChar == 0)
		{
			characterCheck1.SetActive(true);
			characterCheck2.SetActive(false);
		}
		if (selectedChar == 1)
		{
			characterCheck1.SetActive(false);
			characterCheck2.SetActive(true);
		}
	}

	public void SongBoardGoButtonClicked()
	{
		//configure song info messenger
		SongInfoMessenger.Instance.characterIndex = selectedCharacter;
		SongInfoMessenger.Instance.currentSong = currSong;

		//load scene, optionally put a loading screen 
		SceneManager.LoadSceneAsync("Gameplay");
	}

	//antialiasing dropdown setting
	public void AaDropdownChanged(int i)
	{
		if (i == 0)
		{
			QualitySettings.antiAliasing = 0;
			PlayerPrefs.SetInt("Antialias", 0);
			aaDroplabel.GetComponent<TMPro.TextMeshProUGUI>().text = "ANTIALIAS OFF";
			aaDrop.value = 0;
		}
		if (i == 1)
		{
			QualitySettings.antiAliasing = 2;
			PlayerPrefs.SetInt("Antialias", 1);
			aaDroplabel.GetComponent<TMPro.TextMeshProUGUI>().text = "ANTIALIAS 2x";
			aaDrop.value = 1;
		}
		if (i == 2)
		{
			QualitySettings.antiAliasing = 4;
			PlayerPrefs.SetInt("Antialias", 2);
			aaDroplabel.GetComponent<TMPro.TextMeshProUGUI>().text = "ANTIALIAS 4x";
			aaDrop.value = 2;
		}
		if (i == 3)
		{
			QualitySettings.antiAliasing = 8;
			PlayerPrefs.SetInt("Antialias", 3);
			aaDroplabel.GetComponent<TMPro.TextMeshProUGUI>().text = "ANTIALIAS 8x";
			aaDrop.value = 3;
		}
	}

	//volume setting button
	public void SetVolume(float vol)
	{
		PlayerPrefs.SetFloat("Volume", vol);
		if (vol == 0f)
		{
			volumeOn.SetActive(false);
			volumeLow.SetActive(false);
			volumeOff.SetActive(true);
		}
		if (vol == 0.5f)
		{
			volumeOn.SetActive(false);
			volumeLow.SetActive(true);
			volumeOff.SetActive(false);
		}
		if (vol == 1f)
		{
			volumeOn.SetActive(true);
			volumeLow.SetActive(false);
			volumeOff.SetActive(false);
		}
	}

	//menu transition and animations
	//they can be simplified !
	void FlipToFirstFromSong()
	{
		animating = true;
		StartCoroutine(flipOutAnimation.AnimationCoroutine(
			currSongBoard.rectTransform,
			Vector3.zero,
			verticalFlip,
			FlipAnimationDuration,
			currSongBoard.rectTransform.pivot,
			bottomEdgePivot
		));
		StartCoroutine(flipInAnimation.AnimationCoroutine(
			firstBoardTransform,
			verticalFlip,
			Vector3.zero,
			FlipAnimationDuration,
			firstBoardTransform.pivot,
			topEdgePivot
		));
		Invoke("RestoreAnimating", FlipAnimationDuration);
		activeBoard = 0;
	}

	void FlipToSongFromFirst()
	{
		animating = true;
		StartCoroutine(flipOutAnimation.AnimationCoroutine(
			firstBoardTransform,
			Vector3.zero,
			verticalFlip,
			FlipAnimationDuration,
			firstBoardTransform.pivot,
			topEdgePivot
		));
		StartCoroutine(flipInAnimation.AnimationCoroutine(
			currSongBoard.rectTransform,
			verticalFlip,
			Vector3.zero,
			FlipAnimationDuration,
			currSongBoard.rectTransform.pivot,
			bottomEdgePivot
		));
		Invoke("RestoreAnimating", FlipAnimationDuration);
		activeBoard = 1;
	}

	public void FlipToSettingsFromX()
	{
	//if first board active, invoke transition from first one
		if (activeBoard == 0 && settingsIsActive == false)
		{
			animating = true;
			StartCoroutine(flipOutAnimation.AnimationCoroutine(
				firstBoardTransform,
				Vector3.zero,
				horizontalFlip,
				FlipAnimationDuration,
				firstBoardTransform.pivot,
				topEdgePivot
			));
			StartCoroutine(flipInAnimation.AnimationCoroutine(
				settingsBoardTransform,
				horizontalFlip,
				Vector3.zero,
				FlipAnimationDuration,
				settingsBoardTransform.pivot,
				bottomEdgePivot
			));
			Invoke("RestoreAnimating", FlipAnimationDuration);
			settingsIsActive = true;
		}
		//if songboard active
		if (activeBoard == 1 && settingsIsActive == false)
		{
			animating = true;
			StartCoroutine(flipOutAnimation.AnimationCoroutine(
				currSongBoard.rectTransform,
				Vector3.zero,
				horizontalFlip,
				FlipAnimationDuration,
				currSongBoard.rectTransform.pivot,
				bottomEdgePivot
			));
			StartCoroutine(flipInAnimation.AnimationCoroutine(
				settingsBoardTransform,
				horizontalFlip,
				Vector3.zero,
				FlipAnimationDuration,
				settingsBoardTransform.pivot,
				bottomEdgePivot
			));
			Invoke("RestoreAnimating", FlipAnimationDuration);
			settingsIsActive = true;
		}
	}

	public void FlipToXFromSettings()
	{
		if(activeBoard == 0)
		{
			animating = true;
			StartCoroutine(flipOutAnimation.AnimationCoroutine(
				settingsBoardTransform,
				Vector3.zero,
				horizontalFlip,
				FlipAnimationDuration,
				settingsBoardTransform.pivot,
				bottomEdgePivot
			));
			StartCoroutine(flipInAnimation.AnimationCoroutine(
				firstBoardTransform,
				horizontalFlip,
				Vector3.zero,
				FlipAnimationDuration,
				firstBoardTransform.pivot,
				topEdgePivot
			));
			Invoke("RestoreAnimating", FlipAnimationDuration);
			settingsIsActive = false;
		}
		if (activeBoard == 1)
		{
			animating = true;
			StartCoroutine(flipOutAnimation.AnimationCoroutine(
				settingsBoardTransform,
				Vector3.zero,
				horizontalFlip,
				FlipAnimationDuration,
				settingsBoardTransform.pivot,
				bottomEdgePivot
			));
			StartCoroutine(flipInAnimation.AnimationCoroutine(
				currSongBoard.rectTransform,
				horizontalFlip,
				Vector3.zero,
				FlipAnimationDuration,
				currSongBoard.rectTransform.pivot,
				bottomEdgePivot
			));
			Invoke("RestoreAnimating", FlipAnimationDuration);
			settingsIsActive = false;
		}
	}

	void RestoreAnimating()
	{
		animating = false;
	}
}
