using System;
using System.Collections.Generic;
using System.Reflection;


static class SystemUtil
{
	static internal List<System.Type> GetAllSubClasses<T> ()
	{
		List<System.Type> types = new List<System.Type> ();
		Assembly current = Assembly.GetExecutingAssembly ();
		foreach ( System.Type type in current.GetTypes () )
			if ( type.IsSubclassOf ( typeof ( T ) ) )
				types.Add ( type );

		return types;
	}

	static internal string[] GetAllSubClassesNames<T> ()
	{
		List<string> names = new List<string> ();
		Assembly current = Assembly.GetExecutingAssembly ();
		foreach ( System.Type type in current.GetTypes () )
			if ( type.IsSubclassOf ( typeof ( T ) ) )
				names.Add ( type.Name );

		return names.ToArray ();
	}

	static internal T Instantiate<T> ( Type type ) where T : class
	{
		if ( ! type.IsSubclassOf ( typeof ( T ) ) )
			throw new System.Exception ( "Invalid parameter : argument and template argument are not compatible" );

		object o = System.Activator.CreateInstance ( type, true );
		T instance = o as T;
		return instance;
	}
}
