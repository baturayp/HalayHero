using System;
using System.Collections;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
	public TextMesh timesText;
	public GameObject timesTextBackground;
	public SpriteRenderer ringSprite;
	[NonSerialized] public float startY;
	[NonSerialized] public float endY;
	[NonSerialized] public float removeLineY;
	[NonSerialized] public float beat;
	[NonSerialized] public int times;
	[NonSerialized] public float duration;
	[NonSerialized] public bool paused;


	public void Initialize(float posX, float startY, float endY, float removeLineY, float posZ, float targetBeat, int times, float duration, Color color)
	{
		this.startY = startY;
		this.endY = endY;
		this.beat = targetBeat;
		this.times = times;
		this.duration = duration;
		this.removeLineY = removeLineY;

		paused = false;

		//set position
		transform.position = new Vector3(posX, startY, posZ);

		//set color
		ringSprite.color = color;

		//set scale
		transform.localScale = new Vector3(1, 1, 1);

		//set times
		if (times > 0)
		{
			timesText.text = times.ToString();
			timesTextBackground.SetActive(true);
			ringSprite.size = new Vector2(1.28f, 1.28f);
		}
		else if (duration > 0)
		{
			timesTextBackground.SetActive(false);
			ringSprite.size = new Vector2(1.28f, duration);
		}
		else
		{
			timesTextBackground.SetActive(false);
			ringSprite.size = new Vector2(1.28f, 1.28f);
		}

	}

	void Update()
	{
		if (Conductor.pauseTimeStamp > 0f) return; //resume not managed

		if (paused) return; //multi-times notes might be paused on the finish line

		transform.position = new Vector3(transform.position.x, startY + (endY - startY) * (1f - (beat - Conductor.songposition) / Conductor.BeatsShownOnScreen), transform.position.z);

		//remove itself when out of the screen (remove line)
		if (duration > 0)
		{
			if (transform.position.y < (removeLineY + duration / 2f))
			{
				gameObject.SetActive(false);
			}
		}
		else
		{
			if (transform.position.y < (removeLineY))
			{
				gameObject.SetActive(false);
			}
		}
	}

	IEnumerator FadeOut()
	{
		float elapsedTime = 0.0f;
		Color c = ringSprite.color;
		while (elapsedTime < 0.2f)
		{
			elapsedTime += Time.deltaTime;
			c.a = 1.0f - Mathf.Clamp01(elapsedTime / 0.2f);
			ringSprite.color = c;
			transform.localScale = new Vector3(1 + elapsedTime, 1 + elapsedTime, 1);
			yield return null;
		}
		gameObject.SetActive(false);
	}

	//remove (multi-times note failed), might apply some animations later
	public void MultiTimesFailed()
	{
		gameObject.SetActive(false);
	}

	//if the node is removed, return true
	public bool MultiTimesHit()
	{
		//update text
		ringSprite.color = Color.green;
		times--;
		if (times == 0)
		{
			gameObject.SetActive(false);
			return true;
		}

		timesText.text = times.ToString();

		return false;
	}

	public void PerfectHit()
	{
		paused = true;
		ringSprite.color = Color.green;
		StartCoroutine(FadeOut());
	}

	public void GoodHit()
	{
		paused = true;
		ringSprite.color = Color.yellow;
		StartCoroutine(FadeOut());
	}

	public void BadHit()
	{
		paused = true;
		ringSprite.color = Color.red;
		StartCoroutine(FadeOut());
	}
}
