using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class ToolsSupport
{

	public static bool Hidden
	{
		get
		{
			System.Type type = typeof ( Tools );
			FieldInfo field = type.GetField ( "s_Hidden", BindingFlags.NonPublic | BindingFlags.Static );
			return ( (bool)field.GetValue ( null ) );
		}
		set
		{
			System.Type type = typeof ( Tools );
			FieldInfo field = type.GetField ( "s_Hidden", BindingFlags.NonPublic | BindingFlags.Static );
			field.SetValue ( null, value );
		}
	}
}

[CustomEditor ( typeof ( PolyMesh ) ), CanEditMultipleObjects]
public class PolyMeshEditor : Editor
{
	GeometrySceneGUIEditor sceneGUIEditor = new GeometrySceneGUIEditor();
	GenericMenu toolsMenu;

	void OnSceneGUI ()
	{
		if ( Selection.gameObjects.Length == 1 )
		{
			EditableCurvableGeometry editableGeometry = target as EditableCurvableGeometry;
			if ( editableGeometry != null )
			{
				bool rightClick = e.type == EventType.MouseDown && e.isMouse && ( e.button == 1 );
				if ( rightClick )
				{
					if ( toolsMenu == null )
					{
						toolsMenu = new GenericMenu ();
						toolsMenu.AddItem ( new GUIContent ( "Edit" ), false, SwitchEditionMode );
						toolsMenu.AddItem ( new GUIContent ( "Reset XForm" ), false, ResetXForm );
					}
					toolsMenu.ShowAsContext ();

					e.Use ();

					return;
				}
				else
					sceneGUIEditor.OnSceneGUI ( editableGeometry );
			}
		}
	}

	private void OnDestroy ()
	{
		sceneGUIEditor.editing = false;
	}

	private void SwitchEditionMode ()
	{
		sceneGUIEditor.SwitchEditionMode ();
	}

	private void ResetXForm ()
	{
		RecordMultipleObjectUndo ( "Reset XForm" );

		foreach ( PolyMesh pm in targets )
		{
			if ( pm.keyPoints.Count > 0 )
			{
				for ( int i = 0; i < pm.keyPoints.Count; i++ )
					pm.keyPoints[i] = pm.transform.TransformVector ( pm.keyPoints[i] );

				Vector3 min = new Vector3 ( float.MaxValue, float.MaxValue, float.MaxValue );
				Vector3 max = new Vector3 ( float.MinValue, float.MinValue, float.MinValue );

				for ( int i = 0; i < pm.keyPoints.Count; i++ )
				{
					min = Vector3.Min ( min, pm.keyPoints[i] );
					max = Vector3.Max ( max, pm.keyPoints[i] );
				}

				Vector3 center = ( min + max ) / 2;

				pm.transform.localPosition += center;

				for ( int i = 0; i < pm.keyPoints.Count; i++ )
					pm.keyPoints[i] -= center;

				pm.transform.localEulerAngles = Vector3.zero;
				pm.transform.localScale = Vector3.one;

				( pm as EditableCurvableGeometry ).NotifyGeometryChange ();

				sceneGUIEditor.keyPoints = null;
			}
		}

		Resources.UnloadUnusedAssets ();
	}

	private void NotifyGeometryChange ()
	{
		geometry.NotifyGeometryChange ();
		Resources.UnloadUnusedAssets ();
	}

