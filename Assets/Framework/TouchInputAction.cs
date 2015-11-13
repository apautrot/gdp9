using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInputAction : MonoBehaviour
{
	bool isTouchedRightNow;

	void Awake ()
	{
		InputAction inputAction = GetComponent<InputAction> ();
		if ( inputAction == null )
			Debug.LogError ( "TouchAction can't find a component InputAction on object " + name );

		inputAction.IsInputActionDown += IsInputActionDown;
	}

	void Reawake ()
	{
		Awake ();
	}

	void IsInputActionDown ( InputActionDownEventArgs args )
	{
		args.IsDown = isTouchedRightNow;
	}

	internal void OnTouch ( TouchPhase phase )
	{
		switch ( phase )
		{
			case TouchPhase.Began:
				isTouchedRightNow = true;
				break;

			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				isTouchedRightNow = false;
				break;
		}

		// DebugWindow.Log ( "TouchZone", "OnTouch : ", phase.ToString() );
	}

#if UNITY_EDITOR || UNITY_STANDALONE

	void OnMouseDown ()
	{
		OnTouch ( TouchPhase.Began );
	}

	void OnMouseUp ()
	{
		OnTouch ( TouchPhase.Ended );
	}

#endif
}
