using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonInputAction : MonoBehaviour
{
	public string keyOrButtonName;

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
		args.IsDown = Input.GetButton ( keyOrButtonName );
	}
}