	public override void OnInspectorGUI ()
	{
		if ( target == null )
		{
			return;
		}

		if ( polyMesh.keyPoints.Count == 0 )
			CreateSquare ( polyMesh, 0.5f );

		bool hasSpriteShader = false;
		{
			MeshRenderer meshRenderer = polyMesh.GetComponent<MeshRenderer> ();
			if ( meshRenderer != null )
				if ( meshRenderer.sharedMaterial != null )
				{
					Shader shader = meshRenderer.sharedMaterial.shader;
					if ( shader != null )
						if ( shader.name == "Sprites/Default" )
							hasSpriteShader = true;
				}
		}

		if ( hasSpriteShader )
		{
			GUI.changed = false;
			Color color = EditorGUILayout.ColorField ( "Color", polyMesh.Color );
			if ( GUI.changed )
			{
				GUI.changed = false;

				foreach ( PolyMesh pm in targets )
				{
					pm.Color = color;
// 					MeshRenderer mr = pm.GetComponent<MeshRenderer> ();
// 					if ( mr != null )
// 						if ( mr.sharedMaterial != null )
// 						{
// 							Shader shader = mr.sharedMaterial.shader;
// 							if ( shader != null )
// 								if ( shader.name == "Sprites/Default" )
// 								{
// 									Material material = new Material ( shader );
// 									material.SetColor ( "_Color", color );
// 									mr.material = material;
// 								}
// 						}
				}

				Resources.UnloadUnusedAssets ();
			}
		}
		else if ( GUILayout.Button ( "Apply Sprite/Default shader" ) )
		{
			foreach ( PolyMesh pm in targets )
			{
				MeshRenderer mr = pm.GetComponent<MeshRenderer> ();
				if ( mr != null )
				{
					Shader shader = Shader.Find ( "Sprites/Default" );
					if ( shader != null )
					{
						Material material = new Material ( shader );
						mr.material = material;
					}
				}
			}
		}


		UnityToolbag.SortingLayerExposedEditor.DrawSortingLayerGUI ( target, targets );

		if ( targets.Length == 1 )
		{
			//Toggle editing mode
			if ( sceneGUIEditor.editing )
			{
				if ( GUILayout.Button ( "Stop Editing" ) )
				{
					sceneGUIEditor.editing = false;
					HideWireframe ( false );
				}
			}
			else if ( GUILayout.Button ( "Edit PolyMesh" ) )
			{
				sceneGUIEditor.editing = true;
				HideWireframe ( hideWireframe );
			}
		}

		if ( GUILayout.Button ( "Reset XForm" ) )
		{
			ResetXForm ();
			return;
		}

		if ( targets.Length > 1 )
		{
			return;
		}

		if ( polyMesh.gameObject.GetComponent<PolygonCollider2D> () == null )
			if ( GUILayout.Button ( "Create Collider" ) )
			{
				RecordDeepUndo ();
				polyMesh.gameObject.AddComponent<PolygonCollider2D> ();
				NotifyGeometryChange ();
			}

		gridSnap = EditorGUILayout.FloatField ( "Grid Snap", gridSnap );

//		globalSnap = EditorGUILayout.Toggle ( "Global Snap", globalSnap );

		//UV settings
		if ( uvSettings = EditorGUILayout.Foldout ( uvSettings, "UVs" ) )
		{
			Sprite sprite = EditorGUILayout.ObjectField ( "Sprite", polyMesh.sprite, typeof ( Sprite ), false ) as Sprite;
			if ( GUI.changed )
			{
				RecordUndo ();
				polyMesh.sprite = sprite;
				polyMesh.BuildMaterial ();
				Resources.UnloadUnusedAssets ();
			}
			if ( sprite != null )
			{
				var deformableSprite = EditorGUILayout.Toggle ( "Deformable Sprite", polyMesh.deformableSprite );
				if ( GUI.changed )
				{
					RecordUndo ();
					polyMesh.deformableSprite = deformableSprite;
				}

				if ( deformableSprite )
				{
					if ( GUILayout.Button ( "Resize geometry" ) )
					{
						float hw = sprite.texture.width / 2;
						float hh = sprite.texture.height / 2;
						polyMesh.keyPoints.Resize ( 4 );
						polyMesh.keyPoints[0] = new Vector3 ( hw, hh, 0 );
						polyMesh.keyPoints[1] = new Vector3 ( hw, -hh, 0 );
						polyMesh.keyPoints[2] = new Vector3 ( -hw, -hh, 0 );
						polyMesh.keyPoints[3] = new Vector3 ( -hw, hh, 0 );
					}
				}
			}
			else
			{
				var uvPosition = EditorGUILayout.Vector2Field ( "Position", polyMesh.uvPosition );
				var uvScale = EditorGUILayout.FloatField ( "Scale", polyMesh.uvScale );
				var uvRotation = EditorGUILayout.Slider ( "Rotation", polyMesh.uvRotation, -180, 180 ) % 360;
				if ( uvRotation < -180 )
					uvRotation += 360;
				if ( GUI.changed )
				{
					RecordUndo ();
					polyMesh.uvPosition = uvPosition;
					polyMesh.uvScale = uvScale;
					polyMesh.uvRotation = uvRotation;
				}
				if ( GUILayout.Button ( "Reset UVs" ) )
				{
					polyMesh.uvPosition = Vector3.zero;
					polyMesh.uvScale = 1;
					polyMesh.uvRotation = 0;
				}
			}
		}

		/*
		//Mesh settings
		if ( meshSettings = EditorGUILayout.Foldout ( meshSettings, "Mesh" ) )
		{
			var curveDetail = EditorGUILayout.Slider ( "Curve Detail", polyMesh.curveDetail, 0.01f, 1f );
			curveDetail = Mathf.Clamp ( curveDetail, 0.01f, 1f );
			if ( GUI.changed )
			{
				RecordUndo ();
				polyMesh.curveDetail = curveDetail;
			}

			//Buttons
			EditorGUILayout.BeginHorizontal ();
			if ( GUILayout.Button ( "Build Mesh" ) )
				NotifyGeometryChange ();

			// 			if (GUILayout.Button("Make Mesh Unique"))
			// 			{
			// 				RecordUndo();
			// 				polyMesh.GetComponent<MeshFilter>().mesh = null;
			// 				NotifyGeometryChange ();
			// 			}
			EditorGUILayout.EndHorizontal ();
		}

		//Create collider
		if ( colliderSettings = EditorGUILayout.Foldout ( colliderSettings, "Collider" ) )
		{
			//Collider depth
			var colliderDepth = EditorGUILayout.FloatField ( "Depth", polyMesh.colliderDepth );
			colliderDepth = Mathf.Max ( colliderDepth, 0.01f );
			var buildColliderEdges = EditorGUILayout.Toggle ( "Build Edges", polyMesh.buildColliderEdges );
			var buildColliderFront = EditorGUILayout.Toggle ( "Build Front", polyMesh.buildColliderFront );
			if ( GUI.changed )
			{
				RecordUndo ();
				polyMesh.colliderDepth = colliderDepth;
				polyMesh.buildColliderEdges = buildColliderEdges;
				polyMesh.buildColliderFront = buildColliderFront;
			}

			if ( GUILayout.Button ( "Create Collider" ) )
			{
				RecordDeepUndo ();
				polyMesh.gameObject.AddComponent<PolygonCollider2D> ();
			}

			//Destroy collider
// 			if ( polyMesh.meshCollider == null )
// 			{
// 				if ( GUILayout.Button ( "Create Collider" ) )
// 				{
// 					RecordDeepUndo ();
// 					polyMesh.meshCollider = polyMesh.gameObject.GetOrCreateComponent<PolygonCollider2D> ();
// 					// 					var obj = new GameObject("Collider", typeof(MeshCollider));
// 					// 					polyMesh.meshCollider = obj.GetComponent<MeshCollider>();
// 					// 					obj.transform.parent = polyMesh.transform;
// 					// 					obj.transform.localPosition = Vector3.zero;
// 				}
// 			}
// 			else if ( GUILayout.Button ( "Destroy Collider" ) )
// 			{
// 				RecordDeepUndo ();
// 				// DestroyImmediate ( polyMesh.meshCollider.gameObject );
// 				DestroyImmediate ( polyMesh.meshCollider );
// 			}
		}

		//Update mesh
		if ( GUI.changed )
			NotifyGeometryChange ();

		//Editor settings
		if ( editorSettings = EditorGUILayout.Foldout ( editorSettings, "Editor" ) )
		{
			gridSnap = EditorGUILayout.FloatField ( "Grid Snap", gridSnap );
			autoSnap = EditorGUILayout.Toggle ( "Auto Snap", autoSnap );
			globalSnap = EditorGUILayout.Toggle ( "Global Snap", globalSnap );
			EditorGUI.BeginChangeCheck ();
			hideWireframe = EditorGUILayout.Toggle ( "Hide Wireframe", hideWireframe );
			if ( EditorGUI.EndChangeCheck () )
				HideWireframe ( hideWireframe );

			editKey = (KeyCode)EditorGUILayout.EnumPopup ( "[Toggle Edit] Key", editKey );
			selectAllKey = (KeyCode)EditorGUILayout.EnumPopup ( "[Select All] Key", selectAllKey );
			splitKey = (KeyCode)EditorGUILayout.EnumPopup ( "[Split] Key", splitKey );
			extrudeKey = (KeyCode)EditorGUILayout.EnumPopup ( "[Extrude] Key", extrudeKey );
		}

		//Editor settings
		if ( infoSettings = EditorGUILayout.Foldout ( infoSettings, "Infos" ) )
		{
			Vector3 min = new Vector3 ( float.MaxValue, float.MaxValue, float.MaxValue );
			Vector3 max = new Vector3 ( float.MinValue, float.MinValue, float.MinValue );

			for ( int i = 0; i < polyMesh.keyPoints.Count; i++ )
			{
				min = Vector3.Min ( min, polyMesh.keyPoints[i] );
				max = Vector3.Max ( max, polyMesh.keyPoints[i] );
			}

			Vector3 s = polyMesh.transform.lossyScale;

			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			EditorGUILayout.LabelField ( "Local size : " + ( max.x - min.x ) + " x " + ( max.y - min.y ) );
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			EditorGUILayout.LabelField ( "World size : " + ( ( max.x - min.x ) * s.x ) + " x " + ( ( max.y - min.y ) * s.y ) );
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();
		}
		 * */
	}

	void RecordMultipleObjectUndo ( string action )
	{
		Undo.RecordObjects ( targets, action );
	}

	void RecordUndo ()
	{
		Undo.RecordObject ( geometry as Object, "Geometry Changed" );
	}

	void RecordDeepUndo ()
	{
		Undo.RegisterFullObjectHierarchyUndo ( geometry as Object, geometry.ToString () );
	}

