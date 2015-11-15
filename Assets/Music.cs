using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
	static bool alreadyCreated;

	void Awake()
	{
		if ( !alreadyCreated )
		{
			alreadyCreated = true;
			DontDestroyOnLoad ( gameObject );
		}
		else
			gameObject.DestroySelf ();
	}
}
