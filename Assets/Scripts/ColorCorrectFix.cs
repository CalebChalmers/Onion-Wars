using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class ColorCorrectFix : MonoBehaviour
{
    public Texture2D lookupTexture;
    
    void Start()
    {
        ColorCorrectionLookup ccl = gameObject.GetComponent<ColorCorrectionLookup>();
        ccl.Convert(lookupTexture, "");
    }
}