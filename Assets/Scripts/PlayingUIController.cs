using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayingUIController : MonoBehaviour
{
	//dancing right guy for tiggering animations
	public GameObject comboGuy;

	//tissue and gun objects
	public GameObject tissue;
	public GameObject gun;

	private float lastbeat;

	//combo
	public Image comboCircle;
	public Text comboScoreText;
	public GameObject comboText;

	//perfection
	public Image perfectionCircle;
	public Text perfectionScoreText;
	public GameObject perfectionText;

	//colors for scores
	public Color zeroColor;
	public Color startColor;
	public Color fullColor;

	//save score
	private int maxCombo = 0;
	private int currCombo = 0;
	private int fullNoteCounts;
	private int currPerfection = 0; //good: +1, perfect: +2 (full: fullCount * 2)

	//pause scene
	public GameObject pauseButton;
	public GameObject pauseScene;

	//win scene
	public GameObject winBackground;
	public GameObject winTitle;
	public Image winComboCircle;
	public Text winComboScoreText;
	public GameObject winComboTextBackground;
	public Text winComboText;
	public Image winPerfectCircle;
	public Text winPerfectScoreText;
	public Text winPerfectText;
	public GameObject winPerfectTextBackground;
	public GameObject winNextButton;
	public GameObject winRetryButton;
	private const float DelayBetweenElements = 0.75f;
	private const float NumberAnimationDuration = 2f;


	public AudioSource songAudioSource; //bad design, but whatever!

	void Start()
	{
		//get the length of all notes from messenger
		fullNoteCounts = SongInfoMessenger.Instance.currentSong.TotalHitCounts();

		lastbeat = 0f;

		//register to events
		Conductor.BeatOnHitEvent += BeatOnHit;
		Conductor.SongCompletedEvent += SongCompleted;
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
			//update perfection
			currPerfection += 2;

			//update combo
			currCombo++;
		}
		else if (rank == Conductor.Rank.GOOD)
		{
			//update perfection
			currPerfection++;

			//update combo
			currCombo++;
		}
		else if (rank == Conductor.Rank.BAD)
		{
			//update combo
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

		if (Conductor.songposition > lastbeat + Conductor.crotchet)
		{
			lastbeat += Conductor.crotchet;
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
		comboScoreText.text = currCombo == 0 ? "-" : currCombo.ToString();
		perfectionScoreText.text = currPerfection == 0 ? "-" : string.Format("%{0:F0}", ((float)currPerfection / (float)(fullNoteCounts * 2) * 100f));

		//color
		Color comboColor = currCombo == 0 ? zeroColor : Color.Lerp(startColor, fullColor, (float)currCombo / (float)fullNoteCounts);
		Color perfectionColor = currPerfection == 0 ? zeroColor : Color.Lerp(startColor, fullColor, (float)currPerfection / (float)(fullNoteCounts * 2));

		comboScoreText.color = comboColor;
		comboText.GetComponent<TMPro.TextMeshProUGUI>().color = comboColor;
		comboCircle.color = comboColor;

		perfectionCircle.color = perfectionColor;
		perfectionScoreText.color = perfectionColor;
		perfectionText.GetComponent<TMPro.TextMeshProUGUI>().color = perfectionColor;

	}

	public void PauseButtonOnClick()
	{
		//display pause scene
		pauseScene.SetActive(true);

		Conductor.paused = true;
	}

	//pause scene
	public void ResumeButtonOnClick()
	{
		//disable pause scene
		pauseScene.SetActive(false);

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
		StartCoroutine(ShowWinScene());
	}

	IEnumerator ShowWinScene()
	{
		//background gradually appears
		winBackground.SetActive(true);

		comboCircle.gameObject.SetActive(false);
		perfectionCircle.gameObject.SetActive(false);
		pauseButton.SetActive(false);

		yield return new WaitForSeconds(DelayBetweenElements);

		//show title
		winTitle.SetActive(true);

		yield return new WaitForSeconds(DelayBetweenElements);

		//show combo
		winComboCircle.gameObject.SetActive(true);
		winComboTextBackground.SetActive(true);

		//increase by animation
		float i = 0f;
		while (i <= 1f)
		{
			print(2);
			i += Time.deltaTime / NumberAnimationDuration;

			int newCombo = (int)Mathf.Lerp(0f, (float)maxCombo, i);
			Color newColor = Color.Lerp(startColor, fullColor, (float)newCombo / (float)fullNoteCounts);

			//set text
			winComboScoreText.text = newCombo.ToString();

			//set color
			winComboCircle.color = newColor;
			winComboScoreText.color = newColor;
			winComboText.color = newColor;

			yield return null;
		}

		//ensure that the final score is shown correct
		winComboScoreText.text = maxCombo.ToString();

		yield return new WaitForSeconds(DelayBetweenElements);

		//show perfection
		winPerfectCircle.gameObject.SetActive(true);
		winPerfectTextBackground.SetActive(true);

		float perfectionPercentage = (float)currPerfection / (float)(fullNoteCounts * 2) * 100f;

		i = 0f;
		while (i <= 1f)
		{
			i += Time.deltaTime / NumberAnimationDuration;

			float newPerfection = Mathf.Lerp(0f, perfectionPercentage, i);
			Color newColor = Color.Lerp(startColor, fullColor, newPerfection / 100f);

			//show text
			winPerfectScoreText.text = string.Format("{0:F1}%", newPerfection);

			//show color
			winPerfectText.color = newColor;
			winPerfectCircle.color = newColor;
			winPerfectScoreText.color = newColor;

			yield return null;
		}

		//ensure the correct perfection is shown
		winPerfectScoreText.text = string.Format("{0:F1}%", perfectionPercentage);

		yield return new WaitForSeconds(DelayBetweenElements);

		yield return new WaitForSeconds(DelayBetweenElements);

		//show retry & next buttons
		winRetryButton.SetActive(true);
		winNextButton.SetActive(true);

	}
}