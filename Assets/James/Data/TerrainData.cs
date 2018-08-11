using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdateableData {
    float savedMin;
    float savedMax;

    public Color[] baseColors;

    [Range(0, 1)]
    public float[] baseStartHeights;

    [Range(0, 1)]
    public float[] baseBlendValues;

    public void ApplyToMaterial(Material mat)
    {
        mat.SetColorArray("baseColors", baseColors);
        mat.SetFloatArray("baseStartHeights", baseStartHeights);
        mat.SetFloatArray("baseBlendValues", baseBlendValues);
        mat.SetInt("baseColorCount", baseColors.Length);

        UpdateMeshHeights(mat, savedMin, savedMax);
    }

    public void UpdateMeshHeights(Material mat, float minHeight, float maxHeight)
    {
        savedMin = minHeight;
        savedMax = maxHeight;

        mat.SetFloat("minHeight", minHeight);
        mat.SetFloat("maxHeight", maxHeight);
    }

}