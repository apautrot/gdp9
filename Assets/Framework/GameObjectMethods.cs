#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PositionSpace
{
	Local,
	World
}

public enum GetChildOption
{
	ChildOnly,
	FullHierarchy
}

public enum GetBoundsOption
{
	ChildOnly,
	FullHierarchy
}

public enum FadeOutEndAction
{
	None,
	SetInactive,
	Destroy
}

public static class RandomEnum
{
	public static T Of<T> ()
	{
		if ( !typeof ( T ).IsEnum )
			throw new System.InvalidOperationException ( "Must use Enum type" );

		System.Array enumValues = System.Enum.GetValues ( typeof ( T ) );
		return (T)enumValues.GetValue ( RandomInt.Range ( 0, enumValues.Length-1 ) );
	}
}

public static class GameObjects
{
	public static void BroadcastMessageToScene ( string messageName, System.Object messageParameter = null )
	{
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType ( typeof ( GameObject ) );
		foreach ( GameObject go in gos )
		{
			if ( go && go.transform.parent == null )
			{
				go.gameObject.BroadcastMessage ( messageName, messageParameter, SendMessageOptions.DontRequireReceiver );
			}
		}
	}
}

public static class GameObjectExtensions
{
	public static Vector3 GetWorldScale ( this Transform transform )
	{
		Vector3 worldScale = transform.localScale;
		Transform parent = transform.parent;

		while ( parent != null )
		{
			worldScale = Vector3.Scale ( worldScale, parent.localScale );
			parent = parent.parent;
		}

		return worldScale;
	}

	public static void MoveChildTo ( this GameObject self, GameObject to )
	{
		List<Transform> childs = new List<Transform> ();
		for ( int i = 0; i < self.transform.childCount; i++ )
			childs.Add ( self.transform.GetChild ( i ) );

		foreach ( Transform child in childs )
			child.parent = to.transform;
	}

	public static T GetOrCreateComponent<T> ( this GameObject gameObject ) where T : Component
	{
		T component = gameObject.GetComponent<T> ();
		if ( component == null )
			component = gameObject.AddComponent<T> ();

		return component;
	}

	public static T GetComponentInParentHierarchy<T> ( this GameObject self ) where T : MonoBehaviour
	{
		GameObject parent = self.transform.parent.gameObject;
		if ( parent == null )
			return null;

		T t = parent.GetComponent<T> ();
		if ( t != null )
			return t;

		return parent.GetComponentAsInParentHierarchy<T> ();
	}

	public static T GetComponentAsInParentHierarchy<T> ( this GameObject self ) where T : class
	{
		GameObject parent = self.transform.parent.gameObject;
		if ( parent == null )
			return null;

		T t = parent.GetComponentAs<T> ();
		if ( t != null )
			return t;

		return parent.GetComponentAsInParentHierarchy<T> ();
	}

	public static T GetComponentOfChildUnderPoint<T> ( this GameObject gameObject, Vector2 point ) where T : MonoBehaviour
	{
		GameObject go = gameObject.GetChildUnderPoint ( point );
		if ( go != null )
			return go.GetComponent<T> ();
		else
			return null;
	}

	static public T FindComponentInChildren<T> ( this GameObject self, bool recursive = false ) where T : Component
	{
		T component = null;

		Transform transform = self.transform;
		int count = transform.childCount;

		for ( int i = 0; i < count; i++ )
		{
			GameObject gameObject = transform.GetChild ( i ).gameObject;
			
			component = gameObject.GetComponent<T> ();
			
			if ( ( component == null ) && recursive )
				component = gameObject.GetComponentInChildren<T> ();

			if ( component != null )
				break;
		}

		return component;
	}

	public static T GetComponentAs<T> ( this GameObject self ) where T : class
	{
		Component[] all = self.GetComponents<Component>();
		for ( int i = 0 ; i < all.Length; i++ )
		{
			if ( all[i] is T )
				return all[i] as T;
		}

		return null;
	}

