﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
	private int heartsLeft = 3;
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
	private const float DelayBetweenElements = 0.2f;

	//inform conductor when lost before song completed
	public delegate void LostAction();
	public static event LostAction LostEvent;

	private bool collFinished = false;

	//show ads event
	public delegate void ShowAdsAction();
	public static event ShowAdsAction ShowAdsEvent;

	//finish scene
	public GameObject finishedScene;
	public Animator FinishPimp;
	public GameObject finishVCam;
	public GameObject mainCamera;
	public GameObject finishedSuccessState;
	public GameObject finishedFailState;
	public GameObject finalStarImage;
	public Image starImagetop;
	public Image starImagecenter;
	public Image starImagebottom;
	public GameObject finalMedalText;
	public GameObject finalMedalImage;
	public GameObject finishedControls;
	public GameObject nextButton;

	public AudioSource songAudioSource; //bad design, but whatever!

	void Start()
	{
		//reset showNext switch and inform singleton
		SingletonScript.instance.openFrame = true;
		SingletonScript.instance.collNumber = SongInfoMessenger.Instance.currCollNumber;

		//get the length of all notes from messenger
		fullNoteCounts = SongInfoMessenger.Instance.currentSong.TotalHitCounts();

		//register to events
		Conductor.BeatOnHitEvent += BeatOnHit;
		Conductor.SongCompletedEvent += SongCompleted;
		IntersititialAd.AdsShownEvent += GotoNextScene;
	}

	private void Update()
	{
		//in-game chronometer logic
		float remainingTime = Conductor.remainingTime;
		float songLength = SongInfoMessenger.Instance.currentSong.endTime;
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
		IntersititialAd.AdsShownEvent -= GotoNextScene;
	}

	//called by event
	void BeatOnHit(int trackNumber, Conductor.Rank rank)
	{
		if (rank == Conductor.Rank.PERFECT)
		{
			//icon animations
			heartIcon.SetTrigger("HeartHit");
			perfectIcon.Play("PerfectAnimHit");
			
			//gaining heart points makes game too easy, disable it for now
			//if (currHeartCount < 5) { currHeartCount++; heartScoreAnim.SetTrigger("scoreup"); }

			//make game incrementally harder
			if (SongInfoMessenger.Instance.currSongNumber == 0)
            {
				if (currHeartCount < 5) { currHeartCount++; heartScoreAnim.SetTrigger("scoreup"); }
			}
			if (SongInfoMessenger.Instance.currSongNumber == 1)
            {
				if (currHeartCount < 4) { currHeartCount++; heartScoreAnim.SetTrigger("scoreup"); }
			}
			if (SongInfoMessenger.Instance.currSongNumber == 2)
            {
				if (currHeartCount < 3) { currHeartCount++; heartScoreAnim.SetTrigger("scoreup"); }
			}
			if (SongInfoMessenger.Instance.currSongNumber == 3)
            {
				if (currHeartCount < 2) { currHeartCount++; heartScoreAnim.SetTrigger("scoreup"); }
			}
			
			//update perfection
			currPerfection += 2;
		}
		else if (rank == Conductor.Rank.GOOD)
		{
			currPerfection++;
		}
		else if (rank == Conductor.Rank.BAD)
		{
			//if (currHeartCount > 0)
   //         {
			//	currHeartCount--;
   //         }
		}
		else if (rank == Conductor.Rank.MISS)
		{
			//first heart loss
			if (currHeartCount == 0 && heartsLeft == 3)
			{
				heartsLeft = 2;
				currHeartCount = 5;
				HeartsLeft2();
			}
			//second heart loss
			else if (currHeartCount == 0 && heartsLeft == 2)
			{
				heartsLeft = 1;
				currHeartCount = 5;
				HeartsLeft1();
			}
			//no more hearts left, trigger game loss
			else if (currHeartCount == 0 && heartsLeft == 1)
			{
				heartsLeft = 0;
				LostEvent?.Invoke();
			}
			else
			{
				currHeartCount--;
			}
		}

        //dancer combo placemarks
        if (currPerfection == 20 || currPerfection == 50 || currPerfection == 80 || currPerfection == 110)
        {
            TriggerAnim(1);
        }
        if (currPerfection == 30 || currPerfection == 60 || currPerfection == 90 || currPerfection == 120)
        {
            TriggerAnim(2);
        }
        if (currPerfection == 40 || currPerfection == 70 || currPerfection == 100 || currPerfection == 130)
        {
            TriggerAnim(3);
        }

        //enable warning sign on low rank
        if (currHeartCount == 3 && heartsLeft == 1)
		{
			warningIcon.SetActive(true);
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
		string newCombo = currHeartCount == 0 ? "" : new String('.', currHeartCount);
		heartScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = newCombo;
		perfectionScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = currPerfection == 0 ? "-" : string.Format("%{0:F0}", ((float)currPerfection / (float)(fullNoteCounts * 2) * 100f));
	}

	void HeartsLeft2()
    {
		heartIcon.SetTrigger("HeartLost");
		heartScoreAnim.SetTrigger("zero");
		heartCenter.GetComponent<Animator>().enabled = true;
		heartLeft.GetComponent<Animator>().enabled = true;
	}

	void HeartsLeft1()
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

	public void NextButtonOnClick()
    {
		int currSongNumber = SongInfoMessenger.Instance.currSongNumber;
		SongCollection collection = SongInfoMessenger.Instance.currentCollection;
		int collLength = collection.songSets.Length;
		int currCollNumber = SongInfoMessenger.Instance.currCollNumber;
		if (currSongNumber < collLength - 1)
		{
			int curr = currSongNumber + 1;
			SongInfoMessenger.Instance.currentSong = collection.songSets[curr].song;
			SongInfoMessenger.Instance.currSongNumber = curr;
			ShowAdsEvent?.Invoke();
			//GotoNextScene();
		}
		else 
		{
			collFinished = true;
			SingletonScript.instance.collNumber = currCollNumber + 1;
			ShowAdsEvent?.Invoke();
			//GotoNextScene();
		}
	}

	public void GotoNextScene()
    {
		if(!collFinished) SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		else SceneManager.LoadSceneAsync("MainMenu");
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

		//save progress
		if (heartsLeft > 0)
        {
			int currSongNumber = SongInfoMessenger.Instance.currSongNumber;
			int currCollNumber = SongInfoMessenger.Instance.currCollNumber;
			string stringCollNumber = currCollNumber.ToString();
			SongCollection collection = SongInfoMessenger.Instance.currentCollection;
			int collLength = collection.songSets.Length;
			int currProgress = PlayerPrefs.GetInt(stringCollNumber, 1);
			if (currProgress - 1 == currSongNumber && collLength != currProgress)
			{
				PlayerPrefs.SetInt(stringCollNumber, currProgress + 1);
			}
			PlayerPrefs.Save();
		}

		//pimp animation
		if (heartsLeft == 3) FinishPimp.Play("Good");
		if (heartsLeft == 2 || heartsLeft == 1) FinishPimp.Play("Normal");
		if (heartsLeft == 0) FinishPimp.Play("Bad");

		yield return new WaitForSeconds(DelayBetweenElements);
		
		finishedControls.SetActive(true);

		yield return new WaitForSeconds(DelayBetweenElements);

		//stars animation
		finalStarImage.SetActive(true);
		if (heartsLeft < 3) starImagebottom.color = Color.gray;
		if (heartsLeft < 2) starImagecenter.color = Color.gray;
		if (heartsLeft < 1) starImagetop.color = Color.gray;

		yield return new WaitForSeconds(DelayBetweenElements);

		finalMedalImage.SetActive(true);
		finalMedalText.SetActive(true);

		//perfection animation
		float perfectionPercentage = (float)currPerfection / (float)(fullNoteCounts * 2) * 100f;
		float i = 0f;
		while (i <= 1f)
		{
			i += Time.deltaTime;
			float newPerfection = Mathf.Lerp(0f, perfectionPercentage, i);
			finalMedalText.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:F0}%", newPerfection);
			yield return null;
		}
		//ensure correct percentage shown
		finalMedalText.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0:F0}%", perfectionPercentage);

		yield return new WaitForSeconds(DelayBetweenElements);

		if (heartsLeft > 0)
        {
			finishedSuccessState.SetActive(true);
			nextButton.SetActive(true);
        }
        else
        {
			finishedFailState.SetActive(true);
			nextButton.SetActive(false);
        }

		yield return null;
	}
}