	void HideWireframe ( bool hide )
	{
		if ( ( geometry as Component ).GetComponent<Renderer> () != null )
			EditorUtility.SetSelectedWireframeHidden ( ( geometry as Component ).GetComponent<Renderer> (), hide );
	}

	//
	//
	//


	PolyMesh polyMesh
	{
		get { return (PolyMesh)target; }
	}

	EditableCurvableGeometry geometry
	{
		get { return (EditableCurvableGeometry)target; }
	}

	Event e
	{
		get { return Event.current; }
	}

	bool control
	{
		get { return Application.platform == RuntimePlatform.OSXEditor ? e.command : e.control; }
	}

	bool doSnap
	{
		get { return autoSnap ? !control : control; }
	}

	static bool meshSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_meshSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_meshSettings", value ); }
	}

	static bool colliderSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_colliderSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_colliderSettings", value ); }
	}

	static bool uvSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_uvSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_uvSettings", value ); }
	}

	static bool editorSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_editorSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_editorSettings", value ); }
	}

	static bool infoSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_infoSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_infoSettings", value ); }
	}

	static bool autoSnap
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_autoSnap", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_autoSnap", value ); }
	}

	static bool globalSnap
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_globalSnap", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_globalSnap", value ); }
	}

	static float gridSnap
	{
		get { return EditorPrefs.GetFloat ( "PolyMeshEditor_gridSnap", 1 ); }
		set { EditorPrefs.SetFloat ( "PolyMeshEditor_gridSnap", value ); }
	}

	static bool hideWireframe
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_hideWireframe", true ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_hideWireframe", value ); }
	}

	public KeyCode editKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_editKey", (int)KeyCode.Tab ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_editKey", (int)value ); }
	}

	public KeyCode selectAllKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_selectAllKey", (int)KeyCode.A ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_selectAllKey", (int)value ); }
	}

	public KeyCode splitKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_splitKey", (int)KeyCode.S ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_splitKey", (int)value ); }
	}

	public KeyCode extrudeKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_extrudeKey", (int)KeyCode.D ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_extrudeKey", (int)value ); }
	}

	//
	//
	//

	[MenuItem ( "GameObject/Create Other/PolyMesh", false, 1000 )]
	static void CreatePolyMesh ()
	{
		GameObject go = new GameObject ( "PolyMesh", typeof ( MeshFilter ), typeof ( MeshRenderer ) );
		PolyMesh polyMesh = go.AddComponent<PolyMesh> ();
		CreateSquare ( polyMesh, 10 );

		MeshRenderer renderer = go.GetOrCreateComponent<MeshRenderer> ();
		Shader shader = Shader.Find ( "Sprites/Default" );
		if ( shader != null )
		{
			Material material = new Material ( shader );
			material.name = "PolyMesh Material";
			// material.hideFlags = HideFlags.DontSave;
			renderer.material = material;
		}

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}

	static void CreateSquare ( PolyMesh polyMesh, float size )
	{
		polyMesh.keyPoints.AddRange ( new Vector3[] { new Vector3 ( size, size ), new Vector3 ( size, -size ), new Vector3 ( -size, -size ), new Vector3 ( -size, size ) } );
		polyMesh.curvePoints.AddRange ( new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero } );
		polyMesh.isCurve.AddRange ( new bool[] { false, false, false, false } );

		polyMesh.BuildMesh ();
		Resources.UnloadUnusedAssets ();
	}
}

class GeometrySceneGUIEditor
{
	// public UnityEngine.Object target { get; set; }
	enum State { Hover, Drag, BoxSelect, DragSelected, RotateSelected, ScaleSelected, Extrude }

	const float clickRadius = 0.10f;

	FieldInfo undoCallback;

	private bool _editing;
	internal bool editing
	{
		get { return _editing; }
		set
		{
			_editing = value;
			ToolsSupport.Hidden = value;
		}

	}
	State state;

	public List<Vector3> keyPoints;
	public List<Vector3> curvePoints;
	public List<bool> isCurve;

	Matrix4x4 worldToLocal;
	Quaternion inverseRotation;

	Vector3 mousePosition;
	Vector3 clickPosition;
	Vector3 screenMousePosition;
	MouseCursor mouseCursor = MouseCursor.Arrow;
	float snap;

	int dragIndex;
	List<int> selectedIndices = new List<int> ();
	int nearestLine;
	Vector3 splitPosition;
	bool extrudeKeyDown;
	bool doExtrudeUpdate;
	bool draggingCurve;

	#region Scene GUI

	const float doubleClickInterval = 0.5f;
	float clickTime = 0;

//	Tool previousTool = Tool.None;

	internal void SwitchEditionMode ()
	{
		editing = !editing;
// 		if ( editing )
// 		{
// 			previousTool = Tools.current;
// 			Tools.current = Tool.None;
// 		}
// 		else
// 		{
// 			Tools.current = previousTool;
// 		}
	}

