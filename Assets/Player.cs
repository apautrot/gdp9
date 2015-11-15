using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputConfiguration
{
	Keyboard,
	Joystick1,
	Joystick2
}

public enum PlayerMonster
{
	Godzilla,
	Poulpe,
	Robot
}

public enum PlayerSide
{
	Left,
	Right
}

public class Player : MonoBehaviour
{
	public PlayerSide playerSide;
	public Player opponent;
	public PlayerMonster playerMonster;
	public InputConfiguration inputConfiguration;

	internal SkeletonAnimation spineAnimation;

	void Awake()
	{
		spineAnimation = gameObject.FindChildByComponent<SkeletonAnimation> ();
		// spineAnimation = GetComponent<SkeletonAnimation> ();
		if ( spineAnimation == null )
		{
			switch ( playerMonster )
			{
				case PlayerMonster.Godzilla:
					spineAnimation = gameObject.InstantiateChild ( Game.Instance.prefabs.Godzilla ).GetComponent<SkeletonAnimation> ();
					break;
				case PlayerMonster.Poulpe:
					spineAnimation = gameObject.InstantiateChild ( Game.Instance.prefabs.Poulpe ).GetComponent<SkeletonAnimation> ();
					break;
				case PlayerMonster.Robot:
					spineAnimation = gameObject.InstantiateChild ( Game.Instance.prefabs.Robot ).GetComponent<SkeletonAnimation> ();
					break;
			}
		}
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
		{
			spineAnimation.loop = true;
			spineAnimation.AnimationName = "IDLE";
		}

		if ( Input.GetKeyDown ( KeyCode.F2 ) )
		{
			spineAnimation.loop = false;
			spineAnimation.AnimationName = "ATTACK";
		}

		if ( Input.GetKeyDown ( KeyCode.F3 ) )
		{
			spineAnimation.loop = false;
			spineAnimation.AnimationName = "HIT";
		}

		if ( Input.GetKeyDown ( KeyCode.F4 ) )
		{
			spineAnimation.loop = false;
			spineAnimation.AnimationName = "ANTE_ATTACK";
		}
	}

	internal void PrepareAttack()
	{
		Debug.Log ( "PrepareAttack" );

		spineAnimation.loop = false;
		spineAnimation.AnimationName = "ANTE_ATTACK";
		spineAnimation.state.Complete += AnteAttackAnimationComplete;
    }

	private void AnteAttackAnimationComplete ( Spine.AnimationState state, int trackIndex, int loopCount )
	{
		Debug.Log ( "AnteAttackAnimationComplete" );

		spineAnimation.state.Complete -= AnteAttackAnimationComplete;
		spineAnimation.loop = false;
		spineAnimation.AnimationName = "ATTACK";

		spineAnimation.state.Complete += AttackAnimationComplete;
	}

	private void AttackAnimationComplete( Spine.AnimationState state, int trackIndex, int loopCount )
	{
		Debug.Log ( "AttackAnimationComplete" );
		
		spineAnimation.state.Complete -= AttackAnimationComplete;
		spineAnimation.loop = true;
		spineAnimation.AnimationName = "IDLE";
	}

	internal void UpdateInputs()
	{
		List<InputActionName> newInputs = new List<InputActionName>();
		UpdateNewInputs ( newInputs );
		if ( newInputs.Count > 0 )
			inputsEntered.AddRange ( newInputs );

		bool oneCompleted = false;
		bool onePartiallyCompleted = false;

		foreach ( ComboPane cp in Game.Instance.ActiveComboPanes )
			if ( !cp.isCompleted )
			{
				ComboPaneResolution resolution = cp.TryResolvingState ( this, inputsEntered );
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

	void UpdateNewInputs ( List<InputActionName> newInputs )
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
