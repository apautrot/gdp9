using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour
{
	[RangeAttribute(0,1)]
	public float Life;

	public Color FullLife;
	public Color NoLife;

	ColorHSL hslFullLife;
	ColorHSL hslNoLife;

	SpriteRenderer spriteRenderer;
	CutOutParameter cutOutParameter;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		cutOutParameter = GetComponent<CutOutParameter> ();
		ColorUtil.rgbToHsl ( FullLife, ref hslFullLife );
		ColorUtil.rgbToHsl ( NoLife, ref hslNoLife );
	}

	void OnValidate()
	{
		Awake ();
		UpdateColor ();
    }

	void UpdateColor()
	{
        ColorHSL hslColor = new ColorHSL();
		hslColor.h = Mathf.Lerp ( hslNoLife.h, hslFullLife.h, Life );
		hslColor.l = Mathf.Lerp ( hslNoLife.l, hslFullLife.l, Life );
		hslColor.s = Mathf.Lerp ( hslNoLife.s, hslFullLife.s, Life );
		// 		hslColor.l = hslFullLife.l;
		// 		hslColor.s = hslFullLife.s;
		Color color = Color.white;
		ColorUtil.hslToRgb ( hslColor, ref color );
		spriteRenderer.color = color;
		cutOutParameter.CutOut = Life;
	}
}
