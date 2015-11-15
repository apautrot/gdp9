using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : SceneSingleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		public GameObject ComboPane;
		public GameObject Godzilla;
		public GameObject Robot;
		public GameObject Poulpe;
		public GameObject HitFx;
	}

	public Prefabs prefabs;



	[System.Serializable]
	public class Sounds
	{
		public AudioClip GodzillaAnteAttack;
		public AudioClip PoulpeAnteAttack;
		public AudioClip RobotAnteAttack;

		public AudioClip GodzillaAttack;
		public AudioClip PoulpeAttack;
		public AudioClip RobotAttack;

		public AudioClip GodzillaHit;
		public AudioClip PoulpeHit;
		public AudioClip RobotHit;

		public AudioClip StartSound;
	}

	public Sounds sounds = new Sounds();



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

	internal List<ComboPane> ActiveComboPanes = new List<ComboPane>();

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

	public Player leftPlayer;
	public Player rightPlayer;

	void Update()
	{
// 		if ( Input.GetKeyDown ( KeyCode.F1 ) )
// 			Application.LoadLevel ( "GodzillaWon" );
// 
// 		if ( Input.GetKeyDown ( KeyCode.F2 ) )
// 			Application.LoadLevel ( "PoulpeWon" );
// 
// 		if ( Input.GetKeyDown ( KeyCode.F3 ) )
// 			Application.LoadLevel ( "RobotWon" );

		leftPlayer.UpdateInputs ();
		rightPlayer.UpdateInputs ();

		foreach ( ComboPane cp in ActiveComboPanes )
			cp.UpdateGraphicalState ();
    }

	// public Monster MonsterLeft;
	
}
