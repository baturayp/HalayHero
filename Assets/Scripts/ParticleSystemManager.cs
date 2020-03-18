using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
	public ParticleSet[] particleSet;

	public ParticleSystem perfectionEffect;
	public ParticleSystem comboEffect;

	void Start()
	{
		Conductor.BeatOnHitEvent += BeatOnHit;
		Conductor.KeyUpEvent += KeyUp;
	}

	void OnDestroy()
	{
		Conductor.BeatOnHitEvent -= BeatOnHit;
		Conductor.KeyUpEvent -= KeyUp;
	}

	//will be informed by the Conductor after a beat is hit
	void BeatOnHit(int track, Conductor.Rank rank)
	{
		if (rank == Conductor.Rank.PERFECT)
		{
			particleSet[track].perfect.Play();
			perfectionEffect.Play();
			comboEffect.Play();
		}
		if (rank == Conductor.Rank.GOOD)
		{
			//particleSet[track].good.Play();
			comboEffect.Play();
		}
		if (rank == Conductor.Rank.CONT)
		{
			particleSet[track].cont.Play();
		}

		//do nothing if missed
	}

	void KeyUp(int track)
	{
		particleSet[track].cont.Pause();
		particleSet[track].cont.Clear();
	}

	[System.Serializable]
	public class ParticleSet
	{
		public ParticleSystem perfect;
		public ParticleSystem good;
		public ParticleSystem bad;
		public ParticleSystem cont;
	}
}