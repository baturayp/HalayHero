using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SongPickingControl : MonoBehaviour
{
	[Header("Setting objects")]
	public GameObject aaDroplabel;
	public Dropdown aaDrop;
	public GameObject volumeOn;
	public GameObject volumeOff;
	public GameObject volumeLow;
	public GameObject englishButton;
	public GameObject turkishButton;
	public GameObject settingsButton;
	public GameObject settingsLayer;
	public GameObject aboutLayer;
	public static bool settingsIsActive = false;

	[Header("Board transforms")]
	//First Board
	public RectTransform firstBoardTransform;
	public RectTransform songBoardTransform;
	public RectTransform settingsBoardTransform;

	//[Header("Song board settings & collections")]
	//public SongCollection[] songCollections;

	//curr song properties
	//public int currCollectionIndex;
	//public int currSongSetIndex;
	//public bool currEasyDifficulty;

	//the messenger to pass through other scenes
	public GameObject songInfoMessengerPrefab;

	//animation
	private RotationAnimation flipInAnimation;
	private RotationAnimation flipOutAnimation;
	private const float FlipAnimationDuration = 0.25f;
	private bool animating = false;

	//rotation
	private Vector3 verticalFlip = new Vector3(90f, 0f, 0f);
	private Vector3 horizontalFlip = new Vector3(0f, 90f, 0f);

	//pivot
	private Vector2 topEdgePivot = new Vector2(0.5f, 1f);
	private Vector2 bottomEdgePivot = new Vector2(0.5f, 0f);

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

		/*check system language
		  if Turkish, then set default to Turkish
		  if not, set it as English.
		  also remember user choice.*/
		//string sysLang; string langPref;
		//if (Application.systemLanguage == SystemLanguage.Turkish) { sysLang = "Turkish"; } else { sysLang = "English"; }
		//if (PlayerPrefs.HasKey("LangPref")) { langPref = PlayerPrefs.GetString("LangPref"); } else { langPref = sysLang; }
		//LangSelect(langPref);
		
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

	//antialiasing dropdown setting
	public void AaDropdownChanged(int i)
	{
		if (i == 0)
		{
			QualitySettings.antiAliasing = 0;
			PlayerPrefs.SetInt("Antialias", 0);
			aaDroplabel.GetComponent<TMPro.TextMeshProUGUI>().text = "ANTIALIAS 0x";
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

	/*menu transition and animations
	they can be simplified !*/
	//void FlipToFirstFromSong()
	//{
	//	animating = true;
	//	StartCoroutine(flipOutAnimation.AnimationCoroutine(
	//		songBoardTransform,
	//		Vector3.zero,
	//		verticalFlip,
	//		FlipAnimationDuration,
	//		songBoardTransform.pivot,
	//		bottomEdgePivot
	//	));
	//	StartCoroutine(flipInAnimation.AnimationCoroutine(
	//		firstBoardTransform,
	//		verticalFlip,
	//		Vector3.zero,
	//		FlipAnimationDuration,
	//		firstBoardTransform.pivot,
	//		topEdgePivot
	//	));
	//	Invoke("RestoreAnimating", FlipAnimationDuration);
	//}

	//void FlipToSongFromFirst()
	//{
	//	animating = true;
	//	StartCoroutine(flipOutAnimation.AnimationCoroutine(
	//		firstBoardTransform,
	//		Vector3.zero,
	//		verticalFlip,
	//		FlipAnimationDuration,
	//		firstBoardTransform.pivot,
	//		topEdgePivot
	//	));
	//	StartCoroutine(flipInAnimation.AnimationCoroutine(
	//		songBoardTransform,
	//		verticalFlip,
	//		Vector3.zero,
	//		FlipAnimationDuration,
	//		songBoardTransform.pivot,
	//		bottomEdgePivot
	//	));
	//	Invoke("RestoreAnimating", FlipAnimationDuration);
	//}

	public void SettingsButtonToggle()
	{
		if (settingsIsActive)
		{
			FlipToFirstFromSettings();
		}
		else
		{
			FlipToSettingsFromFirst();
		}
	}

	public void FlipToSettingsFromFirst()
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
		aboutLayer.SetActive(false);
		settingsLayer.SetActive(true);
	}

	public void FlipToFirstFromSettings()
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

	public void AboutToggle()
	{
		if (aboutLayer.activeSelf)
		{
			aboutLayer.SetActive(false);
			settingsLayer.SetActive(true);
		}
		else
		{
			aboutLayer.SetActive(true);
			settingsLayer.SetActive(false);
		}
	}

	void RestoreAnimating()
	{
		animating = false;
	}
}
