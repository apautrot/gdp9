using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : SceneSingleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		public GameObject ComboPane;
	}

	public Prefabs prefabs;

	

	internal BoxCollider2D BoxCollider2D;

	internal Rect Rectangle;

	void Awake()
	{
		BoxCollider2D = GetComponent<BoxCollider2D> ();
		if ( BoxCollider2D == null )
			Debug.LogError ( "There is no BoxCollider2D component on Game singleton" );

		Rectangle = new Rect
		(
			BoxCollider2D.bounds.min,
			BoxCollider2D.bounds.size
		);
    }

	List<ComboPane> ActiveComboPanes = new List<ComboPane>();

	internal void RegisterComboPane( ComboPane comboPane )
	{
		ActiveComboPanes.Add ( comboPane );
	}

	internal void UnregisterComboPane( ComboPane comboPane )
	{
		ActiveComboPanes.Remove ( comboPane );
	}




	void Start()
	{

	}

	

	internal List<InputActionName> inputsEntered = new List<InputActionName>();

	void Update()
	{
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
		InputManager.Instance.Update ();
		if ( InputManager.Instance.NewInputsQueue.Count > 0 )
		{
			inputsEntered.AddRange ( InputManager.Instance.NewInputsQueue );
			InputManager.Instance.NewInputsQueue.Clear ();
        }

		bool oneCompleted = false;
		bool onePartiallyCompleted = false;

		foreach ( ComboPane cp in ActiveComboPanes )
			if ( !cp.isCompleted )
			{
				ComboPaneResolution resolution = cp.UpdateState ( inputsEntered );
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
		else if ( ! onePartiallyCompleted )
			inputsEntered.Clear ();
	}
}
