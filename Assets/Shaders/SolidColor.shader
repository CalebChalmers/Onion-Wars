Shader "Unlit/Solid Color"
{
	Properties
	{
		_Color ("Solid Color", Color) = (0,0,0,0)
	}
	Category
	{

		Tags{ "Queue" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Subshader
		{
			Color [_Color]
			Pass{}
		}
	}
}