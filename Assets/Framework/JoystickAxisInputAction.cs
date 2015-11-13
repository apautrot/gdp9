using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoystickAxisInputAction : MonoBehaviour
{
	public Direction direction;

	void Awake ()
	{
		InputAction inputAction = GetComponent<InputAction> ();
		if ( inputAction == null )
			Debug.LogError ( "TouchAction can't find a component InputAction on object " + name );

		inputAction.IsInputActionDown += IsInputActionDown;
	}

	void Reawake()
	{
		Awake ();
	}

	void IsInputActionDown ( InputActionDownEventArgs args )
	{
		float horizontal = Input.GetAxis ( "Horizontal" );
		float vertical = Input.GetAxis ( "Vertical" );
		
		float threshold = 0.125f;

		bool isDown = false;
		switch ( direction )
		{
			case Direction.TopLeft:
				isDown = ( horizontal < -threshold ) && ( vertical > threshold );
				break;
			case Direction.TopRight:
				isDown = ( horizontal > threshold ) && ( vertical > threshold );
				break;
			case Direction.BottomLeft:
				isDown = ( horizontal < -threshold ) && ( vertical < -threshold );
				break;
			case Direction.BottomRight:
				isDown = ( horizontal > threshold ) && ( vertical < -threshold );
				break;
		}

		args.IsDown = isDown;
	}
}
