using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song Info")]
public class SongInfo : ScriptableObject
{
	public AudioClip song;

	[Header("Every Song Has a Unique ID")]
	public int songID;
	public int collection;
	public int songNum;

	[Header("Song Text Information")]
	public string songTitle;

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
		//uncomment line below after writing scripts for performance
		//if (totalHits != -1) return totalHits;

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
	}

	[System.Serializable]
	public class Track
	{
		public Note[] notes;
	}
}