	public static T[] GetComponentsAs<T> ( this GameObject self ) where T : class
	{
		List<T> result = new List<T> ();
		Component[] all = self.GetComponents<Component> ();
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i] as T;
			if ( t != null )
				result.Add ( t );
		}

		return result.ToArray();
	}

	public static GameObject GetChildUnderPoint ( this GameObject gameObject, Vector2 point )
	{
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D> ();
			if ( boxCollider2D != null )
			{
				Rect r = RectUtil.FromCollider ( boxCollider2D );
				// Debug.Log ( "Rect : " + r.ToString() + " - Point : " + point.ToString() );
				if ( r.Contains ( point ) )
					return child;
			}
		}

		return null;
	}

	public static bool IsUnderPoint ( this GameObject gameObject, Vector2 point )
	{
		BoxCollider2D boxCollider2D = gameObject.GetComponent<BoxCollider2D> ();
		if ( boxCollider2D != null )
		{
			Rect r = RectUtil.FromCollider ( boxCollider2D );
			return r.Contains ( point );
		}

		CircleCollider2D circleCollider2D = gameObject.GetComponent<CircleCollider2D> ();
		if ( circleCollider2D != null )
		{
			Vector2 dist = ( (Vector2)circleCollider2D.transform.position + circleCollider2D.offset ) - point;
			return ( dist.magnitude < circleCollider2D.radius );
		}

		Debug.LogWarning ( "IsUnderPoint : No collider on object " + gameObject.name );
		return false;
	}

	public static bool IsUnderMouse ( this GameObject gameObject )
	{
		int count = Camera.allCameras.Length;
		for ( int i = 0; i < count; i++ )
		{
			Camera camera = Camera.allCameras[i];
			int layerMask = 1 << gameObject.layer;
			if ( ( layerMask & camera.cullingMask ) != 0 )
			{
				Vector3 touchPositionOnObjectPlane = TouchInput.GetWorldPositionOnXYPlane ( camera, Input.mousePosition, gameObject.transform.position.z );
				bool isUnderTouch = gameObject.IsUnderPoint ( touchPositionOnObjectPlane );
				// bool isUnderMouse = gameObject.IsUnderPoint ( camera.ScreenToWorldPoint ( Input.mousePosition ) );
				return isUnderTouch;
			}
		}

		return false;
	}

	public static Bounds GetBounds ( this GameObject gameObject, GetBoundsOption option = GetBoundsOption.FullHierarchy )
	{
		if ( option == GetBoundsOption.ChildOnly )
		{
			if ( gameObject.GetComponent<Renderer>() != null )
				return gameObject.GetComponent<Renderer>().bounds;
			else
				return new Bounds ();
		}
		else
		{
			Bounds b = gameObject.GetChildBounds ();
			if ( gameObject.GetComponent<Renderer> () != null )
			{
				Bounds rb = gameObject.GetComponent<Renderer> ().bounds;
				if ( rb.size != Vector3.zero )
					b.Encapsulate ( rb );
			}

			return b;
		}
	}

	public static Bounds GetChildBounds ( this GameObject gameObject )
	{
		Bounds bounds = new Bounds ();
		if ( gameObject.transform.childCount > 0 )
		foreach ( Transform t in gameObject.transform )
		{
			Bounds childBound = t.gameObject.GetBounds ( GetBoundsOption.FullHierarchy );
			if ( childBound.size != Vector3.zero )
				if ( bounds.size == Vector3.zero )
					bounds = childBound;
				else
					bounds.Encapsulate ( childBound );
		}

 		if ( bounds.size == Vector3.zero )
 			return new Bounds ( gameObject.transform.position, Vector3.zero );

		return bounds;
	}

