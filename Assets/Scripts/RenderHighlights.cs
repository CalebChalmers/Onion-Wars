using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RenderHighlights : PostEffectsBase
{
    public Camera highlightCam;

    private Shader shader;
    private Material material = null;

    new void Start()
    {
    }

    public override bool CheckResources()
    {
        return isSupported;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }
        if (material == null)
        {
            shader = Shader.Find("Unlit/Transparent");
            material = CheckShaderAndCreateMaterial(shader, material);
        }
            
        Graphics.Blit(highlightCam.targetTexture, source, material);
        Graphics.Blit(source, destination);
    }
}