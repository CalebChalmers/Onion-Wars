using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelectCamera : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        //cam.SetReplacementShader(Shader.Find("Custom/Empty"), null);
        cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
    }

	void LateUpdate ()
    {
        cam.fieldOfView = Camera.main.fieldOfView;
	}
}
