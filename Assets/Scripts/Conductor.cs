using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Cinemachine;

public class Conductor : MonoBehaviour
{
	[Header("Animated objects in the scene")]
	[Tooltip("Select all objects that needs to pause and start")]
	public GameObject[] animatedObjects;
	public ParticleSystem[] particles;
	public GameObject animatedScript;

	[Header("Cinemachine cameras")]
	public GameObject startCam;
	public GameObject vcCam1;
	public CinemachineVirtualCamera cineCam;

	public enum Rank { PERFECT, GOOD, BAD, MISS, CONT };

	public delegate void BeatOnHitAction(int trackNumber, Rank rank);
	public static event BeatOnHitAction BeatOnHitEvent;

	//keyup action
	public delegate void KeyUpBeatAction(int trackNumber);
	public static event KeyUpBeatAction KeyUpBeatEvent;

	//song completion
	public delegate void SongCompletedAction();
	public static event SongCompletedAction SongCompletedEvent;

	private float songLength;

	//if the whole game is paused
	public static bool paused = true;
	private bool songStarted = false;

	public static float pauseTimeStamp = -1f; //negative means not managed
	private float pausedTime = 0f;

	private SongInfo songInfo;

	[Header("Node spawn points")]
	public float[] trackSpawnPosX;

	public float startLineY;
	public float finishLineY;

	public float removeLineY;

	public float badOffsetY;
	public float goodOffsetY;
	public float perfectOffsetY;
	private const float MobileOffsetMultiplier = 1.4f;

	//different colors for each track
	public Color[] trackColors;

	//current song position
	public static float songposition;

	//index for each tracks
	private int[] trackNextIndices;

	//queue, saving the MusicNodes which currently on screen
	private Queue<MusicNode>[] queueForTracks;

	//multi-times notes might be paused on the finish line, but already dequed
	private MusicNode[] previousMusicNodes;

	//keep a reference of the sound tracks
	private SongInfo.Track[] tracks;

	private float dsptimesong;

	public static float BeatsShownOnScreen = 4f;
	public static float tempo = 1f;

	//count down canvas
	private const int StartCountDown = 3;
	public GameObject countDownCanvas;
	public GameObject countDownText;


	//layer each music node, so that the first one would be at the front
	private const float LayerOffsetZ = 0.001f;
	private const float FirstLayerZ = -6f;
	private float[] nextLayerZ;

	//total tracks
	private int len;
	private AudioSource AudioSource { get { return GetComponent<AudioSource>(); } }

	void PlayerInputted(int trackNumber)
	{
		//check if multi-times node exists
		if (previousMusicNodes[trackNumber] != null)
		{
			//dispatch beat on hit event (multi-times node is always PERFECT)
			BeatOnHitEvent?.Invoke(trackNumber, Rank.PERFECT);

			//check if the node should be removed
			if (previousMusicNodes[trackNumber].MultiTimesHit())
			{
				//print("Multi-Times Succeed!");
				previousMusicNodes[trackNumber] = null;
			}
		}
		else if (queueForTracks[trackNumber].Count != 0)
		{
			//peek the node in the queue
			MusicNode frontNode = queueForTracks[trackNumber].Peek();

			//take care multi-notes inside update
			if (frontNode.times > 0) return;

			//long notes
			if (frontNode.duration > 0)
            {
				if (frontNode.paused && frontNode.beat + 0.2f > songposition)
                {
					frontNode.ringSprite.color = Color.green;
					BeatOnHitEvent?.Invoke(trackNumber, Rank.CONT);
				}
            }

			float offsetY = Mathf.Abs(frontNode.gameObject.transform.position.y - finishLineY);

			if (frontNode.times == 0 && frontNode.duration == 0)
			{
				if (offsetY < perfectOffsetY) //perfect hit
				{
					frontNode.PerfectHit();
					BeatOnHitEvent?.Invoke(trackNumber, Rank.PERFECT);
					queueForTracks[trackNumber].Dequeue();
				}
				else if (offsetY < goodOffsetY) //good hit
				{
					frontNode.GoodHit();
					BeatOnHitEvent?.Invoke(trackNumber, Rank.GOOD);
					queueForTracks[trackNumber].Dequeue();
				}
				else if (offsetY < badOffsetY) //bad hit
				{
					frontNode.BadHit();
					BeatOnHitEvent?.Invoke(trackNumber, Rank.BAD);
					queueForTracks[trackNumber].Dequeue();
				}
			}
		}
	}

