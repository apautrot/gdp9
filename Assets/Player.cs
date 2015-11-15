using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputConfiguration
{
	Keyboard,
	Joystick1,
	Joystick2
}

public enum PlayerNumber
{
	A,
	B
}

public class Player : MonoBehaviour
{
	public PlayerNumber playerNumber;
	public InputConfiguration inputConfiguration;

	internal SkeletonAnimation spineAnimation;

	void Awake()
	{
		spineAnimation = GetComponent<SkeletonAnimation> ();
    }

	internal InputDefinition[] CurrentControls
	{
		get
		{
			switch ( inputConfiguration )
			{
				case InputConfiguration.Keyboard:
					return InputManager.Instance.KeyboardDefinition;

				case InputConfiguration.Joystick1:
					return InputManager.Instance.Joystick1Definition;

				case InputConfiguration.Joystick2:
					return InputManager.Instance.Joystick2Definition;

				default:
					return InputManager.Instance.KeyboardDefinition;
			}
		}
	}

	List<InputDefinition> activeDefinitions = new List<InputDefinition>();

	internal List<InputActionName> inputsEntered = new List<InputActionName>();

	void Update()
	{
		if ( Input.GetKeyDown ( KeyCode.F1 ) )
			spineAnimation.AnimationName = "IDLE";

		if ( Input.GetKeyDown ( KeyCode.F2 ) )
			spineAnimation.AnimationName = "ATTACK";

		if ( Input.GetKeyDown ( KeyCode.F3 ) )
			spineAnimation.AnimationName = "HIT";

		if ( Input.GetKeyDown ( KeyCode.F4 ) )
			spineAnimation.AnimationName = "ANTE_ATTACK";

		UpdateInputList ();

		// 		foreach ( ComboPane cp in ActiveComboPanes )
		// 		{
		// 			List<InputActionName> combinaison = cp.combinaison;
		// 			// combinaison.
		// 		}

		string inputsAsString = "";
		foreach ( var v in inputsEntered )
			inputsAsString += v.ToString () + " ";

		if ( Input.GetKeyDown ( KeyCode.Backspace ) )
			inputsEntered.Clear ();

		DebugWindow.Log ( "Input", "List", inputsAsString );
	}

	void UpdateInputList()
	{
		List<InputActionName> newInputs = new List<InputActionName>();
		UpdateInputs ( newInputs );
		if ( newInputs.Count > 0 )
			inputsEntered.AddRange ( newInputs );

		bool oneCompleted = false;
		bool onePartiallyCompleted = false;

		foreach ( ComboPane cp in Game.Instance.ActiveComboPanes )
			if ( !cp.isCompleted )
			{
				ComboPaneResolution resolution = cp.UpdateState ( playerNumber, inputsEntered );
				switch ( resolution )
				{
					case ComboPaneResolution.Failed:
						break;
					case ComboPaneResolution.Completed:
						oneCompleted = true;
						break;
					case ComboPaneResolution.NotCompleted:
						onePartiallyCompleted = true;
						break;
				}
			}

		if ( oneCompleted )
			inputsEntered.Clear ();
		else if ( !onePartiallyCompleted )
			inputsEntered.Clear ();
	}

	void UpdateInputs ( List<InputActionName> newInputs )
	{
		foreach ( InputDefinition def in CurrentControls )
		{
			def.DebugInWindow ( inputConfiguration.ToString() );
		}

		foreach ( InputDefinition def in CurrentControls )
		{
			bool isActivated = def.IsActivated;
			if ( isActivated )
			{
				if ( !activeDefinitions.Contains ( def ) )
				{
					activeDefinitions.Add ( def );
					newInputs.Add ( def.name );
				}
			}
			else
				if ( activeDefinitions.Contains ( def ) )
				activeDefinitions.Remove ( def );
		}
	}
}
