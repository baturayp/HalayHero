﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Song Collection")]
public class SongCollection : ScriptableObject
{
	public SongSet[] songSets;

	[Serializable]
	public class SongSet
	{
		public SongInfo song;
	}
}
