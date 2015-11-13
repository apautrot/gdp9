using UnityEngine;
using System.Collections;



public interface ISingleton
{
	void OnSingletonCreated ();
}


public class Singleton<T> : MonoBehaviour, ISingleton
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
				{
					GameObject singletonManagerObject = GameObject.Find ( "Singletons" );
					if ( singletonManagerObject == null )
					{
						singletonManagerObject = new GameObject ();
						singletonManagerObject.name = "Singletons";
					}

					instance = singletonManagerObject.AddComponent<T> ();
				}

				MonoBehaviour mb = (MonoBehaviour)instance;
				GameObject.DontDestroyOnLoad ( mb.gameObject );
				instance.OnSingletonCreated ();
			}
			return instance;
		}
	}

	internal static T CreateInstance ()
	{
		if ( InstanceCreated )
			throw new System.Exception ( "Instance of " + typeof ( T ).Name + " already created" );

		return Instance;
	}

	internal static bool InstanceCreated
	{
		get { return instance != null; }
	}

	protected void OnDestroy ()
	{
		if ( this == instance )
			instance = null;
	}

	public virtual void OnSingletonCreated ()
	{
	}
}