	internal void OnSceneGUI ( EditableCurvableGeometry geometry )
	{
		this.geometry = geometry;

		// 		switch ( e.type )
		// 		{
		// 			case EventType.Layout:
		// 			case EventType.Repaint:
		// 			case EventType.MouseMove:
		// 				break;
		// 			default:
		// 				Debug.Log ( "e:" + e.type.ToString () );
		// 				break;
		// 		}

		bool doubleClick = false;
		if ( e.type == EventType.MouseDown && !e.control && !e.alt && !e.shift )
		{
			doubleClick = ( Time.realtimeSinceStartup - clickTime < doubleClickInterval );
			if ( doubleClick )
				clickTime = 0;
			else
				clickTime = Time.realtimeSinceStartup;
		}

		if ( geometry == null )
		{
			return;
		}

		if ( doubleClick )
		{
			SwitchEditionMode ();
			return;
		}

		if ( KeyPressed ( editKey ) || KeyPressed ( editKeyAlt ) )
			SwitchEditionMode ();

		if ( editing )
		{
			// 			if ( polyMesh != null )
			// 			{
			// 				Handles.BeginGUI ();
			// 				{
			// 					Vector3 screenPos = Camera.current.WorldToScreenPoint ( polyMesh.transform.position );
			// 					// GUI.Label ( new Rect ( screenPos.x, Screen.height - screenPos.y, 100, 20 ), "This is a label" );
			// 					EditorGUI.ColorField ( new Rect ( screenPos.x, Screen.height - screenPos.y, 100, 20 ), Color.white );
			// 				}
			// 				Handles.EndGUI ();
			// 			}

			//Update lists
			if ( keyPoints == null )
			{
				keyPoints = new List<Vector3> ( geometry.KeyPoints );
				curvePoints = new List<Vector3> ( geometry.CurvePoints );
				isCurve = new List<bool> ( geometry.IsCurve );
			}

			//Crazy hack to register undo
			if ( undoCallback == null )
			{
				undoCallback = typeof ( EditorApplication ).GetField ( "undoRedoPerformed", BindingFlags.NonPublic | BindingFlags.Static );
				if ( undoCallback != null )
					undoCallback.SetValue ( null, new EditorApplication.CallbackFunction ( OnUndoRedo ) );
			}

			//Load handle matrix
			// 			Matrix4x4 cachedMatrix = Handles.matrix;
			Handles.matrix = geometry.Transform.localToWorldMatrix;

			//Draw points and lines
			DrawAxis ();
			Handles.color = Color.white;
			for ( int i = 0; i < keyPoints.Count; i++ )
			{
				Handles.color = nearestLine == i ? Color.green : Color.white;

				DrawSegment ( i );
				if ( selectedIndices.Contains ( i ) )
				{
					Handles.color = Color.green;
					DrawCircle ( keyPoints[i], 0.08f );
				}
				else
					Handles.color = Color.white;
				DrawKeyPoint ( i );
				if ( isCurve[i] )
				{
					Handles.color = ( draggingCurve && dragIndex == i ) ? Color.white : Color.blue;
					DrawCurvePoint ( i );
				}
			}

			//			Handles.matrix = cachedMatrix;

			//Quit on tool change
			if ( e.type == EventType.KeyDown )
			{
				switch ( e.keyCode )
				{
					case KeyCode.Q:
					case KeyCode.W:
					case KeyCode.E:
					case KeyCode.R:
						return;
				}
			}

			//Quit if panning or no camera exists
			if ( Tools.current == Tool.View || ( e.isMouse && e.button > 0 ) || Camera.current == null || e.type == EventType.ScrollWheel )
			{
				return;
			}

			//Quit if laying out
			if ( e.type == EventType.Layout )
			{
				HandleUtility.AddDefaultControl ( GUIUtility.GetControlID ( FocusType.Passive ) );

				return;
			}

			//Cursor rectangle
			EditorGUIUtility.AddCursorRect ( new Rect ( 0, 0, Camera.current.pixelWidth, Camera.current.pixelHeight ), mouseCursor );
			mouseCursor = MouseCursor.Arrow;

			//Extrude key state
			if ( e.keyCode == extrudeKey )
			{
				if ( extrudeKeyDown )
				{
					if ( e.type == EventType.KeyUp )
						extrudeKeyDown = false;
				}
				else if ( e.type == EventType.KeyDown )
					extrudeKeyDown = true;
			}

			//Update matrices and snap
			worldToLocal = geometry.Transform.worldToLocalMatrix;
			inverseRotation = Quaternion.Inverse ( geometry.Transform.rotation ) * Camera.current.transform.rotation;
			snap = gridSnap;

			//Update mouse position
			screenMousePosition = new Vector3 ( e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y );
			var plane = new Plane ( -geometry.Transform.forward, geometry.Transform.position );
			var ray = Camera.current.ScreenPointToRay ( screenMousePosition );
			float hit;
			if ( plane.Raycast ( ray, out hit ) )
				mousePosition = worldToLocal.MultiplyPoint ( ray.GetPoint ( hit ) );
			else
			{
				return;
			}

			//Update nearest line and split position
			nearestLine = NearestLine ( out splitPosition );

			//Update the state and repaint
			var newState = UpdateState ();

			if ( state != newState )
				SetState ( newState );

			HandleUtility.Repaint ();

			switch ( e.type )
			{
				case EventType.Layout:
				case EventType.Repaint:
					break;
				default:
					e.Use ();
					break;
			}
		}
	}

	#endregion

	#region State Control

	//Initialize state
	void SetState ( State newState )
	{
		state = newState;
		switch ( state )
		{
			case State.Hover:
				break;
		}
	}

	//Update state
	State UpdateState ()
	{
		switch ( state )
		{
			//Hovering
			case State.Hover:

				// 			if (TryHoverCurvePoint(out dragIndex) && TryDragCurvePoint(dragIndex))
				// 				return State.Drag;
				if ( TryHoverKeyPoint ( out dragIndex ) )
				{
					if ( TryDeleteKeyPoint ( dragIndex ) )
						return State.Hover;

					if ( TrySwitchKeyPointSelection ( dragIndex ) )
						return State.Hover;

 					if ( TryDragKeyPoint ( dragIndex ) )
 						return State.Drag;
				}

				DrawNearestLineAndSplit ();

				if ( ( ( Tools.current == Tool.Move ) || ( Tools.current == Tool.Rect ) ) && TryExtrude () )
					return State.Extrude;
				if ( ( ( Tools.current == Tool.Move ) || ( Tools.current == Tool.Rect ) ) && TryDragSelected () )
					return State.DragSelected;
				if ( Tools.current == Tool.Rotate && TryRotateSelected () )
					return State.RotateSelected;
				if ( Tools.current == Tool.Scale && TryScaleSelected () )
					return State.ScaleSelected;

				if ( TryMoveKeyPoint () )
					return State.Hover;

				if ( TrySelectAll () )
					return State.Hover;

				if ( TrySplitLine () )
					// return State.Hover;
					return State.Drag;

				if ( TryDeleteSelected () )
					return State.Hover;

				if ( TryBoxSelect () )
					return State.BoxSelect;

				break;

			//Dragging
			case State.Drag:
				{
					mouseCursor = MouseCursor.MoveArrow;
					DrawCircle ( keyPoints[dragIndex], clickRadius );
					// 			if (draggingCurve)
					// 				MoveCurvePoint(dragIndex, mousePosition - clickPosition);
					// 			else

					Vector3 dragAmount = mousePosition - clickPosition;
					if ( Event.current.shift )
					{
						if ( Mathf.Abs ( dragAmount.x ) > Mathf.Abs ( dragAmount.y ) )
							dragAmount = new Vector3 ( dragAmount.x, 0, 0 );
						else
							dragAmount = new Vector3 ( 0, dragAmount.y, 0 );
					}

					MoveKeyPoint ( dragIndex, dragAmount );
					if ( TryStopDrag () )
						return State.Hover;
				}
				break;

			//Box Selecting
			case State.BoxSelect:
				if ( TryBoxSelectEnd () )
					return State.Hover;
				break;

			//Dragging selected
			case State.DragSelected:
				{
					mouseCursor = MouseCursor.MoveArrow;

					Vector3 dragAmount = mousePosition - clickPosition;
					if ( Event.current.shift )
					{
						if ( Mathf.Abs ( dragAmount.x ) > Mathf.Abs ( dragAmount.y ) )
							dragAmount = new Vector3 ( dragAmount.x, 0, 0 );
						else
							dragAmount = new Vector3 ( 0, dragAmount.y, 0 );
					}

					MoveSelected ( dragAmount );
					if ( TryStopDrag () )
						return State.Hover;
				}
				break;

			//Rotating selected
			case State.RotateSelected:
				mouseCursor = MouseCursor.RotateArrow;
				RotateSelected ();
				if ( TryStopDrag () )
					return State.Hover;
				break;

			//Scaling selected
			case State.ScaleSelected:
				mouseCursor = MouseCursor.ScaleArrow;
				ScaleSelected ();
				if ( TryStopDrag () )
					return State.Hover;
				break;

			//Extruding
			case State.Extrude:
				{
					mouseCursor = MouseCursor.MoveArrow;

					Vector3 dragAmount = mousePosition - clickPosition;
					if ( Event.current.shift )
					{
						if ( Mathf.Abs ( dragAmount.x ) > Mathf.Abs ( dragAmount.y ) )
							dragAmount = new Vector3 ( dragAmount.x, 0, 0 );
						else
							dragAmount = new Vector3 ( 0, dragAmount.y, 0 );
					}

					MoveSelected ( dragAmount );
					if ( doExtrudeUpdate && mousePosition != clickPosition )
					{
						UpdatePoly ( false, false );
						doExtrudeUpdate = false;
					}
					if ( TryStopDrag () )
						return State.Hover;
				}
				break;
		}
		return state;
	}

