using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Info")]
public class SongInfo : ScriptableObject
{
	public AudioClip song;

	[Header("Every Song Has a Unique ID")]
	public int songID;

	[Header("Song Text Information")]
	public string songTitle;
	//duration in plain text
	public string songDuration;

	[Header("Some Defaults")]
	//is this song locked by default?
	public bool isLocked;
	//is there any icon margin on main menu?
	public float iconMargin;

	[Header("Playing Information")]
	public AudioClip[] defaultBeats;

	public float songOffset;
	public float bpm;

    [Header("Notes. Set it here 3 for three tracks")]
	public Track[] tracks;

	// when not calculated, it's -1
	private int totalHits = -1;

	//get the total hits of the song
	public int TotalHitCounts()
	{
		totalHits = 0;
		foreach (Track track in tracks)
		{
			foreach (Note note in track.notes)
			{
				if (note.times == 0)
				{
					totalHits += 1;
				}
				if (note.times > 0)
				{
					totalHits += note.times;
				}
				if (note.times < 0)
				{
					totalHits += 1;
				}
			}
		}

		return totalHits;
	}

    [System.Serializable]
	public class Note
	{
		public float note;
		public int times;
		public int BPM;
	}

	[System.Serializable]
	public class Track
	{
		public Note[] notes;
	}

	public TextAsset json;

	void OnEnable()
	{
		var jsonText = json.ToString();
		var scriptJson = JsonUtility.FromJson<Note>(jsonText);
		this.songOffset = scriptJson.BPM;
	}
}
