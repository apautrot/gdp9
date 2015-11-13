using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyboardInputAction : MonoBehaviour
{
	public KeyCode keycode;

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
		args.IsDown = Input.GetKey ( keycode );
	}
}