	void NotifyGeometryChange ()
	{
		geometry.NotifyGeometryChange ();
		Resources.UnloadUnusedAssets ();
	}

	void RecordUndo ()
	{
		Undo.RecordObject ( geometry as Object, "Geometry Changed" );
	}

	//Update the mesh on undo/redo
	void OnUndoRedo ()
	{
		keyPoints = new List<Vector3> ( geometry.KeyPoints );
		curvePoints = new List<Vector3> ( geometry.CurvePoints );
		isCurve = new List<bool> ( geometry.IsCurve );

		NotifyGeometryChange ();
	}

	void LoadPoly ()
	{
		for ( int i = 0; i < keyPoints.Count; i++ )
		{
			keyPoints[i] = geometry.KeyPoints[i];
			curvePoints[i] = geometry.CurvePoints[i];
			isCurve[i] = geometry.IsCurve[i];
		}
	}

	void TransformPoly ( Matrix4x4 matrix )
	{
		for ( int i = 0; i < keyPoints.Count; i++ )
		{
			keyPoints[i] = matrix.MultiplyPoint ( geometry.KeyPoints[i] );
			curvePoints[i] = matrix.MultiplyPoint ( geometry.CurvePoints[i] );
		}
	}

	void UpdatePoly ( bool sizeChanged, bool recordUndo )
	{
		if ( recordUndo )
			RecordUndo ();

		if ( sizeChanged )
		{
			geometry.KeyPoints = new List<Vector3> ( keyPoints );
			geometry.CurvePoints = new List<Vector3> ( curvePoints );
			geometry.IsCurve = new List<bool> ( isCurve );
		}
		else
		{
			for ( int i = 0; i < keyPoints.Count; i++ )
			{
				geometry.KeyPoints[i] = keyPoints[i];
				geometry.CurvePoints[i] = curvePoints[i];
				geometry.IsCurve[i] = isCurve[i];
			}
		}
		for ( int i = 0; i < keyPoints.Count; i++ )
			if ( !isCurve[i] )
				geometry.CurvePoints[i] = curvePoints[i] = Vector3.Lerp ( keyPoints[i], keyPoints[( i + 1 ) % keyPoints.Count], 0.5f );

		NotifyGeometryChange ();
	}

	void MoveKeyPoint ( int index, Vector3 amount )
	{
		var moveCurve = selectedIndices.Contains ( ( index + 1 ) % keyPoints.Count );
		if ( doSnap )
		{
			if ( globalSnap )
			{
				keyPoints[index] = Snap ( geometry.KeyPoints[index] + amount );
				if ( moveCurve )
					curvePoints[index] = Snap ( geometry.CurvePoints[index] + amount );
			}
			else
			{
				amount = Snap ( amount );
				keyPoints[index] = geometry.KeyPoints[index] + amount;
				if ( moveCurve )
					curvePoints[index] = geometry.CurvePoints[index] + amount;
			}
		}
		else
		{
			keyPoints[index] = geometry.KeyPoints[index] + amount;
			if ( moveCurve )
				curvePoints[index] = geometry.CurvePoints[index] + amount;
		}
	}

	void MoveCurvePoint ( int index, Vector3 amount )
	{
		isCurve[index] = true;
		if ( doSnap )
		{
			if ( globalSnap )
				curvePoints[index] = Snap ( geometry.CurvePoints[index] + amount );
			else
				curvePoints[index] = geometry.CurvePoints[index] + amount;
		}
		else
			curvePoints[index] = geometry.CurvePoints[index] + amount;
	}


	void MoveSelected ( Vector3 amount )
	{
		foreach ( var i in selectedIndices )
			MoveKeyPoint ( i, amount );
	}

	void RotateSelected ()
	{
		var center = GetSelectionCenter ();

		Handles.color = Color.white;
		Handles.DrawLine ( center, clickPosition );
		Handles.color = Color.green;
		Handles.DrawLine ( center, mousePosition );

		var clickOffset = clickPosition - center;
		var mouseOffset = mousePosition - center;
		var clickAngle = Mathf.Atan2 ( clickOffset.y, clickOffset.x );
		var mouseAngle = Mathf.Atan2 ( mouseOffset.y, mouseOffset.x );
		var angleOffset = mouseAngle - clickAngle;

		foreach ( var i in selectedIndices )
		{
			var point = geometry.KeyPoints[i];
			var pointOffset = point - center;
			var a = Mathf.Atan2 ( pointOffset.y, pointOffset.x ) + angleOffset;
			var d = pointOffset.magnitude;
			keyPoints[i] = center + new Vector3 ( Mathf.Cos ( a ) * d, Mathf.Sin ( a ) * d );
		}
	}

	void ScaleSelected ()
	{
		Handles.color = Color.green;
		Handles.DrawLine ( clickPosition, mousePosition );

		var center = GetSelectionCenter ();
		var scale = mousePosition - clickPosition;

		//Uniform scaling if shift pressed
		if ( e.shift )
		{
			if ( Mathf.Abs ( scale.x ) > Mathf.Abs ( scale.y ) )
				scale.y = scale.x;
			else
				scale.x = scale.y;
		}

		//Determine direction of scaling
		if ( scale.x < 0 )
			scale.x = 1 / ( -scale.x + 1 );
		else
			scale.x = 1 + scale.x;
		if ( scale.y < 0 )
			scale.y = 1 / ( -scale.y + 1 );
		else
			scale.y = 1 + scale.y;

		foreach ( var i in selectedIndices )
		{
			var point = geometry.KeyPoints[i];
			var offset = point - center;
			offset.x *= scale.x;
			offset.y *= scale.y;
			keyPoints[i] = center + offset;
		}
	}

	#endregion

	#region Drawing

	void DrawAxis ()
	{
		Handles.color = Color.red;
		var size = HandleUtility.GetHandleSize ( Vector3.zero ) * 0.1f;
		Handles.DrawLine ( new Vector3 ( -size, 0 ), new Vector3 ( size, 0 ) );
		Handles.DrawLine ( new Vector3 ( 0, -size ), new Vector2 ( 0, size ) );
	}

	void DrawKeyPoint ( int index, float size = 0.03f )
	{
		float pixelSize = HandleUtility.GetHandleSize ( Vector3.zero ) / 160;

		Color color = Handles.color;
		Handles.color = Color.black;
		Handles.DotCap ( 0, keyPoints[index], Quaternion.identity, pixelSize * 7 );

		Handles.color = color;
		// Handles.DotCap ( 0, keyPoints[index], Quaternion.identity, HandleUtility.GetHandleSize ( keyPoints[index] ) * size );
		Handles.DotCap ( 0, keyPoints[index], Quaternion.identity, pixelSize * 5 );
	}

