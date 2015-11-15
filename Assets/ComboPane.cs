using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal enum ComboPaneResolution
{
	Failed,
	NotCompleted,
	Completed
}

enum ComboPaneState
{
	Scrolling,
	Absorbed																											
}

public class ComboPane : MonoBehaviour
{
	ComboPaneState state;

	internal bool isCompleted;
	private Combo combo;
	private List<ComboButton> buttons = new List<ComboButton>();
	private BoxCollider2D BoxCollider2D;

	public float Speed;

	GameObject p1;
	GameObject p2;
	NinePatch back;

	internal int Size
	{
		get
		{
			return combo.actions.Count;
		}
	}

	void Awake()
	{
		back = gameObject.FindChildByName ( "back" ).GetComponent<NinePatch> ();
		p1 = gameObject.FindChildByName ( "p1" );
		p2 = gameObject.FindChildByName ( "p2" );

		BoxCollider2D = GetComponent<BoxCollider2D> ();
		if ( BoxCollider2D == null )
			Debug.LogError ( "There is no BoxCollider2D component on Game singleton" );

		combo = ComboList.Instance.GetCombo();

		Create ();

		Game.Instance.RegisterComboPane ( this );
    }

	internal void AlignLeftOn ( float x, float y )
	{
		// Debug.Log ( "Align on " + x );
		transform.localPosition = transform.localPosition.WithXYReplacedBy ( x + ( BoxCollider2D.size.x / 2 ), y );
	}

// 	void Start()
// 	{
// // 		transform
// // 			.localPositionTo ( 3, new Vector3 ( -1280, 0, 0 ), true )
// // 			.eases ( GoEaseType.Linear )
// // 			.setOnCompleteHandler ( c => gameObject.DestroySelf () );
// 	}

	void OnDestroy()
	{
		if ( Game.InstanceCreated )
			Game.Instance.UnregisterComboPane ( this );
    }

	void Create()
	{
		// gameObject.DestroyAllChilds ();

		float spacing = 150;
		int count = combo.actions.Count;
		if ( count > 0 )
		{
			// float buttonWidth = Game.Instance.Settings.ComboButtonWidth
			float x = - ( ( ( count - 1 ) * spacing ) / 2 );

			foreach ( InputActionName name in combo.actions )
			{
				GameObject prefab = InputManager.Instance.GetComboButtonPrefab ( name );
				GameObject buttonGO = gameObject.InstantiateChild ( prefab );
				buttonGO.transform.localPosition = new Vector3 ( x, 0, 0 );
				buttonGO.transform.localScale = Vector3.one;

				ComboButton button = buttonGO.GetComponent<ComboButton> ();
				if ( button == null )
					Debug.LogError ( "There is no ComboButton component on the game object " + buttonGO );
				buttons.Add ( button );

				x += spacing;
			}
		}

		float width = ( ( ( count - 1 ) * spacing ) );
		Vector2 size = new Vector2 ( width + 128 + 32, 128 + 32 );

		// Debug.Log ( "Size:" + size );

		BoxCollider2D.size = size;

		back.Width = size.x;
		back.Height = size.y;
		back.Recreate ();
    }

	void Update()
	{
		if ( state == ComboPaneState.Scrolling )
		{
			transform.localPosition -= new Vector3 ( ComboList.Instance.ScrollingSpeed * Time.deltaTime, 0, 0 );

			{
				bool isInGameArea = BoxCollider2D.IsTouching ( Game.Instance.BoxCollider2D );
				// DebugWindow.Log ( name, "isInGameArea", isInGameArea );

				if ( !isInGameArea && transform.localPosition.x < 0 )
					gameObject.DestroySelf ();
			}

			// CreateRightPaneIfNeeded ();
        }
    }

	internal int activatedCountPlayerLeft = 0;
	internal int activatedCountPlayerRight = 0;

	internal ComboPaneResolution TryResolvingState ( Player player, List<InputActionName> inputs )
	{
		if ( isCompleted )
			return ComboPaneResolution.Completed;

		bool isFailed = false;

		int activatedCount = 0;
		for ( int i = 0; i < combo.actions.Count; i++ )
		{
			if ( i < inputs.Count )
				if ( inputs[i] == combo.actions[i] )
					activatedCount++;
				else
				{
					isFailed = true;
					break;
				}
        }

		if ( player.playerSide == PlayerSide.Left )
			activatedCountPlayerLeft = activatedCount;

		if ( player.playerSide == PlayerSide.Right )
			activatedCountPlayerRight = activatedCount;

		if ( isFailed )
		{
			return ComboPaneResolution.Failed;
		}

		isCompleted = ( activatedCount == buttons.Count );
		if ( isCompleted )
		{
			state = ComboPaneState.Absorbed;

			player.PrepareAttack ();

			GoTweenChain chain = new GoTweenChain();
			chain.append ( transform.scaleTo ( 0.125f, 1.5f ).eases ( GoEaseType.Linear ) );
			chain.append ( transform.scaleTo ( 0.125f, 1.0f ).eases ( GoEaseType.Linear ) );

			chain.insert ( 0.25f, transform.scaleTo ( 1.25f, 0.0f ).eases ( GoEaseType.QuadInOut ) );
			chain.insert ( 0.25f, transform.positionTo ( 1.25f,
				player.transform.position + new Vector3 ( 0, 400, 0 )
				).eases ( GoEaseType.QuadInOut ) );
			chain.setOnCompleteHandler ( c => {
				gameObject.DestroySelf ();
                } );
			chain.Start ();

			return ComboPaneResolution.Completed;
		}
		else
		{
			return ComboPaneResolution.NotCompleted;
		}
	}

	internal void UpdateGraphicalState()
	{
		int maxActivatedCount = Mathf.Max ( activatedCountPlayerLeft, activatedCountPlayerRight );
        for ( int i = 0; i < buttons.Count; i++ )
			buttons[i].Activated = ( i < maxActivatedCount );
	}
}
