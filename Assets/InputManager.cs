using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputActionName
{
    A,
    B,
    X,
    Y,
    Up,
    Down,
    Left,
    Right
}

[System.Serializable]
public class InputDefinition
{
	public InputActionName name;
	public GameObject buttonPrefab;

	public string joystickButtonName;
	public string joystickAxixName;
	public float joystickAxisThreshold;

	public KeyCode keyboardKeyCode;

	internal void DebugInWindow()
	{
		bool isActivated = IsActivated;
		DebugWindow.Log ( "Input", name.ToString (), isActivated );
	}

	public bool IsActivated
	{
		get
		{
			if ( joystickButtonName.Length > 0 )
			{
				return Input.GetButton ( joystickButtonName );
			}

			if ( joystickAxixName.Length > 0 )
			{
				float value = Input.GetAxis ( joystickAxixName );
				if ( joystickAxisThreshold < 0 )
					return value < joystickAxisThreshold;
				else
					return value > joystickAxisThreshold;
			}

			if ( keyboardKeyCode != KeyCode.None )
			{
				return Input.GetKey ( keyboardKeyCode );
			}

			Debug.LogError ( "Invalid input setup found for button " + name );
			return false;
		}
	}
}

public class InputManager : SceneSingleton<InputManager>
{
    internal GameObject GetComboButtonPrefab( InputActionName name )
    {
        foreach ( InputDefinition def in JoystickDefinition )
            if ( def.name == name )
                return def.buttonPrefab;

        Debug.LogError ( "Can't find any prefab. There is no definition found for name " + name );

        return null;
    }

    public InputDefinition[] JoystickDefinition;

	
}
