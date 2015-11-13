
public class CoreSingleton<T> : ISingleton where T : ISingleton, new ()
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = new T ();
				instance.OnSingletonCreated ();
			}
			return instance;
		}
	}

	internal static bool InstanceCreated
	{
		get { return instance != null; }
	}

	public virtual void OnSingletonCreated ()
	{
	}
}
