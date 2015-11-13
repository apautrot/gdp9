using UnityEngine;
using System.Collections;

internal class InputActionDownEventArgs
{
	private bool isDown;
	internal bool IsDown
	{
		get { return isDown; }
		set { isDown |= value; }
	}
}

public class InputAction : MonoBehaviour
{
	internal bool isDown;
	internal bool isJustDown;
	internal bool isJustUp;
	internal float downTime;

	internal delegate void InputActionDown ( InputActionDownEventArgs args );
	internal event InputActionDown IsInputActionDown;

	internal void Update ()
	{
		if ( IsInputActionDown != null )
		{
			InputActionDownEventArgs args = new InputActionDownEventArgs();
			IsInputActionDown ( args );
			UpdateState ( args.IsDown );
		}
	}

	void UpdateState ( bool isDownNow )
	{
		isJustDown = ( !isDown && isDownNow );
		isJustUp = ( isDown && !isDownNow );

		if ( isDownNow )
		{
			if ( !isDown )
			{
				downTime = 0;
				isDown = true;
				OnDown ();
			}
			else
				downTime += Time.deltaTime;
		}
		else
		{
			if ( isDown )
			{
				isDown = false;
				OnUp ();
			}
		}
	}

	internal virtual void OnDown ()
	{
	}

	internal virtual void OnUp ()
	{
	}
}
