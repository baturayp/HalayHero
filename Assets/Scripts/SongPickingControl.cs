using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SongPickingControl : MonoBehaviour
{
	[Header("Setting objects")]
	public GameObject aaHd;
	public GameObject aaSd;
	public GameObject volumeOn;
	public GameObject volumeOff;
	public GameObject volumeLow;
	public GameObject settingsButton;
	public GameObject settingsLayer;
	public GameObject aboutLayer;
	public static bool settingsIsActive = false;
	public static bool songBoardIsActive = false;

	[Header("Board transforms")]
	//First Board
	public RectTransform firstBoardTransform;
	public RectTransform songBoardTransform;
	public RectTransform settingsBoardTransform;

	//the messenger to pass through other scenes
	public GameObject songInfoMessengerPrefab;

	//animation
	private RotationAnimation flipInAnimation;
	private RotationAnimation flipOutAnimation;
	private const float FlipAnimationDuration = 0.25f;

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
		AaChanged(aaPref);

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

	void Start()
	{
		LogicScript.OpenSongBoardEvent += FlipToSongFromFirst;
		LogicScript.CloseSongBoardEvent += FlipToFirstFromSong;
	}

    void OnDestroy()
    {
		LogicScript.OpenSongBoardEvent -= FlipToSongFromFirst;
		LogicScript.CloseSongBoardEvent -= FlipToFirstFromSong;
	}

    //antialiasing dropdown setting
    public void AaChanged(int i)
	{
		if (i == 0)
		{
			QualitySettings.antiAliasing = 0;
			PlayerPrefs.SetInt("Antialias", 0);
			aaSd.SetActive(true);
			aaHd.SetActive(false);
		}
		if (i == 1)
		{
			QualitySettings.antiAliasing = 4;
			PlayerPrefs.SetInt("Antialias", 1);
			aaHd.SetActive(true);
			aaSd.SetActive(false);
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
    public void FlipToFirstFromSong()
    {
        StartCoroutine(flipOutAnimation.AnimationCoroutine(
            songBoardTransform,
            Vector3.zero,
            verticalFlip,
            FlipAnimationDuration,
            songBoardTransform.pivot,
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
		songBoardIsActive = false;
    }

     public void FlipToSongFromFirst()
    {
        StartCoroutine(flipOutAnimation.AnimationCoroutine(
            firstBoardTransform,
            Vector3.zero,
            verticalFlip,
            FlipAnimationDuration,
            firstBoardTransform.pivot,
            topEdgePivot
        ));
        StartCoroutine(flipInAnimation.AnimationCoroutine(
            songBoardTransform,
            verticalFlip,
            Vector3.zero,
            FlipAnimationDuration,
            songBoardTransform.pivot,
            bottomEdgePivot
        ));
		songBoardIsActive = true;
    }

    public void SettingsButtonToggle()
	{
		if (settingsIsActive)
		{
			FlipToFirstFromSettings();
		}
		if (songBoardIsActive)
        {
			FlipToFirstFromSong();
        }
		else
		{
			FlipToSettingsFromFirst();
		}
	}

	public void FlipToSettingsFromFirst()
	{
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
		settingsIsActive = true;
		aboutLayer.SetActive(false);
		settingsLayer.SetActive(true);
	}

	public void FlipToFirstFromSettings()
	{
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
		settingsIsActive = false;
	}

	public void AboutToggle()
	{
		StartCoroutine(AboutAnim());
	}

	IEnumerator AboutAnim()
	{
		float elapsedTime = 0.0f;
		aboutLayer.SetActive(true);
		settingsLayer.SetActive(false);
		while (elapsedTime < 0.2f)
		{
			elapsedTime += Time.deltaTime;
			aboutLayer.transform.localScale = new Vector3(0 + 5 * elapsedTime, 0 + 5 * elapsedTime, 1);
			yield return null;
		}
		aboutLayer.transform.localScale = new Vector3(1, 1, 1);
	}
}
