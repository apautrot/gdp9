using UnityEngine;
using System.Collections;

public class WaitForAKey : MonoBehaviour
{
	void Update ()
	{
		if ( Input.GetButton ( "J1A" ) || Input.GetButton ( "J2A" ) )
			Application.LoadLevel ( "Game" );
    }
}
