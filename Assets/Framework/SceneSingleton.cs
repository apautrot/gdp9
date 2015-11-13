using UnityEngine;
using System.Collections;



public class SceneSingleton<T> : MonoBehaviour, ISingleton
 	where T : MonoBehaviour, ISingleton
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = GameObject.FindObjectOfType<T> () as T;
				if ( instance == null )
					throw new System.Exception ( "Scene singleton " + typeof ( T ).ToString () + " not found in scene." );

				instance.OnSingletonCreated ();
			}
			return instance;
		}
	}

	public static bool InstanceCreated
	{
		get
		{
			if ( instance == null )
			{
				instance = GameObject.FindObjectOfType<T> () as T;
				if ( instance != null )
					instance.OnSingletonCreated ();
			}

			return instance != null;
		}
	}

	void OnDestroy ()
	{
		if ( this == instance )
			instance = null;
	}

	public virtual void OnSingletonCreated ()
	{
	}
}