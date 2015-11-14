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

public class InputManager : SceneSingleton<InputManager>
{
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

    internal GameObject GetComboButtonPrefab( InputActionName name )
    {
        foreach ( InputDefinition def in CurrentDefinition )
            if ( def.name == name )
                return def.buttonPrefab;

        Debug.LogError ( "Can't find any prefab. There is no definition found for name " + name );

        return null;
    }

    public InputDefinition[] JoystickDefinition;
    public InputDefinition[] KeyboardDefinition;

    internal InputDefinition[] CurrentDefinition
	{
		get
		{
			return JoystickDefinition;
        }
	}

	internal List<InputActionName> NewInputsQueue = new List<InputActionName>();

	List<InputDefinition> activeDefinitions = new List<InputDefinition>();

    internal void Update ()
	{
        foreach ( InputDefinition def in CurrentDefinition )
        {
            def.DebugInWindow ();
        }

		foreach ( InputDefinition def in CurrentDefinition )
		{
			bool isActivated = def.IsActivated;
			if ( isActivated )
			{
				if ( !activeDefinitions.Contains ( def ) )
				{
					activeDefinitions.Add ( def );
					NewInputsQueue.Add ( def.name );
				}
			}
			else
				if ( activeDefinitions.Contains ( def ) )
					activeDefinitions.Remove ( def );
        }
    }
}
