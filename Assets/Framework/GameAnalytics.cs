// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.Analytics;
// 
// public class GameAnalytics : SceneSingleton<GameAnalytics>
// {
// 	internal enum LevelEndReason
// 	{
// 		LevelRestarted,
// 		LevelFinished,
// 		ApplicationExited
// 	}
// 
// 	private bool levelStarted;
// 
// 	private float timeAtLevelStart;
// 
// 	private float playtime;
// 	private float Playtime
// 	{
// 		get
// 		{
// 			return playtime;
// 		}
// 		set
// 		{
// 			playtime = value;
// 			PlayerPrefs.SetFloat ( "Analytics.Playtime", playtime );
// 		}
// 	}
// 
// 	private static bool sessionStarted;
// 
// 	void Start ()
// 	{
// 		playtime = PlayerPrefs.GetFloat ( "Analytics.Playtime", 0.0f );
// 
// 		if ( !sessionStarted )
// 		{
// 			// Analytics.StartSDK ( "22ce74ea-d414-4a24-80c2-79415f67238a" );
// 
// 			Dictionary<string, object> parameters = new Dictionary<string, object> ()
// 			{
// 				{ "PlayTime", Playtime }
// 			};
// 
// 			/*AnalyticsResult result = */
// 			Analytics.CustomEvent ( "SessionStart", parameters );
// 			// Debug.Log ( "Custom event LevelStart : " + result.ToString() );
// 
// 			sessionStarted = true;
// 
// 			StartCoroutine ( IncrementPlaytime () );
// 		}
// 	}
// 
// 	private WaitForSeconds waitForSeconds = new WaitForSeconds ( 1 );
// 
// 	IEnumerator IncrementPlaytime ()
// 	{
// 		while ( true )
// 		{
// 			yield return waitForSeconds;
// 			Playtime += 1;
// 		}
// 	}
// 
// 	void Restart ()
// 	{
// 		Start ();
// 	}
// 
// 	internal void OnLevelStart ()
// 	{
// 		// Debug.Log ( "OnLevelStart" );
// 		levelStarted = true;
// 		timeAtLevelStart = Time.timeSinceLevelLoad;
// 	}
// 
// 	internal void OnLevelEnd ( LevelEndReason reason )
// 	{
// 		// Debug.Log ( "OnLevelEnd" );
// 
// 		float lifeTime = Time.timeSinceLevelLoad - timeAtLevelStart;
// 
// 		Dictionary<string, object> parameters = new Dictionary<string, object> ()
// 		{
// 			{ "PlayTime", Playtime },
// 			// { "LevelName", GameFSM.Instance.LevelName },
// 			{ "LevelPlayTime", Time.timeSinceLevelLoad },
// 			{ "LifeTime", lifeTime },
// 			{ "Reason", reason.ToString() },
// 		};
// 
// 		/*AnalyticsResult result = */Analytics.CustomEvent ( "LevelStart", parameters );
// 		// Debug.Log ( "Custom event LevelStart : " + result.ToString() );
// 
// 		levelStarted = false;
// 	}
// 
// 	internal void OnLevelRestart ()
// 	{
// 		// Debug.Log ( "OnLevelRestart" );
// 
// 		OnLevelEnd ( LevelEndReason.LevelRestarted );
// 		OnLevelStart ();
// 	}
// 
// 	internal void OnApplicationQuit ()
// 	{
// 		// Debug.Log ( "OnApplicationQuit" );
// 
// 		if ( levelStarted )
// 			OnLevelEnd ( LevelEndReason.ApplicationExited );
// 	}
// }