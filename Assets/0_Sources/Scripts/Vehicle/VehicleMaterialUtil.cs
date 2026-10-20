using UnityEngine;
using UnityEngine.Rendering;

public static class VehicleMaterialUtil
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int SurfaceId = Shader.PropertyToID("_Surface");
    private static readonly int BlendId = Shader.PropertyToID("_Blend");
    private static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");

    public static Color GetColor(Material material)
    {
        if (material.HasProperty(BaseColorId))
            return material.GetColor(BaseColorId);
        if (material.HasProperty(ColorId))
            return material.GetColor(ColorId);
        return Color.white;
    }

    public static void ApplyColor(Material material, Color color)
    {
        if (material.HasProperty(BaseColorId))
            material.SetColor(BaseColorId, color);
        if (material.HasProperty(ColorId))
            material.SetColor(ColorId, color);
    }

    public static void ApplyTint(Material material, Color original, float darken, float alpha)
    {
        Color color = original;
        color.r *= darken;
        color.g *= darken;
        color.b *= darken;
        color.a = alpha;
        ApplyColor(material, color);
    }

    public static void MakeTransparent(Material material)
    {
        if (material.HasProperty(SurfaceId))
            material.SetFloat(SurfaceId, 1f);
        if (material.HasProperty(BlendId))
            material.SetFloat(BlendId, 0f);
        if (material.HasProperty(SrcBlendId))
            material.SetFloat(SrcBlendId, (float)BlendMode.SrcAlpha);
        if (material.HasProperty(DstBlendId))
            material.SetFloat(DstBlendId, (float)BlendMode.OneMinusSrcAlpha);
        if (material.HasProperty(ZWriteId))
            material.SetFloat(ZWriteId, 0f);

        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.SetOverrideTag("RenderType", "Transparent");
        material.renderQueue = 3000;
    }
}
