using UnityEngine;
using System.Collections;

public class RecompilationGuard : MonoBehaviour
{
	private static bool startMethodHasBeenCalled;
	private static bool awakeMethodHasBeenCalled;

	RecompilationGuard ()
	{
	}

	void Awake ()
	{
		awakeMethodHasBeenCalled = true;
	}

	void Start ()
	{
		startMethodHasBeenCalled = true;
	}

	void FixedUpdate ()
	{
		if ( !awakeMethodHasBeenCalled )
		{
			awakeMethodHasBeenCalled = true;
			GameObjects.BroadcastMessageToScene ( "Reawake" );
		}

		if ( !startMethodHasBeenCalled )
		{
			startMethodHasBeenCalled = true;
			GameObjects.BroadcastMessageToScene ( "Restart" );
		}
	}
}