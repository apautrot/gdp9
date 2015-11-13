using UnityEngine;
using System.Collections;

// http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector

[ExecuteInEditMode]
public class TextMeshEx : MonoBehaviour
{
	[SerializeField, TextArea]
	private string _Text = "";

	public string Text
	{
		get { return _Text; }
		set
		{
			_Text = value;
			textRenderer.Text = value;
		}
	}


	[SerializeField]
	private float _CharacterSize = 1.0f;

	public float CharacterSize
	{
		get { return _CharacterSize; }
		set
		{
			_CharacterSize = value;
			textRenderer.Scale = value;
		}
	}


	[SerializeField]
	private float _LineMaxWidth = 0.0f;

	public float LineMaxWidth
	{
		get { return _LineMaxWidth; }
		set
		{
			_LineMaxWidth = value;
			textRenderer.LineMaxWidth = value > 0 ? value : float.MaxValue;
		}
	}


	[SerializeField]
	private TextAnchor _Anchor;

	public TextAnchor Anchor
	{
		get { return _Anchor; }
		set
		{
			_Anchor = value;
			textRenderer.Anchor = value;
		}
	}


	[SerializeField]
	private TextAlignment _Alignment;

	public TextAlignment Alignement
	{
		get { return _Alignment; }
		set
		{
			_Alignment = value;
			textRenderer.Alignment = value;
		}
	}


	[SerializeField]
	private Color _Color = UnityEngine.Color.white;

	public Color Color
	{
		get { return _Color; }
		set
		{
			_Color = value;
			textRenderer.Color = value;
		}
	}


	[SerializeField]
	private Font _Font;

	public Font Font
	{
		get { return _Font; }
		set
		{
			_Font = value;
			textRenderer.Font = value;
		}
	}

	[SerializeField]
	private Material _CustomMaterial;

	public Material CustomMaterial
	{
		get { return _CustomMaterial; }
		set
		{
			_CustomMaterial = value;
			// textRenderer.CustomMaterial = value;
		}
	}

	// public float OffsetZ;
	// public float LineSpacing = 1;
	// public bool RichText;

	private TextRenderer textRenderer = new TextRenderer ();
	public TextRenderer TextRenderer
	{
		get { return textRenderer; }
	}

	public float Height
	{
		get
		{
			return textRenderer.Bounds.size.y;
		}
	}

	void Awake ()
	{
		ForceRecreate ();
	}

	void Restart ()
	{
		ForceRecreate ();
	}

	void OnValidate ()
	{
		ForceRecreate ();
	}

	public void ForceRecreate ()
	{
		textRenderer.Text = Text;
		textRenderer.Scale = CharacterSize;
		textRenderer.Alignment = Alignement;
		textRenderer.Anchor = Anchor;
		textRenderer.Color = Color;
		textRenderer.Font = Font;
		textRenderer.LineMaxWidth = LineMaxWidth > 0 ? LineMaxWidth : float.MaxValue;

		UpdateGeometry ();
	}

	public void UpdateGeometry ()
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();

		if ( Font != null )
		{
			if ( Application.isEditor && !Application.isPlaying )
			{
				filter.sharedMesh = textRenderer.Mesh;
				renderer.sharedMaterial = _CustomMaterial != null ? _CustomMaterial : Font.material;
			}
			else
			{
				filter.mesh = textRenderer.Mesh;

 				if ( renderer.sharedMaterial == null )
					renderer.sharedMaterial = _CustomMaterial != null ? _CustomMaterial : Font.material;
			}
		}
		else
			filter.mesh = null;
	}

	private void OnWillRenderObject ()
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
			return;
#endif
		if ( textRenderer.IsDirty () )
			UpdateGeometry ();
	}

}