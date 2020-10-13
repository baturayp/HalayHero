using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Accessibility;
using System;

public class PlayingUIController : MonoBehaviour
{
	//dancing right guy for tiggering animations
	public GameObject comboGuy;

	//tissue and gun objects
	public GameObject tissue;
	public GameObject gun;

	//in-game score panel
	public GameObject score;

	//score texts
	public GameObject comboScoreText;
	public GameObject perfectionScoreText;
	public GameObject timeRemainingText;
	public Animator comboIcon;
	public Animator perfectIcon;

	//colors for scores
	public Color zeroColor;
	public Color startColor;
	public Color fullColor;

	//save score
	private int maxCombo = 0;
	private int currCombo = 0;
	private int fullNoteCounts;
	private int currPerfection = 0; //good: +1, perfect: +2 (full: fullCount * 2)

	//fade in-out layer
	public Image fadeInOut;

	//chronometer parts
	public GameObject chronoMain;
	public GameObject chronoMinute;
	public GameObject chronoSeconds;

	//pause scene
	public GameObject pauseButton;
	public GameObject pauseScene;

	//win scene
	private const float DelayBetweenElements = 0.5f;
	private const float NumberAnimationDuration = 2f;

	//finish scene
	public GameObject finishedScene;
	public GameObject finishVCam;
	public GameObject mainCamera;
	public GameObject finishedSuccessState;
	public GameObject finishedFailState;
	public GameObject finalStarImage;
	public GameObject finalStarText;
	public GameObject finalMedalText;
	public GameObject finalMedalImage;
	public GameObject finishedControls;

	public AudioSource songAudioSource; //bad design, but whatever!

	void Start()
	{
		//get the length of all notes from messenger
		fullNoteCounts = SongInfoMessenger.Instance.currentSong.TotalHitCounts();

		//register to events
		Conductor.BeatOnHitEvent += BeatOnHit;
		Conductor.SongCompletedEvent += SongCompleted;
	}

	private void Update()
	{
		float remainingTime = Conductor.remainingTime;
		float songLength = SongInfoMessenger.Instance.currentSong.song.length;
		TimeSpan remTime = TimeSpan.FromSeconds(remainingTime);
		float correctedSecAngle = -45 - ((60 - remTime.Seconds) * 6);
		float correctedMinAngle = 1 / (songLength / Conductor.songposition);
		chronoSeconds.transform.eulerAngles = new Vector3(0f, 0f, correctedSecAngle);
		chronoMinute.GetComponent<Image>().fillAmount = correctedMinAngle;
	}


    void OnDestroy()
	{
		//unregister from events
		Conductor.BeatOnHitEvent -= BeatOnHit;
		Conductor.SongCompletedEvent -= SongCompleted;
	}

	//called by event
	void BeatOnHit(int trackNumber, Conductor.Rank rank)
	{
		if (rank == Conductor.Rank.PERFECT)
		{
			comboIcon.Play("ComboIconHit");
			perfectIcon.Play("PerfectAnimHit");
			currPerfection += 2;
			currCombo++;
			maxCombo = Mathf.Max(maxCombo, currCombo);
		}
		else if (rank == Conductor.Rank.GOOD)
		{
			currPerfection++;
			currCombo++;
			maxCombo = Mathf.Max(maxCombo, currCombo);
		}
		else if (rank == Conductor.Rank.BAD)
		{
			maxCombo = Mathf.Max(maxCombo, currCombo);
			currCombo = 0;
		}
		else if (rank == Conductor.Rank.MISS)
		{
			//check if it is max combo
			maxCombo = Mathf.Max(maxCombo, currCombo);
			//update combo
			currCombo = 0;
		}

		//dancer combo placemarks
		if(currCombo == 10)
		{
			TriggerAnim(1);
		}
		if(currCombo == 15)
		{
			TriggerAnim(2);
		}
		if(currCombo == 25)
		{
			TriggerAnim(3);
		}

		UpdateScoreUI();
	}

	//trigger dancer animation
	void TriggerAnim(int trig)
	{
		comboGuy.GetComponent<Animator>().SetTrigger(trig.ToString());
		tissue.GetComponent<Animator>().SetTrigger(trig.ToString());
	}

