using System;
using System.Collections;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
	public TextMesh timesText;
	public GameObject timesTextBackground;
	public SpriteRenderer ringSprite;
	public LineRenderer longLineRenderer;
	public Sprite singleSprite;
	public Sprite longSprite;
	[NonSerialized] public float startY;
	[NonSerialized] public float endY;
	[NonSerialized] public float removeLineY;
	[NonSerialized] public float beat;
	[NonSerialized] public int times;
	[NonSerialized] public float duration;
	[NonSerialized] public bool paused;
	[NonSerialized] public bool restartedLong;


	public void Initialize(float posX, float startY, float endY, float removeLineY, float posZ, float targetBeat, int times, float duration, Color color)
	{
		this.startY = startY;
		this.endY = endY;
		this.beat = targetBeat;
		this.times = times;
		this.duration = duration;
		this.removeLineY = removeLineY;

		paused = false;
		restartedLong = false;

		//set position
		transform.position = new Vector3(posX, startY, posZ);
		longLineRenderer.enabled = false;

		//set color
		ringSprite.color = color;

		//set scale
		transform.localScale = new Vector3(1, 1, 1);

		//reset rotation
		transform.Rotate(0, 0, 0);

		//set times
		if (times > 0)
		{
			timesText.text = times.ToString();
			timesTextBackground.SetActive(true);
			ringSprite.sprite = singleSprite;
			longLineRenderer.enabled = false;
		}
		else if (duration > 0)
		{
			ringSprite.color = Color.red;
			timesTextBackground.SetActive(false);
			ringSprite.sprite = longSprite;
			longLineRenderer.enabled = true;
		}
		else
		{
			timesTextBackground.SetActive(false);
			ringSprite.sprite = singleSprite;
			longLineRenderer.enabled = false;
		}

	}

	void Update()
	{
		if (Conductor.pauseTimeStamp > 0f) return; //resume not managed

		//draw long note line
		if (duration > 0)
        {
			longLineRenderer.SetPosition(0, new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z));
			longLineRenderer.SetPosition(1, new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat + (0.2f / Conductor.tempo)) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z));
			longLineRenderer.SetPosition(2, new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat + duration - (0.2f / Conductor.tempo)) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z));
			longLineRenderer.SetPosition(3, new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat + duration) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z));
		}

		if (restartedLong) //restarted long notes
        {
			transform.position = new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat + duration) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z);
		}

		//remove itself when out of the screen (remove line)
		if (transform.position.y < removeLineY)
		{
			gameObject.SetActive(false);
		}

		if (paused) return; //multi-times notes might be paused on the finish line

		transform.position = new Vector3(transform.position.x, startY + (endY - startY) * (1f - ((beat) - Conductor.songposition) / (Conductor.BeatsShownOnScreen / Conductor.tempo)), transform.position.z);
	}

	public void FadeOutRedirector()
    {
		if (duration > 0) StartCoroutine(FadeOutLong());
		else StartCoroutine(FadeOut());
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

	IEnumerator FadeOutLong()
    {
		float elapsedTime = 0.0f;
		while (elapsedTime < 0.2f)
		{
			elapsedTime += Time.deltaTime;
			float value = 1.0f - Mathf.Clamp01(elapsedTime / 0.2f);
			longLineRenderer.materials[0].SetColor("_Color", new Color(1f, 1f, 1f, value));
			longLineRenderer.materials[1].SetColor("_Color", new Color(1f, 1f, 1f, value));
			yield return null;
		}
		gameObject.SetActive(false);
		transform.position = new Vector3(100, 100, 100);
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
			FadeOutRedirector();
			//gameObject.SetActive(false);
			return true;
		}

		timesText.text = times.ToString();

		return false;
	}

	public void PerfectHit()
	{
		paused = true;
		ringSprite.color = Color.green;
		FadeOutRedirector();
	}

	public void GoodHit()
	{
		paused = true;
		ringSprite.color = Color.yellow;
		FadeOutRedirector();
	}

	public void BadHit()
	{
		paused = true;
		ringSprite.color = Color.red;
		FadeOutRedirector();
	}
}