// 	public static Vector3 GetChildBounds ( this GameObject gameObject )
// 	{
// 		Vector3 max = new Vector3 ( float.MinValue, float.MinValue, float.MinValue );
// 		Vector3 min = new Vector3 ( float.MaxValue, float.MaxValue, float.MaxValue );
// 
// 		int count = gameObject.transform.childCount;
// 		for ( int i = 0; i < count; i++ )
// 		{
// 			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
// 			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D> ();
// 			if ( boxCollider2D != null )
// 			{
// 				Vector3 position = child.transform.localPosition;
// 				float left = position.x - ( boxCollider2D.size.x / 2 ) + boxCollider2D.center.x;
// 				float right = position.x + ( boxCollider2D.size.x / 2 ) + boxCollider2D.center.x;
// 				float bottom = position.y - ( boxCollider2D.size.y / 2 ) + boxCollider2D.center.y;
// 				float top = position.y + ( boxCollider2D.size.y / 2 ) + boxCollider2D.center.y;
// 
// 				min.x = System.Math.Min ( min.x, left );
// 				max.x = System.Math.Max ( max.x, right );
// 				min.y = System.Math.Min ( min.y, bottom );
// 				max.y = System.Math.Max ( max.y, top );
// 			}
// 		}
// 
// 		return max - min;
// 	}

	public static void ForEachChildDo ( this GameObject self, System.Action<GameObject> action )
	{
		for ( int i = 0; i < self.transform.childCount; i++ )
			action ( self.transform.GetChild ( i ).gameObject );
	}

	public static List<GameObject> GetChilds<T> ( this GameObject gameObject, GetChildOption option = GetChildOption.ChildOnly ) where T : MonoBehaviour
	{
		List<GameObject> childs = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.GetComponent<T>() != null )
			{
				childs.Add ( child );
				if ( option == GetChildOption.FullHierarchy )
					childs.AddRange ( child.GetChilds ( option ) );
			}
		}
		return childs;
	}

	public static List<GameObject> GetChilds ( this GameObject gameObject, GetChildOption option = GetChildOption.ChildOnly )
	{
		List<GameObject> childs = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			childs.Add ( child );
			if ( option == GetChildOption.FullHierarchy )
				childs.AddRange ( child.GetChilds ( option ) );
		}
		return childs;
	}

	public static GameObject FindChildByName ( this GameObject gameObject, string name, bool errorIfNotFound = true )
	{
// 		if ( gameObject == null )
// 			throw new System.Exception ( "FindChildByName(\"" + name + "\") executed on null reference" );

		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
				return child;
		}

		if ( errorIfNotFound )
		{
			Debug.LogError ( "Child " + name + " not found" );
			Debug.Break ();
		}

		return null;
	}

	public static T FindChildByName<T> ( this GameObject gameObject, string name, bool errorIfNotFound = true ) where T : Component
	{
		// 		if ( gameObject == null )
		// 			throw new System.Exception ( "FindChildByName(\"" + name + "\") executed on null reference" );

		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
			{
				T t = child.GetComponent<T> ();
				if ( t != null )
					return t;

				if ( errorIfNotFound )
				{
					Debug.LogError ( "Child " + name + " found, but no component with given type" );
					Debug.Break ();
				}
			}
		}

		if ( errorIfNotFound )
		{
			Debug.LogError ( "Child " + name + " not found" );
			Debug.Break ();
		}

		return null;
	}

	public static List<GameObject> FindChildsByName ( this GameObject gameObject, string name )
	{
		List<GameObject> list = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
				list.Add ( child );
		}
		return list;
	}

	public static T FindChildByComponent<T> ( this GameObject self ) where T : UnityEngine.Component
	{
		return self.GetComponentInChildren<T> ();
	}

	public static T[] FindChildsByComponent<T> ( this GameObject self, bool recursive = false, bool includeInactive = false ) where T : UnityEngine.Component
	{
		if ( recursive )
			return self.GetComponentsInChildren<T> ( includeInactive );
		else
		{
			List<T> list = new List<T> ();
			for ( int i = 0 ; i < self.transform.childCount; i++ )
			{
				T t = self.transform.GetChild ( i ).GetComponent<T> ();
				if ( ( t != null ) && ( includeInactive || t.gameObject.activeInHierarchy ) )
					list.Add ( t );
			}
			return list.ToArray ();
		}
	}

	public static List<T> FindObjectsOfTypeAs<T> () where T : class
	{
		List<T> result = new List<T>();
		GameObject[] all = Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) ) as GameObject[];
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i].GetComponentAs<T> ();
			if ( t != null )
				result.Add ( t );
		}
		return result;
	}

	public static T FindObjectOfTypeAs<T> () where T : class
	{
		GameObject[] all = Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) ) as GameObject[];
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i].GetComponentAs<T> ();
			if ( t != null )
				return t;
		}

		return null;
	}

	public static void SetLocal2DPosition ( this GameObject gameObject, float x, float y )
	{
		gameObject.transform.localPosition = new Vector3 ( x, y, gameObject.transform.position.z );
	}

	public static void Set2DPosition ( this GameObject gameObject, float x, float y )
	{
		gameObject.transform.position = new Vector3 ( x, y, gameObject.transform.position.z );
	}

	public static GameObject GetRootParent ( this GameObject self )
	{
		Transform parent = self.transform.parent;
		
		if ( parent == null )
			return null;
	
		while ( parent.transform.parent != null )
			parent = parent.transform.parent;

		return parent.gameObject;
	}