	void UpdateScoreUI()
	{
		//text
		string newCombo = currCombo == 0 ? "-" : currCombo.ToString();
		comboScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = newCombo;
		perfectionScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = currPerfection == 0 ? "-" : string.Format("%{0:F0}", ((float)currPerfection / (float)(fullNoteCounts * 2) * 100f));

		//color
		//Color comboColor = currCombo == 0 ? zeroColor : Color.Lerp(startColor, fullColor, (float)currCombo / (float)fullNoteCounts);
		//Color perfectionColor = currPerfection == 0 ? zeroColor : Color.Lerp(startColor, fullColor, (float)currPerfection / (float)(fullNoteCounts * 2));
		//comboScoreText.color = comboColor;
		//perfectionScoreText.color = perfectionColor;

	}

	public void PauseButtonOnClick()
	{
		//display pause scene
		pauseScene.SetActive(true);
		pauseButton.SetActive(false);
		Conductor.paused = true;
	}

	//pause scene
	public void ResumeButtonOnClick()
	{
		//disable pause scene
		pauseScene.SetActive(false);
		pauseButton.SetActive(true);
		Conductor.paused = false;
	}

	public void RetryButtonOnClick()
	{
		//reload current scene
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}

	public void HomeButtonOnClick()
	{
		//go to home scene
		SceneManager.LoadSceneAsync("MainMenu");
	}


	void SongCompleted()
	{
		StartCoroutine(ScreenFadeIn(true));
	}

	IEnumerator ScreenFadeIn(bool finish)
	{
		float elapsedTime = 0.0f;
		Color c = fadeInOut.color;
		while (elapsedTime < 0.6f)
		{
			elapsedTime += Time.deltaTime;
			c.a = 0.0f + Mathf.Clamp01(elapsedTime / 0.6f);
			fadeInOut.color = c;
			yield return null;
		}
		if (finish) 
		{
			StartCoroutine(ShowWinScene());
			StartCoroutine(ScreenFadeOut()); 
		}
	}

	IEnumerator ScreenFadeOut()
    {
		float elapsedTime = 0.0f;
		Color c = fadeInOut.color;
		while (elapsedTime < 0.6f)
		{
			elapsedTime += Time.deltaTime;
			c.a = 1.0f - Mathf.Clamp01(elapsedTime / 0.6f);
			fadeInOut.color = c;
			yield return null;
		}
	}

	IEnumerator ShowWinScene()
	{
		//hide scene elements and start to show finish items
		finishedScene.SetActive(true);
		finishVCam.SetActive(true);
		mainCamera.SetActive(false);
		pauseButton.SetActive(false);
		score.SetActive(false);
		
		yield return new WaitForSeconds(DelayBetweenElements);
		
		finishedControls.SetActive(true);

		yield return new WaitForSeconds(DelayBetweenElements);

		//combo animation
		finalStarImage.SetActive(true);
		finalStarText.SetActive(true);
		float i = 0f;
		while (i <= 1f)
		{
			i += Time.deltaTime / NumberAnimationDuration;
			int newCombo = (int)Mathf.Lerp(0f, (float)maxCombo, i);
			finalStarText.GetComponent<TMPro.TextMeshProUGUI>().text = newCombo.ToString();
			yield return null;
		}
		//ensure correct score shown
		finalStarText.GetComponent<TMPro.TextMeshProUGUI>().text = maxCombo.ToString();

		yield return new WaitForSeconds(DelayBetweenElements);

		finalMedalImage.SetActive(true);
		finalMedalText.SetActive(true);
		//perfection animation
		float perfectionPercentage = (float)currPerfection / (float)(fullNoteCounts * 2) * 100f;
		i = 0f;
		while (i <= 1f)
		{
			i += Time.deltaTime / NumberAnimationDuration;
			float newPerfection = Mathf.Lerp(0f, perfectionPercentage, i);
			finalMedalText.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:F0}%", newPerfection);
			yield return null;
		}
		//ensure correct percentage shown
		finalMedalText.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:F0}%", perfectionPercentage);

		yield return new WaitForSeconds(DelayBetweenElements);

		finishedSuccessState.SetActive(true);

		yield return null;
	}
}