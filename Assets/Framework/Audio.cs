using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Audio : SceneSingleton<Audio>
{
	private AudioSource[] sources;

	void Awake ()
	{
		sources = gameObject.GetComponents<AudioSource> ();
		if ( sources.Length == 0 )
			Debug.LogWarning ( "There is no audio source on " + gameObject.name + ". No sound will be played" );
	}

	void Reawake ()
	{
		Awake ();
	}

// 	internal void FixedUpdate ()
// 	{
// 		if ( DebugWindow.InstanceCreated )
// 		{
// 			for ( int i = 0; i < sources.Length; i++ )
// 			{
// 				AudioSource source = sources[i];
// 				DebugWindow.Instance.AddEntry ( "AudioSource[" + i + "]", "State", source.isPlaying ? "Playing" : "Free" );
// 				DebugWindow.Instance.AddEntry ( "AudioSource[" + i + "]", "Clip", source.clip ? source.clip.name : "None" );
// 			}
// 		}
//	}

	internal AudioSource PlaySound ( AudioClip clip, float volume = 1.0f )
	{
		for ( int i = 0; i < sources.Length; i++ )
		{
			AudioSource source = sources[i];
			if ( !source.isPlaying )
			{
				source.clip = clip;
				source.volume = volume;
				source.Play ();
				return source;
			}
		}

		Debug.LogWarning ( "There is no free audio source on " + gameObject.name + ". No sound will be played" );
		return null;
	}
}
