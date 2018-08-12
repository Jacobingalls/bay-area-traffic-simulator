using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdateableData {
    [Header("Terrain Manager State")]
    public int width = 10;

	public int height = 10;

	[Range(0.1f, 2.0f)]
	public float yScale = 1.0f / 1.2f;

	[Range(1, 20)]
	public int quantizationRange = 6;

	public Texture2D heightmap;
	public Material terrainMaterial;
	public Material roadMaterial;

    float savedMin;
    float savedMax;

    [Header("Terrain Shader State")]
    public Texture2D roadMap;

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