	void DrawCurvePoint ( int index )
	{
		Handles.DotCap ( 0, curvePoints[index], Quaternion.identity, HandleUtility.GetHandleSize ( keyPoints[index] ) * 0.03f );
	}

	void DrawSegment ( int index )
	{
		var from = keyPoints[index];
		var to = keyPoints[( index + 1 ) % keyPoints.Count];
		if ( isCurve[index] )
		{
			var control = Bezier.Control ( from, to, curvePoints[index] );
			var count = Mathf.Ceil ( 1 / geometry.CurveDetail );
			for ( int i = 0; i < count; i++ )
				Handles.DrawLine ( Bezier.Curve ( from, control, to, i / count ), Bezier.Curve ( from, control, to, ( i + 1 ) / count ) );
		}
		else
			Handles.DrawLine ( from, to );
	}

	void DrawCircle ( Vector3 position, float size )
	{
// 		float pixelSize = HandleUtility.GetHandleSize ( Vector3.zero ) / 160;
// 
// 		Color color = Handles.color;
// 		Handles.color = Color.black;
// 		Handles.CircleCap ( 0, position, inverseRotation, 27 * pixelSize );
// 		Handles.CircleCap ( 0, position, inverseRotation, 23 * pixelSize );
// 
// 		Handles.color = color;
// 		Handles.CircleCap ( 0, position, inverseRotation, 25 * pixelSize );
		Handles.CircleCap ( 0, position, inverseRotation, HandleUtility.GetHandleSize ( position ) * size );
	}

	void DrawNearestLineAndSplit ()
	{
		if ( nearestLine >= 0 )
		{
			Handles.color = Color.green;
			if ( extrudeKeyDown )
				Handles.color = Color.red;

			DrawSegment ( nearestLine );

			bool createPointKeyDown = Event.current.alt;
			if ( createPointKeyDown && dragIndex == -1 )
			{
				Handles.color = Color.cyan;
				Handles.DotCap ( 0, splitPosition, Quaternion.identity, HandleUtility.GetHandleSize ( splitPosition ) * 0.05f );
			}
		}
	}

	#endregion

	#region State Checking

	bool TryHoverKeyPoint ( out int index )
	{
		if ( TryHover ( keyPoints, Color.white, out index ) )
		{
			mouseCursor = MouseCursor.MoveArrow;
			return true;
		}
		return false;
	}

	bool TryHoverCurvePoint ( out int index )
	{
		if ( TryHover ( curvePoints, Color.white, out index ) )
		{
			mouseCursor = MouseCursor.MoveArrow;
			return true;
		}
		return false;
	}

	bool TryDragKeyPoint ( int index )
	{
		if ( ! selectedIndices.Contains ( index ) )
			if ( TryDrag ( keyPoints, index ) )
			{
				draggingCurve = false;
				return true;
			}
		return false;
	}

	bool TryDragCurvePoint ( int index )
	{
		if ( TryDrag ( curvePoints, index ) )
		{
			draggingCurve = true;
			return true;
		}
		return false;
	}

	bool TryHover ( List<Vector3> points, Color color, out int index )
	{
		if ( ( Tools.current == Tool.Move ) || ( Tools.current == Tool.Rect ) )
		{
			index = NearestPoint ( points );
			if ( index >= 0 && IsHovering ( points[index] ) )
			{
				Handles.color = color;
				DrawCircle ( points[index], clickRadius );

				bool altDown = Event.current.alt;
				Handles.color = altDown ? Color.red : Color.cyan;
				DrawKeyPoint ( index, 0.05f );
				return true;
			}
		}
		index = -1;
		return false;
	}

	bool TryDrag ( List<Vector3> points, int index )
	{
		if ( e.type == EventType.MouseDown && IsHovering ( points[index] ) )
		{
			clickPosition = mousePosition;
			return true;
		}
		return false;
	}

	bool TryStopDrag ()
	{
		if ( e.type == EventType.MouseUp )
		{
			dragIndex = -1;
			UpdatePoly ( false, state != State.Extrude );
			return true;
		}
		return false;
	}

	bool TryBoxSelect ()
	{
		if ( e.type == EventType.MouseDown )
		{
			clickPosition = mousePosition;
			return true;
		}
		return false;
	}

	bool TryBoxSelectEnd ()
	{
		var min = new Vector3 ( Mathf.Min ( clickPosition.x, mousePosition.x ), Mathf.Min ( clickPosition.y, mousePosition.y ) );
		var max = new Vector3 ( Mathf.Max ( clickPosition.x, mousePosition.x ), Mathf.Max ( clickPosition.y, mousePosition.y ) );
		Handles.color = Color.white;
		Handles.DrawLine ( new Vector3 ( min.x, min.y ), new Vector3 ( max.x, min.y ) );
		Handles.DrawLine ( new Vector3 ( min.x, max.y ), new Vector3 ( max.x, max.y ) );
		Handles.DrawLine ( new Vector3 ( min.x, min.y ), new Vector3 ( min.x, max.y ) );
		Handles.DrawLine ( new Vector3 ( max.x, min.y ), new Vector3 ( max.x, max.y ) );

		if ( e.type == EventType.MouseUp )
		{
			var rect = new Rect ( min.x, min.y, max.x - min.x, max.y - min.y );

			if ( !control )
				selectedIndices.Clear ();
			for ( int i = 0; i < keyPoints.Count; i++ )
				if ( rect.Contains ( keyPoints[i] ) )
					selectedIndices.Add ( i );

			return true;
		}
		return false;
	}

	bool TryDragSelected ()
	{
		if ( selectedIndices.Count > 0 && ( dragIndex != -1 ) && ( e.type == EventType.MouseDown ) ) // && TryDragButton ( GetSelectionCenter (), 0.2f ) )
		{
			clickPosition = mousePosition;
			return true;
		}
		return false;
	}

	bool TryRotateSelected ()
	{
		if ( selectedIndices.Count > 0 && TryRotateButton ( GetSelectionCenter (), 0.3f ) )
		{
			clickPosition = mousePosition;
			return true;
		}
		return false;
	}

	bool TryScaleSelected ()
	{
		if ( selectedIndices.Count > 0 && TryScaleButton ( GetSelectionCenter (), 0.3f ) )
		{
			clickPosition = mousePosition;
			return true;
		}
		return false;
	}

// 	bool TryDragButton ( Vector3 position, float size )
// 	{
// 		size *= HandleUtility.GetHandleSize ( position );
// 		if ( Vector3.Distance ( mousePosition, position ) < size )
// 		{
// 			if ( e.type == EventType.MouseDown )
// 				return true;
// 			else
// 			{
// 				mouseCursor = MouseCursor.MoveArrow;
// 				Handles.color = Color.green;
// 			}
// 		}
// 		else
// 			Handles.color = Color.white;
// 		var buffer = size / 2;
// 		Handles.DrawLine ( new Vector3 ( position.x - buffer, position.y ), new Vector3 ( position.x + buffer, position.y ) );
// 		Handles.DrawLine ( new Vector3 ( position.x, position.y - buffer ), new Vector3 ( position.x, position.y + buffer ) );
// 		Handles.RectangleCap ( 0, position, Quaternion.identity, size );
// 		return false;
// 	}

