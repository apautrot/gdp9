using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using System;

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

	public LifeBar lifeBar;

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

	float AnteAttackDuration
	{
		get
		{
			switch ( playerMonster )
			{
				case PlayerMonster.Godzilla:
					return 1;
				case PlayerMonster.Poulpe:
					return 1;
				case PlayerMonster.Robot:
					return 1;
			}
			return 0;
		}
	}

	internal void PrepareAttack ( int attackPower )
	{
		spineAnimation.loop = false;
		spineAnimation.AnimationName = "ANTE_ATTACK";
		spineAnimation.state.Complete += AnteAttackAnimationComplete;

		this.WaitAndDo ( AnteAttackDuration, () => AttackOpponent ( attackPower ) );
	}

	private void AnteAttackAnimationComplete ( Spine.AnimationState state, int trackIndex, int loopCount )
	{
		spineAnimation.state.Complete -= AnteAttackAnimationComplete;
		spineAnimation.loop = false;
		spineAnimation.AnimationName = "ATTACK";

		spineAnimation.state.Complete += AttackAnimationComplete;
	}

	private void AttackAnimationComplete( Spine.AnimationState state, int trackIndex, int loopCount )
	{
		spineAnimation.state.Complete -= AttackAnimationComplete;
		spineAnimation.loop = true;
		spineAnimation.AnimationName = "IDLE";
	}

	private void AttackOpponent( int attackPower )
	{
		SkeletonAnimation fx = GameObject.Instantiate ( Game.Instance.prefabs.HitFx ).GetComponent<SkeletonAnimation>();
		switch ( playerMonster )
		{
			case PlayerMonster.Godzilla:
				fx.AnimationName = "FX_Attack_Godzilla";
				break;
			case PlayerMonster.Poulpe:
				fx.AnimationName = "FX_Attack_Poulpe";
				break;
			case PlayerMonster.Robot:
				fx.AnimationName = "FX_Attack_Robot";
				break;
		}
		if ( playerSide == PlayerSide.Right )
		{
			Vector3 ls = fx.transform.localScale;
			ls.y = -ls.y;
			fx.transform.localScale = ls;
        }
		fx.transform.position = AttackFxStartPosition;
		fx.transform
			.positionTo ( 0.5f, opponent.HitEndPosition )
			.eases ( GoEaseType.QuadInOut )
			.setOnCompleteHandler ( c => {
				opponent.OnHit ( attackPower, playerMonster );
				fx.gameObject.DestroySelf ();
            } );
    }

	Vector3 AttackFxStartPosition
	{
		get
		{
			return transform.position + new Vector3 (
				( playerSide == PlayerSide.Left ) ? 200 : -200
				, 400, 0 );
		}
	}

	Vector3 HitEndPosition
	{
		get
		{
			return transform.position + new Vector3 (
				( playerSide == PlayerSide.Left ) ? 200 : -200
				, 400, 0 );
		}
	}

	internal Vector3 ComboCollectPosition
	{
		get
		{
			return transform.position + new Vector3 ( 0, 400, 0 );
		}
	}

	internal void OnHit ( int attackPower, PlayerMonster fromMonster )
	{
		SkeletonAnimation fx = GameObject.Instantiate ( Game.Instance.prefabs.HitFx ).GetComponent<SkeletonAnimation>();
		switch ( fromMonster )
		{
			case PlayerMonster.Godzilla:
				fx.AnimationName = "FX_ImpactAttack_Godzilla";
				break;
			case PlayerMonster.Poulpe:
				fx.AnimationName = "FX_ImpactAttack_Poulpe";
				break;
			case PlayerMonster.Robot:
				fx.AnimationName = "FX_ImpactAttack_Robot";
				break;
		}
		if ( playerSide == PlayerSide.Right )
		{
			Vector3 ls = fx.transform.localScale;
			ls.y = -ls.y;
			fx.transform.localScale = ls;
		}
		fx.transform.position = HitEndPosition;
		fx.transform
			.scaleTo ( 0.125f, 2.0f )
			.eases ( GoEaseType.QuadInOut );
		// 			.setOnCompleteHandler ( c =>
		// 			{
		// 				fx.gameObject.DestroySelf ();
		// 			} );
		fx.state.Complete += ( Spine.AnimationState state, int trackIndex, int loopCount )
			=> fx.gameObject.DestroySelf ();

		lifeBar.Life = Mathf.Max ( lifeBar.Life - ( attackPower * 0.05f ), 0 );
		lifeBar.UpdateColor ();

		spineAnimation.state.Complete += HitAnimationComplete;
		spineAnimation.loop = false;
		spineAnimation.AnimationName = "HIT";
	}

	private void HitAnimationComplete( Spine.AnimationState state, int trackIndex, int loopCount )
	{
		spineAnimation.state.Complete -= HitAnimationComplete;
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