// 	public static void SetAlpha ( this GameObject self, float alpha )
// 	{
// 		SpriteRenderer sr = self.GetComponent<SpriteRenderer> ();
// 		if ( sr != null )
// 			sr.color = ColorUtils.fromColor ( sr.color, alpha );
// 	}

	public static void FadeIn ( this GameObject self, float duration = 0.5f, float alpha = 1 )
	{
		self.SetActive ( true );
		Renderer renderer = self.GetComponent<Renderer> ();
		if ( renderer != null )
		{
			renderer.material.SetAlpha ( 0 );
			renderer.material.alphaTo ( duration, alpha );
		}
	}

	public static void FadeOut ( this GameObject self, float duration = 0.5f, FadeOutEndAction action = FadeOutEndAction.Destroy )
	{
		Renderer renderer = self.GetComponent<Renderer> ();
		if ( renderer != null )
		{
			// renderer.material.SetAlpha ( 1 );
			GoTween tween = renderer.material.alphaTo ( duration, 0 );
			switch ( action )
			{
				case FadeOutEndAction.None: break;
				case FadeOutEndAction.SetInactive:
					tween.setOnCompleteHandler ( c => self.SetActive ( false ) );
					break;
				case FadeOutEndAction.Destroy:
					tween.setOnCompleteHandler ( c => self.DestroySelf() );
					break;
			}
		}
		else
			switch ( action )
			{
				case FadeOutEndAction.None: break;
				case FadeOutEndAction.SetInactive:
					self.SetActive ( false );
					break;
				case FadeOutEndAction.Destroy:
					self.DestroySelf ();
					break;
			}
	}


	public static void SetScale ( this GameObject self, float scale )
	{
		self.transform.localScale = new Vector3 ( scale, scale, scale );
	}

	public static void SetAlpha ( this Material self, float alpha, string propertyName = "_Color" )
	{
		Color c = self.GetColor ( propertyName );
		c.a = alpha;
		// self.color = c;
		self.SetColor ( propertyName, c );
	}

	public static void SetAlpha ( this GameObject self, float alpha, bool recursiveOnChildren = false )
	{
		if ( self.GetComponent<Renderer>() != null )
			if ( self.GetComponent<Renderer>().material != null )
				self.GetComponent<Renderer>().material.SetAlpha ( alpha );

		if ( recursiveOnChildren )
			for ( int i = 0; i < self.transform.childCount; i++ )
				self.transform.GetChild ( i ).gameObject.SetAlpha ( alpha, recursiveOnChildren );
	}

	public static void SetColor ( this GameObject self, Color color, bool recursiveOnChildren = false )
	{
		if ( self.GetComponent<Renderer>() != null )
			if ( self.GetComponent<Renderer>().material != null )
				self.GetComponent<Renderer>().material.SetColor ( color );

		if ( recursiveOnChildren )
			for ( int i = 0; i < self.transform.childCount; i++ )
				self.transform.GetChild ( i ).gameObject.SetColor ( color, recursiveOnChildren );
	}

	public static void SetSpriteColor ( this GameObject self, Color color )
	{
		SpriteRenderer sr = self.GetComponent<SpriteRenderer> ();
		if ( sr != null )
			sr.color = color;
	}

	public static AbstractGoTween colorTo ( this GameObject self, float duration, Color endValue, GoEaseType easeType = GoEaseType.Linear )
	{
		List<GameObject> all = self.GetChilds ( GetChildOption.FullHierarchy );
		all.Add ( self );
		AbstractGoTween tween = null;
		GoTweenFlow tweens = null;

		foreach ( var v in all )
		{
			if ( v.GetComponent<Renderer>() != null )
				if ( v.GetComponent<Renderer>().material != null )
				{
					if ( tween != null )
						if ( tweens == null )
						{
							tweens = new GoTweenFlow ();
							tweens.insert ( 0, tween );
						}

					tween = v.GetComponent<Renderer>().material.colorTo ( duration, endValue );
					if ( tweens != null )
						tweens.insert ( 0, tween );
				}
		}

		if ( tweens != null )
		{
			Go.addTween ( tweens );
			return tweens;
		}
		else
			return tween;
	}

	public static GoTween colorTo ( this SpriteRenderer self, float duration, Color endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().colorProp ( "color", endValue, isRelative ) );
	}

	static AbstractGoTween alphaTween ( this GameObject self, bool directionTo, float duration, float endValue, GoEaseType easeType, float delay )
	{
		List<GameObject> all = self.GetChilds ( GetChildOption.FullHierarchy );
		all.Add ( self );
		AbstractGoTween tween = null;
		GoTweenFlow tweens = null;

		foreach ( var v in all )
		{
			if ( v.GetComponent<Renderer>() != null )
				if ( v.GetComponent<Renderer>().material != null )
				{
					if ( tween != null )
						if ( tweens == null )
						{
							tweens = new GoTweenFlow ();
							tweens.insert ( 0, tween );
						}

					if ( directionTo )
						tween = v.GetComponent<Renderer>().material.alphaTo ( duration, endValue ).delays ( delay );
					else
						tween = v.GetComponent<Renderer>().material.alphaFrom ( duration, endValue ).delays ( delay );

					if ( tweens != null )
						tweens.insert ( 0, tween );
				}
		}

		if ( tweens != null )
		{
			Go.addTween ( tweens );
			return tweens;
		}
		else
			return tween;
	}

	public static AbstractGoTween alphaFrom ( this GameObject self, float duration, float endValue, GoEaseType easeType = GoEaseType.Linear, float delay = 0 )
	{
		return self.alphaTween ( false, duration, endValue, easeType, delay );
	}

	public static AbstractGoTween alphaTo ( this GameObject self, float duration, float endValue, GoEaseType easeType = GoEaseType.Linear, float delay = 0 )
	{
		return self.alphaTween ( true, duration, endValue, easeType, delay );
	}

	public static void DestroyAllChilds ( this GameObject self )
	{
		int count = self.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = self.transform.GetChild ( i ).gameObject;
			GameObject.Destroy ( child );
		}
	}

	public static void DestroyAllChilds<T> ( this GameObject self ) where T : MonoBehaviour
	{
		List<GameObject> list = self.GetChilds<T>();
		for ( int i = 0; i < list.Count; i++ )
			GameObject.Destroy ( list[i] );
	}

	public static bool DestroyChilds ( this GameObject self, string name )
	{
		List<GameObject> childs = self.FindChildsByName ( name );
		foreach ( GameObject child in childs )
			GameObject.Destroy ( child );

		return ( childs.Count > 0 );
	}

	public static bool DestroyChild ( this GameObject self, string name )
	{
		GameObject child = self.FindChildByName ( name );
		if ( child != null )
		{
			GameObject.Destroy ( child );
			return true;
		}
		else
			return false;
	}

	public static GameObject Instantiate ( GameObject prefab, Vector3 position, string name = null )
	{
		GameObject go = GameObject.Instantiate ( prefab ) as GameObject;
		go.transform.position = position;
		if ( name != null )
			go.name = name;
		return go;
	}

	public static GameObject InstantiateAtLocalPosition ( this GameObject self, GameObject prefab, Vector3 position, /*PositionSpace positionSpace, */string name = null )
	{
		GameObject child = GameObject.Instantiate ( prefab ) as GameObject;
//		if ( positionSpace == PositionSpace.Local )
			child.transform.position = self.transform.position + position;
// 		else
// 			child.transform.position = position;
		if ( name != null )
			child.name = name;
		return child;
	}

	public static SpriteRenderer InstantiateSprite ( Sprite sprite, Vector3 position, GameObject parent = null, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		GameObject spriteGO = new GameObject ();
		if ( parent != null )
		{
			spriteGO.transform.parent = parent.transform;
			if ( positionSpace == PositionSpace.Local )
				spriteGO.transform.localPosition = position;
			else
				spriteGO.transform.position = position;
		}
		else
			spriteGO.transform.position = position;

		if ( name != null )
			spriteGO.name = name;

		SpriteRenderer sr = spriteGO.GetOrCreateComponent<SpriteRenderer> ();
		if ( sprite == null )
			Debug.LogWarning ( "InstantiateSprite : Sprite parameter is null" );
		sr.sprite = sprite;
		return sr;
	}

	public static SpriteRenderer InstantiateSprite ( this GameObject self, Sprite sprite, Vector3 position, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		return InstantiateSprite ( sprite, position, self, positionSpace, name );
	}

	public static GameObject InstantiateChild ( this GameObject self, GameObject prefab, string name = null )
	{
		return self.InstantiateChild ( prefab, prefab.transform.localPosition, PositionSpace.Local, name );
	}

	public static GameObject InstantiateChild ( this GameObject self, GameObject prefab, Vector3 position, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		GameObject child = GameObject.Instantiate ( prefab ) as GameObject;
		child.transform.parent = self.transform;
		if ( positionSpace == PositionSpace.Local )
			child.transform.localPosition = position;
		else
			child.transform.position = position;
		if ( name != null )
			child.name = name;
		return child;
	}

	public static GameObject InstantiateReplace ( this GameObject self, GameObject prefab, bool destroySelf = true )
	{
		GameObject replacer = GameObject.Instantiate ( prefab ) as GameObject;
		replacer.transform.parent = self.transform.parent;
		replacer.transform.localPosition = self.transform.localPosition;
		replacer.transform.localRotation = self.transform.localRotation;
		replacer.transform.localScale = self.transform.localScale;
		replacer.name = self.name;
		if ( destroySelf )
			GameObject.Destroy ( self );
		return replacer;
	}

	public static void DestroySelf ( this GameObject self )
	{
		GameObject.Destroy ( self );
	}

	public static bool SendMessageToChild ( this GameObject self, string childName, string message )
	{
		GameObject child = self.FindChildByName ( childName );
		if ( child != null )
		{
			child.SendMessage ( message );
			return true;
		}
		else return false;
	}

	public static void WaitAndDo ( this MonoBehaviour self, float duration, System.Action action )
	{
		if ( duration != 0.0f )
			self.StartCoroutine ( WaitDoCoroutine ( duration, action ) );
		else
			action ();
	}

	private static System.Collections.IEnumerator WaitDoCoroutine ( float duration, System.Action action )
	{
		yield return new WaitForSeconds ( duration );
		action ();
		yield break;
	}

	public static double DistanceTo ( this GameObject self, GameObject other )
	{
		return ( self.transform.position - other.transform.position ).magnitude;
	}

	public static double DistanceTo ( this GameObject self, Vector3 position )
	{
		return ( self.transform.position - position ).magnitude;
	}

	public static double DistanceTo ( this Transform self, GameObject other )
	{
		return ( self.position - other.transform.position ).magnitude;
	}

	public static double DistanceTo ( this Transform self, Vector3 position )
	{
		return ( self.position - position ).magnitude;
	}

	internal static GoTweenChain Start ( this GoTweenChain self )
	{
		Go.addTween ( self );
		return self;
	}

	internal static GoTweenFlow Start ( this GoTweenFlow self )
	{
		Go.addTween ( self );
		return self;
	}

	internal static T GetRandomElement<T> ( this List<T> self )
	{
		int count = self.Count;
		if ( count == 0 )
			return default(T);
		else
			return self[RandomInt.Range ( 0, count - 1 )];
	}

	internal static T GetRandomElement<T> ( this T[] self )
	{
		int count = self.Length;
		if ( count == 0 )
			return default ( T );
		else
			return self[RandomInt.Range ( 0, count - 1 )];
	}

	internal static bool GetRandomElementNotIn<T> ( this T[] self, IEnumerable<T> list, out T element )
	{
		int count = self.Length;
		int initialIndex = RandomInt.Range ( 0, count - 1 );
		int index = initialIndex;
		
		while ( true )
		{
			element = self[index];
			if ( ! list.Contains<T> ( element ) )
				return true;

			index = ( index + 1 ) % count;
			if ( index == initialIndex )
				return false;
		}
	}

	public static bool Contains<T> ( this T[] self, T element )
	{
		return ( self.IndexOf ( element ) != -1 );
	}

	public static int IndexOf<T> ( this T[] self, T element )
	{
		for ( int i = 0; i < self.Length; i++ )
			if ( element.Equals ( self[i] ) )
				return i;

		return -1;
	}

	public static void AddOnlyOnce<T> ( this List<T> list, T element )
	{
		if ( ! list.Contains ( element ) )
			list.Add ( element );
	}

	public static void AddRangeOnlyOnce<T> ( this List<T> list, T[] elements )
	{
		int count = elements.Length;
		for ( int i = 0; i < count; i++ )
			list.AddOnlyOnce ( elements[i] );
	}