	bool TryRotateButton ( Vector3 position, float size )
	{
		size *= HandleUtility.GetHandleSize ( position );
		var dist = Vector3.Distance ( mousePosition, position );
		var buffer = size / 4;
		if ( dist < size + buffer && dist > size - buffer )
		{
			if ( e.type == EventType.MouseDown )
				return true;
			else
			{
				mouseCursor = MouseCursor.RotateArrow;
				Handles.color = Color.green;
			}
		}
		else
			Handles.color = Color.white;
		Handles.CircleCap ( 0, position, inverseRotation, size - buffer / 2 );
		Handles.CircleCap ( 0, position, inverseRotation, size + buffer / 2 );
		return false;
	}

	bool TryScaleButton ( Vector3 position, float size )
	{
		size *= HandleUtility.GetHandleSize ( position );
		if ( Vector3.Distance ( mousePosition, position ) < size )
		{
			if ( e.type == EventType.MouseDown )
				return true;
			else
			{
				mouseCursor = MouseCursor.ScaleArrow;
				Handles.color = Color.green;
			}
		}
		else
			Handles.color = Color.white;
		var buffer = size / 4;
		Handles.DrawLine ( new Vector3 ( position.x - size - buffer, position.y ), new Vector3 ( position.x - size + buffer, position.y ) );
		Handles.DrawLine ( new Vector3 ( position.x + size - buffer, position.y ), new Vector3 ( position.x + size + buffer, position.y ) );
		Handles.DrawLine ( new Vector3 ( position.x, position.y - size - buffer ), new Vector3 ( position.x, position.y - size + buffer ) );
		Handles.DrawLine ( new Vector3 ( position.x, position.y + size - buffer ), new Vector3 ( position.x, position.y + size + buffer ) );
		Handles.RectangleCap ( 0, position, Quaternion.identity, size );
		return false;
	}

	bool TryMoveKeyPoint ()
	{
		bool moved = false;

		if ( KeyPressed ( KeyCode.LeftArrow ) )
		{
			foreach ( var i in selectedIndices )
				MoveKeyPoint ( i, new Vector3 ( -snap, 0, 0 ) );
			moved = true;
		}

		if ( KeyPressed ( KeyCode.RightArrow ) )
		{
			foreach ( var i in selectedIndices )
				MoveKeyPoint ( i, new Vector3 ( snap, 0, 0 ) );
			moved = true;
		}

		if ( KeyPressed ( KeyCode.UpArrow ) )
		{
			foreach ( var i in selectedIndices )
				MoveKeyPoint ( i, new Vector3 ( 0, snap, 0 ) );
			moved = true;
		}

		if ( KeyPressed ( KeyCode.DownArrow ) )
		{
			foreach ( var i in selectedIndices )
				MoveKeyPoint ( i, new Vector3 ( 0, -snap, 0 ) );
			moved = true;
		}

		if ( moved )
			UpdatePoly ( false, false );

		return moved;
	}

	bool TrySelectAll ()
	{
		if ( KeyPressed ( selectAllKey ) )
		{
			selectedIndices.Clear ();
			for ( int i = 0; i < keyPoints.Count; i++ )
				selectedIndices.Add ( i );
			return true;
		}
		return false;
	}

	bool TrySplitLine ()
	{
		bool createPointKeyDown = Event.current.alt;
		bool mouseDown = e.type == EventType.MouseDown;

		// if (nearestLine >= 0 && KeyPressed(splitKey))
		if ( ( nearestLine >= 0 ) && createPointKeyDown && mouseDown )
		{
			if ( nearestLine == keyPoints.Count - 1 )
			{
				keyPoints.Add ( splitPosition );
				curvePoints.Add ( Vector3.zero );
				isCurve.Add ( false );
			}
			else
			{
				keyPoints.Insert ( nearestLine + 1, splitPosition );
				curvePoints.Insert ( nearestLine + 1, Vector3.zero );
				isCurve.Insert ( nearestLine + 1, false );
			}
			isCurve[nearestLine] = false;
			UpdatePoly ( true, true );

			dragIndex = nearestLine + 1;

			clickPosition = mousePosition;

			return true;
		}
		return false;
	}

	bool TryExtrude ()
	{
		if ( nearestLine >= 0 && extrudeKeyDown && e.type == EventType.MouseDown )
		{
			var a = nearestLine;
			var b = ( nearestLine + 1 ) % keyPoints.Count;
			if ( b == 0 && a == keyPoints.Count - 1 )
			{
				//Extrude between the first and last points
				keyPoints.Add ( geometry.KeyPoints[a] );
				keyPoints.Add ( geometry.KeyPoints[b] );
				curvePoints.Add ( Vector3.zero );
				curvePoints.Add ( Vector3.zero );
				isCurve.Add ( false );
				isCurve.Add ( false );

				selectedIndices.Clear ();
				selectedIndices.Add ( keyPoints.Count - 2 );
				selectedIndices.Add ( keyPoints.Count - 1 );
			}
			else
			{
				//Extrude between two inner points
				var pointA = keyPoints[a];
				var pointB = keyPoints[b];
				keyPoints.Insert ( a + 1, pointA );
				keyPoints.Insert ( a + 2, pointB );
				curvePoints.Insert ( a + 1, Vector3.zero );
				curvePoints.Insert ( a + 2, Vector3.zero );
				isCurve.Insert ( a + 1, false );
				isCurve.Insert ( a + 2, false );

				selectedIndices.Clear ();
				selectedIndices.Add ( a + 1 );
				selectedIndices.Add ( a + 2 );
			}
			isCurve[nearestLine] = false;

			clickPosition = mousePosition;
			doExtrudeUpdate = true;
			UpdatePoly ( true, true );
			return true;
		}
		return false;
	}

	bool TrySwitchKeyPointSelection ( int index )
	{
		bool switchSelectionKeyDown = Event.current.shift;
		bool mouseDown = e.type == EventType.MouseDown;

		if ( switchSelectionKeyDown && mouseDown )
		{
			if ( selectedIndices.Contains ( index ) )
				selectedIndices.Remove ( index );
			else
				selectedIndices.Add ( index );

			return true;
		}
		else
			return false;
	}

	void DeleteKeyPoint ( int index )
	{
		keyPoints.RemoveAt ( index );
		curvePoints.RemoveAt ( index );
		isCurve.RemoveAt ( index );
		selectedIndices.Remove ( index );
		UpdatePoly ( true, true );
	}

	bool TryDeleteKeyPoint ( int index )
	{
		bool removePointKeyDown = Event.current.alt;
		bool mouseDown = e.type == EventType.MouseDown;

		if ( removePointKeyDown && mouseDown )
		{
			DeleteKeyPoint ( index );

			return true;
		}
		else
			return false;
	}