	void KeyUp(int trackNumber)
	{
		if (queueForTracks[trackNumber].Count != 0)
		{
			MusicNode frontNode = queueForTracks[trackNumber].Peek();
			if (frontNode.duration == 0) return;
			KeyUpBeatEvent?.Invoke(trackNumber);
			if (frontNode.ringSprite.color == Color.green) { frontNode.PerfectHit(); BeatOnHitEvent?.Invoke(trackNumber, Rank.PERFECT); queueForTracks[trackNumber].Dequeue(); }
		}
	}

	void Start()
	{
		//reset static variables
		paused = true;
		pauseTimeStamp = -1f;

		//if in mobile platforms, multiply the offsets
#if UNITY_IOS || UNITY_ANDROID
		perfectOffsetY *= MobileOffsetMultiplier;
		goodOffsetY *= MobileOffsetMultiplier;
		badOffsetY *= MobileOffsetMultiplier;
#endif

		//display countdown canvas
		countDownCanvas.SetActive(true);

		//get the song info from messenger
		songInfo = SongInfoMessenger.Instance.currentSong;
		tempo = songInfo.tempo;
		BeatsShownOnScreen = songInfo.beatsShownOnScreen;

		//listen to player input
		PlayerInputControl.InputtedEvent += PlayerInputted;
		PlayerInputControl.KeyupEvent += KeyUp;

		songLength = songInfo.song.length;

		//initialize arrays
		len = trackSpawnPosX.Length;
		trackNextIndices = new int[len];
		nextLayerZ = new float[len];
		queueForTracks = new Queue<MusicNode>[len];
		previousMusicNodes = new MusicNode[len];
		for (int i = 0; i < len; i++)
		{
			trackNextIndices[i] = 0;
			queueForTracks[i] = new Queue<MusicNode>();
			previousMusicNodes[i] = null;
			nextLayerZ[i] = FirstLayerZ;
		}

		tracks = songInfo.tracks; //keep a reference of the tracks

		//toggle game objects
		SetGameObjects(true);
		StartCoroutine(AnimationCoroutine());

		//initialize audioSource
		AudioSource.clip = songInfo.song;
		AudioSource.volume = PlayerPrefs.GetFloat("Volume", 1f);

		//start countdown
		StartCoroutine(CountDown());
	}

	IEnumerator AnimationCoroutine()
	{
		yield return new WaitForSeconds(0.001f);
		SetGameObjects(false);
		startCam.SetActive(false);
		vcCam1.SetActive(true);
	}

	void StartSong()
	{
		//get dsptime
		dsptimesong = (float)AudioSettings.dspTime;

		//play song
		AudioSource.Play();

		SetGameObjects(true);

        //unpause
        paused = false;
		songStarted = true;
	}