#if UNITY_EDITOR
	internal static bool IsPrefab ( this GameObject self )
	{
		return ( PrefabUtility.GetPrefabType ( self ) != PrefabType.None );
	}
#endif

	internal static Color WithAlphaAt ( this Color self, float alpha )
	{
		self.a = alpha;
		return self;
	}

	internal static Vector3 GetRandomPosition ( this BoxCollider2D self, PositionSpace space = PositionSpace.World )
	{
		Vector3 pos = ( space == PositionSpace.Local ) ?
						self.transform.localPosition
					:	self.transform.position;

		pos += (Vector3)self.offset + new Vector3 ( RandomFloat.Range ( -self.size.x, self.size.x ) / 2, RandomFloat.Range ( -self.size.y, self.size.y ) / 2, 0 );

		return pos;
	}

	internal static Vector3 GetRandomPosition ( this CircleCollider2D self, PositionSpace space = PositionSpace.World )
	{
		Vector3 pos = ( space == PositionSpace.Local ) ?
						self.transform.localPosition
					:	self.transform.position;

		Vector3 point = new Vector3 ( RandomFloat.Range ( -self.radius, self.radius ), 0, 0 );
		point = point.RotateZ ( RandomFloat.Range ( 0, 360 ) );

		pos += (Vector3)self.offset + point;

		return pos;
	}

	public static void Resize<T> ( this List<T> list, int sz, T c )
	{
		int cur = list.Count;
		if ( sz < cur )
			list.RemoveRange ( sz, cur - sz );
		else if ( sz > cur )
		{
			if ( sz > list.Capacity )//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
				list.Capacity = sz;
			for ( int i = 0 ; i < sz - cur; i++ )
				list.Add ( c );
		}
	}
	public static void Resize<T> ( this List<T> list, int sz ) where T : new ()
	{
		Resize ( list, sz, new T () );
	}

	public static void Shuffle<T> ( this IList<T> list )
	{
		int n = list.Count;
		while ( n > 1 )
		{
			n--;
			int k = RandomInt.Range ( 0, n );
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static bool Between ( this float self, float a, float b, bool inclusive = false )
	{
		if ( a > b )
		{
			float acopy = a;
			a = b;
			b = acopy;
		}

		return inclusive
			? a <= self && self <= b
			: a < self && self < b;
	}
}

public static class Physics2DExtensions
{
	public static void AddForce ( this Rigidbody2D rigidbody2D, Vector2 force, ForceMode mode = ForceMode.Force )
	{
		switch ( mode )
		{
			case ForceMode.Force:
				rigidbody2D.AddForce ( force );
				break;
			case ForceMode.Impulse:
				rigidbody2D.AddForce ( force / Time.fixedDeltaTime );
				break;
			case ForceMode.Acceleration:
				rigidbody2D.AddForce ( force * rigidbody2D.mass );
				break;
			case ForceMode.VelocityChange:
				rigidbody2D.AddForce ( force * rigidbody2D.mass / Time.fixedDeltaTime );
				break;
		}
	}

	public static void AddForce ( this Rigidbody2D rigidbody2D, float x, float y, ForceMode mode = ForceMode.Force )
	{
		rigidbody2D.AddForce ( new Vector2 ( x, y ), mode );
	}
}
