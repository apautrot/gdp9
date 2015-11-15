using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal enum ComboPaneResolution
{
	Failed,
	NotCompleted,
	Completed
}

public class ComboPane : MonoBehaviour
{
	internal bool isCompleted;
	private Combo combo;
	private List<ComboButton> buttons = new List<ComboButton>();
	private BoxCollider2D BoxCollider2D;

	public float Speed;

	private ComboPane paneAtRight;

	GameObject p1;
	GameObject p2;
	NinePatch back;

	void Awake()
	{
		BoxCollider2D = GetComponent<BoxCollider2D> ();
		if ( BoxCollider2D == null )
			Debug.LogError ( "There is no BoxCollider2D component on Game singleton" );

		combo = ComboList.Instance.GetCombo();

		Create ();

		Game.Instance.RegisterComboPane ( this );

		AlignLeftOn ( 640 );
    }

	void AlignLeftOn( float x )
	{
		// Debug.Log ( "Align on " + x );
		transform.position = transform.position.WithXReplacedBy ( x + ( BoxCollider2D.size.x / 2 ) );
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
		gameObject.DestroyAllChilds ();

		int count = combo.actions.Count;
		if ( count > 0 )
		{
			// float buttonWidth = Game.Instance.Settings.ComboButtonWidth
			float spacing = 150;

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

		Vector2 size = gameObject.GetBounds ().size;
		size.x *= 1 / transform.lossyScale.x;
		size.y *= 1 / transform.lossyScale.y;
		BoxCollider2D.size = size;
	}

	void Update()
	{
		// DebugWindow.Log ( name, "lossyScale", transform.lossyScale.ToStringEx() );

		transform.localPosition -= new Vector3 ( ComboList.Instance.ScrollingSpeed * Time.deltaTime, 0, 0 );

		{
			bool isInGameArea = BoxCollider2D.IsTouching ( Game.Instance.BoxCollider2D );
			// DebugWindow.Log ( name, "isInGameArea", isInGameArea );

			if ( !isInGameArea && transform.localPosition.x < 0 )
				gameObject.DestroySelf ();
		}

		if ( paneAtRight == null )
		{
			Vector2 rightCenter = new Vector2 ( BoxCollider2D.bounds.max.x, BoxCollider2D.bounds.center.y );
			bool isInGameArea = Game.Instance.Rectangle.Contains ( rightCenter );
			// DebugWindow.Log ( name, "is rightCenter InGameArea", isInGameArea );
			if ( isInGameArea )
			{
				GameObject prefab = Game.Instance.prefabs.ComboPane;
				if ( prefab == null )
					Debug.LogError ( "Missing Game.prefabs.ComboPane prefab" );

				GameObject paneAtRightGO = transform.parent.gameObject.InstantiateChild ( prefab );
				paneAtRightGO.transform.localScale = Vector3.one;
				paneAtRight = paneAtRightGO.GetComponent<ComboPane> ();
				if ( paneAtRight == null )
					Debug.LogError ( "Missing ComboPane component on Game.prefabs.ComboPane prefab" );

				paneAtRightGO.transform.localPosition = transform.localPosition;
				paneAtRight.AlignLeftOn ( rightCenter.x + ComboList.Instance.InterComboSpacing );
            }
		}
    }

	internal ComboPaneResolution UpdateState ( PlayerNumber playerNumber, List<InputActionName> inputs )
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

		for ( int i = 0 ; i < buttons.Count; i++ )
			buttons[i].Activated = ( i < activatedCount );

		if ( isFailed )
		{
			return ComboPaneResolution.Failed;
		}

		isCompleted = ( activatedCount == buttons.Count );
		if ( isCompleted )
		{
			transform.localScale = new Vector3 ( 0.5f, 0.5f, 0.5f );
			return ComboPaneResolution.Completed;
		}
		else
		{
			transform.localScale = Vector3.one;
			return ComboPaneResolution.NotCompleted;
		}
	}
}
