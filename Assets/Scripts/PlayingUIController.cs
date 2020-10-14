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
	public GameObject heartLeft;
	public GameObject heartCenter;
	public GameObject warningIcon;

	//score texts
	public GameObject heartScoreText;
	public GameObject perfectionScoreText;
	public Animator heartIcon;
	public Animator perfectIcon;
	public Animator heartScoreAnim;

	//save score
	private int heartsLeft = 2;
	private int currHeartCount = 5;
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
		//in-game chronometer logic
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
			//icon animations
			heartIcon.SetTrigger("HeartHit");
			perfectIcon.Play("PerfectAnimHit");
			heartScoreAnim.SetTrigger("scoreup");
			//heart count can be maximum 10
			if (currHeartCount < 10) currHeartCount++;
			//update perfection
			currPerfection += 2;
		}
		else if (rank == Conductor.Rank.GOOD)
		{
			currPerfection++;
		}
		else if (rank == Conductor.Rank.BAD)
		{
			if (currHeartCount > 0)
            {
				currHeartCount--;
            }
		}
		else if (rank == Conductor.Rank.MISS)
		{
			//first heart loss
			if (currHeartCount == 0 && heartsLeft == 2)
			{
				heartsLeft = 1;
				currHeartCount = 5;
				HeartsLeft1();
			}
			//second heart loss
			else if (currHeartCount == 0 && heartsLeft == 1)
			{
				heartsLeft = 0;
				currHeartCount = 5;
				HeartsLeft0();
			}
			else if (currHeartCount == 0 && heartsLeft == 0)
			{
				//when no more hearst left
			}
			else
			{
				currHeartCount--;
			}
		}

		//dancer combo placemarks
		//if(currCombo == 10)
		//{
		//	TriggerAnim(1);
		//}
		//if(currCombo == 15)
		//{
		//	TriggerAnim(2);
		//}
		//if(currCombo == 25)
		//{
		//	TriggerAnim(3);
		//}

		//enable warning sign on low rank
		if (currHeartCount == 3 && heartsLeft == 0)
		{
			warningIcon.GetComponent<Animator>().enabled = true;
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
		string newCombo = currHeartCount.ToString();
		heartScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = newCombo;
		perfectionScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = currPerfection == 0 ? "-" : string.Format("%{0:F0}", ((float)currPerfection / (float)(fullNoteCounts * 2) * 100f));
	}

	void HeartsLeft1()
    {
		heartIcon.SetTrigger("HeartLost");
		heartScoreAnim.SetTrigger("zero");
		heartCenter.GetComponent<Animator>().enabled = true;
		heartLeft.GetComponent<Animator>().enabled = true;
	}

	void HeartsLeft0()
    {
		heartIcon.SetTrigger("HeartLost");
		heartScoreAnim.SetTrigger("zero");
		heartCenter.SetActive(false);
		heartLeft.SetActive(false);
		heartCenter.SetActive(true);
	}

	public void PauseButtonOnClick()
	{
		//display pause scene
		pauseScene.SetActive(true);
		pauseButton.SetActive(false);
		heartIcon.enabled = false;
		Conductor.paused = true;
	}

	//pause scene
	public void ResumeButtonOnClick()
	{
		//disable pause scene
		pauseScene.SetActive(false);
		pauseButton.SetActive(true);
		heartIcon.enabled = true;
		Conductor.paused = false;
	}

	public void RetryButtonOnClick()
	{
		//reload current scene
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}

	public void HomeButtonOnClick()
	{
		StartCoroutine(ScreenFadeIn(false, true));
	}


	void SongCompleted()
	{
		StartCoroutine(ScreenFadeIn(true, false));
	}

	IEnumerator ScreenFadeIn(bool finish, bool returnMain)
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
		
		//song completed, slight fade while camera focusing to finish scene
		if (finish) 
		{
			StartCoroutine(ShowWinScene());
			StartCoroutine(ScreenFadeOut()); 
		}

		//exit to home button pressed, just fade in
		if (returnMain)
        {
			SceneManager.LoadSceneAsync("MainMenu");
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
			int newCombo = (int)Mathf.Lerp(0f, (float)currHeartCount, i);
			finalStarText.GetComponent<TMPro.TextMeshProUGUI>().text = newCombo.ToString();
			yield return null;
		}
		//ensure correct score shown
		finalStarText.GetComponent<TMPro.TextMeshProUGUI>().text = currHeartCount.ToString();

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