	void Update()
	{

			//for count down
			if (!songStarted) return;

		//for pausing
		if (paused)
		{
			if (pauseTimeStamp < 0f) //not managed
			{
				pauseTimeStamp = (float)AudioSettings.dspTime;
				AudioSource.Pause();
				SetGameObjects(false);
			}

			return;
		}
		else if (pauseTimeStamp > 0f) //resume not managed
		{
			pausedTime += (float)AudioSettings.dspTime - pauseTimeStamp;
			AudioSource.Play();
			SetGameObjects(true);

			pauseTimeStamp = -1f;
		}

		//calculate songposition
		songposition = (float)(AudioSettings.dspTime - dsptimesong - pausedTime) * AudioSource.pitch - (songInfo.songOffset);

		//check if need to instantiate new nodes
		float beatToShow = songposition + (BeatsShownOnScreen / tempo);

		//loop the tracks for new MusicNodes
		for (int i = 0; i < len; i++)
		{
			int nextIndex = trackNextIndices[i];
			SongInfo.Track currTrack = tracks[i];

			if (nextIndex < currTrack.notes.Length && currTrack.notes[nextIndex].dueTo < beatToShow)
			{
				SongInfo.Note currNote = currTrack.notes[nextIndex];

				//set z position
				float layerZ = nextLayerZ[i];
				nextLayerZ[i] += LayerOffsetZ;

				//get a new node
				MusicNode musicNode = MusicNodePool.instance.GetNode(trackSpawnPosX[i], startLineY, finishLineY, removeLineY, layerZ, currNote.dueTo, currNote.manyTimes, currNote.duration, trackColors[i]);

				//enqueue
				queueForTracks[i].Enqueue(musicNode);

				//update the next index
				trackNextIndices[i]++;
			}
		}

		//set dynamic tempo when applicable
		if(songInfo.dynamicTempo != null && songInfo.dynamicTempo.Length > 0)
		{
			int length = songInfo.dynamicTempo.Length;
			for (int i = 0; i < length; i++)
			{
				if (songposition > songInfo.dynamicTempo[i].startTime)
				{
					tempo = songInfo.dynamicTempo[i].tempoVal;
				}
			}
		}

		//loop the queue to check if any of them reaches the finish line
		for (int i = 0; i < len; i++)
		{
			//empty queue, continue
			if (queueForTracks[i].Count == 0) continue;

			MusicNode currNode = queueForTracks[i].Peek();

			//multi-times note
			if (currNode.times > 0 && currNode.transform.position.y <= finishLineY + goodOffsetY)
			{
				//have previous note stuck on the finish line
				if (previousMusicNodes[i] != null)
				{
					previousMusicNodes[i].MultiTimesFailed();
					BeatOnHitEvent?.Invoke(i, Rank.MISS);
				}
				//pause the note
				currNode.paused = true;
				//align to finish line
				currNode.transform.position = new Vector3(currNode.transform.position.x, finishLineY, currNode.transform.position.z);
				//deque, but keep a reference
				previousMusicNodes[i] = currNode;
				queueForTracks[i].Dequeue();
			}

			//long note
			else if (currNode.duration > 0 && currNode.transform.position.y <= finishLineY)
			{
				if (previousMusicNodes[i] != null)
				{
					previousMusicNodes[i].MultiTimesFailed();
					previousMusicNodes[i] = null;
					BeatOnHitEvent?.Invoke(i, Rank.MISS);
				}
				//pause the note
				currNode.paused = true;
				if (!currNode.restartedLong)
                {
					//align to finish line
					currNode.transform.position = new Vector3(currNode.transform.position.x, finishLineY, currNode.transform.position.z);
				}
				//keep long note at the position til duration ends
				if (songposition > currNode.beat + currNode.duration)
                {
					currNode.restartedLong = true;
					KeyUpBeatEvent?.Invoke(i);
					queueForTracks[i].Dequeue();
				}
			}
			else if (currNode.times == 0 && currNode.duration == 0 && currNode.transform.position.y <= finishLineY - goodOffsetY)   //single time note
			{
				//have previous note stuck on the finish line
				if (previousMusicNodes[i] != null)
				{
					previousMusicNodes[i].MultiTimesFailed();
					previousMusicNodes[i] = null;
					BeatOnHitEvent?.Invoke(i, Rank.MISS);
				}
				queueForTracks[i].Dequeue();
				//dispatch miss event (if a multi-times note is missed, its next single note would also be missed)
				BeatOnHitEvent?.Invoke(i, Rank.MISS);
			}
		}


		//check to see if the song reaches its end
		if (songposition > songLength)
		{
			songStarted = false;
			SongCompletedEvent?.Invoke();
		}
	}

	IEnumerator CountDown()
	{
		yield return new WaitForSeconds(1f);

		for (int i = StartCountDown; i >= 1; i--)
		{
			countDownText.GetComponent<TMPro.TextMeshProUGUI>().text = i.ToString();
			yield return new WaitForSeconds(1f);
		}
		countDownCanvas.SetActive(false);

		StartSong();
	}

	void OnDestroy()
	{
		PlayerInputControl.InputtedEvent -= PlayerInputted;
	}

	void SetGameObjects(bool state)
	{
		//set animation components of defined objects true or false
		foreach(var ani in animatedObjects){ani.GetComponent<Animator>().enabled = state;}
		//set components of certain scripts true or false
		animatedScript.GetComponent<Circularmovement>().enabled = state;
		//set or remove camera noise
		if (state) { cineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 1; }
		if (!state) { cineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0; }
		//set particles
		if (state) { foreach (var particle in particles) { particle.Play(); } }
		if (!state) { foreach (var particle in particles) { particle.Pause(); } }
	}
}