	bool TryDeleteSelected ()
	{
		if ( KeyPressed ( KeyCode.Backspace ) || KeyPressed ( KeyCode.Delete ) )
		{
			if ( selectedIndices.Count > 0 )
			{
				if ( keyPoints.Count - selectedIndices.Count >= 3 )
				{
					for ( int i = selectedIndices.Count - 1; i >= 0; i-- )
					{
						var index = selectedIndices[i];
						keyPoints.RemoveAt ( index );
						curvePoints.RemoveAt ( index );
						isCurve.RemoveAt ( index );
					}
					selectedIndices.Clear ();
					UpdatePoly ( true, true );
					return true;
				}
			}
			else if ( ( dragIndex >= 0 ) /* && IsHovering ( keyPoints[dragIndex] )*/ )
			{
				DeleteKeyPoint ( dragIndex );
				return true;
			}
			else if ( ( nearestLine >= 0 ) && IsHovering ( curvePoints[nearestLine] ) )
			{
				isCurve[nearestLine] = false;
				UpdatePoly ( false, true );
				return true;
			}
		}
		return false;
	}

	bool IsHovering ( Vector3 point )
	{
		return Vector3.Distance ( mousePosition, point ) < HandleUtility.GetHandleSize ( point ) * clickRadius;
	}

	int NearestPoint ( List<Vector3> points )
	{
		var near = -1;
		var nearDist = float.MaxValue;
		for ( int i = 0; i < points.Count; i++ )
		{
			var dist = Vector3.Distance ( points[i], mousePosition );
			if ( dist < nearDist )
			{
				nearDist = dist;
				near = i;
			}
		}
		return near;
	}

	int NearestLine ( out Vector3 position )
	{
		var near = -1;
		float nearDist = 50; // float.MaxValue;
		position = keyPoints[0];
		var linePos = Vector3.zero;
		for ( int i = 0; i < keyPoints.Count; i++ )
		{
			var j = ( i + 1 ) % keyPoints.Count;
			var line = keyPoints[j] - keyPoints[i];
			var offset = mousePosition - keyPoints[i];
			var dot = Vector3.Dot ( line.normalized, offset );
			if ( dot >= 0 && dot <= line.magnitude )
			{
				if ( isCurve[i] )
					linePos = Bezier.Curve ( keyPoints[i], Bezier.Control ( keyPoints[i], keyPoints[j], curvePoints[i] ), keyPoints[j], dot / line.magnitude );
				else
					linePos = keyPoints[i] + line.normalized * dot;
				var dist = Vector3.Distance ( linePos, mousePosition );
				if ( dist < nearDist )
				{
					nearDist = dist;
					position = linePos;
					near = i;
				}
			}
		}
		return near;
	}

	bool KeyPressed ( KeyCode key )
	{
		return e.type == EventType.KeyDown && e.keyCode == key;
	}

	bool KeyReleased ( KeyCode key )
	{
		return e.type == EventType.KeyUp && e.keyCode == key;
	}

	Vector3 Snap ( Vector3 value )
	{
		value.x = Mathf.Round ( value.x / snap ) * snap;
		value.y = Mathf.Round ( value.y / snap ) * snap;
		return value;
	}

	Vector3 GetSelectionCenter ()
	{
		var center = Vector3.zero;
		foreach ( var i in selectedIndices )
			center += geometry.KeyPoints[i];
		return center / selectedIndices.Count;
	}

	#endregion

	#region Properties

	EditableCurvableGeometry geometry
	{
		get;
		set;
	}

// 	PolyMesh polyMesh
// 	{
// 		get { return (PolyMesh)target; }
// 	}

	Event e
	{
		get { return Event.current; }
	}

	bool control
	{
		get { return Application.platform == RuntimePlatform.OSXEditor ? e.command : e.control; }
	}

	bool doSnap
	{
		get { return autoSnap ? !control : control; }
	}

	static bool meshSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_meshSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_meshSettings", value ); }
	}

	static bool colliderSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_colliderSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_colliderSettings", value ); }
	}

	static bool uvSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_uvSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_uvSettings", value ); }
	}

	static bool editorSettings
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_editorSettings", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_editorSettings", value ); }
	}

	static bool autoSnap
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_autoSnap", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_autoSnap", value ); }
	}

	static bool globalSnap
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_globalSnap", false ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_globalSnap", value ); }
	}

	static float gridSnap
	{
		get { return EditorPrefs.GetFloat ( "PolyMeshEditor_gridSnap", 1 ); }
		set { EditorPrefs.SetFloat ( "PolyMeshEditor_gridSnap", value ); }
	}

	static bool hideWireframe
	{
		get { return EditorPrefs.GetBool ( "PolyMeshEditor_hideWireframe", true ); }
		set { EditorPrefs.SetBool ( "PolyMeshEditor_hideWireframe", value ); }
	}

	public KeyCode editKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_editKey", (int)KeyCode.Tab ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_editKey", (int)value ); }
	}

	public KeyCode editKeyAlt
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_editKeyAlt", (int)KeyCode.F2 ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_editKeyAlt", (int)value ); }
	}

	public KeyCode selectAllKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_selectAllKey", (int)KeyCode.A ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_selectAllKey", (int)value ); }
	}

	public KeyCode splitKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_splitKey", (int)KeyCode.S ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_splitKey", (int)value ); }
	}

	public KeyCode extrudeKey
	{
		get { return (KeyCode)EditorPrefs.GetInt ( "PolyMeshEditor_extrudeKey", (int)KeyCode.D ); }
		set { EditorPrefs.SetInt ( "PolyMeshEditor_extrudeKey", (int)value ); }
	}

	#endregion

	#region Menu Items

	[MenuItem ( "GameObject/Create Other/PolyMesh", false, 1000 )]
	static void Create ()
	{
		GameObject go = new GameObject ( "PolyMesh", typeof ( MeshFilter ), typeof ( MeshRenderer ) );
		PolyMesh polyMesh = go.AddComponent<PolyMesh> ();
		CreateSquare ( polyMesh, 10 );

		MeshRenderer renderer = go.GetOrCreateComponent<MeshRenderer> ();
		Shader shader = Shader.Find ( "Sprites/Default" );
		if ( shader != null )
		{
			Material material = new Material ( shader );
			material.name = "PolyMesh Material";
			material.SetColor ( "_Color", Color.gray );
			// material.hideFlags = HideFlags.DontSave;
			renderer.material = material;
		}

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}

	static void CreateSquare ( PolyMesh polyMesh, float size )
	{
		polyMesh.keyPoints.AddRange ( new Vector3[] { new Vector3 ( size, size ), new Vector3 ( size, -size ), new Vector3 ( -size, -size ), new Vector3 ( -size, size ) } );
		polyMesh.curvePoints.AddRange ( new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero } );
		polyMesh.isCurve.AddRange ( new bool[] { false, false, false, false } );

		polyMesh.BuildMesh ();
		Resources.UnloadUnusedAssets ();
	}

	#endregion
}